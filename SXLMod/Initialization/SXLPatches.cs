using Harmony12;

namespace SXL.Main
{
    [HarmonyPatch(typeof(LevelManager), "FixBrokenShader")]
    internal class SXLPatches
    {
        private static bool Prefix()
        {
            return !SXLRuntime.enabled;
        }
    }
}
