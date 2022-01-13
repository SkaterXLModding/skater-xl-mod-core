using System.Collections;

using UnityEngine;

namespace SXLMod.Customization
{
    public static class SXLPlayer
    {
        private static POVCameraController POV_CONTROLLER;
        private static Camera MAIN_CAMERA;

        public static void SetRealisticMode(bool value)
        {
            SXLFile.GetConfigFile().Write("p_realistic", value.ToString(), "player");
            SXLSettings.realisticMode = value;
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

        private static POVCameraController SetupCameraController(HeadIK headIK, float offset=0f)
        {
            GameObject pov = new GameObject("POV_Camera");
            pov.transform.parent = headIK.head.gameObject.transform;

            Camera camera = pov.AddComponent<Camera>();
            camera.nearClipPlane = 0.15f;
            camera.depth = 999;
            POVCameraController controller = pov.AddComponent<POVCameraController>();
            controller.povCam = camera;
            pov.transform.position = headIK.head.position + new Vector3(0f, offset, 0f);
            pov.transform.rotation = headIK.head.rotation * Quaternion.Euler(new Vector3(-100f, 13f, 43f));
            pov.transform.localRotation = pov.transform.localRotation * Quaternion.Euler(new Vector3(20f, 0f, 50f));

            return controller;
        }

        public static void SetPlayerView(int value, float fov, float headOffset)
        {
            HeadIK headIK = Object.FindObjectOfType<HeadIK>();
            // POV camera
            if (POV_CONTROLLER == null)
            {
                POV_CONTROLLER = SetupCameraController(headIK, headOffset);
            }

            if (value == 1)
            {
                POV_CONTROLLER.gameObject.transform.position = headIK.head.position + new Vector3(0f, headOffset, 0f);
                POV_CONTROLLER.povCam.fieldOfView = fov;
                POV_CONTROLLER.SetViewMode(POVCameraController.ViewMode.FPV);


                SXLSettings.firstPersonStereoView = false;
                SXLSettings.firstPersonView = true;
                SXLSettings.firstPersonViewFOV = fov;
            }
            else if (value == 2)
            {
                POV_CONTROLLER.gameObject.transform.position = headIK.head.position + new Vector3(0f, headOffset, 0f);
                POV_CONTROLLER.SetViewMode(POVCameraController.ViewMode.STEREO);
                
                SXLSettings.firstPersonView = false;
                SXLSettings.firstPersonStereoView = true;
            }
            else
            {
                POV_CONTROLLER.SetViewMode(POVCameraController.ViewMode.DEFAULT);
                SXLSettings.firstPersonView = false;
                SXLSettings.firstPersonStereoView = false;
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
        public enum ViewMode { DEFAULT, FPV, STEREO };

        public Camera povCam;
        private Camera leftEyeCam;
        private Camera rightEyeCam;

        private PlayerController pc = PlayerController.Instance;
        private EventManager em = EventManager.Instance;
        HeadIK headIK = UnityEngine.Object.FindObjectOfType<HeadIK>();

        private Vector3 pushingVectorOffset = new Vector3(-20f, -15f, -8f);
        private Vector3 crouchingVectorOffset = Vector3.zero;

        private StickInput leftStick;
        private StickInput rightStick;
        float blendAmount = 0.0f;

        public bool isStereoRendering = false;

        void Awake()
        {
            GameObject stereoCameraRoot = new GameObject("StereoCameraRoot");
            stereoCameraRoot.transform.SetParent(this.transform);

            GameObject leftEyeRoot = new GameObject("LeftEye");
            leftEyeRoot.transform.SetParent(stereoCameraRoot.transform);
            leftEyeRoot.transform.localPosition -= new Vector3(0.04f, 0f, 0f);
            leftEyeCam = leftEyeRoot.AddComponent<Camera>();
            leftEyeCam.rect = new Rect(0, 0, 0.5f, 1);
            leftEyeCam.enabled = false;

            GameObject rightEyeRoot = new GameObject("RightEye");
            rightEyeRoot.transform.SetParent(stereoCameraRoot.transform);
            rightEyeRoot.transform.localPosition += new Vector3(0.04f, 0f, 0f);
            rightEyeCam = rightEyeRoot.AddComponent<Camera>();
            rightEyeCam.rect = new Rect(0.5f, 0, 0.5f, 1);
            rightEyeCam.enabled = false;

            isStereoRendering = false;

        }

        void Start()
        {
            InputController ic = InputController.Instance;
            leftStick = ic.LeftStick;
            rightStick = ic.RightStick;
        }

        void Update()
        {
           if (povCam == null || !povCam.enabled)
                return;

           UpdateFPVCamera();
        }
        
        public void SetViewMode(ViewMode mode)
        {
            switch (mode)
            {
                case ViewMode.FPV:
                    leftEyeCam.enabled = false;
                    rightEyeCam.enabled = false;
                    povCam.enabled = true;
                    break;
                case ViewMode.STEREO:
                    leftEyeCam.enabled = true;
                    rightEyeCam.enabled = true;
                    povCam.enabled = false;
                    break;
                case ViewMode.DEFAULT:
                default:
                    leftEyeCam.enabled = false;
                    rightEyeCam.enabled = false;
                    povCam.enabled = false;
                    break;

            }
        }

        void UpdateFPVCamera()
        {
            float rPos = (float)System.Math.Round(rightStick.rawInput.prevPos.y, 3);
            float lPos = (float)System.Math.Round(leftStick.rawInput.prevPos.y, 3);

            if (pc.popped || !pc.IsGrounded() || !pc.boardController.Grounded ||
                em.IsInAir || em.IsGrabbing || em.IsGrinding)
            {
                blendAmount = 1.0f;
            }
            else
            {
                blendAmount = lPos > 0.0f ? lPos : rPos < 0.0f ? Mathf.Abs(rPos) : 0.0f;
            }
            Vector3 cameraRotationOffset = Vector3.Lerp(pushingVectorOffset, Vector3.zero, blendAmount);
            GameObject root = this.gameObject;
            root.transform.rotation = headIK.head.rotation * Quaternion.Euler(new Vector3(-100f, 13f, 43f));
            root.transform.localRotation = root.transform.localRotation * Quaternion.Euler(new Vector3(20f, 0f, 50f) + cameraRotationOffset);
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
