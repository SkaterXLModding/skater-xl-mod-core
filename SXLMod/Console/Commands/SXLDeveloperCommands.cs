using System;
using System.IO;

using UnityEngine;

using SXLMod.Utilities;

namespace SXLMod.Console
{
    class SXLDeveloperCommands
    {
        [RegisterCommand(Name = "dev_hot_reload", Help = "Puts a file watcher on custom maps. Reloads map on asset bundle update.", Hint = "dev_hot_reload <0|1>", ArgMin = 1, ArgMax = 1)]
        static void CommandDevHotReload(CommandArg[] args)
        {
            LevelInfo info = LevelManager.Instance.currentLevel;
            if (info.isAssetBundle)
            {
                switch (args[0].Int)
                {
                    case 0:
                        SXLFileWatcher.StopMapFileWatcher();
                        break;
                    case 1:
                        SXLFileWatcher.StartMapFileWatcher(LevelManager.Instance.currentLevel);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                SXLConsole.Log($"{info.name} is not a custom map. Hot Reload is not Enabled.");
            }
        }

        [RegisterCommand(Name = "dev_screenshot", Help = "Takes a screenshot of the current frame", Hint ="dev_screenshot <[opt] map>", ArgMin = 0, ArgMax = 1)]
        static void CommandDevScreenshot(CommandArg[] args)
        {

            LevelManager manager = LevelManager.Instance;
            string levelName = manager.currentLevel.FullName;
            string suffix = "";
            bool isMapCommand = false;

            string screenshotRoot = $"{SXLFile.userModRoot}\\Screenshots";
            Directory.CreateDirectory(screenshotRoot);  // Try to create folder, if it exists nothing happens.
            
            if (args.Length == 1)
            {
                if (args[0].ToString().ToLower() == "map")
                {
                    screenshotRoot = $"{SXLFile.userModRoot}\\Maps";
                    isMapCommand = true;
                }
                else
                {
                    SXLConsole.Log($"Argument {args[0].ToString()} is not a valid argument.");
                    return;
                }
            }
            else
            {
                DirectoryInfo d = new DirectoryInfo(screenshotRoot);
                FileInfo[] files = d.GetFiles("*.png");
                int fileCounter = 0;

                foreach (FileInfo f in files)
                {
                    if (f.FullName.Contains(levelName))
                    {
                        fileCounter++;
                    }
                }
                suffix = $"_{fileCounter}";
            }

            SXLConsole.Instance.StartCoroutine(SXLCoreUtilities.TakeDevScreenshot($"{screenshotRoot}\\{levelName}{suffix}.png", isMapCommand));
        }
    }
}
