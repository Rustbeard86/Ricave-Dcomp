using System;

namespace Ricave.Core
{
    public class LessonSpec : Spec
    {
        public string Text
        {
            get
            {
                return this.text;
            }
        }

        public string TextController
        {
            get
            {
                return this.textController;
            }
        }

        public string TextSteamDeck
        {
            get
            {
                return this.textSteamDeck;
            }
        }

        public string FlashingText
        {
            get
            {
                return this.flashingText;
            }
        }

        public string FlashingTextController
        {
            get
            {
                return this.flashingTextController;
            }
        }

        public float Order
        {
            get
            {
                return this.order;
            }
        }

        public bool DespawnGate
        {
            get
            {
                return this.despawnGate;
            }
        }

        public bool InLobbyOnly
        {
            get
            {
                return this.inLobbyOnly;
            }
        }

        [Saved]
        [Translatable]
        private string text;

        [Saved]
        [Translatable]
        private string textController;

        [Saved]
        [Translatable]
        private string textSteamDeck;

        [Saved]
        [Translatable]
        private string flashingText;

        [Saved]
        [Translatable]
        private string flashingTextController;

        [Saved]
        private float order;

        [Saved]
        private bool despawnGate;

        [Saved]
        private bool inLobbyOnly;
    }
}