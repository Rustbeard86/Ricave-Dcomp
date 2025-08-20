using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ricave.Rendering;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class Progress
    {
        public string PlayerName
        {
            get
            {
                return this.playerName;
            }
            set
            {
                if (this.playerName == value)
                {
                    return;
                }
                this.playerName = value;
                DialogueDrawer.OnPlayerNameChanged();
            }
        }

        public DifficultySpec LastChosenDifficulty
        {
            get
            {
                return this.lastChosenDifficulty ?? Get.Difficulty_Normal;
            }
            set
            {
                this.lastChosenDifficulty = value;
            }
        }

        public ClassSpec LastChosenClass
        {
            get
            {
                return this.lastChosenClass;
            }
            set
            {
                this.lastChosenClass = value;
            }
        }

        public ChooseablePartyMemberSpec LastChosenPartyMember
        {
            get
            {
                return this.lastChosenPartyMember;
            }
            set
            {
                this.lastChosenPartyMember = value;
            }
        }

        public int TotalStardustReceivedFromQuests
        {
            get
            {
                return this.totalStardustReceivedFromQuests;
            }
        }

        public TotalLobbyItems TotalLobbyItems
        {
            get
            {
                return this.totalLobbyItems;
            }
        }

        public QuestManager QuestManager
        {
            get
            {
                return this.questManager;
            }
        }

        public UnlockableManager UnlockableManager
        {
            get
            {
                return this.unlockableManager;
            }
        }

        public TraitManager TraitManager
        {
            get
            {
                return this.traitManager;
            }
        }

        public CosmeticsManager CosmeticsManager
        {
            get
            {
                return this.cosmeticsManager;
            }
        }

        public PrivateRoom PrivateRoom
        {
            get
            {
                return this.privateRoom;
            }
        }

        public DialoguesManager DialoguesManager
        {
            get
            {
                return this.dialoguesManager;
            }
        }

        public TotalKillCounter TotalKillCounter
        {
            get
            {
                return this.totalKillCounter;
            }
        }

        public TotalQuestsState TotalQuestsState
        {
            get
            {
                return this.totalQuestsState;
            }
        }

        public LessonManager LessonManager
        {
            get
            {
                return this.lessonManager;
            }
        }

        public Vector3Int? PreferredRespawnPos
        {
            get
            {
                return this.preferredRespawnPos;
            }
        }

        public Vector3Int? PreferredRespawnDir
        {
            get
            {
                return this.preferredRespawnDir;
            }
        }

        public int Score
        {
            get
            {
                return this.score;
            }
        }

        public int ScoreLevel
        {
            get
            {
                return ScoreLevelUtility.GetLevel(this.score);
            }
        }

        public int ScoreBetweenLevels
        {
            get
            {
                return ScoreLevelUtility.GetTotalScoreRequired(this.ScoreLevel + 1) - ScoreLevelUtility.GetTotalScoreRequired(this.ScoreLevel);
            }
        }

        public int ScoreSinceLeveling
        {
            get
            {
                return ScoreLevelUtility.GetScoreSinceLeveling(this.score);
            }
        }

        public int ScoreToNextLevel
        {
            get
            {
                return ScoreLevelUtility.GetTotalScoreRequired(this.ScoreLevel + 1) - this.score;
            }
        }

        public HashSet<EntitySpec> SeenActorSpecs
        {
            get
            {
                return this.seenActorSpecs;
            }
        }

        public HashSet<EntitySpec> SeenItemSpecs
        {
            get
            {
                return this.seenItemSpecs;
            }
        }

        public HashSet<NoteSpec> CollectedNoteSpecs
        {
            get
            {
                return this.collectedNoteSpecs;
            }
        }

        public bool FinishedGame
        {
            get
            {
                return this.finishedGame;
            }
        }

        public bool EverUsedWatch
        {
            get
            {
                return this.everUsedWatch;
            }
        }

        public bool EverInspectedAnythingInMainRun
        {
            get
            {
                return this.everInspectedAnythingInMainRun;
            }
        }

        public bool EverShownStardustInfo
        {
            get
            {
                return this.everShownStardustInfo;
            }
        }

        public bool EverShownPuzzlePieceInfo
        {
            get
            {
                return this.everShownPuzzlePieceInfo;
            }
        }

        public int NextRunRetainedExp
        {
            get
            {
                return this.nextRunRetainedExp;
            }
        }

        public int NextRunRetainedGold
        {
            get
            {
                return this.nextRunRetainedGold;
            }
        }

        public bool FileExistsButCouldNotRead
        {
            get
            {
                return this.fileExistsButCouldNotRead;
            }
        }

        public string LoadedVersion
        {
            get
            {
                return this.loadedVersion;
            }
        }

        public bool TextOnFirstTimeFloorReached1Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached1Shown;
            }
        }

        public bool TextOnFirstTimeFloorReached2Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached2Shown;
            }
        }

        public bool TextOnFirstTimeFloorReached3Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached3Shown;
            }
        }

        public bool TextOnFirstTimeFloorReached4Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached4Shown;
            }
        }

        public bool TextOnFirstTimeFloorReached5Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached5Shown;
            }
        }

        public bool TextOnFirstTimeFloorReached6Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached6Shown;
            }
        }

        public bool TextOnFirstTimeFloorReached7Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached7Shown;
            }
        }

        public bool TextAfterFirstDungeonCompletedShown
        {
            get
            {
                return this.textAfterFirstDungeonCompletedShown;
            }
            set
            {
                this.textAfterFirstDungeonCompletedShown = value;
            }
        }

        public bool DemonNotePickedUp
        {
            get
            {
                return this.demonNotePickedUp;
            }
            set
            {
                this.demonNotePickedUp = value;
            }
        }

        public int PetRatSatiation
        {
            get
            {
                return this.petRatSatiation;
            }
            set
            {
                this.petRatSatiation = value;
            }
        }

        public int? LastDailyChallengeRunSeed
        {
            get
            {
                return this.lastDailyChallengeRunSeed;
            }
        }

        public int LastDailyChallengeScore
        {
            get
            {
                return this.lastDailyChallengeScore;
            }
        }

        public int AllDailyChallengesBestScore
        {
            get
            {
                return this.allDailyChallengesBestScore;
            }
        }

        public int MaxFloorReachedWithoutClass
        {
            get
            {
                return this.maxFloorReachedWithoutClass;
            }
        }

        public List<RunSpec> MainDungeonsInOrder
        {
            get
            {
                if (this.mainDungeonsInOrder == null)
                {
                    this.mainDungeonsInOrder = (from x in Get.Specs.GetAll<RunSpec>()
                                                where x.IsMain
                                                orderby x.SpecID
                                                select x).ToList<RunSpec>();
                }
                return this.mainDungeonsInOrder;
            }
        }

        public float AverageFloorReached
        {
            get
            {
                if (this.Runs != 0)
                {
                    return (float)this.FloorReachedSum / (float)this.Runs;
                }
                return 0f;
            }
        }

        public double TotalPlaytime
        {
            get
            {
                double num = 0.0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    num += keyValuePair.Value.TotalPlaytime;
                }
                return num;
            }
        }

        public int MaxFloorReached
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    num = Math.Max(num, keyValuePair.Value.MaxFloorReached);
                }
                return num;
            }
        }

        public int MaxMainFloorReached
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    if (keyValuePair.Key.IsMain)
                    {
                        num = Math.Max(num, keyValuePair.Value.MaxFloorReached);
                    }
                }
                return num;
            }
        }

        public int FloorReachedSum
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    num += keyValuePair.Value.FloorReachedSum;
                }
                return num;
            }
        }

        public int Runs
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    num += keyValuePair.Value.Runs;
                }
                return num;
            }
        }

        public int MainRuns
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    if (keyValuePair.Key.IsMain)
                    {
                        num += keyValuePair.Value.Runs;
                    }
                }
                return num;
            }
        }

        public int TotalStardustCollected
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    num += keyValuePair.Value.TotalStardustCollected;
                }
                return num;
            }
        }

        public int TotalChestsOpened
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    num += keyValuePair.Value.ChestsOpened;
                }
                return num;
            }
        }

        public int TotalSpiritsSetFree
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    num += keyValuePair.Value.TotalSpiritsSetFree;
                }
                return num;
            }
        }

        public int TotalRecipesUsed
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    num += keyValuePair.Value.RecipesUsed;
                }
                return num;
            }
        }

        public int MaxScore
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    num = Math.Max(num, keyValuePair.Value.MaxScore);
                }
                return num;
            }
        }

        public int MaxMainScore
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    if (keyValuePair.Key.IsMain)
                    {
                        num = Math.Max(num, keyValuePair.Value.MaxScore);
                    }
                }
                return num;
            }
        }

        public int MaxKillCount
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    num = Math.Max(num, keyValuePair.Value.MaxKillCount);
                }
                return num;
            }
        }

        public int KillCount
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    num += keyValuePair.Value.KillCount;
                }
                return num;
            }
        }

        public int MainKillCount
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<RunSpec, ProgressPerRunSpecStats> keyValuePair in this.stats)
                {
                    if (keyValuePair.Key.IsMain)
                    {
                        num += keyValuePair.Value.KillCount;
                    }
                }
                return num;
            }
        }

        public void ApplyCurrentRunProgress()
        {
            if (Get.InMainMenu)
            {
                Log.Error("Tried to apply run progress in main menu.", false);
                return;
            }
            if (Get.RunConfig.IsDailyChallenge)
            {
                this.lastDailyChallengeRunSeed = new int?(Get.RunConfig.RunSeed);
                this.lastDailyChallengeScore = Get.Player.Score;
                this.allDailyChallengesBestScore = Math.Max(this.allDailyChallengesBestScore, Get.Player.Score);
                this.totalLobbyItems.ChangeCount(Get.Entity_Stardust, 20);
                DailyChallengeUtility.UploadScore(Get.Player.Score, Get.RunConfig.DailyChallengeDate, Get.RunConfig.RunSeed);
                return;
            }
            if (Get.RunConfig.ProgressDisabled)
            {
                return;
            }
            if (Get.InLobby)
            {
                Log.Error("Tried to apply run progress while in lobby.", false);
                return;
            }
            if (Get.RunConfig.SavefileName != "Current")
            {
                Log.Error("Tried to apply progress for a custom savefile with a custom name. This is not allowed to make testing easier. Otherwise it'd be possible to load the same file and apply the same progress again and again.", false);
                return;
            }
            if (Get.RunInfo.ProgressApplied)
            {
                Log.Error("Tried to apply progress for the same run twice.", false);
                return;
            }
            if (Get.RunInfo.GlobalRandomID == this.lastAppliedRunID)
            {
                Log.Error("Tried to apply progress for the same run twice (the run didn't have its ProgressApplied flag set, so most likely the savefile has been copied manually).", false);
                return;
            }
            Progress.applying = true;
            try
            {
                ProgressPerRunSpecStats progressPerRunSpecStats;
                if (!this.stats.TryGetValue(Get.RunSpec, out progressPerRunSpecStats))
                {
                    progressPerRunSpecStats = new ProgressPerRunSpecStats();
                    this.stats.Add(Get.RunSpec, progressPerRunSpecStats);
                }
                progressPerRunSpecStats.ApplyCurrentRunProgress();
                if (!Get.RunInfo.ReturnedToLobby && !Get.RunInfo.FinishedRun)
                {
                    foreach (UnlockableSpec unlockableSpec in Get.UnlockableManager.DirectlyUnlocked.ToTemporaryList<UnlockableSpec>())
                    {
                        if (unlockableSpec.ResetAfterRun)
                        {
                            Get.UnlockableManager.RemoveFromDirectlyUnlocked(unlockableSpec);
                        }
                    }
                }
                if (Get.RunInfo.ReturnedToLobby)
                {
                    this.nextRunRetainedExp = Calc.RoundToIntHalfUp((float)Get.Player.Experience * 0.25f);
                    this.nextRunRetainedGold = Calc.RoundToIntHalfUp((float)Get.Player.Gold * 0.5f);
                }
                else
                {
                    this.nextRunRetainedExp = 0;
                    this.nextRunRetainedGold = 0;
                }
                this.score += Get.Player.Score;
                this.totalKillCounter.Apply(Get.KillCounter);
                this.totalLobbyItems.Apply(Get.ThisRunLobbyItems);
                this.privateRoom.Apply(Get.ThisRunPrivateRoomStructures);
                this.totalQuestsState.Apply(Get.ThisRunQuestsState);
                this.questManager.Apply(Get.ThisRunCompletedQuests);
                this.traitManager.ApplyRunStats(Get.KillCounter);
                this.seenActorSpecs.AddRange<EntitySpec>(Get.Player.SeenActorSpecs);
                this.seenItemSpecs.AddRange<EntitySpec>(Get.Player.SeenItemSpecs);
                this.collectedNoteSpecs.AddRange<NoteSpec>(Get.Player.CollectedNoteSpecs);
                this.lastAppliedRunID = Get.RunInfo.GlobalRandomID;
                this.everUsedWatch = this.everUsedWatch || Get.Player.UsedWatch;
                this.everInspectedAnythingInMainRun = this.everInspectedAnythingInMainRun || (Get.Player.InspectedAnything && Get.RunSpec.IsMain);
                this.everShownStardustInfo = this.everShownStardustInfo || Get.Player.ShownStardustInfo;
                this.everShownPuzzlePieceInfo = this.everShownPuzzlePieceInfo || Get.Player.ShownPuzzlePieceInfo;
                this.textOnFirstTimeFloorReached1Shown = this.textOnFirstTimeFloorReached1Shown || Get.RunInfo.TextOnFirstTimeFloorReached1Shown;
                this.textOnFirstTimeFloorReached2Shown = this.textOnFirstTimeFloorReached2Shown || Get.RunInfo.TextOnFirstTimeFloorReached2Shown;
                this.textOnFirstTimeFloorReached3Shown = this.textOnFirstTimeFloorReached3Shown || Get.RunInfo.TextOnFirstTimeFloorReached3Shown;
                this.textOnFirstTimeFloorReached4Shown = this.textOnFirstTimeFloorReached4Shown || Get.RunInfo.TextOnFirstTimeFloorReached4Shown;
                this.textOnFirstTimeFloorReached5Shown = this.textOnFirstTimeFloorReached5Shown || Get.RunInfo.TextOnFirstTimeFloorReached5Shown;
                this.textOnFirstTimeFloorReached6Shown = this.textOnFirstTimeFloorReached6Shown || Get.RunInfo.TextOnFirstTimeFloorReached6Shown;
                this.textOnFirstTimeFloorReached7Shown = this.textOnFirstTimeFloorReached7Shown || Get.RunInfo.TextOnFirstTimeFloorReached7Shown;
                this.textAfterFirstDungeonCompletedShown = this.textAfterFirstDungeonCompletedShown || Get.RunInfo.TextAfterFirstDungeonCompletedShown;
                this.ApplyClassStats();
                Get.RunInfo.ProgressApplied = true;
                this.Save();
                Get.Run.DeleteSavefile();
            }
            finally
            {
                Progress.applying = false;
            }
        }

        public void Save()
        {
            SaveLoadManager.Save(this, FilePaths.Progress, "Progress");
        }

        public void TryLoad()
        {
            if (File.Exists(FilePaths.Progress))
            {
                this.fileExistsButCouldNotRead = !SaveLoadManager.Load(this, FilePaths.Progress, "Progress", out this.loadedVersion);
                if (this.completedRuns != null)
                {
                    foreach (RunSpec runSpec in this.completedRuns)
                    {
                        ProgressPerRunSpecStats progressPerRunSpecStats;
                        if (runSpec != null && this.stats.TryGetValue(runSpec, out progressPerRunSpecStats))
                        {
                            progressPerRunSpecStats.OnBackCompatSetCompleted();
                        }
                    }
                    this.completedRuns = null;
                    return;
                }
            }
            else
            {
                this.fileExistsButCouldNotRead = false;
                this.loadedVersion = App.Version;
            }
        }

        public void OnPressedPlayInMainMenu()
        {
            this.loadedVersion = App.Version;
        }

        public void OnQuestRewardClaimed(QuestSpec questSpec)
        {
            this.totalLobbyItems.ChangeCount(Get.Entity_Stardust, questSpec.StardustReward);
            this.totalStardustReceivedFromQuests += questSpec.StardustReward;
        }

        public T GetOrCreateModState<T>(string modId) where T : ModStatePerProgress, new()
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

        public void ResetPreferredRespawn()
        {
            this.preferredRespawnPos = null;
            this.preferredRespawnDir = null;
        }

        public void SetPreferredRespawn(Vector3Int pos, Vector3Int dir)
        {
            this.preferredRespawnPos = new Vector3Int?(pos);
            this.preferredRespawnDir = new Vector3Int?(dir);
        }

        public ProgressPerRunSpecStats GetRunStats(RunSpec spec)
        {
            ProgressPerRunSpecStats progressPerRunSpecStats;
            if (!this.stats.TryGetValue(spec, out progressPerRunSpecStats))
            {
                progressPerRunSpecStats = new ProgressPerRunSpecStats();
                this.stats.Add(spec, progressPerRunSpecStats);
            }
            return progressPerRunSpecStats;
        }

        public static void ThrowIfNotInLobby()
        {
            if (!Progress.applying && !Get.InLobby)
            {
                throw new Exception("This method changes progress state that can only be changed from lobby.");
            }
        }

        public void OnFinishedGame()
        {
            this.finishedGame = true;
        }

        public bool IsRunCompleted(RunSpec spec)
        {
            ProgressPerRunSpecStats progressPerRunSpecStats;
            return this.stats.TryGetValue(spec, out progressPerRunSpecStats) && progressPerRunSpecStats.Completed;
        }

        public DifficultySpec GetHighestCompletedDifficulty(RunSpec spec)
        {
            ProgressPerRunSpecStats progressPerRunSpecStats;
            if (!this.stats.TryGetValue(spec, out progressPerRunSpecStats))
            {
                return null;
            }
            return progressPerRunSpecStats.CompletedHighestDifficulty;
        }

        private void ApplyClassStats()
        {
            ClassSpec playerClass = Get.RunConfig.PlayerClass;
            if (playerClass != null)
            {
                int num;
                if (!this.maxFloorReachedWithClass.TryGetValue(playerClass, out num) || Get.Floor > num)
                {
                    this.maxFloorReachedWithClass[playerClass] = Get.Floor;
                    return;
                }
            }
            else if (Get.Floor > this.maxFloorReachedWithoutClass)
            {
                this.maxFloorReachedWithoutClass = Get.Floor;
            }
        }

        public int GetMaxFloorReachedWithClass(ClassSpec classSpec)
        {
            return this.maxFloorReachedWithClass.GetOrDefault(classSpec, 0);
        }

        [Saved("Nick", false)]
        private string playerName = "Nick";

        [Saved]
        private DifficultySpec lastChosenDifficulty;

        [Saved]
        private ClassSpec lastChosenClass;

        [Saved]
        private ChooseablePartyMemberSpec lastChosenPartyMember;

        [Saved]
        private int lastAppliedRunID;

        [Saved]
        private Vector3Int? preferredRespawnPos;

        [Saved]
        private Vector3Int? preferredRespawnDir;

        [Saved(Default.New, true)]
        private Dictionary<RunSpec, ProgressPerRunSpecStats> stats = new Dictionary<RunSpec, ProgressPerRunSpecStats>();

        [Saved]
        private int totalStardustReceivedFromQuests;

        [Saved]
        private int score;

        [Saved]
        private bool finishedGame;

        [Saved]
        private bool everUsedWatch;

        [Saved]
        private bool everInspectedAnythingInMainRun;

        [Saved]
        private bool everShownStardustInfo;

        [Saved]
        private bool everShownPuzzlePieceInfo;

        [Saved]
        private int nextRunRetainedExp;

        [Saved]
        private int nextRunRetainedGold;

        [Saved(Default.New, false)]
        private TotalLobbyItems totalLobbyItems = new TotalLobbyItems();

        [Saved(Default.New, false)]
        private QuestManager questManager = new QuestManager();

        [Saved(Default.New, false)]
        private UnlockableManager unlockableManager = new UnlockableManager();

        [Saved(Default.New, false)]
        private PrivateRoom privateRoom = new PrivateRoom();

        [Saved(Default.New, false)]
        private DialoguesManager dialoguesManager = new DialoguesManager();

        [Saved(Default.New, false)]
        private TotalKillCounter totalKillCounter = new TotalKillCounter();

        [Saved(Default.New, false)]
        private TotalQuestsState totalQuestsState = new TotalQuestsState();

        [Saved(Default.New, false)]
        private LessonManager lessonManager = new LessonManager();

        [Saved(Default.New, false)]
        private TraitManager traitManager = new TraitManager();

        [Saved(Default.New, false)]
        private CosmeticsManager cosmeticsManager = new CosmeticsManager();

        [Saved(Default.New, true)]
        private List<ModStatePerProgress> modsState = new List<ModStatePerProgress>();

        [Saved(Default.New, true)]
        private HashSet<EntitySpec> seenActorSpecs = new HashSet<EntitySpec>();

        [Saved(Default.New, true)]
        private HashSet<EntitySpec> seenItemSpecs = new HashSet<EntitySpec>();

        [Saved(Default.New, true)]
        private HashSet<NoteSpec> collectedNoteSpecs = new HashSet<NoteSpec>();

        [Saved]
        private bool textOnFirstTimeFloorReached1Shown;

        [Saved]
        private bool textOnFirstTimeFloorReached2Shown;

        [Saved]
        private bool textOnFirstTimeFloorReached3Shown;

        [Saved]
        private bool textOnFirstTimeFloorReached4Shown;

        [Saved]
        private bool textOnFirstTimeFloorReached5Shown;

        [Saved]
        private bool textOnFirstTimeFloorReached6Shown;

        [Saved]
        private bool textOnFirstTimeFloorReached7Shown;

        [Saved]
        private bool textAfterFirstDungeonCompletedShown;

        [Saved]
        private List<RunSpec> completedRuns;

        [Saved]
        private bool demonNotePickedUp;

        [Saved]
        private int petRatSatiation;

        [Saved]
        private int? lastDailyChallengeRunSeed;

        [Saved]
        private int lastDailyChallengeScore;

        [Saved]
        private int allDailyChallengesBestScore;

        [Saved(Default.New, false)]
        private Dictionary<ClassSpec, int> maxFloorReachedWithClass = new Dictionary<ClassSpec, int>();

        [Saved]
        private int maxFloorReachedWithoutClass;

        private bool fileExistsButCouldNotRead;

        private string loadedVersion;

        private static bool applying;

        private List<RunSpec> mainDungeonsInOrder;

        public const int TotalPuzzlePieces = 10;
    }
}