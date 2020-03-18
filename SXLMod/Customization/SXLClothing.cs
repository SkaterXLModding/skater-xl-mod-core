using System;
using System.Linq;

using UnityEngine;

using SXLMod.Skinning;

namespace SXLMod.Customization
{
    public class ClothingTextures
    {
        public Texture2D baseColor { get; set; }
        public Texture2D normal { get; set; }
        public Texture2D rma { get; set; }

        public ClothingTextures(Texture2D bc, Texture2D nm, Texture2D rma)
        {
            this.baseColor = bc;
            this.normal = nm;
            this.rma = rma;
        }
    }

    public class ClothingSet
    {
        public SkinnedMeshRenderer hat;
        public SkinnedMeshRenderer shirt;
        public SkinnedMeshRenderer hoodie;
        public SkinnedMeshRenderer pants;
        public SkinnedMeshRenderer shoeL;
        public SkinnedMeshRenderer shoeR;

        public ClothingSet() { }
    }


    public class ClothingData
    {
        public Mesh mesh { get; set; }

        public ClothingTextures textures;
        private readonly bool isShoes;

        public ClothingData(Mesh m, Texture2D bc, Texture2D nm, Texture2D rma, bool isShoes=false)
        {
            this.mesh = m;
            this.textures = new ClothingTextures(bc, nm, rma);
            this.isShoes = isShoes;
        }

        public void TransferBoneWeights(Mesh xferMesh)
        {
            BoneWeight[] boneWeights = SXLSkinning.TransferSkinWeights(xferMesh, this.mesh);
            this.mesh.boneWeights = boneWeights;
        }
    }

    public class SXLClothing
    {
        private const string BASE_COLOR = "Texture2D_4128E5C7";
        private const string NORMAL_MAP = "Texture2D_BEC07F52";
        private const string RMA_MAP = "Texture2D_B56F9766";

        public static ClothingSet GetCurrentPlayerGear()
        {
            ClothingSet clothingSet = new ClothingSet();

            foreach (Tuple<CharacterGear, GameObject> gear in SXLCustomization.GetGearList())
            {
                // Shoes
                if (gear.Item1.categoryName.Equals("Shoes"))
                {
                    clothingSet.shoeL = UnityEngine.Object.Instantiate(gear.Item2.transform.Find("Shoe_L").gameObject.GetComponent<SkinnedMeshRenderer>());
                    clothingSet.shoeR = UnityEngine.Object.Instantiate(gear.Item2.transform.Find("Shoe_R").gameObject.GetComponent<SkinnedMeshRenderer>());
                }
                // Pants
                else if (gear.Item1.categoryName.Equals("Pants"))
                {
                    clothingSet.pants = UnityEngine.Object.Instantiate(gear.Item2.gameObject.GetComponent<SkinnedMeshRenderer>());
                }
                // Hoodie
                else if (gear.Item1.categoryName.Equals("Hoodie"))
                {
                    clothingSet.hoodie = UnityEngine.Object.Instantiate(gear.Item2.gameObject.GetComponent<SkinnedMeshRenderer>());
                }
                // Shirt
                else if (gear.Item1.categoryName.Equals("Shirt"))
                {
                    clothingSet.shirt = UnityEngine.Object.Instantiate(gear.Item2.gameObject.GetComponent<SkinnedMeshRenderer>());
                }
                // Hat
                else if (gear.Item1.categoryName.Equals("Hat"))
                {
                    clothingSet.hat = UnityEngine.Object.Instantiate(gear.Item2.gameObject.GetComponent<SkinnedMeshRenderer>());
                }
            }

            return clothingSet;
        }

        private static void SetClothingTextures(Material clothingMaterial, ClothingTextures clothingTexture)
        {
            // Base Color
            clothingMaterial.SetTexture(BASE_COLOR, clothingTexture.baseColor);
            // Normal Map
            clothingMaterial.SetTexture(NORMAL_MAP, clothingTexture.normal);
            // Roughness Metallic AO
            clothingMaterial.SetTexture(RMA_MAP, clothingTexture.rma);
        }

        private static Mesh MirrorClothingMesh(Mesh clothingItem)
        {
            // Flip Vertices and vertex normals for mesh
            Vector3[] vertices = clothingItem.vertices;
            Vector3[] normals = clothingItem.normals;
            for (int i=0; i<vertices.Length; i++)
            {
                var v = vertices[i];
                var n = normals[i];
                vertices[i] = new Vector3(-v.x, v.y, v.z);
                normals[i] = new Vector3(-n.x, n.y, n.z);
            }
            clothingItem.vertices = vertices;
            clothingItem.normals = normals;
            clothingItem.triangles = clothingItem.triangles.Reverse().ToArray();
            clothingItem.RecalculateBounds();

            return clothingItem;
        }

        public static void SetClothingModel(SkinnedMeshRenderer baseClothing, ClothingData newClothing)
        {
            // Set bind poses
            newClothing.mesh.bindposes = baseClothing.sharedMesh.bindposes;

            // Set new mesh to old mesh renderer
            baseClothing.sharedMesh = newClothing.mesh;

            // set new textures
            SetClothingTextures(baseClothing.sharedMaterial, newClothing.textures);
        }

        public static void SetShoeModel(SkinnedMeshRenderer leftShoe, SkinnedMeshRenderer rightShoe, ClothingData clothingData)
        {
            // Create Right Shoe Mesh
            Mesh tempRightShoe = MirrorClothingMesh(clothingData.mesh);
            // Set Bind poses to new shoes
            clothingData.mesh.bindposes = leftShoe.sharedMesh.bindposes;
            tempRightShoe.bindposes = rightShoe.sharedMesh.bindposes;
            // Set new shoe to old she mesh renderer
            leftShoe.sharedMesh = clothingData.mesh;
            rightShoe.sharedMesh = tempRightShoe;
            // Set new shoe Textures
            SetClothingTextures(leftShoe.sharedMaterial, clothingData.textures);
        }
    }
}
