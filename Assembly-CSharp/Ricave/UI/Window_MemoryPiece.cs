using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_MemoryPiece : Window
    {
        public Window_MemoryPiece(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public void SetMemoryPiece(EntitySpec memoryPiece)
        {
            this.memoryPiece = memoryPiece;
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            Rect rect2 = rect.TopPart(60f).ContractedToSquare();
            GUI.color = this.memoryPiece.IconColorAdjusted;
            GUI.DrawTexture(rect2, this.memoryPiece.IconAdjusted);
            GUI.color = Color.white;
            string text;
            if (this.memoryPiece == Get.Entity_MemoryPiece1)
            {
                text = "MemoryPiece1".Translate();
            }
            else if (this.memoryPiece == Get.Entity_MemoryPiece2)
            {
                text = "MemoryPiece2".Translate();
            }
            else if (this.memoryPiece == Get.Entity_MemoryPiece3)
            {
                text = "MemoryPiece3".Translate();
            }
            else if (this.memoryPiece == Get.Entity_MemoryPiece4)
            {
                text = "MemoryPiece4".Translate();
            }
            else
            {
                text = this.memoryPiece.DescriptionAdjusted;
            }
            Widgets.Align = TextAnchor.UpperCenter;
            Widgets.LabelScrollable(rect.CutFromTop(70f), text, true, null, null, false);
            Widgets.ResetAlign();
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private EntitySpec memoryPiece;
    }
}