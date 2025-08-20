using System;
using UnityEngine;

namespace Ricave.Core
{
    public class WeatherManager
    {
        public Material CurrentSkyMaterial
        {
            get
            {
                return WeatherManager.SkyMaterial;
            }
        }

        public bool IsRainingOutside
        {
            get
            {
                return (Get.InLobby && Get.Progress.GetRunStats(Get.Run_Main1).Runs <= 0) || (Get.Floor <= 1 && Rand.ChanceSeeded(0.15f, Calc.CombineHashes<int, int>(Get.WorldSeed, 390215389)));
            }
        }

        public void Update()
        {
            if (this.IsRainingOutside)
            {
                if (this.nextLightningStrikeTime == null)
                {
                    this.nextLightningStrikeTime = new float?(Clock.Time + WeatherManager.TimeBetweenLightningStrikes.RandomInRange);
                }
                else
                {
                    float time = Clock.Time;
                    float? num = this.nextLightningStrikeTime;
                    if ((time >= num.GetValueOrDefault()) & (num != null))
                    {
                        this.DoLightningStrike();
                    }
                }
            }
            this.UpdateLightningStrikeEffect();
            if (this.nextLightningStrikeSoundTime != null)
            {
                float time2 = Clock.Time;
                float? num = this.nextLightningStrikeSoundTime;
                if ((time2 >= num.GetValueOrDefault()) & (num != null))
                {
                    this.nextLightningStrikeSoundTime = null;
                    Rand.Element<SoundSpec>(Get.Sound_LightningStrike1, Get.Sound_LightningStrike2, Get.Sound_LightningStrike3, Get.Sound_LightningStrike4).PlayOneShot(null, 1f, 1f);
                }
            }
        }

        private void DoLightningStrike()
        {
            this.lastLightningStrikeTime = new float?(Clock.Time);
            this.nextLightningStrikeTime = new float?(Clock.Time + WeatherManager.TimeBetweenLightningStrikes.RandomInRange);
            this.nextLightningStrikeSoundTime = new float?(Clock.Time + WeatherManager.TimeBetweenLightningStrikeAndSound.RandomInRange);
        }

        private void UpdateLightningStrikeEffect()
        {
            float num = ((this.lastLightningStrikeTime != null) ? (Clock.Time - this.lastLightningStrikeTime.Value) : 0.37f);
            float num2 = ((num >= 0.37f) ? 0f : (Math.Min(Noise.PerlinNoise(Clock.Time * 12f, 5846f), 1f) * Calc.ResolveFadeInStayOut(num, 0.1f, 0.17f, 0.1f)));
            Shader.SetGlobalFloat(Get.ShaderPropertyIDs.LightningStrikeID, num2);
        }

        private float? nextLightningStrikeTime;

        private float? nextLightningStrikeSoundTime;

        private float? lastLightningStrikeTime;

        public static readonly Material SkyMaterial = Assets.Get<Material>("Materials/Structures/SkyboxOnAnySurface");

        private static readonly Material BlackMaterial = Assets.Get<Material>("Materials/Structures/VisibilityBlocker");

        private static readonly FloatRange TimeBetweenLightningStrikes = new FloatRange(20f, 250f);

        private static readonly FloatRange TimeBetweenLightningStrikeAndSound = new FloatRange(1f, 1.5f);
    }
}