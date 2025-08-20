using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Transfer : Action
    {
        public Actor From
        {
            get
            {
                return this.from;
            }
        }

        public Actor To
        {
            get
            {
                return this.to;
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
                return Calc.CombineHashes<int, int, int, int>(this.from.MyStableHash, this.to.MyStableHash, this.item.MyStableHash, 38095899);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.from, this.item, this.to);
            }
        }

        protected Action_Transfer()
        {
        }

        public Action_Transfer(ActionSpec spec, Actor from, Actor to, Item item)
            : base(spec)
        {
            this.from = from;
            this.to = to;
            this.item = item;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (this.item.Spawned)
            {
                return false;
            }
            if (!this.to.Spawned)
            {
                return false;
            }
            if (!InstructionSets_Actor.HasSpaceToPickUp(this.to, this.item))
            {
                return false;
            }
            if (!ignoreActorState)
            {
                if (!this.from.Spawned)
                {
                    return false;
                }
                if (!Action_Transfer.CanReach(this.from, this.to))
                {
                    return false;
                }
                if (!this.from.Inventory.Contains(this.item))
                {
                    return false;
                }
                if (this.from.Inventory.Equipped.IsEquipped(this.item) && this.item.Cursed)
                {
                    return false;
                }
                if (this.to.IsHostile(this.from))
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.from) || ActionUtility.TargetConcernsPlayer(this.to) || ActionUtility.TargetConcernsPlayer(this.item);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(this.item))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            foreach (Instruction instruction2 in InstructionSets_Actor.AddToInventoryOrSpawnNear(this.to, this.item))
            {
                yield return instruction2;
            }
            enumerator = null;
            if (!this.to.IsNowControlledActor && !this.to.IsPlayerParty && this.item.Spec.Item.IsEquippableWeapon && this.to.Inventory.EquippedWeapon == null && this.to.Inventory.Contains(this.item))
            {
                foreach (Instruction instruction3 in InstructionSets_Actor.Equip(this.to, this.item))
                {
                    yield return instruction3;
                }
                enumerator = null;
            }
            yield return new Instruction_Sound(Get.Sound_PickUpItem, new Vector3?(this.to.Position), 1f, 1f);
            if (ActionUtility.TargetConcernsPlayer(this.from) || ActionUtility.TargetConcernsPlayer(this.to))
            {
                yield return new Instruction_PlayLog("ActorGivesItemToActor".Translate(this.from, this.item, this.to));
            }
            yield break;
            yield break;
        }

        public static bool CanReach(Actor giver, Actor taker)
        {
            return giver.Position.GetGridDistance(taker.Position) <= 2 && !Get.CellsInfo.IsFallingAt(giver.Position, giver, false) && LineOfSight.IsLineOfFire(giver.Position, taker.Position) && LineOfSight.IsLineOfSight(giver.Position, taker.Position);
        }

        [Saved]
        private Actor from;

        [Saved]
        private Actor to;

        [Saved]
        private Item item;
    }
}