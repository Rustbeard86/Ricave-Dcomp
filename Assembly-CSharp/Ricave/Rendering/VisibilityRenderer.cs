using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ricave.Rendering
{
    public class VisibilityRenderer
    {
        public VisibilityRenderer(World world, VisibilityCache visibilityCache)
        {
            this.world = world;
            this.visibilityCache = visibilityCache;
            Vector3Int size = world.Size;
            this.data = new VisibilityRenderer.FogData[size.x, size.y, size.z];
            this.RebuildAll();
        }

        public void RebuildAll()
        {
            Vector3Int size = this.world.Size;
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    for (int k = 0; k < size.z; k++)
                    {
                        this.UpdateAt(new Vector3Int(i, j, k));
                    }
                }
            }
        }

        public void OnFogChanged(List<Vector3Int> unfoggedOrFogged)
        {
            this.tmpToUpdate.Clear();
            for (int i = 0; i < unfoggedOrFogged.Count; i++)
            {
                for (int j = 0; j < Vector3IntUtility.DirectionsCardinalAndInside.Length; j++)
                {
                    Vector3Int vector3Int = unfoggedOrFogged[i] + Vector3IntUtility.DirectionsCardinalAndInside[j];
                    if (this.world.InBounds(vector3Int) && this.tmpToUpdate.Add(vector3Int))
                    {
                        this.UpdateAt(vector3Int);
                        this.slowlyUpdateCardinalOf.Add(vector3Int);
                    }
                }
            }
        }

        public void OnVisibilityChanged(List<Vector3Int> noLongerSeen, List<Vector3Int> newlySeen)
        {
            this.tmpToUpdate.Clear();
            for (int i = 0; i < noLongerSeen.Count; i++)
            {
                for (int j = 0; j < Vector3IntUtility.DirectionsCardinalAndInside.Length; j++)
                {
                    Vector3Int vector3Int = noLongerSeen[i] + Vector3IntUtility.DirectionsCardinalAndInside[j];
                    if (this.world.InBounds(vector3Int) && this.tmpToUpdate.Add(vector3Int))
                    {
                        this.UpdateAt(vector3Int);
                    }
                }
            }
            for (int k = 0; k < newlySeen.Count; k++)
            {
                for (int l = 0; l < Vector3IntUtility.DirectionsCardinalAndInside.Length; l++)
                {
                    Vector3Int vector3Int2 = newlySeen[k] + Vector3IntUtility.DirectionsCardinalAndInside[l];
                    if (this.world.InBounds(vector3Int2) && this.tmpToUpdate.Add(vector3Int2))
                    {
                        this.UpdateAt(vector3Int2);
                    }
                }
            }
        }

        public void OnEntityMoved(Entity entity, Vector3Int prev)
        {
            if (!this.CanEverAffectVisibilityRenderer(entity))
            {
                return;
            }
            this.ProcessEntityChangedAt(prev);
            this.ProcessEntityChangedAt(entity.Position);
        }

        public void OnEntitySpawned(Entity entity)
        {
            if (!this.CanEverAffectVisibilityRenderer(entity))
            {
                return;
            }
            this.ProcessEntityChangedAt(entity.Position);
        }

        public void OnEntityDeSpawned(Entity entity)
        {
            if (!this.CanEverAffectVisibilityRenderer(entity))
            {
                return;
            }
            this.ProcessEntityChangedAt(entity.Position);
        }

        private bool CanEverAffectVisibilityRenderer(Entity entity)
        {
            return !entity.Spec.CanSeeThrough || (entity is Structure && entity.Spec.Structure.IsFilled);
        }

        public void Update()
        {
            Profiler.Begin("Draw fog");
            try
            {
                Quaternion quaternion = Quaternion.Euler(0f, Clock.Time * 100f, 0f);
                int i = 0;
                int count = this.fog.Count;
                while (i < count)
                {
                    Graphics.DrawMesh(VisibilityRenderer.FogMesh, Matrix4x4.TRS(this.fog[i], quaternion, VisibilityRenderer.FogScale), VisibilityRenderer.FogMaterial, 0, null, 0, null, ShadowCastingMode.Off, false);
                    i++;
                }
                int j = 0;
                int count2 = this.fogLight.Count;
                while (j < count2)
                {
                    Graphics.DrawMesh(VisibilityRenderer.FogMesh, Matrix4x4.TRS(this.fogLight[j], quaternion, VisibilityRenderer.FogScale), VisibilityRenderer.FogLightMaterial, 0, null, 0, null, ShadowCastingMode.Off, false);
                    j++;
                }
            }
            finally
            {
                Profiler.End();
            }
            int num = 0;
            for (int k = this.slowlyUpdateCardinalOf.Count - 1; k >= 0; k--)
            {
                for (int l = 0; l < Vector3IntUtility.DirectionsCardinal.Length; l++)
                {
                    Vector3Int vector3Int = this.slowlyUpdateCardinalOf[k] + Vector3IntUtility.DirectionsCardinal[l];
                    if (this.world.InBounds(vector3Int))
                    {
                        this.UpdateAt(vector3Int);
                    }
                }
                this.slowlyUpdateCardinalOf.RemoveAt(k);
                num++;
                if (num >= 10)
                {
                    break;
                }
            }
        }

        private void ProcessEntityChangedAt(Vector3Int pos)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsCardinalAndInside.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsCardinalAndInside[i];
                if (this.world.InBounds(vector3Int))
                {
                    this.UpdateAt(vector3Int);
                }
            }
        }

        private void UpdateAt(Vector3Int pos)
        {
            VisibilityRenderer.FogData fogData = this.data[pos.x, pos.y, pos.z];
            FogOfWar fogOfWar = this.world.FogOfWar;
            CellsInfo cellsInfo = this.world.CellsInfo;
            if (fogOfWar.IsUnfogged(pos) || !this.AnyAdjacentCardinalUnfogged(pos))
            {
                if (fogData.hasFog)
                {
                    fogData.hasFog = false;
                    this.fog.RemoveLast(pos);
                }
                if (fogData.forwardExists)
                {
                    this.RemoveBorder_Forward(pos);
                    fogData.forwardExists = false;
                }
                if (fogData.backExists)
                {
                    this.RemoveBorder_Back(pos);
                    fogData.backExists = false;
                }
                if (fogData.leftExists)
                {
                    this.RemoveBorder_Left(pos);
                    fogData.leftExists = false;
                }
                if (fogData.rightExists)
                {
                    this.RemoveBorder_Right(pos);
                    fogData.rightExists = false;
                }
                if (fogData.upExists)
                {
                    this.RemoveBorder_Up(pos);
                    fogData.upExists = false;
                }
                if (fogData.downExists)
                {
                    this.RemoveBorder_Down(pos);
                    fogData.downExists = false;
                }
            }
            else
            {
                if (!fogData.hasFog && this.AnyAdjacentCardinalUnfoggedAndNotFilled(pos))
                {
                    fogData.hasFog = true;
                    this.fog.Add(pos);
                }
                bool flag = cellsInfo.AnyFilledCantSeeThroughAt(pos);
                for (int i = 0; i < Vector3IntUtility.DirectionsCardinal.Length; i++)
                {
                    Vector3Int vector3Int = Vector3IntUtility.DirectionsCardinal[i];
                    Vector3Int vector3Int2 = pos + vector3Int;
                    bool flag2 = !this.world.InBounds(vector3Int2) || (flag && (!cellsInfo.AnyFilledCantSeeThroughAt(vector3Int2) || !fogOfWar.IsFogged(vector3Int2))) || (fogOfWar.IsFogged(vector3Int2) && !this.AnyAdjacentCardinalUnfogged(vector3Int2));
                    if (vector3Int.z == 1)
                    {
                        if (flag2)
                        {
                            if (!fogData.forwardExists)
                            {
                                this.AddBorder_Forward(pos);
                                fogData.forwardExists = true;
                            }
                        }
                        else if (fogData.forwardExists)
                        {
                            this.RemoveBorder_Forward(pos);
                            fogData.forwardExists = false;
                        }
                    }
                    else if (vector3Int.z == -1)
                    {
                        if (flag2)
                        {
                            if (!fogData.backExists)
                            {
                                this.AddBorder_Back(pos);
                                fogData.backExists = true;
                            }
                        }
                        else if (fogData.backExists)
                        {
                            this.RemoveBorder_Back(pos);
                            fogData.backExists = false;
                        }
                    }
                    else if (vector3Int.x == 1)
                    {
                        if (flag2)
                        {
                            if (!fogData.rightExists)
                            {
                                this.AddBorder_Right(pos);
                                fogData.rightExists = true;
                            }
                        }
                        else if (fogData.rightExists)
                        {
                            this.RemoveBorder_Right(pos);
                            fogData.rightExists = false;
                        }
                    }
                    else if (vector3Int.x == -1)
                    {
                        if (flag2)
                        {
                            if (!fogData.leftExists)
                            {
                                this.AddBorder_Left(pos);
                                fogData.leftExists = true;
                            }
                        }
                        else if (fogData.leftExists)
                        {
                            this.RemoveBorder_Left(pos);
                            fogData.leftExists = false;
                        }
                    }
                    else if (vector3Int.y == 1)
                    {
                        if (flag2)
                        {
                            if (!fogData.upExists)
                            {
                                this.AddBorder_Up(pos);
                                fogData.upExists = true;
                            }
                        }
                        else if (fogData.upExists)
                        {
                            this.RemoveBorder_Up(pos);
                            fogData.upExists = false;
                        }
                    }
                    else if (vector3Int.y == -1)
                    {
                        if (flag2)
                        {
                            if (!fogData.downExists)
                            {
                                this.AddBorder_Down(pos);
                                fogData.downExists = true;
                            }
                        }
                        else if (fogData.downExists)
                        {
                            this.RemoveBorder_Down(pos);
                            fogData.downExists = false;
                        }
                    }
                }
            }
            if (this.visibilityCache.PlayerSees(pos) || fogOfWar.IsFogged(pos) || cellsInfo.IsFilled(pos) || !this.AnyAdjacentCardinalPlayerSees(pos))
            {
                if (fogData.hasFogLight)
                {
                    fogData.hasFogLight = false;
                    this.fogLight.RemoveLast(pos);
                }
            }
            else if (!fogData.hasFogLight)
            {
                fogData.hasFogLight = true;
                this.fogLight.Add(pos);
            }
            this.data[pos.x, pos.y, pos.z] = fogData;
        }

        private bool AnyAdjacentCardinalUnfogged(Vector3Int pos)
        {
            FogOfWar fogOfWar = this.world.FogOfWar;
            for (int i = 0; i < Vector3IntUtility.DirectionsCardinal.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsCardinal[i];
                if (this.world.InBounds(vector3Int) && fogOfWar.IsUnfogged(vector3Int))
                {
                    return true;
                }
            }
            return false;
        }

        private bool AnyAdjacentCardinalUnfoggedAndNotFilled(Vector3Int pos)
        {
            FogOfWar fogOfWar = this.world.FogOfWar;
            CellsInfo cellsInfo = this.world.CellsInfo;
            for (int i = 0; i < Vector3IntUtility.DirectionsCardinal.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsCardinal[i];
                if (this.world.InBounds(vector3Int) && fogOfWar.IsUnfogged(vector3Int) && !cellsInfo.AnyFilledCantSeeThroughAt(vector3Int))
                {
                    return true;
                }
            }
            return false;
        }

        private bool AnyAdjacentCardinalPlayerSees(Vector3Int pos)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsCardinal.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsCardinal[i];
                if (this.world.InBounds(vector3Int) && this.visibilityCache.PlayerSees(vector3Int))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddBorder_Forward(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Forward(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Add(vector, vector2, vector3, vector4);
        }

        private void AddBorder_Back(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Back(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Add(vector, vector2, vector3, vector4);
        }

        private void AddBorder_Left(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Left(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Add(vector, vector2, vector3, vector4);
        }

        private void AddBorder_Right(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Right(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Add(vector, vector2, vector3, vector4);
        }

        private void AddBorder_Up(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Up(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Add(vector, vector2, vector3, vector4);
        }

        private void AddBorder_Down(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Down(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Add(vector, vector2, vector3, vector4);
        }

        private void RemoveBorder_Forward(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Forward(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Remove(vector, vector2, vector3, vector4);
        }

        private void RemoveBorder_Back(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Back(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Remove(vector, vector2, vector3, vector4);
        }

        private void RemoveBorder_Left(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Left(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Remove(vector, vector2, vector3, vector4);
        }

        private void RemoveBorder_Right(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Right(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Remove(vector, vector2, vector3, vector4);
        }

        private void RemoveBorder_Up(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Up(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Remove(vector, vector2, vector3, vector4);
        }

        private void RemoveBorder_Down(Vector3Int pos)
        {
            SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(pos);
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            this.GetBorderPos_Down(pos, out vector, out vector2, out vector3, out vector4);
            sectionRenderer.CombinedFogOfWarMeshes.Remove(vector, vector2, vector3, vector4);
        }

        private void GetBorderPos_Forward(Vector3Int pos, out Vector3 v1, out Vector3 v2, out Vector3 v3, out Vector3 v4)
        {
            v1 = (v2 = (v3 = (v4 = pos)));
            v1.z += 0.499f;
            v2.z += 0.499f;
            v3.z += 0.499f;
            v4.z += 0.499f;
            v1.y += 0.5f;
            v1.x -= 0.5f;
            v2.y += 0.5f;
            v2.x += 0.5f;
            v3.y -= 0.5f;
            v3.x += 0.5f;
            v4.y -= 0.5f;
            v4.x -= 0.5f;
        }

        private void GetBorderPos_Back(Vector3Int pos, out Vector3 v1, out Vector3 v2, out Vector3 v3, out Vector3 v4)
        {
            v1 = (v2 = (v3 = (v4 = pos)));
            v1.z -= 0.499f;
            v2.z -= 0.499f;
            v3.z -= 0.499f;
            v4.z -= 0.499f;
            v1.y += 0.5f;
            v1.x += 0.5f;
            v2.y += 0.5f;
            v2.x -= 0.5f;
            v3.y -= 0.5f;
            v3.x -= 0.5f;
            v4.y -= 0.5f;
            v4.x += 0.5f;
        }

        private void GetBorderPos_Left(Vector3Int pos, out Vector3 v1, out Vector3 v2, out Vector3 v3, out Vector3 v4)
        {
            v1 = (v2 = (v3 = (v4 = pos)));
            v1.x -= 0.499f;
            v2.x -= 0.499f;
            v3.x -= 0.499f;
            v4.x -= 0.499f;
            v1.y += 0.5f;
            v1.z -= 0.5f;
            v2.y += 0.5f;
            v2.z += 0.5f;
            v3.y -= 0.5f;
            v3.z += 0.5f;
            v4.y -= 0.5f;
            v4.z -= 0.5f;
        }

        private void GetBorderPos_Right(Vector3Int pos, out Vector3 v1, out Vector3 v2, out Vector3 v3, out Vector3 v4)
        {
            v1 = (v2 = (v3 = (v4 = pos)));
            v1.x += 0.499f;
            v2.x += 0.499f;
            v3.x += 0.499f;
            v4.x += 0.499f;
            v1.y += 0.5f;
            v1.z += 0.5f;
            v2.y += 0.5f;
            v2.z -= 0.5f;
            v3.y -= 0.5f;
            v3.z -= 0.5f;
            v4.y -= 0.5f;
            v4.z += 0.5f;
        }

        private void GetBorderPos_Up(Vector3Int pos, out Vector3 v1, out Vector3 v2, out Vector3 v3, out Vector3 v4)
        {
            v1 = (v2 = (v3 = (v4 = pos)));
            v1.y += 0.499f;
            v2.y += 0.499f;
            v3.y += 0.499f;
            v4.y += 0.499f;
            v1.z += 0.5f;
            v1.x += 0.5f;
            v2.z += 0.5f;
            v2.x -= 0.5f;
            v3.z -= 0.5f;
            v3.x -= 0.5f;
            v4.z -= 0.5f;
            v4.x += 0.5f;
        }

        private void GetBorderPos_Down(Vector3Int pos, out Vector3 v1, out Vector3 v2, out Vector3 v3, out Vector3 v4)
        {
            v1 = (v2 = (v3 = (v4 = pos)));
            v1.y -= 0.499f;
            v2.y -= 0.499f;
            v3.y -= 0.499f;
            v4.y -= 0.499f;
            v1.z += 0.5f;
            v1.x -= 0.5f;
            v2.z += 0.5f;
            v2.x += 0.5f;
            v3.z -= 0.5f;
            v3.x += 0.5f;
            v4.z -= 0.5f;
            v4.x -= 0.5f;
        }

        private World world;

        private VisibilityRenderer.FogData[,,] data;

        private List<Vector3Int> fog = new List<Vector3Int>(200);

        private List<Vector3Int> fogLight = new List<Vector3Int>(200);

        private VisibilityCache visibilityCache;

        private HashSet<Vector3Int> tmpToUpdate = new HashSet<Vector3Int>();

        private List<Vector3Int> slowlyUpdateCardinalOf = new List<Vector3Int>();

        private static readonly Mesh FogMesh = Assets.Get<Mesh>("Models/Fog");

        private static readonly Material FogMaterial = Assets.Get<Material>("Materials/FogOfWar/FogOfWar");

        private static readonly Material FogLightMaterial = Assets.Get<Material>("Materials/FogOfWar/FogOfWarLight");

        private static readonly Vector3 FogScale = new Vector3(0.5f, 0.4f, 0.5f) * 0.9f;

        private const float FogAnimationSpeed = 100f;

        private const float DepthEps = 0.001f;

        private const float SizeEps = 0f;

        private const int SlowlyUpdateCardinalOfRate = 10;

        private struct FogData
        {
            public bool hasFog;

            public bool hasFogLight;

            public bool leftExists;

            public bool rightExists;

            public bool forwardExists;

            public bool backExists;

            public bool upExists;

            public bool downExists;
        }
    }
}