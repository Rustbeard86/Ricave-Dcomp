using System;
using System.Runtime.CompilerServices;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class CameraEffects
    {
        public float LastExperienceParticleArrivedTime
        {
            get
            {
                return this.lastExperienceParticleArrivedTime;
            }
        }

        public bool WalkingStaircase
        {
            get
            {
                return this.walkDownStaircaseAnimationStartTime != null || this.walkUpStaircaseAnimationStartTime != null;
            }
        }

        public float LastRewindTime
        {
            get
            {
                return this.lastRewindTime;
            }
        }

        private float VignetteAmountDecaySpeed
        {
            get
            {
                if (!PlayerActorStatusControls.IsLowHP)
                {
                    return 1f;
                }
                return 0.5f;
            }
        }

        private bool ShouldHitstop
        {
            get
            {
                float num = Clock.UnscaledTime - this.lastActorHitByPlayerWithImpactTime;
                return num > 0.03f && num < 0.11f && this.lastActorHitByPlayerWithImpactActor.Spawned;
            }
        }

        public float TargetTimeScale
        {
            get
            {
                if (Clock.UnscaledTime - this.lastBossKillTime < 1.6f)
                {
                    return 0.14f;
                }
                if (Clock.UnscaledTime - this.lastNormalEnemySlowdownKillTime < 0.38f)
                {
                    return 0.2f;
                }
                if (Get.UI.InventoryOpen || Get.UI.IsWheelSelectorOpen)
                {
                    return 0.25f;
                }
                if (this.ShouldHitstop)
                {
                    return 0.04f;
                }
                return 1f;
            }
        }

        private float TargetGrayscale
        {
            get
            {
                if (Get.DeathScreenDrawer.ShouldShow)
                {
                    return 0f;
                }
                float num = (float)Get.NowControlledActor.HP / (float)Get.NowControlledActor.MaxHP;
                return 1f - Calc.InverseLerp(0f, 0.2f, num);
            }
        }

        private float TargetMotionBlur
        {
            get
            {
                if (Get.NowControlledActor.SequencePerTurn <= 12)
                {
                    return 0f;
                }
                return 0.31f;
            }
        }

        public void Shake(float amount)
        {
            if (this.shakeAmount.CurrentValue < 0.011f)
            {
                this.shakeStartUnscaledTime = Clock.UnscaledTime;
            }
            float num = Math.Max(this.shakeAmount.CurrentValue, amount);
            this.shakeAmount.SetTarget(num, num);
        }

        public void Wobble(float amount)
        {
            this.targetWobbleAmount = Math.Max(this.targetWobbleAmount, amount);
        }

        public void DoVignette(float amount)
        {
            this.targetVignetteAmount = Math.Min(Math.Max(this.targetVignetteAmount, amount), 1f);
        }

        public void DoFallEffect()
        {
            this.targetFallAmount = Math.Max(this.targetFallAmount, 1f);
        }

        public void BumpIntoObstacle(Vector3Int dir)
        {
            this.bumpOffsetTarget = dir * 0.1f;
        }

        public void StartWalkDownStaircaseAnimation(Vector3Int staircasePos, Vector3Int staircaseDir)
        {
            this.walkDownStaircaseAnimationStartTime = new float?(Clock.Time);
            this.staircasePos = staircasePos;
            this.staircaseDir = staircaseDir;
        }

        public void StartWalkUpStaircaseAnimation(Vector3Int staircasePos, Vector3Int staircaseDir)
        {
            this.walkUpStaircaseAnimationStartTime = new float?(Clock.Time);
            this.staircasePos = staircasePos;
            this.staircaseDir = staircaseDir;
        }

        public void OnSceneChanged()
        {
            this.walkDownStaircaseAnimationStartTime = null;
            this.walkUpStaircaseAnimationStartTime = null;
        }

        public void Update()
        {
            if (this.targetWobbleAmount > 0f)
            {
                this.targetWobbleAmount -= Clock.DeltaTime * 1.4f;
                this.targetWobbleAmount = Math.Max(this.targetWobbleAmount, 0f);
            }
            if (this.targetFallAmount > 0f)
            {
                this.targetFallAmount -= Clock.DeltaTime * 1.6f;
                this.targetFallAmount = Math.Max(this.targetFallAmount, 0f);
            }
            if (this.targetVignetteAmount > 0f)
            {
                this.targetVignetteAmount -= Clock.UnscaledDeltaTime * this.VignetteAmountDecaySpeed;
                this.targetVignetteAmount = Math.Max(this.targetVignetteAmount, 0f);
            }
            this.bumpOffset = Vector3.MoveTowards(this.bumpOffset, this.bumpOffsetTarget, 0.75f * Clock.DeltaTime);
            if (this.bumpOffsetTarget != Vector3.zero)
            {
                this.bumpOffsetTarget = Vector3.MoveTowards(this.bumpOffsetTarget, Vector3.zero, 0.6f * Clock.DeltaTime);
            }
            if (Clock.TimeScale != this.TargetTimeScale)
            {
                if (this.ShouldHitstop)
                {
                    Clock.TimeScale = this.TargetTimeScale;
                    this.lastSetTimeScaleInstantlyDueToHitstop = true;
                }
                else if (this.lastSetTimeScaleInstantlyDueToHitstop)
                {
                    Clock.TimeScale = this.TargetTimeScale;
                    this.lastSetTimeScaleInstantlyDueToHitstop = false;
                }
                else
                {
                    Clock.TimeScale = Calc.StepTowards(Clock.TimeScale, this.TargetTimeScale, 4f * Clock.UnscaledDeltaTime);
                }
            }
            this.SetShakeAndBump();
            this.SetVignette();
            this.SetGrayscale();
            this.SetMotionBlur();
            this.UpdateFOVAndBlur();
            this.UpdateWalkDownStaircaseAnimation();
            this.UpdateWalkUpStaircaseAnimation();
            this.UpdateHighSpeedParticles();
        }

        public void FixedUpdate()
        {
            this.wobbleAmount.SetTarget(Calc.Lerp(this.wobbleAmount.Target, this.targetWobbleAmount, 0.1f));
            this.fallAmount.SetTarget(Calc.Lerp(this.fallAmount.Target, this.targetFallAmount, 0.3f));
            this.vignetteAmount.SetTarget(Calc.Lerp(this.vignetteAmount.Target, this.targetVignetteAmount, 0.3f));
            this.shakeAmount.SetTarget(Calc.Lerp(this.shakeAmount.Target, 0f, 0.082f));
            this.grayscale.SetTarget(Calc.Lerp(this.grayscale.Target, this.TargetGrayscale, 0.25f));
            this.FOV.SetTarget(Calc.Lerp(this.FOV.Target, this.targetFOV, 0.2f));
            this.blur.SetTarget(Calc.Lerp(this.blur.Target, this.targetBlur, 0.6f));
            this.motionBlur.SetTarget(Calc.Lerp(this.motionBlur.Target, this.TargetMotionBlur, 0.6f));
        }

        public void OnGUI()
        {
            this.DoTintFromConditions();
            this.DoExperienceParticleArrivedOverlay();
        }

        private void DoTintFromConditions()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (Get.NowControlledActor == null)
            {
                return;
            }
            Color? actorOverlayColorForScreen = Get.NowControlledActor.ConditionsAccumulated.ActorOverlayColorForScreen;
            if (actorOverlayColorForScreen == null)
            {
                this.curCameraTintAlpha -= Clock.UnscaledDeltaTime * 0.7f;
                if (this.curCameraTintAlpha < 0f)
                {
                    this.curCameraTintAlpha = 0f;
                }
            }
            else
            {
                this.lastCameraTintColor = actorOverlayColorForScreen.Value;
                this.curCameraTintAlpha += Clock.UnscaledDeltaTime * 0.7f;
                if (this.curCameraTintAlpha > 1f)
                {
                    this.curCameraTintAlpha = 1f;
                }
            }
            if (this.curCameraTintAlpha <= 0f)
            {
                return;
            }
            Color color = this.lastCameraTintColor.WithAlphaFactor(this.curCameraTintAlpha * 0.3f);
            GUIExtra.DrawRect(Widgets.ScreenRect, color);
        }

        private void DoExperienceParticleArrivedOverlay()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            float num = Calc.ResolveFadeInStayOut(Clock.UnscaledTime - this.lastExperienceParticleArrivedTime, 0.08f, 0f, 0.2f);
            if (num <= 0f)
            {
                return;
            }
            Color color = new Color(0.8f, 0.8f, 0.5f, num * 0.1f);
            GUIExtra.DrawRect(Widgets.ScreenRect, color);
        }

        private void SetShakeAndBump()
        {
            Get.CameraOffset.transform.localPosition = Vector3.zero;
            Get.CameraOffset.transform.localRotation = Quaternion.identity;
            if (this.wobbleAmount.CurrentValue > 0.0001f)
            {
                float num = Clock.Time * 35f;
                Vector2 vector = new Vector2(Calc.Cos(num * 0.017453292f), Calc.Sin(num * 0.017453292f)) * this.wobbleAmount.CurrentValue * 0.05f;
                Get.CameraOffset.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
                Get.CameraOffset.transform.localRotation = Quaternion.Euler(0f, vector.x * 30f, vector.y * 30f);
            }
            if (this.bumpOffset != Vector3.zero)
            {
                Get.CameraOffset.transform.position += this.bumpOffset;
            }
            if (this.fallAmount.CurrentValue > 0.0001f)
            {
                Get.CameraOffset.transform.localPosition += Vector3.down * this.fallAmount.CurrentValue * 0.2f;
                Get.CameraOffset.transform.localRotation *= Quaternion.Euler(this.fallAmount.CurrentValue * 10f, 0f, 0f);
            }
            if (this.shakeAmount.CurrentValue > 0.0001f)
            {
                Quaternion quaternion = Quaternion.Euler(new Vector3(Noise.PerlinNoiseMinusOneToOne(Clock.UnscaledTime * 4f, 7368f), Noise.PerlinNoiseMinusOneToOne(Clock.UnscaledTime * 4f, 1364f), Noise.PerlinNoiseMinusOneToOne(Clock.UnscaledTime * 4f, 4857f)) * this.shakeAmount.CurrentValue * 2f * Math.Min((Clock.UnscaledTime - this.shakeStartUnscaledTime) / 0.09f, 1f));
                Get.CameraOffset.transform.localRotation *= quaternion;
            }
            if (Get.NowControlledActor.CanFly && Get.CellsInfo.IsFallingAt(Get.NowControlledActor.Position, Get.NowControlledActor.Gravity, false, Get.NowControlledActor.CanUseLadders, false))
            {
                this.levitatingFadeInOut += Clock.DeltaTime * 2f;
                this.levitatingFadeInOut = Math.Min(this.levitatingFadeInOut, 1f);
            }
            else
            {
                this.levitatingFadeInOut -= Clock.DeltaTime * 2f;
                this.levitatingFadeInOut = Math.Max(this.levitatingFadeInOut, 0f);
            }
            if (this.levitatingFadeInOut > 0.0001f)
            {
                Get.CameraOffset.transform.localPosition += Vector3.up * Calc.Sin(Clock.Time * 2.5f) * 0.03f * this.levitatingFadeInOut;
            }
        }

        public void OnActionUndone()
        {
            this.lastRewindTime = Clock.UnscaledTime;
        }

        public void OnExperienceParticleArrived()
        {
            this.lastExperienceParticleArrivedTime = Clock.UnscaledTime;
        }

        public void OnActorKilled(Actor actor)
        {
            CameraEffects.<> c__DisplayClass84_0 CS$<> 8__locals1;
            CS$<> 8__locals1.actor = actor;
            if (CS$<> 8__locals1.actor == Get.NowControlledActor)
			{
                return;
            }
            if ((CS$<> 8__locals1.actor.Position - Get.CameraPosition).sqrMagnitude > 26.009998f)
			{
                return;
            }
            if (!Get.NowControlledActor.Sees(CS$<> 8__locals1.actor.Position, null))
            {
                return;
            }
            if (CS$<> 8__locals1.actor.IsBoss)
			{
                if (CS$<> 8__locals1.actor.Position.IsAdjacent(Get.NowControlledActor.Position) || Vector3.Dot(Get.CameraTransform.forward, (CS$<> 8__locals1.actor.Position - Get.CameraPosition).normalized) >= 0f)
				{
                    this.lastBossKillTime = Clock.UnscaledTime;
                    Clock.TimeScale = this.TargetTimeScale;
                    Get.ShatterManager.OnTimeScaleChangedDrastically();
                    Get.VolumeShatterManager.OnTimeScaleChangedDrastically();
                    Get.BlackBars.ShowBlackBars();
                    return;
                }
            }

            else if (CS$<> 8__locals1.actor.IsHostile(Get.NowControlledActor) && Vector3.Dot(Get.CameraTransform.forward, (CS$<> 8__locals1.actor.Position - Get.CameraPosition).normalized) >= 0f && !CameraEffects.< OnActorKilled > g__AnyOtherHostileActorNearby | 84_0(ref CS$<> 8__locals1))
			{
                this.lastNormalEnemySlowdownKillTime = Clock.UnscaledTime;
                Clock.TimeScale = this.TargetTimeScale;
                Get.ShatterManager.OnTimeScaleChangedDrastically();
                Get.VolumeShatterManager.OnTimeScaleChangedDrastically();
            }
        }

        public void OnActorHitByPlayerWithImpact(Actor actor)
        {
            if (!actor.Spawned)
            {
                return;
            }
            if (!actor.Position.IsAdjacentOrInside(Get.NowControlledActor.Position))
            {
                return;
            }
            if (!Get.NowControlledActor.Sees(actor.Position, null))
            {
                return;
            }
            if (Vector3.Dot(Get.CameraTransform.forward, (actor.Position - Get.CameraPosition).normalized) < 0f)
            {
                return;
            }
            this.lastActorHitByPlayerWithImpactTime = Clock.UnscaledTime;
            this.lastActorHitByPlayerWithImpactActor = actor;
        }

        private void SetVignette()
        {
            Get.Vignette.intensity = this.vignetteAmount.CurrentValue;
            Get.Vignette.enabled = this.vignetteAmount.CurrentValue > 0.001f;
        }

        private void SetGrayscale()
        {
            Get.Grayscale.saturation = 1f - this.grayscale.CurrentValue;
            Get.Grayscale.enabled = this.grayscale.CurrentValue > 0.001f;
        }

        private void SetMotionBlur()
        {
            Get.MotionBlur.blurAmount = this.motionBlur.CurrentValue;
            Get.MotionBlur.enabled = this.motionBlur.CurrentValue > 0.001f;
        }

        private void UpdateFOVAndBlur()
        {
            if (Get.UI.InventoryOpen || Get.UI.IsWheelSelectorOpen || Get.TurnManager.Rewinding || Clock.UnscaledTime - this.lastRewindTime < 0.2f || Get.WindowManager.IsOpen(Get.Window_QuestLog) || Get.WindowManager.IsOpen(Get.Window_Stats) || Get.WindowManager.IsOpen(Get.Window_Traits) || Get.WindowManager.IsOpen(Get.Window_TrainingRoom) || Get.WindowManager.IsOpen(Get.Window_NewRun))
            {
                this.targetFOV = PrefsHelper.FOV + 2f;
                this.targetBlur = 4f;
            }
            else
            {
                this.targetFOV = PrefsHelper.FOV;
                this.targetBlur = 0f;
            }
            if (Get.NowControlledActor.SequencePerTurn > 12)
            {
                this.targetFOV += 2f;
            }
            else if (Get.NowControlledActor.SequencePerTurn < 12)
            {
                this.targetFOV -= 2f;
            }
            Get.Camera.fieldOfView = this.FOV.CurrentValue;
            Get.Blur.blurSize = this.blur.CurrentValue;
            Get.Blur.enabled = this.blur.CurrentValue > 0.001f;
        }

        private void UpdateWalkDownStaircaseAnimation()
        {
            if (this.walkDownStaircaseAnimationStartTime == null)
            {
                return;
            }
            float num = (Clock.Time - this.walkDownStaircaseAnimationStartTime.Value) * 2.5f;
            Vector3 vector = new Vector3(Get.FPPControllerGOC.StaircaseAnimation_xOffset.Evaluate(num), Get.FPPControllerGOC.StaircaseAnimation_yOffset.Evaluate(num), Get.FPPControllerGOC.StaircaseAnimation_zOffset.Evaluate(num));
            Get.FPPControllerGOC.TargetPosition = this.staircasePos + Vector3.up * 0.1f + vector.RotateY(this.staircaseDir.ToXZAngle());
        }

        private void UpdateWalkUpStaircaseAnimation()
        {
            if (this.walkUpStaircaseAnimationStartTime == null)
            {
                return;
            }
            float num = (Clock.Time - this.walkUpStaircaseAnimationStartTime.Value) * 2.5f;
            Vector3 vector = new Vector3(Get.FPPControllerGOC.StaircaseUpAnimation_xOffset.Evaluate(num), Get.FPPControllerGOC.StaircaseUpAnimation_yOffset.Evaluate(num), Get.FPPControllerGOC.StaircaseUpAnimation_zOffset.Evaluate(num));
            Get.FPPControllerGOC.TargetPosition = this.staircasePos + vector.RotateY(this.staircaseDir.ToXZAngle());
        }

        private void UpdateHighSpeedParticles()
        {
            if (Get.NowControlledActor.SequencePerTurn < 12)
            {
                if (!Get.HighSpeedParticles.isPlaying)
                {
                    Get.HighSpeedParticles.Play();
                    return;
                }
            }
            else if (Get.HighSpeedParticles.isPlaying)
            {
                Get.HighSpeedParticles.Stop();
            }
        }

        [CompilerGenerated]
        internal static bool <OnActorKilled>g__AnyOtherHostileActorNearby|84_0(ref CameraEffects.<>c__DisplayClass84_0 A_0)
		{
			if (A_0.actor.Spec == Get.Entity_Slime && A_0.actor.IsHostile(Get.NowControlledActor))
			{
				return true;
			}
			foreach (Actor actor in Get.World.Actors)
			{
				if (actor != A_0.actor && actor != Get.NowControlledActor && actor.IsHostile(Get.NowControlledActor) && AIUtility.SeesOrJustSeen(Get.NowControlledActor, actor))
				{
					return true;
				}
			}
			return false;
		}

private InterpolatedFloat shakeAmount;

private InterpolatedFloat wobbleAmount;

private float targetWobbleAmount;

private InterpolatedFloat fallAmount;

private float targetFallAmount;

private InterpolatedFloat vignetteAmount;

private float targetVignetteAmount;

private InterpolatedFloat FOV = new InterpolatedFloat(PrefsHelper.FOV);

private float targetFOV = PrefsHelper.FOV;

private InterpolatedFloat blur;

private float targetBlur;

private InterpolatedFloat motionBlur;

private float levitatingFadeInOut;

private float shakeStartUnscaledTime;

private Vector3 bumpOffset;

private Vector3 bumpOffsetTarget;

private float curCameraTintAlpha;

private Color lastCameraTintColor;

private float lastRewindTime = -999999f;

private float lastExperienceParticleArrivedTime = -999999f;

private float lastBossKillTime = -999999f;

private float lastNormalEnemySlowdownKillTime = -999999f;

private float lastActorHitByPlayerWithImpactTime = -999999f;

private Actor lastActorHitByPlayerWithImpactActor;

private InterpolatedFloat grayscale;

private bool lastSetTimeScaleInstantlyDueToHitstop;

private float? walkDownStaircaseAnimationStartTime;

private float? walkUpStaircaseAnimationStartTime;

private Vector3Int staircasePos;

private Vector3Int staircaseDir;

private const float FOVBlurredOffset = 2f;

private const float FOVLerpSpeed = 0.2f;

private const float BlurSize = 4f;

private const float BlurLerpSpeed = 0.6f;

private const float MotionBlurLerpSpeed = 0.6f;

private const float WobbleAmountDecaySpeed = 1.4f;

private const float FallAmountDecaySpeed = 1.6f;

private const float WobbleAngleChangeSpeed = 35f;

private const float WobblePosOffsetFactor = 0.05f;

private const float ShakeStrength = 2f;

private const float TargetWobbleAmountLerpSpeed = 0.1f;

private const float TargetFallAmountLerpSpeed = 0.3f;

private const float TargetVignetteAmountLerpSpeed = 0.3f;

private const float CameraWobbleOffsetToRotFactor = 30f;

private const float BumpSize = 0.1f;

private const float BumpReachTargetSpeed = 0.75f;

private const float BumpDecaySpeed = 0.6f;

private const float ShakeFadeInTime = 0.09f;

private const float CameraTintAlphaFadeSpeed = 0.7f;

private const float TimeScaleReachTargetSpeed = 4f;

private const float TargetGrayscaleLerpSpeed = 0.25f;

private const float ShakeAmountDecayLerpSpeed = 0.082f;
	}
}