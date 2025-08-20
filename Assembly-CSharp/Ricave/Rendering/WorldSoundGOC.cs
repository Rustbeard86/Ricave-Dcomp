using System;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class WorldSoundGOC : MonoBehaviour
    {
        private void Start()
        {
            this.CheckInit();
        }

        private void Update()
        {
            this.ApplyScreenFadeOutVolumeCap();
            if ((Clock.Frame + this.instanceID) % 6 == 0)
            {
                this.UpdateTargetLOSLowPassFilter();
            }
            this.curLOSLowPassFilter = Calc.StepTowards(this.curLOSLowPassFilter, this.targetLOSLowPassFilter, Clock.UnscaledDeltaTime * 3f);
            this.ApplyLowPassFilter();
            this.ApplyModifiedPitch();
        }

        public void OnAboutToPlay()
        {
            this.CheckInit();
            this.initialPitch = this.audioSource.pitch;
            this.ApplyScreenFadeOutVolumeCap();
            this.UpdateTargetLOSLowPassFilter();
            this.curLOSLowPassFilter = this.targetLOSLowPassFilter;
            this.ApplyLowPassFilter();
            this.ApplyModifiedPitch();
        }

        private void CheckInit()
        {
            if (this.audioSource != null)
            {
                return;
            }
            this.instanceID = base.GetInstanceID();
            this.audioSource = base.GetComponent<AudioSource>();
            this.lowPassFilter = base.GetComponent<AudioLowPassFilter>();
        }

        private void UpdateTargetLOSLowPassFilter()
        {
            if ((base.transform.position - Get.CameraPosition).sqrMagnitude > 100f)
            {
                this.targetLOSLowPassFilter = 1f;
                return;
            }
            WorldSoundGOC.<> c__DisplayClass11_0 CS$<> 8__locals1;
            CS$<> 8__locals1.pos = base.transform.position.RoundToVector3Int();
            if (!CS$<> 8__locals1.pos.InBounds())
			{
                this.targetLOSLowPassFilter = 1f;
                return;
            }
            if (!Get.NowControlledActor.Spawned)
            {
                this.targetLOSLowPassFilter = 1f;
                return;
            }
            CS$<> 8__locals1.playerPos = Get.NowControlledActor.Position;
            CS$<> 8__locals1.cellsInfo = Get.CellsInfo;
            if (WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos, CS$<> 8__locals1.pos, ref CS$<> 8__locals1))
            {
                this.targetLOSLowPassFilter = 0f;
                return;
            }
            Vector3Int vector3Int = (CS$<> 8__locals1.pos - Get.NowControlledActor.Position).NormalizedToCardinalXZDir();
            Vector3Int vector3Int2 = vector3Int.RightDir();
            if (WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos + vector3Int2, CS$<> 8__locals1.pos, ref CS$<> 8__locals1) || WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos - vector3Int2, CS$<> 8__locals1.pos, ref CS$<> 8__locals1) || WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos + vector3Int, CS$<> 8__locals1.pos, ref CS$<> 8__locals1) || WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos + Vector3IntUtility.Up, CS$<> 8__locals1.pos, ref CS$<> 8__locals1) || WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos + Vector3IntUtility.Down, CS$<> 8__locals1.pos, ref CS$<> 8__locals1) || WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos, CS$<> 8__locals1.pos + vector3Int2, ref CS$<> 8__locals1) || WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos, CS$<> 8__locals1.pos - vector3Int2, ref CS$<> 8__locals1))
            {
                this.targetLOSLowPassFilter = 0.3f;
                return;
            }
            if (WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos + vector3Int2, CS$<> 8__locals1.pos + vector3Int2, ref CS$<> 8__locals1) || WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos - vector3Int2, CS$<> 8__locals1.pos - vector3Int2, ref CS$<> 8__locals1) || WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos + vector3Int, CS$<> 8__locals1.pos + vector3Int2, ref CS$<> 8__locals1) || WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos + vector3Int, CS$<> 8__locals1.pos - vector3Int2, ref CS$<> 8__locals1) || WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos + Vector3IntUtility.Up, CS$<> 8__locals1.pos + vector3Int2, ref CS$<> 8__locals1) || WorldSoundGOC.< UpdateTargetLOSLowPassFilter > g__CanSoundPass | 11_0(CS$<> 8__locals1.playerPos + Vector3IntUtility.Up, CS$<> 8__locals1.pos - vector3Int2, ref CS$<> 8__locals1))
            {
                this.targetLOSLowPassFilter = 0.6f;
                return;
            }
            this.targetLOSLowPassFilter = 1f;
        }

        private void ApplyScreenFadeOutVolumeCap()
        {
            if (Get.ScreenFader.AnyActionQueued)
            {
                this.audioSource.volume = Math.Min(this.audioSource.volume, Calc.LerpDouble(0.6f, 0.99f, 1f, 0f, Get.ScreenFader.Alpha));
                return;
            }
            if (Get.TextSequenceDrawer.Showing || Get.DungeonMapDrawer.Showing)
            {
                this.audioSource.volume = 0f;
            }
        }

        private void ApplyLowPassFilter()
        {
            if (this.curLOSLowPassFilter > 0f)
            {
                this.lowPassFilter.cutoffFrequency = Calc.Lerp(22000f, 4650f, Calc.Pow(this.curLOSLowPassFilter, 0.15f));
                this.lowPassFilter.enabled = true;
                return;
            }
            this.lowPassFilter.enabled = false;
        }

        private void ApplyModifiedPitch()
        {
            if (!Calc.Approximately(Time.timeScale, 1f))
            {
                this.audioSource.pitch = this.initialPitch * Calc.Pow(Time.timeScale, 0.09f);
                this.usingModifiedPitch = true;
                return;
            }
            if (this.usingModifiedPitch)
            {
                this.audioSource.pitch = this.initialPitch;
                this.usingModifiedPitch = false;
                return;
            }
            this.initialPitch = this.audioSource.pitch;
        }

        [CompilerGenerated]
        internal static bool <UpdateTargetLOSLowPassFilter>g__CanSoundPass|11_0(Vector3Int nearPlayerPos, Vector3Int nearDestPos, ref WorldSoundGOC.<>c__DisplayClass11_0 A_2)
		{
			if (A_2.playerPos != nearPlayerPos)
			{
				if (!nearPlayerPos.InBounds())
				{
					return false;
				}
				if (!A_2.cellsInfo.CanProjectilesPassThrough(nearPlayerPos))
				{
					return false;
				}
			}
			if (A_2.pos != nearDestPos)
			{
				if (!nearDestPos.InBounds())
				{
					return false;
				}
if (!A_2.cellsInfo.CanProjectilesPassThrough(nearDestPos))
{
    return false;
}
			}
			return LineOfSight.IsLineOfFire(nearPlayerPos, nearDestPos);
		}

		private int instanceID;

private AudioSource audioSource;

private AudioLowPassFilter lowPassFilter;

private float targetLOSLowPassFilter;

private float curLOSLowPassFilter;

private float initialPitch = 1f;

private bool usingModifiedPitch;
	}
}