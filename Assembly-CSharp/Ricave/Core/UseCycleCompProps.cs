using System;

namespace Ricave.Core
{
    public class UseCycleCompProps : EntityCompProps
    {
        public int IntervalTurns
        {
            get
            {
                return this.intervalTurns;
            }
        }

        [Saved]
        private int intervalTurns;
    }
}