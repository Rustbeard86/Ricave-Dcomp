using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UseEffect_BlindingFlash : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public int TurnsBlinded
        {
            get
            {
                return this.turnsBlinded;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(RichText.Turns(" ({0})".Formatted(StringUtility.TurnsString(this.turnsBlinded))));
            }
        }

        protected UseEffect_BlindingFlash()
        {
        }

        public UseEffect_BlindingFlash(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_BlindingFlash)clone).turnsBlinded = this.turnsBlinded;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user == null)
            {
                yield break;
            }
            foreach (Actor actor in Get.World.Actors.ToTemporaryList<Actor>())
            {
                if (actor != user && actor.Spawned && actor.Sees(user, null))
                {
                    Condition_Blind condition_Blind = new Condition_Blind(Get.Condition_Blind, this.turnsBlinded);
                    foreach (Instruction instruction in InstructionSets_Misc.AddCondition(condition_Blind, actor.Conditions, user != null && user.IsPlayerParty, true))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                    yield return new Instruction_SetAttackTarget(actor, null);
                    yield return new Instruction_Awareness_Clear(actor);
                    actor = null;
                }
            }
            List<Actor>.Enumerator enumerator = default(List<Actor>.Enumerator);
            yield break;
            yield break;
        }

        [Saved]
        private int turnsBlinded;
    }
}