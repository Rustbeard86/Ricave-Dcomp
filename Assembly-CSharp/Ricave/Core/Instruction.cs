using System;

namespace Ricave.Core
{
    public abstract class Instruction
    {
        public virtual bool CausesRewindPoint
        {
            get
            {
                return false;
            }
        }

        private int RandState
        {
            get
            {
                return Calc.CombineHashes<int, int, int>(Rand.StateCompressed, base.GetType().TypeNameStableHashCode(), 741906221);
            }
        }

        protected abstract void DoImpl();

        protected abstract void UndoImpl();

        public void Do()
        {
            if (Instruction.executing)
            {
                Log.Error("Tried to execute an instruction but another instruction execution is in progress. Nested Do() calls are not allowed.", false);
                return;
            }
            Instruction.executing = true;
            Rand.PushState(this.RandState);
            try
            {
                this.DoImpl();
            }
            finally
            {
                Rand.PopState();
                Instruction.executing = false;
            }
        }

        public void Undo()
        {
            if (Instruction.executing)
            {
                Log.Error("Tried to execute an instruction but another instruction execution is in progress. Nested Undo() calls are not allowed.", false);
                return;
            }
            Instruction.executing = true;
            Rand.PushState(this.RandState);
            try
            {
                this.UndoImpl();
            }
            finally
            {
                Rand.PopState();
                Instruction.executing = false;
            }
        }

        public static void ThrowIfNotExecuting()
        {
            if (!Instruction.executing && !WorldGen.Working && !Maker.MakingEntity && !Maker.MakingSpell)
            {
                throw new Exception("This method changes game state and can only be called from an instruction. Otherwise it would break the timeline. (Calling Instruction.Do() without an Action also breaks the timeline)");
            }
        }

        public static void ThrowIfExecuting()
        {
            if (Instruction.executing && !MediaCreatorHelper.Running)
            {
                throw new Exception("This method has non-rewindable side effects (like getting new unique ID for a new object) and can only be called outside of instruction. Otherwise rewinding it and then doing again would produce different results than the first time. Entities are normally instantiated in Actions outside of instructions.");
            }
        }

        private static bool executing;
    }
}