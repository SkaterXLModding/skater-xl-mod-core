using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

using Harmony12;

namespace SXLMod.Console
{
    public class SXLConsoleDebug
    {
        private bool _trick = false;
        private string _lastPop = "--";
        private string _lastFlip = "--";
        private string _lastGrind = "--";
        private string _lastGrab = "--";
        private string _lastManual = "--";
        private string _lastCombo = "--";

        private bool _pState = false;
        private string _lastState = "--";
        private string _currentState = "--";

        public SXLConsoleDebug()
        {

        }

        public void ToggleTrickInfo(bool enabled)
        {
            this._trick = enabled;
        }

        public void ToggleStateInfo(bool enabled)
        {
            this._pState = enabled;
        }

        public void DrawDebugUI(int windowID)
        {
            GUILayout.BeginHorizontal();
            if (this._trick)
            {
                List<string> combo = new List<string>();
                var manager = Traverse.Create(RunManager.Instance);
                List<GPEvent> comboList = manager.Field("comboEvents").GetValue() as List<GPEvent>;
                /* TODO: FIX TRICK FOR v0.3b
                if (comboList != null && comboList.Count > 0)
                {
                    foreach (GPEvent e in comboList)
                    {
                        var type = e.GetType();
                        if (type == typeof(JumpEvent))
                        {
                            this._lastPop = (e as JumpEvent).popType.ToString();
                        }
                        if (type == typeof(FlipEvent))
                        {
                            JumpEvent j = RunManager.Instance.GetLastJumpEvent();
                            if (j == null)
                            {
                                continue;
                            }
                            this._lastFlip = (e as FlipEvent).GetTrickName(j.popType);
                            combo.Add(this._lastFlip);
                        }
                        if (type == typeof(GrindEvent))
                        {
                            this._lastGrind = (e as GrindEvent).GetTrickName();
                            combo.Add(this._lastGrind);
                        }
                        if (type == typeof(GrabEvent))
                        {
                            this._lastGrab = (e as GrabEvent).GetTrickName();
                            combo.Add(this._lastGrab);
                        }
                        if (type == typeof(ManualEvent))
                        {
                            this._lastManual = (e as ManualEvent).GetTrickName();
                            combo.Add(this._lastManual);
                        }
                    }
                }
                this._lastCombo = combo.Count > 0 ? string.Join(" => ", combo) : this._lastCombo;

                GUILayout.Label($"<b>TRICK DEBUG</b>\nPOP TYPE: {this._lastPop}\nLAST FLIP TRICK: {this._lastFlip}\nLAST GRIND TRICK: {this._lastGrind}\nLAST GRAB: {this._lastGrab}\nLAST SEQUENCE: {this._lastCombo}", SXLConsoleUI.labelStyle);
                GUILayout.Space(20f);
                */
            }

            if (this._pState)
            {
                this._lastState = this._currentState;
                this._currentState = PlayerController.Instance.currentState;

                GUILayout.Label($"<b>PLAYER</b>\nSTATE: <i>LAST</i>: {this._lastState} | <i>CURRENT</i>: {this._currentState}", SXLConsoleUI.labelStyle);
                GUILayout.Space(20f);
            }

            GUILayout.EndHorizontal();
        }

        public void DrawUI()
        {
            GUILayout.Window(100, new Rect(0, Screen.height - 110f, Screen.width, 0f), this.DrawDebugUI, "", SXLConsoleUI.windowStyle);
        }
    }

    class SXLDebugCommands
    {
        [RegisterCommand(Name = "d_trick", Help = "Debug Tricks", Hint = "d_trick <0|1>", ArgMin = 1, ArgMax = 1)]
        static void CommandDebug(CommandArg[] args)
        {
            switch (args[0].Int)
            {
                case 0:
                    SXLConsole.Instance.Debug.ToggleTrickInfo(false);
                    break;
                case 1:
                    SXLConsole.Instance.Debug.ToggleTrickInfo(true);
                    break;
                default:
                    break;
            }
        }

        [RegisterCommand(Name = "d_pstate", Help = "Debug Player State", Hint = "d_pstate <0|1>", ArgMin = 1, ArgMax = 1)]
        static void CommandPlayerState(CommandArg[] args)
        {
            switch (args[0].Int)
            {
                case 0:
                    SXLConsole.Instance.Debug.ToggleStateInfo(false);
                    break;
                case 1: SXLConsole.Instance.Debug.ToggleStateInfo(true);
                    break;
                default:
                    break;
            }
        }

        [RegisterCommand(Name = "d_dumptextures", Help = "Dump texture data tied to Renderer objects loaded in the scene", Hint = "d_texture", ArgMax = 0)]
        static void CommandDumpTextureData(CommandArg[] args)
        {
            List<System.Tuple<string, float, float>> textures = new List<System.Tuple<string, float, float>>();

            foreach (Renderer r in (Renderer[])Object.FindObjectsOfType(typeof(Renderer)))
            {
                foreach (Material m in r.materials)
                {
                    string[] properties = m.GetTexturePropertyNames();

                    foreach (string p in properties)
                    {
                        Texture t = m.GetTexture(p);
                        if (t == null)
                        {
                            continue;
                        }
                        textures.Add(new System.Tuple<string, float, float>(t.name, t.width, t.height));
                    }
                }
            }

            var orderedTex = textures.Distinct().OrderBy(t => t.Item2);
            var texCounts = orderedTex.GroupBy(t => t.Item2).ToDictionary(d => d.Key, d => d.Count());

            foreach (var texture in orderedTex)
            {
                Debug.Log($"{texture.Item1} {texture.Item2}x{texture.Item3}");
            }

            List<string> totals = new List<string>();
            foreach (var tuple in texCounts)
            {
                totals.Add($"{tuple.Key}x{tuple.Key} - {tuple.Value}");
            }
            Debug.Log($"<b>TOTAL</b>: {string.Join(" | ", totals)}");
        }
        
        [RegisterCommand(Name = "d_fixhdrp", Help = "Run HDRP Fix on current map", Hint = "d_fixhdrp", ArgMax=0)]
        static void CommandFixHDRP(CommandArg[] args)
        {
            Shader hdrpLit = Shader.Find("HDRP/Lit");
            Shader hdrpDecal = Shader.Find("HDRP/Decal");
            Material debugMat = new Material(hdrpLit);
            debugMat.color = Color.red;

            Debug.Log("Running Shader Fixes...");
            foreach (Renderer renderer in UnityEngine.Object.FindObjectsOfType<Renderer>())
            {
                Material[] sharedMaterials = renderer.sharedMaterials;
                for (int index = 0; index < sharedMaterials.Length; ++index)
                {
                    try
                    {
                        Shader s = sharedMaterials[index].shader;
                        bool hdrp = s.name.Contains("HDRP");
                        bool legacyHDRP = s.name.Contains("HDRenderPipeline");
                        sharedMaterials[index].shader = hdrp ? Shader.Find(s.name) : legacyHDRP ? s.name.Contains("Decal") ? hdrpDecal : hdrpLit : Shader.Find(s.name);
                        continue;
                    }
                    catch
                    {
                        Debug.Log($"{renderer.gameObject.name} Has a material ({index}) that needs to be addressed.");
                        renderer.material = debugMat;
                    }
                    continue;
                }
            }
        }
    }
}
