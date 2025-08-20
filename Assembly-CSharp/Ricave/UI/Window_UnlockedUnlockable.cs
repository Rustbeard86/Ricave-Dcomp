using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_UnlockedUnlockable : Window
    {
        public Window_UnlockedUnlockable(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public void SetUnlockable(UnlockableSpec unlockable)
        {
            this.unlockable = unlockable;
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            GUI.DrawTexture(rect.TopPart(60f).ContractedToSquare(), this.unlockable.Icon);
            string text;
            if (!this.unlockable.UnlockedText.NullOrEmpty())
            {
                text = this.unlockable.UnlockedText.FormattedKeyBindings();
            }
            else
            {
                text = this.unlockable.Description;
            }
            Widgets.Align = TextAnchor.UpperCenter;
            Widgets.LabelScrollable(rect.CutFromTop(70f), text, true, null, null, false);
            Widgets.ResetAlign();
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private UnlockableSpec unlockable;
    }
}