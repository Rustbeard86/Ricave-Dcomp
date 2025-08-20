using System;
using System.Collections.Generic;
using System.Text;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public static class TimelineDrawer
    {
        public static void Do()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            float num = Math.Max((1f - (Clock.UnscaledTime - Get.UI.LastTimeOpenedAutoOpenInUIModeWindows) / 0.036363635f) * 8f, 0f);
            GUIExtra.DrawRectBump(new Rect(0f, Widgets.VirtualHeight - 25f + num, Widgets.VirtualWidth, 25f), new Color(0.25f, 0.25f, 0.25f), false);
            int num2 = (((float)Sys.Resolution.x < 1500f) ? 20 : 25);
            float num3 = Widgets.VirtualWidth / (float)num2;
            float num4 = Widgets.VirtualWidth - num3;
            int num5 = Get.TurnManager.RewindPoint;
            List<Action> recentActions = Get.TurnManager.RecentActions;
            int i = recentActions.Count - 1;
            int num6 = 0;
            while (num6 < num2 && num5 >= 0)
            {
                TimelineDrawer.actionsThisTurn.Clear();
                while (i >= 0)
                {
                    if (recentActions[i].DoneAtRewindPoint == num5)
                    {
                        TimelineDrawer.actionsThisTurn.Add(recentActions[i]);
                    }
                    else if (recentActions[i].DoneAtRewindPoint < num5)
                    {
                        break;
                    }
                    i--;
                }
                Rect rect = new Rect(num4, Widgets.VirtualHeight - 25f + num, num3, 25f);
                GUIExtra.DrawRectBump(rect, GUIExtra.HighlightedColorIfMouseover(rect, new Color(0.25f, 0.25f, 0.25f), true, 0.06f), false);
                if (TimelineDrawer.actionsThisTurn.Count != 0)
                {
                    Rect rect2 = rect;
                    rect2.xMax -= 12f;
                    int doneAtSequence = TimelineDrawer.actionsThisTurn[0].DoneAtSequence;
                    Widgets.Align = TextAnchor.MiddleRight;
                    GUI.color = (TimelineDrawer.AnyPlayLogInstruction(TimelineDrawer.actionsThisTurn) ? Color.white : new Color(1f, 1f, 1f, 0.5f));
                    Widgets.Label(rect2, GameTime.TimeToString(doneAtSequence, false), true, null, null, false);
                    GUI.color = Color.white;
                    Widgets.ResetAlign();
                    if (Mouse.Over(rect))
                    {
                        Get.Tooltips.RegisterTip(rect, TimelineDrawer.GetTip(TimelineDrawer.actionsThisTurn), null);
                    }
                }
                num4 -= num3;
                num5--;
                num6++;
            }
        }

        private static string GetTip(List<Action> actionsThisTurn)
        {
            int doneAtRewindPoint = actionsThisTurn[0].DoneAtRewindPoint;
            if (TimelineDrawer.cachedTipForRewindPoint == doneAtRewindPoint)
            {
                return TimelineDrawer.cachedTip;
            }
            int doneAtSequence = actionsThisTurn[0].DoneAtSequence;
            TimelineDrawer.tmpSb.Clear();
            TimelineDrawer.tmpSb.Append(GameTime.TimeToString(doneAtSequence, false));
            bool flag = true;
            for (int i = actionsThisTurn.Count - 1; i >= 0; i--)
            {
                if (actionsThisTurn[i].ConcernsPlayer)
                {
                    if (flag)
                    {
                        flag = false;
                        TimelineDrawer.tmpSb.AppendLine();
                    }
                    TimelineDrawer.tmpSb.AppendLine().Append(actionsThisTurn[i].Description);
                    List<Instruction> instructionsDone = actionsThisTurn[i].InstructionsDone;
                    for (int j = 0; j < instructionsDone.Count; j++)
                    {
                        Instruction_PlayLog instruction_PlayLog = instructionsDone[j] as Instruction_PlayLog;
                        if (instruction_PlayLog != null)
                        {
                            TimelineDrawer.tmpSb.AppendLine().Append("  - ").Append(instruction_PlayLog.Text);
                        }
                    }
                }
            }
            string text = (TimelineDrawer.cachedTip = TimelineDrawer.tmpSb.ToString());
            TimelineDrawer.cachedTipForRewindPoint = doneAtRewindPoint;
            return text;
        }

        public static void ClearCache()
        {
            TimelineDrawer.anyPlayLogCached.Clear();
            TimelineDrawer.cachedTip = null;
            TimelineDrawer.cachedTipForRewindPoint = -1;
        }

        private static bool AnyPlayLogInstruction(List<Action> actionsThisTurn)
        {
            int doneAtRewindPoint = actionsThisTurn[0].DoneAtRewindPoint;
            bool flag;
            if (TimelineDrawer.anyPlayLogCached.TryGetValue(doneAtRewindPoint, out flag))
            {
                return flag;
            }
            bool flag2 = false;
            foreach (Action action in actionsThisTurn)
            {
                using (List<Instruction>.Enumerator enumerator2 = action.InstructionsDone.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        if (enumerator2.Current is Instruction_PlayLog)
                        {
                            flag2 = true;
                            break;
                        }
                    }
                }
                if (flag2)
                {
                    break;
                }
            }
            TimelineDrawer.anyPlayLogCached.Add(doneAtRewindPoint, flag2);
            return flag2;
        }

        private const int Height = 25;

        private const int Count = 25;

        private const int CountLowRes = 20;

        private static List<Action> actionsThisTurn = new List<Action>();

        private static StringBuilder tmpSb = new StringBuilder();

        private static Dictionary<int, bool> anyPlayLogCached = new Dictionary<int, bool>();

        private static string cachedTip;

        private static int cachedTipForRewindPoint = -1;
    }
}