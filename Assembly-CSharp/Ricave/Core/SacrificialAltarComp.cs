using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class SacrificialAltarComp : EntityComp
    {
        public new SacrificialAltarCompProps Props
        {
            get
            {
                return (SacrificialAltarCompProps)base.Props;
            }
        }

        public int CountSacrificed
        {
            get
            {
                return this.countSacrificed;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.countSacrificed = value;
            }
        }

        public bool BossSacrificed
        {
            get
            {
                return this.bossSacrificed;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.bossSacrificed = value;
            }
        }

        public Item Reward
        {
            get
            {
                return this.reward;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.reward = value;
            }
        }

        protected SacrificialAltarComp()
        {
        }

        public SacrificialAltarComp(Entity parent)
            : base(parent)
        {
        }

        public override IEnumerable<Instruction> MakeActorDestroyedHereInstructions(Actor actor)
        {
            if (actor.IsNowControlledActor || actor.IsMainActor)
            {
                yield return new Instruction_PlayLog("YouSacrificedYourself".Translate());
                if (!Get.Achievement_SacrificeYourself.IsCompleted)
                {
                    yield return new Instruction_CompleteAchievement(Get.Achievement_SacrificeYourself);
                }
                yield return new Instruction_VisualEffect(Get.VisualEffect_Sacrifice, base.Parent.Position);
                yield break;
            }
            if (actor.KilledExperience <= 0)
            {
                yield break;
            }
            yield return new Instruction_SacrificialAltar_ChangeCountSacrificed(this, 1);
            if (actor.IsBoss)
            {
                yield return new Instruction_SacrificialAltar_SetBossSacrificed(this, true);
            }
            yield return new Instruction_VisualEffect(Get.VisualEffect_Sacrifice, base.Parent.Position);
            if (this.countSacrificed >= 2)
            {
                foreach (Instruction instruction in InstructionSets_Entity.Destroy(base.Parent, null, null))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                if (this.reward != null)
                {
                    Vector3Int vector3Int = SpawnPositionFinder.Near(base.Parent.Position, this.reward, false, false, null);
                    foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(this.reward, vector3Int, null))
                    {
                        yield return instruction2;
                    }
                    enumerator = null;
                    yield return new Instruction_StartItemJumpAnimation(this.reward);
                }
                if (this.bossSacrificed)
                {
                    int num;
                    for (int i = 0; i < 3; i = num + 1)
                    {
                        Item gold = ItemGenerator.Gold((i < 2) ? 20 : 10);
                        Vector3Int vector3Int2 = SpawnPositionFinder.Near(base.Parent.Position, gold, false, false, null);
                        foreach (Instruction instruction3 in InstructionSets_Entity.Spawn(gold, vector3Int2, null))
                        {
                            yield return instruction3;
                        }
                        enumerator = null;
                        yield return new Instruction_StartItemJumpAnimation(gold);
                        gold = null;
                        num = i;
                    }
                }
            }
            if (!Get.Achievement_Sacrifice.IsCompleted)
            {
                yield return new Instruction_CompleteAchievement(Get.Achievement_Sacrifice);
            }
            yield break;
            yield break;
        }

        [Saved]
        private int countSacrificed;

        [Saved]
        private bool bossSacrificed;

        [Saved]
        private Item reward;

        private const int CountToSacrifice = 2;
    }
}