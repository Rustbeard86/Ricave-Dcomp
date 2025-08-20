using System;

namespace Ricave.Core
{
    public enum ModEventType
    {
        ContentReloaded,

        Update,

        FixedUpdate,

        RunOnGUIEarly,

        RunOnGUILate,

        MainMenuOnGUIEarly,

        MainMenuOnGUILate,

        SceneChanged,

        NewRunStarted,

        RunLoaded,

        RunSceneReloaded,

        RunSaved,

        EntitySpawned,

        EntityDeSpawned,

        EntityMoved,

        CellsNoLongerSeen,

        CellsNewlySeen,

        EntitiesNoLongerSeen,

        EntitiesNewlySeen,

        FoggedCells,

        UnfoggedCells,

        WorldDiscarded,

        WorldGenerated
    }
}