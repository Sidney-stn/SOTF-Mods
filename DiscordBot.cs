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

        // Start the bot using Task.Run to handle async operations
        public void StartBot()
        {
            Task.Run(async () =>
            {
                try
                {
                    await StartBotInternalAsync();
                }
                catch (Exception ex)
                {
                    Misc.Msg($"Error starting bot: {ex.Message}");
                }
            });
        }

        // Internal async method to handle bot startup
        private async Task StartBotInternalAsync()
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

        // Stop the bot using Task.Run to handle async operations
        public void StopBot()
        {
            Task.Run(async () =>
            {
                try
                {
                    await StopBotInternalAsync();
                }
                catch (Exception ex)
                {
                    Misc.Msg($"Error stopping bot: {ex.Message}");
                }
            });
        }

        // Internal async method to handle bot shutdown
        private async Task StopBotInternalAsync()
        {
            if (_client != null)
            {
                await _client.LogoutAsync();
                _client.Dispose();
                _client = null;
                Misc.Msg("Bot is disconnected!");
            }
        }

        // Method to send a message to a specified channel
        public void SendMessageToChannel(ulong channelId, string message)
        {
            Task.Run(async () =>
            {
                try
                {
                    await SendMessageToChannelInternalAsync(channelId, message);
                }
                catch (Exception ex)
                {
                    Misc.Msg($"Error sending message: {ex.Message}");
                }
            });
        }

        // Internal async method to send a message
        private async Task SendMessageToChannelInternalAsync(ulong channelId, string message)
        {
            var channel = _client.GetChannel(channelId) as IMessageChannel;

            if (channel != null)
            {
                await channel.SendMessageAsync(message);
                Misc.Msg($"Sent message to channel {channel.Name}: {message}");
            }
            else
            {
                Misc.Msg("Channel not found or invalid.");
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

        // Event delegate for received messages
        public event Action<SocketMessage> OnMessageReceived;
    }
}
