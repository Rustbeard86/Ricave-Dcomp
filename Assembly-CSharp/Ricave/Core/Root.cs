using System;
using System.Globalization;
using System.Threading;
using Ricave.Rendering;
using Ricave.UI;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

namespace Ricave.Core
{
    public static class Root
    {
        public static Run Run
        {
            get
            {
                return Root.run;
            }
        }

        public static Progress Progress
        {
            get
            {
                return Root.progress;
            }
        }

        public static Audio Audio
        {
            get
            {
                return Root.audio;
            }
        }

        public static Specs Specs
        {
            get
            {
                return Root.specs;
            }
        }

        public static Languages Languages
        {
            get
            {
                return Root.languages;
            }
        }

        public static Layers Layers
        {
            get
            {
                return Root.layers;
            }
        }

        public static ShaderPropertyIDs ShaderPropertyIDs
        {
            get
            {
                return Root.shaderPropertyIDs;
            }
        }

        public static MainMenuUI MainMenuUI
        {
            get
            {
                return Root.mainMenuUI;
            }
        }

        public static ModManager ModManager
        {
            get
            {
                return Root.modManager;
            }
        }

        public static bool InMainMenu
        {
            get
            {
                return Root.inMainMenu;
            }
        }

        public static bool ChangingScene
        {
            get
            {
                return Root.onSceneChanged != null;
            }
        }

        public static int MainThreadId
        {
            get
            {
                return Root.mainThreadId;
            }
        }

        public static float LastFixedUpdateTime
        {
            get
            {
                return Root.lastFixedUpdateTime;
            }
        }

        public static float UnscaledTimeRecalculatedForTrailerMode
        {
            get
            {
                return Root.unscaledTimeRecalculatedForTrailerMode;
            }
        }

        public static void OnSceneChanged()
        {
            Profiler.Begin("Root.OnSceneChanged()");
            try
            {
                Root.inMainMenu = SceneManager.GetActiveScene().name == "MainMenu";
                Clock.TimeScale = 1f;
                if (!Root.staticInitDone)
                {
                    Root.staticInitDone = true;
                    Root.StaticInit();
                    try
                    {
                        if (BatchModeHandler.CheckAndHandleBatchMode())
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error while handling batch mode commands. Quitting.", ex);
                        Root.Quit();
                        return;
                    }
                }
                PrefsHelper.Apply();
                CachedGUI.Clear();
                Root.audio.OnSceneChanged();
                Root.CreateOrNotifyRunOfSceneChanged();
                Get.ModsEventsManager.CallEvent(ModEventType.SceneChanged, null);
                FrameLocalPools.Clear();
                VisibilityCache.ResetStaticDataOnSceneChanged();
                FogOfWar.ResetStaticDataOnSceneChanged();
                if (!Root.inMainMenu)
                {
                    MainMenuUI.UnloadMenuBackground();
                }
            }
            catch (Exception ex2)
            {
                if (!Root.inMainMenu)
                {
                    Log.Error("Error in Root.OnSceneChanged(). Will try to go back to the main menu.", ex2);
                    Root.onSceneChanged = null;
                    Root.LoadMainMenuScene();
                }
                else
                {
                    Log.Error("Error in Root.OnSceneChanged(). Quitting the application since an error happened in the main menu, so there's nothing we can do.", ex2);
                    Root.Quit();
                }
            }
            finally
            {
                Profiler.End();
            }
        }

        private static void CreateOrNotifyRunOfSceneChanged()
        {
            if (Root.InMainMenu)
            {
                Root.run = null;
                Get.CacheReferences();
                Root.mainMenuUI = new MainMenuUI();
                Root.mainMenuUI.Init();
            }
            else
            {
                Root.mainMenuUI = null;
                Get.CacheReferences();
                if (Root.onSceneChanged == null && Root.run == null)
                {
                    Root.run = new Run();
                    Get.CacheReferences();
                    Root.run.InitAsNew(new RunConfig(Get.Run_Main1, Rand.Int, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null));
                }
                else if (Root.onSceneChanged != null && !Root.onSceneChanged.RunToLoad.NullOrEmpty())
                {
                    Root.run = new Run();
                    Get.CacheReferences();
                    Root.run.InitAsLoadedFrom(Root.onSceneChanged.RunToLoad);
                }
                else if (Root.onSceneChanged != null && Root.onSceneChanged.RunToStart != null)
                {
                    Root.run = new Run();
                    Get.CacheReferences();
                    Root.run.InitAsNew(Root.onSceneChanged.RunToStart);
                }
                else if (Root.run != null)
                {
                    Root.run.OnSceneReloaded();
                }
                else
                {
                    Log.Error("Unknown onSceneChanged state.", false);
                }
            }
            Root.onSceneChanged = null;
        }

        public static void LoadPlayScene(RootOnSceneChanged onSceneChanged)
        {
            if (onSceneChanged == null)
            {
                Log.Error("Tried to load play scene using null RootOnSceneChanged.", false);
                return;
            }
            if (Root.ChangingScene)
            {
                Log.Error("Can't load play scene because another scene loading is already in progress.", false);
                return;
            }
            Root.onSceneChanged = onSceneChanged;
            SceneManager.LoadScene("Play");
        }

        public static void LoadMainMenuScene()
        {
            if (Root.ChangingScene)
            {
                Log.Error("Can't load main menu scene because another scene loading is already in progress.", false);
                return;
            }
            Root.onSceneChanged = RootOnSceneChanged.Nothing();
            SceneManager.LoadScene("MainMenu");
        }

        public static void Update()
        {
            Profiler.Begin("Root.Update()");
            try
            {
                Root.unscaledTimeRecalculatedForTrailerMode += ((Time.timeScale == 0f) ? Time.unscaledDeltaTime : (Time.deltaTime / Time.timeScale));
                try
                {
                    Profiler.Begin("FrameLocalPools.OnNewFrame()");
                    FrameLocalPools.OnNewFrame();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in FrameLocalPools.OnNewFrame().", ex);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("Rand.EnsureRandStateStackEmpty()");
                    Rand.EnsureRandStateStackEmpty();
                }
                catch (Exception ex2)
                {
                    Log.Error("Error in Rand.EnsureRandStateStackEmpty().", ex2);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("RenderTexturePool.Update()");
                    RenderTexturePool.Update();
                }
                catch (Exception ex3)
                {
                    Log.Error("Error in RenderTexturePool.Update().", ex3);
                }
                finally
                {
                    Profiler.End();
                }
                if (Root.run != null)
                {
                    try
                    {
                        Profiler.Begin("run.Update()");
                        Root.run.Update();
                    }
                    catch (Exception ex4)
                    {
                        Log.Error("Error during run update.", ex4);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                }
                if (Root.mainMenuUI != null)
                {
                    try
                    {
                        Profiler.Begin("mainMenuUI.Update()");
                        Root.mainMenuUI.Update();
                    }
                    catch (Exception ex5)
                    {
                        Log.Error("Error during main menu update.", ex5);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                }
                try
                {
                    Profiler.Begin("audio.Update()");
                    Root.audio.Update();
                }
                catch (Exception ex6)
                {
                    Log.Error("Error during audio update.", ex6);
                }
                finally
                {
                    Profiler.End();
                }
                Get.ModsEventsManager.CallEvent(ModEventType.Update, null);
                App.HideDeveloperConsole();
            }
            catch (Exception ex7)
            {
                Log.Error("Error in Root.Update().", ex7);
            }
            finally
            {
                Profiler.End();
            }
        }

        public static void LateUpdate()
        {
            try
            {
                Profiler.Begin("CubemapMaker.LateUpdate()");
                CubemapMaker.LateUpdate();
            }
            catch (Exception ex)
            {
                Log.Error("Error in CubemapMaker.LateUpdate.", ex);
            }
            finally
            {
                Profiler.End();
            }
        }

        public static void FixedUpdate()
        {
            Profiler.Begin("Root.FixedUpdate()");
            try
            {
                Root.lastFixedUpdateTime = Clock.Time;
                if (Root.run != null)
                {
                    try
                    {
                        Profiler.Begin("run.FixedUpdate()");
                        Root.run.FixedUpdate();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error during run fixed update.", ex);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                }
                if (Root.mainMenuUI != null)
                {
                    try
                    {
                        Profiler.Begin("mainMenuUI.FixedUpdate()");
                        Root.mainMenuUI.FixedUpdate();
                    }
                    catch (Exception ex2)
                    {
                        Log.Error("Error during main menu fixed update.", ex2);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                }
                Get.ModsEventsManager.CallEvent(ModEventType.FixedUpdate, null);
            }
            catch (Exception ex3)
            {
                Log.Error("Error in Root.FixedUpdate().", ex3);
            }
            finally
            {
                Profiler.End();
            }
        }

        public static void OnGUI()
        {
            Profiler.Begin("Root.OnGUI()");
            try
            {
                try
                {
                    Profiler.Begin("Widgets.OnGUI()");
                    Widgets.OnGUI();
                }
                catch (Exception ex)
                {
                    Log.Error("Error while setting Widgets skin.", ex);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("GUIExtra.OnGUI()");
                    GUIExtra.OnGUI();
                }
                catch (Exception ex2)
                {
                    Log.Error("Error in GUIExtra.OnGUI().", ex2);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("CachedGUI.OnGUI()");
                    CachedGUI.OnGUI();
                }
                catch (Exception ex3)
                {
                    Log.Error("Error in CachedGUI.OnGUI().", ex3);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("ControllerUtility.OnGUI()");
                    ControllerUtility.OnGUI();
                }
                catch (Exception ex4)
                {
                    Log.Error("Error in ControllerUtility.OnGUI().", ex4);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("SteamDeckUtility.OnGUI()");
                    SteamDeckUtility.OnGUI();
                }
                catch (Exception ex5)
                {
                    Log.Error("Error in SteamDeckUtility.OnGUI().", ex5);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("SteamKeyboardUtility.OnGUI()");
                    SteamKeyboardUtility.OnGUI();
                }
                catch (Exception ex6)
                {
                    Log.Error("Error in SteamKeyboardUtility.OnGUI().", ex6);
                }
                finally
                {
                    Profiler.End();
                }
                if (!DebugUI.HideUI || Event.current.type != EventType.Repaint)
                {
                    if (Root.run != null)
                    {
                        try
                        {
                            Profiler.Begin("run.OnGUI()");
                            Root.run.OnGUI();
                        }
                        catch (Exception ex7)
                        {
                            Log.Error("Error during run OnGUI().", ex7);
                        }
                        finally
                        {
                            Profiler.End();
                        }
                    }
                    if (Root.mainMenuUI != null)
                    {
                        try
                        {
                            Profiler.Begin("mainMenuUI.OnGUI()");
                            Root.mainMenuUI.OnGUI();
                        }
                        catch (Exception ex8)
                        {
                            Log.Error("Error during main menu OnGUI().", ex8);
                        }
                        finally
                        {
                            Profiler.End();
                        }
                    }
                }
                Get.RootGOC.useGUILayout = Get.WindowManager.AnyWindowOpen || Get.DevConsole.IsOnOrStillVisible;
            }
            catch (Exception ex9)
            {
                Log.Error("Error in Root.OnGUI().", ex9);
            }
            finally
            {
                Profiler.End();
            }
        }

        private static void StaticInit()
        {
            Profiler.Begin("Root.StaticInit()");
            try
            {
                Root.mainThreadId = Thread.CurrentThread.ManagedThreadId;
                Log.RegisterGlobalLogHandler();
                Log.Message("Root init.");
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                Xorshift128.InitStateWithSeed((int)(DateTime.Now.Ticks % 2147483647L));
                Get.CacheReferences();
                FilePaths.Init();
                PrimitiveMeshes.Init();
                ShatterMeshes.Init();
                Root.layers.Init();
                Root.shaderPropertyIDs.Init();
                Root.modManager.LoadAll();
                Root.modManager.ReloadAllContent();
                Root.audio.MusicManager.Init();
                TypeUtility.PreJITMethods(typeof(Entity).Assembly);
            }
            catch (Exception ex)
            {
                Log.Error("Error in Root.StaticInit(). Quitting the application.", ex);
                Root.Quit();
            }
            finally
            {
                Profiler.End();
            }
        }

        public static void LoadProgress()
        {
            Root.progress = new Progress();
            Root.progress.TryLoad();
        }

        public static void Quit()
        {
            try
            {
                SteamManager.OnShuttingDown();
                SteamAPI.Shutdown();
            }
            catch (Exception)
            {
            }
            Application.Quit();
        }

        private static bool staticInitDone;

        private static RootOnSceneChanged onSceneChanged;

        private static bool inMainMenu;

        private static int mainThreadId;

        private static float lastFixedUpdateTime;

        private static float unscaledTimeRecalculatedForTrailerMode;

        private static Run run;

        private static MainMenuUI mainMenuUI;

        private static Progress progress;

        private static Audio audio = new Audio();

        private static Specs specs = new Specs();

        private static Languages languages = new Languages();

        private static Layers layers = new Layers();

        private static ShaderPropertyIDs shaderPropertyIDs = new ShaderPropertyIDs();

        private static ModManager modManager = new ModManager();
    }
}