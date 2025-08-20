using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_NewRunWithSeed : Window
    {
        public Action<string> OnAccept { get; set; }

        public Window_NewRunWithSeed(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            if (Get.KeyBinding_Accept.JustPressed && this.seed.Length != 0)
            {
                this.OnAccept(this.seed);
                Get.WindowManager.Close(this, true);
            }
            Widgets.FontSizeScalable = 25;
            Widgets.LabelCenteredH(rect.TopCenter(), "EnterSeed".Translate(), true, null, null, false);
            Widgets.ResetFontSize();
            GUI.SetNextControlName("NewRunSeed");
            this.seed = Widgets.TextField(RectUtility.CenteredAt(rect.TopCenter().MovedBy(0f, 100f), 400f, 30f), this.seed, true, false);
            if (!this.focusedTextField)
            {
                this.focusedTextField = true;
                GUI.FocusControl("NewRunSeed");
            }
            if (base.DoBottomButton("Cancel".Translate(), inRect, 0, 2, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
            if ((base.DoBottomButton("Accept".Translate(), inRect, 1, 2, true, null) || Get.KeyBinding_Accept.JustPressed) && this.seed.Length != 0)
            {
                this.OnAccept(this.seed);
                Get.WindowManager.Close(this, true);
            }
        }

        public override void OnClosed()
        {
            base.OnClosed();
            SteamKeyboardUtility.OnTextFieldLikelyNoLongerFocused();
        }

        private string seed = "";

        private bool focusedTextField;
    }
}