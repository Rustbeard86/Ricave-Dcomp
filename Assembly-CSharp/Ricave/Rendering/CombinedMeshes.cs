using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ricave.Rendering
{
    public class CombinedMeshes
    {
        public void Update()
        {
            for (int i = this.combinedGameObjects.Count - 1; i >= 0; i--)
            {
                if (this.combinedGameObjects[i] == null)
                {
                    Log.Error("One of the game objects with its meshes combined has been destroyed without being removed from the combined meshes. This means that its mesh will stay combined forever.", false);
                    this.combinedGameObjectsHashSet.Remove(this.combinedGameObjects[i]);
                    this.combinedGameObjects.RemoveAt(i);
                }
            }
            if (this.meshesToRebuild.Count != 0)
            {
                Profiler.Begin("Rebuild combined meshes");
                try
                {
                    for (int j = 0; j < this.meshesToRebuild.Count; j++)
                    {
                        this.RebuildCombinedMeshForMaterial(this.meshesToRebuild[j].Item1, this.meshesToRebuild[j].Item2);
                    }
                    this.meshesToRebuild.Clear();
                }
                finally
                {
                    Profiler.End();
                }
            }
            if (this.optimizeMeshesOnFrame != -1 && Clock.Frame >= this.optimizeMeshesOnFrame)
            {
                Profiler.Begin("Optimize combined meshes");
                try
                {
                    this.optimizeMeshesOnFrame = -1;
                    for (int k = 0; k < this.meshesToOptimize.Count; k++)
                    {
                        if (this.meshesToOptimize[k] != null)
                        {
                            this.meshesToOptimize[k].Optimize();
                        }
                    }
                    this.meshesToOptimize.Clear();
                }
                finally
                {
                    Profiler.End();
                }
            }
            int l = 0;
            int count = this.meshes.Count;
            while (l < count)
            {
                ValueTuple<Material, Mesh, ShadowCastingMode> valueTuple = this.meshes[l];
                Graphics.DrawMesh(valueTuple.Item2, Matrix4x4.identity, valueTuple.Item1, 0, null, 0, null, valueTuple.Item3);
                l++;
            }
        }

        public bool IsCombined(GameObject gameObject)
        {
            return gameObject != null && this.combinedGameObjectsHashSet.Contains(gameObject);
        }

        public void Add(GameObject gameObject, bool canNotifyAdjacentFilledToShowOrHideFaces = true)
        {
            if (gameObject == null)
            {
                Log.Error("Tried to add null game object to combined meshes.", false);
                return;
            }
            if (this.IsCombined(gameObject))
            {
                Log.Error("Tried to add the same game object twice to combined meshes.", false);
                return;
            }
            if (Get.GameObjectFader.IsFading(gameObject))
            {
                Log.Error("Tried to add a game object to combined meshes but it's still fading in or out. Wait until it's no longer fading before calling this method or remove it forcefully.", false);
                return;
            }
            if (!gameObject.activeSelf)
            {
                Log.Error("Tried to add an inactive game object to combined meshes. This would theoretically normally work, but we don't allow this since it may indicate a bug somewhere. If an entity is disabled, then it should never be combined. Otherwise it wouldn't be possible to raycast it.", false);
                return;
            }
            this.combinedGameObjects.Add(gameObject);
            this.combinedGameObjectsHashSet.Add(gameObject);
            foreach (MeshRenderer meshRenderer in gameObject.GetMeshRenderersInChildren(false))
            {
                if (!(meshRenderer.sharedMaterial == null))
                {
                    MeshFilter meshFilter = meshRenderer.GetMeshFilter();
                    if (!(meshFilter == null) && !(meshFilter.sharedMesh == null))
                    {
                        if (!this.meshesToRebuild.Contains(new ValueTuple<Material, ShadowCastingMode>(meshRenderer.sharedMaterial, meshRenderer.shadowCastingMode)))
                        {
                            this.Add(this.GetPostProcessedMeshToMerge(meshFilter), meshRenderer.sharedMaterial, meshRenderer.shadowCastingMode, meshFilter.transform.localToWorldMatrix);
                        }
                        meshRenderer.enabled = false;
                    }
                }
            }
            if (canNotifyAdjacentFilledToShowOrHideFaces)
            {
                this.CheckNotifyAdjacentFilledToShowOrHideFaces(gameObject, false);
            }
        }

        public bool TryRemove(GameObject gameObject)
        {
            if (this.IsCombined(gameObject))
            {
                this.Remove(gameObject, true);
                return true;
            }
            return false;
        }

        public void Remove(GameObject gameObject, bool canNotifyAdjacentFilledToShowOrHideFaces = true)
        {
            if (gameObject == null)
            {
                Log.Error("Tried to remove null game object from combined meshes.", false);
                return;
            }
            if (!this.IsCombined(gameObject))
            {
                Log.Error("Tried to remove game object from combined meshes but it's not here.", false);
                return;
            }
            this.combinedGameObjects.RemoveLast(gameObject);
            this.combinedGameObjectsHashSet.Remove(gameObject);
            foreach (MeshRenderer meshRenderer in gameObject.GetMeshRenderersInChildren(true))
            {
                if (meshRenderer.sharedMaterial != null && !this.meshesToRebuild.Contains(new ValueTuple<Material, ShadowCastingMode>(meshRenderer.sharedMaterial, meshRenderer.shadowCastingMode)))
                {
                    bool flag = false;
                    for (int i = 0; i < this.meshes.Count; i++)
                    {
                        if (this.meshes[i].Item1 == meshRenderer.sharedMaterial && this.meshes[i].Item3 == meshRenderer.shadowCastingMode)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        this.meshesToRebuild.Add(new ValueTuple<Material, ShadowCastingMode>(meshRenderer.sharedMaterial, meshRenderer.shadowCastingMode));
                    }
                }
            }
            if (canNotifyAdjacentFilledToShowOrHideFaces)
            {
                this.CheckNotifyAdjacentFilledToShowOrHideFaces(gameObject, true);
            }
        }

        private void Add(Mesh mesh, Material material, ShadowCastingMode shadowCastingMode, Matrix4x4 matrix)
        {
            if (mesh.subMeshCount == 0 || mesh.vertexCount == 0)
            {
                return;
            }
            Mesh mesh2 = this.GetCombinedMeshFor(material, shadowCastingMode);
            if (mesh2 == null)
            {
                mesh2 = this.GetPooledMesh();
                this.meshes.Add(new ValueTuple<Material, Mesh, ShadowCastingMode>(material, mesh2, shadowCastingMode));
            }
            CombineInstance combineInstance = default(CombineInstance);
            combineInstance.mesh = mesh2;
            combineInstance.transform = Matrix4x4.identity;
            CombineInstance combineInstance2 = default(CombineInstance);
            combineInstance2.mesh = mesh;
            combineInstance2.transform = matrix;
            CombinedMeshes.tmpCombineInstanceArray[0] = combineInstance;
            CombinedMeshes.tmpCombineInstanceArray[1] = combineInstance2;
            Mesh pooledMesh = this.GetPooledMesh();
            pooledMesh.CombineMeshes(CombinedMeshes.tmpCombineInstanceArray, true);
            if (!this.meshesToOptimize.Contains(pooledMesh))
            {
                this.meshesToOptimize.Add(pooledMesh);
            }
            this.optimizeMeshesOnFrame = Clock.Frame + 1;
            this.ReturnPooledMesh(mesh2);
            for (int i = 0; i < this.meshes.Count; i++)
            {
                if (this.meshes[i].Item1 == material && this.meshes[i].Item3 == shadowCastingMode)
                {
                    this.meshes[i] = new ValueTuple<Material, Mesh, ShadowCastingMode>(material, pooledMesh, shadowCastingMode);
                    return;
                }
            }
        }

        private void RebuildCombinedMeshForMaterial(Material material, ShadowCastingMode shadowCastingMode)
        {
            Mesh combinedMeshFor = this.GetCombinedMeshFor(material, shadowCastingMode);
            if (combinedMeshFor != null)
            {
                this.ReturnPooledMesh(combinedMeshFor);
                this.tmpMeshesToCombine.Clear();
                for (int i = 0; i < this.combinedGameObjects.Count; i++)
                {
                    foreach (MeshRenderer meshRenderer in this.combinedGameObjects[i].GetMeshRenderersInChildren(false))
                    {
                        if (!(meshRenderer.sharedMaterial != material) && meshRenderer.shadowCastingMode == shadowCastingMode)
                        {
                            MeshFilter meshFilter = meshRenderer.GetMeshFilter();
                            if (!(meshFilter == null) && !(meshFilter.sharedMesh == null))
                            {
                                Mesh postProcessedMeshToMerge = this.GetPostProcessedMeshToMerge(meshFilter);
                                if (postProcessedMeshToMerge.subMeshCount != 0 && postProcessedMeshToMerge.vertexCount != 0)
                                {
                                    this.tmpMeshesToCombine.Add(new ValueTuple<Mesh, Matrix4x4>(postProcessedMeshToMerge, meshFilter.transform.localToWorldMatrix));
                                }
                            }
                        }
                    }
                }
                if (this.tmpCombineInstances == null || this.tmpCombineInstances.Length != this.tmpMeshesToCombine.Count)
                {
                    this.tmpCombineInstances = new CombineInstance[this.tmpMeshesToCombine.Count];
                }
                for (int j = 0; j < this.tmpMeshesToCombine.Count; j++)
                {
                    CombineInstance combineInstance = default(CombineInstance);
                    combineInstance.mesh = this.tmpMeshesToCombine[j].Item1;
                    combineInstance.transform = this.tmpMeshesToCombine[j].Item2;
                    this.tmpCombineInstances[j] = combineInstance;
                }
                Mesh pooledMesh = this.GetPooledMesh();
                pooledMesh.CombineMeshes(this.tmpCombineInstances, true);
                if (!this.meshesToOptimize.Contains(pooledMesh))
                {
                    this.meshesToOptimize.Add(pooledMesh);
                }
                this.optimizeMeshesOnFrame = Clock.Frame + 1;
                for (int k = 0; k < this.meshes.Count; k++)
                {
                    if (this.meshes[k].Item1 == material && this.meshes[k].Item3 == shadowCastingMode)
                    {
                        this.meshes[k] = new ValueTuple<Material, Mesh, ShadowCastingMode>(material, pooledMesh, shadowCastingMode);
                        return;
                    }
                }
                return;
            }
        }

        private Mesh GetCombinedMeshFor(Material material, ShadowCastingMode shadowCastingMode)
        {
            for (int i = 0; i < this.meshes.Count; i++)
            {
                if (this.meshes[i].Item1 == material && this.meshes[i].Item3 == shadowCastingMode)
                {
                    return this.meshes[i].Item2;
                }
            }
            return null;
        }

        public static bool ShouldEntityBeCombined(EntitySpec entitySpec)
        {
            return entitySpec.RenderIfPlayerCantSee && (!entitySpec.IsStructure || !entitySpec.Structure.Animated);
        }

        private Mesh GetPostProcessedMeshToMerge(MeshFilter meshFilter)
        {
            if ((PrimitiveMeshes.IsCube(meshFilter.sharedMesh) || PrimitiveMeshes.IsCubeXZ(meshFilter.sharedMesh) || PrimitiveMeshes.IsCubeUpDown(meshFilter.sharedMesh) || meshFilter.gameObject.CompareTag("HasBevels") || meshFilter.gameObject.CompareTag("HasBevelsUpDown") || meshFilter.gameObject.CompareTag("HasBevelsSide")) && meshFilter.transform.lossyScale == Vector3.one && meshFilter.transform.rotation == Quaternion.identity)
            {
                EntityGOC componentInParent = meshFilter.gameObject.GetComponentInParent<EntityGOC>();
                if (componentInParent != null && this.WantsToHaveFacesHiddenByAdjacentFilled(componentInParent.Entity))
                {
                    if (meshFilter.gameObject.CompareTag("HasBevels"))
                    {
                        return CubesWithBevels.Get(((StructureGOC)componentInParent).Bevels, !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Left), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Right), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Up), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Down), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Forward), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Back));
                    }
                    if (meshFilter.gameObject.CompareTag("HasBevelsSide"))
                    {
                        Mesh mesh;
                        return CubesWithBevels.GetSplit(((StructureGOC)componentInParent).Bevels, !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Left), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Right), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Up), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Down), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Forward), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Back), out mesh);
                    }
                    if (meshFilter.gameObject.CompareTag("HasBevelsUpDown"))
                    {
                        Mesh mesh2;
                        CubesWithBevels.GetSplit(((StructureGOC)componentInParent).Bevels, !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Left), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Right), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Up), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Down), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Forward), !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Back), out mesh2);
                        return mesh2;
                    }
                    return PrimitiveMeshes.CubeWithMissingFaces(!PrimitiveMeshes.IsCubeUpDown(meshFilter.sharedMesh) && !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Left), !PrimitiveMeshes.IsCubeUpDown(meshFilter.sharedMesh) && !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Right), !PrimitiveMeshes.IsCubeXZ(meshFilter.sharedMesh) && !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Up), !PrimitiveMeshes.IsCubeXZ(meshFilter.sharedMesh) && !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Down), !PrimitiveMeshes.IsCubeUpDown(meshFilter.sharedMesh) && !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Forward), !PrimitiveMeshes.IsCubeUpDown(meshFilter.sharedMesh) && !this.< GetPostProcessedMeshToMerge > g__AnyCombinedFilledAt | 19_0(componentInParent.Entity.Position + Vector3IntUtility.Back));
                }
            }
            return meshFilter.sharedMesh;
        }

        private void CheckNotifyAdjacentFilledToShowOrHideFaces(GameObject gameObject, bool assumeWasCombined = false)
        {
            EntityGOC entityGOC;
            if (!gameObject.TryGetComponent<EntityGOC>(out entityGOC))
            {
                return;
            }
            if (!this.HidesFacesInAdjacentFilledEntities(entityGOC.Entity, assumeWasCombined))
            {
                return;
            }
            foreach (Vector3Int vector3Int in Vector3IntUtility.DirectionsCardinal)
            {
                Vector3Int vector3Int2 = entityGOC.Entity.Position + vector3Int;
                if (vector3Int2.InBounds() && Get.CellsInfo.AnyFilledCantSeeThroughAt(vector3Int2))
                {
                    foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int2))
                    {
                        if (this.WantsToHaveFacesHiddenByAdjacentFilled(entity) && entity.EntityGOC.IsMeshCombined)
                        {
                            CombinedMeshes combinedMeshes = Get.SectionsManager.GetSectionRenderer(entity.Position).CombinedMeshes;
                            combinedMeshes.Remove(entity.GameObject, false);
                            combinedMeshes.Add(entity.GameObject, false);
                        }
                    }
                }
            }
        }

        private bool WantsToHaveFacesHiddenByAdjacentFilled(Entity entity)
        {
            return entity is Structure && entity.Spec.Structure.IsFilled && !entity.Spec.CanSeeThrough;
        }

        private bool HidesFacesInAdjacentFilledEntities(Entity entity, bool assumeCombined = false)
        {
            return entity is Structure && entity.Spec.Structure.IsFilled && !entity.Spec.CanSeeThrough && !entity.Spec.Structure.HideAdjacentFacesUnfoggedOrNotFilled && (assumeCombined || entity.EntityGOC.IsMeshCombined);
        }

        private Mesh GetPooledMesh()
        {
            if (this.meshPool.Count != 0)
            {
                Mesh mesh = this.meshPool[this.meshPool.Count - 1];
                this.meshPool.RemoveAt(this.meshPool.Count - 1);
                return mesh;
            }
            return new Mesh();
        }

        private void ReturnPooledMesh(Mesh mesh)
        {
            mesh.Clear();
            this.meshPool.Add(mesh);
            this.meshesToOptimize.Remove(mesh);
        }

        [CompilerGenerated]
        private bool <GetPostProcessedMeshToMerge>g__AnyCombinedFilledAt|19_0(Vector3Int at)
		{
			if (!at.InBounds() || !Get.CellsInfo.AnyFilledCantSeeThroughAt(at))
			{
				return false;
			}
			foreach (Entity entity in Get.World.GetEntitiesAt(at))
			{
				if (this.HidesFacesInAdjacentFilledEntities(entity, false))

                {
            Structure structure = entity as Structure;
            if (structure == null || structure.StructureGOC.BevelsMask == 0)
            {
                return true;
            }
        }
        }
			return false;
		}

		private List<GameObject> combinedGameObjects = new List<GameObject>(20);

        private HashSet<GameObject> combinedGameObjectsHashSet = new HashSet<GameObject>();

        private List<ValueTuple<Material, Mesh, ShadowCastingMode>> meshes = new List<ValueTuple<Material, Mesh, ShadowCastingMode>>(20);

        private List<ValueTuple<Material, ShadowCastingMode>> meshesToRebuild = new List<ValueTuple<Material, ShadowCastingMode>>(20);

        private int optimizeMeshesOnFrame = -1;

        private List<Mesh> meshesToOptimize = new List<Mesh>(20);

        private List<Mesh> meshPool = new List<Mesh>();

        private static readonly CombineInstance[] tmpCombineInstanceArray = new CombineInstance[2];

        private List<ValueTuple<Mesh, Matrix4x4>> tmpMeshesToCombine = new List<ValueTuple<Mesh, Matrix4x4>>();

        private CombineInstance[] tmpCombineInstances;
    }
}