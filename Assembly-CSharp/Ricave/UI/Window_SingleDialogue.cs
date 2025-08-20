using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_SingleDialogue : Window
    {
        public Window_SingleDialogue(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public void Init(DialogueSpec dialogue)
        {
            this.dialogue = dialogue;
        }

        protected override void DoWindowContents(Rect inRect)
        {
            float num = 20f;
            Rect rect = Widgets.BeginScrollView(inRect, new int?(61328965));
            Rect rect2 = rect.ContractedBy(20f);
            DialogueDrawer.Draw(this.dialogue, rect2.x, rect2.width, ref num, ref this.letterShowingTimer);
            if (this.dialogue.Ended)
            {
                num += 80f;
                if (this.frameEnded == -1)
                {
                    this.frameEnded = Clock.Frame;
                }
                if (Clock.Frame != this.frameEnded && (base.DoBottomButton("Close".Translate(), rect, 0, 1, true, null) || Get.KeyBinding_Accept.JustPressed || KeyCodeUtility.CancelJustPressed))
                {
                    if (Event.current.type == EventType.KeyDown)
                    {
                        Event.current.Use();
                    }
                    Get.WindowManager.Close(this, true);
                }
            }
            Widgets.EndScrollView(inRect, num, true, true);
        }

        public override void OnClosed()
        {
            base.OnClosed();
            Get.Progress.Save();
        }

        private DialogueSpec dialogue;

        private float letterShowingTimer;

        private int frameEnded = -1;
    }
}