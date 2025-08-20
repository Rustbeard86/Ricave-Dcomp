using System;
using Ricave.Rendering;

namespace Ricave.Core
{
    public class Ethereal : Entity
    {
        public EtherealGOC EtherealGOC
        {
            get
            {
                return (EtherealGOC)base.EntityGOC;
            }
        }

        protected Ethereal()
        {
        }

        public Ethereal(EntitySpec spec)
            : base(spec)
        {
        }
    }
}