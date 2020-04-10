using System;
using System.IO;
using System.Linq;

using UnityEngine;

namespace SXLMod.Console
{
    class SXLCoreCommands
    {
        [RegisterCommand(Name = "CLEAR", Help = "Clear the console", ArgMax = 0)]
        static void CommandClear(CommandArg[] args)
        {
            SXLConsole.Buffer.Clear();
        }

        [RegisterCommand(Name = "QUIT", Help = "Quit Application", ArgMax = 0)]
        static void CommandQuit(CommandArg[] args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

            Application.Quit();
        }

        [RegisterCommand(Name = "HELP", Help = "Display command context", Hint = "<COMMAND>", ArgMax = 1)]
        static void CommandHelp(CommandArg[] args)
        {
            if (args.Length == 0)
            {
                foreach (var command in SXLConsole.Shell.Commands)
                {
                    SXLConsole.Log("{0}: {1}", command.Key.PadRight(16), command.Value.help);
                }
                return;
            }

            string command_name = args[0].String.ToUpper();

            if (!SXLConsole.Shell.Commands.ContainsKey(command_name))
            {
                SXLConsole.Shell.IssueErrorMessage("Command {0} could not be found.", command_name);
                return;
            }

            var info = SXLConsole.Shell.Commands[command_name];

            if (info.help == null)
            {
                SXLConsole.Log($"{command_name} does not provide any help documentation.");
            }
            else if (info.hint == null)
            {
                SXLConsole.Log(info.help);
            }
            else
            {
                SXLConsole.Log($"{command_name} <color=lime>{info.hint}</color>  <color=silver># {info.help}</color>");
            }
        }
    }
}
