using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace SXLMod.Console
{
    public class SXLConsolePerformance
    {
        private bool _fps = false;
        private bool _system = false;
        private bool _memory = false;
        private bool _physics = false;
        private bool _lighting = false;
        private bool _actors = false;
        private Rect perfWindow;
        private GUIStyle perfStyle;
        private GUIStyle labelStyle;

        public SXLConsolePerformance()
        {
            this.SetupUI();
        }

        public void ToggleFPS(bool enabled)
        {
            this._fps = enabled;
        }

        public void ToggleSystemInfo(bool enabled)
        {
            this._system = enabled;
        }

        public void ToggleMemoryInfo(bool enabled)
        {
            this._memory = enabled;
        }

        public void ToggleActorInfo(bool enabled)
        {
            this._actors = enabled;
        }

        public void TogglePhysicsInfo(bool enabled)
        {
            this._physics = enabled;
        }

        public void ToggleLightingInfo(bool enabled)
        {
            this._lighting = enabled;
        }

        public void SetupUI()
        {
            this.perfStyle = new GUIStyle();
            this.perfStyle.normal.textColor = Color.black;
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
            t.Apply();
            this.perfStyle.normal.background = t;

            this.labelStyle = new GUIStyle();
            this.labelStyle.normal.textColor = Color.white;
            this.labelStyle.fontSize = 14;
            this.labelStyle.stretchWidth = false;
            this.labelStyle.padding = new RectOffset(8, 8, 8, 8);
            Texture2D l = new Texture2D(1, 1);
            l.SetPixel(0, 0, new Color(.1f, .1f, .1f));
            l.Apply();
            this.labelStyle.normal.background = l;
        }

        private void DrawPerfUI(int windowID)
        {
            GUILayout.BeginHorizontal();
            if (this._system)
            {
                string sysOS = SystemInfo.operatingSystem;
                int sysMemory = SystemInfo.systemMemorySize;
                string sysProcessor = SystemInfo.processorType;

                GUILayout.Label($"<b>PROCESSOR</b>\n{sysProcessor}\n{sysOS} {(int)(sysMemory / 1000f)}GB RAM", this.labelStyle);
                GUILayout.Space(20f);
            }
            if (this._fps)
            {
                string gpuName = SystemInfo.graphicsDeviceName;
                int gpuMemory = SystemInfo.graphicsMemorySize;
                float framerate = (float)System.Math.Round(1f / Time.unscaledDeltaTime, 2);
                float ms = (float)System.Math.Round((1 / framerate) * 1000, 1);
                string color = ms <= 16.0f ? "green" : ms < 30f ? "yellow" : "red";

                GUILayout.Label($"<b>{gpuName} {gpuMemory}MB</b>\nFPS: {framerate}\nFRAME: <color={color}>{ms}ms</color>", this.labelStyle);
                GUILayout.Space(20f);
            }
            if (this._memory)
            {
                long totalMem = Profiler.GetTotalAllocatedMemoryLong() / 1048576;
                long reservedMem = Profiler.GetTotalReservedMemoryLong() / 1048576;

                GUILayout.Label($"<b>MEMORY</b>\nALLOCATED: {totalMem}MB\nRESERVED: {reservedMem}MB", this.labelStyle);
                GUILayout.Space(20f);
            }
            if (this._physics)
            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

                BoxCollider[] boxColliders = Object.FindObjectsOfType<BoxCollider>();
                BoxCollider[] visibleBox = boxColliders.Where(b => GeometryUtility.TestPlanesAABB(planes, b.bounds) == true).ToArray();

                CapsuleCollider[] capsuleColliders = Object.FindObjectsOfType<CapsuleCollider>();
                CapsuleCollider[] visibleCapsule = capsuleColliders.Where(c => GeometryUtility.TestPlanesAABB(planes, c.bounds) == true).ToArray();

                MeshCollider[] meshColliders = Object.FindObjectsOfType<MeshCollider>();
                MeshCollider[] visibleMesh = meshColliders.Where(m => GeometryUtility.TestPlanesAABB(planes, m.bounds) == true).ToArray();

                int total = boxColliders.Length + capsuleColliders.Length + meshColliders.Length;
                int totalView = visibleBox.Length + visibleCapsule.Length + visibleMesh.Length;

                string subCounts = $"BOX: {visibleBox.Length} / {boxColliders.Length}\nCAPSULE: {visibleCapsule.Length} / {capsuleColliders.Length}\nMESH: {visibleMesh.Length} / {meshColliders.Length}";

                GUILayout.Label($"<b>PHYSICS</b>\nTOTAL COLLIDERS: {total}\nTOTAL IN VIEW: {totalView}\n{subCounts}", this.labelStyle);
                GUILayout.Space(20f);
            }
            if (this._lighting)
            {
                Light[] lights = Object.FindObjectsOfType<Light>();
                int baked = lights.Where(light => light.bakingOutput.lightmapBakeType == LightmapBakeType.Baked).Count();
                int realtime = lights.Where(light => light.bakingOutput.lightmapBakeType == LightmapBakeType.Realtime).Count();
                int mixed = lights.Length - (baked + realtime);

                GUILayout.Label($"<b>LIGHTING</b>\nTOTAL: {lights.Length}\nREALTIME: {realtime}\nMIXED: {mixed}\nBAKED: {baked}", this.labelStyle);
                GUILayout.Space(20f);
            }
            if (this._actors)
            {
                LODGroup[] lodGroups = Object.FindObjectsOfType<LODGroup>();
                Renderer[] sceneRenderers = Object.FindObjectsOfType<Renderer>();
                Renderer[] visible = sceneRenderers.Where(r => r.isVisible).ToArray();

                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
                Renderer[] inFrustrum = visible.Where(v => GeometryUtility.TestPlanesAABB(planes, v.bounds) == true).ToArray();
                
                GUILayout.Label($"<b>ACTORS</b>\nVISIBLE: {visible.Length}\nTOTAL: {sceneRenderers.Length}\nLOD GROUPS: {lodGroups.Length}", this.labelStyle);
                GUILayout.Space(20f);
            }
            GUILayout.EndHorizontal();
        }

        public void DrawUI()
        {
            this.perfWindow = GUILayout.Window(99, new Rect(0, 0, Screen.width, 0f), this.DrawPerfUI, "", this.perfStyle);
        }
    }

    class SXLPerformanceCommands
    {
        [RegisterCommand(Name = "stat_fps", Help = "Display FPS Stats", Hint = "stat_fps <0|1>")]
        static void CommandStatFPS(CommandArg[] args)
        {
            switch (args[0].Int)
            {
                case 0:
                    SXLConsole.Instance.Performance.ToggleFPS(false);
                    break;
                case 1:
                    SXLConsole.Instance.Performance.ToggleFPS(true);
                    break;
                default:
                    break;
            }
        }

        [RegisterCommand(Name = "stat_system", Help = "Display System Information", Hint = "stat_system <0|1>")]
        static void CommandStatSystem(CommandArg[] args)
        {
            switch (args[0].Int)
            {
                case 0:
                    SXLConsole.Instance.Performance.ToggleSystemInfo(false);
                    break;
                case 1:
                    SXLConsole.Instance.Performance.ToggleSystemInfo(true);
                    break;
                default:
                    break;
            }
        }

        [RegisterCommand(Name = "stat_memory", Help = "Display Memory Stats", Hint = "stat_memory <0|1>")]
        static void CommandStatMemory(CommandArg[] args)
        {
            switch (args[0].Int)
            {
                case 0:
                    SXLConsole.Instance.Performance.ToggleMemoryInfo(false);
                    break;
                case 1:
                    SXLConsole.Instance.Performance.ToggleMemoryInfo(true);
                    break;
                default:
                    break;
            }
        }

        [RegisterCommand(Name = "stat_physics", Help = "Display Physics Stats", Hint = "stat_physics <0|1>")]
        static void CommandStatPhysics(CommandArg[] args)
        {
            switch (args[0].Int)
            {
                case 0:
                    SXLConsole.Instance.Performance.TogglePhysicsInfo(false);
                    break;
                case 1:
                    SXLConsole.Instance.Performance.TogglePhysicsInfo(true);
                    break;
                default:
                    break;
            }
        }

        [RegisterCommand(Name = "stat_lighting", Help = "Display Lighting Stats", Hint = "stat_lighting <0|1>")]
        static void CommandStatLighting(CommandArg[] args)
        {
            switch (args[0].Int)
            {
                case 0:
                    SXLConsole.Instance.Performance.ToggleLightingInfo(false);
                    break;
                case 1:
                    SXLConsole.Instance.Performance.ToggleLightingInfo(true);
                    break;
                default:
                    break;
            }
        }

        [RegisterCommand(Name = "stat_actors", Help = "Display Actor Stats", Hint = "stat_actors <0|1>")]
        static void CommandStatActors(CommandArg[] args)
        {
            switch (args[0].Int)
            {
                case 0:
                    SXLConsole.Instance.Performance.ToggleActorInfo(false);
                    break;
                case 1:
                    SXLConsole.Instance.Performance.ToggleActorInfo(true);
                    break;
                default:
                    break;
            }
        }
    }
}
