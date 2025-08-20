using System;

namespace Ricave.Core
{
    public class SpawnerCompProps : EntityCompProps
    {
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public int IntervalTurns
        {
            get
            {
                return this.intervalTurns;
            }
        }

        public EntitySpec EntitySpec
        {
            get
            {
                return this.entitySpec;
            }
        }

        public bool ActiveByDefault
        {
            get
            {
                return this.activeByDefault;
            }
        }

        [Saved]
        private int count;

        [Saved]
        private int intervalTurns;

        [Saved]
        private EntitySpec entitySpec;

        [Saved(true, false)]
        private bool activeByDefault = true;
    }
}