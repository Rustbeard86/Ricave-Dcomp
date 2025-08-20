using System;
using System.Collections.Generic;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class TiledDecals : ISaveableEventsReceiver
    {
        public bool TemporarilyIgnoreEntityChanges
        {
            get
            {
                return this.temporarilyIgnoreEntityChanges;
            }
            set
            {
                if (this.temporarilyIgnoreEntityChanges == value)
                {
                    return;
                }
                this.temporarilyIgnoreEntityChanges = value;
                if (!this.temporarilyIgnoreEntityChanges)
                {
                    this.RecalculateAll();
                }
            }
        }

        protected TiledDecals()
        {
        }

        public TiledDecals(World world)
        {
            this.world = world;
            this.specsThatDontHaveDataYet.AddRange(Get.Specs.GetAll<TiledDecalsSpec>());
        }

        public void Update()
        {
            foreach (ValueTuple<Entity, List<Renderer>> valueTuple in Get.GameObjectFader.FadingEntities)
            {
                if (valueTuple.Item1 is Structure && valueTuple.Item1.Spec.Structure.IsFilled && valueTuple.Item2.AnyInCameraFrustum())
                {
                    this.RecalculateFilledEntityChangedAt(valueTuple.Item1.Position, true);
                }
            }
            if (this.anyNeedUpdateMesh)
            {
                for (int i = 0; i < this.data.Count; i++)
                {
                    TiledDecals.TiledDecalsData tiledDecalsData = this.data[i];
                    foreach (Vector3Int vector3Int in tiledDecalsData.needUpdateMeshAtDown)
                    {
                        this.UpdateMeshAt(vector3Int, tiledDecalsData.spec, tiledDecalsData.presentDown, new Vector3Int(0, -1, 0), 1f);
                    }
                    tiledDecalsData.needUpdateMeshAtDown.Clear();
                    foreach (Vector3Int vector3Int2 in tiledDecalsData.needUpdateMeshAtUp)
                    {
                        this.UpdateMeshAt(vector3Int2, tiledDecalsData.spec, tiledDecalsData.presentUp, new Vector3Int(0, 1, 0), 1f);
                    }
                    tiledDecalsData.needUpdateMeshAtUp.Clear();
                    foreach (Vector3Int vector3Int3 in tiledDecalsData.needUpdateMeshAtLeft)
                    {
                        this.UpdateMeshAt(vector3Int3, tiledDecalsData.spec, tiledDecalsData.presentLeft, new Vector3Int(-1, 0, 0), 1f);
                    }
                    tiledDecalsData.needUpdateMeshAtLeft.Clear();
                    foreach (Vector3Int vector3Int4 in tiledDecalsData.needUpdateMeshAtRight)
                    {
                        this.UpdateMeshAt(vector3Int4, tiledDecalsData.spec, tiledDecalsData.presentRight, new Vector3Int(1, 0, 0), 1f);
                    }
                    tiledDecalsData.needUpdateMeshAtRight.Clear();
                    foreach (Vector3Int vector3Int5 in tiledDecalsData.needUpdateMeshAtForward)
                    {
                        this.UpdateMeshAt(vector3Int5, tiledDecalsData.spec, tiledDecalsData.presentForward, new Vector3Int(0, 0, 1), 1f);
                    }
                    tiledDecalsData.needUpdateMeshAtForward.Clear();
                    foreach (Vector3Int vector3Int6 in tiledDecalsData.needUpdateMeshAtBack)
                    {
                        this.UpdateMeshAt(vector3Int6, tiledDecalsData.spec, tiledDecalsData.presentBack, new Vector3Int(0, 0, -1), 1f);
                    }
                    tiledDecalsData.needUpdateMeshAtBack.Clear();
                }
                this.anyNeedUpdateMesh = false;
            }
        }

        public bool AnyForcedAt(Vector3Int pos)
        {
            for (int i = 0; i < this.forcedDecals.Count; i++)
            {
                if (this.forcedDecals[i].pos == pos)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetForced(TiledDecalsSpec spec, Vector3Int pos)
        {
            Instruction.ThrowIfNotExecuting();
            for (int i = 0; i < this.forcedDecals.Count; i++)
            {
                if (this.forcedDecals[i].spec == spec && this.forcedDecals[i].pos == pos)
                {
                    return;
                }
            }
            this.forcedDecals.Add(new TiledDecals.ForcedDecal
            {
                spec = spec,
                pos = pos
            });
            this.everForced.Add(spec);
            this.CheckAddData(spec);
            for (int j = 0; j < this.data.Count; j++)
            {
                if (this.data[j].spec == spec)
                {
                    this.data[j].forced[pos.x, pos.y, pos.z] = true;
                }
            }
        }

        public void OnEntityMoved(Entity entity, Vector3Int prev)
        {
            if (!this.CanEverAffectTiledDecals(entity))
            {
                return;
            }
            if (!this.temporarilyIgnoreEntityChanges)
            {
                this.ProcessEntityChangedAt(prev);
                this.ProcessEntityChangedAt(entity.Position);
                if (entity.Spec.IsStructure && entity.Spec.Structure.IsFilled)
                {
                    this.RecalculateFilledEntityChangedAt(prev, false);
                    this.RecalculateFilledEntityChangedAt(entity.Position, false);
                }
            }
        }

        public void OnEntitySpawned(Entity entity)
        {
            if (!this.CanEverAffectTiledDecals(entity))
            {
                return;
            }
            for (int i = this.specsThatDontHaveDataYet.Count - 1; i >= 0; i--)
            {
                this.CheckAddData(this.specsThatDontHaveDataYet[i]);
            }
            if (!this.temporarilyIgnoreEntityChanges)
            {
                this.ProcessEntityChangedAt(entity.Position);
                if (entity.Spec.IsStructure && entity.Spec.Structure.IsFilled)
                {
                    this.RecalculateFilledEntityChangedAt(entity.Position, false);
                }
            }
        }

        public void OnEntityDeSpawned(Entity entity)
        {
            if (!this.CanEverAffectTiledDecals(entity))
            {
                return;
            }
            if (!this.temporarilyIgnoreEntityChanges)
            {
                this.ProcessEntityChangedAt(entity.Position);
                if (entity.Spec.IsStructure && entity.Spec.Structure.IsFilled)
                {
                    this.RecalculateFilledEntityChangedAt(entity.Position, false);
                }
            }
        }

        public void OnEntityNoLongerFadingIn(Entity entity)
        {
            if (entity.Spec.IsStructure && entity.Spec.Structure.IsFilled)
            {
                this.RecalculateFilledEntityChangedAt(entity.Position, false);
            }
        }

        private void ProcessEntityChangedAt(Vector3Int pos)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsCardinalAndInside.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsCardinalAndInside[i];
                if (this.world.InBounds(vector3Int))
                {
                    this.RecalculateAt(vector3Int);
                }
            }
        }

        private void RecalculateFilledEntityChangedAt(Vector3Int pos, bool onlyIfPresent = false)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsCardinal.Length; i++)
            {
                Vector3Int vector3Int = Vector3IntUtility.DirectionsCardinal[i];
                Vector3Int vector3Int2 = pos + vector3Int;
                if (this.world.InBounds(vector3Int2))
                {
                    for (int j = 0; j < this.data.Count; j++)
                    {
                        TiledDecals.TiledDecalsData tiledDecalsData = this.data[j];
                        if (vector3Int == Vector3Int.up)
                        {
                            if (!onlyIfPresent || tiledDecalsData.presentDown[vector3Int2.x, vector3Int2.y, vector3Int2.z])
                            {
                                tiledDecalsData.needUpdateMeshAtDown.Add(vector3Int2);
                            }
                        }
                        else if (vector3Int == Vector3Int.down)
                        {
                            if (!onlyIfPresent || tiledDecalsData.presentUp[vector3Int2.x, vector3Int2.y, vector3Int2.z])
                            {
                                tiledDecalsData.needUpdateMeshAtUp.Add(vector3Int2);
                            }
                        }
                        else if (vector3Int == Vector3Int.right)
                        {
                            if (!onlyIfPresent || tiledDecalsData.presentLeft[vector3Int2.x, vector3Int2.y, vector3Int2.z])
                            {
                                tiledDecalsData.needUpdateMeshAtLeft.Add(vector3Int2);
                            }
                        }
                        else if (vector3Int == Vector3Int.left)
                        {
                            if (!onlyIfPresent || tiledDecalsData.presentRight[vector3Int2.x, vector3Int2.y, vector3Int2.z])
                            {
                                tiledDecalsData.needUpdateMeshAtRight.Add(vector3Int2);
                            }
                        }
                        else if (vector3Int == new Vector3Int(0, 0, -1))
                        {
                            if (!onlyIfPresent || tiledDecalsData.presentForward[vector3Int2.x, vector3Int2.y, vector3Int2.z])
                            {
                                tiledDecalsData.needUpdateMeshAtForward.Add(vector3Int2);
                            }
                        }
                        else if (vector3Int == new Vector3Int(0, 0, 1) && (!onlyIfPresent || tiledDecalsData.presentBack[vector3Int2.x, vector3Int2.y, vector3Int2.z]))
                        {
                            tiledDecalsData.needUpdateMeshAtBack.Add(vector3Int2);
                        }
                    }
                    this.anyNeedUpdateMesh = true;
                }
            }
        }

        public void RecalculateAll()
        {
            Profiler.Begin("TiledDecals.RecalculateAll()");
            try
            {
                foreach (Vector3Int vector3Int in Get.World.Bounds)
                {
                    this.RecalculateAt(vector3Int);
                }
            }
            finally
            {
                Profiler.End();
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
                        this.RecalculateAt(vector3Int);
                    }
                }
            }
        }

        private void RecalculateAt(Vector3Int pos)
        {
            Profiler.Begin("Recalculate tiled decals at cell");
            try
            {
                int i = 0;
                int count = this.data.Count;
                while (i < count)
                {
                    TiledDecals.TiledDecalsData tiledDecalsData = this.data[i];
                    bool flag = this.ShouldBeAt(tiledDecalsData.spec, tiledDecalsData.forced, pos, new Vector3Int(0, -1, 0));
                    if (flag != tiledDecalsData.presentDown[pos.x, pos.y, pos.z])
                    {
                        tiledDecalsData.presentDown[pos.x, pos.y, pos.z] = flag;
                        this.AddAdjacentToNeedUpdateMesh(pos, tiledDecalsData.needUpdateMeshAtDown);
                    }
                    bool flag2 = this.ShouldBeAt(tiledDecalsData.spec, tiledDecalsData.forced, pos, new Vector3Int(0, 1, 0));
                    if (flag2 != tiledDecalsData.presentUp[pos.x, pos.y, pos.z])
                    {
                        tiledDecalsData.presentUp[pos.x, pos.y, pos.z] = flag2;
                        this.AddAdjacentToNeedUpdateMesh(pos, tiledDecalsData.needUpdateMeshAtUp);
                    }
                    bool flag3 = this.ShouldBeAt(tiledDecalsData.spec, tiledDecalsData.forced, pos, new Vector3Int(-1, 0, 0));
                    if (flag3 != tiledDecalsData.presentLeft[pos.x, pos.y, pos.z])
                    {
                        tiledDecalsData.presentLeft[pos.x, pos.y, pos.z] = flag3;
                        this.AddAdjacentToNeedUpdateMesh(pos, tiledDecalsData.needUpdateMeshAtLeft);
                    }
                    bool flag4 = this.ShouldBeAt(tiledDecalsData.spec, tiledDecalsData.forced, pos, new Vector3Int(1, 0, 0));
                    if (flag4 != tiledDecalsData.presentRight[pos.x, pos.y, pos.z])
                    {
                        tiledDecalsData.presentRight[pos.x, pos.y, pos.z] = flag4;
                        this.AddAdjacentToNeedUpdateMesh(pos, tiledDecalsData.needUpdateMeshAtRight);
                    }
                    bool flag5 = this.ShouldBeAt(tiledDecalsData.spec, tiledDecalsData.forced, pos, new Vector3Int(0, 0, 1));
                    if (flag5 != tiledDecalsData.presentForward[pos.x, pos.y, pos.z])
                    {
                        tiledDecalsData.presentForward[pos.x, pos.y, pos.z] = flag5;
                        this.AddAdjacentToNeedUpdateMesh(pos, tiledDecalsData.needUpdateMeshAtForward);
                    }
                    bool flag6 = this.ShouldBeAt(tiledDecalsData.spec, tiledDecalsData.forced, pos, new Vector3Int(0, 0, -1));
                    if (flag6 != tiledDecalsData.presentBack[pos.x, pos.y, pos.z])
                    {
                        tiledDecalsData.presentBack[pos.x, pos.y, pos.z] = flag6;
                        this.AddAdjacentToNeedUpdateMesh(pos, tiledDecalsData.needUpdateMeshAtBack);
                    }
                    i++;
                }
            }
            finally
            {
                Profiler.End();
            }
        }

        private void AddAdjacentToNeedUpdateMesh(Vector3Int pos, HashSet<Vector3Int> needUpdate)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsAndInside[i];
                if (this.world.InBounds(vector3Int))
                {
                    needUpdate.Add(vector3Int);
                    this.anyNeedUpdateMesh = true;
                }
            }
        }

        private void UpdateMeshAt(Vector3Int pos, TiledDecalsSpec spec, bool[,,] present, Vector3Int dir, float alpha = 1f)
        {
            SectionRendererGOC sectionRenderer = this.world.SectionsManager.GetSectionRenderer(pos);
            if (!present[pos.x, pos.y, pos.z] || this.world.CellsInfo.IsFilled(pos))
            {
                sectionRenderer.CombinedTiledDecalsMeshes.TryRemove(pos, spec, dir);
                return;
            }
            Vector3 vector;
            Quaternion quaternion;
            Vector3 vector2;
            int num;
            bool flag;
            float filledEntitySmoothedPctAt = Get.GameObjectFader.GetFilledEntitySmoothedPctAt(pos + dir, out vector, out quaternion, out vector2, out num, out flag);
            if (filledEntitySmoothedPctAt <= 0f)
            {
                sectionRenderer.CombinedTiledDecalsMeshes.TryRemove(pos, spec, dir);
                return;
            }
            Vector3Int vector3Int;
            Vector3Int vector3Int2;
            this.GetLocalDirsFor(dir, out vector3Int, out vector3Int2);
            Vector3Int vector3Int3 = pos - vector3Int;
            Vector3Int vector3Int4 = pos + vector3Int;
            Vector3Int vector3Int5 = pos + vector3Int2;
            Vector3Int vector3Int6 = pos - vector3Int2;
            Vector3Int vector3Int7 = pos + vector3Int2 - vector3Int;
            Vector3Int vector3Int8 = pos + vector3Int2 + vector3Int;
            Vector3Int vector3Int9 = pos - vector3Int2 - vector3Int;
            Vector3Int vector3Int10 = pos - vector3Int2 + vector3Int;
            bool flag2 = this.world.InBounds(vector3Int3) && present[vector3Int3.x, vector3Int3.y, vector3Int3.z] && this.world.InBounds(vector3Int5) && present[vector3Int5.x, vector3Int5.y, vector3Int5.z] && this.world.InBounds(vector3Int7) && present[vector3Int7.x, vector3Int7.y, vector3Int7.z];
            bool flag3 = this.world.InBounds(vector3Int5) && present[vector3Int5.x, vector3Int5.y, vector3Int5.z];
            bool flag4 = this.world.InBounds(vector3Int4) && present[vector3Int4.x, vector3Int4.y, vector3Int4.z] && this.world.InBounds(vector3Int5) && present[vector3Int5.x, vector3Int5.y, vector3Int5.z] && this.world.InBounds(vector3Int8) && present[vector3Int8.x, vector3Int8.y, vector3Int8.z];
            bool flag5 = this.world.InBounds(vector3Int4) && present[vector3Int4.x, vector3Int4.y, vector3Int4.z];
            bool flag6 = this.world.InBounds(vector3Int4) && present[vector3Int4.x, vector3Int4.y, vector3Int4.z] && this.world.InBounds(vector3Int6) && present[vector3Int6.x, vector3Int6.y, vector3Int6.z] && this.world.InBounds(vector3Int10) && present[vector3Int10.x, vector3Int10.y, vector3Int10.z];
            bool flag7 = this.world.InBounds(vector3Int6) && present[vector3Int6.x, vector3Int6.y, vector3Int6.z];
            bool flag8 = this.world.InBounds(vector3Int3) && present[vector3Int3.x, vector3Int3.y, vector3Int3.z] && this.world.InBounds(vector3Int6) && present[vector3Int6.x, vector3Int6.y, vector3Int6.z] && this.world.InBounds(vector3Int9) && present[vector3Int9.x, vector3Int9.y, vector3Int9.z];
            bool flag9 = this.world.InBounds(vector3Int3) && present[vector3Int3.x, vector3Int3.y, vector3Int3.z];
            Matrix4x4 matrix4x;
            if (filledEntitySmoothedPctAt < 1f)
            {
                matrix4x = GameObjectFader.GetAnimatedRelativeMatrix(vector, quaternion, vector2, filledEntitySmoothedPctAt, num, flag);
            }
            else
            {
                matrix4x = Matrix4x4.identity;
            }
            sectionRenderer.CombinedTiledDecalsMeshes.AddOrUpdate(pos, spec, dir, !flag2, !flag3, !flag4, !flag5, !flag6, !flag7, !flag8, !flag9, 1f, matrix4x, pos + dir, spec.TexCoordsScale);
        }

        private void GetLocalDirsFor(Vector3Int dir, out Vector3Int right, out Vector3Int up)
        {
            if (dir == new Vector3Int(0, -1, 0))
            {
                right = new Vector3Int(1, 0, 0);
                up = new Vector3Int(0, 0, 1);
                return;
            }
            if (dir == new Vector3Int(0, 1, 0))
            {
                right = new Vector3Int(1, 0, 0);
                up = new Vector3Int(0, 0, -1);
                return;
            }
            if (dir == new Vector3Int(1, 0, 0))
            {
                right = new Vector3Int(0, 0, -1);
                up = new Vector3Int(0, 1, 0);
                return;
            }
            if (dir == new Vector3Int(-1, 0, 0))
            {
                right = new Vector3Int(0, 0, 1);
                up = new Vector3Int(0, 1, 0);
                return;
            }
            if (dir == new Vector3Int(0, 0, 1))
            {
                right = new Vector3Int(1, 0, 0);
                up = new Vector3Int(0, 1, 0);
                return;
            }
            right = new Vector3Int(-1, 0, 0);
            up = new Vector3Int(0, 1, 0);
        }

        private bool ShouldBeAt(TiledDecalsSpec spec, bool[,,] forced, Vector3Int pos, Vector3Int dir)
        {
            if (!this.world.InBounds(pos))
            {
                return false;
            }
            Vector3Int vector3Int = pos + dir;
            if (!this.world.InBounds(vector3Int) || !this.world.CellsInfo.AnyStructureAt(vector3Int) || this.world.FogOfWar.IsFogged(vector3Int))
            {
                return false;
            }
            if (spec.OnlyIfForced && !forced[pos.x, pos.y, pos.z])
            {
                return false;
            }
            if (spec.DisallowInLobby && Get.InLobby)
            {
                return false;
            }
            float num;
            float num2;
            if (dir == new Vector3Int(0, 1, 0))
            {
                if (!spec.AllowOnCeiling)
                {
                    return false;
                }
                num = (float)pos.x;
                num2 = (float)pos.z;
            }
            else if (dir == new Vector3Int(0, -1, 0))
            {
                if (!spec.AllowOnFloor)
                {
                    return false;
                }
                num = (float)pos.x;
                num2 = (float)pos.z;
            }
            else
            {
                if (!spec.AllowOnWalls)
                {
                    return false;
                }
                if (dir == new Vector3Int(1, 0, 0) || dir == new Vector3Int(-1, 0, 0))
                {
                    num = (float)pos.y;
                    num2 = (float)pos.z;
                }
                else
                {
                    num = (float)pos.x;
                    num2 = (float)pos.y;
                }
            }
            if (!spec.CanTransitionToFilledEntities && this.world.CellsInfo.IsFilled(pos))
            {
                return false;
            }
            bool flag = false;
            List<Entity> entitiesAt = this.world.GetEntitiesAt(vector3Int);
            int i = 0;
            int count = entitiesAt.Count;
            while (i < count)
            {
                if (spec.CanAppearOnEntity(entitiesAt[i].Spec))
                {
                    Structure structure = entitiesAt[i] as Structure;
                    if (structure == null || structure.StructureGOC.BevelsMask == 0)
                    {
                        flag = true;
                        break;
                    }
                }
                i++;
            }
            if (!flag)
            {
                return false;
            }
            if (forced[pos.x, pos.y, pos.z])
            {
                return true;
            }
            num += Calc.HashToRange(Calc.CombineHash(spec.MyStableHash, 817361990), 0f, 1000f);
            return Noise.PerlinNoise(num / 4f, num2 / 4f) > 0.5f;
        }

        private bool CanEverAffectTiledDecals(Entity entity)
        {
            return entity is Structure;
        }

        private bool NeedsData(TiledDecalsSpec spec)
        {
            if (spec.DisallowInLobby && Get.InLobby)
            {
                return false;
            }
            if (spec.OnlyIfForced && !this.everForced.Contains(spec))
            {
                return false;
            }
            bool flag = false;
            foreach (EntitySpec entitySpec in spec.CanAppearOnEntities)
            {
                if (this.world.AnyEntityOfSpec(entitySpec))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        private void CheckAddData(TiledDecalsSpec spec)
        {
            if (this.hasData.Contains(spec) || !this.NeedsData(spec))
            {
                return;
            }
            Vector3Int size = this.world.Size;
            this.data.Add(new TiledDecals.TiledDecalsData
            {
                spec = spec,
                presentDown = new bool[size.x, size.y, size.z],
                presentUp = new bool[size.x, size.y, size.z],
                presentLeft = new bool[size.x, size.y, size.z],
                presentRight = new bool[size.x, size.y, size.z],
                presentForward = new bool[size.x, size.y, size.z],
                presentBack = new bool[size.x, size.y, size.z],
                forced = new bool[size.x, size.y, size.z],
                needUpdateMeshAtDown = new HashSet<Vector3Int>(),
                needUpdateMeshAtUp = new HashSet<Vector3Int>(),
                needUpdateMeshAtLeft = new HashSet<Vector3Int>(),
                needUpdateMeshAtRight = new HashSet<Vector3Int>(),
                needUpdateMeshAtForward = new HashSet<Vector3Int>(),
                needUpdateMeshAtBack = new HashSet<Vector3Int>()
            });
            bool[,,] forced = this.data[this.data.Count - 1].forced;
            for (int i = 0; i < this.forcedDecals.Count; i++)
            {
                if (this.forcedDecals[i].spec == spec)
                {
                    Vector3Int pos = this.forcedDecals[i].pos;
                    forced[pos.x, pos.y, pos.z] = true;
                }
            }
            this.hasData.Add(spec);
            this.specsThatDontHaveDataYet.Remove(spec);
        }

        public void CheckAddDataAll()
        {
            List<TiledDecalsSpec> all = Get.Specs.GetAll<TiledDecalsSpec>();
            for (int i = 0; i < all.Count; i++)
            {
                try
                {
                    this.CheckAddData(all[i]);
                }
                catch (Exception ex)
                {
                    Log.Error("Error in CheckAddData() for tiled decals.", ex);
                }
            }
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            for (int i = 0; i < this.forcedDecals.Count; i++)
            {
                this.everForced.Add(this.forcedDecals[i].spec);
            }
            this.specsThatDontHaveDataYet.Clear();
            this.specsThatDontHaveDataYet.AddRange(Get.Specs.GetAll<TiledDecalsSpec>());
        }

        [Saved]
        private World world;

        [Saved]
        private List<TiledDecals.ForcedDecal> forcedDecals = new List<TiledDecals.ForcedDecal>();

        private List<TiledDecals.TiledDecalsData> data = new List<TiledDecals.TiledDecalsData>();

        private HashSet<Vector3Int> tmpToUpdate = new HashSet<Vector3Int>();

        private bool anyNeedUpdateMesh;

        private HashSet<TiledDecalsSpec> everForced = new HashSet<TiledDecalsSpec>();

        private List<TiledDecalsSpec> specsThatDontHaveDataYet = new List<TiledDecalsSpec>();

        private HashSet<TiledDecalsSpec> hasData = new HashSet<TiledDecalsSpec>();

        private bool temporarilyIgnoreEntityChanges;

        private struct TiledDecalsData
        {
            public TiledDecalsSpec spec;

            public bool[,,] presentDown;

            public bool[,,] presentUp;

            public bool[,,] presentLeft;

            public bool[,,] presentRight;

            public bool[,,] presentForward;

            public bool[,,] presentBack;

            public bool[,,] forced;

            public HashSet<Vector3Int> needUpdateMeshAtDown;

            public HashSet<Vector3Int> needUpdateMeshAtUp;

            public HashSet<Vector3Int> needUpdateMeshAtLeft;

            public HashSet<Vector3Int> needUpdateMeshAtRight;

            public HashSet<Vector3Int> needUpdateMeshAtForward;

            public HashSet<Vector3Int> needUpdateMeshAtBack;
        }

        private struct ForcedDecal
        {
            [Saved]
            public TiledDecalsSpec spec;

            [Saved]
            public Vector3Int pos;
        }
    }
}