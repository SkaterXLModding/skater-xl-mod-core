using System.Collections.Generic;

using UnityEngine;

using SXLMod.Customization;

namespace SXLMod.Console
{
    class SXLCameraCommands
    {
        [RegisterCommand(Name = "c_position", Help = "Sets the position offset from the Default Camera", Hint = "c_position <reset|#,#,#>", ArgMin = 1, ArgMax = 3)]
        static void CommandCameraPosition(CommandArg[] args)
        {
            Vector3 offsetVector = Vector3.zero;

            if (args.Length == 3)
            {
                offsetVector.x = args[0].Float;
                offsetVector.y = args[1].Float;
                offsetVector.z = args[2].Float;
            }
            else if (args[0].String.ToLower() != "reset")
                return;

            SXLCamera.SetFilmerCameraPosition(offsetVector);
        }

        [RegisterCommand(Name = "c_shake", Help = "Enable or Disable Camera Shake", Hint = "c_shake <0|1>", ArgMin = 1, ArgMax = 1)]
        static void CommandCameraShake(CommandArg[] args)
        {
            SXLCamera.SetCameraShake(args[0].Int == 0 ? false : true);
        }

        [RegisterCommand(Name = "c_follow", Help = "Enable or Disable Follow Camera", Hint = "c_follow <0|1>", ArgMin = 1, ArgMax = 1)]
        static void CommandCameraFollow(CommandArg[] args)
        {
            SXLCamera.SetFollowCamera(args[0].Int == 0 ? false : true);
        }

        [RegisterCommand(Name = "c_skate", Help = "Enable or Disable SKATE. Camera", Hint = "c_skate <0|1>", ArgMin = 1, ArgMax = 1)]
        static void CommandCameraSkate(CommandArg[] args)
        {
            SXLCamera.SetSkateCamera(args[0].Int == 0 ? false : true);
        }
    }
}