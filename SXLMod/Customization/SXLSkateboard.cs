using System.IO;
using System.Linq;

using UnityEngine;

namespace SXLMod.Customization
{
    class SkateboardItem
    {
        public Texture2D texture { get; set; }

        public SkateboardItem(Texture2D boardTexture)
        {
            this.texture = boardTexture;
        }
    }

    class SXLSkateboard
    {
        private const string boardTextureName = "Texture2D_694A07B4";

        private readonly string[] boardMaterials = new string[7]
        {
          "GripTape",
          "Deck",
          "Hanger",
          "Wheel1 Mesh",
          "Wheel2 Mesh",
          "Wheel3 Mesh",
          "Wheel4 Mesh"
        };

        public GameObject GetPlayerSkateboard()
        {
            for (int i=0; i < SXLCustomization.playerComponents.Length; i++)
            {
                if (SXLCustomization.playerComponents[i].gameObject.name.Equals("Skateboard"))
                {
                    return SXLCustomization.playerComponents[i].gameObject;
                }
            }
            return null;
        }

        public SkateboardItem[] GetCustomSkateboards()
        {
            if (Directory.Exists(SXLCustomization.MOD_PATH + "\\Skateboard"))
            {
                string[] boardPaths = Directory.GetFiles(SXLCustomization.MOD_PATH + "\\Skateboard", "*.png");
                SkateboardItem[] boardItems = new SkateboardItem[boardPaths.Length];

                for (int i=0; i<boardPaths.Length; i++)
                {
                    boardItems[i] = new SkateboardItem(SXLFile.LoadImageFromFile(boardPaths[i]));
                }

                return boardItems;
            }
            return new SkateboardItem[0];
        }

        public void SetSkateboardTexture(SkateboardItem boardItem)
        {
            foreach(Transform child in GetPlayerSkateboard().GetComponentsInChildren<Transform>())
            {
                if (this.boardMaterials.Contains<string>(child.name))
                {
                    Renderer component = child.GetComponent<Renderer>();
                    if (component != null)
                    {
                        component.sharedMaterial.SetTexture(boardTextureName, (Texture)boardItem.texture);
                    }
                }
            }
        }
    }
}
