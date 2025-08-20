using System;

namespace Ricave.Core
{
    public abstract class GenPass
    {
        public abstract int SeedPart { get; }

        public abstract void DoPass(WorldGenMemory memory);
    }
}