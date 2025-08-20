using System;

namespace Ricave.Core
{
    public static class GameTime
    {
        public static void GetTime(out int hour, out int minute)
        {
            GameTime.GetTime(Get.TurnManager.CurrentSequence, out hour, out minute);
        }

        public static void GetTime(int sequence, out int hour, out int minute)
        {
            bool flag = Get.InLobby && Get.Progress.GetRunStats(Get.Run_Main1).Runs <= 0;
            Rand.PushState(Calc.CombineHashes<int, int>(Get.RunSeed, 713538052));
            int num = (flag ? 0 : Rand.RangeInclusive(0, 1439));
            Rand.PopState();
            int num2 = sequence / 12 + num;
            hour = num2 / 60 % 24;
            minute = num2 % 60;
        }

        public static string TimeToString(int hour, int minute, bool animate = false)
        {
            bool flag;
            if (animate)
            {
                float num = 12f / (float)Get.NowControlledActor.ConditionsAccumulated.SequencePerTurn;
                flag = Clock.UnscaledTime % (2f * num) < num;
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                return GameTime.timeCache_separator.Get(new ValueTuple<int, int>(hour, minute));
            }
            return GameTime.timeCache_noSeparator.Get(new ValueTuple<int, int>(hour, minute));
        }

        public static string TimeToString(int sequence, bool animate = false)
        {
            int num;
            int num2;
            GameTime.GetTime(sequence, out num, out num2);
            return GameTime.TimeToString(num, num2, animate);
        }

        public static string TimeToString(bool animate = false)
        {
            return GameTime.TimeToString(Get.TurnManager.CurrentSequence, animate);
        }

        private static readonly CalculationCache<ValueTuple<int, int>, string> timeCache_separator = new CalculationCache<ValueTuple<int, int>, string>((ValueTuple<int, int> x) => x.Item1.ToString("00") + ":" + x.Item2.ToString("00"), 300);

        private static readonly CalculationCache<ValueTuple<int, int>, string> timeCache_noSeparator = new CalculationCache<ValueTuple<int, int>, string>((ValueTuple<int, int> x) => x.Item1.ToString("00") + " " + x.Item2.ToString("00"), 300);
    }
}