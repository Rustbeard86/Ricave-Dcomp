using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class ItemGOC : EntityGOC
    {
        public Item Item
        {
            get
            {
                return (Item)base.Entity;
            }
        }

        protected override Vector3 DesiredGameObjectPosition
        {
            get
            {
                return base.DesiredGameObjectPosition - new Vector3(0f, (1f - base.transform.localScale.y) / 2f - 0.02f, 0f) + this.JumpAnimationOffset + this.IdleAnimationOffset + this.ActorInSameCellOffset;
            }
        }

        protected override Vector3 DesiredGameObjectScale
        {
            get
            {
                float num = (Clock.Time - this.jumpAnimationStartTime) / 0.45f;
                float num2;
                float num3;
                if (num < 0.5f)
                {
                    num2 = Calc.LerpDouble(0f, 0.5f, 1.5f, 1f, num);
                    num3 = Calc.LerpDouble(0f, 0.5f, 0.5f, 1.3f, num);
                }
                else
                {
                    num2 = 1f;
                    num3 = Calc.LerpDouble(0.5f, 1f, 1.3f, 1f, num);
                }
                return new Vector3(num2, num3, 1f).Multiply(base.Entity.Scale) * 0.42f * Calc.LerpDouble(0f, 0.6f, 0.2f, 1f, (Get.CameraPosition - base.Entity.Position).magnitude) * Math.Max(Get.GameObjectFader.GetSmoothedFadePct(this.Item), 0.01f);
            }
        }

        protected override Quaternion DesiredGameObjectRotation
        {
            get
            {
                return base.transform.localRotation;
            }
        }

        private Vector3 JumpAnimationOffset
        {
            get
            {
                if (this.jumpAnimationStartTime == -1f)
                {
                    return default(Vector3);
                }
                float num = Clock.Time - this.jumpAnimationStartTime;
                float num2;
                if (num < 0.5f)
                {
                    num2 = Calc.Sin(num / 0.5f * 3.1415927f) * 0.35f;
                }
                else if (num < 1f)
                {
                    num2 = Calc.Sin((num - 0.5f) / 0.5f * 3.1415927f) * 0.18f;
                }
                else
                {
                    num2 = 0f;
                }
                return new Vector3(0f, num2, 0f);
            }
        }

        private Vector3 IdleAnimationOffset
        {
            get
            {
                return new Vector3(0f, Calc.Sin(Clock.Time * 2f) * 0.02f, 0f);
            }
        }

        private Vector3 ActorInSameCellOffset
        {
            get
            {
                if (Get.CellsInfo.AnyActorAt(base.Entity.Position) && base.Entity.Position != Get.NowControlledActor.Position)
                {
                    return (base.Entity.Position - Get.CameraPosition).normalized * 0.18f;
                }
                return Vector3.zero;
            }
        }

        public void StartJumpAnimation()
        {
            this.jumpAnimationStartTime = Clock.Time;
            if ((Get.CameraPosition - base.Entity.Position).sqrMagnitude < 9.61f)
            {
                Get.Sound_ItemJump.PlayOneShot(new Vector3?(base.Entity.Position), 1f, 1f);
            }
            base.SetTransformToDesiredInstantly();
        }

        public void StopJumpAnimation()
        {
            this.jumpAnimationStartTime = -1f;
        }

        public override void OnEntitySpawned()
        {
            base.OnEntitySpawned();
            if (!(base.Entity is Item))
            {
                Log.Error("ItemGOC spawned with a non-Item entity.", false);
            }
            this.CheckAddItemBackground();
            this.CheckAddBlobShadow();
        }

        public override void OnSetGameObjectAfterLoading()
        {
            base.OnSetGameObjectAfterLoading();
            this.CheckAddItemBackground();
            this.CheckAddBlobShadow();
        }

        private void CheckAddBlobShadow()
        {
            if (this.blobShadow != null)
            {
                return;
            }
            if (ItemGOC.ItemBlobShadowPrefab == null)
            {
                ItemGOC.ItemBlobShadowPrefab = Assets.Get<GameObject>("Prefabs/Misc/ItemBlobShadow");
            }
            this.blobShadowUndoNonUniformScale = new GameObject("blobShadowUndoNonUniformScale");
            this.blobShadowUndoNonUniformScale.transform.SetParent(base.transform, false);
            this.blobShadow = Object.Instantiate<GameObject>(ItemGOC.ItemBlobShadowPrefab, Vector3.zero, Quaternion.identity, this.blobShadowUndoNonUniformScale.transform);
            ActorGOC.SetBlobShadowTransform(this.blobShadow, this.blobShadowUndoNonUniformScale, base.transform, base.transform.position.RoundToVector3Int(), false);
        }

        private void CheckAddItemBackground()
        {
            if (this.itemBackground != null && this.itemGlow != null)
            {
                return;
            }
            if (ItemGOC.ItemGlowPrefab == null)
            {
                ItemGOC.ItemGlowPrefab = Assets.Get<GameObject>("Prefabs/Misc/ItemGlow");
            }
            if (ItemGOC.LobbyItemGlowPrefab == null)
            {
                ItemGOC.LobbyItemGlowPrefab = Assets.Get<GameObject>("Prefabs/Misc/LobbyItemGlow");
            }
            if (ItemGOC.ImportantLobbyItemGlowPrefab == null)
            {
                ItemGOC.ImportantLobbyItemGlowPrefab = Assets.Get<GameObject>("Prefabs/Misc/ImportantLobbyItemGlow");
            }
            if (this.itemGlow == null && this.Item.Spec.IsLobbyItemOrLobbyRelated && !Get.InLobby)
            {
                GameObject gameObject;
                if (this.Item.Spec == Get.Entity_MemoryPiece1 || this.Item.Spec == Get.Entity_MemoryPiece2 || this.Item.Spec == Get.Entity_MemoryPiece3 || this.Item.Spec == Get.Entity_MemoryPiece4)
                {
                    gameObject = ItemGOC.ImportantLobbyItemGlowPrefab;
                }
                else if (this.Item.Spec.IsLobbyItemOrLobbyRelated)
                {
                    gameObject = ItemGOC.LobbyItemGlowPrefab;
                }
                else
                {
                    gameObject = ItemGOC.ItemGlowPrefab;
                }
                this.itemGlow = Object.Instantiate<GameObject>(gameObject, Vector3.zero, Quaternion.identity, base.transform);
                Transform transform = this.itemGlow.transform;
                transform.localPosition = new Vector3(0f, 0f, 0f);
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }
        }

        private void LateUpdate()
        {
            this.LookAtPlayer();
            ActorGOC.SetBlobShadowTransform(this.blobShadow, this.blobShadowUndoNonUniformScale, base.transform, this.Item.Position, false);
        }

        private void LookAtPlayer()
        {
            Transform transform = base.transform;
            float num;
            float num2;
            ActorGOC.GetLookAtPlayerRot(transform.position, transform.localScale, out num, out num2);
            transform.rotation = Quaternion.Euler(num2, num, 0f);
        }

        public override bool UpdateAppearance()
        {
            this.LookAtPlayer();
            return true;
        }

        private float jumpAnimationStartTime = -1f;

        private GameObject blobShadow;

        private GameObject blobShadowUndoNonUniformScale;

        private GameObject itemBackground;

        private GameObject itemGlow;

        private static GameObject ItemGlowPrefab;

        private static GameObject LobbyItemGlowPrefab;

        private static GameObject ImportantLobbyItemGlowPrefab;

        private const float ItemGlowOffsetY = 0f;

        private const float ItemBackgroundOffsetY = 0f;

        private const float ItemScale = 0.42f;

        private static GameObject ItemBlobShadowPrefab;
    }
}