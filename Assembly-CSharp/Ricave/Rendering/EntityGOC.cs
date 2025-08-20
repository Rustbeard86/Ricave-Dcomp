using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public abstract class EntityGOC : MonoBehaviour
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
            set
            {
                this.entity = value;
            }
        }

        private bool WantsTransformLerped
        {
            get
            {
                return !CombinedMeshes.ShouldEntityBeCombined(this.entity.Spec) && this.entity.Spec.WantsUpdate;
            }
        }

        public bool TransformCurrentlyHandledByGameObjectFader
        {
            get
            {
                return Get.GameObjectFader.IsTransformCurrentlyHandledByFader(this.Entity);
            }
        }

        public Bounds BoundingBox
        {
            get
            {
                return EntityGameObjectCenterCalculator.CalculateBoundingBox(this);
            }
        }

        private bool CanUpdateTransformNow
        {
            get
            {
                return (this.entity.EverChangedPositionOrBecameActiveSinceSpawningUnsaved || base.gameObject.activeSelf) && this.WantsTransformLerped && !this.TransformCurrentlyHandledByGameObjectFader;
            }
        }

        protected virtual bool InterpolateRotation
        {
            get
            {
                return false;
            }
        }

        protected virtual bool InterpolateScale
        {
            get
            {
                return false;
            }
        }

        protected virtual Vector3 DesiredGameObjectPosition
        {
            get
            {
                if (this.entity.Spec.UsePositionOffsetFromStairs && Get.CellsInfo.AnyStairsAt(this.entity.Position))
                {
                    return this.entity.Position + Vector3.up * Get.CellsInfo.OffsetFromStairsAt(this.entity.Position);
                }
                return this.entity.Position;
            }
        }

        protected virtual Quaternion DesiredGameObjectRotation
        {
            get
            {
                return this.entity.Rotation;
            }
        }

        protected virtual Vector3 DesiredGameObjectScale
        {
            get
            {
                return this.entity.Scale;
            }
        }

        public List<MeshRenderer> MeshRenderers
        {
            get
            {
                if (this.cachedMeshRenderers == null)
                {
                    this.cachedMeshRenderers = new List<MeshRenderer>();
                }
                GameObjectUtility.FilterActive<MeshRenderer>(this.MeshRenderersIncludingInactive, this.cachedMeshRenderers);
                return this.cachedMeshRenderers;
            }
        }

        public List<MeshRenderer> MeshRenderersIncludingInactive
        {
            get
            {
                if (this.cachedMeshRenderersIncludingInactive == null)
                {
                    this.cachedMeshRenderersIncludingInactive = new List<MeshRenderer>();
                    base.gameObject.GetComponentsInChildren<MeshRenderer>(true, this.cachedMeshRenderersIncludingInactive);
                }
                return this.cachedMeshRenderersIncludingInactive;
            }
        }

        public List<Renderer> Renderers
        {
            get
            {
                if (this.cachedRenderers == null)
                {
                    this.cachedRenderers = new List<Renderer>();
                }
                GameObjectUtility.FilterActive<Renderer>(this.RenderersIncludingInactive, this.cachedRenderers);
                return this.cachedRenderers;
            }
        }

        public List<Renderer> RenderersIncludingInactive
        {
            get
            {
                if (this.cachedRenderersIncludingInactive == null)
                {
                    this.cachedRenderersIncludingInactive = new List<Renderer>();
                    base.gameObject.GetComponentsInChildren<Renderer>(true, this.cachedRenderersIncludingInactive);
                }
                return this.cachedRenderersIncludingInactive;
            }
        }

        public List<Collider> Colliders
        {
            get
            {
                if (this.cachedColliders == null)
                {
                    this.cachedColliders = new List<Collider>();
                }
                GameObjectUtility.FilterActive<Collider>(this.CollidersIncludingInactive, this.cachedColliders);
                return this.cachedColliders;
            }
        }

        public List<Collider> CollidersIncludingInactive
        {
            get
            {
                if (this.cachedCollidersIncludingInactive == null)
                {
                    this.cachedCollidersIncludingInactive = new List<Collider>();
                    base.gameObject.GetComponentsInChildren<Collider>(true, this.cachedCollidersIncludingInactive);
                }
                return this.cachedCollidersIncludingInactive;
            }
        }

        public bool HasAnyNonInspectModeOnlyCollider
        {
            get
            {
                List<Collider> colliders = this.Colliders;
                for (int i = 0; i < colliders.Count; i++)
                {
                    if (colliders[i].gameObject.layer != Get.InspectModeOnlyLayer)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsMeshCombined
        {
            get
            {
                return CombinedMeshes.ShouldEntityBeCombined(this.entity.Spec) && Get.SectionsManager.GetSectionRenderer(this.entity.Position).CombinedMeshes.IsCombined(base.gameObject);
            }
        }

        public virtual void OnEntitySpawned()
        {
            this.SetTransformToDesiredInstantly();
        }

        public virtual void OnSetGameObjectAfterLoading()
        {
            this.SetTransformToDesiredInstantly();
        }

        public void SetTransformToDesiredInstantly()
        {
            Transform transform = base.transform;
            transform.position = this.DesiredGameObjectPosition;
            transform.rotation = this.DesiredGameObjectRotation;
            transform.localScale = this.DesiredGameObjectScale;
            transform.position = this.DesiredGameObjectPosition;
        }

        public void SetScaleToDesiredInstantly()
        {
            base.transform.localScale = this.DesiredGameObjectScale;
        }

        public virtual void OnEntityFixedUpdate()
        {
        }

        public virtual void OnEntityUpdate()
        {
            if (this.CanUpdateTransformNow)
            {
                Transform transform = base.transform;
                transform.position = Vector3Utility.LerpWithDeltaTime(transform.position, this.DesiredGameObjectPosition, 6.963f);
                if (this.InterpolateRotation)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, this.DesiredGameObjectRotation, 550f * Clock.DeltaTime);
                }
                else
                {
                    transform.rotation = this.DesiredGameObjectRotation;
                }
                if (this.InterpolateScale)
                {
                    transform.localScale = Vector3.MoveTowards(transform.localScale, this.DesiredGameObjectScale, 6f * Clock.DeltaTime);
                    return;
                }
                transform.localScale = this.DesiredGameObjectScale;
            }
        }

        public virtual void OnDestroy()
        {
            if (this.entity != null)
            {
                Get.SectionsManager.GetSectionRenderer(this.entity.Position).CombinedMeshes.TryRemove(base.gameObject);
            }
        }

        public void OnDestroyEvenIfInactive()
        {
            this.OnDestroy();
        }

        public void OnFadeInComplete()
        {
            this.UpdateAppearance();
            if (CombinedMeshes.ShouldEntityBeCombined(this.Entity.Spec))
            {
                Get.SectionsManager.GetSectionRenderer(this.Entity.Position).CombinedMeshes.Add(base.gameObject, true);
            }
        }

        public void OnFadeInInterrupted()
        {
            if (this.entity.Spawned)
            {
                this.UpdateAppearance();
            }
        }

        public virtual bool UpdateAppearance()
        {
            if (!this.TransformCurrentlyHandledByGameObjectFader)
            {
                Transform transform = base.transform;
                Vector3 position = transform.position;
                Quaternion rotation = transform.rotation;
                Vector3 localScale = transform.localScale;
                bool wantsTransformLerped = this.WantsTransformLerped;
                if (!wantsTransformLerped)
                {
                    transform.position = this.DesiredGameObjectPosition;
                }
                if (!wantsTransformLerped || !this.InterpolateRotation)
                {
                    transform.rotation = this.DesiredGameObjectRotation;
                }
                if (!wantsTransformLerped || !this.InterpolateScale)
                {
                    transform.localScale = this.DesiredGameObjectScale;
                }
                if (!wantsTransformLerped)
                {
                    transform.position = this.DesiredGameObjectPosition;
                }
                return transform.position != position || transform.rotation != rotation || transform.localScale != localScale;
            }
            return false;
        }

        public static bool CanEverAffectAdjacentEntitiesGameObjectAppearance(EntitySpec entity)
        {
            return entity.IsStructure;
        }

        public virtual void OnEntityChangedPosition(Vector3Int prevPos)
        {
        }

        public override string ToString()
        {
            Entity entity = this.Entity;
            if (entity != null)
            {
                Entity entity2 = entity;
                return ((entity2 != null) ? entity2.ToString() : null) + "'s GOC (" + base.GetType().Name + ")";
            }
            return base.GetType().Name + " (null Entity)";
        }

        private Entity entity;

        private List<MeshRenderer> cachedMeshRenderers;

        private List<MeshRenderer> cachedMeshRenderersIncludingInactive;

        private List<Renderer> cachedRenderers;

        private List<Renderer> cachedRenderersIncludingInactive;

        private List<Collider> cachedColliders;

        private List<Collider> cachedCollidersIncludingInactive;

        public const float PositionLerpSpeed = 0.13f;

        public const float PositionLerpSpeedWithDelta = 6.963f;
    }
}