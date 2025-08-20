using System;

namespace Ricave.Core
{
    public class FactionHostility
    {
        public Faction Faction1
        {
            get
            {
                return this.faction1;
            }
        }

        public Faction Faction2
        {
            get
            {
                return this.faction2;
            }
        }

        protected FactionHostility()
        {
        }

        public FactionHostility(Faction faction1, Faction faction2)
        {
            this.faction1 = faction1;
            this.faction2 = faction2;
        }

        [Saved]
        private Faction faction1;

        [Saved]
        private Faction faction2;
    }
}