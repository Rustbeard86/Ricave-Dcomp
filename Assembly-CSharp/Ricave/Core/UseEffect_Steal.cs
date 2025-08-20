using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class UseEffect_Steal : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        protected UseEffect_Steal()
        {
        }

        public UseEffect_Steal(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user == null || target == user)
            {
                yield break;
            }
            Entity entity = target.Entity;
            Actor targetActor = entity as Actor;
            if (targetActor == null)
            {
                yield break;
            }
            Item item = this.FindItemToSteal(targetActor);
            if (item == null)
            {
                yield break;
            }
            foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(item))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            foreach (Instruction instruction2 in InstructionSets_Actor.AddToInventoryOrSpawnNear(user, item))
            {
                yield return instruction2;
            }
            enumerator = null;
            if (ActionUtility.TargetConcernsPlayer(user) || ActionUtility.TargetConcernsPlayer(item) || ActionUtility.TargetConcernsPlayer(targetActor))
            {
                yield return new Instruction_PlayLog("StoleItem".Translate(user, item, targetActor));
            }
            yield break;
            yield break;
        }

        private Item FindItemToSteal(Actor stealFrom)
        {
            if (stealFrom.Inventory.EquippedWeapon != null && !stealFrom.Inventory.EquippedWeapon.Cursed)
            {
                return stealFrom.Inventory.EquippedWeapon;
            }
            Item item;
            if (stealFrom.Inventory.Equipped.Items.Where<Item>((Item x) => !x.Cursed).TryGetRandomElement<Item>(out item))
            {
                return item;
            }
            if (stealFrom.Inventory.AllNonEquippedItems.TryGetRandomElement<Item>(out item))
            {
                return item;
            }
            return null;
        }
    }
}