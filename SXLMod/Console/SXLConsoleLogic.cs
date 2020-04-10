using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

namespace SXLMod.Console
{
    public enum TerminalLogType
    {
        ERROR = LogType.Error,
        ASSERT = LogType.Assert,
        WARNING = LogType.Warning,
        MESSAGE = LogType.Log,
        EXCEPTION = LogType.Exception,
        INPUT,
        SHELL
    }

    public struct LogItem
    {
        public TerminalLogType type;
        public string message;
        public string stackTrace;
    }

    public class CommandLog
    {
        List<LogItem> _logs = new List<LogItem>();
        int maxItems;

        public List<LogItem> Logs {
            get { return _logs; }
        }

        public CommandLog(int maxItems)
        {
            this.maxItems = maxItems;
        }

        public void HandleLog(string message, TerminalLogType type)
        {
            HandleLog(message, "", type);
        }

        public void HandleLog(string message, string stackTrace, TerminalLogType type)
        {
            LogItem log = new LogItem() { message = message, stackTrace = stackTrace, type = type };
            this._logs.Add(log);

            if (this._logs.Count > this.maxItems)
            {
                this._logs.RemoveAt(0);
            }

        }

        public void Clear()
        {
            this._logs.Clear();
        }
    }

    public class CommandHistory
    {
        List<string> history = new List<string>();
        int position;

        public void Push(string command)
        {
            if (command == "")
            {
                return;
            }
            this.history.Add(command);
            this.position = this.history.Count;
        }

        public string Next()
        {
            this.position++;

            if (this.position >= this.history.Count)
            {
                this.position = this.history.Count;
                return "";
            }
            return this.history[this.position];
        }

        public string Previous()
        {
            if (this.history.Count == 0)
            {
                return "";
            }

            this.position--;

            if (this.position < 0)
            {
                this.position = 0;
            }

            return this.history[this.position];
        }

        public void Clear()
        {
            this.history.Clear();
            this.position = 0;
        }
    }

    public class CommandAutoComplete
    {
        List<string> knownWords = new List<string>();
        List<string> buffer = new List<string>();

        public void Register(string word)
        {
            this.knownWords.Add(word.ToLower());
        }

        public string[] Complete(ref string text, ref int formatWidth)
        {
            string partial = this.EatLastWord(ref text).ToLower();
            string known;

            for (int i=0; i < this.knownWords.Count; i++)
            {
                known = this.knownWords[i];

                if (known.StartsWith(partial))
                {
                    this.buffer.Add(known);

                    if (known.Length > formatWidth)
                    {
                        formatWidth = known.Length;
                    }
                }
            }

            string[] completions = buffer.ToArray();
            buffer.Clear();

            text += PartialWord(completions);
            return completions;
        }

        string EatLastWord(ref string text)
        {
            int last = text.LastIndexOf(' ');
            string result = text.Substring(last + 1);

            text = text.Substring(0, last + 1);  // Keep space
            return result;
        }

        string PartialWord(string[] words)
        {
            if (words.Length == 0)
            {
                return "";
            }

            string firstMatch = words[0];
            int pLength = firstMatch.Length;

            if (words.Length == 1)
            {
                return firstMatch;
            }

            foreach (string word in words)
            {
                if (pLength > word.Length)
                {
                    pLength = word.Length;
                }

                for (int i = 0; i < pLength; i++)
                {
                    if (word[i] != firstMatch[i])
                    {
                        pLength = i;
                    }
                }
            }
            return firstMatch.Substring(0, pLength);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RegisterCommandAttribute : Attribute
    {
        int _argMin = 0;
        int _argMax = -1;

        public int ArgMin {
            get { return this._argMin; }
            set { this._argMin = value; }
        }

        public int ArgMax {
            get { return this._argMax; }
            set { this._argMax = value; }
        }

        public string Name { get; set; }
        public string Help { get; set; }
        public string Hint { get; set; }

        public RegisterCommandAttribute(string command = null)
        {
            this.Name = command;
        }
    }
}
