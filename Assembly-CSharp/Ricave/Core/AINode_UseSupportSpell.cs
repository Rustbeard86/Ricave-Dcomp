using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class AINode_UseSupportSpell : AINode
    {
        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            return Enumerable.Empty<Instruction>();
        }

        public override Action GetNextAction(Actor actor)
        {
            if (!actor.Spells.Any)
            {
                return null;
            }
            foreach (Actor actor2 in AINode_UseSupportSpell.GetJustSeenAlliesInTargetPriority(actor))
            {
                if ((this.onlyOnTarget == null || actor2.Spec == this.onlyOnTarget) && actor2.Spec != Get.Entity_Roothealer)
                {
                    Spell supportSpell = AINode_UseSupportSpell.GetSupportSpell(actor, actor2);
                    if (supportSpell != null)
                    {
                        return new Action_Use(Get.Action_Use, actor, supportSpell, actor2, null);
                    }
                }
            }
            return null;
        }

        public static IEnumerable<Actor> GetJustSeenAlliesInTargetPriority(Actor actor)
        {
            return from x in AIUtility.GetJustSeenAllies(actor)
                   orderby x.Position.GetGridDistance(actor.Position), x.IsPlayerParty descending, x.MyStableHash
                   select x;
        }

        public static Spell GetSupportSpell(Actor actor, Actor target)
        {
            foreach (Spell spell in actor.Spells.All)
            {
                if (spell.Spec.IsSupportSpellForAI && (!spell.UseEffects.AnyOfSpec(Get.UseEffect_Heal) || (float)target.HP < (float)target.MaxHP * 0.8f) && actor.CanUseOn(spell, target, null, false, null))
                {
                    return spell;
                }
            }
            return null;
        }

        [Saved]
        private EntitySpec onlyOnTarget;
    }
}