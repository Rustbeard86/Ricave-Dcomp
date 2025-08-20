using System;

namespace Ricave.Core
{
    public class RetractableCompProps : EntityCompProps
    {
        public int TurnsRetracted
        {
            get
            {
                return this.turnsRetracted;
            }
        }

        public int TurnsNotRetracted
        {
            get
            {
                return this.turnsNotRetracted;
            }
        }

        public SoundSpec AppearSound
        {
            get
            {
                return this.appearSound;
            }
        }

        public SoundSpec RetractSound
        {
            get
            {
                return this.retractSound;
            }
        }

        [Saved]
        private int turnsRetracted;

        [Saved]
        private int turnsNotRetracted;

        [Saved]
        private SoundSpec appearSound;

        [Saved]
        private SoundSpec retractSound;
    }
}