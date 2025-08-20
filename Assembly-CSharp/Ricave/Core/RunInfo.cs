using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class RunInfo : ISaveableEventsReceiver
    {
        public RunConfig Config
        {
            get
            {
                return this.config;
            }
        }

        public int GlobalRandomID
        {
            get
            {
                return this.globalRandomId;
            }
        }

        public List<string> UsedNames
        {
            get
            {
                return this.usedNames;
            }
        }

        public List<string> UsedGhostNames
        {
            get
            {
                return this.usedGhostNames;
            }
        }

        public bool ProgressApplied
        {
            get
            {
                return this.progressApplied;
            }
            set
            {
                this.progressApplied = value;
            }
        }

        public bool ReturnedToLobby
        {
            get
            {
                return this.returnedToLobby;
            }
            set
            {
                this.returnedToLobby = value;
            }
        }

        public bool FinishedRun
        {
            get
            {
                return this.finishedRun;
            }
            set
            {
                this.finishedRun = value;
            }
        }

        public bool TextOnFirstTimeFloorReached1Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached1Shown;
            }
            set
            {
                this.textOnFirstTimeFloorReached1Shown = value;
            }
        }

        public bool TextOnFirstTimeFloorReached2Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached2Shown;
            }
            set
            {
                this.textOnFirstTimeFloorReached2Shown = value;
            }
        }

        public bool TextOnFirstTimeFloorReached3Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached3Shown;
            }
            set
            {
                this.textOnFirstTimeFloorReached3Shown = value;
            }
        }

        public bool TextOnFirstTimeFloorReached4Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached4Shown;
            }
            set
            {
                this.textOnFirstTimeFloorReached4Shown = value;
            }
        }

        public bool TextOnFirstTimeFloorReached5Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached5Shown;
            }
            set
            {
                this.textOnFirstTimeFloorReached5Shown = value;
            }
        }

        public bool TextOnFirstTimeFloorReached6Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached6Shown;
            }
            set
            {
                this.textOnFirstTimeFloorReached6Shown = value;
            }
        }

        public bool TextOnFirstTimeFloorReached7Shown
        {
            get
            {
                return this.textOnFirstTimeFloorReached7Shown;
            }
            set
            {
                this.textOnFirstTimeFloorReached7Shown = value;
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

        public bool GeneratedAnvil
        {
            get
            {
                return this.generatedAnvil;
            }
            set
            {
                this.generatedAnvil = value;
            }
        }

        public bool GeneratedBeggar
        {
            get
            {
                return this.generatedBeggar;
            }
            set
            {
                this.generatedBeggar = value;
            }
        }

        public RunInfo()
        {
        }

        public RunInfo(RunConfig config)
        {
            this.globalRandomId = Rand.Int;
            this.config = config;
        }

        public void Update()
        {
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            if (this.config == null)
            {
                Log.Error("Run config was null after loading.", false);
                this.config = new RunConfig(Get.Run_Main1, 0, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null);
            }
        }

        [Saved]
        private RunConfig config;

        [Saved]
        private int globalRandomId;

        [Saved]
        private bool progressApplied;

        [Saved(Default.New, true)]
        private List<string> usedNames = new List<string>();

        [Saved(Default.New, true)]
        private List<string> usedGhostNames = new List<string>();

        [Saved]
        private bool returnedToLobby;

        [Saved]
        private bool finishedRun;

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
        private bool generatedAnvil;

        [Saved]
        private bool generatedBeggar;
    }
}