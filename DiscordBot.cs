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

        private Thread botThread;
        private bool shouldStop = false;

        // Start the bot using a background thread
        public void StartBot()
        {
            botThread = new Thread(RunBot);
            botThread.IsBackground = true;
            botThread.Start();
        }

        // The main bot loop running on a separate thread
        private void RunBot()
        {
            try
            {
                InitializeBot().GetAwaiter().GetResult(); // Initialize and connect the bot synchronously
            }
            catch (Exception ex)
            {
                Misc.Msg($"Error starting bot: {ex.Message}");
            }
        }

        // Initialize and connect the bot
        private async Task InitializeBot()
        {
            Misc.Msg("DiscordBotManager InitializeBot()");
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

            // Keep the bot running
            while (!shouldStop)
            {
                Thread.Sleep(100); // Keep the thread alive
            }
        }

        // Stop the bot
        public void StopBot()
        {
            shouldStop = true;

            if (_client != null)
            {
                _client.LogoutAsync().GetAwaiter().GetResult();
                _client.Dispose();
                _client = null;
                Misc.Msg("Bot is disconnected!");
            }

            if (botThread != null && botThread.IsAlive)
            {
                botThread.Join(); // Wait for the bot thread to finish
            }
        }

        // Method to send a message to a specified channel using a separate thread
        public void SendMessageToChannel(ulong channelId, string message)
        {
            new Thread(() =>
            {
                try
                {
                    SendMessageToChannelInternal(channelId, message).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Misc.Msg($"Error sending message: {ex.Message}");
                }
            }).Start();
        }

        // Internal method to send a message
        private async Task SendMessageToChannelInternal(ulong channelId, string message)
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
