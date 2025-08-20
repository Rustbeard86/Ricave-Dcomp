using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Character : Window
    {
        public Window_Character(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Widgets.Tabs(inRect.TopPart(32f).ContractedBy(1f, 0f), (Get.NowControlledActor == Get.MainActor) ? this.tabs : this.tabsNonMain, ref this.currentTab, true, (Get.NowControlledActor == Get.MainActor) ? this.tabTips : this.tabTipsNonMain, Window_Character.TabIcons);
            Rect rect = inRect.CutFromTop(32f);
            if (this.currentTab == 0)
            {
                this.DoEquipment(rect);
                return;
            }
            if (this.currentTab == 1)
            {
                this.DoCrafting(rect);
                return;
            }
            if (this.currentTab == 2)
            {
                this.DoIdentificationQueue(rect);
            }
        }

        private void DoEquipment(Rect inRect)
        {
            this.DoConditionIcons(inRect.xMax - 20f, inRect.y + 9f);
            float num = inRect.y + 49f;
            Rect rect = new Rect(inRect.x + 20f, num - 7f, 50f, 50f).ExpandedBy(10f);
            Rect rect2 = rect.ContractedBy(9f);
            GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            if (Get.NowControlledActor == Get.MainActor)
            {
                GUI.DrawTexture(rect, Get.PlayerModel.Icon);
            }
            else
            {
                GUI.color = Get.NowControlledActor.IconColor;
                GUI.DrawTexture(rect, Get.NowControlledActor.Icon);
                GUI.color = Color.white;
            }
            Get.Tooltips.RegisterTip(rect2, Get.NowControlledActor, null, null);
            string text;
            if (Get.NowControlledActor == Get.MainActor)
            {
                if (Get.Player.Class == null)
                {
                    text = RichText.Bold(Get.Progress.PlayerName);
                }
                else
                {
                    text = RichText.Bold(RichText.CreateColorTag(Get.Player.Class.LabelCap, new Color(1f, 1f, 0.5f)).AppendedWithSpace(Get.Progress.PlayerName));
                }
            }
            else
            {
                text = RichText.Label(Get.NowControlledActor);
            }
            Rect rect3 = Widgets.TableRowL(RichText.Grayed("Name".Translate()), text, inRect.x + 75f, ref num, this.characterTableLabels);
            if (Get.NowControlledActor == Get.MainActor && Get.Player.Class != null)
            {
                Get.Tooltips.RegisterTip(rect3, Get.Player.Class, null, null);
            }
            else
            {
                Get.Tooltips.RegisterTip(rect3, Get.NowControlledActor, null, null);
            }
            if (Get.NowControlledActor == Get.MainActor)
            {
                string text2 = Get.Player.Level.ToStringCached().AppendedWithSpace(RichText.Grayed("({0} {1} / {2})".Formatted("ExperienceShort".Translate(), Get.Player.ExperienceSinceLeveling.ToStringCached(), Get.Player.ExperienceBetweenLevels.ToStringCached())));
                Rect rect4 = Widgets.TableRowL(RichText.Grayed("Level".Translate()), RichText.Bold(text2), inRect.x + 75f, ref num, this.characterTableLabels);
                if (Mouse.Over(rect4))
                {
                    Get.Tooltips.RegisterTip(rect4, PlayerActorStatusControls.PlayerLevelTip, null);
                }
            }
            IUsable usable;
            Rect rect5 = Widgets.TableRowL(RichText.Grayed("AttackDamage".Translate()), RichText.Bold(this.GetAttackDamageText(out usable)), inRect.x + 75f, ref num, this.characterTableLabels);
            if (Mouse.Over(rect5) && usable != null)
            {
                Get.Tooltips.RegisterTip(rect5, usable, null, null);
            }
            foreach (ItemSlotSpec itemSlotSpec in Get.Specs.GetAll<ItemSlotSpec>())
            {
                ItemSlotDrawer.DoItemSlot(RectUtility.CenteredAt(inRect.center.WithAddedY(15f) + new Vector2((float)itemSlotSpec.Offset.x * (ItemSlotDrawer.SlotSize.x + 5f), (float)itemSlotSpec.Offset.y * (ItemSlotDrawer.SlotSize.y + 5f)), ItemSlotDrawer.SlotSize), Get.NowControlledActor.Inventory.Equipped.GetEquippedItemWithSlot(itemSlotSpec), this.GetCachedItemMoveActionGetter(itemSlotSpec), true, itemSlotSpec, true);
            }
            if (Get.NowControlledActor == Get.MainActor)
            {
                this.DoSkillIcons(inRect.xMax - 20f, inRect.yMax - 60f);
                this.DoTraitIcons(inRect.x + 20f, num - 2f);
            }
        }

        public Func<Item, Action> GetCachedItemMoveActionGetter(ItemSlotSpec slot)
        {
            Func<Item, Action> func;
            if (!this.cachedItemMoveActionGetters.TryGetValue(slot, out func))
            {
                func = this.CreateItemMoveActionGetter(slot);
                this.cachedItemMoveActionGetters.Add(slot, func);
            }
            return func;
        }

        private Func<Item, Action> CreateItemMoveActionGetter(ItemSlotSpec slot)
        {
            return delegate (Item item)
            {
                if (item.Spec.Item.ItemSlot != slot)
                {
                    return null;
                }
                return new Action_Equip(Get.Action_Equip, Get.NowControlledActor, item);
            };
        }

        private void DoCrafting(Rect inRect)
        {
            if (this.recipeSpecsInUIOrder == null)
            {
                this.recipeSpecsInUIOrder = new List<RecipeSpec>();
                this.recipeSpecsInUIOrder.AddRange(Get.Specs.GetAll<RecipeSpec>());
                this.recipeSpecsInUIOrder.Sort((RecipeSpec a, RecipeSpec b) => a.UIOrder.CompareTo(b.UIOrder));
            }
            float num = 0f;
            Rect rect = Widgets.BeginScrollView(inRect, null);
            num = rect.y + 20f;
            this.< DoCrafting > g__DoCategory | 17_1(RecipeSpec.Category.Utilities, "RecipeCategory_Utilities".Translate(), rect, ref num);
            this.< DoCrafting > g__DoCategory | 17_1(RecipeSpec.Category.Potions, "RecipeCategory_Potions".Translate(), rect, ref num);
            this.< DoCrafting > g__DoCategory | 17_1(RecipeSpec.Category.Scrolls, "RecipeCategory_Scrolls".Translate(), rect, ref num);
            this.< DoCrafting > g__DoCategory | 17_1(RecipeSpec.Category.WeaponsAndGear, "RecipeCategory_WeaponsAndGear".Translate(), rect, ref num);
            Widgets.EndScrollView(inRect, num + 20f, false, false);
        }

        private void DoItemSpec(Rect rect, EntitySpec itemSpec, float darken = 1f, bool tipAndHighlight = true)
        {
            if (tipAndHighlight)
            {
                GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
                Get.Tooltips.RegisterTip(rect, itemSpec, null, null);
            }
            GUI.color = itemSpec.IconColorAdjustedIfIdentified.MultipliedColor(darken);
            GUI.DrawTexture(rect, itemSpec.IconAdjustedIfIdentified);
            GUI.color = Color.white;
        }

        private void DoCustomIngredient(Rect rect, Texture2D icon, Color iconColor, string tip, float darken = 1f)
        {
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            Get.Tooltips.RegisterTip(rect, tip, null);
            GUI.color = iconColor.MultipliedColor(darken);
            GUI.DrawTexture(rect, icon);
            GUI.color = Color.white;
        }

        private void DoCustomIngredient(Rect rect, string text, Color color, string tip, float darken = 1f)
        {
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            Get.Tooltips.RegisterTip(rect, tip, null);
            GUI.color = color.MultipliedColor(darken);
            Widgets.FontSize = Widgets.GetFontSizeToFitInHeight(rect.height);
            Widgets.LabelCentered(rect, text, true, null, null);
            Widgets.ResetFontSize();
            GUI.color = Color.white;
        }

        private void DoIdentificationQueue(Rect inRect)
        {
            GUI.color = Color.gray;
            Widgets.Align = TextAnchor.UpperCenter;
            Widgets.Label(inRect.ContractedBy(20f), "IdentificationQueueExplanation".Translate(), true, null, null, false);
            Widgets.ResetAlign();
            GUI.color = Color.white;
            this.DoUnidentifiedItemsList(inRect.CutFromTop(100f));
        }

        private void DoUnidentifiedItemsList(Rect inRect)
        {
            if (Get.NowControlledActor.Inventory.UnidentifiedItemsInIdentifyOrder.Count == 0)
            {
                GUI.color = Color.gray;
                Widgets.Align = TextAnchor.UpperCenter;
                Widgets.Label(new Rect(inRect.x + 15f, inRect.y + 25f, inRect.width - 30f, 50f), "NoUnidentifiedItems".Translate(), true, null, null, false);
                Widgets.ResetAlign();
                GUI.color = Color.white;
                return;
            }
            int num = 0;
            int num2 = 0;
            Rect rect = Widgets.BeginScrollView(inRect, null);
            this.tmpUnidentifiedItems.Clear();
            this.tmpUnidentifiedItems.AddRange(Get.NowControlledActor.Inventory.UnidentifiedItemsInIdentifyOrder);
            bool stopIdentification = Get.NowControlledActor.ConditionsAccumulated.StopIdentification;
            for (int i = 0; i < this.tmpUnidentifiedItems.Count; i++)
            {
                Item item = this.tmpUnidentifiedItems[i];
                Rect rect2 = new Rect(rect.x, (float)(num * 70), rect.width, 70f);
                int num3 = item.Spec.Item.TurnsToIdentify - item.TurnsLeftToIdentify;
                GUIExtra.DrawRoundedRectBump(rect2, new Color(0.188f, 0.188f, 0.188f), false, i == 0, i == 0, i == this.tmpUnidentifiedItems.Count - 1, i == this.tmpUnidentifiedItems.Count - 1, null);
                ProgressBarDrawer progressBarDrawer = Get.ProgressBarDrawer;
                Rect rect3 = rect2;
                int num4 = num3;
                int turnsToIdentify = item.Spec.Item.TurnsToIdentify;
                Color color = (stopIdentification ? new Color(0.5f, 0.15f, 0.15f) : new Color(0.16f, 0.27f, 0.376f));
                float num5 = 1f;
                float num6 = 1f;
                bool flag = false;
                bool flag2 = false;
                bool flag3 = i == 0;
                bool flag4 = i == 0;
                bool flag5 = i == this.tmpUnidentifiedItems.Count - 1;
                bool flag6 = i == this.tmpUnidentifiedItems.Count - 1;
                progressBarDrawer.Draw(rect3, num4, turnsToIdentify, color, num5, num6, flag, flag2, null, ProgressBarDrawer.ValueChangeDirection.Constant, null, flag3, flag4, flag5, flag6, true, false, null);
                if (!Get.DragAndDrop.Dragging && !Get.WindowManager.IsContextMenuOpen)
                {
                    GUIExtra.DrawHighlightIfMouseover(rect2, true, i == 0, i == 0, i == this.tmpUnidentifiedItems.Count - 1, i == this.tmpUnidentifiedItems.Count - 1);
                }
                this.HandleDragAndDrop(rect2, item, num);
                Rect rect4 = rect2.ContractedBy(20f);
                Rect rect5 = new Rect(rect4.x, rect4.y, rect4.height, rect4.height);
                GUI.color = item.IconColor;
                GUI.DrawTexture(rect5, item.Icon);
                GUI.color = Color.white;
                Rect rect6 = new Rect(rect5.xMax + 5f, rect4.y, rect4.width - rect5.width - 5f, rect4.height);
                Widgets.Align = TextAnchor.MiddleLeft;
                Widgets.Label(rect6, RichText.Grayed("{0}: ".Formatted(num + 1)).Concatenated(RichText.Label(item)), true, null, null, false);
                Widgets.Align = TextAnchor.UpperRight;
                Widgets.Label(rect6.MovedBy(0f, 23f), RichText.Turns("{0} / {1}".Formatted(num3, item.Spec.Item.TurnsToIdentify)), true, null, null, false);
                Widgets.ResetAlign();
                num2 += item.TurnsLeftToIdentify;
                if (Mouse.Over(rect2))
                {
                    string text = "ItemIdentificationTip".Translate(RichText.Turns(StringUtility.TurnsString(num2)));
                    if (stopIdentification)
                    {
                        text = text.AppendedInDoubleNewLine(RichText.Error("IdentificationStoppedTip".Translate()));
                    }
                    Get.Tooltips.RegisterTip(rect2, item, text, null);
                }
                if (Get.DragAndDrop.IsDragging(item))
                {
                    GUIExtra.DrawRoundedRect(rect2, new Color(0f, 0f, 0f, 0.3f), i == 0, i == 0, i == this.tmpUnidentifiedItems.Count - 1, i == this.tmpUnidentifiedItems.Count - 1, null);
                }
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.Over(rect2))
                {
                    List<ValueTuple<string, Action, string>> possibleInteractions = Get.NowControlledActor.Inventory.GetPossibleInteractions(item, out flag6, false);
                    Get.WindowManager.OpenContextMenu(possibleInteractions, item.LabelCap);
                    Event.current.Use();
                }
                ItemSlotDrawer.HandleInventoryManagementSpecialClicks(rect2, item);
                num++;
            }
            Widgets.EndScrollView(inRect, (float)(num * 70) + 1f, false, false);
        }

        private void HandleDragAndDrop(Rect rowRect, Item item, int index)
        {
            ValueTuple<Item, int>? valueTuple = Reorderable.RegisterVerticalReorderable<Item>(item, rowRect, this.tmpUnidentifiedItems, DragAndDrop.DragSource.IdentificationQueue, new Vector2?(new Vector2(rowRect.height, rowRect.height)));
            if (valueTuple != null)
            {
                Get.TurnManager.TryDoAllNextForcedOrNonPlayerActionsNow();
                if (Get.NowControlledActor.Inventory.Contains(valueTuple.Value.Item1) && Get.TurnManager.IsPlayerTurn_CanChooseNextAction)
                {
                    this.Reorder(valueTuple.Value.Item1, valueTuple.Value.Item2);
                }
            }
        }

        private void Reorder(Item toReorder, int to)
        {
            List<Item> unidentifiedItemsInIdentifyOrder = Get.NowControlledActor.Inventory.UnidentifiedItemsInIdentifyOrder;
            int num = unidentifiedItemsInIdentifyOrder.IndexOf(toReorder);
            if (num == to)
            {
                return;
            }
            int identifyOrder = unidentifiedItemsInIdentifyOrder[to].IdentifyOrder;
            for (int i = 0; i < to; i++)
            {
                Item item = unidentifiedItemsInIdentifyOrder[i];
                int num2 = item.IdentifyOrder;
                item.IdentifyOrder = num2 - 1;
            }
            for (int j = to + 1; j < unidentifiedItemsInIdentifyOrder.Count; j++)
            {
                Item item2 = unidentifiedItemsInIdentifyOrder[j];
                int num2 = item2.IdentifyOrder;
                item2.IdentifyOrder = num2 + 1;
            }
            if (num < to)
            {
                Item item3 = unidentifiedItemsInIdentifyOrder[to];
                int num2 = item3.IdentifyOrder;
                item3.IdentifyOrder = num2 - 1;
            }
            else
            {
                Item item4 = unidentifiedItemsInIdentifyOrder[to];
                int num2 = item4.IdentifyOrder;
                item4.IdentifyOrder = num2 + 1;
            }
            toReorder.IdentifyOrder = identifyOrder;
        }

        private void DoConditionIcons(float startX, float y)
        {
            float num = startX;
            foreach (ConditionDrawRequest conditionDrawRequest in Get.NowControlledActor.ConditionsAccumulated.AllConditionDrawRequestsPlusExtra)
            {
                Rect rect = new Rect(num - 30f, y, 30f, 30f);
                ExpandingIconAnimation.Do(rect, conditionDrawRequest.Icon, conditionDrawRequest.IconColor, conditionDrawRequest.TimeStartedAffectingActor, 1f, 0.6f, 0.55f);
                if (Mouse.Over(rect))
                {
                    Get.Tooltips.RegisterTip(rect, conditionDrawRequest, null, null);
                }
                GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
                GUI.color = conditionDrawRequest.IconColor;
                GUI.DrawTexture(rect, conditionDrawRequest.Icon);
                GUI.color = Color.white;
                if (conditionDrawRequest.TurnsLeft > 0)
                {
                    Widgets.Align = TextAnchor.MiddleCenter;
                    Widgets.FontBold = true;
                    Widgets.Label(rect, Math.Min(conditionDrawRequest.TurnsLeft, 99).ToStringCached(), true, null, null, true);
                    Widgets.FontBold = false;
                    Widgets.ResetAlign();
                }
                num -= 34f;
                if (num - 30f < 20f)
                {
                    break;
                }
            }
        }

        private void DoSkillIcons(float startX, float y)
        {
            float num = startX;
            int num2 = 39;
            if (Get.SkillManager.UnlockedSkills.Count >= 9)
            {
                num2 = 27;
                y += 12f;
            }
            foreach (SkillSpec skillSpec in Get.SkillManager.UnlockedSkills)
            {
                Rect rect = new Rect(num - (float)num2, y, (float)num2, (float)num2);
                Window_Skills.DoSkill(rect, skillSpec);
                if (Mouse.OverCircle(rect.center, rect.width / 2f) && Widgets.ButtonInvisible(rect, false, false))
                {
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    Get.UI.SetCurrentTabAndOpenWindows(Get.MainTab_Skills);
                }
                num -= (float)(num2 + 6);
                if (num - (float)num2 < 20f)
                {
                    break;
                }
            }
            if (!Get.WindowManager.IsOpen(Get.Window_Skills))
            {
                foreach (SkillSpec skillSpec2 in Get.SkillManager.UnlockedSkills)
                {
                    ExpandingIconAnimation.Do(new Rect(startX - (float)num2, y, (float)num2, (float)num2), skillSpec2.Icon, skillSpec2.IconColor, Get.SkillManager.GetTimeUnlocked(skillSpec2), 1f, 0.6f, 0.55f);
                }
            }
        }

        private void DoTraitIcons(float x, float y)
        {
            float num = x;
            foreach (TraitSpec traitSpec in Get.TraitManager.Chosen)
            {
                Window_Traits.DoTrait(new Rect(num, y, 39f, 39f), traitSpec, true, false);
                num += 45f;
            }
        }

        public void OpenCraftingTab()
        {
            if (this.currentTab == 1)
            {
                return;
            }
            this.currentTab = 1;
            Get.Sound_Click.PlayOneShot(null, 1f, 1f);
        }

        private string GetAttackDamageText(out IUsable mainWeapon)
        {
            mainWeapon = null;
            UseEffect_Damage useEffect_Damage = null;
            Item equippedWeapon = Get.NowControlledActor.Inventory.EquippedWeapon;
            if (equippedWeapon != null)
            {
                if (!equippedWeapon.Identified)
                {
                    mainWeapon = equippedWeapon;
                    return "?";
                }
                mainWeapon = equippedWeapon;
                useEffect_Damage = (UseEffect_Damage)equippedWeapon.UseEffects.GetFirstOfSpec(Get.UseEffect_Damage);
            }
            else if (Get.NowControlledActor.NativeWeapons.Count != 0)
            {
                mainWeapon = Get.NowControlledActor.NativeWeapons[0];
                useEffect_Damage = (UseEffect_Damage)mainWeapon.UseEffects.GetFirstOfSpec(Get.UseEffect_Damage);
            }
            if (mainWeapon == null || useEffect_Damage == null)
            {
                return "-";
            }
            string text;
            int num;
            int num2;
            useEffect_Damage.GetFinalDamageAmount(Get.NowControlledActor, out text, out num, out num2, false, false, false, false, false, 1f);
            return StringUtility.RangeToString(num, num2).AppendedWithSpace(RichText.Grayed("({0})".Formatted(TipSubjectDrawer.GetMissAndCritChanceText(mainWeapon.MissChance, mainWeapon.CritChance, Get.NowControlledActor))));
        }

        [CompilerGenerated]
        private void <DoCrafting>g__DoCategory|17_1(RecipeSpec.Category category, string categoryLabel, Rect viewRect, ref float curY)
		{
			Widgets.Section(categoryLabel, viewRect.x + 20f, viewRect.width - 40f, ref curY, true, 12f);
			int num = Mathf.FloorToInt((viewRect.width + 10f) / 92f);
			if (num< 1)
			{
				num = 1;
			}
			int num2 = 0;
			for (int i = 0; i< this.recipeSpecsInUIOrder.Count; i++)

            {
        if (this.recipeSpecsInUIOrder[i].RecipeCategory == category)
        {
            num2++;
        }
    }
			float num3 = (float)Mathf.CeilToInt((float)num2 / (float)num) * 92f;
			int num4 = 0;
			for (int j = 0; j < this.recipeSpecsInUIOrder.Count; j++)

            {
        RecipeSpec spec = this.recipeSpecsInUIOrder[j];
        if (spec.RecipeCategory == category)
        {
            int num5 = num4 / num;
            int num6 = num4 % num;
            num4++;
            Rect rect = new Rect(viewRect.x + (float)num6 * 92f + 20f, curY + (float)num5 * 92f, 82f, 82f);
            if (Widgets.VisibleInScrollView(rect))
            {
                Window_Character.tmpPresentIngredients.Clear();
                bool flag = InventoryExtraPossibleInteractions.TryFindIngredients(Get.NowControlledActor, spec, null, Window_Character.tmpPresentIngredients, false, true);
                float num7 = (flag ? 1f : 0.61f);
                Rect rect2 = rect.TopPartPct(0.8f).ContractedToSquare();
                if (Widgets.Button(rect, "", true, new Color?(flag ? new Color(0.25f, 0.25f, 0.25f) : new Color(0.17f, 0.14f, 0.14f)), null, true, true, true, true, null, true, true, null, false, null, null))
                {
                    if (flag)
                    {
                        ActionViaInterfaceHelper.TryDo(delegate
                        {
                            List<Item> list = new List<Item>();
                            if (InventoryExtraPossibleInteractions.TryFindIngredients(Get.NowControlledActor, spec, null, list, true, true))
                            {
                                return new Action_UseRecipe(Get.Action_UseRecipe, Get.NowControlledActor, spec, list);
                            }
                            return null;
                        });
                    }
                    else
                    {
                        Get.PlayLog.Add("MissingIngredients".Translate());
                    }
                }
                this.DoItemSpec(rect2.ContractedBy(1f), spec.Product, num7, false);
                Get.Tooltips.RegisterTip(rect, spec.Product, null, null);
                if (Mouse.Over(rect))
                {
                    List<EntitySpec> ingredients = spec.Ingredients;
                    int num8 = ingredients.Count + ((spec.ManaIngredient != 0) ? 1 : 0);
                    float num9 = Mathf.Min(rect2.width / (float)num8, rect2.height * 0.5f);
                    float num10 = num9 * (float)num8;
                    float num11 = rect.center.x - num10 * 0.5f;
                    for (int k = 0; k < ingredients.Count; k++)
                    {
                        bool flag2 = false;
                        for (int l = 0; l < Window_Character.tmpPresentIngredients.Count; l++)
                        {
                            if (Window_Character.tmpPresentIngredients[l].Spec == ingredients[k])
                            {
                                Window_Character.tmpPresentIngredients.RemoveAt(l);
                                flag2 = true;
                                break;
                            }
                        }
                        Rect rect3 = new Rect(num11 + (float)k * num9, rect.yMax - num9, num9, num9);
                        GUIExtra.DrawRoundedRect(rect3, flag2 ? new Color(0.12f, 0.12f, 0.12f, 0.5f) : new Color(0.05f, 0.05f, 0.05f, 0.5f), true, true, true, true, null);
                        this.DoItemSpec(rect3, ingredients[k], flag2 ? 1f : num7, true);
                    }
                    if (spec.ManaIngredient != 0)
                    {
                        Rect rect4 = new Rect(num11 + (float)ingredients.Count * num9, rect.yMax - num9, num9, num9);
                        bool flag3 = Get.NowControlledActor.Mana >= spec.ManaIngredient;
                        GUIExtra.DrawRoundedRect(rect4, flag3 ? new Color(0.12f, 0.12f, 0.12f, 0.5f) : new Color(0.05f, 0.05f, 0.05f, 0.5f), true, true, true, true, null);
                        this.DoCustomIngredient(rect4, RichText.Mana(spec.ManaIngredient.ToStringCached()), Color.white, "Mana".Translate(spec.ManaIngredient), flag3 ? 1f : num7);
                    }
                }
                else
                {
                    GUI.color = new Color(num7, num7, num7);
                    Widgets.Align = TextAnchor.LowerCenter;
                    Widgets.Label(rect.ContractedBy(1f), spec.Product.LabelCap, true, null, null, true);
                    Widgets.ResetAlign();
                    GUI.color = Color.white;
                }
            }
        }
    }
    curY += num3 + 20f;
    }

    private List<Item> tmpUnidentifiedItems = new List<Item>();

    private int currentTab;

    private List<string> tabs = new List<string>
        {
            "EquipmentTab".Translate(),
            "CraftingTab".Translate(),
            "IdentificationTab".Translate()
        };

    private List<string> tabsNonMain = new List<string>
        {
            "EquipmentTab".Translate(),
            "CraftingTab".Translate()
        };

    private List<string> tabTips = new List<string>
        {
            "EquipmentTabTip".Translate(),
            "CraftingTabTip".Translate(),
            "IdentificationTabTip".Translate()
        };

    private List<string> tabTipsNonMain = new List<string>
        {
            "EquipmentTabTip".Translate(),
            "CraftingTabTip".Translate()
        };

    private List<string> characterTableLabels = new List<string>
        {
            "Name".Translate(),
            "Level".Translate(),
            "AttackDamage".Translate()
        };

    private Dictionary<ItemSlotSpec, Func<Item, Action>> cachedItemMoveActionGetters = new Dictionary<ItemSlotSpec, Func<Item, Action>>();

    private List<RecipeSpec> recipeSpecsInUIOrder;

    private const int RowHeight = 70;

    private static readonly List<Texture2D> TabIcons = new List<Texture2D>
        {
            Assets.Get<Texture2D>("Textures/UI/TabIcons/Equipment"),
            Assets.Get<Texture2D>("Textures/UI/TabIcons/Crafting"),
            Assets.Get<Texture2D>("Textures/UI/TabIcons/Identification")
        };

    private static List<Item> tmpPresentIngredients = new List<Item>();
}
}