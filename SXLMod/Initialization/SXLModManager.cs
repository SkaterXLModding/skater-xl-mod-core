﻿using System.Collections;
using UnityEngine;

using SXLMod.Console;

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

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.BackQuote) && this.developerConsole != null)
            {
                this.developerConsole.ToggleState(this.developerConsole.CurrentState);
            }
        }
    }
}

    
