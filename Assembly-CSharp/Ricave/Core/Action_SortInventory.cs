using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_SortInventory : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.actor.MyStableHash, 859476302);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor);
            }
        }

        protected Action_SortInventory()
        {
        }

        public Action_SortInventory(ActionSpec spec, Actor actor)
            : base(spec)
        {
            this.actor = actor;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return ignoreActorState || this.actor.Spawned;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.actor);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            List<Item> list = new List<Item>();
            Item[,] backpackItems = this.actor.Inventory.BackpackItems;
            int upperBound = backpackItems.GetUpperBound(0);
            int num = backpackItems.GetUpperBound(1);
            for (int i = backpackItems.GetLowerBound(0); i <= upperBound; i++)
            {
                for (int j = backpackItems.GetLowerBound(1); j <= num; j++)
                {
                    Item item3 = backpackItems[i, j];
                    if (item3 != null)
                    {
                        list.Add(item3);
                    }
                }
            }
            List<Item> sortedItems = list.OrderBy<Item, int>(delegate (Item item)
            {
                int num2;
                if (item.Spec.Item.Generator_IsPotion)
                {
                    num2 = 0;
                }
                else if (item.Spec.Item.Generator_IsScroll)
                {
                    num2 = 1;
                }
                else if (item.Spec.Item.IsEquippable)
                {
                    num2 = 2;
                }
                else
                {
                    num2 = 3;
                }
                return num2;
            }).ThenBy<Item, string>(delegate (Item item)
            {
                if (!item.Identified)
                {
                    return item.Label;
                }
                return item.Spec.SpecID;
            }).ToList<Item>();
            foreach (Item item2 in list)
            {
                foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(item2))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator2 = null;
            }
            List<Item>.Enumerator enumerator = default(List<Item>.Enumerator);
            int index = 0;
            Inventory_Backpack backpack = this.actor.Inventory.Backpack;
            int y = 0;
            while (y < backpack.Height && index < sortedItems.Count)
            {
                int x = 0;
                while (x < backpack.Width && index < sortedItems.Count)
                {
                    yield return new Instruction_AddToInventory(this.actor, sortedItems[index], new Vector2Int?(new Vector2Int(x, y)), null);
                    num = index;
                    index = num + 1;
                    num = x;
                    x = num + 1;
                }
                num = y;
                y = num + 1;
            }
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;
    }
}