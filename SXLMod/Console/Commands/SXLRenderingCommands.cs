using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

namespace SXLMod.Console
{
    class SXLRenderingCommands
    {
        [RegisterCommand(Name = "r_postprocessing", Help = "Toggle Postprocessing Effects", Hint = "r_postprocessing <0|1>", ArgMin = 1, ArgMax = 1)]
        static void CommandRenderingPostprocessing(CommandArg[] args)
        {
            List<string> ppNames = new List<string>() {"visual environment", "gradient sky", "hdri sky", "physically based sky"};
            foreach (Volume v in GameObject.FindObjectsOfType<Volume>())
            {
                foreach (VolumeComponent component in v.profile.components)
                {
                    SXLConsole.Log(component.displayName);
                    SXLConsole.Log(component.name);
                    if (ppNames.Contains(component.displayName.ToLower()))
                    {
                        continue;
                    }
                    switch (args[0].Int)
                    {
                        case 0:
                            component.active = false;
                            break;
                        case 1:
                            component.active = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        [RegisterCommand(Name = "r_dump_lighting", Help = "Dumps lighting data to console", Hint = "r_dump_lighting", ArgMax = 0)]
        static void CommandRenderingDumpLighting(CommandArg[] args)
        {
            

            foreach (Volume v in GameObject.FindObjectsOfType<Volume>())
            {
                foreach (VolumeComponent component in v.profile.components)
                {
                    Type componentType = component.GetType();
                    SXLConsole.Log($"Component: {componentType}");
                    SXLConsole.Log("----------------------");
                    foreach(VolumeParameter attr in component.parameters)
                    {
                        SXLConsole.Log(attr.GetType().ToString());
                        try
                        {
                            SXLConsole.Log($"Attribute:Value: {attr.ToString()} | Type: {attr.GetType()}");
                        }
                        catch (Exception e)
                        {
                            SXLConsole.LogError(e.ToString());
                        }
                    }
                    SXLConsole.Log("======================");
                }
            }
        }
    }
}
