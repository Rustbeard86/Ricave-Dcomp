using System;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class MouseAttachmentDrawerGOC : MonoBehaviour
    {
        private void OnGUI()
        {
            Widgets.ApplySkin();
            GUI.depth = -100;
            Get.DragAndDrop.DrawMouseAttachment();
            this.DrawMouseTextAttachment();
            Get.Tooltips.DrawTooltips();
        }

        public void CheckEnableDisable()
        {
            bool flag = Get.DragAndDrop.Dragging || Get.Tooltips.Any || !this.GetMouseTextAttachment().Item1.NullOrEmpty();
            base.gameObject.SetActive(flag);
        }

        private void DrawMouseTextAttachment()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            ValueTuple<string, Color?> mouseTextAttachment = this.GetMouseTextAttachment();
            if (mouseTextAttachment.Item1.NullOrEmpty())
            {
                return;
            }
            GUI.color = mouseTextAttachment.Item2 ?? new Color(0.72f, 0.72f, 0.72f);
            Widgets.LabelCenteredV(Event.current.mousePosition.WithAddedY(48f), mouseTextAttachment.Item1, true, null, null, false);
            GUI.color = Color.white;
        }

        private ValueTuple<string, Color?> GetMouseTextAttachment()
        {
            if (!Cursor.visible || Get.DragAndDrop.Dragging || Get.WindowManager.IsContextMenuOpen)
            {
                return default(ValueTuple<string, Color?>);
            }
            if (!Get.InMainMenu && (Get.UI.InventoryOpen || Get.UI.IsWheelSelectorOpen))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    return new ValueTuple<string, Color?>("Equip".Translate(), null);
                }
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    return new ValueTuple<string, Color?>("Drop".Translate(), null);
                }
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {
                    return new ValueTuple<string, Color?>("Destroy".Translate(), new Color?(new Color(1f, 0.25f, 0.25f)));
                }
            }
            return default(ValueTuple<string, Color?>);
        }
    }
}