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
        private static Int64 _channelID;
        private static readonly string botTokenFilePath = Path.Join(directory, "bot_token.txt");
        private static readonly string responseFilePath = Path.Join(directory, "bot_response.txt");
        private static readonly string discordChannelFilePath = Path.Join(directory, "discord_channel_id.txt");

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
            if (File.Exists(discordChannelFilePath))
            {
                string int64String = File.ReadAllText(discordChannelFilePath);
                // Convert the string to Int64 and assign it to _channelID
                if (Int64.TryParse(int64String, out _channelID))
                {
                    Console.WriteLine($"Channel ID loaded successfully: {_channelID}");
                }
                else
                {
                    Console.WriteLine("Failed to parse the Channel ID from the file.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Discord Channel Id file not found.");
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
            while (true)
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
                                    ulong channelId = (ulong)_channelID; // Replace with your actual channel ID
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
                            catch (ObjectDisposedException)
                            {
                                // This exception might be thrown if the pipe is disposed while reading
                                Console.WriteLine("Pipe was disposed.");
                                break;
                            }
                        }
                    }

                    Console.WriteLine("Client disconnected, waiting for a new connection...");
                }

                // Pause briefly before accepting the next connection to avoid a tight loop
                await Task.Delay(100);
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

            // Append the message to the response file with a new line
            // Use AppendAllText to ensure each message is on a new line
            File.AppendAllText(responseFilePath, $"{message.Author.Username}: {message.Content}{Environment.NewLine}");

            // Example response
            if (message.Content.ToLower().Contains("hello bot"))
            {
                await message.Channel.SendMessageAsync("Hello! How can I assist you today?");
            }
        }
    }
}
