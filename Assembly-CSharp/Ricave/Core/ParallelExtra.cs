using System;
using System.Collections.Generic;
using System.Threading;

namespace Ricave.Core
{
    public static class ParallelExtra
    {
        public static void ParallelFor(int from, int to, Action<int> action)
        {
            int num = to - from;
            if (num <= 0)
            {
                return;
            }
            int num2 = Math.Min(num, 4);
            int num3 = Calc.CeilToInt((float)num / (float)num2);
            int num4 = 0;
            ParallelExtra.action = action;
            ParallelExtra.threadDoneEvent.Reset(1);
            for (int i = 0; i < num2 - 1; i++)
            {
                int num5 = num4;
                int num6 = Math.Min(num5 + num3, to);
                if (num6 <= num5)
                {
                    break;
                }
                ParallelExtra.ThreadState threadState = ParallelExtra.GetThreadState(num5, num6);
                ParallelExtra.threadDoneEvent.AddCount();
                if (ThreadPool.QueueUserWorkItem(ParallelExtra.ThreadBodyDelegate, threadState))
                {
                    num4 = num6 + 1;
                }
                else
                {
                    ParallelExtra.threadDoneEvent.Signal();
                    object obj = ParallelExtra.threadStatePoolLock;
                    lock (obj)
                    {
                        ParallelExtra.threadStatePool.Add(threadState);
                    }
                }
            }
            int num7 = num4;
            int num8 = Math.Min(num7 + num3, to);
            for (int j = num7; j < num8; j++)
            {
                action(j);
            }
            ParallelExtra.threadDoneEvent.Signal();
            if (!ParallelExtra.threadDoneEvent.Wait(20000))
            {
                Log.Error("Waited for threads to finish for too long.", false);
            }
        }

        private static void ThreadBody(object stateObj)
        {
            ParallelExtra.ThreadState threadState = (ParallelExtra.ThreadState)stateObj;
            try
            {
                int to = threadState.to;
                for (int i = threadState.from; i < to; i++)
                {
                    ParallelExtra.action(i);
                }
            }
            finally
            {
                ParallelExtra.threadDoneEvent.Signal();
                object obj = ParallelExtra.threadStatePoolLock;
                lock (obj)
                {
                    ParallelExtra.threadStatePool.Add(threadState);
                }
            }
        }

        private static ParallelExtra.ThreadState GetThreadState(int from, int to)
        {
            object obj = ParallelExtra.threadStatePoolLock;
            ParallelExtra.ThreadState threadState;
            lock (obj)
            {
                if (ParallelExtra.threadStatePool.Count != 0)
                {
                    threadState = ParallelExtra.threadStatePool[ParallelExtra.threadStatePool.Count - 1];
                    ParallelExtra.threadStatePool.RemoveAt(ParallelExtra.threadStatePool.Count - 1);
                }
                else
                {
                    threadState = new ParallelExtra.ThreadState();
                }
            }
            threadState.from = from;
            threadState.to = to;
            return threadState;
        }

        private const int MaxThreads = 4;

        private static readonly WaitCallback ThreadBodyDelegate = new WaitCallback(ParallelExtra.ThreadBody);

        private static CountdownEvent threadDoneEvent = new CountdownEvent(0);

        private static Action<int> action;

        private static List<ParallelExtra.ThreadState> threadStatePool = new List<ParallelExtra.ThreadState>();

        private static object threadStatePoolLock = new object();

        private class ThreadState
        {
            public int from;

            public int to;
        }
    }
}