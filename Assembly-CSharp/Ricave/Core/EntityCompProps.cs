using System;

namespace Ricave.Core
{
    public abstract class EntityCompProps
    {
        public Type CompClass
        {
            get
            {
                return this.compClass;
            }
        }

        [Saved(typeof(EntityComp), false)]
        private Type compClass = typeof(EntityComp);
    }
}