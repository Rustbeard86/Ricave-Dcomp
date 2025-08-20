using System;

namespace Ricave.Core
{
    public class ProximityTriggerCompProps : EntityCompProps
    {
        public int Turns
        {
            get
            {
                return this.turns;
            }
        }

        [Saved]
        private int turns;
    }
}