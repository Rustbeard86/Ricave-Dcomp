using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Equip : Action
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
                return Calc.CombineHashes<int, int, int>(this.actor.MyStableHash, this.item.MyStableHash, 95634218);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor, this.item);
            }
        }

        protected Action_Equip()
        {
        }

        public Action_Equip(ActionSpec spec, Actor actor, Item item)
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
            if (!this.item.Spec.Item.IsEquippable)
            {
                return false;
            }
            if (!ignoreActorState)
            {
                if (!this.actor.Spawned)
                {
                    return false;
                }
                if (!this.actor.Inventory.Contains(this.item))
                {
                    return false;
                }
                if (this.actor.Inventory.Equipped.IsEquipped(this.item))
                {
                    return false;
                }
                Item equippedItemCollidingWith = this.actor.Inventory.Equipped.GetEquippedItemCollidingWith(this.item);
                if (equippedItemCollidingWith != null && equippedItemCollidingWith.Cursed)
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
            Item colliding = this.actor.Inventory.Equipped.GetEquippedItemCollidingWith(this.item);
            IEnumerator<Instruction> enumerator;
            if (colliding != null)
            {
                foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(colliding))
                {
                    yield return instruction;
                }
                enumerator = null;
            }
            Vector2Int? backpackSlot = this.actor.Inventory.Backpack.GetCurrentSlotOf(this.item);
            int? quickbarSlot = this.actor.Inventory.Quickbar.GetCurrentIndexOf(this.item);
            foreach (Instruction instruction2 in InstructionSets_Actor.RemoveFromInventory(this.item))
            {
                yield return instruction2;
            }
            enumerator = null;
            foreach (Instruction instruction3 in InstructionSets_Actor.Equip(this.actor, this.item))
            {
                yield return instruction3;
            }
            enumerator = null;
            if (this.actor.IsNowControlledActor)
            {
                yield return new Instruction_Sound(this.item.Spec.Item.IsEquippableWeapon ? Get.Sound_Equip : Get.Sound_Wear, null, 1f, 1f);
            }
            if (colliding != null)
            {
                yield return new Instruction_AddToInventory(this.actor, colliding, backpackSlot, quickbarSlot);
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