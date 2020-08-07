using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace SXLMod
{
    public static class SXLFile
    {
        public static readonly string userModRoot = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkaterXL";
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

        public static Stream LoadDLLResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = $"{assembly.GetName().Name}.{resourceName}";
            Stream resourceStream = assembly.GetManifestResourceStream(name);
            return resourceStream;
        }

        public static Byte[] LoadDLLResourceToBytes(string resourceName)
        {
            Stream s = LoadDLLResource(resourceName);
            if (s == null)
            {
                return null;
            }
            byte[] ba = new byte[s.Length];
            s.Read(ba, 0, ba.Length);
            return ba;
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
            {
                return default(T);
            }
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
}
