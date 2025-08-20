using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class EntitiesWithSoundLoop
    {
        public void Update()
        {
            if (Get.InMainMenu)
            {
                return;
            }
            List<Entity> allEntitiesWithSoundLoop = Get.World.AllEntitiesWithSoundLoop;
            Vector3 cameraPosition = Get.CameraPosition;
            if (this.ShouldRecalculateClosestEntities())
            {
                this.tmpClosestEntities.Clear();
                int i = 0;
                int count = allEntitiesWithSoundLoop.Count;
                while (i < count)
                {
                    Entity entity = allEntitiesWithSoundLoop[i];
                    float num;
                    SoundSpec resolvedSoundLoop = this.GetResolvedSoundLoop(entity, out num);
                    if (resolvedSoundLoop != null)
                    {
                        Entity entity2;
                        if (!this.tmpClosestEntities.TryGetValue(resolvedSoundLoop, out entity2))
                        {
                            this.tmpClosestEntities.Add(resolvedSoundLoop, entity);
                        }
                        else if ((entity.RenderPosition - cameraPosition).sqrMagnitude < (entity2.RenderPosition - cameraPosition).sqrMagnitude)
                        {
                            this.tmpClosestEntities[resolvedSoundLoop] = entity;
                        }
                    }
                    i++;
                }
            }
            foreach (KeyValuePair<SoundSpec, Entity> keyValuePair in this.tmpClosestEntities)
            {
                float num2;
                this.GetResolvedSoundLoop(keyValuePair.Value, out num2);
                this.UpdateLoop(keyValuePair.Key, keyValuePair.Value, num2);
            }
            if (this.loopsPlaying.Count != 0)
            {
                this.tmpLoopsPlayingNoEntity.Clear();
                foreach (KeyValuePair<SoundSpec, SoundHandle> keyValuePair2 in this.loopsPlaying)
                {
                    if (!this.tmpClosestEntities.ContainsKey(keyValuePair2.Key))
                    {
                        this.tmpLoopsPlayingNoEntity.Add(keyValuePair2.Key);
                    }
                }
                for (int j = 0; j < this.tmpLoopsPlayingNoEntity.Count; j++)
                {
                    this.UpdateLoop(this.tmpLoopsPlayingNoEntity[j], null, 10f);
                }
                this.tmpLoopsPlayingNoEntity.Clear();
            }
        }

        private void UpdateLoop(SoundSpec soundLoop, Entity closestEntity, float soundLoopMaxDistance)
        {
            bool flag = closestEntity != null && (closestEntity.RenderPosition - Get.CameraPosition).sqrMagnitude < Math.Min(100f, soundLoopMaxDistance * soundLoopMaxDistance);
            SoundHandle soundHandle;
            if (!this.loopsPlaying.TryGetValue(soundLoop, out soundHandle))
            {
                if (!flag)
                {
                    return;
                }
                soundHandle = Get.SustainingSounds.CreateHandleAt(soundLoop.AudioClip, closestEntity.RenderPositionComputedCenter, true);
                soundHandle.Volume = 0f;
                soundHandle.Pitch = soundLoop.PitchRange.Middle;
                soundHandle.SpatialBlend = EntitiesWithSoundLoop.< UpdateLoop > g__GetSpatialBlend | 5_0(soundHandle.Position);
                soundHandle.MaxDistance = soundLoopMaxDistance;
                soundHandle.Time = Rand.Range(0f, soundHandle.Length);
                soundHandle.Play();
                this.loopsPlaying.Add(soundLoop, soundHandle);
            }
            if (flag)
            {
                Vector3 renderPositionComputedCenter = closestEntity.RenderPositionComputedCenter;
                if ((soundHandle.Position - renderPositionComputedCenter).sqrMagnitude <= 25f)
                {
                    soundHandle.Position = Vector3.MoveTowards(soundHandle.Position, renderPositionComputedCenter, Clock.UnscaledDeltaTime * 5f);
                }
                else
                {
                    soundHandle.Position = renderPositionComputedCenter;
                }
                soundHandle.SpatialBlend = EntitiesWithSoundLoop.< UpdateLoop > g__GetSpatialBlend | 5_0(soundHandle.Position);
                soundHandle.Volume = Math.Min(soundHandle.Volume + Clock.UnscaledDeltaTime * 1f, soundLoop.VolumeRange.Middle);
                soundHandle.MaxDistance = soundLoopMaxDistance;
                return;
            }
            soundHandle.Volume = Math.Max(soundHandle.Volume - Clock.UnscaledDeltaTime * 1f, 0f);
            if (soundHandle.Volume <= 0f)
            {
                soundHandle.Dispose();
                this.loopsPlaying.Remove(soundLoop);
            }
        }

        private bool ShouldRecalculateClosestEntities()
        {
            if (Clock.Frame % 2 == 0)
            {
                return true;
            }
            foreach (KeyValuePair<SoundSpec, Entity> keyValuePair in this.tmpClosestEntities)
            {
                if (!keyValuePair.Value.Spawned)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnSceneChanged()
        {
            this.loopsPlaying.Clear();
            this.tmpClosestEntities.Clear();
        }

        private SoundSpec GetResolvedSoundLoop(Entity entity, out float maxDistance)
        {
            Actor actor = entity as Actor;
            if (actor != null)
            {
                ValueTuple<SoundSpec, float> soundLoop = actor.ConditionsAccumulated.SoundLoop;
                if (soundLoop.Item1 != null)
                {
                    maxDistance = soundLoop.Item2;
                    return soundLoop.Item1;
                }
            }
            maxDistance = entity.Spec.SoundLoopMaxDistance;
            if (entity.Spec.SoundLoop != Get.Sound_NightLoop)
            {
                return entity.Spec.SoundLoop;
            }
            if (Get.Floor > 1)
            {
                return null;
            }
            if (Get.WeatherManager.IsRainingOutside)
            {
                return Get.Sound_RainLoop;
            }
            int num;
            int num2;
            GameTime.GetTime(out num, out num2);
            if (num < 5 || num >= 20)
            {
                return Get.Sound_NightLoop;
            }
            return Get.Sound_DayLoop;
        }

        [CompilerGenerated]
        internal static float <UpdateLoop>g__GetSpatialBlend|5_0(Vector3 soundPos)
		{
			float num = Vector3.Distance(soundPos, Get.CameraPosition);
			if (num< 0.5f)
			{
				return Calc.LerpDouble(0f, 0.5f, 0f, 1f, num);
			}
			return 1f;
		}

private Dictionary<SoundSpec, SoundHandle> loopsPlaying = new Dictionary<SoundSpec, SoundHandle>(10);

private Dictionary<SoundSpec, Entity> tmpClosestEntities = new Dictionary<SoundSpec, Entity>(10);

private List<SoundSpec> tmpLoopsPlayingNoEntity = new List<SoundSpec>(10);

private const float VolumeFadeInOutSpeed = 1f;
	}
}