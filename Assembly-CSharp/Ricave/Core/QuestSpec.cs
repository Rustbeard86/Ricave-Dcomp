using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class QuestSpec : Spec
    {
        public DialogueSpec Dialogue
        {
            get
            {
                return this.dialogue;
            }
        }

        public int StardustReward
        {
            get
            {
                return this.stardustReward;
            }
        }

        public bool Visible
        {
            get
            {
                return this.prerequisite == null || this.prerequisite.IsCompletedAndClaimed();
            }
        }

        public string LongDescription
        {
            get
            {
                return this.longDescription ?? base.Description;
            }
        }

        public List<string> DungeonModifiers
        {
            get
            {
                return this.dungeonModifiers;
            }
        }

        public List<string> ExtraRewards
        {
            get
            {
                return this.extraRewards;
            }
        }

        public bool NoDialogueOrEnded
        {
            get
            {
                return this.dialogue == null || this.dialogue.Ended;
            }
        }

        public string ExtraLabelFormat
        {
            get
            {
                return this.extraLabelFormat;
            }
        }

        public bool IsMainQuestline
        {
            get
            {
                QuestSpec quest_MemoryPiece = Get.Quest_MemoryPiece4;
                int num = 0;
                while (quest_MemoryPiece != null)
                {
                    num++;
                    if (num > 1000)
                    {
                        Log.Error("Too many iterations.", false);
                        break;
                    }
                    if (quest_MemoryPiece == this)
                    {
                        return true;
                    }
                    quest_MemoryPiece = quest_MemoryPiece.prerequisite;
                }
                return false;
            }
        }

        [Saved]
        private DialogueSpec dialogue;

        [Saved]
        private int stardustReward;

        [Saved]
        private QuestSpec prerequisite;

        [Saved]
        [Translatable]
        private string longDescription;

        [Saved(Default.New, false)]
        [Translatable]
        private List<string> dungeonModifiers = new List<string>();

        [Saved(Default.New, false)]
        [Translatable]
        private List<string> extraRewards = new List<string>();

        [Saved]
        [Translatable]
        private string extraLabelFormat;
    }
}