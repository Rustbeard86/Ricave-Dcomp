using System;

namespace Ricave.Core
{
    public class DialogueEntry
    {
        public SpeakerSpec Speaker
        {
            get
            {
                return this.speaker;
            }
        }

        public string Text
        {
            get
            {
                return this.text;
            }
        }

        protected DialogueEntry()
        {
        }

        public DialogueEntry(SpeakerSpec speaker, string text)
        {
            this.speaker = speaker;
            this.text = text;
        }

        [Saved]
        private SpeakerSpec speaker;

        [Saved]
        private string text;
    }
}