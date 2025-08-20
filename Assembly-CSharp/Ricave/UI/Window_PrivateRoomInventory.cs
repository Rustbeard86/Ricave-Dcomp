using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_PrivateRoomInventory : Window
    {
        public Window_PrivateRoomInventory(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            int num = 0;
            Rect rect = inRect;
            rect.height -= 60f;
            Rect rect2 = Widgets.BeginScrollView(rect, null);
            foreach (ValueTuple<EntitySpec, int> valueTuple in Get.PrivateRoom.StructuresInInventoryInDisplayOrder)
            {
                EntitySpec item = valueTuple.Item1;
                int item2 = valueTuple.Item2;
                Rect rect3 = new Rect(rect2.x, (float)(num * 75), rect2.width, 75f);
                GUIExtra.DrawRoundedRectBump(rect3, this.BackgroundColor.Lighter(0.04f), false, true, true, true, true, null);
                GUIExtra.DrawHighlightIfMouseover(rect3, true, true, true, true, true);
                Rect rect4 = rect3.ContractedBy(20f);
                Rect rect5 = new Rect(rect4.x, rect4.y, rect4.height, rect4.height);
                GUI.color = item.IconColor;
                GUI.DrawTexture(rect5, item.Icon);
                GUI.color = Color.white;
                Rect rect6 = new Rect(rect5.xMax + 5f, rect4.y, rect4.width - rect5.width - 5f, rect4.height);
                Widgets.Align = TextAnchor.MiddleLeft;
                string text = RichText.Label(item, false);
                if (item2 != 1)
                {
                    text = text.Concatenated(RichText.Bold(" x{0}".Formatted(item2)));
                }
                Widgets.Label(rect6, text, true, null, null, false);
                Widgets.ResetAlign();
                Rect rect7 = new Rect(rect4.xMax - 100f, rect4.y, 100f, 29f);
                if (Widgets.Button(rect7, "Place".Translate(), true, null, null, true, true, true, true, null, true, true, null, false, null, null))
                {
                    Get.Player.PrivateRoomPlaceStructureUsable.StartTargeting(item);
                }
                if (!Mouse.Over(rect7))
                {
                    Get.Tooltips.RegisterTip(rect3, item, null, null);
                }
                num++;
            }
            Widgets.EndScrollView(rect, (float)(num * 75), false, false);
            if (Widgets.Button(new Rect(inRect.center.x - 90f, inRect.yMax - 30f - 14.5f, 180f, 29f), "PickUp".Translate(), true, null, null, true, true, true, true, null, true, true, null, false, null, null))
            {
                Get.UseOnTargetUI.BeginTargeting(Get.Player.PrivateRoomPickUpStructureUsable, false);
            }
        }

        private const int RowHeight = 75;

        private const float PlaceButtonWidth = 100f;

        private const float PlaceButtonHeight = 29f;

        private const float PickUpButtonWidth = 180f;

        private const float BottomPartHeight = 60f;
    }
}