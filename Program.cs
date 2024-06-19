using System;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Reflection;

namespace DiscordBotApp
{
    class Program
    {
        internal static string dllPath = Assembly.GetExecutingAssembly().Location;
        internal static string directory = Path.GetDirectoryName(dllPath);

        private static DiscordSocketClient _client;
        private static string _botToken;
        private static readonly string botTokenFilePath = Path.Join(directory, "bot_token.txt");
        private static readonly string responseFilePath = Path.Join(directory, "bot_response.txt");

        static async Task Main(string[] args)
        {
            //Console.WriteLine($"BotTokenFilePath: {botTokenFilePath}, ResponseFilePath: {responseFilePath}");
            // Read the bot token from the file
            if (File.Exists(botTokenFilePath))
            {
                _botToken = File.ReadAllText(botTokenFilePath);
            }
            else
            {
                Console.WriteLine("Bot token file not found.");
                return;
            }

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

            // Start a background task to listen for named pipe commands
            Task.Run(() => ListenForCommands());

            // Keep the application running until user exits
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            // Clean up
            await _client.LogoutAsync();
            _client.Dispose();
        }

        private static async Task ListenForCommands()
        {
            using (var pipeServer = new NamedPipeServerStream("DiscordBotPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                Console.WriteLine("Waiting for connection...");
                await pipeServer.WaitForConnectionAsync();

                Console.WriteLine("Client connected.");
                using (var reader = new StreamReader(pipeServer))
                using (var writer = new StreamWriter(pipeServer) { AutoFlush = true })
                {
                    while (pipeServer.IsConnected)
                    {
                        try
                        {
                            string command = await reader.ReadLineAsync();
                            if (!string.IsNullOrEmpty(command))
                            {
                                Console.WriteLine($"Command received: {command}");
                                // Handle the command, for example by sending it as a message
                                ulong channelId = 1116004344370827466; // Replace with your actual channel ID
                                var channel = _client.GetChannel(channelId) as IMessageChannel;
                                if (channel != null)
                                {
                                    await channel.SendMessageAsync(command);
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine($"Named pipe read error: {ex.Message}");
                            break;
                        }
                    }
                }
            }
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
            Console.WriteLine($"Received message from {message.Author.Username}: {message.Content}");

            // Write the message to the response file
            File.WriteAllText(responseFilePath, $"{message.Author.Username}: {message.Content}");

            // Example response
            if (message.Content.ToLower().Contains("hello bot"))
            {
                await message.Channel.SendMessageAsync("Hello! How can I assist you today?");
            }
        }
    }
}
