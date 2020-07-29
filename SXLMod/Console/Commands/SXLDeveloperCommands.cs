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
                Debug.Log($"{info.name} is not a custom map. Hot Reload is not Enabled.");
            }
        }
    }
}
