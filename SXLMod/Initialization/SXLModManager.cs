using System.Collections;
using System.Linq;
using System.Reflection;

using HarmonyLib;

using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR;

using SXLMod.Console;
using SXLMod.Customization;

namespace SXLMod
{
    public class SXLModManager : Singleton<SXLModManager>
    {
        // Create Global References Here
        private SXLConsole developerConsole;

        public void Create()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            this.developerConsole = this.gameObject.AddComponent<SXLConsole>();
            
            Debug.Log("Created SXL Mod Manager");
        }

        void Awake()
        {
            string enableVRArg = "-vr";

            // Only run when we want VR
            if (SXLCoreUtilities.GetLaunchArg(enableVRArg))
            {
                // StartCoroutine(InitVR());
            }
        }

        void Start()
        {
            SXLPlayer.SetPlayerSettingsFromConfig();
            // SXLCamera.SetCameraSettingsFromConfig();
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.BackQuote) && this.developerConsole != null)
            {
                this.developerConsole.ToggleState(this.developerConsole.CurrentState);
            }
        }

        public static IEnumerator InitVR()
        {
            XRSettings.LoadDeviceByName("OpenVR");
            yield return null;
            if (XRSettings.loadedDeviceName != "OpenVR") yield break;

            XRSettings.enabled = true;
            XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);  // Depricated but whatever

            Valve.VR.SteamVR_Settings.instance.trackingSpace = Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated;
            Valve.VR.SteamVR.Initialize();
            Valve.VR.SteamVR_Actions.gameplay.Activate();
            Valve.VR.SteamVR_Actions.ui.Activate();
        }

        public static void StopXR()
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }
}

    
