using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class DialogueNode
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
                return this.text.FormattedWithPlayerName();
            }
        }

        public List<DialogueNode.Response> Responses
        {
            get
            {
                return this.responses;
            }
        }

        public bool HasResponses
        {
            get
            {
                return !this.responses.NullOrEmpty<DialogueNode.Response>();
            }
        }

        public string Tag
        {
            get
            {
                return this.tag;
            }
        }

        public string Next
        {
            get
            {
                return this.next;
            }
        }

        [Saved]
        private SpeakerSpec speaker;

        [Saved]
        [Translatable]
        private string text;

        [Saved]
        private List<DialogueNode.Response> responses;

        [Saved]
        private string tag;

        [Saved]
        private string next;

        public class Response
        {
            public string Text
            {
                get
                {
                    return this.text;
                }
            }

            public string JumpToNode
            {
                get
                {
                    return this.jumpToNode;
                }
            }

            [Saved]
            [Translatable]
            private string text;

            [Saved]
            private string jumpToNode;
        }
    }
}