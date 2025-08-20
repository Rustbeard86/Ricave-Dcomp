using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_ClampHPToMax : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        protected UseEffect_ClampHPToMax()
        {
        }

        public UseEffect_ClampHPToMax(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!target.IsEntity || target.Entity.HP <= target.Entity.MaxHP || target.Entity.MaxHP <= 0)
            {
                yield break;
            }
            int num = target.Entity.HP - target.Entity.MaxHP;
            yield return new Instruction_ChangeHP(target.Entity, -num);
            yield break;
        }
    }
}