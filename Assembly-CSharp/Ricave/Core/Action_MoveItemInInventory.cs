using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_MoveItemInInventory : Action
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

        public Vector2Int? ToSlot
        {
            get
            {
                return this.toSlot;
            }
        }

        public int? ToQuickbar
        {
            get
            {
                return this.toQuickbar;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int, Vector2Int?, int?, int>(this.actor.MyStableHash, this.item.MyStableHash, this.toSlot, this.toQuickbar, 734180961);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor, this.item);
            }
        }

        protected Action_MoveItemInInventory()
        {
        }

        public Action_MoveItemInInventory(ActionSpec spec, Actor actor, Item item, Vector2Int? toSlot, int? toQuickbar)
            : base(spec)
        {
            this.actor = actor;
            this.item = item;
            this.toSlot = toSlot;
            this.toQuickbar = toQuickbar;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (this.item.Spawned)
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
                if (this.actor.Inventory.Equipped.IsEquipped(this.item) && this.item.Cursed)
                {
                    return false;
                }
                if (this.toSlot != null && this.actor.Inventory.Backpack.GetCurrentSlotOf(this.item) == this.toSlot)
                {
                    return false;
                }
                if (this.toQuickbar != null)
                {
                    int? currentIndexOf = this.actor.Inventory.Quickbar.GetCurrentIndexOf(this.item);
                    int? num = this.toQuickbar;
                    if ((currentIndexOf.GetValueOrDefault() == num.GetValueOrDefault()) & (currentIndexOf != null == (num != null)))
                    {
                        return false;
                    }
                }
                if (this.actor.Inventory.Equipped.IsEquipped(this.item))
                {
                    if (this.toSlot != null && this.actor.Inventory.Backpack.Items[this.toSlot.Value.x, this.toSlot.Value.y] != null)
                    {
                        return false;
                    }
                    if (this.toQuickbar != null && this.actor.Inventory.Quickbar.Items[this.toQuickbar.Value] != null)
                    {
                        return false;
                    }
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
            Item existing;
            if (this.toSlot != null)
            {
                existing = this.actor.Inventory.Backpack.Items[this.toSlot.Value.x, this.toSlot.Value.y];
            }
            else if (this.toQuickbar != null)
            {
                existing = this.actor.Inventory.Quickbar.Items[this.toQuickbar.Value];
            }
            else
            {
                existing = null;
            }
            IEnumerator<Instruction> enumerator;
            if (existing != null)
            {
                foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(existing))
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
            yield return new Instruction_AddToInventory(this.actor, this.item, this.toSlot, this.toQuickbar);
            if (existing != null)
            {
                yield return new Instruction_AddToInventory(this.actor, existing, backpackSlot, quickbarSlot);
            }
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Item item;

        [Saved]
        private Vector2Int? toSlot;

        [Saved]
        private int? toQuickbar;
    }
}