using System;
using System.Collections;

using UnityEngine;
using HarmonyLib;

using SXLMod.Console;

namespace SXLMod.Customization
{
    public static class SXLCamera
    {
        public static readonly Camera MainCamera = Camera.main;
        private static readonly Vector3 s_DefaultCameraPosition = new Vector3(0, 0, 0);
        private static SXLCameraShake s_ShakeComponent = null;
        private static SXLPlayerCameraController s_SXLCameraController = null;

        public static void SetFilmerCameraPosition(Vector3 offset)
        {
            SXLFile.GetConfigFile().Write("c_cameraoffset", $"({offset.x},{offset.y},{offset.z})", "camera");
            
            Vector3 pos = offset + s_DefaultCameraPosition;
            SXLSettings.cameraPosition = pos;
            
            MainCamera.gameObject.transform.localPosition = pos;
        }

        public static void SetFollowCamera(bool active)
        {
            SXLFile.GetConfigFile().Write("c_camerafollow", active ? "True" : "False", "camera");
            SXLSettings.followCamera = active;
            MainCamera.fieldOfView = active ? SXLSettings.followCameraFOV : SXLSettings.fieldOfView;

            if (s_SXLCameraController == null)
                s_SXLCameraController = MainCamera.gameObject.AddComponent<SXLPlayerCameraController>();

            s_SXLCameraController.State = active ? CameraState.FOLLOW : CameraState.DEFAULT;
        }

        public static void SetSkateCamera(bool active)
        {
            SXLFile.GetConfigFile().Write("c_cameraskate", active ? "True" : "False", "camera");
            SXLSettings.skateCamera = active;

            if (s_SXLCameraController == null)
                s_SXLCameraController = MainCamera.gameObject.AddComponent<SXLPlayerCameraController>();

            s_SXLCameraController.State = active ? CameraState.SKATE : CameraState.DEFAULT;
        }

        public static void SetCameraShake(bool active)
        {
            if (s_ShakeComponent == null)
                s_ShakeComponent = MainCamera.gameObject.AddComponent<SXLCameraShake>();

            s_ShakeComponent.gameObject.SetActive(active);
        }

        public static void SetCameraSettingsFromConfig()
        {
            SXLConfiguration config = SXLFile.GetConfigFile();

            string[] positionConfigStr = config.TryGet("c_cameraoffset", "camera", "").Split(',');
            SXLSettings.followCamera = bool.Parse(config.TryGet("c_camerafollow", "camera", "false"));

            SetFollowCamera(SXLSettings.followCamera);

            if (positionConfigStr.Length == 3)
                SetFilmerCameraPosition(new Vector3(Int32.Parse(positionConfigStr[0]), Int32.Parse(positionConfigStr[1]), Int32.Parse(positionConfigStr[2])));
        }
    }

    public enum CameraState
    {
        DEFAULT,
        FOLLOW,
        SKATE
    }

    public class SXLCameraShake : MonoBehaviour
    {

        void Update()
        {

        }
    }

    public class SXLPlayerCameraController : MonoBehaviour
    {
        private CameraState m_CameraState = CameraState.DEFAULT;

        public CameraState State
        {
            set {  m_CameraState = value; }
        }

        private SkaterController m_Skater;
        public SkaterController Skater
        {
            get
            {
                if (m_Skater == null) m_Skater = PlayerController.Instance.skaterController;
                return m_Skater;
            }
        }

        private BoardController m_Board;
        private BoardController Board
        {
            get
            {
                if (m_Board == null) m_Board = PlayerController.Instance.boardController;
                return m_Board;
            }
        }

        private Transform CameraTransform
        {
            get 
            {
                return PlayerController.Instance.cameraController._actualCam;
            }
        }

        private float m_OldOffsetX = 0.0f;
        private Vector3 m_OldPos = Vector3.zero;
        private Quaternion m_OldRot = Quaternion.identity;
        private TransformStore m_Xform;
        private Vector3 m_Offset = Vector3.zero;

        public float followCamYOffset = 0;

        private Traverse<bool> m_CameraControllerTraverse;

        void Start()
        {
            m_CameraControllerTraverse = Traverse.Create(PlayerController.Instance.cameraController).Field<bool>("_right");
            m_OldPos = CameraTransform.position;
            m_OldRot = CameraTransform.rotation;
        }

        void LateUpdate()
        {
            switch(m_CameraState)
            {
                case CameraState.FOLLOW:
                    UpdateFollowCamera();
                    break;
                case CameraState.SKATE:
                    UpdateSkateCamera();
                    break;
                case CameraState.DEFAULT:
                default:
                    return;
            }
        }

        private void UpdateFollowCamera()
        {
            Vector3 position = Skater.skaterTransform.position;
            position.y = ((position.y + Board.boardTransform.position.y) * 0.5f) + followCamYOffset;

            if (m_Xform == null)
                m_Xform = new TransformStore();

            m_Xform.position = position;
            m_Xform.rotation = CameraTransform.rotation;

            m_Offset.x = Math.Abs(m_Offset.x);
            Vector3 followOffset = m_Offset;

            if (!m_CameraControllerTraverse.Value)
                followOffset.x *= -1f;

            m_OldOffsetX = followOffset.x = Mathf.Lerp(m_OldOffsetX, followOffset.x, 0.02f);
            m_Xform.position = transform.TransformPoint(followOffset);
            position = m_Xform.position;
            position.y = Mathf.Max(m_Xform.position.y, Board.boardTransform.position.y + 0.2f);
            m_OldPos = CameraTransform.position = Vector3.Lerp(m_OldPos, position, 0.7f);
            m_OldRot = CameraTransform.rotation = Quaternion.Lerp(m_OldRot, CameraTransform.rotation, 0.7f);
        }

        private void UpdateSkateCamera()
        {
            CameraTransform.position = Board.boardTransform.position;
            CameraTransform.rotation = Board.boardTransform.rotation;
            CameraTransform.position = CameraTransform.TransformPoint(Vector3.zero);
            m_OldPos = CameraTransform.position = Vector3.Lerp(m_OldPos, CameraTransform.position, 0.9f);
            m_OldRot = CameraTransform.rotation = Quaternion.Lerp(m_OldRot, CameraTransform.rotation, 0.9f);
        }
    }
}