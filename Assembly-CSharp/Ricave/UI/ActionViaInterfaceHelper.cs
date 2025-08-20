using System;
using Ricave.Core;

namespace Ricave.UI
{
    public static class ActionViaInterfaceHelper
    {
        public static bool TryDo(Func<Action> actionGetter)
        {
            Profiler.Begin("ActionViaInterfaceHelper.TryDo()");
            bool flag;
            try
            {
                if (!Get.TurnManager.CanDoActionsAtAllNow)
                {
                    flag = false;
                }
                else
                {
                    Get.TurnManager.TryDoAllNextForcedOrNonPlayerActionsNow();
                    if (!Get.TurnManager.IsPlayerTurn_CanChooseNextAction)
                    {
                        flag = false;
                    }
                    else if (!Get.TurnManager.CanDoActionsAtAllNow)
                    {
                        flag = false;
                    }
                    else
                    {
                        Action action = actionGetter();
                        if (action != null && action.CanDo(false))
                        {
                            action.Do(false);
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                }
            }
            finally
            {
                Profiler.End();
            }
            return flag;
        }
    }
}