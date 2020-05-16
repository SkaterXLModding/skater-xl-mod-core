using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class SXLEditorTools
{
    // Build Asset Bundles
    [MenuItem("SkaterXL/AssetBundles/Build AssetBundles")]
    static void BuildAssetBundles()
    {
        string bundleDirectory = "Assets/AssetBundles";
        if (!Directory.Exists(bundleDirectory))
        {
            Directory.CreateDirectory(bundleDirectory);
        }
        GameObject go = GameObject.Find("Grinds");
        if (go == null)
        {
            new GameObject("Grinds");
        }

        BuildPipeline.BuildAssetBundles(bundleDirectory, BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
    }

    // Capture screenshot for sharing
    [MenuItem("SkaterXL/Take Screenshot")]
    static void CaptureScreengrab()
    {
        string path = EditorUtility.SaveFilePanel("Save Screenshot", "", "", ".png");
        Debug.Log(path);

        if (!string.IsNullOrEmpty(path))
        {
            UnityEngine.ScreenCapture.CaptureScreenshot(path, 1);
        }
    }
}
#endif