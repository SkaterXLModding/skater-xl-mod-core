using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SXLMod.Console
{
    public class SXLConsolePerformance
    {
        private bool _fps = false;
        private bool _system = false;
        private bool _visibleRenderer = false;
        private Rect perfWindow;
        private GUIStyle perfStyle;

        public SXLConsolePerformance()
        {
            this.SetupUI();
        }

        public void ToggleFPS()
        {
            this._fps = !this._fps;
        }

        public void ToggleSystemInfo()
        {
            this._system = !this._system;
        }

        public void ToggleVisibleRenderer()
        {
            this._visibleRenderer = !this._visibleRenderer;
        }

        public void Disable()
        {
            this._fps = false;
            this._system = false;
        }

        private void SetupUI()
        {
            this.perfStyle = new GUIStyle();
            this.perfStyle.normal.textColor = Color.black;
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
            t.Apply();
            this.perfStyle.normal.background = t;
        }

        private void DrawPerfUI(int windowID)
        {
            if (this._system)
            {
                string sysOS = SystemInfo.operatingSystem;
                int sysMemory = SystemInfo.systemMemorySize;
                string sysProcessor = SystemInfo.processorType;

                GUILayout.BeginHorizontal();
                GUILayout.Label($"{sysProcessor}\n{sysOS} {sysMemory}MB RAM");
                GUILayout.EndHorizontal();
            }
            if (this._fps)
            {
                string gpuName = SystemInfo.graphicsDeviceName;
                int gpuMemory = SystemInfo.graphicsMemorySize;
                float framerate = (float)System.Math.Round(1f / Time.unscaledDeltaTime, 2);
                float ms = (float)System.Math.Round((1 / framerate) * 1000, 1);
                string color = ms <= 16.0f ? "green" : ms < 30f ? "yellow" : "red";
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{gpuName} {gpuMemory}MB\nFPS: {framerate}\nFrame: <color={color}>{ms}ms</color>");
                GUILayout.EndHorizontal();
            }
            if (this._visibleRenderer)
            {
                Renderer[] sceneRenderers = Object.FindObjectsOfType<Renderer>();
                Renderer[] visible = sceneRenderers.Where(r => r.isVisible).ToArray();

                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
                Renderer[] inFrustrum = visible.Where(v => GeometryUtility.TestPlanesAABB(planes, v.bounds) == true).ToArray();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Visible Actors: [Camera Frustrum: <b>{inFrustrum.Length}</b>] [Total Visible: <b>{visible.Length}</b>] [Total Actors: <b>{sceneRenderers.Length}</b>]");
                GUILayout.EndHorizontal();
            }
        }

        public void DrawUI()
        {
            this.perfWindow = GUILayout.Window(99, new Rect(0, 0, Screen.width, 0f), this.DrawPerfUI, "", this.perfStyle);
        }
    }

    class SXLPerformanceCommands
    {
        [RegisterCommand(Name = "STAT", Help = "Display Status Commands", Hint = "<FPS | SYSTEM | ACTORS>", ArgMax = 1)]
        static void CommandStat(CommandArg[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            foreach (var a in args)
            {
                string arg = a.ToString().ToLower();
                if (arg == "fps")
                {
                    SXLConsole.Instance.Performance.ToggleFPS();
                }
                else if (arg == "system")
                {
                    SXLConsole.Instance.Performance.ToggleSystemInfo();
                }
                else if (arg == "actors")
                {
                    SXLConsole.Instance.Performance.ToggleVisibleRenderer();
                }
                else if (arg == "off")
                {
                    SXLConsole.Instance.Performance.Disable();
                }
            }
        }
    }
}
