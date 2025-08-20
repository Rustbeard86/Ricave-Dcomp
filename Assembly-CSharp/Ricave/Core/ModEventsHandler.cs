using System;

namespace Ricave.Core
{
    public class ModEventsHandler
    {
        public Mod Mod
        {
            get
            {
                return this.mod;
            }
        }

        public string ModId
        {
            get
            {
                return this.mod.ModId;
            }
        }

        public ModEventsHandler(Mod mod)
        {
            this.mod = mod;
        }

        public T GetCurrentProgressState<T>() where T : ModStatePerProgress, new()
        {
            T t = this.cachedProgressState as T;
            if (t != null && Get.Progress == this.cachedProgressStateForProgress)
            {
                return t;
            }
            T orCreateModState = Get.Progress.GetOrCreateModState<T>(this.mod.ModId);
            this.cachedProgressState = orCreateModState;
            this.cachedProgressStateForProgress = Get.Progress;
            return orCreateModState;
        }

        public T GetCurrentRunState<T>() where T : ModStatePerRun, new()
        {
            T t = this.cachedRunState as T;
            if (t != null && Get.Run == this.cachedRunStateForRun)
            {
                return t;
            }
            T orCreateModState = Get.Run.GetOrCreateModState<T>(this.mod.ModId);
            this.cachedRunState = orCreateModState;
            this.cachedRunStateForRun = Get.Run;
            return orCreateModState;
        }

        public T GetCurrentWorldState<T>() where T : ModStatePerWorld, new()
        {
            T t = this.cachedWorldState as T;
            if (t != null && Get.World == this.cachedWorldStateForWorld)
            {
                return t;
            }
            T orCreateModState = Get.World.GetOrCreateModState<T>(this.mod.ModId);
            this.cachedWorldState = orCreateModState;
            this.cachedWorldStateForWorld = Get.World;
            return orCreateModState;
        }

        public virtual void Initialize()
        {
        }

        public virtual void OnEnabled()
        {
        }

        public virtual void OnDisabled()
        {
        }

        public virtual void SubscribeToEvents(ModsEventsManager eventsManager)
        {
        }

        private Mod mod;

        private ModStatePerProgress cachedProgressState;

        private Progress cachedProgressStateForProgress;

        private ModStatePerRun cachedRunState;

        private Run cachedRunStateForRun;

        private ModStatePerWorld cachedWorldState;

        private World cachedWorldStateForWorld;
    }
}