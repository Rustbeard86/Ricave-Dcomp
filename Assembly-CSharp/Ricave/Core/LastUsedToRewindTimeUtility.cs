using System;
using Ricave.UI;

namespace Ricave.Core
{
    public static class LastUsedToRewindTimeUtility
    {
        public static bool CanRewindTime(IUsable usable)
        {
            return LastUsedToRewindTimeUtility.CanRewindTime(usable.LastUsedToRewindTimeSequence, usable.CanRewindTimeEveryTurns);
        }

        public static bool CanRewindTime(int? lastUsedToRewindTime, int? canRewindTimeEveryTurns)
        {
            if (lastUsedToRewindTime == null)
            {
                return true;
            }
            if (canRewindTimeEveryTurns != null)
            {
                int num = Get.TurnManager.CurrentSequence / 12;
                int num2 = lastUsedToRewindTime.Value / 12;
                return num >= num2 + canRewindTimeEveryTurns.Value;
            }
            return false;
        }

        public static string LastUsedToRewindTimeRichString(IUsable usable)
        {
            return LastUsedToRewindTimeUtility.LastUsedToRewindTimeRichString(usable.LastUsedToRewindTimeSequence, usable.CanRewindTimeEveryTurns);
        }

        public static string LastUsedToRewindTimeRichString(int? lastUsedToRewindTime, int? canRewindTimeEveryTurns)
        {
            if (lastUsedToRewindTime == null)
            {
                return "";
            }
            if (canRewindTimeEveryTurns == null)
            {
                return " {0}".Formatted(RichText.Error("({0})".Formatted("Used".Translate())));
            }
            int num = Get.TurnManager.CurrentSequence / 12;
            int num2 = lastUsedToRewindTime.Value / 12;
            if (num >= num2 + canRewindTimeEveryTurns.Value)
            {
                return "";
            }
            int num3 = num2 + canRewindTimeEveryTurns.Value - num;
            return " {0}".Formatted(RichText.Error("({0})".Formatted("CanRewindTimeAgainIn".Translate(StringUtility.TurnsString(num3)))));
        }
    }
}