using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public abstract class Action
    {
        public ActionSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public List<Instruction> InstructionsDone
        {
            get
            {
                return this.instructionsDone;
            }
        }

        public bool Done
        {
            get
            {
                return this.doneAtSequence >= 0;
            }
        }

        public int DoneAtSequence
        {
            get
            {
                return this.doneAtSequence;
            }
        }

        public int DoneAtRewindPoint
        {
            get
            {
                return this.doneAtRewindPoint;
            }
        }

        public bool ConcernsPlayer
        {
            get
            {
                return this.concernsPlayer;
            }
        }

        public bool WasForced
        {
            get
            {
                return this.wasForced;
            }
        }

        protected abstract int RandSeedPart { get; }

        public abstract string Description { get; }

        public bool IsRewindPoint
        {
            get
            {
                for (int i = 0; i < this.instructionsDone.Count; i++)
                {
                    if (this.instructionsDone[i].CausesRewindPoint)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static Action ExecutingAction
        {
            get
            {
                return Action.executingAction;
            }
        }

        protected Action()
        {
        }

        public Action(ActionSpec spec)
        {
            this.spec = spec;
            if (spec.ActionClass != base.GetType())
            {
                string[] array = new string[5];
                array[0] = "Created an Action with spec ";
                array[1] = ((spec != null) ? spec.ToString() : null);
                array[2] = ", but the created Action type is ";
                int num = 3;
                Type type = base.GetType();
                array[num] = ((type != null) ? type.ToString() : null);
                array[4] = ".";
                Log.Warning(string.Concat(array), false);
            }
            if (ActorTurnResolver.ShowAIDebugFor != null && ActorTurnResolver.ShowAIDebugFor == ActorTurnResolver.CurrentlyResolvingAIFor)
            {
                string text = "Debug: Created action ";
                string text2 = ((spec != null) ? spec.ToString() : null);
                string text3 = " for ";
                Actor showAIDebugFor = ActorTurnResolver.ShowAIDebugFor;
                Log.Message(text + text2 + text3 + ((showAIDebugFor != null) ? showAIDebugFor.ToString() : null));
            }
        }

        protected abstract IEnumerable<Instruction> MakeInstructions();

        protected abstract bool CalculateConcernsPlayer();

        public abstract bool CanDo(bool ignoreActorState = false);

        public void Do(bool forced = false)
        {
            if (this.Done)
            {
                Log.Error("Tried to do the same action twice.", false);
                return;
            }
            if (Get.TurnManager.Rewinding)
            {
                Log.Error("Tried to do an action, but we're currently rewinding time. This should have been checked before.", false);
                return;
            }
            if (!this.CanDo(false))
            {
                Log.Error("Tried to do an action which can't be done (" + this.ToStringSafe() + "). In general, avoid instantiating actions which can't be done in the first place.", false);
                return;
            }
            this.wasForced = forced;
            int currentSequence = Get.TurnManager.CurrentSequence;
            int rewindPoint = Get.TurnManager.RewindPoint;
            Profiler.Begin(this.spec.SpecID);
            try
            {
                Action action = Action.executingAction;
                Rand.PushState(Calc.CombineHashes<int, int>(this.RandSeedPart, currentSequence));
                try
                {
                    Action.executingAction = this;
                    foreach (Instruction instruction in this.MakeInstructions())
                    {
                        try
                        {
                            instruction.Do();
                            this.instructionsDone.Add(instruction);
                            if (instruction is Instruction_PlayLog)
                            {
                                this.concernsPlayer = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error while doing instruction.", ex);
                        }
                    }
                }
                catch (Exception ex2)
                {
                    Log.Error("Error while making instructions for an action.", ex2);
                }
                finally
                {
                    Action.executingAction = action;
                    Rand.PopState();
                }
            }
            finally
            {
                Profiler.End();
            }
            this.doneAtSequence = currentSequence;
            this.doneAtRewindPoint = rewindPoint;
            if (!this.concernsPlayer)
            {
                try
                {
                    this.concernsPlayer = this.CalculateConcernsPlayer();
                }
                catch (Exception ex3)
                {
                    Log.Error("Error in CalculateConcernsPlayer().", ex3);
                }
            }
            Get.TurnManager.OnActionDone(this);
        }

        public void Undo()
        {
            if (!this.Done)
            {
                Log.Error("Tried to undo an action which was never done (or done and then undone).", false);
                return;
            }
            if (Get.TurnManager.MostRecentAction != this)
            {
                Log.Error("Tried to undo an action but it's not the most recent one. This would break the timeline.", false);
                return;
            }
            bool isRewindPoint = this.IsRewindPoint;
            Action action = Action.executingAction;
            Rand.PushState(Calc.CombineHashes<int, int>(this.RandSeedPart, this.doneAtSequence));
            try
            {
                Action.executingAction = this;
                for (int i = this.instructionsDone.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        this.instructionsDone[i].Undo();
                        this.instructionsDone.RemoveAt(i);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error while undoing instruction.", ex);
                    }
                }
            }
            finally
            {
                Action.executingAction = action;
                Rand.PopState();
            }
            this.doneAtSequence = -1;
            this.doneAtRewindPoint = -1;
            this.concernsPlayer = false;
            this.instructionsDone.Clear();
            Get.TurnManager.OnActionUndone(this, isRewindPoint);
            Get.CameraEffects.OnActionUndone();
        }

        public override string ToString()
        {
            if (this.spec != null)
            {
                return this.spec.SpecID + " (" + base.GetType().Name + ")";
            }
            return base.GetType().Name;
        }

        [Saved]
        private ActionSpec spec;

        [Saved(Default.New, true)]
        private List<Instruction> instructionsDone = new List<Instruction>();

        [Saved(-1, false)]
        private int doneAtSequence = -1;

        [Saved(-1, false)]
        private int doneAtRewindPoint = -1;

        [Saved]
        private bool concernsPlayer;

        [Saved]
        private bool wasForced;

        private static Action executingAction;
    }
}