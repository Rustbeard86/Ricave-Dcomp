using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_AddToInventory : Action
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
                return Calc.CombineHashes<int, int, int>(this.actor.MyStableHash, this.item.MyStableHash, 402915368);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.item, this.actor);
            }
        }

        protected Action_Debug_AddToInventory()
        {
        }

        public Action_Debug_AddToInventory(ActionSpec spec, Actor actor, Item item)
            : base(spec)
        {
            this.actor = actor;
            this.item = item;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            foreach (Instruction instruction in InstructionSets_Actor.AddToInventory(this.actor, this.item))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (!this.actor.IsNowControlledActor && this.item.Spec.Item.IsEquippableWeapon && this.actor.Inventory.EquippedWeapon == null && this.actor.Inventory.Contains(this.item))
            {
                foreach (Instruction instruction2 in InstructionSets_Actor.Equip(this.actor, this.item))
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Item item;
    }
}