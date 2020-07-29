using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

using Harmony12;

namespace SXLMod.Console
{
    class SXLDebugCommands
    {
        [RegisterCommand(Name = "d_dumptextures", Help = "Dump texture data tied to Renderer objects loaded in the scene", Hint = "d_texture", ArgMax = 0)]
        static void CommandDumpTextureData(CommandArg[] args)
        {
            List<System.Tuple<string, float, float>> textures = new List<System.Tuple<string, float, float>>();

            foreach (Renderer r in (Renderer[])Object.FindObjectsOfType(typeof(Renderer)))
            {
                foreach (Material m in r.materials)
                {
                    string[] properties = m.GetTexturePropertyNames();

                    foreach (string p in properties)
                    {
                        Texture t = m.GetTexture(p);
                        if (t == null)
                        {
                            continue;
                        }
                        textures.Add(new System.Tuple<string, float, float>(t.name, t.width, t.height));
                    }
                }
            }

            var orderedTex = textures.Distinct().OrderBy(t => t.Item2);
            var texCounts = orderedTex.GroupBy(t => t.Item2).ToDictionary(d => d.Key, d => d.Count());

            foreach (var texture in orderedTex)
            {
                Debug.Log($"{texture.Item1} {texture.Item2}x{texture.Item3}");
            }

            List<string> totals = new List<string>();
            foreach (var tuple in texCounts)
            {
                totals.Add($"{tuple.Key}x{tuple.Key} - {tuple.Value}");
            }
            Debug.Log($"<b>TOTAL</b>: {string.Join(" | ", totals)}");
        }
    }
}
