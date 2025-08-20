using System;
using System.Collections.Generic;
using Ricave.Core;

namespace Ricave.UI
{
    public class PlannedPlayerActions
    {
        public bool AnySet
        {
            get
            {
                return !this.actions.NullOrEmpty<Action>() || this.leftWaitActions > 0;
            }
        }

        public int LeftWaitActions
        {
            get
            {
                return this.leftWaitActions;
            }
        }

        public bool ShouldStopOnSpaceKeyPressed
        {
            get
            {
                return (this.actions != null && this.actions.Count >= 2) || this.leftWaitActions > 1;
            }
        }

        public float LastInterruptTime
        {
            get
            {
                return this.lastInterruptTime;
            }
        }

        public void Set(List<Action> actions, IUsable usableForUsePrompt = null)
        {
            this.Stop();
            this.actions = actions;
            this.originalActionsCount = actions.Count;
            this.lastActionTime = -99999f;
        }

        public void SetMultipleWait(int count, float timeBetweenActions = 0.14f)
        {
            this.Stop();
            this.leftWaitActions = count;
            this.originalActionsCount = count;
            this.timeBetweenActions = timeBetweenActions;
            this.lastActionTime = -99999f;
        }

        public void Stop()
        {
            this.actions = null;
            this.originalActionsCount = 0;
            this.leftWaitActions = 0;
            this.timeBetweenActions = 0.14f;
        }

        public void Interrupt()
        {
            if (this.originalActionsCount != 1 && this.AnySet)
            {
                bool flag;
                if (this.actions != null && this.actions.Count == 1)
                {
                    Action_Use action_Use = this.actions[0] as Action_Use;
                    if (action_Use != null)
                    {
                        Structure structure = action_Use.Usable as Structure;
                        if (structure != null)
                        {
                            flag = structure.Spec.Structure.InteractingIsFreeUIHint;
                            goto IL_005E;
                        }
                    }
                }
                flag = false;
            IL_005E:
                this.Stop();
                if (!flag)
                {
                    this.lastInterruptTime = Clock.Time;
                }
            }
        }

        public void Update()
        {
            if (Clock.Time - this.lastActionTime >= this.timeBetweenActions)
            {
                if (this.leftWaitActions > 0 && this.actions.NullOrEmpty<Action>())
                {
                    if (this.actions == null)
                    {
                        this.actions = new List<Action>();
                    }
                    this.actions.Add(new Action_Wait(Get.Action_Wait, Get.NowControlledActor, null, null));
                    this.leftWaitActions--;
                }
                this.TryDoNextAction();
            }
        }

        private void TryDoNextAction()
        {
            if (this.actions.NullOrEmpty<Action>())
            {
                return;
            }
            if (!Get.TurnManager.CanDoActionsAtAllNow)
            {
                return;
            }
            Get.TurnManager.TryDoAllNextForcedOrNonPlayerActionsNow();
            if (!Get.TurnManager.IsPlayerTurn_CanChooseNextAction)
            {
                return;
            }
            if (this.actions.NullOrEmpty<Action>())
            {
                return;
            }
            if (!Get.TurnManager.CanDoActionsAtAllNow)
            {
                return;
            }
            bool flag = true;
            for (int i = 0; i < this.actions.Count; i++)
            {
                bool flag2 = i != 0;
                if (!this.actions[i].CanDo(flag2))
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                Action action = this.actions[0];
                this.actions.RemoveAt(0);
                this.lastActionTime = Clock.Time;
                if (this.actions.Count == 0)
                {
                    Action_Use action_Use = action as Action_Use;
                    if (action_Use != null)
                    {
                        UsePrompt usePrompt = action_Use.Usable.UsePrompt;
                        if (usePrompt != null)
                        {
                            if (!Get.UI.IsWheelSelectorOpen)
                            {
                                usePrompt.ShowUsePrompt(action_Use);
                                return;
                            }
                            return;
                        }
                    }
                }
                action.Do(false);
                return;
            }
            this.Stop();
        }

        [Saved(Default.Null, true)]
        private List<Action> actions;

        [Saved]
        private int originalActionsCount;

        [Saved]
        private int leftWaitActions;

        [Saved(0.14f, false)]
        private float timeBetweenActions = 0.14f;

        private float lastActionTime = -99999f;

        private float lastInterruptTime = -99999f;

        private const float DefaultTimeBetweenActions = 0.14f;
    }
}