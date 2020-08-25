using System;
using System.Collections.Generic;
using System.Linq;

using TwitchLib.Client.Events;

using UnityEngine;

namespace SXLMod.Twitch
{
    public static class SXLTwitchChat
    {
        public static void HandleChatCommand(object sender, OnChatCommandReceivedArgs e)
        {
            string command = e.Command.CommandText.ToLower();
            Debug.Log(command);
            if (SXLTwitchCommands.ChatCommands.ContainsKey(command))
            {
                SXLTwitchCommands.ChatCommands[command]();
            }
        }

        public static void HandleChatMessage(object sender, OnMessageReceivedArgs e)
        {
            Debug.Log(e.ChatMessage.Message);
        }
    }
}
