using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Unequip : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Item Item
        {
            get
            {
                return this.item;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int, int>(this.actor.MyStableHash, this.item.MyStableHash, 384750909);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor, this.item);
            }
        }

        protected Action_Unequip()
        {
        }

        public Action_Unequip(ActionSpec spec, Actor actor, Item item)
            : base(spec)
        {
            this.actor = actor;
            this.item = item;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (this.item.Spawned)
            {
                return false;
            }
            if (this.item.Cursed)
            {
                return false;
            }
            if (!ignoreActorState)
            {
                if (!this.actor.Spawned)
                {
                    return false;
                }
                if (!this.actor.Inventory.Equipped.IsEquipped(this.item))
                {
                    return false;
                }
                if (this.actor.Inventory.BackpackAndQuickbarFull)
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.actor) || ActionUtility.TargetConcernsPlayer(this.item);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(this.item))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (this.actor.IsNowControlledActor)
            {
                yield return new Instruction_Sound(this.item.Spec.Item.IsEquippableWeapon ? Get.Sound_Unequip : Get.Sound_Unwear, null, 1f, 1f);
            }
            yield return new Instruction_AddToInventory(this.actor, this.item, null, null);
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Item item;
    }
}