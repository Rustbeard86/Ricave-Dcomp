using System;
using System.Collections.Generic;
using System.Threading;
using Ricave.Rendering;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public static class Log
    {
        public static List<string> All
        {
            get
            {
                return Log.log;
            }
        }

        public static bool AnyError
        {
            get
            {
                return Log.anyError;
            }
        }

        public static bool ReachedLogLimit
        {
            get
            {
                return Log.loggedCount >= 1500;
            }
        }

        public static void Message(string message)
        {
            if (Log.ReachedLogLimit)
            {
                return;
            }
            Debug.Log(message);
            Log.Register(message);
        }

        public static void Warning(string message, bool once = false)
        {
            if (once && !Log.warningsOnce.Add(message))
            {
                return;
            }
            if (Log.ReachedLogLimit)
            {
                return;
            }
            Debug.LogWarning(message);
            Log.Register(RichText.Warning(message));
        }

        public static void Error(string message, bool once = false)
        {
            if (once && !Log.errorsOnce.Add(message))
            {
                return;
            }
            Log.anyError = true;
            if (Log.ReachedLogLimit)
            {
                return;
            }
            Debug.LogError(message);
            Log.Register(RichText.Error(message));
        }

        public static void Error(string message, Exception exception)
        {
            Log.anyError = true;
            if (Log.ReachedLogLimit)
            {
                return;
            }
            string text = message + " Exception: " + ((exception != null) ? exception.ToString() : null);
            Debug.LogError(text);
            Log.Register(RichText.Error(text));
        }

        public static void GlobalLogHandler(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Assert)
            {
                Log.OnAssertLogged(condition, stackTrace);
                return;
            }
            if (type == LogType.Exception)
            {
                Log.OnExceptionLogged(condition, stackTrace);
                return;
            }
        }

        public static void RegisterGlobalLogHandler()
        {
            Application.logMessageReceived += Log.GlobalLogHandler;
        }

        public static void ResetLogLimit()
        {
            Log.loggedCount = 0;
        }

        private static void OnExceptionLogged(string condition, string stackTrace)
        {
            Log.anyError = true;
            if (Log.ReachedLogLimit)
            {
                return;
            }
            Log.Register(RichText.Error("Exception caught by Unity: " + condition + " Stack trace: " + stackTrace));
        }

        private static void OnAssertLogged(string condition, string stackTrace)
        {
            Log.anyError = true;
            if (Log.ReachedLogLimit)
            {
                return;
            }
            Log.Register(RichText.Error("Assert logged by Unity: " + condition + " Stack trace: " + stackTrace));
        }

        private static void Register(string message)
        {
            object obj = Log.logLock;
            lock (obj)
            {
                Log.log.Add(RichText.LogDate("[" + DateTime.Now.ToString("hh:mm") + "]") + " " + message);
                Log.loggedCount++;
                if (Log.ReachedLogLimit)
                {
                    string text = "Reached log limit! Further messages, warnings, and errors won't be logged to avoid writing huge logfiles to the disk.";
                    Debug.LogError(text);
                    Log.log.Add(RichText.Error(text));
                }
                if (Root.MainThreadId == Thread.CurrentThread.ManagedThreadId)
                {
                    App.HideDeveloperConsole();
                }
            }
        }

        private static List<string> log = new List<string>(16);

        private static bool anyError;

        private static int loggedCount;

        private static HashSet<string> warningsOnce = new HashSet<string>();

        private static HashSet<string> errorsOnce = new HashSet<string>();

        private static object logLock = new object();

        private const int StopLoggingAtCount = 1500;
    }
}