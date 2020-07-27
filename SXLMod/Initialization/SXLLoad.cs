using System.Reflection;

using Harmony12;
using UnityModManagerNet;

using SXLMod;


namespace SXL.Main
{
    internal static class SXLRuntime
    {
        public static string mID;
        public static HarmonyInstance instance;
        public static UnityModManager.ModEntry modEntry;

        public static bool enabled;

        /// <summary>
        /// Unity Mod Manager Load function
        /// </summary>
        /// <param name="entry">Mod entry</param>
        /// <returns>True on complete</returns>
        public static bool Load(UnityModManager.ModEntry entry)
        {
            SXLRuntime.mID = entry.Info.Id;
            entry.OnToggle = SXLRuntime.OnToggle;
            SXLRuntime.modEntry = entry;
            // HarmonyInstance.Create(entry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());

            SXLModManager.Instance.Create();  // Create the Persistant Manager for runtime event handling

            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            bool flag;
            if (SXLRuntime.enabled == value)
            {
                flag = true;
            }
            else
            {
                SXLRuntime.enabled = value;
                if (SXLRuntime.enabled)
                {
                    HarmonyInstance instance = HarmonyInstance.Create(modEntry.Info.Id);
                    instance.PatchAll(Assembly.GetExecutingAssembly());
                }
                flag = true;
            }
            return flag;
        }
    }
}
