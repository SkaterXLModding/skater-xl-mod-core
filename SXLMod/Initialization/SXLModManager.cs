using System.Collections;
using UnityEngine;

using SXLMod.Customization;
using SXLMod.Console;

namespace SXLMod
{
    public class SXLModManager : Singleton<SXLModManager>
    {
        // Create Global References Here
        private SXLConsole developerConsole;

        private ClothingSet _baseClothing;


        public void Create()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            this.developerConsole = this.gameObject.AddComponent<SXLConsole>();

            UnityEngine.Debug.Log("Created SXL Mod Manager");
            this._baseClothing = SXLClothing.GetCurrentPlayerGear();
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.BackQuote) && this.developerConsole != null)
            {
                this.developerConsole.ToggleState(this.developerConsole.CurrentState);
            }
        }

        public ClothingSet GetBaseClothing()
        {
            return this._baseClothing;
        }
    }
}

    
