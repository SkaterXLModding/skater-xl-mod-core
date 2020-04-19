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

        private Rect debugWindow;
        private GUIStyle windowStyle;
        private GUIStyle labelStyle;

        public SXLConsoleDebug()
        {
            this.SetupUI();
        }

        public void SetupUI()
        {
            this.windowStyle = new GUIStyle();
            this.windowStyle.normal.textColor = Color.black;
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
            t.Apply();
            this.windowStyle.normal.background = t;

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

        public void ToggleTrickInfo()
        {
            this._trick = !this._trick;
        }

        private void DrawDebugUI(int windowID)
        {
            GUILayout.BeginHorizontal();
            if (this._trick)
            {
                List<string> combo = new List<string>();
                var manager = Traverse.Create(RunManager.Instance);
                List<GPEvent> comboList = manager.Field("comboEvents").GetValue() as List<GPEvent>;

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

                GUILayout.Label($"<b>TRICK DEBUG</b>\nPOP TYPE: {this._lastPop}\nLAST FLIP TRICK: {this._lastFlip}\nLAST GRIND TRICK: {this._lastGrind}\nLAST GRAB: {this._lastGrab}\nLAST SEQUENCE: {this._lastCombo}", this.labelStyle);
                GUILayout.Space(20f);
            }

            GUILayout.EndHorizontal();
        }

        public void DrawUI()
        {
            this.debugWindow = GUILayout.Window(100, new Rect(0, Screen.height - 110f, Screen.width, 0f), this.DrawDebugUI, "", this.windowStyle);
        }
    }

    class SXLDebugCommands
    {
        [RegisterCommand(Name = "DEBUG", Help = "Display Debug Commands", Hint = "<TRICK> <0 | 1>", ArgMin = 1, ArgMax = 1)]
        static void CommandDebug(CommandArg[] args)
        {
            if (args.Length != 1)
            {
                return;
            }
            
            switch (args[0].ToString().ToLower())
            {
                case "trick":
                    SXLConsole.Instance.Debug.ToggleTrickInfo();
                    break;
                default:
                    break;
            }
        }
    }
}
