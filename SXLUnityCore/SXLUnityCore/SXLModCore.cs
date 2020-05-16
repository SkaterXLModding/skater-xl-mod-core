using UnityEngine;

namespace SXLUnityCore
{
    public static class SXLUnity
    {
        public static bool Init()
        {
            Cursor.lockState = CursorLockMode.None;
            return true;
        }
    }
}
