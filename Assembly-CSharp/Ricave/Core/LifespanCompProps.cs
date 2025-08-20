using System;

namespace Ricave.Core
{
    public class LifespanCompProps : EntityCompProps
    {
        public int LifespanTurns
        {
            get
            {
                return this.lifespanTurns;
            }
        }

        public SoundSpec ExpiredSound
        {
            get
            {
                return this.expiredSound;
            }
        }

        public bool ShowTurnsLeftAsStaticTextOverlay
        {
            get
            {
                return this.showTurnsLeftAsStaticTextOverlay;
            }
        }

        public string TurnsLeftStaticTextOverlayFormat
        {
            get
            {
                return this.turnsLeftStaticTextOverlayFormat;
            }
        }

        [Saved]
        private int lifespanTurns;

        [Saved]
        private SoundSpec expiredSound;

        [Saved]
        private bool showTurnsLeftAsStaticTextOverlay;

        [Saved]
        [Translatable]
        private string turnsLeftStaticTextOverlayFormat;
    }
}