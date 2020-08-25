using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using HarmonyLib;

using UnityEngine;


namespace SXLMod.Utilities
{
    public static class SXLFileWatcher
    {
        private static FileSystemWatcher fileSystemWatcher;

        private static Vector3 playerPos;
        private static Quaternion playerRot;
        private static Vector3 spawnPos;
        private static Quaternion spawnRot;

        public static void StartMapFileWatcher(LevelInfo level)
        {
            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\SkaterXL\\Maps";

            fileSystemWatcher = new FileSystemWatcher(path);
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.Changed += OnMapChanged;
            fileSystemWatcher.EnableRaisingEvents = true;

            Debug.Log($"File Watcher is watching {path} for updates to {CleanLevelName(level.FullName)}");
        }

        public static void StopMapFileWatcher()
        {
            fileSystemWatcher.Changed -= OnMapChanged;
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();

            // SceneManager.sceneLoaded -= OnHotReload;

            Debug.Log("File watcher is Disabled.");
        }

        private static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }

        private static string CleanLevelName(string inputLevelName)
        {
            if (inputLevelName.Contains('/'))
            {
                string[] nameSplit = inputLevelName.Split('/');
                inputLevelName = $"{nameSplit[0].Trim()} {nameSplit[1].Trim()}";
            }
            return inputLevelName;
        }

        private static string GetLevelBaseName(string inputLevelName)
        {
            return inputLevelName.Split(' ')[0];
        }

        private static void OnMapChanged(object source, FileSystemEventArgs e)
        {
            Debug.Log($"Map Changed...{e.Name}");
            LevelManager.Instance.StartCoroutine(OnMapChanged(e.Name));
        }

        private static IEnumerator<UnityEngine.Coroutine> OnMapChanged(string name)
        {
            LevelManager manager = LevelManager.Instance;
            string levelName = CleanLevelName(manager.currentLevel.FullName);  // Just in case this acts as a pointer.

            if (Path.GetFileName(name) == levelName)
            {
                Respawn r = PlayerController.Instance.respawn;
                Transform playerXform = PlayerController.Instance.boardController.boardTransform;
                // Create a copy since transform is a pointer
                playerPos = new Vector3(playerXform.position.x, playerXform.position.y, playerXform.position.z);  
                playerRot = new Quaternion(playerXform.rotation.x, playerXform.rotation.y, playerXform.rotation.z, playerXform.rotation.w);
                spawnPos = new Vector3(r.pin.position.x, r.pin.position.y, r.pin.position.z);
                spawnRot = new Quaternion(r.pin.rotation.x, r.pin.rotation.y, r.pin.rotation.z, r.pin.rotation.w);
                
                yield return manager.StartCoroutine(manager.PlayLevelRoutine(manager.currentLevel));

                r.SetSpawnPos(playerPos, playerRot);
                r.ForceRespawn();
                r.SetSpawnPos(spawnPos, spawnRot);
            }
            yield return null;
        }
    }
}
