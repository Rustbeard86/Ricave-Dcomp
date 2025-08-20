using System;
using System.Collections.Generic;
using System.Text;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Quickbar : Window
    {
        public Window_Quickbar(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Item[] quickbarItems = Get.NowControlledActor.Inventory.QuickbarItems;
            for (int i = 0; i < quickbarItems.Length; i++)
            {
                ItemSlotDrawer.DoItemSlot(new Rect(inRect.x + (float)i * (ItemSlotDrawer.SlotSize.x + 5f), inRect.y, ItemSlotDrawer.SlotSize.x, ItemSlotDrawer.SlotSize.y), quickbarItems[i], Window_Quickbar.GetCachedItemMoveActionGetter(i), true, null, true);
            }
        }

        protected override void DoExtraTitleGUI(Rect titleRect)
        {
            Rect rect = titleRect.RightPart(32f);
            rect.height = 32f;
            GUI.DrawTexture(rect, Window_Quickbar.AvailableRecipesIcon);
            if (Mouse.Over(rect))
            {
                GUIExtra.DrawHighlight(rect, true, true, true, true, 1f);
                if (this.availableRecipesTip == null || this.availableRecipesTipMostRecentAction != Get.TurnManager.MostRecentAction)
                {
                    this.availableRecipesTip = Window_Quickbar.CalculateAvailableRecipesTip();
                    this.availableRecipesTipMostRecentAction = Get.TurnManager.MostRecentAction;
                }
                Get.Tooltips.RegisterTip(rect, this.availableRecipesTip, null);
            }
            else
            {
                this.availableRecipesTip = null;
            }
            if (Widgets.ButtonInvisible(rect, false, false))
            {
                Window_Character window_Character = (Window_Character)Get.WindowManager.GetFirstWindow(Get.Window_Character);
                if (window_Character != null)
                {
                    window_Character.OpenCraftingTab();
                }
            }
        }

        public static Func<Item, Action> GetCachedItemMoveActionGetter(int index)
        {
            if (Window_Quickbar.CachedItemMoveActionGetters[index] == null)
            {
                Window_Quickbar.CachedItemMoveActionGetters[index] = Window_Quickbar.CreateItemMoveActionGetter(index);
            }
            return Window_Quickbar.CachedItemMoveActionGetters[index];
        }

        private static Func<Item, Action> CreateItemMoveActionGetter(int index)
        {
            return (Item item) => new Action_MoveItemInInventory(Get.Action_MoveItemInInventory, Get.NowControlledActor, item, null, new int?(index));
        }

        private static string CalculateAvailableRecipesTip()
        {
            Window_Quickbar.sb.Clear();
            Window_Quickbar.sb.Append("AvailableRecipes".Translate());
            Window_Quickbar.sb.Append(":");
            Window_Quickbar.sb.AppendLine();
            Window_Quickbar.tmpRecipes.Clear();
            List<Item> allItems = Get.NowControlledActor.Inventory.AllItems;
            for (int i = 0; i < allItems.Count; i++)
            {
                List<RecipeSpec> all = Get.Specs.GetAll<RecipeSpec>();
                for (int j = 0; j < all.Count; j++)
                {
                    if (!Window_Quickbar.tmpRecipes.Contains(all[j]) && all[j].Ingredients.Contains(allItems[i].Spec) && InventoryExtraPossibleInteractions.TryFindIngredients(Get.NowControlledActor, all[j], allItems[i], null, true, true))
                    {
                        Window_Quickbar.tmpRecipes.Add(all[j]);
                        Window_Quickbar.sb.AppendLine();
                        List<EntitySpec> ingredients = all[j].Ingredients;
                        for (int k = 0; k < ingredients.Count; k++)
                        {
                            if (k != 0)
                            {
                                Window_Quickbar.sb.Append(" + ");
                            }
                            Window_Quickbar.sb.Append(ingredients[k].LabelAdjustedCap);
                        }
                        if (all[j].ManaIngredient != 0)
                        {
                            if (ingredients.Count != 0)
                            {
                                Window_Quickbar.sb.Append(" + ");
                            }
                            Window_Quickbar.sb.Append("Mana".Translate(all[j].ManaIngredient));
                        }
                        Window_Quickbar.sb.Append(" = ");
                        Window_Quickbar.sb.Append(all[j].Product.LabelAdjustedCap);
                    }
                }
            }
            if (Window_Quickbar.tmpRecipes.Count == 0)
            {
                Window_Quickbar.sb.AppendLine();
                Window_Quickbar.sb.Append("None".Translate());
            }
            return Window_Quickbar.sb.ToString();
        }

        private string availableRecipesTip;

        private Action availableRecipesTipMostRecentAction;

        private static readonly Texture2D AvailableRecipesIcon = Assets.Get<Texture2D>("Textures/UI/AvailableRecipes");

        private static readonly Func<Item, Action>[] CachedItemMoveActionGetters = new Func<Item, Action>[10];

        private static StringBuilder sb = new StringBuilder();

        private static HashSet<RecipeSpec> tmpRecipes = new HashSet<RecipeSpec>();
    }
}