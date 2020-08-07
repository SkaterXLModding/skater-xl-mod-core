﻿using System.Collections.Generic;
using UnityEngine;

using SXLMod.Console;

namespace SXLMod
{
    public static class SXLCoreUtilities
    {
        public static IEnumerator<UnityEngine.WaitUntil> TakeDevScreenshot(string filePath)
        {
            SXLConsole.Instance.ForceCloseConsole();
            yield return new WaitUntil(() => SXLConsole.Instance.IsClosed == true);
            ScreenCapture.CaptureScreenshot(filePath);
        }
    }
}