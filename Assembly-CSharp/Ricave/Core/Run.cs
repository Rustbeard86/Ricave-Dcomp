using System;
using System.Collections.Generic;
using System.IO;
using Ricave.Rendering;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class Run
    {
        public RunInfo RunInfo
        {
            get
            {
                return this.runInfo;
            }
        }

        public RunConfig Config
        {
            get
            {
                return this.runInfo.Config;
            }
        }

        public RunSpec Spec
        {
            get
            {
                return this.Config.Spec;
            }
        }

        public Player Player
        {
            get
            {
                return this.player;
            }
        }

        public World World
        {
            get
            {
                return this.world;
            }
        }

        public TurnManager TurnManager
        {
            get
            {
                return this.turnManager;
            }
        }

        public IdentificationGroups IdentificationGroups
        {
            get
            {
                return this.identificationGroups;
            }
        }

        public PlayLog PlayLog
        {
            get
            {
                return this.playLog;
            }
        }

        public UniqueIDsManager UniqueIDsManager
        {
            get
            {
                return this.uniqueIDsManager;
            }
        }

        public PlaceManager PlaceManager
        {
            get
            {
                return this.placeManager;
            }
        }

        public FactionManager FactionManager
        {
            get
            {
                return this.factionManager;
            }
        }

        public UI UI
        {
            get
            {
                return this.ui;
            }
        }

        public MainSceneObjects MainSceneObjects
        {
            get
            {
                return this.mainSceneObjects;
            }
        }

        public CameraEffects CameraEffects
        {
            get
            {
                return this.cameraEffects;
            }
        }

        public DayNightCycleManager DayNightCycleManager
        {
            get
            {
                return this.dayNightCycleManager;
            }
        }

        public AsyncOperation UnloadUnusedAssetsAsyncOperation
        {
            get
            {
                return this.unloadUnusedAssetsAsyncOperation;
            }
        }

        public AssetsPrewarmer AssetsPrewarmer
        {
            get
            {
                return this.assetsPrewarmer;
            }
        }

        public void InitAsNew(RunConfig runConfig)
        {
            string[] array = new string[5];
            array[0] = "Starting new run with spec ";
            int num = 1;
            RunSpec spec = runConfig.Spec;
            array[num] = ((spec != null) ? spec.ToString() : null);
            array[2] = " and seed ";
            array[3] = runConfig.RunSeed.ToString();
            array[4] = ".";
            Log.Message(string.Concat(array));
            Rand.PushState(Calc.CombineHashes<int, int>(runConfig.RunSeed, 481644891));
            try
            {
                this.runInfo = new RunInfo(runConfig);
                this.identificationGroups.InitRandom();
                this.factionManager.GenerateInitialFactions();
                this.player.MakeActor();
                this.player.MakeChooseablePartyMemberActor();
                this.factionManager.ReserveFactionColors();
                PlacesGenerator.GeneratePlaces();
                this.world = new World();
                Get.CacheReferences();
                Place randomInitialPlace = Get.PlaceManager.GetRandomInitialPlace();
                int num2 = WorldGenUtility.CreateSeedForWorld(runConfig.RunSeed, randomInitialPlace);
                WorldGenMemory worldGenMemory = WorldGen.Generate(this.world, new WorldConfig(((randomInitialPlace != null) ? randomInitialPlace.Spec.WorldSpec : null) ?? runConfig.Spec.DefaultWorldSpec, num2, randomInitialPlace, null, null, false));
                this.player.SpawnPlayerPartyAndFollowers(worldGenMemory.playerStartPos, worldGenMemory.playerStartDir ?? Vector3IntUtility.Forward);
                this.player.DoInitialInstructions();
            }
            finally
            {
                Rand.PopState();
            }
            this.ui.Init();
            this.autosaver.Init();
            this.Save();
            Get.Progress.Save();
            Get.ModsEventsManager.CallEvent(ModEventType.NewRunStarted, null);
            FrameLocalPools.Clear();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true);
            GC.WaitForPendingFinalizers();
            this.ShowIntroOrWelcomeTexts(true, false);
            Log.ResetLogLimit();
        }

        public void InitAsLoadedFrom(string name)
        {
            Log.Message("Loading run from " + name);
            SaveLoadManager.Load(this, FilePaths.Savefile(name), "Run");
            this.Config.CheckUpdateSavefileName(name);
            this.ui.Init();
            this.autosaver.Init();
            Get.ModsEventsManager.CallEvent(ModEventType.RunLoaded, null);
            FrameLocalPools.Clear();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true);
            GC.WaitForPendingFinalizers();
            this.ShowIntroOrWelcomeTexts(false, true);
            Log.ResetLogLimit();
        }

        public void OnSceneReloaded()
        {
            if (this.onSceneChanged == null)
            {
                Log.Error("Scene changed but Run doesn't know what to do. Why did it change in the first place?", false);
                return;
            }
            this.mainSceneObjects.OnSceneChanged();
            this.cameraEffects.OnSceneChanged();
            this.ui.OnWorldAboutToRegenerate();
            Rand.PushState(Calc.CombineHashes<int, int, int>(this.runInfo.Config.RunSeed, this.turnManager.CurrentSequence, 53760912));
            try
            {
                if (this.world != null)
                {
                    this.world.Discard();
                }
                this.world = new World();
                Get.CacheReferences();
                WorldGenMemory worldGenMemory = WorldGen.Generate(this.world, this.onSceneChanged.WorldToGenerate);
                this.player.SavedCameraPosition.OnWorldGenerated();
                this.player.SpawnPlayerPartyAndFollowers(worldGenMemory.playerStartPos, worldGenMemory.playerStartDir ?? Vector3IntUtility.Forward);
                this.player.DoInitialInstructions();
                PlacesGenerator.GenerateNextPlaceIfNone();
                this.onSceneChanged = null;
            }
            finally
            {
                Rand.PopState();
            }
            this.ui.Init();
            this.autosaver.Init();
            this.Save();
            Get.Progress.Save();
            Get.ModsEventsManager.CallEvent(ModEventType.RunSceneReloaded, null);
            FrameLocalPools.Clear();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true);
            GC.WaitForPendingFinalizers();
            this.ShowIntroOrWelcomeTexts(false, false);
        }

        private void ShowIntroOrWelcomeTexts(bool newRun = false, bool loaded = false)
        {
            Get.UI.MemoryPieceHintUI.AllowedToShow = !loaded;
            if (Get.RunSpec == Get.Run_Tutorial)
            {
                if (newRun)
                {
                    Get.LessonManager.ResetLessons();
                    Get.TextSequenceDrawer.Show(TextSequence.Intro, null);
                    return;
                }
            }
            else if (Get.InLobby && Get.Progress.GetRunStats(Get.Run_Main1).Completed && !Get.RunInfo.TextAfterFirstDungeonCompletedShown && !Get.Progress.TextAfterFirstDungeonCompletedShown)
            {
                Get.TextSequenceDrawer.Show(TextSequence.AfterFirstDungeonCompleted, null);
                Get.RunInfo.TextAfterFirstDungeonCompletedShown = true;
                if (Get.InLobby)
                {
                    Get.Progress.TextAfterFirstDungeonCompletedShown = true;
                    return;
                }
            }
            else if (!Get.RunConfig.ProgressDisabled)
            {
                string text;
                if (Get.Floor == 1 && Get.PlaceSpec == Get.Place_Normal && !Get.RunInfo.TextOnFirstTimeFloorReached1Shown && !Get.Progress.TextOnFirstTimeFloorReached1Shown)
                {
                    text = "TextOnFirstTimeFloorReached_1".Translate();
                    Get.RunInfo.TextOnFirstTimeFloorReached1Shown = true;
                }
                else if (Get.Floor == 1 && Get.PlaceSpec == Get.Place_Normal && Get.Progress.Runs != 0 && !Get.RunInfo.TextOnFirstTimeFloorReached1Shown && !Get.RunInfo.TextOnFirstTimeFloorReached2Shown && !Get.Progress.TextOnFirstTimeFloorReached2Shown)
                {
                    text = "TextOnFirstTimeFloorReached_2".Translate();
                    Get.RunInfo.TextOnFirstTimeFloorReached2Shown = true;
                }
                else if (Get.Floor == 2 && !Get.RunInfo.TextOnFirstTimeFloorReached3Shown && !Get.Progress.TextOnFirstTimeFloorReached3Shown)
                {
                    text = "TextOnFirstTimeFloorReached_3".Translate();
                    Get.RunInfo.TextOnFirstTimeFloorReached3Shown = true;
                }
                else if (Get.PlaceSpec == Get.Place_GoogonsLair && !Get.RunInfo.TextOnFirstTimeFloorReached4Shown && !Get.Progress.TextOnFirstTimeFloorReached4Shown)
                {
                    text = "TextOnFirstTimeFloorReached_4".Translate();
                    Get.RunInfo.TextOnFirstTimeFloorReached4Shown = true;
                }
                else if (Get.Floor == 5 && Get.PlaceSpec == Get.Place_Shelter && !Get.RunInfo.TextOnFirstTimeFloorReached5Shown && !Get.Progress.TextOnFirstTimeFloorReached5Shown)
                {
                    text = "TextOnFirstTimeFloorReached_5".Translate();
                    Get.RunInfo.TextOnFirstTimeFloorReached5Shown = true;
                }
                else if (Get.PlaceSpec == Get.Place_DemonsLair && !Get.RunInfo.TextOnFirstTimeFloorReached6Shown && !Get.Progress.TextOnFirstTimeFloorReached6Shown)
                {
                    text = "TextOnFirstTimeFloorReached_6".Translate();
                    Get.RunInfo.TextOnFirstTimeFloorReached6Shown = true;
                }
                else if (Get.Floor == 10 && !Get.RunInfo.TextOnFirstTimeFloorReached7Shown && !Get.Progress.TextOnFirstTimeFloorReached7Shown)
                {
                    text = "TextOnFirstTimeFloorReached_7".Translate();
                    Get.RunInfo.TextOnFirstTimeFloorReached7Shown = true;
                }
                else
                {
                    text = null;
                }
                if (text != null)
                {
                    Get.TextSequenceDrawer.Show(TextSequence.SingleText, text);
                    return;
                }
                this.ui.ShowWelcomeTexts();
                return;
            }
            else
            {
                this.ui.ShowWelcomeTexts();
            }
        }

        public void ReloadScene(RunOnSceneChanged onSceneChanged, bool followersCanFollow)
        {
            if (onSceneChanged == null)
            {
                Log.Error("Tried to reload scene using null RunOnSceneChanged.", false);
                return;
            }
            if (this.onSceneChanged != null)
            {
                Log.Error("Can't set new on scene changed action because one is already queued.", false);
                return;
            }
            Rand.PushState(Calc.CombineHashes<int, int, int>(this.runInfo.Config.RunSeed, this.turnManager.CurrentSequence, 267435442));
            try
            {
                this.player.DeSpawnPlayerPartyAndFollowers(followersCanFollow);
            }
            finally
            {
                Rand.PopState();
            }
            this.onSceneChanged = onSceneChanged;
            Root.LoadPlayScene(RootOnSceneChanged.Nothing());
        }

        public void Save()
        {
            string text = FilePaths.Savefile(this.Config.SavefileName);
            SaveLoadManager.Save(this, text, "Run");
            Get.ModsEventsManager.CallEvent(ModEventType.RunSaved, null);
        }

        public void DeleteSavefile()
        {
            try
            {
                string text = FilePaths.Savefile(this.Config.SavefileName);
                if (File.Exists(text))
                {
                    File.Delete(text);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while deleting savefile.", ex);
            }
        }

        public void Update()
        {
            try
            {
                Profiler.Begin("assetsPrewarmer.Update()");
                this.assetsPrewarmer.Update();
            }
            catch (Exception ex)
            {
                Log.Error("Error in assetsPrewarmer.Update().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("runInfo.Update()");
                this.runInfo.Update();
            }
            catch (Exception ex2)
            {
                Log.Error("Error in runInfo.Update().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("turnManager.Update()");
                this.turnManager.Update();
            }
            catch (Exception ex3)
            {
                Log.Error("Error in turnManager.Update().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("world.Update()");
                this.world.Update();
            }
            catch (Exception ex4)
            {
                Log.Error("Error in world.Update().", ex4);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("player.Update()");
                this.player.Update();
            }
            catch (Exception ex5)
            {
                Log.Error("Error in player.Update().", ex5);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("cameraEffects.Update()");
                this.cameraEffects.Update();
            }
            catch (Exception ex6)
            {
                Log.Error("Error in cameraEffects.Update().", ex6);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("dayNightCycleManager.Update()");
                this.dayNightCycleManager.Update();
            }
            catch (Exception ex7)
            {
                Log.Error("Error in dayNightCycleManager.Update().", ex7);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("UnlockableManager.Update()");
                Get.UnlockableManager.Update();
            }
            catch (Exception ex8)
            {
                Log.Error("Error in UnlockableManager.Update().", ex8);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("ui.Update()");
                this.ui.Update();
            }
            catch (Exception ex9)
            {
                Log.Error("Error during UI update.", ex9);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("autosaver.Update()");
                this.autosaver.Update();
            }
            catch (Exception ex10)
            {
                Log.Error("Error during autosaver update.", ex10);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void FixedUpdate()
        {
            try
            {
                Profiler.Begin("world.FixedUpdate()");
                this.world.FixedUpdate();
            }
            catch (Exception ex)
            {
                Log.Error("Error in world.FixedUpdate().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("cameraEffects.FixedUpdate()");
                this.cameraEffects.FixedUpdate();
            }
            catch (Exception ex2)
            {
                Log.Error("Error in cameraEffects.FixedUpdate().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("playLog.FixedUpdate()");
                this.playLog.FixedUpdate();
            }
            catch (Exception ex3)
            {
                Log.Error("Error in playLog.FixedUpdate().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("ui.FixedUpdate()");
                this.ui.FixedUpdate();
            }
            catch (Exception ex4)
            {
                Log.Error("Error during UI fixed update.", ex4);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void OnGUI()
        {
            try
            {
                Profiler.Begin("TextSequenceDrawer.HandleGUIEvents()");
                Get.TextSequenceDrawer.HandleGUIEvents();
            }
            catch (Exception ex)
            {
                Log.Error("Error in TextSequenceDrawer.HandleGUIEvents().", ex);
            }
            finally
            {
                Profiler.End();
            }
            if (!Get.TextSequenceDrawer.Showing && !Get.DungeonMapDrawer.Showing)
            {
                try
                {
                    Profiler.Begin("FancyOutlineGOC.DrawOnGUI()");
                    Get.FancyOutlineGOC.DrawOnGUI();
                }
                catch (Exception ex2)
                {
                    Log.Error("Error in FancyOutlineGOC.DrawOnGUI().", ex2);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("ui.EarlyOnGUI()");
                    this.ui.EarlyOnGUI();
                }
                catch (Exception ex3)
                {
                    Log.Error("Error during UI EarlyOnGUI().", ex3);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("cameraEffects.OnGUI()");
                    this.cameraEffects.OnGUI();
                }
                catch (Exception ex4)
                {
                    Log.Error("Error in cameraEffects.OnGUI().", ex4);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("FPSCounter.OnGUI()");
                    FPSCounter.OnGUI();
                }
                catch (Exception ex5)
                {
                    Log.Error("Error in FPSCounter.OnGUI().", ex5);
                }
                finally
                {
                    Profiler.End();
                }
                Get.ModsEventsManager.CallEvent(ModEventType.RunOnGUIEarly, null);
                try
                {
                    Profiler.Begin("player.OnGUI()");
                    this.player.OnGUI();
                }
                catch (Exception ex6)
                {
                    Log.Error("Error in player.OnGUI().", ex6);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("playLog.OnGUI()");
                    this.playLog.OnGUI();
                }
                catch (Exception ex7)
                {
                    Log.Error("Error in playLog.OnGUI().", ex7);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("ui.OnGUI()");
                    this.ui.OnGUI();
                }
                catch (Exception ex8)
                {
                    Log.Error("Error during UI OnGUI().", ex8);
                }
                finally
                {
                    Profiler.End();
                }
                Get.ModsEventsManager.CallEvent(ModEventType.RunOnGUILate, null);
                try
                {
                    Profiler.Begin("player.OnGUIAfterUI()");
                    this.player.OnGUIAfterUI();
                }
                catch (Exception ex9)
                {
                    Log.Error("Error in player.OnGUIAfterUI().", ex9);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("windowManager.LateOnGUI()");
                    Get.WindowManager.LateOnGUI();
                    return;
                }
                catch (Exception ex10)
                {
                    Log.Error("Error in windowManager.LateOnGUI().", ex10);
                    return;
                }
                finally
                {
                    Profiler.End();
                }
            }
            try
            {
                Profiler.Begin("tooltips.OnGUI()");
                Get.Tooltips.OnGUI();
            }
            catch (Exception ex11)
            {
                Log.Error("Error in tooltips.OnGUI().", ex11);
            }
            finally
            {
                Profiler.End();
            }
        }

        public T GetOrCreateModState<T>(string modId) where T : ModStatePerRun, new()
        {
            for (int i = 0; i < this.modsState.Count; i++)
            {
                if (this.modsState[i].ModId == modId)
                {
                    T t = this.modsState[i] as T;
                    if (t != null)
                    {
                        return t;
                    }
                }
            }
            T t2 = new T();
            t2.ModId = modId;
            this.modsState.Add(t2);
            return t2;
        }

        [Saved]
        private RunInfo runInfo;

        [Saved(Default.New, false)]
        private FactionManager factionManager = new FactionManager();

        [Saved(Default.New, false)]
        private Player player = new Player();

        [Saved(Default.New, false)]
        private PlaceManager placeManager = new PlaceManager();

        [Saved(Default.New, false)]
        private World world;

        [Saved(Default.New, false)]
        private TurnManager turnManager = new TurnManager();

        [Saved(Default.New, false)]
        private IdentificationGroups identificationGroups = new IdentificationGroups();

        [Saved(Default.New, false)]
        private PlayLog playLog = new PlayLog();

        [Saved(Default.New, false)]
        private UniqueIDsManager uniqueIDsManager = new UniqueIDsManager();

        [Saved(Default.New, true)]
        private List<ModStatePerRun> modsState = new List<ModStatePerRun>();

        private UI ui = new UI();

        private MainSceneObjects mainSceneObjects = new MainSceneObjects();

        private CameraEffects cameraEffects = new CameraEffects();

        private Autosaver autosaver = new Autosaver();

        private AssetsPrewarmer assetsPrewarmer = new AssetsPrewarmer();

        private DayNightCycleManager dayNightCycleManager = new DayNightCycleManager();

        private RunOnSceneChanged onSceneChanged;

        private AsyncOperation unloadUnusedAssetsAsyncOperation;
    }
}