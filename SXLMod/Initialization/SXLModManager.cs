using System.Collections;
using UnityEngine;

using SXLMod.Customization;
using SXLMod.Debug;

namespace SXLMod
{
    public class SXLModManager : Singleton<SXLModManager>
    {
        // Create Global References Here
        private SXLDeveloperConsole developerConsole;

        private ClothingSet _baseClothing;


        public void Create()
        {
            developerConsole = this.gameObject.AddComponent<SXLDeveloperConsole>();

            SXLDeveloperConsole.Log("Created SXL Mod Manager");
            this._baseClothing = SXLClothing.GetCurrentPlayerGear();
        }

        void Update()
        {

        }

        public ClothingSet GetBaseClothing()
        {
            return this._baseClothing;
        }
    }
}

    
