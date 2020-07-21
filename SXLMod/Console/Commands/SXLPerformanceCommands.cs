using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

namespace SXLMod.Console
{
    public class SXLConsolePerformance
    {
        private bool _fps = false;
        protected float minFPS = -1f;
        protected float maxFPS = -1f;
        protected float avgFPS = 0.0f;
        protected List<float> fpsList = new List<float>();

        private bool _system = false;
        private bool _memory = false;
        private bool _physics = false;
        private bool _lighting = false;
        private bool _actors = false;

        public SXLConsolePerformance()
        { 

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
            Profiler.enabled = enabled;
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

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            this.minFPS = -1f;
            this.maxFPS = -1f;
        }

        private void EvaluateFramerate(float framerate)
        {
            this.maxFPS = this.maxFPS == -1 ? framerate : framerate > this.maxFPS ? framerate : this.maxFPS;
            this.minFPS = this.minFPS == -1 ? framerate : framerate < this.minFPS ? framerate : this.minFPS;
        }

        private static float GetRuntimeMemorySize<T>(T asset) where T: Object
        {
            long memInBytes = Profiler.GetRuntimeMemorySizeLong(asset);
            return (float)memInBytes * 0.001f;
        }

        private static float GetTotalRuntimeMemorySize<T>() where T: Object
        {
            float totalMemory = 0.0f;
            foreach (T asset in Resources.FindObjectsOfTypeAll<T>())
            {
                totalMemory += GetRuntimeMemorySize<T>(asset);
            }
            return totalMemory;
        }


        public void DrawPerfUI(int windowID)
        {
            GUILayout.BeginHorizontal();
            if (this._system)
            {
                string sysOS = SystemInfo.operatingSystem;
                int sysMemory = SystemInfo.systemMemorySize;
                string sysProcessor = SystemInfo.processorType;

                GUILayout.Label($"<b>PROCESSOR</b>\n{sysProcessor}\n{sysOS} {(int)(sysMemory / 1000f)}GB RAM", SXLConsoleUI.labelStyle);
                GUILayout.Space(20f);
            }
            if (this._fps)
            {
                string gpuName = SystemInfo.graphicsDeviceName;
                int gpuMemory = SystemInfo.graphicsMemorySize;
                float framerate = (float)System.Math.Round(1f / Time.unscaledDeltaTime, 2);
                this.fpsList.Add(framerate);
                this.EvaluateFramerate(framerate);
                float ms = (float)System.Math.Round((1 / framerate) * 1000, 1);
                string color = ms <= 16.0f ? "green" : ms < 30f ? "yellow" : "red";

                if (this.fpsList.Count >= 1000)
                {
                    this.avgFPS = (float)System.Math.Round(this.fpsList.Average(), 2);
                    this.fpsList.Clear();
                }
                GUILayout.Label($"<b>{gpuName} {gpuMemory}MB</b>\nFPS: {framerate}\n<i>MIN</i>: {this.minFPS} | <i>MAX</i>: {this.maxFPS} | <i>AVERAGE</i>: {this.avgFPS}\nFRAME: <color={color}>{ms}ms</color>", SXLConsoleUI.labelStyle);
                GUILayout.Space(20f);
            }
            if (this._memory)
            {
                long totalMem = Profiler.GetTotalAllocatedMemoryLong() / 1048576;
                long reservedMem = Profiler.GetTotalReservedMemoryLong() / 1048576;
                float textureMem = GetTotalRuntimeMemorySize<Texture>();
                float meshMem = GetTotalRuntimeMemorySize<Mesh>();
                float materialMem = GetTotalRuntimeMemorySize<Material>();

                GUILayout.Label($"<b>MEMORY</b>\nALLOCATED: {totalMem}MB\nRESERVED: {reservedMem}MB\nTEXTURE: {textureMem}\nMESH: {meshMem}\nMATERIAL: {materialMem}", SXLConsoleUI.labelStyle);
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

                GUILayout.Label($"<b>PHYSICS</b>\nTOTAL COLLIDERS: {total}\nTOTAL IN VIEW: {totalView}\n{subCounts}", SXLConsoleUI.labelStyle);
                GUILayout.Space(20f);
            }
            if (this._lighting)
            {
                Light[] lights = Object.FindObjectsOfType<Light>();
                ReflectionProbe[] reflectionProbes = Object.FindObjectsOfType<ReflectionProbe>();

                int baked = lights.Where(light => light.bakingOutput.lightmapBakeType == LightmapBakeType.Baked).Count();
                int realtime = lights.Where(light => light.bakingOutput.lightmapBakeType == LightmapBakeType.Realtime).Count();
                int mixed = lights.Length - (baked + realtime);
                int probeRT = reflectionProbes.Where(probe => probe.mode == UnityEngine.Rendering.ReflectionProbeMode.Realtime).Count();
                int probeBaked = reflectionProbes.Where(probe => probe.mode == UnityEngine.Rendering.ReflectionProbeMode.Baked).Count();
                int probeCustom = reflectionProbes.Length - (probeRT + probeBaked);

                GUILayout.Label($"<b>LIGHTING</b>\nTOTAL: {lights.Length}\nREALTIME: {realtime}\nMIXED: {mixed}\nBAKED: {baked}\n<b>REFLECTION PROBES</b>\nRT: {probeRT} BAKED: {probeBaked} CUSTOM: {probeCustom}", SXLConsoleUI.labelStyle);
                GUILayout.Space(20f);
            }
            if (this._actors)
            {
                LODGroup[] lodGroups = Object.FindObjectsOfType<LODGroup>();
                Renderer[] sceneRenderers = Object.FindObjectsOfType<Renderer>();
                Renderer[] visible = sceneRenderers.Where(r => r.isVisible).ToArray();
                DecalProjector[] decals = Object.FindObjectsOfType<DecalProjector>();

                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
                Renderer[] inFrustrum = visible.Where(v => GeometryUtility.TestPlanesAABB(planes, v.bounds) == true).ToArray();
                int decalsInFrustrum = decals.Where(d => GeometryUtility.TestPlanesAABB(planes, new Bounds(d.gameObject.transform.position, d.size)) == true).Count();
                
                GUILayout.Label($"<b>ACTORS</b>\nVISIBLE: {visible.Length}\nTOTAL: {sceneRenderers.Length}\nLOD GROUPS: {lodGroups.Length}\n\nDECALS: {decalsInFrustrum} / {decals.Length}", SXLConsoleUI.labelStyle);
                GUILayout.Space(20f);
            }
            GUILayout.EndHorizontal();
        }

        public void DrawUI()
        {
            GUILayout.Window(99, new Rect(0, 0, Screen.width, 0f), this.DrawPerfUI, "", SXLConsoleUI.windowStyle);
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
