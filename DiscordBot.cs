using Discord;
using Discord.WebSocket;
using RedLoader;
using Sons.World;
using System;
using System.Threading.Tasks;
using UnityEngine;


namespace BroadcastMessage
{
    public class DiscordBotManager
    {
        private DiscordSocketClient _client;
        private string _botToken = Config.DiscordBotToken.Value; // Directly using the configuration value

        // Method to initialize and start the bot
        public async Task StartBotAsync()
        {
            Misc.Msg("DiscordBotManager StartBotAsync()");
            Misc.Msg($"Config DiscordBotToken Value: {_botToken}");

            _client = new DiscordSocketClient();

            // Log bot messages to the console
            _client.Log += Log;

            // Handle message received event
            _client.MessageReceived += MessageReceivedAsync;

            if (string.IsNullOrEmpty(_botToken) || _botToken == "TOKEN")
            {
                Misc.Msg("Config DiscordBotToken is invalid, please update it in your configuration.");
                _client.Log -= Log;
                _client.MessageReceived -= MessageReceivedAsync;
                return;
            }

            await _client.LoginAsync(TokenType.Bot, _botToken);
            await _client.StartAsync();

            Misc.Msg("Bot is connected!");
        }

        // Method to stop the bot and dispose of resources
        public async Task StopBotAsync()
        {
            if (_client != null)
            {
                await _client.LogoutAsync();
                _client.Dispose();
                Misc.Msg("Bot is disconnected!");
            }
        }

        private Task Log(LogMessage log)
        {
            Misc.Msg(log.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // Ignore messages from the bot itself
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            // Log the message to the console
            Misc.Msg($"Received message from {message.Author.Username}: {message.Content}");

            // Trigger the OnMessageReceived event
            OnMessageReceived?.Invoke(message);

            // Send a response to the same channel
            if (message.Content.ToLower().Contains("hello bot"))
            {
                await message.Channel.SendMessageAsync("Hello! How can I assist you today?");
            }
        }

        // Method to send a message to a specified channel
        public async Task SendMessageToChannel(ulong channelId, string message)
        {
            // Get the channel by ID
            var channel = _client.GetChannel(channelId) as IMessageChannel;

            if (channel != null)
            {
                // Send a message to the channel
                await channel.SendMessageAsync(message);
                Misc.Msg($"Sent message to channel {channel.Name}: {message}");
            }
            else
            {
                Misc.Msg("Channel not found or invalid.");
            }
        }

        // Event delegate for received messages
        public event Action<SocketMessage> OnMessageReceived;
    }
}
