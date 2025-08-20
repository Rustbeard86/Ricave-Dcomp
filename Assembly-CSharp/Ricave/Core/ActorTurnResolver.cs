using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class ActorTurnResolver
    {
        public static Actor ShowAIDebugFor
        {
            get
            {
                return ActorTurnResolver.showAIDebugFor;
            }
            set
            {
                ActorTurnResolver.showAIDebugFor = value;
            }
        }

        public static Actor CurrentlyResolvingAIFor
        {
            get
            {
                return ActorTurnResolver.currentlyResolvingAIFor;
            }
        }

        public static void TakeTurn(Actor actor)
        {
            if (actor.IsNowControlledActor)
            {
                Log.Error("TakeTurn() should never be called on the currently controlled actor.", false);
                return;
            }
            if (!actor.Spawned)
            {
                Log.Error("Called TakeTurn() on unspawned actor. Only spawned entities can ever take turns.", false);
                return;
            }
            int sequence = actor.Sequence;
            for (int i = 0; i < 2; i++)
            {
                int num = 0;
                while (ForcedActionsHelper.AnyForcedAction(actor))
                {
                    Action forcedAction = ForcedActionsHelper.GetForcedAction(actor);
                    if (forcedAction == null)
                    {
                        Log.Error("AnyForcedAction() returned true but GetForcedAction() returned null. Proceeding as if it returned false.", false);
                        break;
                    }
                    forcedAction.Do(true);
                    if (actor.Sequence != sequence || !actor.Spawned)
                    {
                        return;
                    }
                    num++;
                    if (num > 1000)
                    {
                        Log.Error("Too many forced actions done in ActorTurnResolver.TakeTurn(). Stopping to avoid infinite loop. Is ForcedActionsHelper.GetForcedAction() broken and returns the same action forever?", false);
                        break;
                    }
                }
                new Action_Think(Get.Action_Think, actor).Do(false);
                ActorTurnResolver.GetNextAIAction(actor).Do(false);
                if (actor.Sequence != sequence || !actor.Spawned)
                {
                    return;
                }
            }
        }

        private static Action GetNextAIAction(Actor actor)
        {
            if (actor.AI != null)
            {
                Profiler.Begin("GetNextAIAction()");
                try
                {
                    Rand.PushState(Calc.CombineHashes<int, Vector3Int, int, int>(actor.MyStableHash, actor.Position, actor.Sequence, 572143995));
                    try
                    {
                        ActorTurnResolver.currentlyResolvingAIFor = actor;
                        if (actor == ActorTurnResolver.showAIDebugFor)
                        {
                            string text = "Debug: GetNextAIAction() for ";
                            string text2 = ((actor != null) ? actor.ToString() : null);
                            string text3 = ". AIMemory: ";
                            AIMemory aimemory = actor.AIMemory;
                            Log.Message(text + text2 + text3 + ((aimemory != null) ? aimemory.ToString() : null));
                        }
                        Action nextAction = actor.AI.Root.GetNextAction(actor);
                        if (nextAction != null)
                        {
                            return nextAction;
                        }
                        Log.Error("Actor's AI returned null action. Returning \"wait\".", false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error while determining next AI action. Returning \"wait\".", ex);
                    }
                    finally
                    {
                        Rand.PopState();
                        ActorTurnResolver.currentlyResolvingAIFor = null;
                    }
                }
                finally
                {
                    Profiler.End();
                }
            }
            return new Action_Wait(Get.Action_Wait, actor, null, null);
        }

        private static Actor showAIDebugFor;

        private static Actor currentlyResolvingAIFor;
    }
}