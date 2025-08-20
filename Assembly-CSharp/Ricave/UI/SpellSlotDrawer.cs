using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class SpellSlotDrawer
    {
        public static void DoSpellSlot(Rect rect, Spell spell, Func<Spell, Action> moveActionGetter, bool doBackground = true)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }
            if (doBackground)
            {
                GUIExtra.DrawRoundedRectBump(rect, SpellSlotDrawer.BackgroundColor, false, true, true, true, true, null);
            }
            if (Get.DragAndDrop.HoveringDragged<Spell>(rect, null) && Get.DragAndDrop.DraggedObject != spell)
            {
                GUIExtra.DrawHighlight(rect, true, true, true, true, 1f);
            }
            else if (!Get.WindowManager.IsContextMenuOpen && spell != null)
            {
                GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            }
            if (spell != null)
            {
                if (Get.DragAndDrop.IsDragging(spell))
                {
                    GUI.color = new Color(0.5f, 0.5f, 0.5f) * spell.IconColor;
                }
                else
                {
                    GUI.color = spell.IconColor;
                }
                GUI.DrawTexture(rect, spell.Icon);
                GUI.color = Color.white;
                ItemSlotDrawer.DoManaAndStaminaCost(spell, rect);
                ItemSlotDrawer.DoCooldown(spell, rect);
                Get.DragAndDrop.RegisterDraggable<Spell>(spell, rect, DragAndDrop.DragSource.SpellsBar, null);
                Get.Tooltips.RegisterTip(rect, spell, null, null);
                if (Get.UseOnTargetUI.TargetingUsable == spell)
                {
                    ItemSlotDrawer.DrawEquippedOutline(rect, false, false);
                }
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.Over(rect))
                {
                    bool flag;
                    List<ValueTuple<string, Action, string>> possibleInteractions = Get.NowControlledActor.Spells.GetPossibleInteractions(spell, out flag, false);
                    Get.WindowManager.OpenContextMenu(possibleInteractions, spell.LabelCap);
                    Event.current.Use();
                }
            }
            Spell spell2 = Get.DragAndDrop.ConsumeDropped<Spell>(rect, null);
            if (spell2 != null)
            {
                SpellSlotDrawer.HandleDroppedSpell(spell2, moveActionGetter);
            }
        }

        private static void HandleDroppedSpell(Spell dropped, Func<Spell, Action> moveActionGetter)
        {
            ActionViaInterfaceHelper.TryDo(() => moveActionGetter(dropped));
        }

        private static readonly Color BackgroundColor = ItemSlotDrawer.BackgroundColor;
    }
}