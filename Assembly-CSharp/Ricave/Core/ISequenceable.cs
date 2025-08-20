using System;

namespace Ricave.Core
{
    public interface ISequenceable
    {
        int Sequence { get; set; }

        void TakeTurn();
    }
}