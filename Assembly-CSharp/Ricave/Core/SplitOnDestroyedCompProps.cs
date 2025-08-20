using System;

namespace Ricave.Core
{
    public class SplitOnDestroyedCompProps : EntityCompProps
    {
        public EntitySpec ActorSpec
        {
            get
            {
                return this.actorSpec;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        [Saved]
        private EntitySpec actorSpec;

        [Saved]
        private int count;
    }
}