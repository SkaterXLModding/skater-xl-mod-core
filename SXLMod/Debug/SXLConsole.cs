using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace SXLMod.Debug
{
    public delegate void Callback();
       
    public class SXLDeveloperConsole : MonoBehaviour
    {
        public static SXLDeveloperConsole Instance { get; private set; }
        public static Dictionary<string, SXLConsoleCommand> commands { get; private set; }

        // TODO: Create SXLDeveloperConsoleInterface class to handle generating the visual content
        public Canvas consoleCanvas;
        public Text consoleText;
        public Text inputText;
        public InputField consoleInput;

        private void Awake()
        {
            if (Instance != null)
            {
                return;
            }
            Instance = this;
            commands = new Dictionary<string, SXLConsoleCommand>();
        }

        private void Start()
        {
            this.consoleCanvas.gameObject.SetActive(false);
            this.CreateBaseCommands();
        }

        private void Update()
        {
            if (this.consoleCanvas.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (this.inputText.text != "")
                    {
                        AddMessageToConsole(this.inputText.text);
                        ParseInput(inputText.text);
                    }
                }
            }
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logMessage, string stackTrace, LogType type)
        {
            AddMessageToConsole($"[{type.ToString()}] {logMessage}\n{stackTrace.ToString()}");
        }

        private void CreateBaseCommands()
        {
            new SXLConsoleCommand("Quit", "quit", "Quit SkaterXL", "<command>", new Callback(() => Application.Quit()));
            new SXLConsoleCommand("Test", "test", "Test Console Command", "<command>", new Callback(() => UnityEngine.Debug.Log("This is a test Console Command")));
        }

        public static void AddCommandsToConsole(string name, SXLConsoleCommand command)
        {
            if (!commands.ContainsKey(name))
            {
                commands.Add(name, command);
            }
        }

        public void AddMessageToConsole(string message)
        {
            this.consoleText.text += $"{message}\n";
        }

        private void ParseInput(string input)
        {
            string[] _input = input.Split(null);

            if (_input.Length == 0 || _input == null || !commands.ContainsKey(_input[0]))
            {
                UnityEngine.Debug.Log($"{input} is not recognized...");
                return;
            }
            else
            {
                commands[_input[0]].RunCommand();
            }
        }

        public void ToggleConsoleVisibility()
        {
            this.consoleCanvas.gameObject.SetActive(!consoleCanvas.gameObject.activeInHierarchy);
        }
    }

    public class SXLConsoleCommand
    {
        public string Name { get; protected set; }
        public string Command { get; protected set; }
        public string Description { get; protected set; }
        public string Help { get; protected set; }
        private dynamic callback;

        public SXLConsoleCommand(string name, string command, string description, string help, Callback callback)
        {
            this.Name = name;
            this.Command = command.ToLower();
            this.Description = description;
            this.Help = help;
            this.callback = callback;

            this.AddCommandToConsole();
        }

        private void AddCommandToConsole()
        {
            SXLDeveloperConsole.AddCommandsToConsole(Command, this);
        }

        public void RunCommand()
        {
            this.callback();
        }
    }
}
