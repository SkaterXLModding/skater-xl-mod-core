using System.Reflection;
using System.Linq;

using HarmonyLib;
using UnityModManagerNet;

using UnityEngine;
using UnityEngine.SceneManagement;

using SXLMod;


namespace SXL.Main
{
    internal static class SXLRuntime
    {
        public static Harmony instance;
        public static UnityModManager.ModEntry modEntry;

        /// <summary>
        /// Unity Mod Manager Load function
        /// </summary>
        /// <param name="entry">Mod entry</param>
        /// <returns>True on complete</returns>
        public static bool Load(UnityModManager.ModEntry entry)
        {
            entry.OnToggle = SXLRuntime.OnToggle;
            SXLRuntime.modEntry = entry;
            SXLModManager.Instance.Create();  // Create the Persistant Manager for runtime event handling


            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (value)
            {
                instance = new Harmony(modEntry.Info.Id);
                instance.PatchAll(Assembly.GetExecutingAssembly());
            }
            else
            {
                instance.UnpatchAll(modEntry.Info.Id);
                var modRef = UnityEngine.Object.FindObjectOfType<SXLModManager>();
                if (modRef != null)
                    UnityEngine.Object.Destroy(modRef.gameObject);
            }
            return true;
        }
    }
}
