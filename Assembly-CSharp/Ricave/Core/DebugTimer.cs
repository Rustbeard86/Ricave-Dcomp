using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ricave.Core
{
    public class DebugTimer : IDisposable
    {
        public DebugTimer(string name = null, bool profiler = false)
        {
            this.name = name;
            this.profiler = profiler;
            if (DebugTimer.stopwatchesPool.Count != 0)
            {
                int num = DebugTimer.stopwatchesPool.Count - 1;
                this.stopwatch = DebugTimer.stopwatchesPool[num];
                DebugTimer.stopwatchesPool.RemoveAt(num);
            }
            else
            {
                this.stopwatch = new Stopwatch();
            }
            if (profiler)
            {
                Profiler.Begin(name ?? "DebugTimer");
            }
            this.stopwatch.Start();
        }

        void IDisposable.Dispose()
        {
            this.stopwatch.Stop();
            if (this.profiler)
            {
                Profiler.End();
            }
            Log.Message(string.Concat(new string[]
            {
                this.name ?? "Stopwatch",
                " elapsed: ",
                this.stopwatch.ElapsedMilliseconds.ToString(),
                "ms (",
                this.stopwatch.ElapsedTicks.ToString(),
                " ticks)"
            }));
            this.stopwatch.Reset();
            DebugTimer.stopwatchesPool.Add(this.stopwatch);
            this.stopwatch = null;
        }

        private string name;

        private Stopwatch stopwatch;

        private bool profiler;

        private static List<Stopwatch> stopwatchesPool = new List<Stopwatch>();
    }
}