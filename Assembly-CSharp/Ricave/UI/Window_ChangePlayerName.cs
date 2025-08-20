using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_ChangePlayerName : Window
    {
        public Window_ChangePlayerName(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            if (Get.KeyBinding_Accept.JustPressed && !this.playerName.NullOrWhitespace())
            {
                this.Accept();
            }
            Widgets.FontSizeScalable = 25;
            Widgets.LabelCenteredH(rect.TopCenter(), "EnterNewName".Translate(), true, null, null, false);
            Widgets.ResetFontSize();
            GUI.SetNextControlName("NewPlayerName");
            this.playerName = Widgets.TextField(RectUtility.CenteredAt(rect.TopCenter().MovedBy(0f, 100f), 400f, 30f), this.playerName, true, false);
            if (this.playerName.Length > 22)
            {
                this.playerName = this.playerName.Substring(0, 22);
            }
            if (this.playerName.NullOrWhitespace())
            {
                this.playerName = "";
            }
            if (!this.focusedTextField)
            {
                this.focusedTextField = true;
                GUI.FocusControl("NewPlayerName");
            }
            if (base.DoBottomButton("Cancel".Translate(), inRect, 0, 2, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
            if ((base.DoBottomButton("Accept".Translate(), inRect, 1, 2, true, null) || Get.KeyBinding_Accept.JustPressed) && !this.playerName.NullOrWhitespace())
            {
                this.Accept();
            }
        }

        private void Accept()
        {
            Get.Progress.PlayerName = this.playerName.Trim();
            Get.WindowManager.Close(this, true);
        }

        public override void OnClosed()
        {
            base.OnClosed();
            SteamKeyboardUtility.OnTextFieldLikelyNoLongerFocused();
        }

        private string playerName = "";

        private bool focusedTextField;
    }
}