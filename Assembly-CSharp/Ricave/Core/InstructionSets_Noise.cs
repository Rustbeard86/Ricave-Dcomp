using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public static class InstructionSets_Noise
    {
        public static IEnumerable<Instruction> MakeNoise(Vector3Int source, int radius, NoiseType type, Actor responsible, bool throughWalls = false, bool canAffectOptionalChallengeRoom = true)
        {
            List<Actor> actors = Get.World.Actors;
            int num;
            for (int i = 0; i < actors.Count; i = num + 1)
            {
                Actor actor = actors[i];
                if (InstructionSets_Noise.Hears(actor, source, radius, type, responsible, throughWalls, canAffectOptionalChallengeRoom))
                {
                    if (actor.Conditions.AnyOfSpec(Get.Condition_Sleeping))
                    {
                        foreach (Instruction instruction in InstructionSets_Actor.WakeUp(actor, InstructionSets_Noise.GetWakeUpReason(actor, type, responsible)))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator = null;
                    }
                    if ((responsible != null && actor.IsHostile(responsible)) || (responsible == null && !actor.Sees(source, null)))
                    {
                        foreach (Instruction instruction2 in InstructionSets_Actor.GoCheckPossibleEnemyLocation(actor, source))
                        {
                            yield return instruction2;
                        }
                        IEnumerator<Instruction> enumerator = null;
                    }
                    actor = null;
                }
                num = i;
            }
            yield break;
            yield break;
        }

        private static bool Hears(Actor actor, Vector3Int source, int radius, NoiseType type, Actor responsible, bool throughWalls, bool canAffectOptionalChallengeRoom)
        {
            return actor.Position.GetGridDistance(source) <= radius && (throughWalls || LineOfSight.IsLineOfSight(source, actor.Position) || LineOfSight.IsLineOfFire(source, actor.Position)) && ((type != NoiseType.ActorMoved && type != NoiseType.ActorUsedSomething) || actor.IsHostile(responsible) || actor.Faction == null || actor.Faction != responsible.Faction || (InstructionSets_Noise.< Hears > g__SeesAnyHostilePlayerParty | 1_0(responsible) && !responsible.IsPlayerParty)) && ((type != NoiseType.ActorMoved && type != NoiseType.ActorUsedSomething) || actor.IsHostile(responsible) || (actor.Faction != null && actor.Faction == responsible.Faction)) && (type != NoiseType.ActorWokeUp || (!actor.IsHostile(responsible) && actor.Faction == responsible.Faction)) && (canAffectOptionalChallengeRoom || !actor.Position.InRoom(Get.Room_OptionalChallengeRoom));
        }

        private static string GetWakeUpReason(Actor actor, NoiseType noiseType, Actor responsible)
        {
            if (noiseType == NoiseType.StructureSetOnFire)
            {
                return "WakeUpReason_StructureSetOnFire".Translate();
            }
            if (noiseType == NoiseType.ActorWokeUp)
            {
                return "WakeUpReason_ActorWokeUp".Translate();
            }
            if (noiseType != NoiseType.ActorMoved)
            {
                return "WakeUpReason_LoudNoise".Translate();
            }
            if (actor.IsHostile(responsible))
            {
                return "WakeUpReason_HostileFootsteps".Translate();
            }
            return "WakeUpReason_AlertAllyPassing".Translate();
        }

        [CompilerGenerated]
        internal static bool <Hears>g__SeesAnyHostilePlayerParty|1_0(Actor actor)
		{
			foreach (Actor actor2 in Get.PlayerParty)
			{
				if (actor.Sees(actor2, null) && actor.IsHostile(actor2))
				{
					return true;
				}
			}
			return false;
		}
	}
}