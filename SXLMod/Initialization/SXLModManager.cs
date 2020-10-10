using System.Collections;
using System.Linq;
using System.Reflection;

using HarmonyLib;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

using SXLMod.Console;
using SXLMod.Customization;
using SXLMod.Twitch;

namespace SXLMod
{
    public class SXLModManager : Singleton<SXLModManager>
    {
        // Create Global References Here
        private SXLConsole developerConsole;
        // public SXLTwitchClient twitchClient;

        public void Create()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            this.developerConsole = this.gameObject.AddComponent<SXLConsole>();
            // this.twitchClient = this.gameObject.AddComponent<SXLTwitchClient>();

            Debug.Log("Created SXL Mod Manager");
        }

        void Start()
        {
            SXLPlayer.SetPlayerSettingsFromConfig();
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.BackQuote) && this.developerConsole != null)
            {
                this.developerConsole.ToggleState(this.developerConsole.CurrentState);
            }
        }
    }
}

    
