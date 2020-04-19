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

        [RegisterCommand(Name = "BOARD", Help = "Player Settings", Hint = "<LOWPOP | HIGHPOP | POPOUT | TRUCKS | DEFAULT>", ArgMin = 0, ArgMax = 2)]
        static void CommandPlayer(CommandArg[] args)
        {
            if (args.Length == 0)
            {
                float popLow = PlayerController.Instance.popForce;
                float popHigh = PlayerController.Instance.highPopForce;
                float popOut = PlayerController.Instance.popOutMultiplier;
                JointDrive truck = PlayerController.Instance.boardController.backTruckJoint.angularXDrive;
                Debug.Log($"BOARD => LOWPOP: {popLow} - HIGHPOP: {popHigh} - POPOUT: {popOut} - TRUCKS: {truck.positionSpring}/{truck.positionDamper}");
                return;
            }

            if (args.Length == 1)
            {
                if (args[0].ToString().ToLower() == "default")
                {
                    PlayerController.Instance.popForce = LOWPOP;
                    PlayerController.Instance.highPopForce = HIGHPOP;
                    PlayerController.Instance.popOutMultiplier = POPOUT;
                    PlayerController.Instance.boardController.frontTruckJoint.angularXDrive = frontTruck;
                    PlayerController.Instance.boardController.backTruckJoint.angularXDrive = backTruck;
                }
                return;
            }

            switch (args[0].ToString().ToLower())
            {
                case "lowpop":
                    PlayerController.Instance.popForce = args[1].Float;
                    return;
                case "highpop":
                    PlayerController.Instance.highPopForce = args[1].Float;
                    return;
                case "popout":
                    PlayerController.Instance.popOutMultiplier = args[1].Float;
                    return;
                case "trucks":
                    float truckVal = args[1].Float;
                    truckVal = truckVal > 1 ? 1 : truckVal;
                    truckVal = truckVal < 0 ? 0 : truckVal;
                    JointDrive jd = new JointDrive();
                    jd.positionSpring = truckVal + 0.25f;
                    jd.positionDamper = 0.04f * jd.positionSpring;
                    jd.maximumForce = 3.402823E+23f;
                    PlayerController.Instance.boardController.frontTruckJoint.angularXDrive = jd;
                    PlayerController.Instance.boardController.backTruckJoint.angularXDrive = jd;
                    return;
                default:
                    Debug.LogWarning($"BOARD {args[0].ToString()} does not exist.");
                    return;
            }
        }

        [RegisterCommand(Name = "POV", Help = "Toggle POV mode", Hint = "<0 | 1 ><[FOV]>", ArgMin = 1, ArgMax =2)]
        static void CommandPOV(CommandArg[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            if (args[0].Int >= 1)
            {
                
                HeadIK headIK = UnityEngine.Object.FindObjectOfType<HeadIK>();
                // POV camera
                if (POV_CAMERA == null)
                {
                    GameObject pov = new GameObject("POV_Camera");
                    pov.transform.parent = headIK.head.gameObject.transform;
                    Debug.Log("parented camera");

                    POV_CAMERA = pov.AddComponent<Camera>();
                    POV_CAMERA.nearClipPlane = 0.15f;
                    POV_CAMERA.depth = 999;
                    pov.transform.position = headIK.head.position;
                    pov.transform.rotation = headIK.head.rotation * Quaternion.Euler(new Vector3(-100f, 13f, 43f));
                    pov.transform.localRotation = pov.transform.localRotation * Quaternion.Euler(new Vector3(20f, 0f, 50f));
                }

                float fov = 72f;
                if (args.Length == 2)
                {
                    fov = args[1].Float;
                }

                POV_CAMERA.fieldOfView = fov;
                POV_CAMERA.enabled = true;
                return;
            }
            POV_CAMERA.enabled = false;
        }
    }
}
