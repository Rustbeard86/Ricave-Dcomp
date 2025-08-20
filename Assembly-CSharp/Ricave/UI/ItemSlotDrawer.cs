using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class ItemSlotDrawer
    {
        public static void DoItemSlot(Rect rect, Item item, Func<Item, Action> moveActionGetter, bool doBackground = true, ItemSlotSpec slot = null, bool animation = true)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }
            if (ItemSlotDrawer.mouseoverItemFrame < Clock.Frame - 1)
            {
                ItemSlotDrawer.mouseoverItem = null;
            }
            if (doBackground)
            {
                GUIExtra.DrawRoundedRectBump(rect, ItemSlotDrawer.BackgroundColor, false, true, true, true, true, null);
            }
            bool flag = Get.DragAndDrop.HoveringDragged<Item>(rect, null) && Get.DragAndDrop.DraggedObject != item && (slot == null || ((Item)Get.DragAndDrop.DraggedObject).Spec.Item.ItemSlot == slot);
            bool flag2;
            if (slot != null)
            {
                Item item2 = (Get.DragAndDrop.DraggedObject ?? ItemSlotDrawer.mouseoverItem) as Item;
                if (item2 != null && item2 != item && item2.Spec.Item.ItemSlot == slot)
                {
                    flag2 = !Get.WindowManager.IsContextMenuOpen;
                    goto IL_00CD;
                }
            }
            flag2 = false;
        IL_00CD:
            bool flag3 = flag2;
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && item != null && item.Spec.Item.IsEquippable && !Get.DragAndDrop.Dragging && !Get.WindowManager.IsContextMenuOpen && Get.UI.InventoryOpen)
            {
                flag3 = true;
            }
            if (flag)
            {
                GUIExtra.DrawHighlight(rect, true, true, true, true, 1f);
            }
            else
            {
                if (flag3)
                {
                    GUIExtra.DrawHighlight(rect, true, true, true, true, 0.4f + Calc.PulseUnscaled(5f, 0.2f));
                }
                if (!Get.WindowManager.IsContextMenuOpen && item != null && !Get.DragAndDrop.Dragging)
                {
                    GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
                }
            }
            if (item != null)
            {
                Color? backgroundColor = ItemSlotDrawer.GetBackgroundColor(item);
                if (backgroundColor != null)
                {
                    GUIExtra.DrawRoundedRect(rect, backgroundColor.Value, true, true, true, true, null);
                }
                if (!item.Identified)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.15f);
                    GUIExtra.DrawTexture(rect, ItemSlotDrawer.UnidentifiedBackground);
                    GUI.color = Color.white;
                }
                if (Get.DragAndDrop.IsDragging(item))
                {
                    GUI.color = new Color(0.5f, 0.5f, 0.5f) * item.IconColor;
                }
                else
                {
                    GUI.color = item.IconColor;
                }
                GUIExtra.DrawTexture(rect, item.Icon);
                GUI.color = Color.white;
                ItemSlotDrawer.DoUsesLeftProgressBar(item, rect);
                ItemSlotDrawer.DoMiniIcons(item, rect);
                ItemSlotDrawer.DoStackCount(item, rect);
                ItemSlotDrawer.DoManaAndStaminaCost(item, rect);
                ItemSlotDrawer.DoRampUp(item, rect);
                ItemSlotDrawer.DoCooldown(item, rect);
                Get.DragAndDrop.RegisterDraggable<Item>(item, rect, DragAndDrop.DragSource.Inventory, null);
                Get.Tooltips.RegisterTip(rect, item, null, null);
                if (Get.LessonManager.CurrentLesson == Get.Lesson_Inventory)
                {
                    GUIExtra.DrawLessonHint(rect);
                }
                if ((Get.NowControlledActor.Inventory.Equipped.IsEquipped(item) && item.Cursed) || Get.UseOnTargetUI.TargetingUsable == item)
                {
                    ItemSlotDrawer.DrawEquippedOutline(rect, item.Cursed && item.Identified, false);
                }
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.Over(rect))
                {
                    bool flag4;
                    List<ValueTuple<string, Action, string>> possibleInteractions = Get.NowControlledActor.Inventory.GetPossibleInteractions(item, out flag4, false);
                    Get.WindowManager.OpenContextMenu(possibleInteractions, item.LabelCap);
                    Event.current.Use();
                }
                ItemSlotDrawer.HandleInventoryManagementSpecialClicks(rect, item);
                if (Mouse.Over(rect))
                {
                    ItemSlotDrawer.mouseoverItem = item;
                    ItemSlotDrawer.mouseoverItemFrame = Clock.Frame;
                }
                if (animation)
                {
                    ExpandingIconAnimation.Do(rect, item.Icon, item.IconColor, item.TimeAddedToInventory, 0.3f, 0.6f, 0.55f);
                }
            }
            else if (slot != null)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.2f);
                GUIExtra.DrawTexture(rect, slot.Icon);
                GUI.color = Color.white;
                Get.Tooltips.RegisterTip(rect, slot.LabelCap, null);
            }
            Item item3 = Get.DragAndDrop.ConsumeDropped<Item>(rect, null);
            if (item3 != null)
            {
                ItemSlotDrawer.HandleDroppedItem(item3, moveActionGetter, slot);
            }
        }

        private static void DoStackCount(Item item, Rect rect)
        {
            if (item.StackCount < 2)
            {
                return;
            }
            Widgets.FontSizeScalable = 17;
            Widgets.FontBold = true;
            Widgets.Align = TextAnchor.UpperRight;
            Widgets.Label(rect.MovedBy(-6f, 2f), "x{0}".Formatted(item.StackCount.ToStringCached()), true, null, null, true);
            Widgets.ResetAlign();
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
        }

        private static void DoRampUp(Item item, Rect rect)
        {
            if (item.RampUp == 0 || !item.Identified)
            {
                return;
            }
            Widgets.FontSizeScalable = 17;
            Widgets.Align = TextAnchor.LowerRight;
            Widgets.Label(rect.MovedBy(-6f, -16f), item.RampUp.ToStringOffset(true), true, null, null, true);
            Widgets.ResetAlign();
            Widgets.ResetFontSize();
        }

        public static void DoManaAndStaminaCost(IUsable usable, Rect rect)
        {
            if (usable.ManaCost != 0)
            {
                ItemSlotDrawer.DoManaCost(usable.ManaCost, rect);
                rect.xMin += 16f;
            }
            ItemSlotDrawer.DoStaminaCost(usable.StaminaCost, rect);
        }

        public static void DoManaCost(int manaCost, Rect rect)
        {
            if (manaCost == 0)
            {
                return;
            }
            string text = manaCost.ToStringCached();
            if (Get.NowControlledActor.Mana >= manaCost)
            {
                text = RichText.Mana(text);
            }
            else
            {
                GUI.color = new Color(1f, 0.2f, 0.2f);
            }
            Widgets.FontSizeScalable = 17;
            Widgets.FontBold = true;
            Widgets.Label(rect.MovedBy(6f, 2f), text, true, null, null, true);
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            GUI.color = Color.white;
        }

        public static void DoStaminaCost(int staminaCost, Rect rect)
        {
            if (staminaCost == 0)
            {
                return;
            }
            string text = staminaCost.ToStringCached();
            if (Get.NowControlledActor.Stamina >= staminaCost)
            {
                text = RichText.Stamina(text);
            }
            else
            {
                GUI.color = new Color(1f, 0.2f, 0.2f);
            }
            Widgets.FontSizeScalable = 17;
            Widgets.FontBold = true;
            Widgets.Label(rect.MovedBy(6f, 2f), text, true, null, null, true);
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            GUI.color = Color.white;
        }

        public static void DoCooldown(IUsable usable, Rect rect)
        {
            if (usable.LastUseSequence == null)
            {
                return;
            }
            int num = Get.TurnManager.CurrentSequence - usable.LastUseSequence.Value;
            int num2 = usable.CooldownTurns * 12;
            if (num >= num2)
            {
                return;
            }
            ProgressBarDrawer progressBarDrawer = Get.ProgressBarDrawer;
            int num3 = num2 - num;
            int num4 = num2;
            Color color = new Color(1f, 0.1f, 0.1f, 0.35f);
            float num5 = 1f;
            float num6 = 1f;
            bool flag = false;
            bool flag2 = false;
            Entity entity = usable as Entity;
            progressBarDrawer.Draw(rect, num3, num4, color, num5, num6, flag, flag2, (entity != null) ? new int?(300000000 + entity.InstanceID) : null, ProgressBarDrawer.ValueChangeDirection.Constant, null, true, true, true, true, false, false, null);
        }

        public static void DrawEquippedOutline(Rect rect, bool cursed = false, bool bold = false)
        {
            if (bold)
            {
                GUIExtra.DrawRoundedRectOutline(rect, cursed ? Color.red : new Color(0.7f, 0.7f, 0.7f), 4f, true, true, true, true);
                return;
            }
            GUIExtra.DrawRoundedRectOutline(rect, cursed ? Color.red : ItemSlotDrawer.EquippedOutlineColor, 2f, true, true, true, true);
        }

        public static void HandleInventoryManagementSpecialClicks(Rect rect, Item item)
        {
            if (item == null)
            {
                return;
            }
            if (!Get.DragAndDrop.DraggingOrDroppedJustNow)
            {
                if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Widgets.ButtonInvisible(rect, false, false) && Event.current.button == 0)
                {
                    ItemSlotDrawer.OnShiftClickedItem(item);
                }
                else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Widgets.ButtonInvisible(rect, false, false) && Event.current.button == 0)
                {
                    ItemSlotDrawer.OnCtrlClickedItem(item);
                }
                else if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Widgets.ButtonInvisible(rect, false, false) && Event.current.button == 0)
                {
                    ItemSlotDrawer.OnAltClickedItem(item);
                }
            }
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 2 && Mouse.Over(rect) && !Get.DragAndDrop.DraggingOrDroppedJustNow && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
            {
                Event.current.Use();
                ItemSlotDrawer.OnDoubleClickedItem(item);
            }
        }

        private static void OnShiftClickedItem(Item item)
        {
            if (item.Spec.Item.IsEquippable)
            {
                if (Get.NowControlledActor.Inventory.Equipped.IsEquipped(item))
                {
                    if (!item.Cursed && !Get.NowControlledActor.Inventory.BackpackAndQuickbarFull)
                    {
                        ActionViaInterfaceHelper.TryDo(() => new Action_Unequip(Get.Action_Unequip, Get.NowControlledActor, item));
                        return;
                    }
                }
                else if (item.Spec.Item.IsEquippable)
                {
                    ActionViaInterfaceHelper.TryDo(() => new Action_Equip(Get.Action_Equip, Get.NowControlledActor, item));
                    return;
                }
            }
            else if (Get.NowControlledActor.Inventory.Backpack.Contains(item))
            {
                int quickbarSlot = Get.NowControlledActor.Inventory.Quickbar.FirstFreeSlot;
                if (quickbarSlot >= 0 && ActionViaInterfaceHelper.TryDo(() => new Action_MoveItemInInventory(Get.Action_MoveItemInInventory, Get.NowControlledActor, item, null, new int?(quickbarSlot))))
                {
                    Get.Sound_DragEndItem.PlayOneShot(null, 1f, 1f);
                    return;
                }
            }
            else if (Get.NowControlledActor.Inventory.Quickbar.Contains(item))
            {
                Vector2Int? slot = Get.NowControlledActor.Inventory.Backpack.FirstFreeSlot;
                if (slot != null && ActionViaInterfaceHelper.TryDo(() => new Action_MoveItemInInventory(Get.Action_MoveItemInInventory, Get.NowControlledActor, item, slot, null)))
                {
                    Get.Sound_DragEndItem.PlayOneShot(null, 1f, 1f);
                }
            }
        }

        private static void OnCtrlClickedItem(Item item)
        {
            if (!Get.NowControlledActor.Inventory.Equipped.IsEquipped(item) || !item.Cursed)
            {
                ActionViaInterfaceHelper.TryDo(() => new Action_DropItem(Get.Action_DropItem, Get.NowControlledActor, item));
            }
        }

        private static void OnAltClickedItem(Item item)
        {
            if (!Get.NowControlledActor.Inventory.Equipped.IsEquipped(item) || !item.Cursed)
            {
                ActionViaInterfaceHelper.TryDo(() => new Action_DestroyItemInInventory(Get.Action_DestroyItemInInventory, Get.NowControlledActor, item));
            }
        }

        private static void OnDoubleClickedItem(Item item)
        {
            Get.DragAndDrop.Interrupt();
            Get.TurnManager.TryDoAllNextForcedOrNonPlayerActionsNow();
            if (!Get.TurnManager.IsPlayerTurn_CanChooseNextAction || !Get.NowControlledActor.Inventory.Contains(item))
            {
                return;
            }
            bool flag;
            List<ValueTuple<string, Action, string>> list = Get.NowControlledActor.Inventory.GetPossibleInteractions(item, out flag, true);
            if (list.Count == 1 && flag && list[0].Item2 != null)
            {
                list[0].Item2();
                return;
            }
            bool flag2;
            list = Get.NowControlledActor.Inventory.GetPossibleInteractions(item, out flag2, false);
            Get.WindowManager.OpenContextMenu(list, item.LabelCap);
        }

        private static void HandleDroppedItem(Item dropped, Func<Item, Action> moveActionGetter, ItemSlotSpec slot = null)
        {
            if (moveActionGetter == null)
            {
                return;
            }
            if (slot != null && dropped.Spec.Item.ItemSlot != slot)
            {
                Get.PlayLog.Add("DroppedItemInInvalidSlot".Translate());
                return;
            }
            Inventory parentInventory = dropped.ParentInventory;
            if (((parentInventory != null) ? parentInventory.Owner : null) != null && dropped.ParentInventory.Owner != Get.NowControlledActor && Get.Player.Party.Contains(dropped.ParentInventory.Owner))
            {
                PartyUI.TryTransferItem(dropped.ParentInventory.Owner, Get.NowControlledActor, dropped);
                return;
            }
            ActionViaInterfaceHelper.TryDo(() => moveActionGetter(dropped));
        }

        public static Color? GetBackgroundColor(Item item)
        {
            if (!item.Identified)
            {
                return new Color?(ItemSlotDrawer.UnidentifiedColor);
            }
            if (item.Cursed)
            {
                return new Color?(ItemSlotDrawer.CursedColor);
            }
            if (item.Spec.Item.IsGoodConsumable)
            {
                return new Color?(ItemSlotDrawer.GoodConsumableColor);
            }
            if (item.Spec.Item.IsBadConsumable)
            {
                return new Color?(ItemSlotDrawer.BadConsumableColor);
            }
            return null;
        }

        private static void DoUsesLeftProgressBar(Item item, Rect rect)
        {
            int num;
            int num2;
            if (item.ChargesLeft > 0)
            {
                num = item.ChargesLeft;
                num2 = item.Spec.Item.DefaultChargesLeft;
            }
            else
            {
                num = item.UsesLeft;
                num2 = item.Spec.Item.DefaultUsesLeft;
            }
            if (num <= 0 || num2 <= 0 || num >= num2)
            {
                return;
            }
            Get.ProgressBarDrawer.Draw(rect.ContractedBy(1f).BottomPart(7f), num, num2, new Color(0.75f, 0.75f, 0.75f, 0.5f), 1f, 1f, false, false, new int?(400000000 + item.InstanceID), ProgressBarDrawer.ValueChangeDirection.Constant, null, false, false, true, true, false, false, null);
        }

        private static void DoMiniIcons(Item item, Rect rect)
        {
            ItemSlotDrawer.<> c__DisplayClass27_0 CS$<> 8__locals1;
            CS$<> 8__locals1.rect = rect;
            if (!item.Identified)
            {
                return;
            }
            List<UseEffectDrawRequest> allDrawRequests = item.UseEffects.AllDrawRequests;
            CS$<> 8__locals1.curX = CS$<> 8__locals1.rect.xMax + 1f;
            CS$<> 8__locals1.count = 0;
            if (Get.IdentificationGroups.GetIdentificationGroup(item.Spec) != null && allDrawRequests.Count != 0)
            {
                ItemSlotDrawer.< DoMiniIcons > g__DoIcon | 27_0(allDrawRequests[0].Icon, allDrawRequests[0].IconColor, 2f, ref CS$<> 8__locals1);
                return;
            }
            foreach (UseEffectDrawRequest useEffectDrawRequest in allDrawRequests)
            {
                ItemSlotDrawer.< DoMiniIcons > g__DoIcon | 27_0(useEffectDrawRequest.Icon, useEffectDrawRequest.IconColor, 1f, ref CS$<> 8__locals1);
                if (CS$<> 8__locals1.count >= 3)
				{
                    break;
                }
            }
            if (CS$<> 8__locals1.count < 3)
			{
                foreach (ConditionDrawRequest conditionDrawRequest in item.ConditionsEquipped.AllDrawRequests)
                {
                    ItemSlotDrawer.< DoMiniIcons > g__DoIcon | 27_0(conditionDrawRequest.Icon, conditionDrawRequest.IconColor, 1f, ref CS$<> 8__locals1);
                    if (CS$<> 8__locals1.count >= 3)
					{
                        break;
                    }
                }
            }
        }

        [CompilerGenerated]
        internal static void <DoMiniIcons>g__DoIcon|27_0(Texture2D icon, Color iconColor, float sizeFactor, ref ItemSlotDrawer.<>c__DisplayClass27_0 A_3)
		{
            float num = 19f * sizeFactor;
        A_3.curX -= num + 2f;
			Rect rect = new Rect(A_3.curX, A_3.rect.yMax - num - 1f, num, num);
        GUI.color = iconColor;
			GUIExtra.DrawTexture(rect, icon);
			GUI.color = Color.white;
			int count = A_3.count;
        A_3.count = count + 1;
		}

		private static Item mouseoverItem;

        private static int mouseoverItemFrame = -99999;

        public static readonly Vector2 SlotSize = new Vector2(80f, 80f);

        public const float Spacing = 5f;

        public static readonly Color BackgroundColor = new Color(0.24f, 0.24f, 0.24f, 0.5f);

        private static readonly Color EquippedOutlineColor = new Color(1f, 1f, 1f);

        private static readonly Color UnidentifiedColor = new Color(0.53f, 0.53f, 1f, 0.3f);

        private static readonly Color CursedColor = new Color(1f, 0.53f, 0.53f, 0.3f);

        private static readonly Color GoodConsumableColor = new Color(0.2f, 1f, 0.2f, 0.3f);

        private static readonly Color BadConsumableColor = new Color(1f, 0.2f, 0.2f, 0.3f);

        private static readonly Texture2D UnidentifiedBackground = Assets.Get<Texture2D>("Textures/UI/UnidentifiedBackground");
    }
}