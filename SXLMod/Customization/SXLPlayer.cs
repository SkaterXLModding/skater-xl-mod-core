using System.Collections;

using UnityEngine;

namespace SXLMod.Customization
{
    public static class SXLPlayer
    {
        private static Camera POV_CAMERA;
        private static Camera MAIN_CAMERA;

        public static void SetRealisticMode(bool value)
        {
            SXLFile.GetConfigFile().Write("p_realistic", value.ToString(), "player");
        }

        public static void SetPopDelayed(bool value)
        {
            SXLFile.GetConfigFile().Write("p_delaypop", value.ToString(), "player");
            SXLSettings.delayPop = value;
        }

        public static void SetLowPop(float value)
        {
            PlayerController.Instance.popForce = value;
            SXLFile.GetConfigFile().Write("p_lowpop", value.ToString(), "player");
            SXLSettings.lowPop = value;
        }

        public static void SetHighPop(float value)
        {
            PlayerController.Instance.highPopForce = value;
            SXLFile.GetConfigFile().Write("p_highpop", value.ToString(), "player");
            SXLSettings.highPop = value;
        }

        public static void SetPopOutMultiplier(float value)
        {
            PlayerController.Instance.popOutMultiplier = value;
            SXLFile.GetConfigFile().Write("p_popout", value.ToString(), "player");
            SXLSettings.popOutMultiplier = value;
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
            SXLSettings.truckTightness = value;
        }

        public static void SetPlayerView(int value, float fov, float headOffset)
        {
            if (value == 1)
            {

                HeadIK headIK = UnityEngine.Object.FindObjectOfType<HeadIK>();
                // POV camera
                if (POV_CAMERA == null)
                {

                    GameObject pov = new GameObject("POV_Camera");
                    pov.transform.parent = headIK.head.gameObject.transform;

                    POV_CAMERA = pov.AddComponent<Camera>();
                    POV_CAMERA.nearClipPlane = 0.15f;
                    POV_CAMERA.depth = 999;
                    POVCameraController povCC = pov.AddComponent<POVCameraController>();
                    povCC.povCam = POV_CAMERA;
                    pov.transform.position = headIK.head.position + new Vector3(0f, headOffset, 0f);
                    pov.transform.rotation = headIK.head.rotation * Quaternion.Euler(new Vector3(-100f, 13f, 43f));
                    pov.transform.localRotation = pov.transform.localRotation * Quaternion.Euler(new Vector3(20f, 0f, 50f));
                    POV_CAMERA.fieldOfView = fov;
                    SXLSettings.firstPersonViewFOV = fov;
                    POV_CAMERA.enabled = true;
                    return;

                }
                POV_CAMERA.gameObject.transform.position = headIK.head.position + new Vector3(0f, headOffset, 0f);
                POV_CAMERA.fieldOfView = fov;
                POV_CAMERA.enabled = true;
                SXLSettings.firstPersonView = true;
                SXLSettings.firstPersonViewFOV = fov;
            }
            else
            {
                POV_CAMERA.enabled = false;
                SXLSettings.firstPersonView = false;
            }
        }

        public static void SetPlayerFOV(float value)
        {
            if (MAIN_CAMERA == null)
            {
                MAIN_CAMERA = Camera.main;
                Debug.Log(MAIN_CAMERA.fieldOfView);
            }
            MAIN_CAMERA.fieldOfView = value > 0.1f && value < 120.0f ? value : MAIN_CAMERA.fieldOfView;
            SXLFile.GetConfigFile().Write("p_fov", value.ToString(), "player");
            SXLSettings.fieldOfView = value;
        }

        public static void SetPlayerSettingsFromConfig()
        {
            SXLConfiguration config = SXLFile.GetConfigFile();

            SXLSettings.realisticMode = bool.Parse(config.TryGet("p_realistic", "player", "false"));
            SXLSettings.delayPop = bool.Parse(config.TryGet("p_delaypop", "player", "false"));
            SXLSettings.lowPop = float.Parse(config.TryGet("p_lowpop", "player", "3.0"), System.Globalization.CultureInfo.InvariantCulture);
            SXLSettings.highPop = float.Parse(config.TryGet("p_highpop", "player", "3.5"), System.Globalization.CultureInfo.InvariantCulture);
            SXLSettings.popOutMultiplier = float.Parse(config.TryGet("p_popout", "player", "1.0"), System.Globalization.CultureInfo.InvariantCulture);
            SXLSettings.truckTightness = float.Parse(config.TryGet("p_trucks", "player", "1.0"), System.Globalization.CultureInfo.InvariantCulture);
            SXLSettings.fieldOfView = float.Parse(config.TryGet("p_fov", "player", "60.0"), System.Globalization.CultureInfo.InvariantCulture);

            SetRealisticMode(SXLSettings.realisticMode);
            SetPopDelayed(SXLSettings.delayPop);
            SetLowPop(SXLSettings.lowPop);
            SetHighPop(SXLSettings.highPop);
            SetPopOutMultiplier(SXLSettings.popOutMultiplier);
            SetTruckTightness(SXLSettings.truckTightness);
            SetPlayerFOV(SXLSettings.fieldOfView);
        }
    }

    public class POVCameraController : MonoBehaviour
    {
        public Camera povCam;
        private PlayerController pc = PlayerController.Instance;
        private EventManager em = EventManager.Instance;
        HeadIK headIK = UnityEngine.Object.FindObjectOfType<HeadIK>();

        private Vector3 pushingVectorOffset = new Vector3(-20f, -15f, -8f);
        private Vector3 crouchingVectorOffset = Vector3.zero;

        private StickInput leftStick;
        private StickInput rightStick;
        float blendAmount = 0.0f;

        void Start()
        {
            InputController ic = InputController.Instance;
            leftStick = ic.LeftStick;
            rightStick = ic.RightStick;
        }

        void Update()
        {
            if (povCam == null || !povCam.enabled) return;

            float rPos = (float)System.Math.Round(rightStick.rawInput.prevPos.y, 3);
            float lPos = (float)System.Math.Round(leftStick.rawInput.prevPos.y, 3);

            if ( pc.popped || !pc.IsGrounded() || !pc.boardController.Grounded ||
                em.IsInAir || em.IsGrabbing || em.IsGrinding )
            {
                blendAmount = 1.0f;
            }
            else
            {
                blendAmount = lPos > 0.0f ? lPos : rPos < 0.0f ? Mathf.Abs(rPos) : 0.0f;
            }
            Vector3 cameraRotationOffset = Vector3.Lerp(pushingVectorOffset, Vector3.zero, blendAmount);
            GameObject pov = povCam.gameObject;
            pov.transform.rotation = headIK.head.rotation * Quaternion.Euler(new Vector3(-100f, 13f, 43f));
            pov.transform.localRotation = pov.transform.localRotation * Quaternion.Euler(new Vector3(20f, 0f, 50f) + cameraRotationOffset);
        }

        IEnumerator FadeCamera(float fadeTime)
        {
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
            {
                blendAmount = Mathf.Lerp(blendAmount, 0.0f, t);
                yield return null;
            }
            blendAmount = 0.0f;

        }
    }
}
