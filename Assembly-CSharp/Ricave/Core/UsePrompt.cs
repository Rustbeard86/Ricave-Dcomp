using System;
using Ricave.UI;

namespace Ricave.Core
{
    public abstract class UsePrompt
    {
        public static object Choice
        {
            get
            {
                return UsePrompt.choice;
            }
        }

        public abstract void ShowUsePrompt(Action_Use intendedAction);

        protected void TryDoActionWithChoice(Action_Use useAction, object choice)
        {
            object obj = UsePrompt.choice;
            UsePrompt.choice = choice;
            try
            {
                ActionViaInterfaceHelper.TryDo(() => useAction);
            }
            finally
            {
                UsePrompt.choice = obj;
            }
        }

        private static object choice;
    }
}