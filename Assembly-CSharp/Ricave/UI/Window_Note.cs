using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Note : Window
    {
        protected override string Title
        {
            get
            {
                NoteSpec noteSpec = this.noteSpec;
                return ((noteSpec != null) ? noteSpec.LabelCap : null) ?? base.Title;
            }
        }

        public Window_Note(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public void SetNoteSpec(NoteSpec noteSpec)
        {
            this.noteSpec = noteSpec;
        }

        public void SetCustomText(string customText)
        {
            this.customText = customText;
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            string text = "\"{0}\"";
            NoteSpec noteSpec = this.noteSpec;
            string text2 = RichText.Italics(text.Formatted(((noteSpec != null) ? noteSpec.Text : null) ?? this.customText));
            Widgets.Align = TextAnchor.UpperCenter;
            Widgets.Label(rect, text2, true, null, null, false);
            Widgets.ResetAlign();
            Widgets.Align = TextAnchor.UpperRight;
            Rect rect2 = rect.CutFromTop(Widgets.CalcHeight(text2, rect.width) + 12f);
            string text3 = "~{0}";
            NoteSpec noteSpec2 = this.noteSpec;
            Widgets.Label(rect2, text3.Formatted(((noteSpec2 != null) ? noteSpec2.AuthorLabel : null) ?? "?"), true, null, null, false);
            Widgets.ResetAlign();
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private NoteSpec noteSpec;

        private string customText;
    }
}