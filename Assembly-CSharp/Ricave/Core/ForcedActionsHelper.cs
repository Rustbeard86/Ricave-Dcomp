using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class ForcedActionsHelper
    {
        public static bool AnyForcedAction(Actor actor)
        {
            return Get.CellsInfo.IsFallingAt(actor.Position, actor, false) || actor.Velocity != 0 || (actor.AttachedToChainPost != null && !actor.Position.IsAdjacentOrInside(actor.AttachedToChainPost.Position)) || actor.ConditionsAccumulated.AnyForcedAction;
        }

        public static Action GetForcedAction(Actor actor)
        {
            Rand.PushState(Calc.CombineHashes<int, Vector3Int, int, int>(actor.MyStableHash, actor.Position, actor.Sequence, 813244150));
            Action action;
            try
            {
                if (Get.CellsInfo.IsFallingAt(actor.Position, actor, false))
                {
                    action = new Action_MoveDownByGravity(Get.Action_MoveDownByGravity, actor, actor.Position);
                }
                else if (actor.Velocity != 0)
                {
                    action = new Action_ResolveVelocity(Get.Action_ResolveVelocity, actor);
                }
                else if (actor.AttachedToChainPost != null && !actor.Position.IsAdjacentOrInside(actor.AttachedToChainPost.Position))
                {
                    action = new Action_PulledByChain(Get.Action_PulledByChain, actor, actor.AttachedToChainPost);
                }
                else
                {
                    Action forcedAction = actor.ConditionsAccumulated.GetForcedAction();
                    if (forcedAction != null)
                    {
                        action = forcedAction;
                    }
                    else
                    {
                        if (actor.ConditionsAccumulated.AnyForcedAction)
                        {
                            Log.Error("Conditions AnyForcedAction returned true but GetForcedAction() returned null.", false);
                        }
                        action = null;
                    }
                }
            }
            finally
            {
                Rand.PopState();
            }
            return action;
        }

        public static string GetForcedActionReasonLabel(Actor actor)
        {
            if (Get.CellsInfo.IsFallingAt(actor.Position, actor, false))
            {
                return "Falling".Translate();
            }
            Condition firstConditionWithForcedAction = actor.ConditionsAccumulated.FirstConditionWithForcedAction;
            if (firstConditionWithForcedAction != null)
            {
                return firstConditionWithForcedAction.LabelCap;
            }
            return null;
        }
    }
}