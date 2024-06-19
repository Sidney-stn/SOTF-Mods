using Discord;
using Discord.WebSocket;
using RedLoader;
using Sons.World;
using System;
using System.Threading.Tasks;
using UnityEngine;


namespace BroadcastMessage
{
    internal class DiscordBot
    {


        [RegisterTypeInIl2Cpp]
        public class DiscordBotManager : MonoBehaviour
        {
            private DiscordSocketClient _client;
            private string _botToken = Config.DiscordBotToken.Value; // Replace with your actual bot token

            void Start()
            {
                // Initialize and start the bot
                Misc.Msg("DiscordBotManager Start()");
                Task.Run(() => StartBotAsync());
            }

            private async Task StartBotAsync()
            {
                Misc.Msg("DiscordBotManager StartBotAsync()");
                Misc.Msg($"Config DiscordBotToken Value: {Config.DiscordBotToken.Value}");
                _client = new DiscordSocketClient();

                // Log bot messages to the Unity console
                _client.Log += Log;

                // Handle message received event
                _client.MessageReceived += MessageReceivedAsync;
                if (Config.DiscordBotToken.Value == null || Config.DiscordBotToken.Value == "" || Config.DiscordBotToken.Value == "TOKEN")
                {
                    Misc.Msg("Config DiscordBotToken is invalid, please update it in BroadcastMessage.cfg");
                    _client.Log -= Log;
                    _client.MessageReceived -= MessageReceivedAsync;
                    return;
                }

                await _client.LoginAsync(TokenType.Bot, _botToken);
                await _client.StartAsync();

                Misc.Msg("Bot is connected!");

                // Keep the bot running indefinitely
                await Task.Delay(-1);
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

                // Log the message to the Unity console
                Misc.Msg($"Received message from {message.Author.Username}: {message.Content}");

                // Send a response to the same channel
                if (message.Content.ToLower().Contains("hello bot"))
                {
                    await message.Channel.SendMessageAsync("Hello! How can I assist you today?");
                }
            }

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
            }

            void OnApplicationQuit()
            {
                // Ensure the bot logs out and disposes of resources when the application quits
                _client.LogoutAsync();
                _client.Dispose();
            }
        }


        //    private DiscordSocketClient _client;
        //    private string _botToken = Config.DiscordBotToken.Value; // Replace with your bot token

        //    public async Task StartBotAsync()
        //    {
        //        _client = new DiscordSocketClient();

        //        // Log bot messages to the console
        //        _client.Log += Log;

        //        // Handle message received event
        //        _client.MessageReceived += MessageReceivedAsync;
        //        if (Config.DiscordBotToken.Value == null || Config.DiscordBotToken.Value == "" || Config.DiscordBotToken.Value == "TOKEN")
        //        {
        //            Misc.Msg("Config DiscordBotToken is invalid, please update it in BroadcastMessage.cfg");
        //            _client.Log -= Log;
        //            _client.MessageReceived -= MessageReceivedAsync;
        //            return;
        //        }
        //        await _client.LoginAsync(TokenType.Bot, _botToken);
        //        await _client.StartAsync();

        //        Misc.Msg("Bot is connected!");

        //        // Block this task until the program is closed.
        //        await Task.Delay(-1);
        //    }

        //    private Task Log(LogMessage log)
        //    {
        //        Misc.Msg(log.ToString());
        //        return Task.CompletedTask;
        //    }

        //    private async Task MessageReceivedAsync(SocketMessage message)
        //    {
        //        // Ignore messages from the bot itself
        //        if (message.Author.Id == _client.CurrentUser.Id)
        //            return;

        //        // Log the message
        //        Misc.Msg($"Received message from {message.Author.Username}: {message.Content}");

        //        // Send a response to the same channel
        //        if (message.Content.ToLower().Contains("hello bot"))
        //        {
        //            await message.Channel.SendMessageAsync("Hello! How can I assist you today?");
        //        }
        //    }

        //    public async Task SendMessageToChannel(ulong channelId, string message)
        //    {
        //        // Get the channel by ID
        //        var channel = _client.GetChannel(channelId) as IMessageChannel;

        //        if (channel != null)
        //        {
        //            // Send a message to the channel
        //            await channel.SendMessageAsync(message);
        //            Misc.Msg($"Sent message to channel {channel.Name}: {message}");
        //        }
        //    }
    }
}
