using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBotApp
{
    class Program
    {
        private static DiscordSocketClient _client;
        private static string _botToken = "MTI1MzAyOTI4ODEyNzYzMTQzMg.G4HH1G.4N35NMqweqaW_E6K_RVVagFGiKNr_KjQAyHCj8"; // Replace with your actual bot token

        static async Task Main(string[] args)
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            _client = new DiscordSocketClient(config);

            // Log bot messages to the console
            _client.Log += Log;

            // Handle message received event
            _client.MessageReceived += MessageReceivedAsync;

            if (string.IsNullOrEmpty(_botToken) || _botToken == "TOKEN")
            {
                Console.WriteLine("Bot token is invalid. Please update it in the code.");
                return;
            }

            await _client.LoginAsync(TokenType.Bot, _botToken);
            await _client.StartAsync();

            Console.WriteLine("Bot is connected!");

            // Keep the application running until user exits
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            // Clean up
            await _client.LogoutAsync();
            _client.Dispose();
        }

        private static Task Log(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private static async Task MessageReceivedAsync(SocketMessage message)
        {
            // Ignore messages from the bot itself
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            // Log the message to the console
            Console.WriteLine($"Received message from {message.Author.Username}, Contet: {message.Content}, CleanContent: {message.CleanContent}");

            // Example response
            if (message.Content.ToLower().Contains("hello bot"))
            {
                await message.Channel.SendMessageAsync("Hello! How can I assist you today?");
            }
        }
    }
}
