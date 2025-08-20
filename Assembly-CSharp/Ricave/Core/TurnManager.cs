using System;
using System.Collections.Generic;
using Ricave.Rendering;
using Ricave.UI;

namespace Ricave.Core
{
    public class TurnManager
    {
        public bool IsPlayerTurn
        {
            get
            {
                return !this.Rewinding && this.PlayerAvailable && this.GetSequenceableWithLowestSequence(false) == Get.NowControlledActor;
            }
        }

        public bool IsPlayerTurn_CanChooseNextAction
        {
            get
            {
                return this.IsPlayerTurn && !ForcedActionsHelper.AnyForcedAction(Get.NowControlledActor);
            }
        }

        public bool OptionalDelay
        {
            get
            {
                return Clock.Time < this.optionalDelayUntil;
            }
        }

        public bool Rewinding
        {
            get
            {
                return this.leftToRewind > 0;
            }
        }

        public Action MostRecentAction
        {
            get
            {
                if (this.recentActions.Count == 0)
                {
                    return null;
                }
                return this.recentActions[this.recentActions.Count - 1];
            }
        }

        public List<Action> RecentActions
        {
            get
            {
                return this.recentActions;
            }
        }

        public int RewindPoint
        {
            get
            {
                return this.rewindPoint;
            }
        }

        private bool PlayerAvailable
        {
            get
            {
                return Get.NowControlledActor != null && Get.NowControlledActor.Spawned;
            }
        }

        public int CurrentSequence
        {
            get
            {
                ISequenceable sequenceableWithLowestSequence = this.GetSequenceableWithLowestSequence(false);
                if (sequenceableWithLowestSequence == null)
                {
                    return this.inceptionSequence;
                }
                return sequenceableWithLowestSequence.Sequence;
            }
        }

        public List<ISequenceable> SequenceablesInOrder
        {
            get
            {
                return this.sequenceablesInOrder;
            }
        }

        public int InceptionSequence
        {
            get
            {
                return this.inceptionSequence;
            }
        }

        public bool CanDoActionsAtAllNow
        {
            get
            {
                return !Root.ChangingScene && !Get.ScreenFader.AnyActionQueued && !Get.TextSequenceDrawer.Showing && !Get.DungeonMapDrawer.Showing && !Get.DeathScreenDrawer.ShouldShow;
            }
        }

        public int OrderBeforeCurrent
        {
            get
            {
                ISequenceable sequenceableWithLowestSequence = this.GetSequenceableWithLowestSequence(false);
                if (sequenceableWithLowestSequence == null)
                {
                    return -1;
                }
                return this.GetSequenceableOrder(sequenceableWithLowestSequence);
            }
        }

        public int OrderAfterCurrent
        {
            get
            {
                ISequenceable sequenceableWithLowestSequence = this.GetSequenceableWithLowestSequence(false);
                if (sequenceableWithLowestSequence == null)
                {
                    return -1;
                }
                return this.GetSequenceableOrder(sequenceableWithLowestSequence) + 1;
            }
        }

        public Actor NextSeenActorTurnForUI
        {
            get
            {
                if (this.Rewinding)
                {
                    return null;
                }
                return (Actor)this.GetSequenceableWithLowestSequence(true);
            }
        }

        public void AddSequenceable(ISequenceable sequenceable, int initialSequenceOffset, int insertAt = -1)
        {
            Instruction.ThrowIfNotExecuting();
            if (sequenceable == null)
            {
                Log.Error("Tried to add null sequenceable to TurnManager.", false);
                return;
            }
            if (this.sequenceablesInOrder.Contains(sequenceable))
            {
                Log.Error("Tried to add the same sequenceable twice to TurnManager.", false);
                return;
            }
            if (initialSequenceOffset < 0)
            {
                Log.Error("Tried to add sequenceable with negative initialSequenceOffset.", false);
                return;
            }
            int currentSequence = this.CurrentSequence;
            if (insertAt >= 0)
            {
                if (insertAt > this.sequenceablesInOrder.Count)
                {
                    Log.Error(string.Concat(new string[]
                    {
                        "Tried to insert sequenceable at index ",
                        insertAt.ToString(),
                        " but sequenceable count is only ",
                        this.sequenceablesInOrder.Count.ToString(),
                        "."
                    }), false);
                    return;
                }
                this.sequenceablesInOrder.Insert(insertAt, sequenceable);
            }
            else
            {
                this.sequenceablesInOrder.Add(sequenceable);
            }
            sequenceable.Sequence = currentSequence + initialSequenceOffset;
            this.sequenceableOrderCachedDirty = true;
            this.sequenceableWithLowestSequenceCachedDirty = true;
        }

        public int RemoveSequenceable(ISequenceable sequenceable)
        {
            Instruction.ThrowIfNotExecuting();
            if (sequenceable == null)
            {
                Log.Error("Tried to remove null sequenceable from TurnManager.", false);
                return -1;
            }
            if (!this.sequenceablesInOrder.Contains(sequenceable))
            {
                Log.Error("Tried to remove sequenceable (" + sequenceable.ToStringSafe() + ") from TurnManager but it's not here.", false);
                return -1;
            }
            int num = this.sequenceablesInOrder.IndexOf(sequenceable);
            this.sequenceablesInOrder.RemoveLast(sequenceable);
            this.sequenceableOrderCachedDirty = true;
            this.sequenceableWithLowestSequenceCachedDirty = true;
            return num;
        }

        public int GetSequenceableOrder(ISequenceable sequenceable)
        {
            if (sequenceable == null)
            {
                return -1;
            }
            if (this.sequenceableOrderCachedDirty)
            {
                this.sequenceableOrderCachedDirty = false;
                this.sequenceableOrderCached.Clear();
                for (int i = 0; i < this.sequenceablesInOrder.Count; i++)
                {
                    this.sequenceableOrderCached.Add(this.sequenceablesInOrder[i], i);
                }
            }
            int num;
            if (this.sequenceableOrderCached.TryGetValue(sequenceable, out num))
            {
                return num;
            }
            return -1;
        }

        public void Update()
        {
            if (!this.CanDoActionsAtAllNow)
            {
                return;
            }
            if (App.IsDebugBuild && Clock.Frame % 5 == 4)
            {
                this.CheckSequenceablesForErrors();
            }
            this.CheckRewindNextRewindPoint();
            this.CheckAutoDoNextForcedOrNonPlayerActions();
        }

        public void TryDoAllNextForcedOrNonPlayerActionsNow()
        {
            int num = this.sequenceablesInOrder.Count * 20 + 10;
            int num2 = 0;
            while (this.CanDoActionsAtAllNow && !this.Rewinding && this.PlayerAvailable && !Get.NowControlledActor.ConditionsAccumulated.AnyForcedAction && !this.IsPlayerTurn_CanChooseNextAction)
            {
                if (this.IsPlayerTurn)
                {
                    if (ForcedActionsHelper.AnyForcedAction(Get.NowControlledActor))
                    {
                        Action forcedAction = ForcedActionsHelper.GetForcedAction(Get.NowControlledActor);
                        if (forcedAction != null)
                        {
                            forcedAction.Do(true);
                        }
                        else
                        {
                            Log.Error("AnyForcedAction() returned true but GetForcedAction() returned null. This will freeze the player's actor.", false);
                        }
                    }
                    else
                    {
                        Log.Error("It's player turn, there's no forced action, but player can't choose next action. What's blocking it?", false);
                    }
                }
                else
                {
                    this.DoNextNonPlayerAction();
                }
                num2++;
                if (num2 > num)
                {
                    Log.Error("Too many actions done in TryDoAllNextNonPlayerActions(). Stopping to avoid infinite loop. Are the executed actions not increasing the sequence?", false);
                    return;
                }
            }
        }

        private void CheckAutoDoNextForcedOrNonPlayerActions()
        {
            if (!this.CanDoActionsAtAllNow)
            {
                return;
            }
            if (this.Rewinding)
            {
                return;
            }
            if (!this.PlayerAvailable)
            {
                return;
            }
            if (this.OptionalDelay)
            {
                return;
            }
            if (this.IsPlayerTurn)
            {
                if (ForcedActionsHelper.AnyForcedAction(Get.NowControlledActor))
                {
                    Action forcedAction = ForcedActionsHelper.GetForcedAction(Get.NowControlledActor);
                    if (forcedAction != null)
                    {
                        forcedAction.Do(true);
                        return;
                    }
                    Log.Error("AnyForcedAction() returned true but GetForcedAction() returned null. This will freeze the player's actor.", false);
                    return;
                }
            }
            else
            {
                int num = Math.Max(6, Calc.CeilToInt((float)this.sequenceablesInOrder.Count / 7f));
                this.DoNextNonPlayerActionsUntilOptionalDelay(num);
            }
        }

        private void DoNextNonPlayerAction()
        {
            if (this.Rewinding)
            {
                Log.Error("Called DoNextNonPlayerAction() but we're currently rewinding time.", false);
                return;
            }
            if (this.IsPlayerTurn)
            {
                Log.Error("Called DoNextNonPlayerAction() but it's player's turn now.", false);
                return;
            }
            ISequenceable sequenceableWithLowestSequence = this.GetSequenceableWithLowestSequence(false);
            if (sequenceableWithLowestSequence == null)
            {
                Log.Error("Called DoNextNonPlayerAction() but there are no sequenceables.", false);
                return;
            }
            int sequence = sequenceableWithLowestSequence.Sequence;
            try
            {
                sequenceableWithLowestSequence.TakeTurn();
            }
            catch (Exception ex)
            {
                Log.Error("Error in sequenceable.TakeTurn().", ex);
            }
            this.sequenceableWithLowestSequenceCachedDirty = true;
            if (sequenceableWithLowestSequence.Sequence < sequence)
            {
                string text = "Sequence has decreased after calling TakeTurn(). This is not allowed. Previous sequence: ";
                string text2 = sequence.ToString();
                string text3 = " Sequenceable: ";
                ISequenceable sequenceable = sequenceableWithLowestSequence;
                Log.Warning(text + text2 + text3 + ((sequenceable != null) ? sequenceable.ToString() : null), false);
                new Action_ErrorRecoveryAddSequence(Get.Action_ErrorRecoveryAddSequence, sequenceableWithLowestSequence, sequence - sequenceableWithLowestSequence.Sequence + 12).Do(false);
                return;
            }
            if (sequenceableWithLowestSequence.Sequence == sequence && this.sequenceablesInOrder.Contains(sequenceableWithLowestSequence))
            {
                string text4 = "Sequence has not changed after calling TakeTurn(). Every such call must increase sequence by at least 1 to avoid potential infinite loops (it would be this sequenceable's turn forever and no one else would be able to make a move). Even if some subsequent call would increase the sequence eventually and everything would work theoretically, make it so that every TakeTurn() call increases sequence by at least 1 just so it's safer (e.g. by executing 2 actions in TakeTurn()). Sequenceable: ";
                ISequenceable sequenceable2 = sequenceableWithLowestSequence;
                Log.Warning(text4 + ((sequenceable2 != null) ? sequenceable2.ToString() : null), false);
                new Action_ErrorRecoveryAddSequence(Get.Action_ErrorRecoveryAddSequence, sequenceableWithLowestSequence, 12).Do(false);
            }
        }

        public void DoNextNonPlayerActionsUntilOptionalDelay(int maxSequenceablesToResolve = -1)
        {
            if (this.Rewinding)
            {
                Log.Error("Called DoNextNonPlayerActionsUntilOptionalDelay() but we're currently rewinding time.", false);
                return;
            }
            if (this.IsPlayerTurn)
            {
                Log.Error("Called DoNextNonPlayerActionsUntilOptionalDelay() but it's player's turn now.", false);
                return;
            }
            if (!this.PlayerAvailable)
            {
                Log.Error("Called DoNextNonPlayerActionsUntilOptionalDelay() but there's no player spawned. This could potentially cause an infinite loop.", false);
                return;
            }
            int num = this.sequenceablesInOrder.Count * 20 + 10;
            int num2 = 0;
            while (this.CanDoActionsAtAllNow && !this.Rewinding && !this.IsPlayerTurn && this.PlayerAvailable && !this.OptionalDelay)
            {
                this.DoNextNonPlayerAction();
                num2++;
                if (num2 > num)
                {
                    Log.Error("Too many actions done in DoNextNonPlayerActionsUntilOptionalDelay(). Stopping to avoid infinite loop. Are the executed actions not increasing the sequence?", false);
                    return;
                }
                if (maxSequenceablesToResolve >= 0 && num2 >= maxSequenceablesToResolve)
                {
                    break;
                }
            }
        }

        private void RewindMostRecentAction(out bool wasRewindPoint)
        {
            if (!this.Rewinding)
            {
                Log.Error("Called RewindMostRecentAction() but we're not rewinding time currently.", false);
                wasRewindPoint = false;
                return;
            }
            Action mostRecentAction = this.MostRecentAction;
            if (mostRecentAction == null)
            {
                this.leftToRewind = 0;
                wasRewindPoint = false;
                return;
            }
            wasRewindPoint = mostRecentAction.IsRewindPoint;
            mostRecentAction.Undo();
        }

        private void CheckRewindNextRewindPoint()
        {
            if (!this.CanDoActionsAtAllNow)
            {
                return;
            }
            if (!this.Rewinding)
            {
                return;
            }
            if (Clock.Time - this.lastActionRewindTime < 0.13f)
            {
                return;
            }
            bool flag = false;
            for (int i = this.recentActions.Count - 1; i >= 0; i--)
            {
                if (this.recentActions[i].IsRewindPoint)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                this.leftToRewind = 0;
                return;
            }
            int num = this.recentActions.Count + 1;
            int num2 = 0;
            while (this.Rewinding)
            {
                bool flag2;
                this.RewindMostRecentAction(out flag2);
                if (flag2)
                {
                    break;
                }
                num2++;
                if (num2 > num)
                {
                    Log.Error("Too many actions undone in CheckRewindNextRewindPoint(). Stopping to avoid infinite loop. Are the undone actions not removed from the list correctly?", false);
                    return;
                }
            }
        }

        public void RewindTime(int turns)
        {
            if (turns < 0)
            {
                Log.Error("Tried to rewind negative turns.", false);
                return;
            }
            this.leftToRewind += turns;
        }

        public ISequenceable GetSequenceableWithLowestSequence(bool seenActorsOnly = false)
        {
            if (this.sequenceablesInOrder.Count == 0)
            {
                return null;
            }
            if (!this.sequenceableWithLowestSequenceCachedDirty && !seenActorsOnly)
            {
                return this.sequenceableWithLowestSequenceCached;
            }
            ISequenceable sequenceable = null;
            int i = 0;
            while (i < this.sequenceablesInOrder.Count)
            {
                if (!seenActorsOnly)
                {
                    goto IL_004C;
                }
                Actor actor = this.sequenceablesInOrder[i] as Actor;
                if (actor != null && Get.VisibilityCache.PlayerSees(actor))
                {
                    goto IL_004C;
                }
            IL_0075:
                i++;
                continue;
            IL_004C:
                if (sequenceable == null || this.sequenceablesInOrder[i].Sequence < sequenceable.Sequence)
                {
                    sequenceable = this.sequenceablesInOrder[i];
                    goto IL_0075;
                }
                goto IL_0075;
            }
            if (!seenActorsOnly)
            {
                this.sequenceableWithLowestSequenceCachedDirty = false;
                this.sequenceableWithLowestSequenceCached = sequenceable;
            }
            return sequenceable;
        }

        private void CheckSequenceablesForErrors()
        {
            for (int i = 0; i < this.sequenceablesInOrder.Count; i++)
            {
                Entity entity = this.sequenceablesInOrder[i] as Entity;
                if (entity != null && !entity.Spawned)
                {
                    string text = "TurnManager has an unspawned Entity sequenceable. Sequenceable: ";
                    Entity entity2 = entity;
                    Log.Warning(text + ((entity2 != null) ? entity2.ToString() : null), true);
                }
                Condition condition = this.sequenceablesInOrder[i] as Condition;
                if (condition != null && (condition.AffectedActor == null || !condition.AffectedActor.Spawned))
                {
                    string text2 = "TurnManager has a Condition sequenceable which doesn't belong to any spawned actor. Sequenceable: ";
                    Condition condition2 = condition;
                    Log.Warning(text2 + ((condition2 != null) ? condition2.ToString() : null), true);
                }
                WorldSequenceable worldSequenceable = this.sequenceablesInOrder[i] as WorldSequenceable;
                if (worldSequenceable != null && worldSequenceable != Get.WorldSequenceable)
                {
                    string text3 = "TurnManager has a WorldSequenceable sequenceable which doesn't belong to the current world. Sequenceable: ";
                    WorldSequenceable worldSequenceable2 = worldSequenceable;
                    Log.Warning(text3 + ((worldSequenceable2 != null) ? worldSequenceable2.ToString() : null), true);
                }
            }
        }

        public bool HasSequencePriorityOver(ISequenceable sequenceable, ISequenceable over)
        {
            return this.HasSequencePriorityOver(sequenceable.Sequence, this.GetSequenceableOrder(sequenceable), over.Sequence, this.GetSequenceableOrder(over));
        }

        public bool HasSequencePriorityOver(int sequence, int sequenceableOrder, int overSequence, int overSequenceableOrder)
        {
            return sequence < overSequence || (sequence == overSequence && sequenceableOrder < overSequenceableOrder);
        }

        public void SetOptionalDelay(float delay)
        {
            this.optionalDelayUntil = Math.Max(this.optionalDelayUntil, Clock.Time + delay);
        }

        public void OnActionDone(Action action)
        {
            this.recentActions.Add(action);
            if (action.IsRewindPoint)
            {
                this.rewindPoint++;
                while (action.DoneAtRewindPoint - this.recentActions[0].DoneAtRewindPoint > 35)
                {
                    this.recentActions.RemoveAt(0);
                }
            }
            this.sequenceableWithLowestSequenceCachedDirty = true;
            TimelineDrawer.ClearCache();
        }

        public void OnActionUndone(Action action, bool wasRewindPoint)
        {
            this.recentActions.Remove(action);
            if (wasRewindPoint)
            {
                if (this.leftToRewind > 0)
                {
                    this.leftToRewind--;
                }
                this.rewindPoint--;
                this.lastActionRewindTime = Clock.Time;
                Get.NextTurnsUI.OnRewindToRewindPoint();
            }
            this.sequenceableWithLowestSequenceCachedDirty = true;
            TimelineDrawer.ClearCache();
        }

        public void OnSequenceChanged()
        {
            this.sequenceableWithLowestSequenceCachedDirty = true;
        }

        public void OnWorldDiscarded()
        {
            this.recentActions.Clear();
            this.inceptionSequence = this.CurrentSequence;
            this.sequenceablesInOrder.Clear();
            this.sequenceableOrderCachedDirty = true;
            this.sequenceableWithLowestSequenceCachedDirty = true;
        }

        public void TradePlaces(ISequenceable sequenceableA, ISequenceable sequenceableB)
        {
            if (sequenceableA == null || sequenceableB == null)
            {
                Log.Error("Tried to trade places with null sequenceable.", false);
                return;
            }
            if (!this.sequenceablesInOrder.Contains(sequenceableA) || !this.sequenceablesInOrder.Contains(sequenceableB))
            {
                Log.Error("Tried to trade places with sequenceable that's not in TurnManager.", false);
                return;
            }
            if (sequenceableA == sequenceableB)
            {
                Log.Error("Tried to trade places with the same sequenceable.", false);
                return;
            }
            int num = this.sequenceablesInOrder.IndexOf(sequenceableA);
            int num2 = this.sequenceablesInOrder.IndexOf(sequenceableB);
            this.sequenceablesInOrder[num] = sequenceableB;
            this.sequenceablesInOrder[num2] = sequenceableA;
            int sequence = sequenceableA.Sequence;
            sequenceableA.Sequence = sequenceableB.Sequence;
            sequenceableB.Sequence = sequence;
            this.sequenceableOrderCachedDirty = true;
            this.sequenceableWithLowestSequenceCachedDirty = true;
        }

        [Saved(Default.New, true)]
        private List<Action> recentActions = new List<Action>();

        [Saved]
        private int leftToRewind;

        [Saved]
        private int rewindPoint;

        [Saved(Default.New, true)]
        private List<ISequenceable> sequenceablesInOrder = new List<ISequenceable>();

        [Saved]
        private int inceptionSequence;

        private Dictionary<ISequenceable, int> sequenceableOrderCached = new Dictionary<ISequenceable, int>();

        private bool sequenceableOrderCachedDirty = true;

        private ISequenceable sequenceableWithLowestSequenceCached;

        private bool sequenceableWithLowestSequenceCachedDirty = true;

        private float optionalDelayUntil;

        private float lastActionRewindTime;

        public const int DefaultSequencePerTurn = 12;

        private const float TimeBetweenRewindingActions = 0.13f;

        private const int MaxRecentRewindPoints = 35;
    }
}