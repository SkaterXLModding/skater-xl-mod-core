using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

using UnityEngine;

namespace SXLMod.Console
{
    public static class SXLConsoleCommands
    {
        [RegisterCommand(Name = "MAP", Help = "Load Map By Name", Hint = "<INDEX>", ArgMax = 1)]
        static void CommandMap(CommandArg[] args)
        {
            string mapsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkaterXL\\Maps\\";
            if (!Directory.Exists(mapsFolder))
            {
                UnityEngine.Debug.LogWarning($"{mapsFolder} does not exist!");
                return;
            }
            string[] maps = Directory.GetFiles(mapsFolder);

            if (maps.Length == 0)
            {
                UnityEngine.Debug.LogWarning($"No custom maps found!");
                return;
            }

            maps = maps.Where(m => (Path.GetExtension(m) == "")).ToArray();

            if (args.Length == 0)
            {
                UnityEngine.Debug.Log("Current installed custom maps:");
                for (int i = 0; i < maps.Length; i++)
                {
                    UnityEngine.Debug.Log($"[{i}] {Path.GetFileName(maps[i])}");
                }
                return;
            }

            int mapIndex = args[0].Int;
            if (maps.ElementAtOrDefault(mapIndex) == null)
            {
                UnityEngine.Debug.LogWarning("Map does not exist for the supplied input!");
                return;
            }
            UnityEngine.Debug.Log($"Loading {Path.GetFileName(maps[mapIndex])}");
            // Load logic?
        }
    }
}
