using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class RunConfig : ISaveableEventsReceiver
    {
        public RunSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public int RunSeed
        {
            get
            {
                return this.runSeed;
            }
        }

        public DifficultySpec Difficulty
        {
            get
            {
                return this.difficulty;
            }
        }

        public ClassSpec PlayerClass
        {
            get
            {
                return this.playerClass;
            }
        }

        public string SavefileName
        {
            get
            {
                return this.savefileName;
            }
        }

        public List<DungeonModifierSpec> DungeonModifiers
        {
            get
            {
                return this.dungeonModifiers;
            }
        }

        public bool ProgressDisabled
        {
            get
            {
                return this.progressDisabled;
            }
        }

        public EntitySpec TrainingActorSpec
        {
            get
            {
                return this.trainingActorSpec;
            }
        }

        public bool TrainingBoss
        {
            get
            {
                return this.trainingBoss;
            }
        }

        public bool HasPetRat
        {
            get
            {
                return this.hasPetRat;
            }
        }

        public string DailyChallengeDate
        {
            get
            {
                return this.dailyChallengeDate;
            }
        }

        public bool IsDailyChallenge
        {
            get
            {
                return !this.dailyChallengeDate.NullOrEmpty();
            }
        }

        public ChooseablePartyMemberSpec PartyMember
        {
            get
            {
                return this.partyMember;
            }
        }

        public bool HasPartyMember
        {
            get
            {
                return this.PartyMember != null;
            }
        }

        public RunConfig()
        {
        }

        public RunConfig(RunSpec spec, int runSeed, DifficultySpec difficulty, ClassSpec playerClass, string savefileName, List<DungeonModifierSpec> dungeonModifiers, bool progressDisabled = false, EntitySpec trainingActorSpec = null, bool trainingBoss = false, bool hasPetRat = false, string dailyChallengeDate = null, ChooseablePartyMemberSpec partyMember = null)
        {
            this.spec = spec;
            this.runSeed = runSeed;
            this.difficulty = difficulty;
            this.playerClass = playerClass;
            this.savefileName = savefileName;
            this.dungeonModifiers = ((dungeonModifiers != null) ? dungeonModifiers.ToList<DungeonModifierSpec>() : null);
            this.progressDisabled = progressDisabled;
            this.trainingActorSpec = trainingActorSpec;
            this.trainingBoss = trainingBoss;
            this.hasPetRat = hasPetRat;
            this.dailyChallengeDate = dailyChallengeDate;
            this.partyMember = partyMember;
        }

        public RunConfig CopyWithSavefileNameAndSeed(string savefileName, int runSeed, bool hasPetRat)
        {
            return new RunConfig(this.spec, runSeed, this.difficulty, this.playerClass, savefileName, this.dungeonModifiers, this.progressDisabled, this.trainingActorSpec, this.trainingBoss, hasPetRat, null, this.partyMember);
        }

        public void CheckUpdateSavefileName(string loadedFrom)
        {
            if (this.savefileName != loadedFrom)
            {
                Log.Message(string.Concat(new string[] { "Updating RunInfo.SavefileName from \"", this.savefileName, "\" to \"", loadedFrom, "\"." }));
                this.savefileName = loadedFrom;
            }
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            if (this.spec == null)
            {
                Log.Error("Run spec was null after loading.", false);
                this.spec = Get.Run_Main1;
            }
            if (this.difficulty == null)
            {
                Log.Error("Difficulty was null after loading.", false);
                this.difficulty = Get.Difficulty_Normal;
            }
        }

        [Saved]
        private RunSpec spec;

        [Saved]
        private int runSeed;

        [Saved]
        private DifficultySpec difficulty;

        [Saved]
        private ClassSpec playerClass;

        [Saved]
        private string savefileName;

        [Saved]
        private List<DungeonModifierSpec> dungeonModifiers;

        [Saved]
        private bool progressDisabled;

        [Saved]
        private EntitySpec trainingActorSpec;

        [Saved]
        private bool trainingBoss;

        [Saved]
        private bool hasPetRat;

        [Saved]
        private string dailyChallengeDate;

        [Saved]
        private ChooseablePartyMemberSpec partyMember;
    }
}