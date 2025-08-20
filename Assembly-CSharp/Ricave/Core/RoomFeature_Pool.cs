using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Pool : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            return this.TryGeneratePool(room, memory) != null;
        }

        protected CellCuboid? TryGeneratePool(Room room, WorldGenMemory memory)
        {
            if (room.RoomFeaturesGenerated.Contains(Get.RoomFeature_WaterPool) || room.RoomFeaturesGenerated.Contains(Get.RoomFeature_ToxicWaterPool) || room.RoomFeaturesGenerated.Contains(Get.RoomFeature_PoolFilledWithBridges) || room.RoomFeaturesGenerated.Contains(Get.RoomFeature_LavaPool))
            {
                return null;
            }
            if (room.RoomBelow != null)
            {
                return null;
            }
            if (room.StartY <= 0)
            {
                return null;
            }
            World world = Get.World;
            CellCuboid cellCuboid;
            if (!BiggestRectFinder.TryFindRectOfMinMaxSize(room.Shape.InnerCuboid(1).BottomSurfaceCuboid, (Vector3Int x) => !world.AnyEntityAt(x) && world.CellsInfo.AnyPermanentFilledImpassableAt(x.Below()) && !world.AnyEntityAt(x.Below().Below()) && !room.IsEntranceCellToAvoidBlockingOnlyFromBelow(x), 4, 5, out cellCuboid))
            {
                return null;
            }
            CellCuboid poolArea = cellCuboid.InnerCuboidXZ(1);
            List<Vector3Int> bridgePositions = new List<Vector3Int>();
            EntitySpec bridgeSpec;
            if (this.waterSpec != Get.Entity_Bridge && this.waterSpec != Get.Entity_Bars)
            {
                if (this.waterSpec == Get.Entity_Lava)
                {
                    bridgeSpec = Get.Entity_Bars;
                }
                else
                {
                    bridgeSpec = Get.Entity_Bridge;
                }
            }
            else
            {
                bridgeSpec = null;
            }
            foreach (Vector3Int vector3Int in poolArea)
            {
                this.DoPoolWaterAt(vector3Int.Below());
            }
            int num = Rand.RangeInclusive(2, 6);
            Func<Vector3Int, bool> <> 9__6;
            Func<Vector3Int, bool> <> 9__5;
            for (int i = 0; i < num; i++)
            {
                IEnumerable<Vector3Int> edgeCellsXZ = poolArea.OuterCuboidXZ(1).EdgeCellsXZ;
                Func<Vector3Int, bool> func;
                if ((func = <> 9__5) == null)
                {
                    func = (<> 9__5 = delegate (Vector3Int x)
                    {
                        if (!world.AnyEntityOfSpecAt(x.Below(), this.waterSpec))
                        {
                            IEnumerable<Vector3Int> enumerable = x.Below().AdjacentCardinalCellsXZ();
                            Func<Vector3Int, bool> func3;
                            if ((func3 = <> 9__6) == null)
                            {
                                func3 = (<> 9__6 = (Vector3Int y) => world.CellsInfo.AnyPermanentFilledImpassableAt(y) || world.AnyEntityOfSpecAt(y, this.waterSpec));
                            }
                            if (enumerable.All<Vector3Int>(func3))
                            {
                                return world.AnyCardinalAdjacentEntityOfSpecAt(x.Below(), this.waterSpec);
                            }
                        }
                        return false;
                    });
                }
                Vector3Int vector3Int2;
                if (!edgeCellsXZ.Where<Vector3Int>(func).TryGetRandomElement<Vector3Int>(out vector3Int2))
                {
                    break;
                }
                this.DoPoolWaterAt(vector3Int2.Below());
                if (bridgeSpec != null)
                {
                    Maker.Make(bridgeSpec, null, false, false, true).Spawn(vector3Int2.Below());
                    bridgePositions.Add(vector3Int2.Below());
                }
            }
            if (bridgeSpec != null)
            {
                Func<Vector3Int, bool> <> 9__7;
                for (int j = 0; j < 2; j++)
                {
                    IEnumerable<Vector3Int> edgeCellsXZ2 = poolArea.EdgeCellsXZ;
                    Func<Vector3Int, bool> func2;
                    if ((func2 = <> 9__7) == null)
                    {
                        func2 = (<> 9__7 = (Vector3Int x) => !world.AnyEntityOfSpecAt(x.Below(), bridgeSpec));
                    }
                    Vector3Int vector3Int3;
                    if (!edgeCellsXZ2.Where<Vector3Int>(func2).TryGetRandomElement<Vector3Int>(out vector3Int3))
                    {
                        break;
                    }
                    Maker.Make(bridgeSpec, null, false, false, true).Spawn(vector3Int3.Below());
                    bridgePositions.Add(vector3Int3.Below());
                }
            }
            Func<Vector3Int, bool> <> 9__9;
            Vector3Int vector3Int4;
            Vector3Int vector3Int5;
            if (this.pipeWaterfallStraightSpec != null && room.RoomAbove == null && Rand.Chance(0.4f) && (from x in poolArea
                                                                                                          where bridgeSpec == null || !world.AnyEntityOfSpecAt(x.Below(), bridgeSpec)
                                                                                                          select x.WithY(room.Shape.yMax) into x
                                                                                                          where world.AnyEntityOfSpecAt(x, Get.Entity_Floor)
                                                                                                          select x).TryGetRandomElement<Vector3Int>(out vector3Int4))
            {
                world.GetFirstEntityOfSpecAt(vector3Int4, Get.Entity_Floor).DeSpawn(false);
                Maker.Make(Get.Entity_CeilingWithPipe, null, false, false, true).Spawn(vector3Int4);
                Maker.Make(this.pipeWaterfallStraightSpec, null, false, false, true).Spawn(vector3Int4);
            }
            else if (this.pipeWaterfallSpec != null && room.Height >= 4 && poolArea.OuterCuboidXZ(1).EdgeCellsXZNoCorners.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (!world.AnyEntityAt(x) && world.CellsInfo.AnyPermanentFilledImpassableAt(x.Below()) && !room.IsEntranceCellToAvoidBlocking(x, true))
                {
                    IEnumerable<Vector3Int> enumerable2 = x.Below().AdjacentCardinalCellsXZ();
                    Func<Vector3Int, bool> func4;
                    if ((func4 = <> 9__9) == null)
                    {
                        func4 = (<> 9__9 = (Vector3Int y) => bridgePositions.Contains(y));
                    }
                    return !enumerable2.Any<Vector3Int>(func4);
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int5))
            {
                Vector3Int vector3Int6 = -poolArea.OuterCuboidXZ(1).GetEdge(vector3Int5);
                Maker.Make(Get.Entity_WallWithPipe, null, false, false, true).Spawn(vector3Int5, vector3Int6);
                Maker.Make(this.pipeWaterfallSpec, null, false, false, true).Spawn(vector3Int5, vector3Int6);
                if (!world.CellsInfo.CanPassThroughNoActors(vector3Int5 - vector3Int6) || world.CellsInfo.AnyAIAvoidsAt(vector3Int5 - vector3Int6) || !world.CellsInfo.IsFloorUnderNoActors(vector3Int5 - vector3Int6))
                {
                    Vector3Int vector3Int7 = vector3Int6.RightDir();
                    Vector3Int vector3Int8 = -vector3Int7;
                    if (world.CellsInfo.CanPassThroughNoActors(vector3Int5 + vector3Int7) && world.CellsInfo.CanPassThroughNoActors(vector3Int5 + vector3Int7 + Vector3IntUtility.Up) && world.CellsInfo.CanPassThroughNoActors(vector3Int5 + Vector3IntUtility.Up))
                    {
                        Maker.Make(Get.Entity_Ladder, null, false, false, true).Spawn(vector3Int5 + vector3Int7, vector3Int7);
                    }
                    if (world.CellsInfo.CanPassThroughNoActors(vector3Int5 + vector3Int8) && world.CellsInfo.CanPassThroughNoActors(vector3Int5 + vector3Int8 + Vector3IntUtility.Up) && world.CellsInfo.CanPassThroughNoActors(vector3Int5 + Vector3IntUtility.Up))
                    {
                        Maker.Make(Get.Entity_Ladder, null, false, false, true).Spawn(vector3Int5 + vector3Int8, vector3Int8);
                    }
                }
            }
            Vector3Int vector3Int9;
            if (poolArea.EdgeCellsXZ.Where<Vector3Int>((Vector3Int x) => world.CellsInfo.CanPassThroughNoActors(x.Below()) && world.CellsInfo.CanPassThroughNoActors(x + poolArea.GetEdge(x))).TryGetRandomElement<Vector3Int>(out vector3Int9) || poolArea.EdgeCellsXZ.TryGetRandomElement<Vector3Int>(out vector3Int9))
            {
                Maker.Make(Get.Entity_Ladder, null, false, false, true).Spawn(vector3Int9.Below());
            }
            return new CellCuboid?(poolArea);
        }

        private void DoPoolWaterAt(Vector3Int c)
        {
            World world = Get.World;
            foreach (Entity entity in world.GetEntitiesAt(c).ToList<Entity>())
            {
                entity.DeSpawn(false);
            }
            Maker.Make(this.waterSpec, null, false, false, true).Spawn(c);
            Vector3Int vector3Int = c.Below();
            if (!world.AnyEntityAt(vector3Int))
            {
                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int);
            }
            foreach (Vector3Int vector3Int2 in Vector3IntUtility.DirectionsXZ)
            {
                Vector3Int vector3Int3 = (c + vector3Int2).Below();
                if (vector3Int3.InBounds() && !world.AnyEntityAt(vector3Int3))
                {
                    Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                }
            }
        }

        [Saved]
        private EntitySpec waterSpec;

        [Saved]
        private EntitySpec pipeWaterfallSpec;

        [Saved]
        private EntitySpec pipeWaterfallStraightSpec;
    }
}