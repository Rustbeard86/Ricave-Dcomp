using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class UseOnTargetUI
    {
        public IUsable TargetingUsable
        {
            get
            {
                return this.targetingUsable;
            }
        }

        public void BeginTargeting(IUsable usable, bool closeIfItemNotInInventory = false)
        {
            this.targetingUsable = usable;
            this.closeIfItemNotInInventory = closeIfItemNotInInventory;
            Get.UI.OnBeginTargeting();
        }

        public void StopTargeting()
        {
            this.targetingUsable = null;
            this.closeIfItemNotInInventory = false;
        }

        public void OnGUI()
        {
            if (this.targetingUsable == null)
            {
                return;
            }
            if (this.closeIfItemNotInInventory)
            {
                Item item = this.targetingUsable as Item;
                if (item != null && !Get.NowControlledActor.Inventory.Contains(item))
                {
                    this.StopTargeting();
                    return;
                }
            }
            if (Get.KeyBinding_Cancel.JustPressed)
            {
                this.StopTargeting();
                if (Event.current.type == EventType.KeyDown)
                {
                    Event.current.Use();
                }
                Get.Sound_CloseWindow.PlayOneShot(null, 1f, 1f);
                return;
            }
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            Rect rect = new Rect(Widgets.ScreenCenter.x - 40f, Widgets.VirtualHeight - 100f - 80f, 80f, 80f);
            if (FPPQuickbarControls.ShouldDoSpellsRow)
            {
                rect.y -= 80f;
            }
            GUI.color = this.targetingUsable.IconColor;
            GUI.DrawTexture(rect, this.targetingUsable.Icon);
            GUI.color = Color.white;
            Get.Tooltips.RegisterTip(rect, this.targetingUsable, null, null);
            Widgets.FontSizeScalable = 18;
            Widgets.LabelCentered(new Vector2(rect.center.x, rect.y - 46f), this.targetingUsable.UseLabel_Other, true, null, null, false, false, false, null);
            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            string text = ((!ControllerUtility.InControllerMode && Get.KeyBinding_Cancel.KeyCode == KeyCode.Escape) ? "EscapeToCancelShort".Translate() : "EscapeToCancel".Translate().FormattedKeyBindings());
            Widgets.LabelCentered(new Vector2(rect.center.x, rect.y - 25f), "({0})".Formatted(text), true, null, null, false, false, false, null);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
        }

        public void OnSpellRemoved(Actor actor, Spell spell)
        {
            if (spell == this.targetingUsable)
            {
                this.StopTargeting();
            }
        }

        private IUsable targetingUsable;

        private bool closeIfItemNotInInventory;

        private const int MarginBottom = 100;

        private const int IconSize = 80;
    }
}