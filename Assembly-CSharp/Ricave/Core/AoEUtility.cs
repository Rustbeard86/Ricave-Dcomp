using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class AoEUtility
    {
        public static void GetAoEArea(Vector3Int center, UseEffect useEffect, IUsable usable, List<Vector3Int> outResult, Predicate<Vector3Int> optimizationPredicateBeforeLOSCheck = null)
        {
            outResult.Clear();
            Vector3Int? vector3Int;
            if (useEffect.AoELineOnly)
            {
                Entity entity = usable as Entity;
                if (entity == null)
                {
                    return;
                }
                vector3Int = new Vector3Int?(entity.DirectionCardinal);
            }
            else
            {
                vector3Int = null;
            }
            AoEUtility.tmpCustomAoEArea.Clear();
            if (useEffect.HasCustomAoEArea)
            {
                useEffect.GetCustomAoEArea(AoEUtility.tmpCustomAoEArea);
            }
            AoEUtility.GetAoEArea(center, useEffect.AoERadius, useEffect.AoEXZOnly, vector3Int, useEffect.AoELineStopOnImpassable, useEffect.AoEExcludeCenter, AoEUtility.tmpCustomAoEArea, outResult, optimizationPredicateBeforeLOSCheck);
        }

        public static void GetAoEArea(Vector3Int center, int? radius, bool xzOnly, Vector3Int? lineOnlyDir, bool lineStopOnImpassable, bool excludeCenter, List<Vector3Int> customArea, List<Vector3Int> outResult, Predicate<Vector3Int> optimizationPredicateBeforeLOSCheck = null)
        {
            outResult.Clear();
            if (radius == null && customArea.Count == 0)
            {
                return;
            }
            if (customArea.Count != 0)
            {
                outResult.AddRange(customArea);
                return;
            }
            if (lineOnlyDir != null)
            {
                for (int i = (excludeCenter ? 1 : 0); i <= radius.Value; i++)
                {
                    Vector3Int vector3Int = center + lineOnlyDir.Value * i;
                    if (!vector3Int.InBounds())
                    {
                        return;
                    }
                    outResult.Add(vector3Int);
                    if (i != 0 && (!Get.CellsInfo.CanProjectilesPassThrough(vector3Int) || (lineStopOnImpassable && !Get.CellsInfo.CanPassThroughNoActors(vector3Int))))
                    {
                        return;
                    }
                }
                return;
            }
            if (xzOnly)
            {
                CellCuboid cellCuboid = new CellCuboid(center, radius.Value);
                cellCuboid.y = center.y;
                cellCuboid.height = 1;
                cellCuboid.ClipToWorld();
                if (!excludeCenter)
                {
                    outResult.Add(center);
                }
                using (CellCuboid.Enumerator enumerator = cellCuboid.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Vector3Int vector3Int2 = enumerator.Current;
                        if (!(vector3Int2 == center) && (optimizationPredicateBeforeLOSCheck == null || optimizationPredicateBeforeLOSCheck(vector3Int2)) && LineOfSight.IsLineOfFire(center, vector3Int2))
                        {
                            outResult.Add(vector3Int2);
                        }
                    }
                    return;
                }
            }
            CellCuboid cellsWithin = center.GetCellsWithin(radius.Value);
            cellsWithin.ClipToWorld();
            if (!excludeCenter)
            {
                outResult.Add(center);
            }
            for (int j = cellsWithin.yMax; j >= cellsWithin.yMin; j--)
            {
                CellCuboid cellCuboid2 = cellsWithin;
                cellCuboid2.y = j;
                cellCuboid2.height = 1;
                foreach (Vector3Int vector3Int3 in cellCuboid2)
                {
                    if (!(vector3Int3 == center) && (optimizationPredicateBeforeLOSCheck == null || optimizationPredicateBeforeLOSCheck(vector3Int3)) && LineOfSight.IsLineOfFire(center, vector3Int3))
                    {
                        outResult.Add(vector3Int3);
                    }
                }
            }
        }

        public static void GetAoETargets(Vector3Int center, UseEffect useEffect, IUsable usable, Actor user, List<Target> outTargets)
        {
            outTargets.Clear();
            if ((useEffect.AoERadius == null && !useEffect.HasCustomAoEArea) || useEffect.AoEHandledManually)
            {
                return;
            }
            World world = Get.World;
            TargetFilter useFilterAoE = usable.UseFilterAoE;
            AoEUtility.tmpAoEArea.Clear();
            AoEUtility.AoETargetsOptimizationPredicate_useFilterAoE = useFilterAoE;
            AoEUtility.AoETargetsOptimizationPredicate_user = user;
            AoEUtility.GetAoEArea(center, useEffect, usable, AoEUtility.tmpAoEArea, AoEUtility.AoETargetsOptimizationPredicateDelegate);
            AoEUtility.AoETargetsOptimizationPredicate_useFilterAoE = null;
            AoEUtility.AoETargetsOptimizationPredicate_user = null;
            foreach (Vector3Int vector3Int in AoEUtility.tmpAoEArea)
            {
                foreach (Entity entity in world.GetEntitiesAt(vector3Int))
                {
                    if (useFilterAoE.Allows(entity, user))
                    {
                        outTargets.Add(entity);
                    }
                }
                if (useFilterAoE.Allows(vector3Int, user))
                {
                    outTargets.Add(vector3Int);
                }
            }
        }

        private static bool AoETargetsOptimizationPredicate(Vector3Int c)
        {
            TargetFilter aoETargetsOptimizationPredicate_useFilterAoE = AoEUtility.AoETargetsOptimizationPredicate_useFilterAoE;
            Actor aoETargetsOptimizationPredicate_user = AoEUtility.AoETargetsOptimizationPredicate_user;
            if (aoETargetsOptimizationPredicate_useFilterAoE.Allows(c, aoETargetsOptimizationPredicate_user))
            {
                return true;
            }
            foreach (Entity entity in Get.World.GetEntitiesAt(c))
            {
                if (aoETargetsOptimizationPredicate_useFilterAoE.Allows(entity, aoETargetsOptimizationPredicate_user))
                {
                    return true;
                }
            }
            return false;
        }

        private static List<Vector3Int> tmpCustomAoEArea = new List<Vector3Int>(30);

        private static List<Vector3Int> tmpAoEArea = new List<Vector3Int>(30);

        private static readonly Predicate<Vector3Int> AoETargetsOptimizationPredicateDelegate = new Predicate<Vector3Int>(AoEUtility.AoETargetsOptimizationPredicate);

        private static TargetFilter AoETargetsOptimizationPredicate_useFilterAoE;

        private static Actor AoETargetsOptimizationPredicate_user;
    }
}