using System;
using UnityEngine;

namespace Ricave.Core
{
    public class MusicSpec : Spec, ISaveableEventsReceiver
    {
        public string Path
        {
            get
            {
                return this.path;
            }
        }

        public float Volume
        {
            get
            {
                return this.volume;
            }
        }

        public bool MainMenu
        {
            get
            {
                return this.mainMenu;
            }
        }

        public AudioClip AudioClip
        {
            get
            {
                return this.audioClip;
            }
        }

        public void OnLoaded()
        {
            if (!this.path.NullOrEmpty())
            {
                this.audioClip = Assets.Get<AudioClip>(this.path);
                return;
            }
            Log.Error("MusicSpec " + base.SpecID + " has no sound path.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string path;

        [Saved(1f, false)]
        private float volume = 1f;

        [Saved]
        public bool mainMenu;

        private AudioClip audioClip;
    }
}