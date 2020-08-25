using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using SXLMod.Debugging;


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

        [RegisterCommand(Name = "d_tracemovement", Help = "Visualize player movement over time", Hint = "d_tracemovement <0|1>", ArgMin = 1, ArgMax = 1)]
        static void CommandTraceMovement(CommandArg[] args)
        {
            switch(args[0].Int)
            {
                case 1:
                    SXLMovementTracer tracer = SXLModManager.Instance.gameObject.AddComponent<SXLMovementTracer>();
                    tracer.StartTracing();
                    break;
                case 0:
                    SXLModManager.Instance.gameObject.GetComponent<SXLMovementTracer>().StopTracing();
                    break;
            }
        }

        /*
        [RegisterCommand(Name = "d_heatmap", Help = "Generate Heatmap of gameplay", Hint = "d_heatmap <0|1>", ArgMin = 1, ArgMax = 1)]
        static void CommandHeatmap(CommandArg[] args)
        {
            switch(args[0].Int)
            {
                case 1:
                    SXLHeatMap heatMap = SXLModManager.Instance.gameObject.AddComponent<SXLHeatMap>();
                    break;
                case 0:
                    GameObject.Destroy(SXLModManager.Instance.gameObject.GetComponent<SXLHeatMap>());
                    break;
            }
        }
        */
    }
}
