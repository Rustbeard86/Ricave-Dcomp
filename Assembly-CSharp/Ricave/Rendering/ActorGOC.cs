using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ricave.Rendering
{
    public class ActorGOC : EntityGOC
    {
        public Actor Actor
        {
            get
            {
                return (Actor)base.Entity;
            }
        }

        public ActorGOC.MovesPerTurnAnimation LastMovesPerTurnAnimation
        {
            get
            {
                return this.movesPerTurnAnimation;
            }
        }

        public Vector3 CurrentVisualMovementDirection
        {
            get
            {
                return (this.DesiredGameObjectPosition - base.gameObject.transform.position).normalized;
            }
        }

        public Color SplatColor
        {
            get
            {
                return this.Actor.Spec.Actor.SplatColor;
            }
        }

        public float LastTimeLostHP
        {
            get
            {
                return this.lastTimeLostHP;
            }
        }

        protected override Vector3 DesiredGameObjectPosition
        {
            get
            {
                return base.DesiredGameObjectPosition + StrikeAnimationUtility.GetPosOffset(this.strikeAnimationStartTime, this.strikeAnimationDir, this.Actor.IsNowControlledActor, this.strikeAnimationExtraDistFactor) + Vector3.up * DanceAnimationUtility.GetPosOffset(this.Actor) * base.transform.localScale.y + this.offsetFromImpacts + Vector3.up * (this.GetStepAnimation() + this.GetLimpAnimation() + this.GetOnGroundOrFlyingOffset() + this.GetCurrentLyingOffset()) + Vector3.up * Calc.ResolveFadeInStayOut(this.lyingRotation + 90f, 10f, 20f, 60f) * 0.16f * base.transform.localScale.y + this.PlayerLeaningOffset;
            }
        }

        public Vector3 DesiredGameObjectScaleWithoutCapAndAnimation
        {
            get
            {
                Actor actor = this.Actor;
                return actor.Scale * this.scaleFactorFromConditions * Calc.HashToRange(Calc.CombineHashes<int, int>(actor.MyStableHash, 872314365), actor.Spec.Actor.ScaleRange) * (actor.IsBoss ? 1.5f : 1f) * (actor.IsBaby ? 0.73f : 1f);
            }
        }

        protected Vector3 DesiredGameObjectScaleWithoutCap
        {
            get
            {
                float num = this.SquashMoveAnimationPct;
                if (!this.Actor.IsNowControlledActor)
                {
                    num += Calc.ResolveFadeInStayOut(Clock.Time - this.lastWakeUpTime, 0.2f, 0f, 0.4f) * 0.75f;
                }
                float num2 = 1f + num * 0.18f + this.BreathingAnimation;
                float num3 = Calc.Clamp01(this.offsetFromImpacts.magnitude / 0.4f);
                return this.DesiredGameObjectScaleWithoutCapAndAnimation.Multiply(new Vector3(1f + num * 0.18f + num3 * 0.15f, Math.Max(1f - num * 0.18f, 0.01f), num2));
            }
        }

        protected override Vector3 DesiredGameObjectScale
        {
            get
            {
                return this.DesiredGameObjectScaleWithoutCap.Multiply(new Vector3(this.widthCapFactor, this.heightCapFactor, this.widthCapFactor)) * (this.Actor.IsNowControlledActor ? 1f : Math.Max(Get.GameObjectFader.GetSmoothedFadePct(this.Actor), 0.01f));
            }
        }

        protected override Quaternion DesiredGameObjectRotation
        {
            get
            {
                return base.transform.localRotation;
            }
        }

        private float BreathingAnimationRaw
        {
            get
            {
                return Calc.Sin(Clock.Time * 1.15f * Rand.RangeSeeded(0.98f, 1.02f, this.GetHashCode()) + (float)(Calc.AbsSafe(this.GetHashCode()) % 1000) / 100f);
            }
        }

        private float BreathingAnimation
        {
            get
            {
                if (this.Actor.IsNowControlledActor)
                {
                    return 0f;
                }
                return Math.Abs(this.lyingRotation) / 90f * this.BreathingAnimationRaw * 0.016f;
            }
        }

        private float SquashMoveAnimationPct
        {
            get
            {
                if (this.Actor.IsNowControlledActor)
                {
                    return 0f;
                }
                Vector3 position = base.transform.position;
                return Calc.Pow(Math.Max(Math.Abs(position.x - (float)Calc.RoundToInt(position.x)), Math.Abs(position.z - (float)Calc.RoundToInt(position.z))) * 2f, 0.43f);
            }
        }

        private Vector3 PlayerLeaningOffset
        {
            get
            {
                if (!this.Actor.IsNowControlledActor || Get.UI.WorldInputBlocked)
                {
                    return Vector3.zero;
                }
                Transform transform = Get.FPPControllerGOC.transform;
                Vector3 forward = transform.forward;
                Vector3 right = transform.right;
                Vector3 vector = Vector3.zero;
                bool heldDown = Get.KeyBinding_StationaryActionsOnly.HeldDown;
                if ((heldDown && (Get.KeyBinding_MoveRight.HeldDown || Get.KeyBinding_MoveRightAlt.HeldDown || Get.KeyBinding_RotateRight.HeldDown || Get.KeyBinding_RotateRightAlt.HeldDown)) || Get.KeyBinding_LeanRight.HeldDown)
                {
                    vector += right;
                }
                if ((heldDown && (Get.KeyBinding_MoveLeft.HeldDown || Get.KeyBinding_MoveLeftAlt.HeldDown || Get.KeyBinding_RotateLeft.HeldDown || Get.KeyBinding_RotateLeftAlt.HeldDown)) || Get.KeyBinding_LeanLeft.HeldDown)
                {
                    vector += -right;
                }
                if (heldDown && (Get.KeyBinding_MoveForward.HeldDown || Get.KeyBinding_MoveForwardAlt.HeldDown))
                {
                    vector += forward;
                }
                if (heldDown && (Get.KeyBinding_MoveBack.HeldDown || Get.KeyBinding_MoveBackAlt.HeldDown))
                {
                    vector += -forward;
                }
                if (vector == Vector3.zero)
                {
                    return vector;
                }
                return vector.normalized * 0.25f;
            }
        }

        private Bounds OccupiedBounds
        {
            get
            {
                if (this.occupiedBoundsCached == null)
                {
                    if (this.Actor.Spec.Actor.Texture == null)
                    {
                        if (this.NonMissingBodyRenderers.NullOrEmpty<ValueTuple<MeshRenderer, int>>())
                        {
                            this.occupiedBoundsCached = new Bounds?(new Bounds(new Vector3(0.5f, 0.5f, 0.5f), Vector3.zero));
                        }
                        else
                        {
                            Bounds bounds = EntityGameObjectCenterCalculator.CalculatePrefabBoundingBox(this.hasPlayerModel ? Get.PlayerModel.Model : this.Actor.Spec.PrefabAdjusted, delegate (GameObject x)
                            {
                                int num;
                                if (!int.TryParse(x.name, out num))
                                {
                                    return false;
                                }
                                if (num == 0)
                                {
                                    return false;
                                }
                                num--;
                                if (num < 0 || num >= this.Actor.BodyParts.Count)
                                {
                                    Log.Error(string.Concat(new string[]
                                    {
                                        "Body part index of a body part game object out of bounds (",
                                        num.ToString(),
                                        ") for actor ",
                                        this.Actor.ToStringSafe(),
                                        ". This means that the model is wrong."
                                    }), false);
                                    return false;
                                }
                                return this.Actor.BodyParts[num].IsMissing;
                            });
                            this.occupiedBoundsCached = new Bounds?(new Bounds(bounds.center + new Vector3(0.5f, 0.5f, 0.5f), bounds.size));
                        }
                    }
                    else
                    {
                        Rect rect = this.Actor.Spec.Actor.NoBodyPartOccupiedTextureRect;
                        foreach (BodyPart bodyPart in this.Actor.BodyParts)
                        {
                            if (!bodyPart.IsMissing)
                            {
                                if (rect.Empty())
                                {
                                    rect = bodyPart.Placement.OccupiedTextureRect;
                                }
                                else
                                {
                                    rect = RectUtility.BoundingBox(rect, bodyPart.Placement.OccupiedTextureRect, true);
                                }
                            }
                        }
                        if (rect.Empty())
                        {
                            this.occupiedBoundsCached = new Bounds?(new Bounds(new Vector3(0.5f, 0.5f, 0.5f), Vector3.zero));
                        }
                        else
                        {
                            this.occupiedBoundsCached = new Bounds?(new Bounds(new Vector3(rect.center.x, rect.center.y, 0.5f), new Vector3(rect.width, rect.height, 0.0001f)));
                        }
                    }
                }
                return this.occupiedBoundsCached.Value;
            }
        }

        private float OnGroundOffset
        {
            get
            {
                float num = base.transform.localScale.y;
                num /= Math.Max(Get.GameObjectFader.GetSmoothedFadePct(this.Actor), 0.01f);
                return -this.OccupiedBounds.min.y * num + (num - 1f) / 2f;
            }
        }

        private float LyingOffset
        {
            get
            {
                if (this.Actor.Spec.Actor.LayDirectionUp)
                {
                    return -(1f - this.OccupiedBounds.max.y) * base.transform.localScale.y + (base.transform.localScale.y - 1f) / 2f;
                }
                return -(1f - this.OccupiedBounds.max.z) * base.transform.localScale.z + (base.transform.localScale.z - 1f) / 2f;
            }
        }

        private float FlyingOffset
        {
            get
            {
                float num = this.OccupiedBounds.size.y * base.transform.localScale.y;
                if (this.heightCapFactor < 1f || num > 1f)
                {
                    return this.OnGroundOffset + 0.023f * base.transform.localScale.y;
                }
                return (-this.OccupiedBounds.min.y + 0.5f - this.OccupiedBounds.size.y / 2f) * base.transform.localScale.y;
            }
        }

        public List<ValueTuple<MeshRenderer, int, Color>> AllBodyRenderers
        {
            get
            {
                if (this.Actor.Spec.Actor.Texture != null)
                {
                    return null;
                }
                if (this.cachedAllBodyRenderers == null)
                {
                    this.cachedAllBodyRenderers = new List<ValueTuple<MeshRenderer, int, Color>>();
                    List<MeshRenderer> meshRenderersIncludingInactive = base.MeshRenderersIncludingInactive;
                    for (int i = 0; i < meshRenderersIncludingInactive.Count; i++)
                    {
                        int num;
                        if (int.TryParse(meshRenderersIncludingInactive[i].name, out num))
                        {
                            Color color = (meshRenderersIncludingInactive[i].sharedMaterial.HasProperty("_Color") ? meshRenderersIncludingInactive[i].sharedMaterial.color : Color.white);
                            this.cachedAllBodyRenderers.Add(new ValueTuple<MeshRenderer, int, Color>(meshRenderersIncludingInactive[i], num, color));
                        }
                    }
                }
                return this.cachedAllBodyRenderers;
            }
        }

        public List<ValueTuple<MeshRenderer, int>> NonMissingBodyRenderers
        {
            get
            {
                if (this.Actor.Spec.Actor.Texture != null)
                {
                    return null;
                }
                if (this.cachedNonMissingBodyRenderers == null)
                {
                    this.cachedNonMissingBodyRenderers = new List<ValueTuple<MeshRenderer, int>>();
                    List<ValueTuple<MeshRenderer, int, Color>> allBodyRenderers = this.AllBodyRenderers;
                    for (int i = 0; i < allBodyRenderers.Count; i++)
                    {
                        int num = allBodyRenderers[i].Item2;
                        if (num == 0)
                        {
                            this.cachedNonMissingBodyRenderers.Add(new ValueTuple<MeshRenderer, int>(allBodyRenderers[i].Item1, allBodyRenderers[i].Item2));
                        }
                        else
                        {
                            num--;
                            if (num < 0 || num >= this.Actor.BodyParts.Count)
                            {
                                Log.Error(string.Concat(new string[]
                                {
                                    "Body part index of a body part game object out of bounds (",
                                    num.ToString(),
                                    ") for actor ",
                                    this.Actor.ToStringSafe(),
                                    ". This means that the model is wrong."
                                }), false);
                            }
                            else if (!this.Actor.BodyParts[num].IsMissing)
                            {
                                this.cachedNonMissingBodyRenderers.Add(new ValueTuple<MeshRenderer, int>(allBodyRenderers[i].Item1, allBodyRenderers[i].Item2));
                            }
                        }
                    }
                }
                return this.cachedNonMissingBodyRenderers;
            }
        }

        public List<ValueTuple<MeshRenderer, int>> MissingBodyRenderers
        {
            get
            {
                if (this.Actor.Spec.Actor.Texture != null)
                {
                    return null;
                }
                if (this.cachedMissingBodyRenderers == null)
                {
                    this.cachedMissingBodyRenderers = new List<ValueTuple<MeshRenderer, int>>();
                    List<ValueTuple<MeshRenderer, int, Color>> allBodyRenderers = this.AllBodyRenderers;
                    List<ValueTuple<MeshRenderer, int>> nonMissingBodyRenderers = this.NonMissingBodyRenderers;
                    for (int i = 0; i < allBodyRenderers.Count; i++)
                    {
                        if (!nonMissingBodyRenderers.Contains(new ValueTuple<MeshRenderer, int>(allBodyRenderers[i].Item1, allBodyRenderers[i].Item2)))
                        {
                            this.cachedMissingBodyRenderers.Add(new ValueTuple<MeshRenderer, int>(allBodyRenderers[i].Item1, allBodyRenderers[i].Item2));
                        }
                    }
                }
                return this.cachedMissingBodyRenderers;
            }
        }

        public override bool UpdateAppearance()
        {
            base.UpdateAppearance();
            this.LookAtPlayer(false);
            this.UpdateOverlayColor();
            return true;
        }

        public override void OnEntitySpawned()
        {
            this.CheckResolvePlayerModel();
            base.OnEntitySpawned();
            if (!(base.Entity is Actor))
            {
                Log.Error("ActorGOC spawned with a non-Actor entity.", false);
            }
            if (base.gameObject.layer != Get.ActorLayer && base.gameObject.layer != Get.PlayerLayer)
            {
                Log.Warning("Actor spawned with non-Actor layer.", false);
            }
            if (this.Actor.Spec.Actor.Texture != null)
            {
                this.actorTextureRenderer = base.GetComponent<MeshRenderer>();
            }
            this.CheckAddXrayGameObject();
            this.CheckAddBodyPartHighlightGameObject();
            this.CheckAddBlobShadow();
        }

        private void CheckAddBlobShadow()
        {
            if (this.blobShadow != null)
            {
                return;
            }
            if (ActorGOC.ActorBlobShadowPrefab == null)
            {
                ActorGOC.ActorBlobShadowPrefab = Assets.Get<GameObject>("Prefabs/Misc/ActorBlobShadow");
            }
            this.blobShadowUndoNonUniformScale = new GameObject("blobShadowUndoNonUniformScale");
            this.blobShadowUndoNonUniformScale.transform.SetParent(base.transform, false);
            this.blobShadow = Object.Instantiate<GameObject>(ActorGOC.ActorBlobShadowPrefab, Vector3.zero, Quaternion.identity, this.blobShadowUndoNonUniformScale.transform);
            ActorGOC.SetBlobShadowTransform(this.blobShadow, this.blobShadowUndoNonUniformScale, base.transform, base.transform.position.RoundToVector3Int(), this.Actor.IsNowControlledActor);
        }

        public static void SetBlobShadowTransform(GameObject blobShadow, GameObject blobShadowUndoNonUniformScale, Transform transform, Vector3Int pos, bool isNowControlledActor)
        {
            if (blobShadow == null)
            {
                return;
            }
            if (pos.InBounds())
            {
                Vector3Int vector3Int = pos;
                while (vector3Int.Below().InBounds() && Get.CellsInfo.CanProjectilesPassThrough(vector3Int.Below()))
                {
                    vector3Int = vector3Int.Below();
                }
                Vector3 vector = (isNowControlledActor ? Get.CameraPosition : transform.position);
                float num = vector.y - (float)vector3Int.y;
                if (num <= 2.4f)
                {
                    if (!blobShadow.activeSelf)
                    {
                        blobShadow.SetActive(true);
                    }
                    blobShadowUndoNonUniformScale.transform.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y, 1f / transform.localScale.z);
                    Transform transform2 = blobShadow.transform;
                    transform2.position = vector.WithY((float)vector3Int.y - 0.495f);
                    transform2.rotation = Quaternion.identity;
                    transform2.localScale = (Vector3.one + num * new Vector3(0.21f, 0.21f, 0.21f)).Multiply(transform.localScale);
                    return;
                }
                if (blobShadow.activeSelf)
                {
                    blobShadow.SetActive(false);
                    return;
                }
            }
            else if (blobShadow.activeSelf)
            {
                blobShadow.SetActive(false);
            }
        }

        public override void OnSetGameObjectAfterLoading()
        {
            this.CheckResolvePlayerModel();
            base.OnSetGameObjectAfterLoading();
            if (this.Actor.Spec.Actor.Texture != null)
            {
                this.actorTextureRenderer = base.GetComponent<MeshRenderer>();
            }
            this.CheckAddXrayGameObject();
            this.CheckAddBodyPartHighlightGameObject();
            this.CheckAddBlobShadow();
        }

        private void OnEnable()
        {
            if (this.Actor == null)
            {
                return;
            }
            if (!this.Actor.IsNowControlledActor)
            {
                if (this.Actor.ConditionsAccumulated.Lying)
                {
                    this.lyingRotation = -90f;
                }
                else
                {
                    this.lyingRotation = 0f;
                }
            }
            if (Get.CellsInfo.IsFallingAt(this.Actor.Position, this.Actor, false))
            {
                this.fallingRotationPct = 1f;
            }
            else
            {
                this.fallingRotationPct = 0f;
            }
            this.curColor = this.targetColor;
            this.curEmissionColor = this.targetEmissionColor;
            this.UpdateConditionParticleSystems(true);
            this.UpdateScaleCap(true);
            ActorGOC.SetBlobShadowTransform(this.blobShadow, this.blobShadowUndoNonUniformScale, base.transform, this.Actor.Position, this.Actor.IsNowControlledActor);
            this.scaleFactorFromConditions = this.Actor.ConditionsAccumulated.ActorScaleFactor;
            this.UpdateMissingBodyPartsActiveStatus();
        }

        public void SetOverlayColor(Color overlayColor)
        {
            if (overlayColor.a <= 0f)
            {
                this.targetColor = Color.white;
                this.targetEmissionColor = Color.black;
                return;
            }
            this.targetColor = this.OverlayColorToColor(overlayColor);
            this.targetEmissionColor = this.OverlayColorToEmissionColor(overlayColor);
        }

        private Color OverlayColorToColor(Color overlayColor)
        {
            return Color.Lerp(Color.white, overlayColor.WithAlpha(1f), overlayColor.a);
        }

        private Color OverlayColorToEmissionColor(Color overlayColor)
        {
            return Color.Lerp(Color.black, (overlayColor * 0.5f).WithAlpha(1f), overlayColor.a);
        }

        public override void OnEntityUpdate()
        {
            base.OnEntityUpdate();
            this.UpdateOffsetFromImpacts();
        }

        private void LateUpdate()
        {
            this.UpdateMissingBodyPartsMaterial();
            this.UpdateBodyPartHighlight();
            this.UpdateMovesPerTurnAnimation();
            this.UpdateLyingRotation();
            this.UpdateFallingRotation();
            this.UpdateLookAwayPct();
            this.LookAtPlayer(true);
            this.UpdateOverlayColor();
            this.UpdateConditionParticleSystems(false);
            ActorGOC.SetBlobShadowTransform(this.blobShadow, this.blobShadowUndoNonUniformScale, base.transform, this.Actor.Position, this.Actor.IsNowControlledActor);
            this.UpdateScaleCap(false);
            this.DoSleepingSound();
            this.HighlightCellIfAdjacentToPlayer();
            this.scaleFactorFromConditions = Calc.StepTowards(this.scaleFactorFromConditions, this.Actor.ConditionsAccumulated.ActorScaleFactor, Clock.DeltaTime * 0.9f);
        }

        private void UpdateScaleCap(bool instantly = false)
        {
            if (this.Actor.IsNowControlledActor)
            {
                return;
            }
            Bounds occupiedBounds = this.OccupiedBounds;
            Vector3 desiredGameObjectScaleWithoutCap = this.DesiredGameObjectScaleWithoutCap;
            bool flag = this.lyingRotation < -45f;
            float num = occupiedBounds.size.y * desiredGameObjectScaleWithoutCap.y;
            float num2 = Math.Max(0.02f, 0.046f) * base.transform.localScale.y;
            float num4;
            if (num > 1f - num2 || Get.CellsInfo.AnyStairsAt(this.Actor.Position))
            {
                float num3 = ((flag && !this.Actor.Spec.Actor.LayDirectionUp) ? this.< UpdateScaleCap > g__HorizontalAvailableSpace | 112_3() : this.< UpdateScaleCap > g__VerticalAvailableSpace | 112_2());
                num3 -= num2;
                if (num > num3)
                {
                    num4 = num3 / num;
                }
                else
                {
                    num4 = 1f;
                }
            }
            else
            {
                num4 = 1f;
            }
            if (instantly)
            {
                this.heightCapFactor = num4;
            }
            else
            {
                if (Math.Max(num4, this.lastTargetHeightCapFactor) / Math.Min(num4, this.lastTargetHeightCapFactor) > 1.2f && Clock.Time - this.lastStretchSoundTime > 0.22f && this.lyingRotation > -10f)
                {
                    this.lastStretchSoundTime = Clock.Time;
                    Get.Sound_Stretch.PlayOneShot(new Vector3?(this.Actor.Position), 1f, 1f);
                }
                this.heightCapFactor = Calc.StepTowards(this.heightCapFactor, num4, 6f * Clock.DeltaTime);
            }
            this.lastTargetHeightCapFactor = num4;
            float num5 = Math.Max(0.5f - occupiedBounds.min.x, occupiedBounds.max.x - 0.5f) * 2f * desiredGameObjectScaleWithoutCap.x;
            float num6 = 0.06f * base.transform.localScale.x;
            float num8;
            if (num5 > 1f - num6)
            {
                float num7 = ((flag && !this.Actor.Spec.Actor.LayDirectionUp) ? this.< UpdateScaleCap > g__VerticalAvailableSpace | 112_2() : this.< UpdateScaleCap > g__HorizontalAvailableSpace | 112_3());
                num7 -= num6;
                if (num5 > num7)
                {
                    num8 = num7 / num5;
                }
                else
                {
                    num8 = 1f;
                }
            }
            else
            {
                num8 = 1f;
            }
            if (instantly)
            {
                this.widthCapFactor = num8;
                return;
            }
            this.widthCapFactor = Calc.StepTowards(this.widthCapFactor, num8, 5f * Clock.DeltaTime);
        }

        public void UpdateMissingBodyPartsActiveStatus()
        {
            if (this.Actor.Spec.Actor.Texture != null)
            {
                return;
            }
            if (this.Actor.IsNowControlledActor && !Get.FPPControllerGOC.CameraSwitchingBetweenActorsNow)
            {
                List<ValueTuple<MeshRenderer, int, Color>> allBodyRenderers = this.AllBodyRenderers;
                for (int i = 0; i < allBodyRenderers.Count; i++)
                {
                    allBodyRenderers[i].Item1.gameObject.SetActive(false);
                }
                return;
            }
            List<ValueTuple<MeshRenderer, int>> missingBodyRenderers = this.MissingBodyRenderers;
            for (int j = 0; j < missingBodyRenderers.Count; j++)
            {
                missingBodyRenderers[j].Item1.gameObject.SetActive(false);
            }
            List<ValueTuple<MeshRenderer, int>> nonMissingBodyRenderers = this.NonMissingBodyRenderers;
            for (int k = 0; k < nonMissingBodyRenderers.Count; k++)
            {
                nonMissingBodyRenderers[k].Item1.gameObject.SetActive(true);
            }
        }

        private void DoSleepingSound()
        {
            if (!this.Actor.IsNowControlledActor && (base.transform.position - Get.CameraPosition).sqrMagnitude < 6.3001f && this.Actor.ConditionsAccumulated.Lying)
            {
                if (this.BreathingAnimationRaw >= 0.69f)
                {
                    this.canPlaySleepingSound = true;
                    return;
                }
                if (this.canPlaySleepingSound)
                {
                    float num = Rand.RangeSeeded(0.9f, 1.1f, Calc.CombineHashes<int, int>(this.Actor.MyStableHash, 831537990)) * EntitySoundUtility.GetPitchFromScale(this.Actor);
                    Get.Sound_Sleeping.PlayOneShot(new Vector3?(base.transform.position), 1f, num);
                    this.canPlaySleepingSound = false;
                    return;
                }
            }
            else
            {
                this.canPlaySleepingSound = false;
            }
        }

        private void DrawXrayMeshes()
        {
            if (ActorGOC.ActorModelXray == null)
            {
                ActorGOC.ActorModelXray = Assets.Get<Material>("Materials/Actors/ActorModelXray");
            }
            if (this.Actor.IsNowControlledActor)
            {
                return;
            }
            if (this.lyingRotation < -45f)
            {
                return;
            }
            List<ValueTuple<MeshRenderer, int>> nonMissingBodyRenderers = this.NonMissingBodyRenderers;
            Vector3 forward = base.transform.forward;
            float num = 0f;
            for (int i = 0; i < nonMissingBodyRenderers.Count; i++)
            {
                MeshRenderer item = nonMissingBodyRenderers[i].Item1;
                if (!ActorGOC.< DrawXrayMeshes > g__ShouldSkip | 115_0(item))
                {
                    float num2 = item.transform.lossyScale.z * (item.localBounds.extents.z + 0.005f + 1E-05f) + 0.05f;
                    num = Math.Max(num, num2);
                    num = Math.Max(num, num2 - item.transform.localPosition.z);
                }
            }
            for (int j = 0; j < nonMissingBodyRenderers.Count; j++)
            {
                MeshRenderer item2 = nonMissingBodyRenderers[j].Item1;
                if (!ActorGOC.< DrawXrayMeshes > g__ShouldSkip | 115_0(item2))
                {
                    Vector3 vector = item2.transform.position;
                    Quaternion rotation = item2.transform.rotation;
                    Vector3 lossyScale = item2.transform.lossyScale;
                    lossyScale.z *= 0.01f;
                    vector -= forward * num + item2.transform.localPosition.z * forward;
                    Graphics.DrawMesh(item2.GetMeshFilter().sharedMesh, Matrix4x4.TRS(vector, rotation, lossyScale), ActorGOC.ActorModelXray, 0, null, 0, null, ShadowCastingMode.Off, false);
                }
            }
        }

        private void HighlightCellIfAdjacentToPlayer()
        {
            if (this.Actor.IsNowControlledActor || !Get.NowControlledActor.Spawned)
            {
                return;
            }
            if (!this.Actor.Position.IsAdjacentOrInside(Get.NowControlledActor.Position))
            {
                return;
            }
            if (!this.Actor.IsHostile(Get.NowControlledActor))
            {
                return;
            }
            if (!Get.NowControlledActor.Sees(this.Actor, null))
            {
                return;
            }
            if (Get.CellsInfo.IsFloorUnderNoActors(this.Actor.Position))
            {
                Get.CellHighlighter.HighlightCell(this.Actor.Position, CellHighlighter.BrightRed);
            }
        }

        private void UpdateBodyPartHighlight()
        {
            if (this.bodyPartTextureHighlightRenderer != null && this.lastHighlightBodyPartFrame < Clock.Frame - 1)
            {
                this.bodyPartTextureHighlightRenderer.SetActive(false);
            }
        }

        private void UpdateMovesPerTurnAnimation()
        {
            if (!this.Actor.IsNowControlledActor && Get.TurnManager.IsPlayerTurn)
            {
                if (!AIUtility.WillMakeMoveBeforeMe(this.Actor, Get.NowControlledActor))
                {
                    this.movesPerTurnAnimation = ActorGOC.MovesPerTurnAnimation.WontMoveThisTurn;
                    return;
                }
                if (AIUtility.WillMakeAtLeastTwoMovesBeforeMe(this.Actor, Get.NowControlledActor))
                {
                    this.movesPerTurnAnimation = ActorGOC.MovesPerTurnAnimation.WillMoveAtLeastTwice;
                    return;
                }
                this.movesPerTurnAnimation = ActorGOC.MovesPerTurnAnimation.Normal;
            }
        }

        private void UpdateLyingRotation()
        {
            if (this.Actor.IsNowControlledActor)
            {
                return;
            }
            if (this.Actor.ConditionsAccumulated.Lying)
            {
                this.lyingRotation = Calc.StepTowards(this.lyingRotation, -90f, Clock.DeltaTime * 350f);
                return;
            }
            if (this.lyingRotation == -90f)
            {
                this.lastWakeUpTime = Clock.Time;
            }
            this.lyingRotation = Calc.StepTowards(this.lyingRotation, 0f, Clock.DeltaTime * 350f);
        }

        private void UpdateFallingRotation()
        {
            if (Get.CellsInfo.IsFallingAt(this.Actor.Position, this.Actor, false))
            {
                this.fallingRotationPct = Calc.StepTowards(this.fallingRotationPct, 1f, Clock.DeltaTime * 10f);
                return;
            }
            this.fallingRotationPct = Calc.StepTowards(this.fallingRotationPct, 0f, Clock.DeltaTime * 10f);
        }

        private void UpdateLookAwayPct()
        {
            if (this.Actor.IsNowControlledActor)
            {
                return;
            }
            if (Get.NowControlledActor.Spawned && Get.NowControlledActor.Sees(this.Actor, null))
            {
                if (this.Actor.Sees(Get.NowControlledActor, null))
                {
                    this.lookAwayPct = Math.Max(this.lookAwayPct - Clock.DeltaTime * 1.7f, 0f);
                    return;
                }
                this.lookAwayPct = Math.Min(this.lookAwayPct + Clock.DeltaTime * 1.7f, 1f);
            }
        }

        private void LookAtPlayer(bool canPlayStepSound)
        {
            if (this.Actor.IsNowControlledActor)
            {
                if (!Get.CameraEffects.WalkingStaircase)
                {
                    float num = 0.724f * base.transform.localScale.y;
                    bool flag = false;
                    foreach (BodyPart bodyPart in this.Actor.BodyParts)
                    {
                        if (bodyPart.Spec == Get.BodyPart_Leg && !bodyPart.IsMissing)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        num *= 0.72f;
                    }
                    if (num > 0.84000003f || Get.CellsInfo.AnyStairsAt(this.Actor.Position))
                    {
                        float num2 = 1f;
                        if (ActorGOC.< LookAtPlayer > g__IsFree | 122_0(this.Actor.Position.Above()))
                        {
                            num2 = 2f;
                            if (ActorGOC.< LookAtPlayer > g__IsFree | 122_0(this.Actor.Position.Above().Above()))
                            {
                                num2 = 3f;
                            }
                        }
                        if (Get.CellsInfo.AnyStairsAt(this.Actor.Position))
                        {
                            num2 -= Get.CellsInfo.OffsetFromStairsAt(this.Actor.Position);
                        }
                        num2 -= 0.16f;
                        num = Math.Min(num, num2);
                    }
                    Vector3 vector = this.DesiredGameObjectPosition.WithAddedY(-0.5f + num);
                    Get.FPPControllerGOC.TargetPosition = vector;
                    return;
                }
            }
            else
            {
                float num3;
                float num4;
                ActorGOC.GetLookAtPlayerRot(base.transform.position, base.transform.localScale, out num3, out num4);
                num3 = Mathf.MoveTowardsAngle(num3, 0f, 180f * Mathf.Clamp01(Mathf.Abs(this.lyingRotation / 90f)));
                num4 = Mathf.MoveTowardsAngle(num4, 0f, 180f * Mathf.Clamp01(Mathf.Abs(this.lyingRotation / 90f)));
                float rotation = DanceAnimationUtility.GetRotation(this.Actor);
                float num5 = Calc.StepTowards(this.lastDanceRot, rotation, Clock.DeltaTime * 25f);
                this.lastDanceRot = num5;
                float num6 = num4;
                if (this.ShouldUseArmlessAttackAnimation())
                {
                    num6 += StrikeAnimationUtility.GetArmlessAnimationRotation(this.strikeAnimationStartTime, this.strikeAnimationDir);
                }
                else
                {
                    num6 += StrikeAnimationUtility.GetRotOffset(this.strikeAnimationStartTime, this.strikeAnimationDir);
                }
                float num7;
                float num8;
                if (this.Actor.Spec.Actor.LayDirectionUp)
                {
                    num7 = 0f;
                    num8 = 2f * this.lyingRotation;
                }
                else
                {
                    num7 = this.lyingRotation;
                    num8 = this.lyingRotation;
                }
                Quaternion quaternion = Quaternion.Euler(num6 - num7 - this.fallingRotationPct * 22f + this.offsetFromImpacts.magnitude * 25f, num3 - this.lookAwayPct * (30f + Calc.Sin(Clock.Time) * 10f) + this.offsetFromImpacts.magnitude * 40f * (this.rotationOffsetFromImpactSign ? 1f : (-1f)), num5 + num8 + this.fallingRotationPct * 13f);
                base.transform.rotation = quaternion;
                if ((rotation == 2f || rotation == -2f) && rotation != this.lastActorAnimationStepSoundRot && canPlayStepSound)
                {
                    this.lastActorAnimationStepSoundRot = rotation;
                    Get.Sound_ActorAnimationStep.PlayOneShot(new Vector3?(this.Actor.Position), 1f, 1f);
                }
            }
        }

        public static void GetLookAtPlayerRot(Vector3 position, Vector3 scale, out float dirDeg, out float tiltDeg)
        {
            Vector3 cameraPosition = Get.CameraPosition;
            float num = Calc.Atan2(cameraPosition.z - position.z, cameraPosition.x - position.x);
            dirDeg = -num * 57.29578f - 90f;
            Vector3 vector = cameraPosition - position;
            float num2 = Calc.Sqrt(vector.x * vector.x + vector.z * vector.z);
            float num3 = Calc.Atan(vector.y / num2);
            float num4 = 25f / Math.Max(Math.Max(scale.x, scale.y), 1f);
            tiltDeg = Calc.Clamp(num3 * 57.29578f, -num4, num4);
            float num5 = Math.Abs(cameraPosition.y - position.y);
            tiltDeg *= Calc.LerpDouble(0.5f, 0.9f, 0f, 1f, num5);
        }

        private void UpdateOverlayColor()
        {
            Color? actorOverlayColor = this.Actor.ConditionsAccumulated.ActorOverlayColor;
            if (actorOverlayColor != null)
            {
                this.SetOverlayColor(actorOverlayColor.Value);
            }
            else if (this.movesPerTurnAnimation == ActorGOC.MovesPerTurnAnimation.WontMoveThisTurn)
            {
                this.SetOverlayColor(new Color(0.3f, 0.3f, 0.3f, 0.85f));
            }
            else
            {
                this.SetOverlayColor(Color.clear);
            }
            this.curColor = Ricave.UI.ColorUtility.HSVMoveTowards(this.curColor, this.targetColor, Clock.DeltaTime * 3f);
            this.curEmissionColor = Ricave.UI.ColorUtility.HSVMoveTowards(this.curEmissionColor, this.targetEmissionColor, Clock.DeltaTime * 0.66f);
            this.ApplyOverlayColorToMaterial(true);
        }

        private void UpdateMissingBodyPartsMaterial()
        {
            if (ActorGOC.ActorWithMissingBodyParts == null)
            {
                ActorGOC.ActorWithMissingBodyParts = Assets.Get<Material>("Materials/Actors/ActorWithMissingBodyParts");
            }
            if (ActorGOC.ActorWithMissingBodyPartsXray == null)
            {
                ActorGOC.ActorWithMissingBodyPartsXray = Assets.Get<Material>("Materials/Actors/ActorWithMissingBodyPartsXray");
            }
            if (this.actorTextureRenderer == null)
            {
                return;
            }
            bool flag = false;
            List<BodyPart> bodyParts = this.Actor.BodyParts;
            for (int i = 0; i < bodyParts.Count; i++)
            {
                if (bodyParts[i].IsMissing)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag && !this.everChangedMaterial)
            {
                return;
            }
            this.everChangedMaterial = true;
            if (!flag)
            {
                if (this.actorTextureRenderer.material.shader == ActorGOC.ActorWithMissingBodyParts.shader)
                {
                    this.actorTextureRenderer.material = this.originalMaterial;
                }
                if (this.xrayRenderer != null && this.xrayRenderer.material.shader == ActorGOC.ActorWithMissingBodyParts.shader)
                {
                    this.xrayRenderer.material = this.originalXrayMaterial;
                    return;
                }
            }
            else
            {
                if (this.actorTextureRenderer.material.shader != ActorGOC.ActorWithMissingBodyParts.shader)
                {
                    this.originalMaterial = this.actorTextureRenderer.material;
                    this.actorTextureRenderer.material = ActorGOC.ActorWithMissingBodyParts;
                    this.actorTextureRenderer.material.mainTexture = this.originalMaterial.mainTexture;
                    this.actorTextureRenderer.material.SetTexture(Get.ShaderPropertyIDs.BodyMapID, this.Actor.Spec.Actor.BodyMap);
                }
                if (this.xrayRenderer != null && this.xrayRenderer.material.shader != ActorGOC.ActorWithMissingBodyPartsXray.shader)
                {
                    this.originalXrayMaterial = this.xrayRenderer.material;
                    this.xrayRenderer.material = ActorGOC.ActorWithMissingBodyPartsXray;
                    this.xrayRenderer.material.mainTexture = this.originalXrayMaterial.mainTexture;
                    this.xrayRenderer.material.SetTexture(Get.ShaderPropertyIDs.BodyMapID, this.Actor.Spec.Actor.BodyMap);
                }
                Material material = this.actorTextureRenderer.material;
                for (int j = 0; j < bodyParts.Count; j++)
                {
                    float num = (bodyParts[j].IsMissing ? 0f : 1f);
                    material.SetFloat(Get.ShaderPropertyIDs.PartXExistsID[j], num);
                    if (this.xrayRenderer != null)
                    {
                        this.xrayRenderer.material.SetFloat(Get.ShaderPropertyIDs.PartXExistsID[j], num);
                    }
                }
            }
        }

        private void ApplyOverlayColorToMaterial(bool canIncludeUIEffects)
        {
            List<ValueTuple<MeshRenderer, int, Color>> allBodyRenderers = this.AllBodyRenderers;
            if (allBodyRenderers.NullOrEmpty<ValueTuple<MeshRenderer, int, Color>>() && this.actorTextureRenderer == null)
            {
                return;
            }
            Color color = this.curColor;
            Color color2 = this.curEmissionColor;
            if (canIncludeUIEffects)
            {
                if (this.lastMarkedForAffectedByAoEHighlightFrame >= Clock.Frame - 1)
                {
                    float num = Calc.Pulse(6f, 1f);
                    Color color3 = Color.Lerp(new Color(0.5f, 0f, 0f, 0.6f), new Color(1f, 0.3f, 0.3f, 0.6f), num);
                    color = this.OverlayColorToColor(color3);
                    color2 = this.OverlayColorToEmissionColor(color3);
                }
                else
                {
                    float num2 = Calc.ResolveFadeInStayOut(Clock.Time - this.lastTimeLostHP, 0.03f, 0.02f, 0.23f);
                    Color color4 = new Color(1f, 0.15f, 0.15f, 1f);
                    color = Color.Lerp(color, this.OverlayColorToColor(color4), num2);
                    color2 = Color.Lerp(color2, this.OverlayColorToEmissionColor(color4), num2);
                }
            }
            if (color == Color.white && color2 == Color.black && !this.everChangedMaterial && this.lastHighlightBodyPart == null)
            {
                return;
            }
            if (this.actorTextureRenderer != null)
            {
                Material material = this.actorTextureRenderer.material;
                material.color = color;
                material.SetColor(Get.ShaderPropertyIDs.EmissionColorID, color2);
            }
            else
            {
                for (int i = 0; i < allBodyRenderers.Count; i++)
                {
                    Material material2 = allBodyRenderers[i].Item1.material;
                    Color item = allBodyRenderers[i].Item3;
                    if (this.lastHighlightBodyPart != null && this.lastHighlightBodyPartFrame >= Clock.Frame - 1 && allBodyRenderers[i].Item2 == this.lastHighlightBodyPart.PlacementIndex + 1 && canIncludeUIEffects)
                    {
                        float num3 = Calc.Pulse(6f, 1f);
                        Color color5 = new Color(1f, 0.7f, 0.3f, Calc.Lerp(0.5f, 1f, num3));
                        material2.color = this.OverlayColorToColor(color5) * item;
                        material2.SetColor(Get.ShaderPropertyIDs.EmissionColorID, this.OverlayColorToEmissionColor(color5));
                    }
                    else
                    {
                        material2.color = color * item;
                        material2.SetColor(Get.ShaderPropertyIDs.EmissionColorID, color2);
                    }
                }
            }
            this.everChangedMaterial = true;
        }

        public void OnPreShatter()
        {
            EyesGOC componentInChildren = base.GetComponentInChildren<EyesGOC>();
            if (componentInChildren != null)
            {
                componentInChildren.OnPreShatter();
            }
            this.ApplyOverlayColorToMaterial(false);
            if (this.bodyPartTextureHighlightRenderer != null)
            {
                this.bodyPartTextureHighlightRenderer.SetActive(false);
            }
            this.UpdateMissingBodyPartsMaterial();
        }

        public void TeleportToDestOnDestroyed()
        {
            base.gameObject.transform.position = this.DesiredGameObjectPosition;
        }

        public void OnPreFadeOut()
        {
            if (this.bodyPartTextureHighlightRenderer != null)
            {
                this.bodyPartTextureHighlightRenderer.SetActive(false);
            }
        }

        public void MarkForAffectedByAoEHighlight()
        {
            this.lastMarkedForAffectedByAoEHighlightFrame = Clock.Frame;
        }

        private void CheckResolvePlayerModel()
        {
            PlayerModelPlaceholderGOC componentInChildren = base.GetComponentInChildren<PlayerModelPlaceholderGOC>();
            if (componentInChildren != null)
            {
                componentInChildren.ResolveModel();
                this.hasPlayerModel = true;
                this.cachedAllBodyRenderers = null;
                this.cachedNonMissingBodyRenderers = null;
                this.cachedMissingBodyRenderers = null;
                this.occupiedBoundsCached = null;
            }
        }

        private void CheckAddXrayGameObject()
        {
            if (ActorGOC.ActorXrayPrefab == null)
            {
                ActorGOC.ActorXrayPrefab = Assets.Get<GameObject>("Prefabs/Actors/ActorXray");
            }
            if (this.xrayRenderer == null && this.actorTextureRenderer != null)
            {
                GameObject gameObject = Object.Instantiate<GameObject>(ActorGOC.ActorXrayPrefab);
                this.xrayRenderer = gameObject.GetComponent<Renderer>();
                this.xrayRenderer.material.mainTexture = this.Actor.Spec.Actor.Texture;
                gameObject.transform.SetParent(base.transform, false);
            }
        }

        private void CheckAddBodyPartHighlightGameObject()
        {
            if (ActorGOC.BodyPartHighlightPrefab == null)
            {
                ActorGOC.BodyPartHighlightPrefab = Assets.Get<GameObject>("Prefabs/Actors/BodyPartHighlight");
            }
            if (this.bodyPartTextureHighlightRenderer == null && this.Actor.Spec.Actor.BodyMap != null)
            {
                this.bodyPartTextureHighlightRenderer = Object.Instantiate<GameObject>(ActorGOC.BodyPartHighlightPrefab);
                this.bodyPartTextureHighlightMaterial = this.bodyPartTextureHighlightRenderer.GetComponent<Renderer>().material;
                this.bodyPartTextureHighlightMaterial.mainTexture = this.Actor.Spec.Actor.BodyMap;
                this.bodyPartTextureHighlightRenderer.transform.SetParent(base.transform, false);
                Vector3 localPosition = this.bodyPartTextureHighlightRenderer.transform.localPosition;
                localPosition.z -= 0.001f;
                this.bodyPartTextureHighlightRenderer.transform.localPosition = localPosition;
                this.bodyPartTextureHighlightRenderer.SetActive(false);
            }
        }

        public void HighlightBodyPart(BodyPart bodyPart)
        {
            if (this.Actor.Spec.Actor.Texture == null)
            {
                this.lastHighlightBodyPartFrame = Clock.Frame;
                this.lastHighlightBodyPart = bodyPart;
                return;
            }
            if (this.bodyPartTextureHighlightRenderer == null)
            {
                return;
            }
            if (bodyPart == null || bodyPart.IsMissing)
            {
                this.bodyPartTextureHighlightRenderer.SetActive(false);
                return;
            }
            Color bodyPartBodyMapColor = BodyPartUtility.GetBodyPartBodyMapColor(bodyPart);
            this.bodyPartTextureHighlightMaterial.color = bodyPartBodyMapColor;
            this.bodyPartTextureHighlightRenderer.SetActive(true);
            this.lastHighlightBodyPartFrame = Clock.Frame;
        }

        public void StartStrikeAnimation(Vector3Int target)
        {
            this.strikeAnimationStartTime = Clock.Time;
            this.strikeAnimationDir = (target - this.Actor.Position).normalized;
            if (this.Actor != Get.NowControlledActor)
            {
                this.strikeAnimationExtraDistFactor = (target.IsAdjacentDiagonal(this.Actor.Position) ? Calc.Sqrt2 : 1f);
                return;
            }
            if (this.Actor.ChargedAttack > 0 && !Get.World.AnyEntityOfSpecAt(target, Get.Entity_Door))
            {
                this.strikeAnimationExtraDistFactor = 1f;
                return;
            }
            this.strikeAnimationExtraDistFactor = 0.15f;
        }

        public void AddOffsetFromImpact(Vector3Int impactSource, bool powerful)
        {
            this.lastOffsetFromImpactWasPowerful = powerful;
            impactSource.y = this.Actor.Position.y;
            if (impactSource == this.Actor.Position)
            {
                return;
            }
            float num = (this.Actor.IsNowControlledActor ? 0.1f : (0.4f * Rand.Range(0.98f, 1.02f)));
            Vector3 normalized = (this.Actor.Position - impactSource).normalized;
            this.offsetFromImpacts += normalized * num;
            this.rotationOffsetFromImpactSign = Rand.Bool;
            float num2 = num * 1.2f;
            if (this.offsetFromImpacts.sqrMagnitude > num2 * num2)
            {
                this.offsetFromImpacts = this.offsetFromImpacts.normalized * num2;
            }
        }

        public void OnLostHP()
        {
            this.lastTimeLostHP = Clock.Time;
        }

        private void UpdateOffsetFromImpacts()
        {
            float num = (this.Actor.IsNowControlledActor ? 0.35f : (this.lastOffsetFromImpactWasPowerful ? 1.2f : 1.5f));
            this.offsetFromImpacts = Vector3.MoveTowards(this.offsetFromImpacts, Vector3.zero, Clock.DeltaTime * num);
        }

        private void UpdateConditionParticleSystems(bool destroyInstantly)
        {
            bool isNowControlledActor = this.Actor.IsNowControlledActor;
            List<Condition> allConditions = this.Actor.ConditionsAccumulated.AllConditions;
            for (int i = 0; i < allConditions.Count; i++)
            {
                Condition condition = allConditions[i];
                if (!(condition.Spec.ParticleSystem == null) && (!isNowControlledActor || condition.Spec.ShowParticleSystemEvenOnPlayer))
                {
                    bool flag = false;
                    for (int j = 0; j < this.conditionParticleSystems.Count; j++)
                    {
                        if (this.conditionParticleSystems[j].Item1 == condition)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        GameObject gameObject = new GameObject("Condition particle system container");
                        gameObject.transform.SetParent(base.transform, false);
                        Transform transform = Object.Instantiate<GameObject>(condition.Spec.ParticleSystem, Vector3.zero, Quaternion.identity, gameObject.transform).transform;
                        transform.localPosition = (isNowControlledActor ? condition.Spec.ParticleSystemPlayerOffset : Vector3.zero);
                        transform.localRotation = Quaternion.identity;
                        transform.localScale = Vector3.one;
                        this.conditionParticleSystems.Add(new ValueTuple<Condition, GameObject>(condition, gameObject));
                    }
                }
            }
            for (int k = this.conditionParticleSystems.Count - 1; k >= 0; k--)
            {
                ValueTuple<Condition, GameObject> valueTuple = this.conditionParticleSystems[k];
                bool flag2 = allConditions.Contains(valueTuple.Item1);
                if (flag2 && isNowControlledActor && !valueTuple.Item1.Spec.ShowParticleSystemEvenOnPlayer)
                {
                    flag2 = false;
                }
                if (!flag2)
                {
                    if (destroyInstantly)
                    {
                        Object.Destroy(valueTuple.Item2);
                    }
                    else
                    {
                        Get.ParticleSystemFinisher.DetachAndFinishParticles(valueTuple.Item2);
                        Object.Destroy(valueTuple.Item2);
                    }
                    this.conditionParticleSystems.RemoveAt(k);
                }
            }
            GenericUsable usableOnTalk = this.Actor.UsableOnTalk;
            UseEffects useEffects = ((usableOnTalk != null) ? usableOnTalk.UseEffects : null);
            if (useEffects != null)
            {
                UseEffect_ShowDialogue useEffect_ShowDialogue = useEffects.GetFirstOfSpec(Get.UseEffect_ShowDialogue) as UseEffect_ShowDialogue;
                if (useEffect_ShowDialogue != null && !useEffect_ShowDialogue.DialogueSpec.Ended && !isNowControlledActor && !this.Actor.IsHostile(Get.NowControlledActor))
                {
                    if (this.talkParticleSystem == null)
                    {
                        if (ActorGOC.DialogueParticlesPrefab == null)
                        {
                            ActorGOC.DialogueParticlesPrefab = Assets.Get<GameObject>("Prefabs/ParticleSystems/DialogueParticles");
                        }
                        GameObject gameObject2 = new GameObject("Talk particle system container");
                        gameObject2.transform.SetParent(base.transform, false);
                        Transform transform2 = Object.Instantiate<GameObject>(ActorGOC.DialogueParticlesPrefab, Vector3.zero, Quaternion.identity, gameObject2.transform).transform;
                        transform2.localPosition = Vector3.zero;
                        transform2.localRotation = Quaternion.identity;
                        transform2.localScale = Vector3.one;
                        this.talkParticleSystem = gameObject2;
                        goto IL_02FE;
                    }
                    goto IL_02FE;
                }
            }
            if (this.talkParticleSystem != null)
            {
                if (destroyInstantly)
                {
                    Object.Destroy(this.talkParticleSystem);
                }
                else
                {
                    Get.ParticleSystemFinisher.DetachAndFinishParticles(this.talkParticleSystem);
                    Object.Destroy(this.talkParticleSystem);
                }
            }
        IL_02FE:
            this.CheckCreateDestroyMoveParticles();
        }

        private void CheckCreateDestroyMoveParticles()
        {
            if (Clock.Time - this.lastMoveTime < 0.3f && !this.Actor.CanFly && !this.Actor.IsNowControlledActor && !this.Actor.ConditionsAccumulated.Lying && Get.CellsInfo.IsFloorUnderNoActors(this.Actor.Position))
            {
                if (this.moveParticleSystem == null)
                {
                    if (ActorGOC.MoveParticlesPrefab == null)
                    {
                        ActorGOC.MoveParticlesPrefab = Assets.Get<GameObject>("Prefabs/ParticleSystems/Move");
                    }
                    GameObject gameObject = new GameObject("Move particle system container");
                    gameObject.transform.SetParent(base.transform, false);
                    Transform transform = Object.Instantiate<GameObject>(ActorGOC.MoveParticlesPrefab, Vector3.zero, Quaternion.identity, gameObject.transform).transform;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    transform.localScale = Vector3.one;
                    this.moveParticleSystem = gameObject;
                    this.moveParticleSystemPs = this.moveParticleSystem.GetComponentInChildren<ParticleSystem>();
                }
                else if (!this.moveParticleSystemPs.isPlaying)
                {
                    this.moveParticleSystemPs.Play();
                }
                this.moveParticleSystem.transform.position = base.transform.position.WithY(base.BoundingBox.min.y + 0.03f);
                return;
            }
            if (this.moveParticleSystem != null)
            {
                this.moveParticleSystemPs.Stop();
            }
        }

        private float GetStepAnimation()
        {
            if (!this.Actor.IsNowControlledActor && this.Actor.CanFly)
            {
                return 0f;
            }
            if (this.Actor.Limping)
            {
                return 0f;
            }
            if (Get.CellsInfo.IsFallingAt(this.Actor.Position, this.Actor.Gravity, false, this.Actor.CanUseLadders, false))
            {
                return 0f;
            }
            Vector3 vector = (this.Actor.IsNowControlledActor ? Get.CameraPosition : base.transform.position);
            float num = Math.Max(Math.Abs(vector.x - (float)Calc.RoundToInt(vector.x)), Math.Abs(vector.z - (float)Calc.RoundToInt(vector.z))) * 2f;
            if (!this.Actor.IsNowControlledActor)
            {
                num = -num * 1.5f;
            }
            float num2 = (this.Actor.IsNowControlledActor ? 0.05f : 0.065f);
            return -num * num2 * base.transform.localScale.y;
        }

        private float GetLimpAnimation()
        {
            if (!this.Actor.Limping)
            {
                return 0f;
            }
            if (this.Actor.CanFly)
            {
                return 0f;
            }
            Vector3 position = base.transform.position;
            return Math.Max(Math.Abs(position.x - (float)Calc.RoundToInt(position.x)), Math.Abs(position.z - (float)Calc.RoundToInt(position.z))) * 2f * 0.23f * base.transform.localScale.y;
        }

        private float GetOnGroundOrFlyingOffset()
        {
            if (this.Actor.ConditionsAccumulated.Lying || this.Actor.IsNowControlledActor)
            {
                return 0f;
            }
            if (this.Actor.CanFly)
            {
                return this.FlyingOffset;
            }
            return this.OnGroundOffset;
        }

        private float GetCurrentLyingOffset()
        {
            if (!this.Actor.ConditionsAccumulated.Lying)
            {
                return 0f;
            }
            if (this.Actor.IsNowControlledActor)
            {
                return -0.25f * base.transform.localScale.y;
            }
            return this.LyingOffset;
        }

        private bool ShouldUseArmlessAttackAnimation()
        {
            if (this.Actor.IsNowControlledActor)
            {
                return false;
            }
            if (this.Actor.HasArm)
            {
                return false;
            }
            bool flag = false;
            List<BodyPart> bodyParts = this.Actor.BodyParts;
            for (int i = 0; i < bodyParts.Count; i++)
            {
                if (bodyParts[i].Spec.IsArm)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public Vector3 GetBodyPartCenterWorldPos(BodyPart bodyPart)
        {
            if (!(this.Actor.Spec.Actor.Texture == null))
            {
                return base.transform.TransformPoint(bodyPart.Placement.OccupiedTextureRect.MovedBy(-0.5f, -0.5f).center);
            }
            List<ValueTuple<MeshRenderer, int, Color>> allBodyRenderers = this.AllBodyRenderers;
            if (allBodyRenderers.NullOrEmpty<ValueTuple<MeshRenderer, int, Color>>())
            {
                return base.transform.position;
            }
            Bounds? bounds = null;
            for (int i = 0; i < allBodyRenderers.Count; i++)
            {
                if (allBodyRenderers[i].Item2 == bodyPart.PlacementIndex + 1)
                {
                    if (bounds == null)
                    {
                        bounds = new Bounds?(allBodyRenderers[i].Item1.bounds);
                    }
                    else
                    {
                        Bounds value = bounds.Value;
                        value.Encapsulate(allBodyRenderers[i].Item1.bounds);
                        bounds = new Bounds?(value);
                    }
                }
            }
            if (bounds == null)
            {
                return base.transform.position;
            }
            return bounds.Value.center;
        }

        public Vector3 GetBodyPartOverlayWorldPos(BodyPart bodyPart)
        {
            return base.transform.TransformPoint(bodyPart.Placement.Offset);
        }

        public void OnBodyPartChanged()
        {
            this.occupiedBoundsCached = null;
            this.cachedNonMissingBodyRenderers = null;
            this.cachedMissingBodyRenderers = null;
            this.UpdateMissingBodyPartsActiveStatus();
        }

        public BodyPart GetBodyPart(GameObject bodyPartGameObject, bool allowMissing)
        {
            if (bodyPartGameObject == null)
            {
                return null;
            }
            List<ValueTuple<MeshRenderer, int, Color>> allBodyRenderers = this.AllBodyRenderers;
            if (allBodyRenderers.NullOrEmpty<ValueTuple<MeshRenderer, int, Color>>())
            {
                return null;
            }
            int i = 0;
            while (i < allBodyRenderers.Count)
            {
                if (!(allBodyRenderers[i].Item1.gameObject != bodyPartGameObject))
                {
                    int num = allBodyRenderers[i].Item2;
                    if (num == 0)
                    {
                        return null;
                    }
                    num--;
                    if (num < 0 || num >= this.Actor.BodyParts.Count)
                    {
                        Log.Error(string.Concat(new string[]
                        {
                            "Body part index of a body part game object out of bounds (",
                            num.ToString(),
                            ") for actor ",
                            this.Actor.ToStringSafe(),
                            ". This means that the model is wrong."
                        }), false);
                        return null;
                    }
                    BodyPart bodyPart = this.Actor.BodyParts[num];
                    if (bodyPart.IsMissing && !allowMissing)
                    {
                        return null;
                    }
                    return bodyPart;
                }
                else
                {
                    i++;
                }
            }
            return null;
        }

        public override void OnEntityChangedPosition(Vector3Int prevPos)
        {
            base.OnEntityChangedPosition(prevPos);
            this.lastMoveTime = Clock.Time;
            this.CheckCreateDestroyMoveParticles();
        }

        public void OnBecameNowControlledActor()
        {
            this.UpdateMissingBodyPartsActiveStatus();
        }

        public void OnNoLongerNowControlledActor()
        {
            this.UpdateMissingBodyPartsActiveStatus();
        }

        [CompilerGenerated]
        internal static bool <UpdateScaleCap>g__IsFree|112_0(Vector3Int c)
		{
			return c.InBounds() && Get.CellsInfo.CanPassThroughNoActors(c);
		}

    [CompilerGenerated]
    internal static bool <UpdateScaleCap>g__IsFree_Vertically|112_1(Vector3Int c)
		{
			return c.InBounds() && Get.CellsInfo.CanPassThrough(c);
		}

[CompilerGenerated]
private float < UpdateScaleCap > g__VerticalAvailableSpace | 112_2()

        {
    float num = 1f;
    if (ActorGOC.< UpdateScaleCap > g__IsFree_Vertically | 112_1(this.Actor.Position.Above()))
    {
        num = 2f;
        if (ActorGOC.< UpdateScaleCap > g__IsFree_Vertically | 112_1(this.Actor.Position.Above().Above()))
        {
            num = 3f;
        }
    }
    if (Get.CellsInfo.AnyStairsAt(this.Actor.Position))
    {
        num -= Get.CellsInfo.OffsetFromStairsAt(this.Actor.Position);
    }
    return num;
}

[CompilerGenerated]
private float < UpdateScaleCap > g__HorizontalAvailableSpace | 112_3()

        {
    float num = 1f;
    if (ActorGOC.< UpdateScaleCap > g__IsFree | 112_0(this.Actor.Position + Vector3IntUtility.DirectionsXZCardinal[0]) && ActorGOC.< UpdateScaleCap > g__IsFree | 112_0(this.Actor.Position + Vector3IntUtility.DirectionsXZCardinal[1]) && ActorGOC.< UpdateScaleCap > g__IsFree | 112_0(this.Actor.Position + Vector3IntUtility.DirectionsXZCardinal[2]) && ActorGOC.< UpdateScaleCap > g__IsFree | 112_0(this.Actor.Position + Vector3IntUtility.DirectionsXZCardinal[3]))
    {
        num = 1.4142f;
        if (ActorGOC.< UpdateScaleCap > g__IsFree | 112_0(this.Actor.Position + Vector3IntUtility.DirectionsXZDiagonal[0]) && ActorGOC.< UpdateScaleCap > g__IsFree | 112_0(this.Actor.Position + Vector3IntUtility.DirectionsXZDiagonal[1]) && ActorGOC.< UpdateScaleCap > g__IsFree | 112_0(this.Actor.Position + Vector3IntUtility.DirectionsXZDiagonal[2]) && ActorGOC.< UpdateScaleCap > g__IsFree | 112_0(this.Actor.Position + Vector3IntUtility.DirectionsXZDiagonal[3]))
        {
            num = 3f;
        }
    }
    return num;
}

[CompilerGenerated]
internal static bool < DrawXrayMeshes > g__ShouldSkip | 115_0(MeshRenderer renderer)

        {
    return !renderer.isVisible || renderer.GetMeshFilter().sharedMesh == PrimitiveMeshes.Get(PrimitiveType.Quad);
}

[CompilerGenerated]
internal static bool < LookAtPlayer > g__IsFree | 122_0(Vector3Int c)

        {
    return c.InBounds() && Get.CellsInfo.CanPassThrough(c);
}

private Color curEmissionColor = Color.black;

private Color curColor = Color.white;

private Color targetEmissionColor = Color.black;

private Color targetColor = Color.white;

private bool everChangedMaterial;

private int lastMarkedForAffectedByAoEHighlightFrame = -1;

private Renderer xrayRenderer;

private ActorGOC.MovesPerTurnAnimation movesPerTurnAnimation;

private float lastDanceRot;

private Material bodyPartTextureHighlightMaterial;

private GameObject bodyPartTextureHighlightRenderer;

private int lastHighlightBodyPartFrame = -99999;

private BodyPart lastHighlightBodyPart;

private Material originalMaterial;

private Material originalXrayMaterial;

private float lyingRotation;

private float fallingRotationPct;

private float lastTimeLostHP = -99999f;

private List<ValueTuple<Condition, GameObject>> conditionParticleSystems = new List<ValueTuple<Condition, GameObject>>();

private bool canPlaySleepingSound;

private GameObject blobShadow;

private GameObject blobShadowUndoNonUniformScale;

private float heightCapFactor = 1f;

private float widthCapFactor = 1f;

private float lastTargetHeightCapFactor = 1f;

private Bounds? occupiedBoundsCached;

private float scaleFactorFromConditions = 1f;

private float lastStretchSoundTime = -99999f;

private float lastActorAnimationStepSoundRot = -99999f;

private MeshRenderer actorTextureRenderer;

private float lookAwayPct;

private float lastWakeUpTime = -99999f;

private List<ValueTuple<MeshRenderer, int, Color>> cachedAllBodyRenderers;

private List<ValueTuple<MeshRenderer, int>> cachedNonMissingBodyRenderers;

private List<ValueTuple<MeshRenderer, int>> cachedMissingBodyRenderers;

private GameObject talkParticleSystem;

private GameObject moveParticleSystem;

private ParticleSystem moveParticleSystemPs;

private float lastMoveTime = -99999f;

private bool hasPlayerModel;

private float strikeAnimationStartTime = -99999f;

private Vector3 strikeAnimationDir;

private float strikeAnimationExtraDistFactor = 1f;

private Vector3 offsetFromImpacts;

private bool lastOffsetFromImpactWasPowerful;

private bool rotationOffsetFromImpactSign;

private const float MaxTilt = 25f;

private const float EmissionColorStrength = 0.5f;

private static GameObject ActorXrayPrefab;

private static GameObject BodyPartHighlightPrefab;

private static Material ActorWithMissingBodyParts;

private static Material ActorWithMissingBodyPartsXray;

private static Material ActorModelXray;

private static GameObject ActorBlobShadowPrefab;

private static GameObject DialogueParticlesPrefab;

private static GameObject MoveParticlesPrefab;

private const float BossActorScale = 1.5f;

private const float BabyActorScale = 0.73f;

public enum MovesPerTurnAnimation
{
    Normal,

    WontMoveThisTurn,

    WillMoveAtLeastTwice
}
	}
}