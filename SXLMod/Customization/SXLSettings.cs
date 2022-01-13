using UnityEngine;

namespace SXLMod.Customization
{
    public static class SXLSettings
    {
        // Player Settings
        public static bool realisticMode = false;

        // Player Board settings
        public static bool delayPop = true;
        public static float lowPop = 3.0f;
        public static float highPop = 3.5f;
        public static float popOutMultiplier = 1.0f;

        public static float truckTightness = 1.0f;

        // Player Camera Settings
        public static float fieldOfView = 60f;
        public static Vector3 cameraPosition;
        public static bool cameraShake = false;
        public static bool firstPersonView = false;
        public static float firstPersonViewFOV = 72f;
        public static bool firstPersonStereoView = false;
        public static bool followCamera = false;
        public static float followCameraFOV = 45f;
        public static bool skateCamera = false;
    }
}