using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;

namespace SXLMod
{
    public static class SXLFile
    {
        public static string modResourceDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\Mods\\SXLMod\\Resources";

        public static Texture2D LoadImageFromFile(string filePath)
        {
            Texture2D tex = new Texture2D(2, 2);
            if (File.Exists(filePath))
            {
                byte[] imageData = File.ReadAllBytes(filePath);
                tex.LoadImage(imageData);
            }
            tex.Apply();

            return tex;
        }

        public static Mesh ImportOBJFromFile(string filePath)
        {
            Mesh mesh = new Mesh();

            if (File.Exists(filePath))
            {
                ObjImporter importer = new ObjImporter();
                mesh = importer.ImportFile(filePath);
            }

            return mesh;
        }
    }
}
