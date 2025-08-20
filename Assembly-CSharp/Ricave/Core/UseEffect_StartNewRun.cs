using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_StartNewRun : UseEffect
    {
        protected UseEffect_StartNewRun()
        {
        }

        public UseEffect_StartNewRun(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (Get.InLobby)
            {
                Structure staircase = usable as Structure;
                if (staircase != null)
                {
                    for (int i = 2; i >= 1; i--)
                    {
                        Vector3Int pos = staircase.Position + staircase.DirectionCardinal * i;
                        if (pos.InBounds() && (Get.CellsInfo.CanPassThrough(pos) || pos == Get.NowControlledActor.Position) && !Get.CellsInfo.IsFallingAt(pos, Vector3IntUtility.Down, false, true, false))
                        {
                            yield return new Instruction_Immediate(delegate
                            {
                                Get.Progress.SetPreferredRespawn(pos, -staircase.DirectionCardinal);
                            });
                            break;
                        }
                    }
                }
            }
            RunConfig runConfig = UsePrompt.Choice as RunConfig;
            RunConfig runConfig2;
            if (runConfig != null)
            {
                runConfig2 = runConfig;
            }
            else
            {
                DifficultySpec difficultySpec = UsePrompt.Choice as DifficultySpec;
                if (difficultySpec != null)
                {
                    runConfig2 = new RunConfig(Get.Run_Main1, Rand.Int, difficultySpec, null, "Current", null, false, null, false, false, null, null);
                }
                else
                {
                    runConfig2 = new RunConfig(Get.Run_Main1, Rand.Int, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null);
                }
            }
            yield return new Instruction_StartNewRun(runConfig2);
            yield break;
        }
    }
}