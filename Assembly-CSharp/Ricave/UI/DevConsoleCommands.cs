using System;
using System.Linq;
using System.Text;
using Ricave.Core;

namespace Ricave.UI
{
    public static class DevConsoleCommands
    {
        public static DevConsoleCommands.Command? GetCommand(string name, int preferredArgCount = -1)
        {
            if (preferredArgCount >= 0)
            {
                for (int i = 0; i < DevConsoleCommandsList.Commands.Length; i++)
                {
                    if (DevConsoleCommandsList.Commands[i].Name == name && DevConsoleCommandsList.Commands[i].Args.Length == preferredArgCount)
                    {
                        return new DevConsoleCommands.Command?(DevConsoleCommandsList.Commands[i]);
                    }
                }
            }
            for (int j = 0; j < DevConsoleCommandsList.Commands.Length; j++)
            {
                if (DevConsoleCommandsList.Commands[j].Name == name)
                {
                    return new DevConsoleCommands.Command?(DevConsoleCommandsList.Commands[j]);
                }
            }
            return null;
        }

        public static void TryExecute(string input)
        {
            if (input.NullOrEmpty())
            {
                return;
            }
            string text = input.Trim(' ');
            if (text.Length == 0)
            {
                return;
            }
            string[] array = text.Split(' ', StringSplitOptions.None);
            int num = array.Length - 1;
            string text2 = array[0];
            DevConsoleCommands.Command? command = DevConsoleCommands.GetCommand(text2, num);
            if (command != null)
            {
                if (command.Value.Args.Length == num)
                {
                    try
                    {
                        Log.Message(text);
                        command.Value.Action(array.Skip<string>(1).ToArray<string>());
                        return;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error while executing command.", ex);
                        return;
                    }
                }
                Log.Error("Incorrect number of arguments for command: " + text2, false);
                return;
            }
            Log.Error("Command not found: " + text2, false);
        }

        public struct Command
        {
            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public DevConsoleCommands.Command.Arg[] Args
            {
                get
                {
                    return this.args;
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }
            }

            public Action<string[]> Action
            {
                get
                {
                    return this.action;
                }
            }

            public string NameWithArgsNames
            {
                get
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(this.name);
                    for (int i = 0; i < this.args.Length; i++)
                    {
                        stringBuilder.Append(' ').Append(this.args[i].Name);
                    }
                    return stringBuilder.ToString();
                }
            }

            public Command(string name, DevConsoleCommands.Command.Arg[] args, string description, Action<string[]> action)
            {
                this.name = name;
                this.args = args;
                this.description = description;
                this.action = action;
            }

            private string name;

            private DevConsoleCommands.Command.Arg[] args;

            private string description;

            private Action<string[]> action;

            public struct Arg
            {
                public string Name
                {
                    get
                    {
                        return this.name;
                    }
                }

                public Type SpecType
                {
                    get
                    {
                        return this.specType;
                    }
                }

                public Arg(string name, Type specType = null)
                {
                    this.name = name;
                    this.specType = specType;
                }

                private string name;

                private Type specType;
            }
        }
    }
}