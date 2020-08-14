using System.Collections.Generic;
using System.Linq;

using HarmonyLib;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

using GameManagement;

using SXLMod.Console;

namespace SXLMod
{
    public static class SXLCoreUtilities
    {
        public static IEnumerator<UnityEngine.WaitUntil> TakeDevScreenshot(string filePath, bool isMap=false)
        {
            SXLConsoleState current = SXLConsole.Instance.CurrentState;

            SXLConsole.Instance.ForceCloseConsole();
            yield return new WaitUntil(() => SXLConsole.Instance.IsClosed == true);
            ScreenCapture.CaptureScreenshot(filePath);
            Debug.Log($"Screenshot Taken: {filePath}");

            SXLConsole.Instance.SetState(current);

            if (isMap)
            {
                LevelManager manager = LevelManager.Instance;
                manager.currentLevel.previewImage = SXLFile.LoadImageFromFile(filePath);}
        }
    }
}
