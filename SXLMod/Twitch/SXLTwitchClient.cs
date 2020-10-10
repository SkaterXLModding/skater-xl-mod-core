using UnityEngine;

using TwitchLib.Client.Models;
using TwitchLib.Unity;

namespace SXLMod.Twitch
{
    public class SXLTwitchClient : MonoBehaviour
    {
        private Client _client;
        private SXLConfiguration _config;

        private void Start()
        {
            Application.runInBackground = true;
            _config = SXLFile.GetConfigFile();
        }

        public bool ConnectClient()
        {
            if (ValidateTwitchConfiguration(_config))
            {
                Debug.Log("Connecting...");
                string name = _config.Read("bot_name", "twitch");
                string secret = _config.Read("bot_access_token", "twitch");
                ConnectionCredentials credentials = new ConnectionCredentials(name, secret);
                _client = new Client();
                _client.Initialize(credentials, _config.Read("channel_name", "twitch"));

                InvokeClientEvents();

                _client.Connect();
            }
            return true;
        }

        public void DisconnectClient()
        {
            if(_client.IsConnected)
            {
                _client.Disconnect();
            }
        }

        public bool IsConnected()
        {
            return _client.IsConnected;
        }

        private void InvokeClientEvents()
        {
            _client.OnConnected += OnConnected;
            _client.OnConnectionError += OnConnectionError;
            _client.OnIncorrectLogin += OnIncorrectLogin;
            _client.OnMessageReceived += OnMessageReceived;
            _client.OnChatCommandReceived += OnChatCommandReceived;
        }

        private bool ValidateTwitchConfiguration(SXLConfiguration config)
        {
            if(config.KeyExists("channel_name", "twitch") && config.KeyExists("client_id", "twitch") && config.KeyExists("client_secret", "twitch") && config.KeyExists("bot_name", "twitch") && config.KeyExists("bot_access_token", "twitch") && config.KeyExists("bot_refresh_token", "twitch"))
            {
                Debug.Log("Twitch Credentials Validated");
                return true;
            }
            Debug.Log("Twitch Credentials are not valid.");
            return false;
        }

        private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Debug.Log($"The bot {e.BotUsername} successfully connected to Twitch channel {_client.JoinedChannels[0].Channel}");
            PostMessage("Hello!");
        }

        private void OnDisconnect(object sender, TwitchLib.Client.Events.OnDisconnectedArgs e)
        {
            Debug.Log("Twitch bot disconnected.");
        }

        private void OnConnectionError(object sender, TwitchLib.Client.Events.OnConnectionErrorArgs e)
        {
            Debug.Log($"The Bot {e.BotUsername} failed to connect: {e.Error.Message}");
        }

        private void OnIncorrectLogin(object sender, TwitchLib.Client.Events.OnIncorrectLoginArgs e)
        {
            Debug.Log(e.Exception);
        }

        public void PostMessage(string message)
        {
            _client.SendMessage(_client.JoinedChannels[0], message);
        }

        private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            Debug.Log("MESSAGE");
            Debug.Log(e.ChatMessage.Message);
        }

        private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
        {
            Debug.Log(e.Command.CommandText);
        }

    }
}
