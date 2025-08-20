using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_PickUpItem : Action
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
                return Calc.CombineHashes<int, int, int>(this.actor.MyStableHash, this.item.MyStableHash, 495069381);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor, this.item);
            }
        }

        protected Action_PickUpItem()
        {
        }

        public Action_PickUpItem(ActionSpec spec, Actor actor, Item item)
            : base(spec)
        {
            this.actor = actor;
            this.item = item;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (!this.item.Spawned)
            {
                return false;
            }
            if (this.item.ForSale)
            {
                return false;
            }
            if (!ignoreActorState)
            {
                if (!this.actor.Spawned)
                {
                    return false;
                }
                if (this.actor.Position != this.item.Position)
                {
                    return false;
                }
                if (!InstructionSets_Actor.HasSpaceToPickUp(this.actor, this.item))
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
            foreach (Instruction instruction in InstructionSets_Actor.PickUpItem(this.actor, this.item, false))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Item item;
    }
}