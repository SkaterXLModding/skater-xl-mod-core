using UnityEngine;

namespace SXLMod.Customization
{
    public static class SXLPlayer
    {
        private static Camera POV_CAMERA;

        public static void SetLowPop(float value)
        {
            PlayerController.Instance.popForce = value;
            SXLFile.GetConfigFile().Write("p_lowpop", value.ToString(), "player");
        }

        public static void SetHighPop(float value)
        {
            PlayerController.Instance.highPopForce = value;
            SXLFile.GetConfigFile().Write("p_highpop", value.ToString(), "player");
        }

        public static void SetPopOutMultiplier(float value)
        {
            PlayerController.Instance.popOutMultiplier = value;
            SXLFile.GetConfigFile().Write("p_popout", value.ToString(), "player");
        }

        public static void SetTruckTightness(float value)
        {
            value = value > 1 ? 1 : value;
            value = value < 0 ? 0 : value;
            JointDrive jd = new JointDrive();
            jd.positionSpring = value + 0.25f;
            jd.positionDamper = 0.04f * jd.positionSpring;
            jd.maximumForce = 3.402823E+23f;
            PlayerController.Instance.boardController.frontTruckJoint.angularXDrive = jd;
            PlayerController.Instance.boardController.backTruckJoint.angularXDrive = jd;
            SXLFile.GetConfigFile().Write("p_trucks", value.ToString(), "player");
        }

        public static void SetPlayerView(int value, float fov)
        {
            if (value == 1)
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
                
                POV_CAMERA.fieldOfView = fov;
                POV_CAMERA.enabled = true;
            }
            else
            {
                POV_CAMERA.enabled = false;
            }
        }

        public static void SetPlayerSettingsFromConfig()
        {
            SXLConfiguration config = SXLFile.GetConfigFile();
            float lowPop = float.Parse(config.TryGet("p_lowpop", "player", "3.0"), System.Globalization.CultureInfo.InvariantCulture);
            float highPop = float.Parse(config.TryGet("p_highpop", "player", "3.5"), System.Globalization.CultureInfo.InvariantCulture);
            float popOut = float.Parse(config.TryGet("p_popout", "player", "1.0"), System.Globalization.CultureInfo.InvariantCulture);
            float truckTightness = float.Parse(config.TryGet("p_trucks", "player", "1.0"), System.Globalization.CultureInfo.InvariantCulture);

            SetLowPop(lowPop);
            SetHighPop(highPop);
            SetPopOutMultiplier(popOut);
            SetTruckTightness(truckTightness);
        }
    }
}
