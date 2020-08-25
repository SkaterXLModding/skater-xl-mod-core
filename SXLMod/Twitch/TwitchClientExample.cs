using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXLMod.Twitch
{
    using TwitchLib.Client.Models;
    using TwitchLib.Unity;
    using UnityEngine;

    public class TwitchClientExample : MonoBehaviour
    {
        [SerializeField] //[SerializeField] Allows the private field to show up in Unity's inspector. Way better than just making it public
        private string _channelToConnectTo = "amatobahn";
        private string _username = "amatobot";
        private string _token = "vgovqdpio50blm2jfmo1m4te0zf2xg";

        private Client _client;

        private void Start()
        {
            // To keep the Unity application active in the background, you can enable "Run In Background" in the player settings:
            // Unity Editor --> Edit --> Project Settings --> Player --> Resolution and Presentation --> Resolution --> Run In Background
            // This option seems to be enabled by default in more recent versions of Unity. An aditional, less recommended option is to set it in code:
            // Application.runInBackground = true;

            //Create Credentials instance
            ConnectionCredentials credentials = new ConnectionCredentials(_username, _token);

            // Create new instance of Chat Client
            _client = new Client();

            // Initialize the client with the credentials instance, and setting a default channel to connect to.
            _client.Initialize(credentials, _channelToConnectTo);

            // Bind callbacks to events
            _client.OnConnected += OnConnected;
            _client.OnJoinedChannel += OnJoinedChannel;
            _client.OnMessageReceived += OnMessageReceived;
            _client.OnChatCommandReceived += OnChatCommandReceived;

            // Connect
            _client.Connect();
        }

        private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Debug.Log($"The bot {e.BotUsername} succesfully connected to Twitch.");

            if (!string.IsNullOrWhiteSpace(e.AutoJoinChannel))
                Debug.Log($"The bot will now attempt to automatically join the channel provided when the Initialize method was called: {e.AutoJoinChannel}");
        }

        private void OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
        {
            Debug.Log($"The bot {e.BotUsername} just joined the channel: {e.Channel}");
            _client.SendMessage(e.Channel, "I just joined the channel! PogChamp");
        }

        private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            Debug.Log($"Message received from {e.ChatMessage.Username}: {e.ChatMessage.Message}");
        }

        private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
        {
            switch (e.Command.CommandText)
            {
                case "hello":
                    _client.SendMessage(e.Command.ChatMessage.Channel, $"Hello {e.Command.ChatMessage.DisplayName}!");
                    break;
                case "about":
                    _client.SendMessage(e.Command.ChatMessage.Channel, "I am a Twitch bot running on TwitchLib!");
                    break;
                default:
                    _client.SendMessage(e.Command.ChatMessage.Channel, $"Unknown chat command: {e.Command.CommandIdentifier}{e.Command.CommandText}");
                    break;
            }
        }

        private void Update()
        {
            // Don't call the client send message on every Update,
            // this is sample on how to call the client,
            // not an example on how to code.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _client.SendMessage(_channelToConnectTo, "I pressed the space key within Unity.");
            }
        }
    }
}
