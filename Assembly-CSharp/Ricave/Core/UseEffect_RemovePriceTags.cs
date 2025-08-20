using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_RemovePriceTags : UseEffect
    {
        protected UseEffect_RemovePriceTags()
        {
        }

        public UseEffect_RemovePriceTags(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            foreach (Vector3Int vector3Int in target.Position.AdjacentCells())
            {
                if (vector3Int.InBounds())
                {
                    foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int))
                    {
                        Item item = entity as Item;
                        if (item != null && item.ForSale)
                        {
                            yield return new Instruction_RemovePriceTag(item);
                        }
                    }
                    List<Entity>.Enumerator enumerator2 = default(List<Entity>.Enumerator);
                }
            }
            IEnumerator<Vector3Int> enumerator = null;
            yield break;
            yield break;
        }
    }
}