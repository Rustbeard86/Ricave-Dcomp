using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public static class BatchModeHandler
    {
        public static bool CheckAndHandleBatchMode()
        {
            if (!Application.isBatchMode)
            {
                return false;
            }
            Log.Message("Running in batch mode.");
            string commandToExecute = BatchModeHandler.GetCommandToExecute();
            if (!commandToExecute.NullOrEmpty())
            {
                Log.Message("Batch mode: Executing command '" + commandToExecute + "'.");
                try
                {
                    DevConsoleCommands.TryExecute(commandToExecute);
                    Log.Message("Batch mode: Done executing command.");
                    goto IL_006C;
                }
                catch (Exception ex)
                {
                    Log.Error("Batch mode: Failed to execute command '" + commandToExecute + "'.", ex);
                    goto IL_006C;
                }
            }
            Log.Warning("Batch mode: No -command argument found. Usage: Ricave -batchmode -command \"commandname arg1 arg2\"", false);
        IL_006C:
            Log.Message("Batch mode: Shutting down.");
            Root.Quit();
            return true;
        }

        private static string GetCommandToExecute()
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            for (int i = 0; i < commandLineArgs.Length; i++)
            {
                if (commandLineArgs[i] == "-command" && i + 1 < commandLineArgs.Length)
                {
                    return commandLineArgs[i + 1];
                }
            }
            return null;
        }
    }
}