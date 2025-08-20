using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_UseRecipe : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public RecipeSpec Recipe
        {
            get
            {
                return this.recipe;
            }
        }

        public List<Item> Ingredients
        {
            get
            {
                return this.ingredients;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int, int>(this.actor.MyStableHash, this.recipe.MyStableHash, 905813513);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor, this.recipe);
            }
        }

        protected Action_UseRecipe()
        {
        }

        public Action_UseRecipe(ActionSpec spec, Actor actor, RecipeSpec recipe, List<Item> ingredients)
            : base(spec)
        {
            this.actor = actor;
            this.recipe = recipe;
            this.ingredients = ingredients;
            if (ingredients.Count != recipe.Ingredients.Count)
            {
                Log.Error("Created invalid UseRecipe action (ingredient counts don't match).", false);
                return;
            }
            List<EntitySpec> list = recipe.Ingredients;
            for (int i = 0; i < ingredients.Count; i++)
            {
                if (ingredients[i].Spec != list[i])
                {
                    Log.Error("Created invalid UseRecipe action (ingredient specs don't match, they must be in the correct order).", false);
                    return;
                }
            }
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            using (List<Item>.Enumerator enumerator = this.ingredients.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Spawned)
                    {
                        return false;
                    }
                }
            }
            foreach (Item item in this.ingredients)
            {
                int num = 0;
                foreach (Item item2 in this.ingredients)
                {
                    if (item == item2)
                    {
                        num++;
                    }
                }
                if (num > item.StackCount)
                {
                    return false;
                }
            }
            if (!ignoreActorState)
            {
                if (!this.actor.Spawned)
                {
                    return false;
                }
                foreach (Item item3 in this.ingredients)
                {
                    if (!this.actor.Inventory.Contains(item3))
                    {
                        return false;
                    }
                    if (item3.Cursed && this.actor.Inventory.Equipped.IsEquipped(item3))
                    {
                        return false;
                    }
                }
                if (this.recipe.ManaIngredient != 0 && this.actor.Mana < this.recipe.ManaIngredient)
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.actor);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            IEnumerator<Instruction> enumerator2;
            foreach (Item item in this.ingredients)
            {
                foreach (Instruction instruction in InstructionSets_Actor.RemoveOneFromInventory(item))
                {
                    yield return instruction;
                }
                enumerator2 = null;
            }
            List<Item>.Enumerator enumerator = default(List<Item>.Enumerator);
            if (this.recipe.ManaIngredient != 0)
            {
                yield return new Instruction_ChangeMana(this.actor, -this.recipe.ManaIngredient);
            }
            Item product = Maker.Make<Item>(this.recipe.Product, delegate (Item x)
            {
                if (RampUpUtility.AffectedByRampUp(x))
                {
                    int num = 0;
                    foreach (Item item2 in this.ingredients)
                    {
                        num = Math.Max(num, item2.RampUp);
                    }
                    x.RampUp = num;
                }
            }, false, false, true);
            if (!product.Identified)
            {
                foreach (Instruction instruction2 in InstructionSets_Entity.Identify(product, product.TurnsLeftToIdentify, false))
                {
                    yield return instruction2;
                }
                enumerator2 = null;
            }
            foreach (Instruction instruction3 in InstructionSets_Actor.AddToInventoryOrSpawnNear(this.actor, product))
            {
                yield return instruction3;
            }
            enumerator2 = null;
            if (this.actor.IsNowControlledActor)
            {
                yield return new Instruction_Sound(Get.Sound_UseRecipe, null, 1f, 1f);
            }
            if (this.actor.IsPlayerParty)
            {
                yield return new Instruction_ChangeRecipesUsed(1);
                if (product.Spec == Get.Entity_Potion_Health && !Get.Achievement_BrewHealthPotion.IsCompleted)
                {
                    yield return new Instruction_CompleteAchievement(Get.Achievement_BrewHealthPotion);
                }
            }
            if (ActionUtility.TargetConcernsPlayer(this.actor))
            {
                yield return new Instruction_PlayLog("UsedRecipe".Translate(this.actor, product));
            }
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private RecipeSpec recipe;

        [Saved]
        private List<Item> ingredients;
    }
}