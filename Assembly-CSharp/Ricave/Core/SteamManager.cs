using System;
using System.Text;
using AOT;
using Ricave.Rendering;
using Ricave.UI;
using Steamworks;
using UnityEngine;

namespace Ricave.Core
{
    [DisallowMultipleComponent]
    public class SteamManager : MonoBehaviour
    {
        private static SteamManager Instance
        {
            get
            {
                if (SteamManager.instance == null)
                {
                    return new GameObject("SteamManager").AddComponent<SteamManager>();
                }
                return SteamManager.instance;
            }
        }

        public static bool Initialized
        {
            get
            {
                return SteamManager.Instance.initialized;
            }
        }

        public static bool InitializedAndNotShuttingDown
        {
            get
            {
                return SteamManager.Initialized && !SteamManager.shuttingDown;
            }
        }

        [MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
        protected static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
        {
            Log.Warning(pchDebugText.ToString(), false);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitOnPlayMode()
        {
            SteamManager.everInitialized = false;
            SteamManager.instance = null;
        }

        protected virtual void Awake()
        {
            if (SteamManager.instance != null)
            {
                Object.Destroy(base.gameObject);
                return;
            }
            SteamManager.instance = this;
            if (SteamManager.everInitialized)
            {
                throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
            }
            Object.DontDestroyOnLoad(base.gameObject);
            if (!Packsize.Test())
            {
                Log.Error("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
            }
            if (!DllCheck.Test())
            {
                Log.Error("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
            }
            try
            {
                if (!Application.isBatchMode && SteamAPI.RestartAppIfNecessary(new AppId_t(App.IsDemo ? 3293980U : 2358360U)))
                {
                    Application.Quit();
                    return;
                }
            }
            catch (DllNotFoundException ex)
            {
                string text = "[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n";
                DllNotFoundException ex2 = ex;
                Log.Error(text + ((ex2 != null) ? ex2.ToString() : null), this);
                App.Quit();
                return;
            }
            this.initialized = SteamAPI.Init();
            if (!this.initialized)
            {
                Log.Error("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
                return;
            }
            SteamManager.everInitialized = true;
            SteamWorkshop.Init();
            SteamInput.Init(false);
            SteamDeckUtility.Init();
            SteamKeyboardUtility.Init();
            SteamUserStats.RequestCurrentStats();
        }

        protected virtual void OnEnable()
        {
            if (SteamManager.instance == null)
            {
                SteamManager.instance = this;
            }
            if (!this.initialized)
            {
                return;
            }
            if (this.m_SteamAPIWarningMessageHook == null)
            {
                this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamAPIDebugTextHook);
                SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
            }
        }

        protected virtual void OnDestroy()
        {
            if (SteamManager.instance != this)
            {
                return;
            }
            SteamManager.instance = null;
            if (!this.initialized)
            {
                return;
            }
            SteamInput.Shutdown();
            SteamDeckUtility.Shutdown();
            SteamAPI.Shutdown();
        }

        protected virtual void Update()
        {
            if (!this.initialized || SteamManager.shuttingDown)
            {
                return;
            }
            SteamAPI.RunCallbacks();
        }

        public static void OnShuttingDown()
        {
            SteamManager.shuttingDown = true;
        }

        private static bool everInitialized;

        private static bool shuttingDown;

        private static SteamManager instance;

        protected bool initialized;

        protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
    }
}