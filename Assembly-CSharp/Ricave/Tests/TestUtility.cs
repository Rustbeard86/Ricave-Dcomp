using System;
using System.Collections.Generic;
using Ricave.Core;

namespace Ricave.Tests
{
    public static class TestUtility
    {
        public static void CheckSame(List<int> first, List<int> second)
        {
            if (!TestUtility.Same(first, second))
            {
                Log.Error(string.Concat(new string[]
                {
                    "Test failed: Collections ",
                    first.GetType().Name,
                    " and ",
                    second.GetType().Name,
                    " are not the same."
                }), false);
            }
        }

        public static void CheckTrue(bool condition, string message = null)
        {
            if (!condition)
            {
                if (message.NullOrEmpty())
                {
                    Log.Error("Test failed", false);
                    return;
                }
                Log.Error("Test failed: " + message, false);
            }
        }

        private static bool Same(List<int> first, List<int> second)
        {
            if (first == second)
            {
                return true;
            }
            if (first == null)
            {
                return false;
            }
            if (second == null)
            {
                return false;
            }
            if (first.Count != second.Count)
            {
                return false;
            }
            for (int i = 0; i < first.Count; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}