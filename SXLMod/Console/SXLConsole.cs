using System.Text;
using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace SXLMod.Console
{
    public enum SXLConsoleState
    {
        CLOSED,
        OPENSHORT,
        OPENFULL
    }

    public class SXLConsole : MonoBehaviour
    {
        // Window
        float maxHeight = 0.7f;
        float smallConsoleRatio = 0.33f;
        float toggleSpeed = 720f;
        Rect window;
        KeyCode toggleKey = KeyCode.BackQuote;

        int bufferSize = 512;

        // Input
        Font consoleFont;
        string inputCaret = ">";
        bool showGUIButtons = false;
        bool rightAlignButtons = false;

        // Theme
        float inputContrast = 0.0f;
        float inputAlpha = 0.5f;

        Color backgroundColor = new Color(0f, 0f, 0f, 0.5f);
        Color foregroundColor = Color.white;
        Color shellColor = Color.white;
        Color inputColor = Color.green;
        Color warningColor = Color.yellow;
        Color errorColor = Color.red;

        // Class
        SXLConsoleState state;
        public SXLConsoleState CurrentState {
            get { return this.state; }
        }
        TextEditor editorState;
        bool inputFix;
        bool moveCursor;
        bool initialOpen;
        float currentOpenTime;
        float openTarget;
        float realWindowSize;
        string commandText;
        string cachedCommandText;
        Vector2 scrollPosition;
        GUIStyle windowStyle;
        GUIStyle labelStyle;
        GUIStyle inputStyle;
        Texture2D backgroundTexture;
        Texture2D inputBackgroundTexture;

        private SXLConsolePerformance _performance;

        public SXLConsolePerformance Performance {
            get {
                if (this._performance == null)
                {
                    this._performance = new SXLConsolePerformance();
                }
                return this._performance;
            }
        }

        private SXLConsoleDebug _debug;

        public SXLConsoleDebug Debug {
            get {
                if (this._debug == null)
                {
                    this._debug = new SXLConsoleDebug();
                }
                return this._debug;
            }
        }

        public static SXLConsole Instance { get; private set; }
        public static CommandLog Buffer { get; private set; }
        public static CommandShell Shell { get; private set; }
        public static CommandHistory History { get; private set; }
        public static CommandAutoComplete Autocomplete { get; private set; }

        public static bool IssuedError {
            get { return Shell.IssuedErrorMessage != null; }
        }

        public bool IsClosed {
            get { return this.state == SXLConsoleState.CLOSED && Mathf.Approximately(currentOpenTime, openTarget); }
        }

        public static void Log(string format, params object[] message)
        {
            Log(TerminalLogType.SHELL, format, message);
        }

        public static void Log(TerminalLogType type, string format, params object[] message)
        {
            Buffer.HandleLog(string.Format(format, message), type);
        }

        private void SetState(SXLConsoleState newState)
        {
            this.inputFix = true;
            this.cachedCommandText = this.commandText;
            commandText = "";

            switch (newState)
            {
                case SXLConsoleState.CLOSED:
                    GUI.FocusControl(null);
                    this.openTarget = 0;
                    break;
                case SXLConsoleState.OPENSHORT:
                    this.initialOpen = true;
                    this.openTarget = Screen.height * this.maxHeight * this.smallConsoleRatio;
                    if (this.currentOpenTime > this.openTarget)
                    {
                        // Prevent resizing if window vertical size is greater than small target
                        this.openTarget = 0;
                        this.state = SXLConsoleState.CLOSED;
                        return;
                    }
                    this.realWindowSize = this.openTarget;
                    this.scrollPosition.y = int.MaxValue;
                    break;
                case SXLConsoleState.OPENFULL:
                default:
                    this.realWindowSize = Screen.height * this.maxHeight;
                    this.openTarget = this.realWindowSize;
                    break;
            }
            this.state = newState;
        }

        public void ToggleState(SXLConsoleState currentState)
        {
            switch (currentState)
            {
                case SXLConsoleState.CLOSED:
                    this.SetState(SXLConsoleState.OPENSHORT);
                    break;
                case SXLConsoleState.OPENSHORT:
                    this.SetState(SXLConsoleState.OPENFULL);
                    break;
                case SXLConsoleState.OPENFULL:
                default:
                    this.SetState(SXLConsoleState.CLOSED);
                    break;
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                return;
            }
            Instance = this;
        }
        

        // Monobehaviour
        void OnEnable()
        {
            Buffer = new CommandLog(this.bufferSize);
            Shell = new CommandShell();
            History = new CommandHistory();
            Autocomplete = new CommandAutoComplete();

            // Hook up unity log events
            Application.logMessageReceivedThreaded += HandleUnityLog;
        }

        void OnDisable()
        {
            Application.logMessageReceivedThreaded -= HandleUnityLog;
        }

        void Start()
        {
            if (this.consoleFont == null)
            {
                this.consoleFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
            }

            this.commandText = "";
            this.cachedCommandText = commandText;
            Assert.AreNotEqual(this.toggleKey, KeyCode.Return, "Return is not a valid console toggle hotkey.");

            SetupWindow();
            SetupInput();
            SetupLabels();

            Shell.RegisterCommands();

            if (IssuedError)
            {
                Log(TerminalLogType.ERROR, "Error: {0}", Shell.IssuedErrorMessage);
            }

            foreach (var cmd in Shell.Commands)
            {
                Autocomplete.Register(cmd.Key);
            }
        }

        void OnGUI()
        {
            this.Performance.DrawUI();
            this.Debug.DrawUI();

            if (IsClosed)
            {
                return;
            }

            if (this.showGUIButtons)
            {
                this.DrawGUIButtons();
            }

            HandleOpenness();
            this.window = GUILayout.Window(88, window, DrawConsole, "", this.windowStyle);
        }

        // Console Logic

        void SetupWindow()
        {
            this.realWindowSize = Screen.height * this.maxHeight / 3;
            this.window = new Rect(0, this.currentOpenTime - this.realWindowSize, Screen.width, this.realWindowSize);

            // Set Color / Theme
            this.backgroundTexture = new Texture2D(1, 1);
            this.backgroundTexture.SetPixel(0, 0, this.backgroundColor);
            this.backgroundTexture.Apply();

            this.windowStyle = new GUIStyle();
            this.windowStyle.normal.background = this.backgroundTexture;
            this.windowStyle.padding = new RectOffset(0, 0, 4, 0);
            this.windowStyle.normal.textColor = this.foregroundColor;
            this.windowStyle.font = this.consoleFont;
        }

        void SetupLabels()
        {
            this.labelStyle = new GUIStyle();
            this.labelStyle.padding = new RectOffset(4, 4, 0, 0);
            this.labelStyle.font = this.consoleFont;
            this.labelStyle.fixedHeight = this.consoleFont.fontSize * 1.25f;
            this.labelStyle.normal.textColor = this.foregroundColor;
            this.labelStyle.wordWrap = true;
        }

        void SetupInput()
        {
            this.inputStyle = new GUIStyle();
            this.inputStyle.padding = new RectOffset(4, 4, 4, 4);
            this.inputStyle.font = this.consoleFont;
            this.inputStyle.fixedHeight = this.consoleFont.fontSize * 1.6f;
            this.inputStyle.normal.textColor = this.inputColor;

            var inputTint = new Color();
            inputTint.r = this.backgroundColor.r - this.inputContrast;
            inputTint.g = this.backgroundColor.g - this.inputContrast;
            inputTint.b = this.backgroundColor.b - this.inputContrast;
            inputTint.a = this.inputAlpha;

            this.inputBackgroundTexture = new Texture2D(1, 1);
            this.inputBackgroundTexture.SetPixel(0, 0, inputTint);
            this.inputBackgroundTexture.Apply();
            this.inputStyle.normal.background = this.inputBackgroundTexture;
        }

        void DrawConsole(int window2D)
        {
            GUILayout.BeginVertical();

            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, false, false, GUIStyle.none, GUIStyle.none);
            GUILayout.FlexibleSpace();
            DrawLogs();
            GUILayout.EndScrollView();

            if (this.moveCursor)
            {
                this.CursorToEnd();
                this.moveCursor = false;
            }

            if (Event.current.Equals(Event.KeyboardEvent("`")))
            {
                this.ToggleState(this.state);
            }

            else if (Event.current.Equals(Event.KeyboardEvent("return")) || Event.current.Equals(Event.KeyboardEvent("[enter]")))
            {
                this.EnterCommand();
            }
            else if (Event.current.Equals(Event.KeyboardEvent("up")))
            {
                this.commandText = History.Previous();
                this.moveCursor = true;
            }
            else if (Event.current.Equals(Event.KeyboardEvent("down")))
            {
                this.commandText = History.Next();
            }
            else if (Event.current.Equals(Event.KeyboardEvent("tab")))
            {
                this.CompleteCommand();
                this.moveCursor = true;
            }

            GUILayout.BeginHorizontal();

            if (this.inputCaret != "")
            {
                GUILayout.Label(this.inputCaret, this.inputStyle, GUILayout.Width(this.consoleFont.fontSize));
            }

            GUI.SetNextControlName("command_text_field");
            this.commandText = GUILayout.TextField(this.commandText, this.inputStyle);

            if (this.inputFix && this.commandText.Length > 0)
            {
                this.commandText = this.cachedCommandText;  // Textfield would then pick up the toggle hot key character event
                this.inputFix = false;
            }

            if (this.initialOpen)
            {
                GUI.FocusControl("command_text_field");
                this.initialOpen = false;
            }

            if (this.showGUIButtons && GUILayout.Button("| run", this.inputStyle, GUILayout.Width(Screen.width / 10)))
            {
                this.EnterCommand();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        void DrawLogs()
        {
            foreach (var log in Buffer.Logs)
            {
                this.labelStyle.normal.textColor = GetLogColor(log.type);
                GUILayout.Label(log.message, this.labelStyle);
            }
        }

        void DrawGUIButtons()
        {
            int size = this.consoleFont.fontSize;
            float xPos = this.rightAlignButtons ? Screen.width - 7f * size : 0;

            GUILayout.BeginArea(new Rect(xPos, this.currentOpenTime, 7f * size, size * 2f));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("SHORT", this.windowStyle))
            {
                this.ToggleState(SXLConsoleState.OPENSHORT);
            }
            else if (GUILayout.Button("FULL", this.windowStyle))
            {
                this.ToggleState(SXLConsoleState.OPENFULL);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        void HandleOpenness()
        {
            float deltaT = this.toggleSpeed * Time.unscaledDeltaTime;

            if (this.currentOpenTime < this.openTarget)
            {
                this.currentOpenTime += deltaT;
                if (this.currentOpenTime > this.openTarget)
                {
                    this.currentOpenTime = this.openTarget;
                }
            }
            else if (this.currentOpenTime > this.openTarget)
            {
                this.currentOpenTime -= deltaT;
                if (this.currentOpenTime < this.openTarget)
                {
                    this.currentOpenTime = this.openTarget;
                }
            }
            else
            {
                if (this.inputFix)
                {
                    this.inputFix = false;
                }
                return; // At Target
            }

            window = new Rect(0, this.currentOpenTime - this.realWindowSize, Screen.width, this.realWindowSize);
        }

        void EnterCommand()
        {
            Log(TerminalLogType.INPUT, "{0}", this.commandText);
            Shell.RunCommand(this.commandText);
            History.Push(this.commandText);

            if (IssuedError)
            {
                Log(TerminalLogType.ERROR, "Error: {0}", Shell.IssuedErrorMessage);
            }

            this.commandText = "";
            this.scrollPosition.y = int.MaxValue;
        }

        void CompleteCommand()
        {
            string head = this.commandText;
            int formatWidth = 0;

            string[] completionBuffer = Autocomplete.Complete(ref head, ref formatWidth);
            int completionLength = completionBuffer.Length;

            if (completionLength != 0)
            {
                this.commandText = head;
            }

            if (completionLength > 1)
            {
                var logBuffer = new StringBuilder();

                foreach (string completion in completionBuffer)
                {
                    logBuffer.Append(completion.PadRight(formatWidth + 4));
                }

                Log("{0}", logBuffer);
                this.scrollPosition.y = int.MaxValue;
            }
        }

        void CursorToEnd()
        {
            if (this.editorState == null)
            {
                this.editorState = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            }
            this.editorState.MoveCursorToPosition(new Vector2(999, 999));
        }

        void HandleUnityLog(string message, string stackTrace, LogType type)
        {
            Buffer.HandleLog(message, stackTrace, (TerminalLogType)type);
            this.scrollPosition.y = int.MaxValue;
        }

        Color GetLogColor(TerminalLogType type)
        {
            switch (type)
            {
                case TerminalLogType.MESSAGE: return this.foregroundColor;
                case TerminalLogType.WARNING: return this.warningColor;
                case TerminalLogType.INPUT: return this.inputColor;
                case TerminalLogType.SHELL: return this.shellColor;
                default: return this.errorColor;
            }
        }

    }
}