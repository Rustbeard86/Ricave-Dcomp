using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class FPPQuickbarControls
    {
        public static bool ShouldDoSpellsRow
        {
            get
            {
                return Get.NowControlledActor.Spells.Any || SpellChooserUtility.ChoicesToShow.Count != 0;
            }
        }

        private static float YAnimation
        {
            get
            {
                if (!Get.UI.InventoryOpen)
                {
                    return 0f;
                }
                return Math.Max((1f - (Clock.UnscaledTime - Get.UI.LastTimeOpenedAutoOpenInUIModeWindows) / 0.025f) * 8f, 0f);
            }
        }

        public static void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }
            if (Event.current.type == EventType.MouseMove)
            {
                return;
            }
            if (Get.NowControlledActor == null)
            {
                return;
            }
            if (Get.InLobby)
            {
                return;
            }
            bool draggingOrDroppedJustNow = Get.DragAndDrop.DraggingOrDroppedJustNow;
            bool shouldDoSpellsRow = FPPQuickbarControls.ShouldDoSpellsRow;
            if (shouldDoSpellsRow)
            {
                FPPQuickbarControls.DoSpells();
            }
            else
            {
                FPPQuickbarControls.lastTimeShowChooseSpellHotkey = -99999f;
            }
            bool flag;
            if (!Get.UI.InventoryOpen)
            {
                FPPQuickbarControls.DoItemSlots();
                flag = true;
            }
            else
            {
                flag = false;
            }
            if (!Get.UI.IsEscapeMenuOpen)
            {
                FPPQuickbarControls.HandleQuickbarKeyEvents(flag, draggingOrDroppedJustNow);
                if (shouldDoSpellsRow)
                {
                    FPPQuickbarControls.HandleSpellsKeyEvents(true, draggingOrDroppedJustNow);
                }
                FPPQuickbarControls.HandleCyclingWithWheelSelectorOpen();
                if (Get.KeyBinding_QuickbarMenu.JustPressed)
                {
                    if (Event.current.type == EventType.KeyDown)
                    {
                        Event.current.Use();
                    }
                    if (Get.UI.WheelSelector != null && Get.UI.WheelSelector.ForSubject == null)
                    {
                        Get.UI.CloseWheelSelector(true);
                        return;
                    }
                    FPPQuickbarControls.OpenQuickbarMenu();
                }
            }
        }

        private static void DoItemSlots()
        {
            Item[] quickbarItems = Get.NowControlledActor.Inventory.QuickbarItems;
            int quickbarCenteringOffset = FPPQuickbarControls.GetQuickbarCenteringOffset();
            int totalWidth = FPPQuickbarControls.GetTotalWidth(quickbarItems.Length);
            Rect quickbarItemRect = FPPQuickbarControls.GetQuickbarItemRect(quickbarCenteringOffset, 0);
            quickbarItemRect.width = (float)totalWidth;
            if (CachedGUI.BeginCachedGUI(quickbarItemRect.ExpandedBy(1f).ExpandedBy(25f, 0f, 0f, 0f), 1, true))
            {
                for (int i = 0; i < quickbarItems.Length; i++)
                {
                    Rect quickbarItemRect2 = FPPQuickbarControls.GetQuickbarItemRect(quickbarCenteringOffset, i);
                    if (!ControllerUtility.InControllerMode)
                    {
                        int num = ((i == 9) ? 0 : (i + 1));
                        GUI.color = new Color(1f, 1f, 1f, 0.55f);
                        Widgets.FontBold = true;
                        Widgets.LabelCentered(quickbarItemRect2.TopCenter().MovedBy(0f, -6f), num.ToStringCached(), true, null, null, false, true, false, null);
                        Widgets.FontBold = false;
                        GUI.color = Color.white;
                    }
                    GUIExtra.DrawRoundedRectBump(quickbarItemRect2, FPPQuickbarControls.BackgroundColor, false, true, true, true, true, null);
                }
            }
            CachedGUI.EndCachedGUI(1f, 1f);
            for (int j = 0; j < quickbarItems.Length; j++)
            {
                Rect quickbarItemRect3 = FPPQuickbarControls.GetQuickbarItemRect(quickbarCenteringOffset, j);
                ItemSlotDrawer.DoItemSlot(quickbarItemRect3, quickbarItems[j], Window_Quickbar.GetCachedItemMoveActionGetter(j), false, null, false);
                if (quickbarItems[j] != null && Get.WheelSelector != null && Get.WheelSelector.ForSubject == quickbarItems[j])
                {
                    FPPQuickbarControls.DrawArrowAbove(quickbarItemRect3);
                }
            }
            for (int k = 0; k < quickbarItems.Length; k++)
            {
                if (quickbarItems[k] != null)
                {
                    ExpandingIconAnimation.Do(FPPQuickbarControls.GetQuickbarItemRect(quickbarCenteringOffset, k), quickbarItems[k].Icon, quickbarItems[k].IconColor, quickbarItems[k].TimeAddedToInventory, 0.9f, 0.6f, 0.55f);
                }
            }
        }

        private static void DoSpells()
        {
            List<Spell> all = Get.NowControlledActor.Spells.All;
            int spellsCenteringOffset = FPPQuickbarControls.GetSpellsCenteringOffset();
            for (int i = 0; i < all.Count; i++)
            {
                Rect spellRect = FPPQuickbarControls.GetSpellRect(spellsCenteringOffset, i);
                GUIExtra.DrawRoundedRectBump(spellRect, FPPQuickbarControls.BackgroundColor, false, true, true, true, true, null);
                SpellSlotDrawer.DoSpellSlot(spellRect, all[i], FPPQuickbarControls.GetCachedSpellMoveActionGetter(i), false);
                if (Get.WheelSelector != null && Get.WheelSelector.ForSubject == all[i])
                {
                    FPPQuickbarControls.DrawArrowAbove(spellRect);
                }
                if (!ControllerUtility.InControllerMode)
                {
                    KeyCode spellKeyCode = FPPQuickbarControls.GetSpellKeyCode(i);
                    FPPQuickbarControls.DrawHotkeyLabel(spellRect, spellKeyCode);
                }
            }
            if (SpellChooserUtility.ChoicesToShow.Count != 0)
            {
                Rect spellRect2 = FPPQuickbarControls.GetSpellRect(spellsCenteringOffset, all.Count);
                GUIExtra.DrawRoundedRectBump(spellRect2, FPPQuickbarControls.BackgroundColor, false, true, true, true, true, null);
                SpellChooserUtility.DoChooseSpellButton(spellRect2);
                if (!ControllerUtility.InControllerMode)
                {
                    KeyCode spellKeyCode2 = FPPQuickbarControls.GetSpellKeyCode(all.Count);
                    FPPQuickbarControls.DrawHotkeyLabel(spellRect2, spellKeyCode2);
                }
                if (all.Count == 0)
                {
                    if (FPPQuickbarControls.lastTimeShowChooseSpellHotkey == -99999f)
                    {
                        FPPQuickbarControls.lastTimeShowChooseSpellHotkey = Clock.UnscaledTime;
                    }
                    GUI.color = new Color(0.7f, 0.7f, 0.7f);
                    string text = (ControllerUtility.InControllerMode ? "ChooseYourSpellController".Translate().FormattedKeyBindings() : "ChooseYourSpell".Translate());
                    float num = 1f - Calc.ResolveFadeIn(Clock.UnscaledTime - FPPQuickbarControls.lastTimeShowChooseSpellHotkey, 1.1f);
                    int num2 = ((Get.RunSpec == Get.Run_Main1 && Get.Progress.GetRunStats(Get.Run_Main1).Runs <= 0) ? Calc.RoundToInt(Calc.PulseUnscaled(2f, 9f) - 4f) : 0);
                    Widgets.FontSizeScalable = Calc.RoundToInt(15f + num * 15f + (float)num2);
                    Widgets.LabelCentered(spellRect2.center.MovedBy(0f, 19f - num * 180f), text, true, null, null, false, true, false, null);
                    Widgets.ResetFontSize();
                    GUI.color = Color.white;
                }
                else
                {
                    FPPQuickbarControls.lastTimeShowChooseSpellHotkey = -99999f;
                }
            }
            else
            {
                FPPQuickbarControls.lastTimeShowChooseSpellHotkey = -99999f;
            }
            for (int j = 0; j < all.Count; j++)
            {
                if (all[j] != null)
                {
                    ExpandingIconAnimation.Do(FPPQuickbarControls.GetSpellRect(spellsCenteringOffset, j), all[j].Icon, all[j].IconColor, all[j].TimeGained, 0.9f, 0.6f, 0.55f);
                }
            }
        }

        private static Rect GetQuickbarItemRect(int centeringOffset, int index)
        {
            return new Rect((float)(centeringOffset + index * 74), Widgets.VirtualHeight - 10f - 70f + FPPQuickbarControls.YAnimation, 70f, 70f);
        }

        private static Rect GetSpellRect(int centeringOffset, int index)
        {
            bool flag = !Get.UI.InventoryOpen;
            return new Rect((float)(centeringOffset + index * 74), Widgets.VirtualHeight - 10f - 70f + FPPQuickbarControls.YAnimation - (float)(flag ? 80 : 20), 70f, 70f);
        }

        private static void HandleQuickbarKeyEvents(bool drawn, bool draggingOrDroppedJustNow)
        {
            Item[] quickbarItems = Get.NowControlledActor.Inventory.QuickbarItems;
            int quickbarCenteringOffset = FPPQuickbarControls.GetQuickbarCenteringOffset();
            for (int i = 0; i < quickbarItems.Length; i++)
            {
                if (quickbarItems[i] != null)
                {
                    Rect quickbarItemRect = FPPQuickbarControls.GetQuickbarItemRect(quickbarCenteringOffset, i);
                    int num = ((i == 9) ? 0 : (i + 1));
                    if ((Event.current.type == EventType.KeyDown && Event.current.keyCode == num.DigitToKeyCode()) || (drawn && Widgets.ButtonInvisible(quickbarItemRect, false, false) && Event.current.button == 0 && !draggingOrDroppedJustNow))
                    {
                        FPPQuickbarControls.OnQuickbarItemClicked(i);
                    }
                }
            }
        }

        private static void OnQuickbarItemClicked(int index)
        {
            Item[] quickbarItems = Get.NowControlledActor.Inventory.QuickbarItems;
            Get.TurnManager.TryDoAllNextForcedOrNonPlayerActionsNow();
            if (!Get.TurnManager.IsPlayerTurn_CanChooseNextAction || quickbarItems[index] == null)
            {
                return;
            }
            bool flag;
            List<ValueTuple<string, Action, string>> possibleInteractions = Get.NowControlledActor.Inventory.GetPossibleInteractions(quickbarItems[index], out flag, true);
            if (possibleInteractions.Count != 0)
            {
                if (possibleInteractions.Count == 1 && flag && possibleInteractions[0].Item2 != null)
                {
                    possibleInteractions[0].Item2();
                    if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
                    {
                        Event.current.Use();
                        return;
                    }
                }
                else if (Get.UI.WheelSelector == null || Get.UI.WheelSelector.ForSubject != quickbarItems[index])
                {
                    Get.UI.OpenWheelSelector(possibleInteractions, quickbarItems[index], null, false, true);
                    if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
                    {
                        Event.current.Use();
                    }
                }
            }
        }

        private static void HandleSpellsKeyEvents(bool drawn, bool draggingOrDroppedJustNow)
        {
            List<Spell> all = Get.NowControlledActor.Spells.All;
            int spellsCenteringOffset = FPPQuickbarControls.GetSpellsCenteringOffset();
            for (int i = 0; i < all.Count; i++)
            {
                Rect spellRect = FPPQuickbarControls.GetSpellRect(spellsCenteringOffset, i);
                KeyCode spellKeyCode = FPPQuickbarControls.GetSpellKeyCode(i);
                if ((Event.current.type == EventType.KeyDown && Event.current.keyCode == spellKeyCode) || (drawn && Widgets.ButtonInvisible(spellRect, false, false) && Event.current.button == 0 && !draggingOrDroppedJustNow))
                {
                    FPPQuickbarControls.OnSpellClicked(i);
                }
            }
            KeyCode spellKeyCode2 = FPPQuickbarControls.GetSpellKeyCode(all.Count);
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == spellKeyCode2 && (Get.UI.WheelSelector == null || !Get.UI.WheelSelector.IsSpellChooser) && SpellChooserUtility.ChoicesToShow.Count != 0)
            {
                Event.current.Use();
                SpellChooserUtility.TryShowWheelSelectorWithChoices();
            }
        }

        private static void OnSpellClicked(int index)
        {
            List<Spell> all = Get.NowControlledActor.Spells.All;
            Get.TurnManager.TryDoAllNextForcedOrNonPlayerActionsNow();
            if (!Get.TurnManager.IsPlayerTurn_CanChooseNextAction || index >= all.Count)
            {
                return;
            }
            bool flag;
            List<ValueTuple<string, Action, string>> possibleInteractions = Get.NowControlledActor.Spells.GetPossibleInteractions(all[index], out flag, true);
            if (possibleInteractions.Count != 0)
            {
                if (possibleInteractions.Count == 1 && flag)
                {
                    possibleInteractions[0].Item2();
                    if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
                    {
                        Event.current.Use();
                        return;
                    }
                }
                else if (Get.UI.WheelSelector == null || Get.UI.WheelSelector.ForSubject != all[index])
                {
                    Get.UI.OpenWheelSelector(possibleInteractions, all[index], null, false, false);
                    if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
                    {
                        Event.current.Use();
                    }
                }
            }
        }

        private static int GetQuickbarCenteringOffset()
        {
            return FPPQuickbarControls.GetCenteringOffset(Get.NowControlledActor.Inventory.QuickbarItems.Length);
        }

        private static int GetSpellsCenteringOffset()
        {
            int num = Get.NowControlledActor.Spells.Count;
            if (SpellChooserUtility.ChoicesToShow.Count != 0)
            {
                num++;
            }
            return FPPQuickbarControls.GetCenteringOffset(num);
        }

        private static int GetCenteringOffset(int elementsCount)
        {
            return Calc.FloorToInt((Widgets.VirtualWidth - (float)FPPQuickbarControls.GetTotalWidth(elementsCount)) / 2f);
        }

        private static int GetTotalWidth(int elementsCount)
        {
            return elementsCount * 70 + (elementsCount - 1) * 4;
        }

        private static KeyCode GetSpellKeyCode(int index)
        {
            if (index == 0)
            {
                return KeyCode.R;
            }
            if (index == 1)
            {
                return KeyCode.T;
            }
            if (index == 2)
            {
                return KeyCode.Y;
            }
            if (index == 3)
            {
                return KeyCode.U;
            }
            if (index == 4)
            {
                return KeyCode.I;
            }
            if (index == 5)
            {
                return KeyCode.O;
            }
            if (index == 6)
            {
                return KeyCode.P;
            }
            return KeyCode.None;
        }

        public static bool IsQuickbarOrSpellHotkey(KeyCode keyCode)
        {
            if (keyCode == KeyCode.None)
            {
                return false;
            }
            if (keyCode.IsAlpha())
            {
                return true;
            }
            for (int i = 0; i < 7; i++)
            {
                if (keyCode == FPPQuickbarControls.GetSpellKeyCode(i))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsActivelyUsedSpellHotkey(KeyCode keyCode)
        {
            if (!FPPQuickbarControls.IsQuickbarOrSpellHotkey(keyCode))
            {
                return false;
            }
            if (!FPPQuickbarControls.ShouldDoSpellsRow)
            {
                return false;
            }
            List<Spell> all = Get.NowControlledActor.Spells.All;
            for (int i = 0; i < all.Count; i++)
            {
                if (FPPQuickbarControls.GetSpellKeyCode(i) == keyCode)
                {
                    return true;
                }
            }
            return FPPQuickbarControls.GetSpellKeyCode(all.Count) == keyCode && SpellChooserUtility.ChoicesToShow.Count != 0;
        }

        private static Func<Spell, Action> GetCachedSpellMoveActionGetter(int index)
        {
            if (FPPQuickbarControls.CachedSpellMoveActionGetters[index] == null)
            {
                FPPQuickbarControls.CachedSpellMoveActionGetters[index] = FPPQuickbarControls.CreateSpellMoveActionGetter(index);
            }
            return FPPQuickbarControls.CachedSpellMoveActionGetters[index];
        }

        private static Func<Spell, Action> CreateSpellMoveActionGetter(int index)
        {
            return (Spell spell) => new Action_ReorderSpell(Get.Action_ReorderSpell, Get.NowControlledActor, spell, index);
        }

        private static void DrawArrowAbove(Rect rect)
        {
            Rect rect2 = new Rect(rect.center.x - 20f, rect.y - 40f, 40f, 40f);
            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            GUI.DrawTexture(rect2, FPPQuickbarControls.ArrowTex);
            GUI.color = Color.white;
        }

        private static void DrawHotkeyLabel(Rect rect, KeyCode keyCode)
        {
            FPPQuickbarControls.DrawHotkeyLabel(rect, keyCode.ToStringCached());
        }

        private static void DrawHotkeyLabel(Rect rect, string hotkey)
        {
            Widgets.FontSizeScalable = 17;
            Widgets.Align = TextAnchor.UpperRight;
            Widgets.FontBold = true;
            Widgets.Label(rect.MovedBy(-6f, 2f), hotkey, true, null, null, true);
            Widgets.FontBold = false;
            Widgets.ResetAlign();
            Widgets.ResetFontSize();
        }

        private static void HandleCyclingWithWheelSelectorOpen()
        {
            if (Get.WheelSelector == null)
            {
                FPPQuickbarControls.scrollAccumulatedDelta = 0f;
                return;
            }
            bool flag = Get.KeyBinding_MoveLeft.JustPressed || Get.KeyBinding_MoveLeftAlt.JustPressed;
            bool flag2 = Get.KeyBinding_MoveRight.JustPressed || Get.KeyBinding_MoveRightAlt.JustPressed;
            if (!ControllerUtility.InControllerMode)
            {
                if (!flag && (Get.KeyBinding_RotateLeft.JustPressed || Get.KeyBinding_RotateLeftAlt.JustPressed))
                {
                    flag = true;
                }
                if (!flag2 && (Get.KeyBinding_RotateRight.JustPressed || Get.KeyBinding_RotateRightAlt.JustPressed))
                {
                    flag2 = true;
                }
            }
            if ((flag || flag2) && Event.current.type == EventType.KeyDown)
            {
                Event.current.Use();
            }
            if (Event.current.type == EventType.ScrollWheel)
            {
                FPPQuickbarControls.scrollAccumulatedDelta += Event.current.delta.y;
                if (FPPQuickbarControls.scrollAccumulatedDelta > 0.95f)
                {
                    flag2 = true;
                    FPPQuickbarControls.scrollAccumulatedDelta = 0f;
                }
                else if (FPPQuickbarControls.scrollAccumulatedDelta < -0.95f)
                {
                    flag = true;
                    FPPQuickbarControls.scrollAccumulatedDelta = 0f;
                }
            }
            if (Event.current.type == EventType.KeyDown)
            {
                FPPQuickbarControls.scrollAccumulatedDelta = 0f;
            }
            if (flag || flag2)
            {
                Item item = Get.WheelSelector.ForSubject as Item;
                int? num = ((item != null) ? Get.NowControlledActor.Inventory.Quickbar.GetCurrentIndexOf(item) : null);
                if (num != null)
                {
                    Item[] quickbarItems = Get.NowControlledActor.Inventory.QuickbarItems;
                    Item item2 = null;
                    if (flag2)
                    {
                        for (int i = num.Value + 1; i < quickbarItems.Length; i++)
                        {
                            if (FPPQuickbarControls.< HandleCyclingWithWheelSelectorOpen > g__HasOptions | 36_0(quickbarItems[i]))
                            {
                                item2 = quickbarItems[i];
                                break;
                            }
                        }
                    }
                    else if (flag)
                    {
                        for (int j = num.Value - 1; j >= 0; j--)
                        {
                            if (FPPQuickbarControls.< HandleCyclingWithWheelSelectorOpen > g__HasOptions | 36_0(quickbarItems[j]))
                            {
                                item2 = quickbarItems[j];
                                break;
                            }
                        }
                    }
                    if (item2 != null)
                    {
                        bool flag3;
                        List<ValueTuple<string, Action, string>> possibleInteractions = Get.NowControlledActor.Inventory.GetPossibleInteractions(item2, out flag3, true);
                        Get.UI.OpenWheelSelector(possibleInteractions, item2, null, false, true);
                    }
                }
            }
        }

        private static void OpenQuickbarMenu()
        {
            List<WheelSelector.Option> list = new List<WheelSelector.Option>();
            Item[] quickbarItems = Get.NowControlledActor.Inventory.QuickbarItems;
            for (int i = 0; i < quickbarItems.Length; i++)
            {
                if (quickbarItems[i] != null)
                {
                    int index2 = i;
                    list.Add(new WheelSelector.Option(quickbarItems[i].LabelCap, delegate
                    {
                        FPPQuickbarControls.OnQuickbarItemClicked(index2);
                    }, null, quickbarItems[i]));
                }
            }
            List<Spell> all = Get.NowControlledActor.Spells.All;
            for (int j = 0; j < all.Count; j++)
            {
                int index = j;
                list.Add(new WheelSelector.Option(all[j].LabelCap, delegate
                {
                    FPPQuickbarControls.OnSpellClicked(index);
                }, null, all[j]));
            }
            if (SpellChooserUtility.ChoicesToShow.Count != 0)
            {
                list.Add(new WheelSelector.Option("QuickbarMenuNewSpell".Translate(), delegate
                {
                    SpellChooserUtility.TryShowWheelSelectorWithChoices();
                }, null, null));
            }
            list.Add(new WheelSelector.Option("Leave".Translate(), delegate
            {
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
            }, null, null));
            Get.UI.OpenWheelSelector(list, null, "QuickbarMenuTitle".Translate(), false, false, true);
        }

        [CompilerGenerated]
        internal static bool <HandleCyclingWithWheelSelectorOpen>g__HasOptions|36_0(Item item)
		{
			bool flag;
			return item != null && Get.NowControlledActor.Inventory.GetPossibleInteractions(item, out flag, true).Count != 0;
		}

		private static float scrollAccumulatedDelta;

        private static float lastTimeShowChooseSpellHotkey = -99999f;

        public const int SlotSize = 70;

        private const int Spacing = 4;

        private const int Margin = 10;

        private const int ArrowSize = 40;

        private static readonly Texture2D ArrowTex = Assets.Get<Texture2D>("Textures/UI/Arrow");

        private static readonly Func<Spell, Action>[] CachedSpellMoveActionGetters = new Func<Spell, Action>[7];

        private static readonly Color BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.83f);

        public const int SpellsRowYOffset = 80;

        private const int MaxSpellKeyCodes = 7;
    }
}