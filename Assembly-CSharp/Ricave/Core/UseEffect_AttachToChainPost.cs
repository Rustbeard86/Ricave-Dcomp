using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_AttachToChainPost : UseEffect
    {
        protected UseEffect_AttachToChainPost()
        {
        }

        public UseEffect_AttachToChainPost(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            if (actor == null || !actor.Spawned || actor.AttachedToChainPost != null)
            {
                yield break;
            }
            foreach (Instruction instruction in UseEffect_AttachToChainPost.SpawnChainPostAndAttachActor(actor))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> SpawnChainPostAndAttachActor(Actor targetActor)
        {
            if (!targetActor.Spawned || targetActor.AttachedToChainPost != null)
            {
                yield break;
            }
            ChainPost chainPost = Maker.Make<ChainPost>(Get.Entity_ChainPost, null, false, false, true);
            Vector3Int vector3Int = SpawnPositionFinder.Near(targetActor.Position, chainPost, false, false, null);
            foreach (Instruction instruction in InstructionSets_Entity.Spawn(chainPost, vector3Int, null))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_AttachToChainPost(targetActor, chainPost);
            yield break;
            yield break;
        }
    }
}