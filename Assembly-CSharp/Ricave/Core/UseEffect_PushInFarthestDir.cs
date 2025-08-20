using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_PushInFarthestDir : UseEffect
    {
        public int Distance
        {
            get
            {
                return this.distance;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(StringUtility.CellsString(this.distance));
            }
        }

        protected UseEffect_PushInFarthestDir()
        {
        }

        public UseEffect_PushInFarthestDir(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public UseEffect_PushInFarthestDir(UseEffectSpec spec, int distance, float chance = 1f, int usesLeft = 0)
            : base(spec, chance, usesLeft, null, false)
        {
            this.distance = distance;
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_PushInFarthestDir)clone).distance = this.distance;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!target.IsEntity || !target.Spawned || !(target.Entity is Actor))
            {
                yield break;
            }
            Actor actor = target.Entity as Actor;
            if (actor != null && actor.ImmuneToPushing)
            {
                yield break;
            }
            Vector3Int value = DamageUtility.DeduceImpactSource(user, target, originalTarget).Value;
            Vector3Int item;
            if (target.Position == value)
            {
                Vector3IntUtility.Directions.TryGetRandomElement<Vector3Int>(out item);
            }
            else
            {
                Vector3Int vector3Int = target.Position - value;
                List<Vector3Int> list = new List<Vector3Int>();
                list.Add(vector3Int.NormalizedToDir());
                list.AddRange(vector3Int.NormalizedToDir().GetSimilarDirectionsGrouped().Skip<List<Vector3Int>>(1)
                    .First<List<Vector3Int>>());
                List<ValueTuple<Vector3Int, int>> list2 = list.Select<Vector3Int, ValueTuple<Vector3Int, int>>((Vector3Int x) => new ValueTuple<Vector3Int, int>(x, PushUtility.GetExpectedPushDistance(target.Position, x, this.distance))).ToList<ValueTuple<Vector3Int, int>>();
                int maxDist = list2.Max<ValueTuple<Vector3Int, int>>(([TupleElementNames(new string[] { "x", null })] ValueTuple<Vector3Int, int> x) => x.Item2);
                ValueTuple<Vector3Int, int> valueTuple;
                list2.Where<ValueTuple<Vector3Int, int>>(([TupleElementNames(new string[] { "x", null })] ValueTuple<Vector3Int, int> x) => x.Item2 == maxDist).TryGetRandomElement<ValueTuple<Vector3Int, int>>(out valueTuple);
                item = valueTuple.Item1;
            }
            foreach (Instruction instruction in InstructionSets_Entity.Push(target.Entity, item, this.distance, false, true, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private int distance;
    }
}