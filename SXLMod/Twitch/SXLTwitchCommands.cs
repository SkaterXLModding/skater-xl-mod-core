using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SXLMod.Console;

namespace SXLMod.Twitch
{
    public static class SXLTwitchCommands
    {
        public static readonly Dictionary<string, Action> ChatCommands = new Dictionary<string, Action>()
        {
            {"!bail", () => PlayerController.Instance.DoBail() },
            {"bail", () => PlayerController.Instance.DoBail() }
        };
        /*
        [RegisterCommand(Name = "twitch", Help = "Connect your twitch bot to your live chat", Hint = "twitch <connect|disconnect>", ArgMin = 1, ArgMax = 1)]
        static void CommandTwitch(CommandArg[] args)
        {
            switch(args[0].ToString().ToLower())
            {
                case "connect":
                    SXLModManager.Instance.twitchClient.ConnectClient();
                    break;
                case "disconnect":
                default:
                    SXLModManager.Instance.twitchClient.PostMessage("Disconnecting SXL Integration from Twitch.");
                    SXLModManager.Instance.twitchClient.DisconnectClient();
                    break;
            }
        }
        */
    }
}
