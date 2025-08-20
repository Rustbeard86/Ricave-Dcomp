using System;
using System.Collections.Generic;
using Ricave.Rendering;
using Ricave.UI;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Ricave.Core
{
    public static class Get
    {
        public static bool InMainMenu
        {
            get
            {
                return Root.InMainMenu;
            }
        }

        public static bool InLobby
        {
            get
            {
                return Get.Run != null && Get.RunSpec.IsLobby;
            }
        }

        public static Run Run
        {
            get
            {
                return Root.Run;
            }
        }

        public static Progress Progress
        {
            get
            {
                return Root.Progress;
            }
        }

        public static Audio Audio
        {
            get
            {
                return Root.Audio;
            }
        }

        public static Specs Specs
        {
            get
            {
                return Root.Specs;
            }
        }

        public static Languages Languages
        {
            get
            {
                return Root.Languages;
            }
        }

        public static Layers Layers
        {
            get
            {
                return Root.Layers;
            }
        }

        public static ShaderPropertyIDs ShaderPropertyIDs
        {
            get
            {
                return Root.ShaderPropertyIDs;
            }
        }

        public static MainMenuUI MainMenuUI
        {
            get
            {
                return Root.MainMenuUI;
            }
        }

        public static ModManager ModManager
        {
            get
            {
                return Root.ModManager;
            }
        }

        public static ModsEventsManager ModsEventsManager
        {
            get
            {
                return Get.ModManager.ModsEventsManager;
            }
        }

        public static OneShotSounds OneShotSounds
        {
            get
            {
                return Get.Audio.OneShots;
            }
        }

        public static SustainingSounds SustainingSounds
        {
            get
            {
                return Get.Audio.Sustaining;
            }
        }

        public static MusicManager MusicManager
        {
            get
            {
                return Get.Audio.MusicManager;
            }
        }

        public static AmbientSoundsManager AmbientSoundsManager
        {
            get
            {
                return Get.Audio.AmbientSoundsManager;
            }
        }

        public static QuestManager QuestManager
        {
            get
            {
                return Get.Progress.QuestManager;
            }
        }

        public static UnlockableManager UnlockableManager
        {
            get
            {
                return Get.Progress.UnlockableManager;
            }
        }

        public static TraitManager TraitManager
        {
            get
            {
                return Get.Progress.TraitManager;
            }
        }

        public static CosmeticsManager CosmeticsManager
        {
            get
            {
                return Get.Progress.CosmeticsManager;
            }
        }

        public static PrivateRoom PrivateRoom
        {
            get
            {
                return Get.Progress.PrivateRoom;
            }
        }

        public static DialoguesManager DialoguesManager
        {
            get
            {
                return Get.Progress.DialoguesManager;
            }
        }

        public static TotalKillCounter TotalKillCounter
        {
            get
            {
                return Get.Progress.TotalKillCounter;
            }
        }

        public static TotalLobbyItems TotalLobbyItems
        {
            get
            {
                return Get.Progress.TotalLobbyItems;
            }
        }

        public static TotalQuestsState TotalQuestsState
        {
            get
            {
                return Get.Progress.TotalQuestsState;
            }
        }

        public static LessonManager LessonManager
        {
            get
            {
                return Get.Progress.LessonManager;
            }
        }

        public static Player Player
        {
            get
            {
                return Get.cachedPlayer;
            }
        }

        public static World World
        {
            get
            {
                return Get.cachedWorld;
            }
        }

        public static TurnManager TurnManager
        {
            get
            {
                return Get.Run.TurnManager;
            }
        }

        public static IdentificationGroups IdentificationGroups
        {
            get
            {
                return Get.Run.IdentificationGroups;
            }
        }

        public static PlayLog PlayLog
        {
            get
            {
                return Get.Run.PlayLog;
            }
        }

        public static UniqueIDsManager UniqueIDsManager
        {
            get
            {
                return Get.Run.UniqueIDsManager;
            }
        }

        public static PlaceManager PlaceManager
        {
            get
            {
                return Get.Run.PlaceManager;
            }
        }

        public static FactionManager FactionManager
        {
            get
            {
                return Get.Run.FactionManager;
            }
        }

        public static UI UI
        {
            get
            {
                return Get.Run.UI;
            }
        }

        public static MainSceneObjects MainSceneObjects
        {
            get
            {
                return Get.Run.MainSceneObjects;
            }
        }

        public static CameraEffects CameraEffects
        {
            get
            {
                return Get.Run.CameraEffects;
            }
        }

        public static RunInfo RunInfo
        {
            get
            {
                return Get.Run.RunInfo;
            }
        }

        public static RunSpec RunSpec
        {
            get
            {
                return Get.Run.Spec;
            }
        }

        public static AssetsPrewarmer AssetsPrewarmer
        {
            get
            {
                return Get.Run.AssetsPrewarmer;
            }
        }

        public static DayNightCycleManager DayNightCycleManager
        {
            get
            {
                return Get.Run.DayNightCycleManager;
            }
        }

        public static WindowManager WindowManager
        {
            get
            {
                MainMenuUI mainMenuUI = Get.MainMenuUI;
                return ((mainMenuUI != null) ? mainMenuUI.WindowManager : null) ?? Get.UI.WindowManager;
            }
        }

        public static Minimap Minimap
        {
            get
            {
                return Get.UI.Minimap;
            }
        }

        public static IconOverlayDrawer IconOverlayDrawer
        {
            get
            {
                return Get.UI.IconOverlayDrawer;
            }
        }

        public static StaticTextOverlays StaticTextOverlays
        {
            get
            {
                return Get.UI.StaticTextOverlays;
            }
        }

        public static DragAndDrop DragAndDrop
        {
            get
            {
                MainMenuUI mainMenuUI = Get.MainMenuUI;
                return ((mainMenuUI != null) ? mainMenuUI.DragAndDrop : null) ?? Get.UI.DragAndDrop;
            }
        }

        public static FloatingTexts FloatingTexts
        {
            get
            {
                return Get.UI.FloatingTexts;
            }
        }

        public static Tooltips Tooltips
        {
            get
            {
                MainMenuUI mainMenuUI = Get.MainMenuUI;
                return ((mainMenuUI != null) ? mainMenuUI.Tooltips : null) ?? Get.UI.Tooltips;
            }
        }

        public static SeenEntitiesDrawer SeenEntitiesDrawer
        {
            get
            {
                return Get.UI.SeenEntitiesDrawer;
            }
        }

        public static StrikeOverlays StrikeOverlays
        {
            get
            {
                return Get.UI.StrikeOverlays;
            }
        }

        public static WheelSelector WheelSelector
        {
            get
            {
                return Get.UI.WheelSelector;
            }
        }

        public static UseOnTargetUI UseOnTargetUI
        {
            get
            {
                return Get.UI.UseOnTargetUI;
            }
        }

        public static ProgressBarDrawer ProgressBarDrawer
        {
            get
            {
                return Get.UI.ProgressBarDrawer;
            }
        }

        public static QuestCompletedText QuestCompletedText
        {
            get
            {
                return Get.UI.QuestCompletedText;
            }
        }

        public static WorldEventNotification WorldEventNotification
        {
            get
            {
                return Get.UI.WorldEventNotification;
            }
        }

        public static BossSlainText BossSlainText
        {
            get
            {
                return Get.UI.BossSlainText;
            }
        }

        public static RewindUI RewindUI
        {
            get
            {
                return Get.UI.RewindUI;
            }
        }

        public static NextTurnsUI NextTurnsUI
        {
            get
            {
                return Get.UI.NextTurnsUI;
            }
        }

        public static TimeDrawer TimeDrawer
        {
            get
            {
                return Get.UI.TimeDrawer;
            }
        }

        public static RandomTip RandomTip
        {
            get
            {
                return Get.UI.RandomTip;
            }
        }

        public static PlayerHitMarkers PlayerHitMarkers
        {
            get
            {
                return Get.UI.PlayerHitMarkers;
            }
        }

        public static ScoreGainEffects ScoreGainEffects
        {
            get
            {
                return Get.UI.ScoreGainEffects;
            }
        }

        public static DeathScreenDrawer DeathScreenDrawer
        {
            get
            {
                return Get.UI.DeathScreenDrawer;
            }
        }

        public static BossHPBar BossHPBar
        {
            get
            {
                return Get.UI.BossHPBar;
            }
        }

        public static ActiveQuestsReadout ActiveQuestsReadout
        {
            get
            {
                return Get.UI.ActiveQuestsReadout;
            }
        }

        public static HPBarOverlayDrawer HPBarOverlayDrawer
        {
            get
            {
                return Get.UI.HPBarOverlayDrawer;
            }
        }

        public static NewImportantConditionsUI NewImportantConditionsUI
        {
            get
            {
                return Get.UI.NewImportantConditionsUI;
            }
        }

        public static RunConfig RunConfig
        {
            get
            {
                return Get.RunInfo.Config;
            }
        }

        public static int RunSeed
        {
            get
            {
                return Get.RunConfig.RunSeed;
            }
        }

        public static DifficultySpec Difficulty
        {
            get
            {
                return Get.RunConfig.Difficulty;
            }
        }

        public static ActiveLanguage ActiveLanguage
        {
            get
            {
                return Get.Languages.ActiveLanguage;
            }
        }

        public static InteractionManager InteractionManager
        {
            get
            {
                return Get.Player.InteractionManager;
            }
        }

        public static PlayerMovementManager PlayerMovementManager
        {
            get
            {
                return Get.Player.PlayerMovementManager;
            }
        }

        public static SavedCameraPosition SavedCameraPosition
        {
            get
            {
                return Get.Player.SavedCameraPosition;
            }
        }

        public static KillCounter KillCounter
        {
            get
            {
                return Get.Player.KillCounter;
            }
        }

        public static ThisRunLobbyItems ThisRunLobbyItems
        {
            get
            {
                return Get.Player.CollectedLobbyItems;
            }
        }

        public static ThisRunPrivateRoomStructures ThisRunPrivateRoomStructures
        {
            get
            {
                return Get.Player.CollectedPrivateRoomStructures;
            }
        }

        public static ThisRunQuestsState ThisRunQuestsState
        {
            get
            {
                return Get.Player.QuestsState;
            }
        }

        public static ThisRunCompletedQuests ThisRunCompletedQuests
        {
            get
            {
                return Get.Player.CompletedQuests;
            }
        }

        public static SkillManager SkillManager
        {
            get
            {
                return Get.Player.SkillManager;
            }
        }

        public static Actor MainActor
        {
            get
            {
                return Get.Player.MainActor;
            }
        }

        public static Actor NowControlledActor
        {
            get
            {
                return Get.Player.NowControlledActor;
            }
        }

        public static List<Actor> PlayerParty
        {
            get
            {
                return Get.Player.Party;
            }
        }

        public static PlayerModel PlayerModel
        {
            get
            {
                return Get.Player.PlayerModel;
            }
        }

        public static PlannedPlayerActions PlannedPlayerActions
        {
            get
            {
                return Get.InteractionManager.PlannedPlayerActions;
            }
        }

        public static WorldSequenceable WorldSequenceable
        {
            get
            {
                return Get.World.WorldSequenceable;
            }
        }

        public static PathFinder PathFinder
        {
            get
            {
                return Get.World.PathFinder;
            }
        }

        public static ShortestPathsCache ShortestPathsCache
        {
            get
            {
                return Get.World.ShortestPathsCache;
            }
        }

        public static BFSCache BFSCache
        {
            get
            {
                return Get.World.BFSCache;
            }
        }

        public static CellsInfo CellsInfo
        {
            get
            {
                return Get.cachedCellsInfo;
            }
        }

        public static FogOfWar FogOfWar
        {
            get
            {
                return Get.World.FogOfWar;
            }
        }

        public static RetainedRoomInfo RetainedRoomInfo
        {
            get
            {
                return Get.World.RetainedRoomInfo;
            }
        }

        public static VisibilityCache VisibilityCache
        {
            get
            {
                return Get.World.VisibilityCache;
            }
        }

        public static VisualEffectsManager VisualEffectsManager
        {
            get
            {
                return Get.World.VisualEffectsManager;
            }
        }

        public static GameObjectHighlighter GameObjectHighlighter
        {
            get
            {
                return Get.World.GameObjectHighlighter;
            }
        }

        public static GameObjectFader GameObjectFader
        {
            get
            {
                return Get.World.GameObjectFader;
            }
        }

        public static SectionsManager SectionsManager
        {
            get
            {
                return Get.World.SectionsManager;
            }
        }

        public static TiledDecals TiledDecals
        {
            get
            {
                return Get.World.TiledDecals;
            }
        }

        public static WorldInfo WorldInfo
        {
            get
            {
                World world = Get.World;
                if (world == null)
                {
                    return null;
                }
                return world.WorldInfo;
            }
        }

        public static CellHighlighter CellHighlighter
        {
            get
            {
                return Get.World.CellHighlighter;
            }
        }

        public static ShatterManager ShatterManager
        {
            get
            {
                return Get.World.ShatterManager;
            }
        }

        public static VolumeShatterManager VolumeShatterManager
        {
            get
            {
                return Get.World.VolumeShatterManager;
            }
        }

        public static LightManager LightManager
        {
            get
            {
                return Get.World.LightManager;
            }
        }

        public static ParticleSystemFinisher ParticleSystemFinisher
        {
            get
            {
                return Get.World.ParticleSystemFinisher;
            }
        }

        public static WeatherManager WeatherManager
        {
            get
            {
                return Get.World.WeatherManager;
            }
        }

        public static ShopkeeperGreetingManager ShopkeeperGreetingManager
        {
            get
            {
                return Get.World.ShopkeeperGreetingManager;
            }
        }

        public static BurrowManager BurrowManager
        {
            get
            {
                return Get.World.BurrowManager;
            }
        }

        public static WorldEventsManager WorldEventsManager
        {
            get
            {
                return Get.World.WorldEventsManager;
            }
        }

        public static WorldSituationsManager WorldSituationsManager
        {
            get
            {
                return Get.World.WorldSituationsManager;
            }
        }

        public static WorldSpec WorldSpec
        {
            get
            {
                return Get.World.Spec;
            }
        }

        public static WorldConfig WorldConfig
        {
            get
            {
                WorldInfo worldInfo = Get.WorldInfo;
                if (worldInfo == null)
                {
                    return null;
                }
                return worldInfo.Config;
            }
        }

        public static int Floor
        {
            get
            {
                WorldConfig worldConfig = Get.WorldConfig;
                if (worldConfig == null)
                {
                    return 1;
                }
                return worldConfig.Floor;
            }
        }

        public static int WorldSeed
        {
            get
            {
                return Get.WorldConfig.WorldSeed;
            }
        }

        public static Place Place
        {
            get
            {
                return Get.WorldConfig.Place;
            }
        }

        public static PlaceSpec PlaceSpec
        {
            get
            {
                Place place = Get.Place;
                if (place == null)
                {
                    return null;
                }
                return place.Spec;
            }
        }

        public static RootGOC RootGOC
        {
            get
            {
                MainMenuUI mainMenuUI = Get.MainMenuUI;
                return ((mainMenuUI != null) ? mainMenuUI.RootGOC : null) ?? Get.MainSceneObjects.RootGOC;
            }
        }

        public static FPPControllerGOC FPPControllerGOC
        {
            get
            {
                return Get.MainSceneObjects.FPPControllerGOC;
            }
        }

        public static FancyOutlineGOC FancyOutlineGOC
        {
            get
            {
                return Get.MainSceneObjects.FancyOutlineGOC;
            }
        }

        public static MouseAttachmentDrawerGOC MouseAttachmentDrawerGOC
        {
            get
            {
                MainMenuUI mainMenuUI = Get.MainMenuUI;
                return ((mainMenuUI != null) ? mainMenuUI.MouseAttachmentDrawerGOC : null) ?? Get.MainSceneObjects.MouseAttachmentDrawerGOC;
            }
        }

        public static LessonDrawerGOC LessonDrawerGOC
        {
            get
            {
                return Get.MainSceneObjects.LessonDrawerGOC;
            }
        }

        public static Camera Camera
        {
            get
            {
                return Get.cachedCamera;
            }
        }

        public static Transform CameraTransform
        {
            get
            {
                return Get.cachedCameraTransform;
            }
        }

        public static Vector3 CameraPosition
        {
            get
            {
                return Get.cachedCameraTransform.position;
            }
        }

        public static Camera FPPItemCamera
        {
            get
            {
                return Get.MainSceneObjects.FPPItemCamera;
            }
        }

        public static Camera HighlightCamera
        {
            get
            {
                return Get.MainSceneObjects.HighlightCamera;
            }
        }

        public static GameObject RuntimeSpecialContainer
        {
            get
            {
                return Get.MainSceneObjects.RuntimeSpecialContainer;
            }
        }

        public static GameObject RuntimeSectionsContainer
        {
            get
            {
                return Get.MainSceneObjects.RuntimeSectionsContainer;
            }
        }

        public static GameObject AudioSourceContainer
        {
            get
            {
                return Get.MainSceneObjects.AudioSourceContainer;
            }
        }

        public static GameObject CameraOffset
        {
            get
            {
                return Get.MainSceneObjects.CameraOffset;
            }
        }

        public static GameObject PlayerGravity
        {
            get
            {
                return Get.MainSceneObjects.PlayerGravity;
            }
        }

        public static ScreenFaderGOC ScreenFader
        {
            get
            {
                MainMenuUI mainMenuUI = Get.MainMenuUI;
                return ((mainMenuUI != null) ? mainMenuUI.ScreenFader : null) ?? Get.MainSceneObjects.ScreenFader;
            }
        }

        public static BlackBarsGOC BlackBars
        {
            get
            {
                return Get.MainSceneObjects.BlackBars;
            }
        }

        public static TextSequenceDrawerGOC TextSequenceDrawer
        {
            get
            {
                return Get.MainSceneObjects.TextSequenceDrawer;
            }
        }

        public static DungeonMapDrawerGOC DungeonMapDrawer
        {
            get
            {
                return Get.MainSceneObjects.DungeonMapDrawer;
            }
        }

        public static DevConsoleGOC DevConsole
        {
            get
            {
                MainMenuUI mainMenuUI = Get.MainMenuUI;
                return ((mainMenuUI != null) ? mainMenuUI.DevConsole : null) ?? Get.MainSceneObjects.DevConsole;
            }
        }

        public static VignetteAndChromaticAberration Vignette
        {
            get
            {
                return Get.MainSceneObjects.Vignette;
            }
        }

        public static BlurOptimized Blur
        {
            get
            {
                return Get.MainSceneObjects.Blur;
            }
        }

        public static ColorCorrectionCurves Grayscale
        {
            get
            {
                return Get.MainSceneObjects.Grayscale;
            }
        }

        public static MotionBlur MotionBlur
        {
            get
            {
                return Get.MainSceneObjects.MotionBlur;
            }
        }

        public static ParticleSystem HighSpeedParticles
        {
            get
            {
                return Get.MainSceneObjects.HighSpeedParticles;
            }
        }

        public static int DefaultLayer
        {
            get
            {
                return Get.Layers.DefaultLayer;
            }
        }

        public static int PlayerLayer
        {
            get
            {
                return Get.Layers.PlayerLayer;
            }
        }

        public static int PlayerMask
        {
            get
            {
                return Get.Layers.PlayerMask;
            }
        }

        public static int FloorMarkersLayer
        {
            get
            {
                return Get.Layers.FloorMarkersLayer;
            }
        }

        public static int FloorMarkersMask
        {
            get
            {
                return Get.Layers.FloorMarkersMask;
            }
        }

        public static int IgnoreRaycastLayer
        {
            get
            {
                return Get.Layers.IgnoreRaycastLayer;
            }
        }

        public static int IgnoreRaycastMask
        {
            get
            {
                return Get.Layers.IgnoreRaycastMask;
            }
        }

        public static int ForHighlightLayer
        {
            get
            {
                return Get.Layers.ForHighlightLayer;
            }
        }

        public static int ActorMask
        {
            get
            {
                return Get.Layers.ActorMask;
            }
        }

        public static int ActorLayer
        {
            get
            {
                return Get.Layers.ActorLayer;
            }
        }

        public static int InspectModeOnlyMask
        {
            get
            {
                return Get.Layers.InspectModeOnlyMask;
            }
        }

        public static int InspectModeOnlyLayer
        {
            get
            {
                return Get.Layers.InspectModeOnlyLayer;
            }
        }

        public static Vector3Int CameraGravity
        {
            get
            {
                return Get.FPPControllerGOC.CameraGravity;
            }
        }

        public static WorldGenMemory WorldGenMemory
        {
            get
            {
                return WorldGen.CurMemory;
            }
        }

        public static void CacheReferences()
        {
            Run run = Get.Run;
            Get.cachedWorld = ((run != null) ? run.World : null);
            Run run2 = Get.Run;
            Get.cachedPlayer = ((run2 != null) ? run2.Player : null);
            World world = Get.cachedWorld;
            Get.cachedCellsInfo = ((world != null) ? world.CellsInfo : null);
            MainMenuUI mainMenuUI = Get.MainMenuUI;
            Camera camera;
            if ((camera = ((mainMenuUI != null) ? mainMenuUI.Camera : null)) == null)
            {
                Run run3 = Get.Run;
                if (run3 == null)
                {
                    camera = null;
                }
                else
                {
                    MainSceneObjects mainSceneObjects = run3.MainSceneObjects;
                    camera = ((mainSceneObjects != null) ? mainSceneObjects.Camera : null);
                }
            }
            Get.cachedCamera = camera;
            Get.cachedCameraTransform = ((Get.cachedCamera != null) ? Get.cachedCamera.transform : null);
        }

        private static World cachedWorld;

        private static Player cachedPlayer;

        private static CellsInfo cachedCellsInfo;

        private static Camera cachedCamera;

        private static Transform cachedCameraTransform;

        public static EntitySpec Entity_Player;

        public static EntitySpec Entity_LobbyPlayer;

        public static EntitySpec Entity_Fellow;

        public static EntitySpec Entity_Wall;

        public static EntitySpec Entity_WoodenWall;

        public static EntitySpec Entity_BrickWall;

        public static EntitySpec Entity_Window;

        public static EntitySpec Entity_Skeleton;

        public static EntitySpec Entity_Dragon;

        public static EntitySpec Entity_Tangler;

        public static EntitySpec Entity_Trickster;

        public static EntitySpec Entity_Door;

        public static EntitySpec Entity_Sword;

        public static EntitySpec Entity_Mace;

        public static EntitySpec Entity_LeatherArmor;

        public static EntitySpec Entity_PlateArmor;

        public static EntitySpec Entity_MagicRobe;

        public static EntitySpec Entity_Axe;

        public static EntitySpec Entity_TemporarilyOpenedDoor;

        public static EntitySpec Entity_BarredDoor;

        public static EntitySpec Entity_Floor;

        public static EntitySpec Entity_Stairs;

        public static EntitySpec Entity_Bridge;

        public static EntitySpec Entity_UnstableBridge;

        public static EntitySpec Entity_Chest;

        public static EntitySpec Entity_Torch;

        public static EntitySpec Entity_VioletTorch;

        public static EntitySpec Entity_RedLamp;

        public static EntitySpec Entity_DiscoBall;

        public static EntitySpec Entity_WallLamp;

        public static EntitySpec Entity_YellowWallLamp;

        public static EntitySpec Entity_GreenWallLamp;

        public static EntitySpec Entity_PinkWallLamp;

        public static EntitySpec Entity_VioletWallLamp;

        public static EntitySpec Entity_CyanWallLamp;

        public static EntitySpec Entity_Spikes;

        public static EntitySpec Entity_PoisonSpikes;

        public static EntitySpec Entity_ContaminatedSpikes;

        public static EntitySpec Entity_Pillar;

        public static EntitySpec Entity_Crates;

        public static EntitySpec Entity_Grass;

        public static EntitySpec Entity_WoodenFloor;

        public static EntitySpec Entity_Sign;

        public static EntitySpec Entity_Gold;

        public static EntitySpec Entity_Diamond;

        public static EntitySpec Entity_Grave;

        public static EntitySpec Entity_SkeletalRemains;

        public static EntitySpec Entity_DragonRemains;

        public static EntitySpec Entity_Mummy;

        public static EntitySpec Entity_Mimic;

        public static EntitySpec Entity_WaspNest;

        public static EntitySpec Entity_Cage;

        public static EntitySpec Entity_HangingCage;

        public static EntitySpec Entity_Dog;

        public static EntitySpec Entity_EnchantingTable;

        public static EntitySpec Entity_SacrificialAltar;

        public static EntitySpec Entity_DevilStatue;

        public static EntitySpec Entity_SurgicalBed;

        public static EntitySpec Entity_CutsceneTrigger;

        public static EntitySpec Entity_Staircase;

        public static EntitySpec Entity_StaircaseToLobby;

        public static EntitySpec Entity_Gate;

        public static EntitySpec Entity_GateReinforced;

        public static EntitySpec Entity_Lever;

        public static EntitySpec Entity_Ladder;

        public static EntitySpec Entity_SilverKey;

        public static EntitySpec Entity_SilverDoor;

        public static EntitySpec Entity_SilverChest;

        public static EntitySpec Entity_Shrine;

        public static EntitySpec Entity_FountainOfLife;

        public static EntitySpec Entity_Fire;

        public static EntitySpec Entity_MagicFire;

        public static EntitySpec Entity_Toxofungus;

        public static EntitySpec Entity_Bread;

        public static EntitySpec Entity_Honey;

        public static EntitySpec Entity_Fish;

        public static EntitySpec Entity_ToxicFish;

        public static EntitySpec Entity_Wings;

        public static EntitySpec Entity_Watch;

        public static EntitySpec Entity_Superwatch;

        public static EntitySpec Entity_NewRunStaircase;

        public static EntitySpec Entity_StaircaseLocked;

        public static EntitySpec Entity_NewRunWithSeedStaircase;

        public static EntitySpec Entity_TrainingRoomStaircase;

        public static EntitySpec Entity_Water;

        public static EntitySpec Entity_ToxicWater;

        public static EntitySpec Entity_PressurePlate;

        public static EntitySpec Entity_TemporaryGate;

        public static EntitySpec Entity_Portal;

        public static EntitySpec Entity_Pedestal;

        public static EntitySpec Entity_Planks;

        public static EntitySpec Entity_PlanksItem;

        public static EntitySpec Entity_BigPoisonTrap;

        public static EntitySpec Entity_QuestLog;

        public static EntitySpec Entity_StatsBoard;

        public static EntitySpec Entity_LockedDoor;

        public static EntitySpec Entity_LockedDoorHallway;

        public static EntitySpec Entity_LockedDoorTrainingRoom;

        public static EntitySpec Entity_LobbyShopkeeper;

        public static EntitySpec Entity_LobbyShopkeeperCage;

        public static EntitySpec Entity_PetRat;

        public static EntitySpec Entity_PetRatCheese;

        public static EntitySpec Entity_UnlockableAsItem;

        public static EntitySpec Entity_PrivateRoomStructureAsItem;

        public static EntitySpec Entity_Potion_Health;

        public static EntitySpec Entity_Potion_Antidote;

        public static EntitySpec Entity_Potion_Mana;

        public static EntitySpec Entity_Potion_Poison;

        public static EntitySpec Entity_Potion_Beer;

        public static EntitySpec Entity_Scroll_Identification;

        public static EntitySpec Entity_Scroll_CurseRemoval;

        public static EntitySpec Entity_Scroll_Awareness;

        public static EntitySpec Entity_Spear;

        public static EntitySpec Entity_HeavySpear;

        public static EntitySpec Entity_Cannon;

        public static EntitySpec Entity_CeilingSpikes;

        public static EntitySpec Entity_Barrel_Damage;

        public static EntitySpec Entity_Barrel_Fire;

        public static EntitySpec Entity_Barrel_Poison;

        public static EntitySpec Entity_BeerBarrel;

        public static EntitySpec Entity_Goo;

        public static EntitySpec Entity_BlindingGoo;

        public static EntitySpec Entity_GooPuddle;

        public static EntitySpec Entity_SpiderWeb;

        public static EntitySpec Entity_MagicWall;

        public static EntitySpec Entity_VialWithWater;

        public static EntitySpec Entity_EmptyVial;

        public static EntitySpec Entity_Healplant;

        public static EntitySpec Entity_Manaplant;

        public static EntitySpec Entity_CeilingBars;

        public static EntitySpec Entity_CeilingBarsReinforced;

        public static EntitySpec Entity_DamagedCeilingBars;

        public static EntitySpec Entity_DamagedCeilingBarsReinforced;

        public static EntitySpec Entity_FloorBars;

        public static EntitySpec Entity_FloorBarsReinforced;

        public static EntitySpec Entity_DescendTrigger;

        public static EntitySpec Entity_VoidSoundSource;

        public static EntitySpec Entity_Rain;

        public static EntitySpec Entity_Fan;

        public static EntitySpec Entity_GentleSmoke;

        public static EntitySpec Entity_WallWithPipe;

        public static EntitySpec Entity_CeilingWithPipe;

        public static EntitySpec Entity_PipeSmoke;

        public static EntitySpec Entity_Shop;

        public static EntitySpec Entity_Shield;

        public static EntitySpec Entity_GreatShield;

        public static EntitySpec Entity_PuzzlePiece;

        public static EntitySpec Entity_PuzzleDoor;

        public static EntitySpec Entity_LockedPuzzleDoor;

        public static EntitySpec Entity_Chandelier;

        public static EntitySpec Entity_Ghost;

        public static EntitySpec Entity_BossTrophy;

        public static EntitySpec Entity_Bandage;

        public static EntitySpec Entity_BigMagicTrap;

        public static EntitySpec Entity_MagicColumns;

        public static EntitySpec Entity_Frame;

        public static EntitySpec Entity_Ventillation;

        public static EntitySpec Entity_Chain;

        public static EntitySpec Entity_TimeMachine;

        public static EntitySpec Entity_PuzzleSculpture;

        public static EntitySpec Entity_BannerPuzzle;

        public static EntitySpec Entity_Sky;

        public static EntitySpec Entity_Shopkeeper;

        public static EntitySpec Entity_HangingRope;

        public static EntitySpec Entity_Shuriken;

        public static EntitySpec Entity_PocketKnife;

        public static EntitySpec Entity_PoisonShuriken;

        public static EntitySpec Entity_VineSeed;

        public static EntitySpec Entity_StaminaBooster;

        public static EntitySpec Entity_TeleportationOrb;

        public static EntitySpec Entity_Rope;

        public static EntitySpec Entity_ChainItem;

        public static EntitySpec Entity_Stone;

        public static EntitySpec Entity_Safe;

        public static EntitySpec Entity_GildedLockbox;

        public static EntitySpec Entity_ElectronicSafe;

        public static EntitySpec Entity_ElectronicSafeButton;

        public static EntitySpec Entity_Fence;

        public static EntitySpec Entity_Platform;

        public static EntitySpec Entity_Slime;

        public static EntitySpec Entity_SmallSlime;

        public static EntitySpec Entity_Rat;

        public static EntitySpec Entity_Sludgen;

        public static EntitySpec Entity_Archer;

        public static EntitySpec Entity_RatNest;

        public static EntitySpec Entity_ProximityBomb;

        public static EntitySpec Entity_Gorhorn;

        public static EntitySpec Entity_Mole;

        public static EntitySpec Entity_Thief;

        public static EntitySpec Entity_Barrel;

        public static EntitySpec Entity_BigCrate;

        public static EntitySpec Entity_BigMagnet;

        public static EntitySpec Entity_LockedMemoryDoor;

        public static EntitySpec Entity_MemoryDoor;

        public static EntitySpec Entity_MemoryPiece1Status;

        public static EntitySpec Entity_MemoryPiece2Status;

        public static EntitySpec Entity_MemoryPiece3Status;

        public static EntitySpec Entity_MemoryPiece4Status;

        public static EntitySpec Entity_MemoryPiece1;

        public static EntitySpec Entity_MemoryPiece2;

        public static EntitySpec Entity_MemoryPiece3;

        public static EntitySpec Entity_MemoryPiece4;

        public static EntitySpec Entity_WallPiece1;

        public static EntitySpec Entity_WallPiece2;

        public static EntitySpec Entity_WallPiece3;

        public static EntitySpec Entity_WallPiece4;

        public static EntitySpec Entity_WallPiece5;

        public static EntitySpec Entity_Skybox;

        public static EntitySpec Entity_EndingPortal;

        public static EntitySpec Entity_FinalDialogueTrigger;

        public static EntitySpec Entity_GuardianDialogueTrigger;

        public static EntitySpec Entity_Vines;

        public static EntitySpec Entity_AncientDevice;

        public static EntitySpec Entity_GreenKeyFragment;

        public static EntitySpec Entity_GreenChest;

        public static EntitySpec Entity_AncientMechanism;

        public static EntitySpec Entity_AncientContainer;

        public static EntitySpec Entity_WallWithCompartment;

        public static EntitySpec Entity_ProximitySmokeBomb;

        public static EntitySpec Entity_ProximityFireBomb;

        public static EntitySpec Entity_Liana;

        public static EntitySpec Entity_SecretPassage;

        public static EntitySpec Entity_FishOil;

        public static EntitySpec Entity_Stardust;

        public static EntitySpec Entity_SecretRoomTrigger;

        public static EntitySpec Entity_CeilingSupport;

        public static EntitySpec Entity_ThrowingKnife;

        public static EntitySpec Entity_PoisonThrowingKnife;

        public static EntitySpec Entity_Wasp;

        public static EntitySpec Entity_SpawnerPressurePlate;

        public static EntitySpec Entity_InvisibleBlocker;

        public static EntitySpec Entity_TimedSpikes;

        public static EntitySpec Entity_DangerDoor;

        public static EntitySpec Entity_Bookshelf;

        public static EntitySpec Entity_Table;

        public static EntitySpec Entity_Chair;

        public static EntitySpec Entity_Bed;

        public static EntitySpec Entity_WallWithHole;

        public static EntitySpec Entity_StaircaseRoomSign;

        public static EntitySpec Entity_LobbySign;

        public static EntitySpec Entity_Piston;

        public static EntitySpec Entity_Bars;

        public static EntitySpec Entity_Lava;

        public static EntitySpec Entity_ShopRoomSign;

        public static EntitySpec Entity_HallwaySign;

        public static EntitySpec Entity_TrainingRoomSign;

        public static EntitySpec Entity_PuzzleRoomSign;

        public static EntitySpec Entity_PrivateRoomSign;

        public static EntitySpec Entity_Display;

        public static EntitySpec Entity_TutorialEndPortal;

        public static EntitySpec Entity_TrainingRoomEndPortal;

        public static EntitySpec Entity_Wanderer;

        public static EntitySpec Entity_Ring;

        public static EntitySpec Entity_MoonlightMedallion;

        public static EntitySpec Entity_Relic;

        public static EntitySpec Entity_MinedRedCrystal;

        public static EntitySpec Entity_MinedGreenCrystal;

        public static EntitySpec Entity_MinedBlueCrystal;

        public static EntitySpec Entity_RedCrystal;

        public static EntitySpec Entity_GreenCrystal;

        public static EntitySpec Entity_BlueCrystal;

        public static EntitySpec Entity_Pickaxe;

        public static EntitySpec Entity_RawMeat;

        public static EntitySpec Entity_Bonfire;

        public static EntitySpec Entity_Shovel;

        public static EntitySpec Entity_Dirt;

        public static EntitySpec Entity_Spider;

        public static EntitySpec Entity_Bone;

        public static EntitySpec Entity_Amulet;

        public static EntitySpec Entity_AmuletOfYerdon;

        public static EntitySpec Entity_TrashPile;

        public static EntitySpec Entity_GlassWall;

        public static EntitySpec Entity_PoisonCloud;

        public static EntitySpec Entity_Boulder;

        public static EntitySpec Entity_MetalCrate;

        public static EntitySpec Entity_Throne;

        public static EntitySpec Entity_Hatch;

        public static EntitySpec Entity_HealingCrystal;

        public static EntitySpec Entity_SmallHealingCrystal;

        public static EntitySpec Entity_ManaCrystal;

        public static EntitySpec Entity_SmallManaCrystal;

        public static EntitySpec Entity_GoldenDoor;

        public static EntitySpec Entity_GoldenKey;

        public static EntitySpec Entity_Googon;

        public static EntitySpec Entity_Demon;

        public static EntitySpec Entity_Roothealer;

        public static EntitySpec Entity_Blazekin;

        public static EntitySpec Entity_MagicMirror;

        public static EntitySpec Entity_Medkit;

        public static EntitySpec Entity_Note;

        public static EntitySpec Entity_Cultist;

        public static EntitySpec Entity_Beggar;

        public static EntitySpec Entity_CultistCircle;

        public static EntitySpec Entity_DailyChallengeBoard;

        public static EntitySpec Entity_HolyWater;

        public static EntitySpec Entity_Guardian;

        public static EntitySpec Entity_HealingCrystalForDiamond;

        public static EntitySpec Entity_RoyalKey;

        public static EntitySpec Entity_RoyalDoor;

        public static EntitySpec Entity_MirrorShard;

        public static EntitySpec Entity_Soffit;

        public static EntitySpec Entity_ChainPost;

        public static EntitySpec Entity_CeilingBarsLight;

        public static EntitySpec Entity_FloorBarsLight;

        public static EntitySpec Entity_RunEndPortal;

        public static EntitySpec Entity_Placeholder;

        public static EntitySpec Entity_Shelf;

        public static EntitySpec Entity_FlowerVase;

        public static EntitySpec Entity_Phantom;

        public static EntitySpec Entity_Rock;

        public static EntitySpec Entity_Pond;

        public static EntitySpec Entity_Spirit;

        public static EntitySpec Entity_AltarOfRedemption;

        public static EntitySpec Entity_CosmeticsWardrobe;

        public static EntitySpec Entity_Soulforge;

        public static EntitySpec Entity_AntidoteBasin;

        public static EntitySpec Entity_UpgradeOrb;

        public static EntitySpec Entity_Anvil;

        public static EntitySpec Entity_IronOre;

        public static EntitySpec Entity_Glyph1;

        public static EntitySpec Entity_Glyph2;

        public static EntitySpec Entity_Glyph3;

        public static EntitySpec Entity_Glyph4;

        public static ActionSpec Action_Wait;

        public static ActionSpec Action_MoveSelf;

        public static ActionSpec Action_MoveDownByGravity;

        public static ActionSpec Action_ResolveVelocity;

        public static ActionSpec Action_Use;

        public static ActionSpec Action_MoveItemInInventory;

        public static ActionSpec Action_Equip;

        public static ActionSpec Action_Unequip;

        public static ActionSpec Action_DropItem;

        public static ActionSpec Action_DestroyItemInInventory;

        public static ActionSpec Action_ResolveCondition;

        public static ActionSpec Action_ResolveWorld;

        public static ActionSpec Action_Sleep;

        public static ActionSpec Action_Think;

        public static ActionSpec Action_BuyItem;

        public static ActionSpec Action_ReorderSpell;

        public static ActionSpec Action_ChooseSpell;

        public static ActionSpec Action_RewindTime;

        public static ActionSpec Action_UseRecipe;

        public static ActionSpec Action_ResolveStructure;

        public static ActionSpec Action_BuyPrivateRoomStructure;

        public static ActionSpec Action_Debug_RewindTime;

        public static ActionSpec Action_Debug_Spawn;

        public static ActionSpec Action_Debug_SpawnBlueprint;

        public static ActionSpec Action_Debug_SaveBlueprint;

        public static ActionSpec Action_Debug_DeSpawn;

        public static ActionSpec Action_Debug_Destroy;

        public static ActionSpec Action_Debug_AddToInventory;

        public static ActionSpec Action_Debug_AddCondition;

        public static ActionSpec Action_Debug_RemoveCondition;

        public static ActionSpec Action_Debug_AddUseEffect;

        public static ActionSpec Action_Debug_RemoveUseEffect;

        public static ActionSpec Action_Debug_SetGravity;

        public static ActionSpec Action_Debug_SetScale;

        public static ActionSpec Action_Debug_UnfogAll;

        public static ActionSpec Action_Debug_LevelUp;

        public static ActionSpec Action_Debug_Teleport;

        public static ActionSpec Action_Debug_SetRampUp;

        public static ActionSpec Action_Debug_AddWorldSituation;

        public static ActionSpec Action_Debug_RemoveWorldSituation;

        public static ActionSpec Action_Debug_DoWorldEvent;

        public static ActionSpec Action_Debug_Control;

        public static ActionSpec Action_Debug_AddParty;

        public static ActionSpec Action_Debug_RemoveParty;

        public static ActionSpec Action_Debug_AttachToChainPost;

        public static ActionSpec Action_ErrorRecoveryAddSequence;

        public static ActionSpec Action_Transfer;

        public static ActionSpec Action_UnlockSkill;

        public static ActionSpec Action_PulledByChain;

        public static ActionSpec Action_SwitchCurrentPartyMember;

        public static ActionSpec Action_ChangePartyMemberAIMode;

        public static ActionSpec Action_SortInventory;

        public static ConditionSpec Condition_Sleeping;

        public static ConditionSpec Condition_Levitating;

        public static ConditionSpec Condition_MaxHPOffset;

        public static ConditionSpec Condition_MaxStaminaOffset;

        public static ConditionSpec Condition_MaxManaOffset;

        public static ConditionSpec Condition_DealtDamageOffset;

        public static ConditionSpec Condition_DealtDamageFactor;

        public static ConditionSpec Condition_SeeRangeOffset;

        public static ConditionSpec Condition_Drunk;

        public static ConditionSpec Condition_Frozen;

        public static ConditionSpec Condition_Poisoned;

        public static ConditionSpec Condition_ImmuneToPoison;

        public static ConditionSpec Condition_ImmuneToFire;

        public static ConditionSpec Condition_Hungry;

        public static ConditionSpec Condition_Hunger;

        public static ConditionSpec Condition_Starving;

        public static ConditionSpec Condition_Burning;

        public static ConditionSpec Condition_Shield;

        public static ConditionSpec Condition_Entangled;

        public static ConditionSpec Condition_Bleeding;

        public static ConditionSpec Condition_BrokenBone;

        public static ConditionSpec Condition_ArmMissing;

        public static ConditionSpec Condition_LegMissing;

        public static ConditionSpec Condition_Disease;

        public static ConditionSpec Condition_SequencePerTurnMultiplier;

        public static ConditionSpec Condition_HPRegen;

        public static ConditionSpec Condition_StaminaRegen;

        public static ConditionSpec Condition_ManaRegen;

        public static ConditionSpec Condition_DamageOverTime;

        public static ConditionSpec Condition_Blind;

        public static ConditionSpec Condition_HungerRateMultiplier;

        public static ConditionSpec Condition_IncomingDamageOffset;

        public static ConditionSpec Condition_IncomingDamageFactor;

        public static ConditionSpec Condition_Berserker;

        public static ConditionSpec Condition_Panic;

        public static ConditionSpec Condition_IdentificationRateMultiplier;

        public static ConditionSpec Condition_CritChanceFactor;

        public static ConditionSpec Condition_MissChanceFactor;

        public static ConditionSpec Condition_SeeRangeCap;

        public static ConditionSpec Condition_InvertHostile;

        public static UseEffectSpec UseEffect_AddCondition;

        public static UseEffectSpec UseEffect_TeleportRandomly;

        public static UseEffectSpec UseEffect_Push;

        public static UseEffectSpec UseEffect_Spawn;

        public static UseEffectSpec UseEffect_PlacePrivateRoomStructure;

        public static UseEffectSpec UseEffect_PickUpPrivateRoomStructure;

        public static UseEffectSpec UseEffect_Damage;

        public static UseEffectSpec UseEffect_MakeNoise;

        public static UseEffectSpec UseEffect_SetOnFire;

        public static UseEffectSpec UseEffect_Pay;

        public static UseEffectSpec UseEffect_UnlockUnlockable;

        public static UseEffectSpec UseEffect_Heal;

        public static UseEffectSpec UseEffect_ShowDialogue;

        public static UseEffectSpec UseEffect_Kick;

        public static KeyBindingSpec KeyBinding_Minimap;

        public static KeyBindingSpec KeyBinding_MinimapAlt;

        public static KeyBindingSpec KeyBinding_MoveForward;

        public static KeyBindingSpec KeyBinding_MoveForwardAlt;

        public static KeyBindingSpec KeyBinding_MoveBack;

        public static KeyBindingSpec KeyBinding_MoveBackAlt;

        public static KeyBindingSpec KeyBinding_MoveLeft;

        public static KeyBindingSpec KeyBinding_MoveLeftAlt;

        public static KeyBindingSpec KeyBinding_MoveRight;

        public static KeyBindingSpec KeyBinding_MoveRightAlt;

        public static KeyBindingSpec KeyBinding_RotateLeft;

        public static KeyBindingSpec KeyBinding_RotateLeftAlt;

        public static KeyBindingSpec KeyBinding_RotateRight;

        public static KeyBindingSpec KeyBinding_RotateRightAlt;

        public static KeyBindingSpec KeyBinding_ShowInventory;

        public static KeyBindingSpec KeyBinding_ShowInventoryAlt;

        public static KeyBindingSpec KeyBinding_Interact;

        public static KeyBindingSpec KeyBinding_Accept;

        public static KeyBindingSpec KeyBinding_Cancel;

        public static KeyBindingSpec KeyBinding_Inspect;

        public static KeyBindingSpec KeyBinding_InspectAlt;

        public static KeyBindingSpec KeyBinding_Wait;

        public static KeyBindingSpec KeyBinding_AttackNearest;

        public static KeyBindingSpec KeyBinding_Fly;

        public static KeyBindingSpec KeyBinding_StationaryActionsOnly;

        public static KeyBindingSpec KeyBinding_OneStep;

        public static KeyBindingSpec KeyBinding_Rewind;

        public static KeyBindingSpec KeyBinding_SnapToCurrentRotation;

        public static KeyBindingSpec KeyBinding_LeanLeft;

        public static KeyBindingSpec KeyBinding_LeanRight;

        public static KeyBindingSpec KeyBinding_RotateToNearestTarget;

        public static KeyBindingSpec KeyBinding_QuickbarMenu;

        public static WorldSpec World_Standard;

        public static WorldSpec World_Shelter;

        public static VisualEffectSpec VisualEffect_Destroyed;

        public static VisualEffectSpec VisualEffect_DestroyedNoFloor;

        public static VisualEffectSpec VisualEffect_PlayerHit_NoImpact;

        public static VisualEffectSpec VisualEffect_PlayerHit_Impact;

        public static VisualEffectSpec VisualEffect_ActorHit;

        public static VisualEffectSpec VisualEffect_PlayerHits;

        public static VisualEffectSpec VisualEffect_PlayerHits_Miss;

        public static VisualEffectSpec VisualEffect_PlayerDrunk;

        public static VisualEffectSpec VisualEffect_PlayerHitsGround;

        public static VisualEffectSpec VisualEffect_PlayerHitsWater;

        public static VisualEffectSpec VisualEffect_ActorHitsGround;

        public static VisualEffectSpec VisualEffect_Explosion;

        public static VisualEffectSpec VisualEffect_CellAffectedByCannon;

        public static VisualEffectSpec VisualEffect_CeilingSpikesAnimation;

        public static VisualEffectSpec VisualEffect_ItemOrStructureFall;

        public static VisualEffectSpec VisualEffect_Fireflies;

        public static VisualEffectSpec VisualEffect_Magic;

        public static VisualEffectSpec VisualEffect_Ability;

        public static VisualEffectSpec VisualEffect_OpenChestAnimation;

        public static VisualEffectSpec VisualEffect_PullLeverAnimation;

        public static VisualEffectSpec VisualEffect_ExperienceParticle;

        public static VisualEffectSpec VisualEffect_CollapsingCeilingCell;

        public static VisualEffectSpec VisualEffect_CeilingCollapse;

        public static VisualEffectSpec VisualEffect_CeilingCollapseSmall;

        public static VisualEffectSpec VisualEffect_ActorDestroyedNearby;

        public static VisualEffectSpec VisualEffect_BossDestroyedNearby;

        public static VisualEffectSpec VisualEffect_PlayerDestroyed;

        public static VisualEffectSpec VisualEffect_GooSplat;

        public static VisualEffectSpec VisualEffect_BlindingGooSplat;

        public static VisualEffectSpec VisualEffect_PotionShatter;

        public static VisualEffectSpec VisualEffect_Sacrifice;

        public static VisualEffectSpec VisualEffect_ButtonPressAnimation;

        public static VisualEffectSpec VisualEffect_HealedOnKilledByWearable;

        public static VisualEffectSpec VisualEffect_BigMagnetPull;

        public static VisualEffectSpec VisualEffect_Gunshot;

        public static VisualEffectSpec VisualEffect_Flamethrower;

        public static VisualEffectSpec VisualEffect_LineAnimation;

        public static VisualEffectSpec VisualEffect_LevelUp;

        public static VisualEffectSpec VisualEffect_BoulderFall;

        public static VisualEffectSpec VisualEffect_Burrow;

        public static VisualEffectSpec VisualEffect_Unburrow;

        public static VisualEffectSpec VisualEffect_GainedSkillPoint;

        public static VisualEffectSpec VisualEffect_PlayerKicksDoor;

        public static VisualEffectSpec VisualEffect_BigActorMovedNearby;

        public static SoundSpec Sound_Walk;

        public static SoundSpec Sound_Fly;

        public static SoundSpec Sound_Hit;

        public static SoundSpec Sound_HighlightedEntity;

        public static SoundSpec Sound_Rotation;

        public static SoundSpec Sound_DragStart;

        public static SoundSpec Sound_DragEnd;

        public static SoundSpec Sound_DragStartItem;

        public static SoundSpec Sound_DragEndItem;

        public static SoundSpec Sound_OpenInventory;

        public static SoundSpec Sound_CloseInventory;

        public static SoundSpec Sound_Wear;

        public static SoundSpec Sound_Unwear;

        public static SoundSpec Sound_Equip;

        public static SoundSpec Sound_EquipCursed;

        public static SoundSpec Sound_Unequip;

        public static SoundSpec Sound_Hover;

        public static SoundSpec Sound_OpenWindow;

        public static SoundSpec Sound_CloseWindow;

        public static SoundSpec Sound_Drink;

        public static SoundSpec Sound_PotionMagicEffect;

        public static SoundSpec Sound_ItemIdentified;

        public static SoundSpec Sound_ConditionWornOff;

        public static SoundSpec Sound_UseEffectWornOff;

        public static SoundSpec Sound_PickUpItem;

        public static SoundSpec Sound_PickUpSpecialItem;

        public static SoundSpec Sound_Click;

        public static SoundSpec Sound_LobbyWelcomeSong;

        public static SoundSpec Sound_WelcomeSong;

        public static SoundSpec Sound_Teleport;

        public static SoundSpec Sound_OpenGate;

        public static SoundSpec Sound_OpenWithKey;

        public static SoundSpec Sound_UseScroll;

        public static SoundSpec Sound_UseWand;

        public static SoundSpec Sound_Eat;

        public static SoundSpec Sound_DroppedItem;

        public static SoundSpec Sound_DestroyItemInInventory;

        public static SoundSpec Sound_UseBow;

        public static SoundSpec Sound_LeverFound;

        public static SoundSpec Sound_SetTrap;

        public static SoundSpec Sound_QuestAccepted;

        public static SoundSpec Sound_QuestCompleted;

        public static SoundSpec Sound_QuestRewardClaimed;

        public static SoundSpec Sound_BoughtItem;

        public static SoundSpec Sound_LoseHealthGeneric;

        public static SoundSpec Sound_LevelUp;

        public static SoundSpec Sound_ThrowWeapon;

        public static SoundSpec Sound_SetOffTrap;

        public static SoundSpec Sound_UseLadder;

        public static SoundSpec Sound_RewindTime;

        public static SoundSpec Sound_CantRewindTime;

        public static SoundSpec Sound_UseSpell;

        public static SoundSpec Sound_ChooseSpell;

        public static SoundSpec Sound_Droplet;

        public static SoundSpec Sound_BossSlain;

        public static SoundSpec Sound_UseRecipe;

        public static SoundSpec Sound_DestroyGeneric;

        public static SoundSpec Sound_UseShield;

        public static SoundSpec Sound_DamageBlocked;

        public static SoundSpec Sound_OpenChest;

        public static SoundSpec Sound_NewActorSeen;

        public static SoundSpec Sound_DestroyFrozenActor;

        public static SoundSpec Sound_PullLever;

        public static SoundSpec Sound_FillVial;

        public static SoundSpec Sound_ExperienceParticleArrived;

        public static SoundSpec Sound_UseStaircase;

        public static SoundSpec Sound_RewindTimeLong;

        public static SoundSpec Sound_EntityPushed;

        public static SoundSpec Sound_NightLoop;

        public static SoundSpec Sound_DayLoop;

        public static SoundSpec Sound_RainLoop;

        public static SoundSpec Sound_LightningStrike1;

        public static SoundSpec Sound_LightningStrike2;

        public static SoundSpec Sound_LightningStrike3;

        public static SoundSpec Sound_LightningStrike4;

        public static SoundSpec Sound_BecameHungry;

        public static SoundSpec Sound_ScoreLevelUp;

        public static SoundSpec Sound_ScoreTick;

        public static SoundSpec Sound_ShopkeeperGreeting;

        public static SoundSpec Sound_ButtonPress;

        public static SoundSpec Sound_OpenSafe;

        public static SoundSpec Sound_TrialCompleted;

        public static SoundSpec Sound_TrialFailed;

        public static SoundSpec Sound_Enchant;

        public static SoundSpec Sound_Fishing;

        public static SoundSpec Sound_TriggerTriggered;

        public static SoundSpec Sound_TriggerTick;

        public static SoundSpec Sound_BigMagnetPull;

        public static SoundSpec Sound_LessonCompleted;

        public static SoundSpec Sound_Ending;

        public static SoundSpec Sound_PieceHit1;

        public static SoundSpec Sound_PieceHit2;

        public static SoundSpec Sound_PieceHit3;

        public static SoundSpec Sound_PieceHit4;

        public static SoundSpec Sound_PieceHit5;

        public static SoundSpec Sound_Miss;

        public static SoundSpec Sound_CriticalHit;

        public static SoundSpec Sound_Sleeping;

        public static SoundSpec Sound_UseFlintlockPistol;

        public static SoundSpec Sound_GunMiss;

        public static SoundSpec Sound_Flamethrower;

        public static SoundSpec Sound_ItemJump;

        public static SoundSpec Sound_Stretch;

        public static SoundSpec Sound_ActorAnimationStep;

        public static SoundSpec Sound_IntroAttention;

        public static SoundSpec Sound_WorldEvent;

        public static SoundSpec Sound_UnlockSkill;

        public static SoundSpec Sound_TelephoneRinging;

        public static SoundSpec Sound_TelephoneHangUp;

        public static SoundSpec Sound_Counter;

        public static SoundSpec Sound_CloseDoor;

        public static SoundSpec Sound_KickDoor;

        public static WindowSpec Window_ContextMenu;

        public static WindowSpec Window_EscapeMenu;

        public static WindowSpec Window_QuestLog;

        public static WindowSpec Window_Options;

        public static WindowSpec Window_Mods;

        public static WindowSpec Window_Confirmation;

        public static WindowSpec Window_Credits;

        public static WindowSpec Window_NewRunWithSeed;

        public static WindowSpec Window_TrainingRoom;

        public static WindowSpec Window_KeyBindings;

        public static WindowSpec Window_Inventory;

        public static WindowSpec Window_Character;

        public static WindowSpec Window_NewRun;

        public static WindowSpec Window_ChangePlayerName;

        public static WindowSpec Window_MemoryPiece;

        public static WindowSpec Window_SingleDialogue;

        public static WindowSpec Window_UnlockedUnlockable;

        public static WindowSpec Window_Skills;

        public static WindowSpec Window_Note;

        public static WindowSpec Window_Stats;

        public static WindowSpec Window_Traits;

        public static WindowSpec Window_Cosmetics;

        public static WindowSpec Window_TutorialPopup;

        public static WindowSpec Window_Achievements;

        public static WindowSpec Window_DailyChallenge;

        public static WindowSpec Window_DailyChallengeLeaderboard;

        public static DialogueSpec Dialogue_Final;

        public static DialogueSpec Dialogue_Wanderer;

        public static DialogueSpec Dialogue_Guardian;

        public static TiledDecalsSpec TiledDecals_Carpet;

        public static TiledDecalsSpec TiledDecals_Slab;

        public static TiledDecalsSpec TiledDecals_Tapestry;

        public static QuestSpec Quest_Introduction;

        public static QuestSpec Quest_RescueLobbyShopkeeper;

        public static QuestSpec Quest_KillSkeletons;

        public static QuestSpec Quest_KillSlimes;

        public static QuestSpec Quest_KillMimics;

        public static QuestSpec Quest_RatInfestation;

        public static QuestSpec Quest_PhantomSwarm;

        public static QuestSpec Quest_MemoryPiece1;

        public static QuestSpec Quest_MemoryPiece2;

        public static QuestSpec Quest_MemoryPiece3;

        public static QuestSpec Quest_MemoryPiece4;

        public static QuestSpec Quest_KillNightmareLord;

        public static QuestSpec Quest_MoonlightMedallions;

        public static QuestSpec Quest_KillHypnorak;

        public static QuestSpec Quest_KillPhantasmos;

        public static QuestSpec Quest_FindRelic;

        public static UnlockableSpec Unlockable_ExtraSpear;

        public static UnlockableSpec Unlockable_ExtraWatch;

        public static UnlockableSpec Unlockable_ExtraShield;

        public static UnlockableSpec Unlockable_PrivateRoom;

        public static UnlockableSpec Unlockable_StardustBoost;

        public static UnlockableSpec Unlockable_GoldBoost;

        public static UnlockableSpec Unlockable_ScoreBoost;

        public static UnlockableSpec Unlockable_ExtraHealthPotion;

        public static UnlockableSpec Unlockable_ExtraScroll;

        public static UnlockableSpec Unlockable_ExtraSword;

        public static UnlockableSpec Unlockable_ExtraAmuletOfYerdon;

        public static UnlockableSpec Unlockable_Hallway;

        public static UnlockableSpec Unlockable_TrainingRoom;

        public static UnlockableSpec Unlockable_TrashPile1;

        public static UnlockableSpec Unlockable_TrashPile2;

        public static UnlockableSpec Unlockable_TrashPile3;

        public static RoomSpec Room_Normal;

        public static RoomSpec Room_Ledge;

        public static RoomSpec Room_PrivateRoom;

        public static RoomSpec Room_OptionalChallengeRoom;

        public static RoomSpec Room_BossRoom;

        public static RoomSpec Room_RewardRoom;

        public static RoomSpec Room_Pit;

        public static IdentificationGroupSpec IdentificationGroup_Potions;

        public static SpeakerSpec Speaker_Player;

        public static DamageTypeSpec DamageType_Physical;

        public static DamageTypeSpec DamageType_Magic;

        public static DamageTypeSpec DamageType_Fire;

        public static DamageTypeSpec DamageType_Poison;

        public static DamageTypeSpec DamageType_Fall;

        public static DamageTypeSpec DamageType_Crush;

        public static DamageTypeSpec DamageType_Bleeding;

        public static DamageTypeSpec DamageType_Starvation;

        public static DamageTypeSpec DamageType_Decay;

        public static DamageTypeSpec DamageType_Explosion;

        public static DamageTypeSpec DamageType_Pacifist;

        public static RoomFeatureSpec RoomFeature_WaterPool;

        public static RoomFeatureSpec RoomFeature_ToxicWaterPool;

        public static RoomFeatureSpec RoomFeature_LavaPool;

        public static RoomFeatureSpec RoomFeature_PoolFilledWithBridges;

        public static RoomFeatureSpec RoomFeature_Spikes;

        public static TitleSpec Title_Noisy;

        public static TitleSpec Title_TargetLevitation;

        public static TitleSpec Title_TargetMaxHP;

        public static TitleSpec Title_Freezing;

        public static TitleSpec Title_Drunkenness;

        public static TitleSpec Title_Entanglement;

        public static TitleSpec Title_Poison;

        public static TitleSpec Title_RandomTeleportation;

        public static TitleSpec Title_Push;

        public static TitleSpec Title_Fire;

        public static TitleSpec Title_Fragility;

        public static TitleSpec Title_Darkness;

        public static TitleSpec Title_Death;

        public static TitleSpec Title_Fatigue;

        public static TitleSpec Title_Life;

        public static TitleSpec Title_Awareness;

        public static TitleSpec Title_Endurance;

        public static TitleSpec Title_FasterIdentification;

        public static TitleSpec Title_FireResistance;

        public static TitleSpec Title_Might;

        public static TitleSpec Title_HPRegen;

        public static TitleSpec Title_ManaRegen;

        public static TitleSpec Title_StaminaRegen;

        public static TitleSpec Title_PoisonImmunity;

        public static TitleSpec Title_FireImmunity;

        public static TitleSpec Title_Hunger;

        public static TitleSpec Title_Levitation;

        public static TitleSpec Title_Strength;

        public static TitleSpec Title_Weakness;

        public static TitleSpec Title_Protection;

        public static TitleSpec Title_Vulnerability;

        public static TitleSpec Title_Childhood;

        public static TitleSpec Title_Adolescence;

        public static TitleSpec Title_Adulthood;

        public static TitleSpec Title_Elderhood;

        public static RunSpec Run_Lobby;

        public static RunSpec Run_Main1;

        public static RunSpec Run_Main2;

        public static RunSpec Run_Main3;

        public static RunSpec Run_Main4;

        public static RunSpec Run_Main5;

        public static RunSpec Run_Main9;

        public static RunSpec Run_Tutorial;

        public static RunSpec Run_Training;

        public static RunSpec Run_Cutscene;

        public static LessonSpec Lesson_Moving;

        public static LessonSpec Lesson_Strafing;

        public static LessonSpec Lesson_Rotating;

        public static LessonSpec Lesson_Waiting;

        public static LessonSpec Lesson_OpeningInventory;

        public static LessonSpec Lesson_Inventory;

        public static LessonSpec Lesson_Inspecting;

        public static LessonSpec Lesson_QuickAttack;

        public static LessonSpec Lesson_BigMap;

        public static LessonSpec Lesson_ClimbingLadders;

        public static LessonSpec Lesson_DrinkingPotion;

        public static LessonSpec Lesson_TargetingBodyParts;

        public static LessonSpec Lesson_Tooltips;

        public static LessonSpec Lesson_OpeningDoors;

        public static LessonSpec Lesson_PickingUpItems;

        public static LessonSpec Lesson_Fighting;

        public static LessonSpec Lesson_Lever;

        public static BodyPartSpec BodyPart_Leg;

        public static DifficultySpec Difficulty_Easy;

        public static DifficultySpec Difficulty_Normal;

        public static DifficultySpec Difficulty_Hard;

        public static ItemSlotSpec ItemSlot_Weapon;

        public static ItemSlotSpec ItemSlot_Armor;

        public static ItemSlotSpec ItemSlot_Ring;

        public static SkillSpec Skill_MoreRawMeat;

        public static SkillSpec Skill_ExplosionProtection;

        public static SkillSpec Skill_NoFallDamage;

        public static SkillSpec Skill_IncreasedMaxHP;

        public static SkillSpec Skill_MoreHealthFromFood;

        public static SkillSpec Skill_FasterHealing;

        public static SkillSpec Skill_MoreTraps;

        public static SkillSpec Skill_BodyPartsDamage;

        public static SkillSpec Skill_SurroundedBonus;

        public static SkillSpec Skill_EntangleOnAttacked;

        public static SkillSpec Skill_ShieldDeflect;

        public static SkillSpec Skill_ShieldStamina;

        public static SkillSpec Skill_Lockpicking;

        public static SkillSpec Skill_LowerPrices;

        public static SkillSpec Skill_SlowerHunger;

        public static SkillSpec Skill_IncreasedMaxStamina;

        public static SkillSpec Skill_IncreasedMaxMana;

        public static SkillSpec Skill_FasterStaminaRegen;

        public static FactionSpec Faction_Monsters;

        public static FactionSpec Faction_Cult;

        public static FactionSpec Faction_Guardians;

        public static MainTabSpec MainTab_Inventory;

        public static MainTabSpec MainTab_Skills;

        public static MainTabSpec MainTab_Factions;

        public static MainTabSpec MainTab_DungeonMap;

        public static MainTabSpec MainTab_Quests;

        public static PlaceSpec Place_Normal;

        public static PlaceSpec Place_GoogonsLair;

        public static PlaceSpec Place_DemonsLair;

        public static PlaceSpec Place_Shelter;

        public static TraitSpec Trait_LastStand;

        public static TraitSpec Trait_NightOwl;

        public static TraitSpec Trait_BossHunter;

        public static TraitSpec Trait_AllIn;

        public static TraitSpec Trait_Vampire;

        public static TraitSpec Trait_Berserker;

        public static TraitSpec Trait_Scary;

        public static ClassSpec Class_Rogue;

        public static ClassSpec Class_Necromancer;

        public static ClassSpec Class_Imposter;

        public static ClassSpec Class_Bogwalker;

        public static AISpec AI_Standard;

        public static AISpec AI_PlayerFollower;

        public static AISpec AI_PartyMember;

        public static AISpec AI_PartyMember_Stay;

        public static AISpec AI_StandNearCultistCircleIfUnprovoked;

        public static GenericUsableSpec GenericUsable_TalkWithWanderer;

        public static GenericUsableSpec GenericUsable_TalkWithWanderer2;

        public static AchievementSpec Achievement_FinishTutorial;

        public static AchievementSpec Achievement_FinishFirstDungeon;

        public static AchievementSpec Achievement_KillGoogon;

        public static AchievementSpec Achievement_KillDemon;

        public static AchievementSpec Achievement_FinishGame;

        public static AchievementSpec Achievement_CollectAllPuzzlePieces;

        public static AchievementSpec Achievement_Sacrifice;

        public static AchievementSpec Achievement_SetSpiritFree;

        public static AchievementSpec Achievement_CompleteTrial;

        public static AchievementSpec Achievement_SacrificeYourself;

        public static AchievementSpec Achievement_FindSecretRoom;

        public static AchievementSpec Achievement_GiveHolyWaterToCultists;

        public static AchievementSpec Achievement_KillShopkeeper;

        public static AchievementSpec Achievement_KillGhost;

        public static AchievementSpec Achievement_JumpIntoPit;

        public static AchievementSpec Achievement_ReachThirdFloorWithoutLosingHP;

        public static AchievementSpec Achievement_ReachFifthFloorWithoutLosingHP;

        public static AchievementSpec Achievement_BrewHealthPotion;

        public static AchievementSpec Achievement_KillEnemyWithCeilingSpikes;

        public static AchievementSpec Achievement_BuyPrivateRoom;

        public static AchievementSpec Achievement_UnlockAllTraits;

        public static AchievementSpec Achievement_ReachSkillTreeTop;

        public static TipsSpec Tips_Interface;

        public static ItemLookSpec ItemLook_ShelterHealthPotion;

        public static ItemLookSpec ItemLook_ShelterCurseRemovalScroll;

        public static ItemLookSpec ItemLook_ShelterIdentificationScroll;

        public static CosmeticSpec Cosmetic_DevilHorns;

        public static DungeonModifierSpec DungeonModifier_NoRandomEvents;

        public static DungeonModifierSpec DungeonModifier_NoMiniBosses;

        public static DungeonModifierSpec DungeonModifier_ExtraMiniBoss;

        public static DungeonModifierSpec DungeonModifier_NoShelters;

        public static DungeonModifierSpec DungeonModifier_MoreEnemies;

        public static DungeonModifierSpec DungeonModifier_FewerEnemies;

        public static DungeonModifierSpec DungeonModifier_ItemsStartIdentified;

        public static DungeonModifierSpec DungeonModifier_NoHunger;

        public static DungeonModifierSpec DungeonModifier_EveryoneStartsAwake;

        public static DungeonModifierSpec DungeonModifier_MoreTraps;

        public static SpellSpec Spell_JumpAbility;
    }
}