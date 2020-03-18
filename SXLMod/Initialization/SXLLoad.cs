using System.Reflection;

using Harmony12;
using UnityModManagerNet;

using SXLMod;
using SXLUnityCore;


namespace SXL.Main
{
    internal static class SXLRuntime
    {
        public static string mID;

        /// <summary>
        /// Unity Mod Manager Load function
        /// </summary>
        /// <param name="entry">Mod entry</param>
        /// <returns>True on complete</returns>
        public static bool Load(UnityModManager.ModEntry entry)
        {
            SXLRuntime.mID = entry.Info.Id;
            HarmonyInstance.Create(entry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());

            SXLUnity.Init();  // Initialize the custom Unity components
            SXLModManager.Instance.Create();  // Create the Persistant Manager for runtime event handling

            return true;
        }
    }
}
