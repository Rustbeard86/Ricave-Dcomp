using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_Receive : UseEffect
    {
        public EntitySpec ItemSpec
        {
            get
            {
                return this.itemSpec;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.itemSpec);
            }
        }

        protected UseEffect_Receive()
        {
        }

        public UseEffect_Receive(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_Receive)clone).itemSpec = this.itemSpec;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user == null)
            {
                yield break;
            }
            Item item = Maker.Make<Item>(this.itemSpec, null, false, false, true);
            foreach (Instruction instruction in InstructionSets_Actor.AddToInventoryOrSpawnNear(user, item))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private EntitySpec itemSpec;
    }
}