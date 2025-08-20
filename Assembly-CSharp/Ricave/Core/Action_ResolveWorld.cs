using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_ResolveWorld : Action
    {
        protected override int RandSeedPart
        {
            get
            {
                return 812194389;
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose;
            }
        }

        private int MaxPenaltyRatsCount
        {
            get
            {
                if (!Get.WorldInfo.EscapingMode)
                {
                    return 1;
                }
                return 2;
            }
        }

        private int SpawnRatAfterTurnsWithNoKill
        {
            get
            {
                if (!Get.WorldInfo.EscapingMode)
                {
                    return 100;
                }
                return 50;
            }
        }

        protected Action_ResolveWorld()
        {
        }

        public Action_ResolveWorld(ActionSpec spec)
            : base(spec)
        {
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return false;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            IEnumerator<Instruction> enumerator;
            if (!Get.MainActor.ConditionsAccumulated.StopIdentification)
            {
                Item item = Get.MainActor.Inventory.UnidentifiedItemsInIdentifyOrder.FirstOrDefault<Item>();
                if (item != null)
                {
                    int num = Calc.RoundToIntHalfUp(Get.MainActor.ConditionsAccumulated.IdentificationRateMultiplier);
                    if (num >= 1)
                    {
                        foreach (Instruction instruction in InstructionSets_Entity.Identify(item, num, true))
                        {
                            yield return instruction;
                        }
                        enumerator = null;
                    }
                }
            }
            foreach (Actor actor in Get.World.Actors)
            {
                int hpmanaStaminaToLose = this.GetHPManaStaminaToLose(actor.HP, actor.MaxHP);
                if (hpmanaStaminaToLose > 0)
                {
                    yield return new Instruction_ChangeHP(actor, -hpmanaStaminaToLose);
                }
                int hpmanaStaminaToLose2 = this.GetHPManaStaminaToLose(actor.Mana, actor.MaxMana);
                if (hpmanaStaminaToLose2 > 0)
                {
                    yield return new Instruction_ChangeMana(actor, -hpmanaStaminaToLose2);
                }
                int hpmanaStaminaToLose3 = this.GetHPManaStaminaToLose(actor.Stamina, actor.MaxStamina);
                if (hpmanaStaminaToLose3 > 0)
                {
                    yield return new Instruction_ChangeStamina(actor, -hpmanaStaminaToLose3);
                }
                actor = null;
            }
            List<Actor>.Enumerator enumerator2 = default(List<Actor>.Enumerator);
            foreach (BurrowManager.BurrowedActor burrowedActor in Get.BurrowManager.BurrowedActors.ToTemporaryList<BurrowManager.BurrowedActor>())
            {
                Vector3Int unburrowPos;
                if (Get.TurnManager.CurrentSequence - burrowedActor.BurrowSequence >= 36 && burrowedActor.Actor.HP > 0 && SpawnPositionFinder.UnburrowPos(burrowedActor.Actor, burrowedActor.BurrowedAt, out unburrowPos))
                {
                    foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(burrowedActor.Actor, unburrowPos, null))
                    {
                        yield return instruction2;
                    }
                    enumerator = null;
                    yield return new Instruction_RemoveFromBurrowedActors(burrowedActor.Actor);
                    yield return new Instruction_VisualEffect(Get.VisualEffect_Unburrow, unburrowPos);
                }
                unburrowPos = default(Vector3Int);
                burrowedActor = null;
            }
            List<BurrowManager.BurrowedActor>.Enumerator enumerator3 = default(List<BurrowManager.BurrowedActor>.Enumerator);
            foreach (Instruction instruction3 in this.CheckSpawnPenaltyRats())
            {
                yield return instruction3;
            }
            enumerator = null;
            LessonSpec currentLesson = Get.LessonManager.CurrentLesson;
            if (currentLesson != null && currentLesson.DespawnGate && !Get.LessonManager.DespawnedGate)
            {
                foreach (Instruction instruction4 in TutorialUtility.DeSpawnClosestGate())
                {
                    yield return instruction4;
                }
                enumerator = null;
            }
            Vector3Int vector3Int;
            if (Get.RunSpec == Get.Run_Training && Get.RunConfig.TrainingActorSpec != null && !Get.World.AnyEntityOfSpec(Get.RunConfig.TrainingActorSpec) && SpawnPositionFinder.FromFloorBars(out vector3Int))
            {
                Actor actor2;
                if (Get.RunConfig.TrainingBoss)
                {
                    actor2 = BossGenerator.Boss(Get.RunConfig.TrainingActorSpec, null, null);
                }
                else
                {
                    actor2 = Maker.Make<Actor>(Get.RunConfig.TrainingActorSpec, null, false, false, true);
                }
                foreach (Instruction instruction5 in InstructionSets_Entity.Spawn(actor2, vector3Int, null))
                {
                    yield return instruction5;
                }
                enumerator = null;
            }
            foreach (WorldSituation worldSituation in Get.World.WorldSituationsManager.Situations)
            {
                foreach (Instruction instruction6 in worldSituation.MakeResolveWorldInstructions())
                {
                    yield return instruction6;
                }
                enumerator = null;
            }
            List<WorldSituation>.Enumerator enumerator4 = default(List<WorldSituation>.Enumerator);
            foreach (Instruction instruction7 in Get.WorldEventsManager.MakeCheckDoEventInstructions())
            {
                yield return instruction7;
            }
            enumerator = null;
            foreach (Instruction instruction8 in Get.ModsEventsManager.CallEvent(ModEventWithInstructionsType.ResolveWorld, null))
            {
                yield return instruction8;
            }
            enumerator = null;
            if (Get.Player.TurnsCanRewind < Get.Player.MaxTurnsCanRewind)
            {
                if (Get.Player.ReplenishTurnsCanRewindTurnsPassed >= 2)
                {
                    yield return new Instruction_ChangeReplenishTurnsCanRewindTurnsPassed(-Get.Player.ReplenishTurnsCanRewindTurnsPassed);
                    yield return new Instruction_ChangeTurnsCanRewind(1);
                    yield return new Instruction_Immediate(delegate
                    {
                        Get.RewindUI.StartReplenishAnimation();
                    });
                }
                else
                {
                    yield return new Instruction_ChangeReplenishTurnsCanRewindTurnsPassed(1);
                }
            }
            yield return new Instruction_AddSequence(Get.WorldSequenceable, 12);
            yield break;
            yield break;
        }

        private int GetHPManaStaminaToLose(int points, int maxPoints)
        {
            if (points <= maxPoints)
            {
                return 0;
            }
            return Math.Max(Calc.RoundToIntHalfUp((float)(points - maxPoints) * 0.25f), 1);
        }

        private IEnumerable<Instruction> CheckSpawnPenaltyRats()
        {
            if (Get.RunSpec == Get.Run_Tutorial || Get.RunSpec == Get.Run_Training)
            {
                yield break;
            }
            int num = 0;
            using (List<Entity>.Enumerator enumerator = Get.World.GetEntitiesOfSpec(Get.Entity_Rat).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (((Actor)enumerator.Current).IsPenaltyRat)
                    {
                        num++;
                    }
                }
            }
            if (num >= this.MaxPenaltyRatsCount)
            {
                yield break;
            }
            int num2 = Get.TurnManager.InceptionSequence;
            if (Get.KillCounter.LastKillSequence != null)
            {
                num2 = Math.Max(num2, Get.KillCounter.LastKillSequence.Value);
            }
            if (Get.WorldInfo.LastPenaltyRatSpawnSequence != null)
            {
                num2 = Math.Max(num2, Get.WorldInfo.LastPenaltyRatSpawnSequence.Value);
            }
            Vector3Int vector3Int;
            if (Get.TurnManager.CurrentSequence - num2 > this.SpawnRatAfterTurnsWithNoKill * 12 && SpawnPositionFinder.FromFloorBars(out vector3Int))
            {
                Actor actor = Maker.Make<Actor>(Get.Entity_Rat, delegate (Actor x)
                {
                    x.IsPenaltyRat = true;
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                    DifficultyUtility.AddConditionsForDifficulty(x);
                    x.CalculateInitialHPManaAndStamina();
                }, false, false, true);
                foreach (Instruction instruction in InstructionSets_Entity.Spawn(actor, vector3Int, null))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator2 = null;
                yield return new Instruction_SetLastPenaltyRatSpawnSequence(Get.TurnManager.CurrentSequence);
            }
            yield break;
            yield break;
        }
    }
}