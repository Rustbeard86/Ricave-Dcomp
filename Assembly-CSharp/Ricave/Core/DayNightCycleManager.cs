using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class DayNightCycleManager
    {
        public DayPart DayPart
        {
            get
            {
                int num;
                int num2;
                GameTime.GetTime(out num, out num2);
                float num3 = (float)num + (float)num2 / 60f;
                if (num3 < 5f)
                {
                    return DayPart.Night;
                }
                if (num3 < 8f)
                {
                    return DayPart.Morning;
                }
                if (num3 < 18f)
                {
                    return DayPart.Day;
                }
                if (num3 < 21f)
                {
                    return DayPart.Evening;
                }
                return DayPart.Night;
            }
        }

        public bool IsNightForNightOwl
        {
            get
            {
                int num;
                int num2;
                GameTime.GetTime(out num, out num2);
                return num >= 19 || num < 7;
            }
        }

        public void Update()
        {
            if ((Get.Trait_NightOwl.IsChosen() || Get.Trait_Vampire.IsChosen()) && this.prevIsNightForNightOwl != this.IsNightForNightOwl && Get.MainActor != null)
            {
                this.prevIsNightForNightOwl = this.IsNightForNightOwl;
                Get.VisibilityCache.OnSeeRangeChanged(Get.MainActor);
            }
            this.UpdateWorldSituationsVars();
            int num;
            int num2;
            GameTime.GetTime(out num, out num2);
            float num3 = (float)num + (float)num2 / 60f;
            for (int i = 0; i < DayNightCycleManager.ColorTargets.Length; i++)
            {
                DayNightCycleManager.ColorsTarget colorsTarget = DayNightCycleManager.ColorTargets[i];
                if ((colorsTarget.minHour > num3 || i == DayNightCycleManager.ColorTargets.Length - 1) && i != 0)
                {
                    DayNightCycleManager.ColorsTarget colorsTarget2 = DayNightCycleManager.ColorTargets[i - 1];
                    float num4 = Calc.InverseLerp(colorsTarget2.minHour, colorsTarget.minHour, num3);
                    Color color = Color.Lerp(colorsTarget2.ambientLight, colorsTarget.ambientLight, num4);
                    Color color2 = Color.Lerp(colorsTarget2.fogColor, colorsTarget.fogColor, num4);
                    float num5 = Calc.Lerp(colorsTarget2.fogIntensity, colorsTarget.fogIntensity, num4);
                    if (!Get.InLobby)
                    {
                        Color.Lerp(colorsTarget2.skyColor, colorsTarget.skyColor, num4);
                    }
                    else
                    {
                        Color black = Color.black;
                    }
                    color = (color * this.worldSituations_ambientLightFactor).WithAlpha(1f);
                    if (this.worldSituations_fogOverrideLerp != 0f)
                    {
                        color2 = Color.Lerp(color2, this.worldSituations_fogOverride, this.worldSituations_fogOverrideLerp);
                        num5 = Calc.Lerp(num5, this.worldSituations_fogIntensityOverride, this.worldSituations_fogOverrideLerp);
                    }
                    RenderSettings.ambientLight = color;
                    Shader.SetGlobalColor(Get.ShaderPropertyIDs.CustomFogColorID, color2);
                    Shader.SetGlobalFloat(Get.ShaderPropertyIDs.CustomFogIntensityID, num5);
                    return;
                }
            }
        }

        private void UpdateWorldSituationsVars()
        {
            if (this.firstUpdate)
            {
                this.firstUpdate = false;
                this.worldSituations_ambientLightFactor = Get.WorldSituationsManager.AmbientLightFactor;
                ValueTuple<Color, float>? fogOverride = Get.WorldSituationsManager.FogOverride;
                if (fogOverride != null)
                {
                    this.worldSituations_fogOverride = fogOverride.Value.Item1;
                    this.worldSituations_fogIntensityOverride = fogOverride.Value.Item2;
                    this.worldSituations_fogOverrideLerp = 1f;
                    return;
                }
            }
            else
            {
                this.worldSituations_ambientLightFactor = Calc.StepTowards(this.worldSituations_ambientLightFactor, Get.WorldSituationsManager.AmbientLightFactor, Clock.DeltaTime * 0.5f);
                ValueTuple<Color, float>? fogOverride2 = Get.WorldSituationsManager.FogOverride;
                if (fogOverride2 != null)
                {
                    if (this.worldSituations_fogOverrideLerp == 0f)
                    {
                        this.worldSituations_fogOverride = fogOverride2.Value.Item1;
                        this.worldSituations_fogIntensityOverride = fogOverride2.Value.Item2;
                    }
                    else
                    {
                        this.worldSituations_fogOverride = Ricave.UI.ColorUtility.MoveTowards(this.worldSituations_fogOverride, fogOverride2.Value.Item1, Clock.DeltaTime * 0.5f);
                        this.worldSituations_fogIntensityOverride = Calc.StepTowards(this.worldSituations_fogIntensityOverride, fogOverride2.Value.Item2, Clock.DeltaTime * 0.5f);
                    }
                    this.worldSituations_fogOverrideLerp = Calc.StepTowards(this.worldSituations_fogOverrideLerp, 1f, Clock.DeltaTime * 0.5f);
                    return;
                }
                this.worldSituations_fogOverrideLerp = Calc.StepTowards(this.worldSituations_fogOverrideLerp, 0f, Clock.DeltaTime * 0.5f);
            }
        }

        private bool firstUpdate = true;

        private float worldSituations_ambientLightFactor = 1f;

        private Color worldSituations_fogOverride;

        private float worldSituations_fogIntensityOverride = 1f;

        private float worldSituations_fogOverrideLerp;

        private bool prevIsNightForNightOwl;

        public const int MorningStart = 5;

        public const int DayStart = 8;

        public const int EveningStart = 18;

        public const int NightStart = 21;

        private const float AmbientLightFactor = 0.85f;

        private static readonly Color NightAmbientLight = new Color32(98, 144, 229, byte.MaxValue).MultipliedColor(0.85f);

        private static readonly Color NightFog = new Color32(25, 78, 87, byte.MaxValue);

        private static readonly Color MorningAmbientLight = new Color32(197, 187, 169, byte.MaxValue).MultipliedColor(0.85f);

        private static readonly Color MorningFog = new Color32(102, 59, 136, byte.MaxValue);

        private static readonly Color DayAmbientLight = new Color32(160, 160, 160, byte.MaxValue).MultipliedColor(0.85f);

        private static readonly Color DayFog = new Color32(0, 130, 131, byte.MaxValue);

        private static readonly Color DaySky = new Color(0.34f, 0.38f, 0.42f);

        private static readonly DayNightCycleManager.ColorsTarget[] ColorTargets = new DayNightCycleManager.ColorsTarget[]
        {
            new DayNightCycleManager.ColorsTarget
            {
                minHour = 0f,
                ambientLight = DayNightCycleManager.NightAmbientLight,
                fogColor = DayNightCycleManager.NightFog,
                fogIntensity = 3f,
                skyColor = Color.black
            },
            new DayNightCycleManager.ColorsTarget
            {
                minHour = 3f,
                ambientLight = DayNightCycleManager.NightAmbientLight,
                fogColor = DayNightCycleManager.NightFog,
                fogIntensity = 3f,
                skyColor = Color.black
            },
            new DayNightCycleManager.ColorsTarget
            {
                minHour = 6f,
                ambientLight = DayNightCycleManager.MorningAmbientLight,
                fogColor = DayNightCycleManager.MorningFog,
                fogIntensity = 2.9f,
                skyColor = DayNightCycleManager.DaySky
            },
            new DayNightCycleManager.ColorsTarget
            {
                minHour = 9f,
                ambientLight = DayNightCycleManager.DayAmbientLight,
                fogColor = DayNightCycleManager.DayFog,
                fogIntensity = 2f,
                skyColor = DayNightCycleManager.DaySky
            },
            new DayNightCycleManager.ColorsTarget
            {
                minHour = 16f,
                ambientLight = DayNightCycleManager.DayAmbientLight,
                fogColor = DayNightCycleManager.DayFog,
                fogIntensity = 2f,
                skyColor = DayNightCycleManager.DaySky
            },
            new DayNightCycleManager.ColorsTarget
            {
                minHour = 19f,
                ambientLight = DayNightCycleManager.MorningAmbientLight,
                fogColor = DayNightCycleManager.MorningFog,
                fogIntensity = 2.9f,
                skyColor = DayNightCycleManager.DaySky
            },
            new DayNightCycleManager.ColorsTarget
            {
                minHour = 22f,
                ambientLight = DayNightCycleManager.NightAmbientLight,
                fogColor = DayNightCycleManager.NightFog,
                fogIntensity = 3f,
                skyColor = Color.black
            },
            new DayNightCycleManager.ColorsTarget
            {
                minHour = 24f,
                ambientLight = DayNightCycleManager.NightAmbientLight,
                fogColor = DayNightCycleManager.NightFog,
                fogIntensity = 3f,
                skyColor = Color.black
            }
        };

        private struct ColorsTarget
        {
            public float minHour;

            public Color ambientLight;

            public Color fogColor;

            public float fogIntensity;

            public Color skyColor;
        }
    }
}