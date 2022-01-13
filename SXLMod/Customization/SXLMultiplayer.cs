using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SXLMod.Console;

using GameManagement;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.HighDefinition;

namespace SXLMod.Customization
{
    public static class SXLMultiplayer
    {
        private static MultiplayerManager s_MultiplayerManager;
        public static MultiplayerManager Manager
        {
            get
            {
                if (s_MultiplayerManager == null) s_MultiplayerManager = MonoBehaviourPunCallbacksSingleton<MultiplayerManager>.Instance;

                return s_MultiplayerManager;
            }
        }
        
        private static GameObject s_SpectateCameraRoot;
        private static RenderTexture s_SpectateRT;
        public static RenderTexture SpectateRT {
            get
            {
                if (s_SpectateRT == null) s_SpectateRT = RenderTexture.GetTemporary(1280, 720);

                return s_SpectateRT;
            }
        }

        private static Material s_SpectateMat;
        public static Material SpectateMat {
            get {
                if (!s_SpectateMat)
                {
                    s_SpectateMat = new Material(Shader.Find("HDRP/Unlit"));
                    s_SpectateMat.SetTexture("_UnlitColorMap", SpectateRT);
                }

                return SpectateMat;
            }
        }

        private static SXLMultiplayerSpectator s_MultiplayerSpectator;
        public static RawImage SpectatorUI;
        private static Camera s_SpectateCamera;
        public static Camera SpectateCamera
        {
            get {
                if (!s_SpectateCamera)
                {
                    s_SpectateCameraRoot = new GameObject("SXLSpectatorCameraRoot");
                    s_SpectateCamera = s_SpectateCameraRoot.AddComponent<Camera>();
                    s_SpectateCamera.targetTexture = s_SpectateRT;
                    s_MultiplayerSpectator = s_SpectateCameraRoot.AddComponent<SXLMultiplayerSpectator>();
                    s_SpectateCamera.depth = 10;
                    s_SpectateCamera.rect = new Rect(0, 0, 0.125f, 0.2f);
                }
                return s_SpectateCamera;
            }
        }

        public static List<NetworkPlayerController> GetAllPlayers()
        {
            List<NetworkPlayerController> netPlayers = new List<NetworkPlayerController>();

            foreach (NetworkPlayerController netPlayer in Manager.networkPlayers.Values)
                netPlayers.Add(netPlayer);

            return netPlayers;
        }

        public static NetworkPlayerController GetPlayerByName(string name)
        {
            foreach (NetworkPlayerController netPlayer in Manager.networkPlayers.Values)
            {
                if (netPlayer.NickName.ToLower().Contains(name))
                    return netPlayer;
            }

            return null;
        }

        public static void AttachSpectateCameraToPlayer(string name)
        {
            if (SpectateCamera == null) return;

            foreach (NetworkPlayerController player in Manager.networkPlayers.Values)
            {
                if (player.NickName.ToLower() == name.ToLower())
                {
                    // Parent the camera to player body root
                    Transform camXForm = s_SpectateCameraRoot.transform;
                    camXForm.SetParent(player.GetBody().transform);
                    camXForm.position = Vector3.zero;
                    camXForm.localPosition = Vector3.zero + new Vector3(0, 1f, 2f);
                    camXForm.transform.LookAt(player.GetBody().transform);
                    SpectatorUI.enabled = true;

                    return;
                }
            }
            
        }


    }
}
