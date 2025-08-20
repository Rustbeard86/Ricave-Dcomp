using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_TutorialPopup : Window
    {
        public Window_TutorialPopup(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public void SetText(Texture2D icon, string text)
        {
            this.icon = icon;
            this.text = text;
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            if (this.icon != null)
            {
                GUI.DrawTexture(rect.TopPart(60f).ContractedToSquare(), this.icon);
            }
            Widgets.Align = TextAnchor.UpperCenter;
            Widgets.LabelScrollable(rect.CutFromTop((this.icon != null) ? 70f : 0f), this.text, true, null, null, false);
            Widgets.ResetAlign();
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private Texture2D icon;

        private string text;
    }
}