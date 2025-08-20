using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_DestroyConnected : UseEffect
    {
        public EntitySpec EntitySpec
        {
            get
            {
                return this.entitySpec;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.entitySpec);
            }
        }

        protected UseEffect_DestroyConnected()
        {
        }

        public UseEffect_DestroyConnected(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_DestroyConnected)clone).entitySpec = this.entitySpec;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            foreach (Instruction instruction in UseEffect_DestroyConnected.DestroyConnectedInstructions(target.Position, this.entitySpec))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> DestroyConnectedInstructions(Vector3Int start, EntitySpec entitySpec)
        {
            List<Entity> list = new List<Entity>();
            Vector3Int start2 = start;
            Predicate<Vector3Int> <> 9__0;
            Predicate<Vector3Int> predicate;
            if ((predicate = <> 9__0) == null)
            {
                predicate = (<> 9__0 = (Vector3Int x) => x == start || Get.World.AnyEntityOfSpecAt(x, entitySpec));
            }
            foreach (Vector3Int vector3Int in FloodFiller.FloodFillEnumerable(start2, predicate))
            {
                Entity firstEntityOfSpecAt = Get.World.GetFirstEntityOfSpecAt(vector3Int, entitySpec);
                if (firstEntityOfSpecAt != null)
                {
                    list.Add(firstEntityOfSpecAt);
                }
            }
            foreach (Entity entity in list)
            {
                if (entity.Spawned)
                {
                    for (; ; )
                    {
                        Entity firstEntityOfSpecAt2 = Get.World.GetFirstEntityOfSpecAt(entity.Position, entitySpec);
                        if (firstEntityOfSpecAt2 == null)
                        {
                            break;
                        }
                        foreach (Instruction instruction in InstructionSets_Entity.Destroy(firstEntityOfSpecAt2, null, null))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator3 = null;
                    }
                    entity = null;
                }
            }
            List<Entity>.Enumerator enumerator2 = default(List<Entity>.Enumerator);
            yield break;
            yield break;
        }

        [Saved]
        private EntitySpec entitySpec;
    }
}