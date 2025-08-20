using System;
using Ricave.Core;

namespace Ricave.Rendering
{
    public class EtherealGOC : EntityGOC
    {
        public Ethereal Ethereal
        {
            get
            {
                return (Ethereal)base.Entity;
            }
        }

        public override void OnEntitySpawned()
        {
            base.OnEntitySpawned();
            if (!(base.Entity is Ethereal))
            {
                Log.Error("EtherealGOC spawned with a non-Ethereal entity.", false);
            }
        }
    }
}