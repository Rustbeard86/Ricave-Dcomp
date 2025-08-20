using System;

namespace Ricave.Core
{
    public interface ISaveableEventsReceiver
    {
        void OnLoaded();

        void OnSaved();
    }
}