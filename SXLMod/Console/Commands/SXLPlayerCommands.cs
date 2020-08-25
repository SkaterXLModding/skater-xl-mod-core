using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using SXLMod.Customization;

namespace SXLMod.Console
{
    class SXLPlayerCommands
    {
        private static readonly Camera MAIN_CAMERA = Camera.main;
        private static Camera POV_CAMERA;
        private static readonly float LOWPOP = PlayerController.Instance.popForce;
        private static readonly float HIGHPOP = PlayerController.Instance.highPopForce;
        private static readonly float POPOUT = PlayerController.Instance.popOutMultiplier;
        private static readonly JointDrive frontTruck = PlayerController.Instance.boardController.frontTruckJoint.angularXDrive;
        private static readonly JointDrive backTruck = PlayerController.Instance.boardController.backTruckJoint.angularXDrive;

        [RegisterCommand(Name = "p_pop", Help = "Sets board low pop and high pop values", Hint = "p_pop <float> <float>", ArgMin = 1, ArgMax = 2)]
        static void CommandPop(CommandArg[] args)
        {
            SXLPlayer.SetLowPop(args[0].Float);
            SXLPlayer.SetHighPop(args.Length == 2 ? args[1].Float : args[0].Float + 0.5f);
        }

        [RegisterCommand(Name = "p_lowpop", Help = "Sets board low pop value", Hint = "p_lowpop <float>", ArgMin = 1, ArgMax = 1)]
        static void CommandLowPop(CommandArg[] args)
        {
            SXLPlayer.SetLowPop(args[0].Float);
        }

        [RegisterCommand(Name = "p_highpop", Help = "Sets board high pop value", Hint = "p_highpop <float>", ArgMin = 1, ArgMax = 1)]
        static void CommandHighPop(CommandArg[] args)
        {
            SXLPlayer.SetHighPop(args[0].Float);
        }

        [RegisterCommand(Name = "p_popout", Help = "Sets board pop out multiplier value", Hint = "p_popout <float>", ArgMin = 1, ArgMax = 1)]
        static void CommandPopOut(CommandArg[] args)
        {
            SXLPlayer.SetPopOutMultiplier(args[0].Float);
        }

        [RegisterCommand(Name = "p_position", Help = "Returns the player position as a vector", Hint = "p_position", ArgMin = 0, ArgMax = 0)]
        static void CommandPlayerPosition(CommandArg[] args)
        {
            Debug.Log(PlayerController.Instance.transform.position);
        }

        [RegisterCommand(Name = "p_trucks", Help = "Sets truck tightness value", Hint = "p_trucks <float>", ArgMin = 1, ArgMax = 1)]
        static void CommandTruckTightness(CommandArg[] args)
        {
            SXLPlayer.SetTruckTightness(args[0].Float);
        }

        [RegisterCommand(Name = "p_fpv", Help = "Enable/Disable First Person Mode and set FOV", Hint = "p_fpv <0|1> <float>", ArgMin = 1, ArgMax =2)]
        static void CommandPOV(CommandArg[] args)
        {
            SXLPlayer.SetPlayerView(args[0].Int, args.Length == 2 ? args[1].Float : 72f);
        }
    }
}
