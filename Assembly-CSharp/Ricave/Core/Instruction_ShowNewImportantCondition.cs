using System;

namespace Ricave.Core
{
    public class Instruction_ShowNewImportantCondition : Instruction
    {
        public Condition Condition
        {
            get
            {
                return this.condition;
            }
        }

        protected Instruction_ShowNewImportantCondition()
        {
        }

        public Instruction_ShowNewImportantCondition(Condition condition)
        {
            this.condition = condition;
        }

        protected override void DoImpl()
        {
            this.importantConditionID = Get.NewImportantConditionsUI.Add(this.condition);
        }

        protected override void UndoImpl()
        {
            Get.NewImportantConditionsUI.TryRemove(this.importantConditionID);
        }

        [Saved]
        private Condition condition;

        [Saved]
        private int importantConditionID;
    }
}