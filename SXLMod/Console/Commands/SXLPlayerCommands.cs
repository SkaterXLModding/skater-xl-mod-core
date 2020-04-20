using System.Collections.Generic;
using System.Linq;

using UnityEngine;

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
            PlayerController.Instance.popForce = args[0].Float;
            PlayerController.Instance.highPopForce = args.Length == 2 ? args[1].Float : args[0].Float + 0.5f;
        }

        [RegisterCommand(Name = "p_lowpop", Help = "Sets board low pop value", Hint = "p_lowpop <float>", ArgMin = 1, ArgMax = 1)]
        static void CommandLowPop(CommandArg[] args)
        {
            PlayerController.Instance.popForce = args[0].Float;
        }

        [RegisterCommand(Name = "p_highpop", Help = "Sets board high pop value", Hint = "p_highpop <float>", ArgMin = 1, ArgMax = 1)]
        static void CommandHighPop(CommandArg[] args)
        {
            PlayerController.Instance.highPopForce = args[0].Float;
        }

        [RegisterCommand(Name = "p_popout", Help = "Sets board pop out multiplier value", Hint = "p_popout <float>", ArgMin = 1, ArgMax = 1)]
        static void CommandPopOut(CommandArg[] args)
        {
            PlayerController.Instance.popOutMultiplier = args[0].Float;
        }

        [RegisterCommand(Name = "p_trucks", Help = "Sets truck tightness value", Hint = "p_trucks <float>", ArgMin = 1, ArgMax = 1)]
        static void CommandTruckTightness(CommandArg[] args)
        {
            float truckVal = args[0].Float;
            truckVal = truckVal > 1 ? 1 : truckVal;
            truckVal = truckVal < 0 ? 0 : truckVal;
            JointDrive jd = new JointDrive();
            jd.positionSpring = truckVal + 0.25f;
            jd.positionDamper = 0.04f * jd.positionSpring;
            jd.maximumForce = 3.402823E+23f;
            PlayerController.Instance.boardController.frontTruckJoint.angularXDrive = jd;
            PlayerController.Instance.boardController.backTruckJoint.angularXDrive = jd;
        }

        [RegisterCommand(Name = "p_fpv", Help = "Enable/Disable First Person Mode and set FOV", Hint = "p_fpv <0|1> <float>", ArgMin = 1, ArgMax =2)]
        static void CommandPOV(CommandArg[] args)
        {
            if (args[0].Int == 1)
            {
                // POV camera
                if (POV_CAMERA == null)
                {
                    HeadIK headIK = UnityEngine.Object.FindObjectOfType<HeadIK>();

                    GameObject pov = new GameObject("POV_Camera");
                    pov.transform.parent = headIK.head.gameObject.transform;

                    POV_CAMERA = pov.AddComponent<Camera>();
                    POV_CAMERA.nearClipPlane = 0.15f;
                    POV_CAMERA.depth = 999;
                    pov.transform.position = headIK.head.position;
                    pov.transform.rotation = headIK.head.rotation * Quaternion.Euler(new Vector3(-100f, 13f, 43f));
                    pov.transform.localRotation = pov.transform.localRotation * Quaternion.Euler(new Vector3(20f, 0f, 50f));
                }

                float fov = args.Length == 2 ? args[1].Float : 72f;
                POV_CAMERA.fieldOfView = fov;
                POV_CAMERA.enabled = true;
            }
            else
            {
                POV_CAMERA.enabled = false;
            }
        }
    }
}
