using System;

namespace Ricave.Core
{
    public class FireSpreaderCompProps : EntityCompProps
    {
        public int SpreadAfterTurns
        {
            get
            {
                return this.spreadAfterTurns;
            }
        }

        [Saved]
        private int spreadAfterTurns;
    }
}