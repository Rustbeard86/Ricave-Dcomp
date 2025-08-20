using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class LightManager
    {
        private int MaxShadowLightsActive
        {
            get
            {
                return PrefsHelper.MaxLights;
            }
        }

        public void RegisterLight(Light light, Func<Color> colorGetter, Func<Vector3> positionGetter, Func<Quaternion> rotationGetter, Entity entity, bool onlyIfEntityVisible, bool fadeOutOnEntityDespawned)
        {
            if (light == null)
            {
                Log.Error("Tried to register null light.", false);
                return;
            }
            if (this.Contains(light))
            {
                Log.Error("Tried to register the same light twice.", false);
                return;
            }
            LightManager.LightEntry lightEntry = new LightManager.LightEntry
            {
                light = light,
                colorGetter = colorGetter,
                positionGetter = positionGetter,
                rotationGetter = rotationGetter,
                entity = entity,
                onlyIfEntityVisible = onlyIfEntityVisible,
                fadeOutOnEntityDespawned = fadeOutOnEntityDespawned
            };
            if (light.shadows != LightShadows.None)
            {
                this.lights_shadow.Add(lightEntry);
            }
            else
            {
                this.lights_noShadow.Add(lightEntry);
            }
            light.enabled = false;
        }

        public void DeregisterLight(Light light)
        {
            if (light == null)
            {
                Log.Error("Tried to deregister null light.", false);
                return;
            }
            if (!this.Contains(light))
            {
                Log.Error("Tried to deregister light but it's not here.", false);
                return;
            }
            for (int i = 0; i < this.lights_shadow.Count; i++)
            {
                if (this.lights_shadow[i].light == light)
                {
                    LightManager.LightEntry lightEntry = this.lights_shadow[i];
                    lightEntry.deregistered = true;
                    this.lights_shadow[i] = lightEntry;
                    if (this.ShouldRemoveLight(this.lights_shadow[i]))
                    {
                        if (this.lights_shadow[i].light != null)
                        {
                            Object.Destroy(this.lights_shadow[i].light.gameObject);
                        }
                        this.lights_shadow.RemoveAt(i);
                    }
                    return;
                }
            }
            for (int j = 0; j < this.lights_noShadow.Count; j++)
            {
                if (this.lights_noShadow[j].light == light)
                {
                    LightManager.LightEntry lightEntry2 = this.lights_noShadow[j];
                    lightEntry2.deregistered = true;
                    this.lights_noShadow[j] = lightEntry2;
                    if (this.ShouldRemoveLight(this.lights_noShadow[j]))
                    {
                        if (this.lights_noShadow[j].light != null)
                        {
                            Object.Destroy(this.lights_noShadow[j].light.gameObject);
                        }
                        this.lights_noShadow.RemoveAt(j);
                    }
                    return;
                }
            }
        }

        public bool Contains(Light light)
        {
            if (light == null)
            {
                return false;
            }
            for (int i = 0; i < this.lights_shadow.Count; i++)
            {
                if (this.lights_shadow[i].light == light)
                {
                    return true;
                }
            }
            for (int j = 0; j < this.lights_noShadow.Count; j++)
            {
                if (this.lights_noShadow[j].light == light)
                {
                    return true;
                }
            }
            return false;
        }

        public void Update()
        {
            int num = this.MaxShadowLightsActive;
            for (int i = this.lights_shadow.Count - 1; i >= 0; i--)
            {
                if (this.ShouldRemoveLight(this.lights_shadow[i]))
                {
                    if (this.lights_shadow[i].light != null)
                    {
                        Object.Destroy(this.lights_shadow[i].light.gameObject);
                    }
                    this.lights_shadow.RemoveAt(i);
                }
                else if (this.lights_shadow[i].light.enabled)
                {
                    num--;
                }
            }
            for (int j = this.lights_noShadow.Count - 1; j >= 0; j--)
            {
                if (this.ShouldRemoveLight(this.lights_noShadow[j]))
                {
                    if (this.lights_noShadow[j].light != null)
                    {
                        Object.Destroy(this.lights_noShadow[j].light.gameObject);
                    }
                    this.lights_noShadow.RemoveAt(j);
                }
            }
            if (Clock.Frame % 3 == 2 && this.lights_shadow.Count >= 2)
            {
                for (int k = 0; k < this.lights_shadow.Count; k++)
                {
                    LightManager.LightEntry lightEntry = this.lights_shadow[k];
                    lightEntry.currentValueToCompareForSort = this.CalculateValueToCompareForSort(this.lights_shadow[k]);
                    this.lights_shadow[k] = lightEntry;
                }
                this.lights_shadow.Sort(LightManager.ByCurrentValueToCompareForSort);
            }
            for (int l = 0; l < this.lights_shadow.Count; l++)
            {
                LightManager.LightEntry lightEntry2 = this.lights_shadow[l];
                if (this.ShouldFadeOut(lightEntry2) || l >= this.MaxShadowLightsActive)
                {
                    if (lightEntry2.light.enabled)
                    {
                        lightEntry2.alpha = Calc.Clamp01(lightEntry2.alpha - 3f * Clock.DeltaTime);
                        if (lightEntry2.alpha <= 0f)
                        {
                            lightEntry2.light.enabled = false;
                        }
                        this.lights_shadow[l] = lightEntry2;
                    }
                }
                else
                {
                    if (num > 0 && !lightEntry2.light.enabled)
                    {
                        lightEntry2.light.enabled = true;
                        num--;
                    }
                    if (lightEntry2.light.enabled)
                    {
                        lightEntry2.alpha = Calc.Clamp01(lightEntry2.alpha + 1.8f * Clock.DeltaTime);
                        this.lights_shadow[l] = lightEntry2;
                    }
                }
                if (lightEntry2.light.enabled)
                {
                    lightEntry2.light.color = lightEntry2.colorGetter() * lightEntry2.alpha;
                    lightEntry2.light.gameObject.transform.SetPositionAndRotation(lightEntry2.positionGetter(), lightEntry2.rotationGetter());
                }
            }
            for (int m = 0; m < this.lights_noShadow.Count; m++)
            {
                LightManager.LightEntry lightEntry3 = this.lights_noShadow[m];
                if (this.ShouldFadeOut(lightEntry3))
                {
                    if (lightEntry3.light.enabled)
                    {
                        lightEntry3.alpha = Calc.Clamp01(lightEntry3.alpha - 3f * Clock.DeltaTime);
                        if (lightEntry3.alpha <= 0f)
                        {
                            lightEntry3.light.enabled = false;
                        }
                        this.lights_noShadow[m] = lightEntry3;
                    }
                }
                else
                {
                    if (!lightEntry3.light.enabled)
                    {
                        lightEntry3.light.enabled = true;
                    }
                    lightEntry3.alpha = Calc.Clamp01(lightEntry3.alpha + 1.8f * Clock.DeltaTime);
                    this.lights_noShadow[m] = lightEntry3;
                }
                if (lightEntry3.light.enabled)
                {
                    lightEntry3.light.color = lightEntry3.colorGetter() * lightEntry3.alpha;
                    lightEntry3.light.gameObject.transform.SetPositionAndRotation(lightEntry3.positionGetter(), lightEntry3.rotationGetter());
                }
            }
        }

        private float CalculateValueToCompareForSort(LightManager.LightEntry light)
        {
            float num = 0f;
            if (light.entity != null && LightManager.< CalculateValueToCompareForSort > g__PlayerSees | 12_0(light.entity.Position))
            {
                num -= 1000000f;
            }
            return num + (light.light.transform.position - Get.CameraPosition).sqrMagnitude;
        }

        private bool ShouldRemoveLight(LightManager.LightEntry entry)
        {
            if (entry.light == null)
            {
                return true;
            }
            if (entry.deregistered)
            {
                if (!entry.fadeOutOnEntityDespawned)
                {
                    return true;
                }
                if (entry.alpha <= 0f || !entry.light.enabled)
                {
                    return true;
                }
            }
            return false;
        }

        private bool ShouldFadeOut(LightManager.LightEntry entry)
        {
            return entry.light == null || entry.deregistered || (entry.onlyIfEntityVisible && (entry.entity == null || !entry.entity.Spawned || !entry.entity.GameObject.activeSelf)) || (entry.light.shadows == LightShadows.None && entry.entity != null && Get.FogOfWar.IsFogged(entry.entity.Position) && (!entry.entity.Position.Above().InBounds() || Get.FogOfWar.IsFogged(entry.entity.Position.Above())));
        }

        [CompilerGenerated]
        internal static bool <CalculateValueToCompareForSort>g__PlayerSees|12_0(Vector3Int pos)
		{
			if (pos.GetGridDistance(Get.NowControlledActor.Position) > 15)
			{
				return false;
			}
			if (Get.VisibilityCache.PlayerSees(pos))
			{
				return true;
			}
			for (int i = 0; i<Vector3IntUtility.DirectionsCardinal.Length; i++)
			{
				Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsCardinal[i];
				if (vector3Int.InBounds() && Get.CellsInfo.CanSeeThrough(vector3Int) && Get.VisibilityCache.PlayerSees(vector3Int))
				{
					return true;
				}
			}
			return Get.NowControlledActor != null && Get.NowControlledActor.Spawned && pos.GetGridDistance(Get.NowControlledActor.Position) <= 10 && LineOfSight.IsLineOfSight(Get.NowControlledActor.Position, pos);
		}

private List<LightManager.LightEntry> lights_shadow = new List<LightManager.LightEntry>(16);

private List<LightManager.LightEntry> lights_noShadow = new List<LightManager.LightEntry>(16);

private const float FadeInSpeed = 1.8f;

private const float FadeOutSpeed = 3f;

private static readonly Comparison<LightManager.LightEntry> ByCurrentValueToCompareForSort = (LightManager.LightEntry a, LightManager.LightEntry b) => a.currentValueToCompareForSort.CompareTo(b.currentValueToCompareForSort);

private struct LightEntry
{
    public Light light;

    public Func<Color> colorGetter;

    public Func<Vector3> positionGetter;

    public Func<Quaternion> rotationGetter;

    public Entity entity;

    public bool onlyIfEntityVisible;

    public bool fadeOutOnEntityDespawned;

    public float alpha;

    public float currentValueToCompareForSort;

    public bool deregistered;
}
	}
}