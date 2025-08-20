using System;
using UnityEngine;

namespace Ricave.Core
{
    public class SpeakerSpec : Spec, ISaveableEventsReceiver
    {
        public string LabelAdjusted
        {
            get
            {
                if (this != Get.Speaker_Player)
                {
                    return base.Label;
                }
                return Get.Progress.PlayerName;
            }
        }

        public string LabelAdjustedCap
        {
            get
            {
                return this.LabelAdjusted.CapitalizeFirst();
            }
        }

        public string LabelAdjustedUppercase
        {
            get
            {
                return this.LabelAdjusted.ToUppercase();
            }
        }

        public Texture2D Portrait
        {
            get
            {
                return this.portrait;
            }
        }

        public SoundSpec Voice
        {
            get
            {
                return this.voice;
            }
        }

        public Color Color
        {
            get
            {
                return this.color;
            }
        }

        public void OnLoaded()
        {
            if (!this.portraitPath.NullOrEmpty())
            {
                this.portrait = Assets.Get<Texture2D>(this.portraitPath);
                return;
            }
            Log.Error("SpeakerSpec " + base.SpecID + " has no portrait.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string portraitPath;

        [Saved]
        private SoundSpec voice;

        [Saved(Default.Color_White, false)]
        private Color color = Color.white;

        private Texture2D portrait;
    }
}