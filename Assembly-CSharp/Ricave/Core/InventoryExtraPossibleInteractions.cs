using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public static class InventoryExtraPossibleInteractions
    {
        public static IEnumerable<ValueTuple<string, Action, string>> GetFor(Item item)
        {
            List<RecipeSpec> recipes = Get.Specs.GetAll<RecipeSpec>();
            int num;
            for (int i = 0; i < recipes.Count; i = num + 1)
            {
                if (recipes[i].Ingredients.Contains(item.Spec) && InventoryExtraPossibleInteractions.TryFindIngredients(Get.NowControlledActor, recipes[i], item, InventoryExtraPossibleInteractions.tmpFoundIngredients, true, true))
                {
                    RecipeSpec recipeLocal = recipes[i];
                    Func<Action> <> 9__1;
                    yield return new ValueTuple<string, Action, string>(InventoryExtraPossibleInteractions.GetLabel(recipes[i], item, InventoryExtraPossibleInteractions.tmpFoundIngredients), delegate
                    {
                        Func<Action> func;
                        if ((func = <> 9__1) == null)
                        {
                            func = (<> 9__1 = delegate
                            {
                                List<Item> list = new List<Item>();
                                if (InventoryExtraPossibleInteractions.TryFindIngredients(Get.NowControlledActor, recipeLocal, item, list, true, true))
                                {
                                    return new Action_UseRecipe(Get.Action_UseRecipe, Get.NowControlledActor, recipeLocal, list);
                                }
                                return null;
                            });
                        }
                        ActionViaInterfaceHelper.TryDo(func);
                    }, null);
                }
                num = i;
            }
            yield break;
        }

        public static bool TryFindIngredients(Actor actor, RecipeSpec recipe, Item oneIngredient, List<Item> outIngredients, bool stopOnFirstMissing = true, bool disallowUnidentifiedWithIdentificationGroup = true)
        {
            if (outIngredients != null)
            {
                outIngredients.Clear();
            }
            if (oneIngredient != null && !actor.Inventory.Contains(oneIngredient))
            {
                return false;
            }
            if (disallowUnidentifiedWithIdentificationGroup && oneIngredient != null && !oneIngredient.Identified && Get.IdentificationGroups.GetIdentificationGroup(oneIngredient.Spec) != null)
            {
                return false;
            }
            InventoryExtraPossibleInteractions.tmpNeededIngredientSpecs.Clear();
            InventoryExtraPossibleInteractions.tmpNeededIngredientSpecs.AddRange<EntitySpec>(recipe.Ingredients);
            InventoryExtraPossibleInteractions.tmpUnusedIngredients.Clear();
            foreach (Item item in actor.Inventory.AllItems)
            {
                if (InventoryExtraPossibleInteractions.tmpNeededIngredientSpecs.Contains(item.Spec) && (!disallowUnidentifiedWithIdentificationGroup || item.Identified || Get.IdentificationGroups.GetIdentificationGroup(item.Spec) == null))
                {
                    int stackCount = item.StackCount;
                    for (int i = 0; i < stackCount; i++)
                    {
                        InventoryExtraPossibleInteractions.tmpUnusedIngredients.Add(item);
                    }
                }
            }
            bool flag = false;
            bool flag2 = false;
            foreach (EntitySpec entitySpec in recipe.Ingredients)
            {
                if (!flag && oneIngredient != null && entitySpec == oneIngredient.Spec)
                {
                    if (outIngredients != null)
                    {
                        outIngredients.Add(oneIngredient);
                    }
                    InventoryExtraPossibleInteractions.tmpUnusedIngredients.Remove(oneIngredient);
                    flag = true;
                }
                else
                {
                    bool flag3 = false;
                    for (int j = 0; j < InventoryExtraPossibleInteractions.tmpUnusedIngredients.Count; j++)
                    {
                        if (InventoryExtraPossibleInteractions.tmpUnusedIngredients[j].Spec == entitySpec)
                        {
                            if (outIngredients != null)
                            {
                                outIngredients.Add(InventoryExtraPossibleInteractions.tmpUnusedIngredients[j]);
                            }
                            InventoryExtraPossibleInteractions.tmpUnusedIngredients.RemoveAt(j);
                            flag3 = true;
                            break;
                        }
                    }
                    if (!flag3)
                    {
                        flag2 = true;
                        if (stopOnFirstMissing)
                        {
                            return false;
                        }
                    }
                }
            }
            return (recipe.ManaIngredient == 0 || actor.Mana >= recipe.ManaIngredient) && !flag2;
        }

        private static string GetLabel(RecipeSpec recipe, Item oneIngredient, List<Item> ingredients)
        {
            InventoryExtraPossibleInteractions.tmpCommaListObjects.Clear();
            InventoryExtraPossibleInteractions.tmpHandledSpecs.Clear();
            int num = ingredients.IndexOf(oneIngredient);
            for (int i = 0; i < ingredients.Count; i++)
            {
                Item item = ingredients[i];
                if (i != num && !InventoryExtraPossibleInteractions.tmpHandledSpecs.Contains(item.Spec))
                {
                    int num2 = 0;
                    for (int j = 0; j < ingredients.Count; j++)
                    {
                        Item item2 = ingredients[j];
                        if (item.Spec == item2.Spec && j != num)
                        {
                            num2++;
                        }
                    }
                    if (num2 == 1)
                    {
                        if (item.StackCount == 1)
                        {
                            InventoryExtraPossibleInteractions.tmpCommaListObjects.Add(item);
                        }
                        else
                        {
                            InventoryExtraPossibleInteractions.tmpCommaListObjects.Add(item.Spec);
                        }
                    }
                    else
                    {
                        InventoryExtraPossibleInteractions.tmpCommaListObjects.Add("{0} x{1}".Formatted(item.Spec, num2.ToStringCached()));
                    }
                    InventoryExtraPossibleInteractions.tmpHandledSpecs.Add(item.Spec);
                }
            }
            if (recipe.ManaIngredient != 0)
            {
                InventoryExtraPossibleInteractions.tmpCommaListObjects.Add("Mana".Translate(recipe.ManaIngredient));
            }
            return "MixWith".Translate(StringUtility.ToCommaListAnd(InventoryExtraPossibleInteractions.tmpCommaListObjects));
        }

        private static List<Item> tmpFoundIngredients = new List<Item>();

        private static List<Item> tmpUnusedIngredients = new List<Item>();

        private static HashSet<EntitySpec> tmpNeededIngredientSpecs = new HashSet<EntitySpec>();

        private static List<object> tmpCommaListObjects = new List<object>();

        private static HashSet<EntitySpec> tmpHandledSpecs = new HashSet<EntitySpec>();
    }
}