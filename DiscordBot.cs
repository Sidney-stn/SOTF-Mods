using Endnight.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;


namespace BroadcastMessage
{
    public class DiscordBotManager
    {
        // PATHS
        internal static string dllPath = Assembly.GetExecutingAssembly().Location;
        internal static string directory = Path.GetDirectoryName(dllPath);
        internal static string fileDir = Path.Combine(directory, "BroadcastMessage");

        private Process botProcess;
        private readonly string botExecutablePath = Path.Combine(fileDir, "DiscordBotApp.exe");
        private readonly string botTokenFilePath = Path.Combine(fileDir, "bot_token.txt");
        private readonly string responseFilePath = Path.Combine(fileDir, "bot_response.txt");
        private readonly string discordChannelFilePath = Path.Combine(fileDir, "discord_channel_id.txt");

        public void StartBot()
        {
            try
            {
                Misc.Msg($"Expected path for DiscordBotApp.exe: {botExecutablePath}");
                if (Config.DiscordChannelId.Value == (Int64)0 || Config.DiscordChannelId.Value == 0) { Misc.ErrorMsg("DiscordChannelId Is null, Pleace Update It BroadcastMessage.cfg"); return; }
                if (File.Exists(discordChannelFilePath))
                {
                    // Write the discord channel id to a file
                    File.WriteAllText(discordChannelFilePath, Config.DiscordChannelId.Value.ToString());
                }
                else
                {
                    Misc.Msg("DiscordChannelId File not found at path, making new one");
                    File.WriteAllText(discordChannelFilePath, Config.DiscordChannelId.Value.ToString());
                }

                if (File.Exists(botExecutablePath))
                {
                    Misc.Msg("Found DiscordBotApp.exe.");

                    // Write the bot token to a file
                    if (Config.DiscordBotToken.Value == "" || Config.DiscordBotToken.Value.IsNullOrWhitespace() || Config.DiscordBotToken.Value.ToLower() == "token") { Misc.ErrorMsg("DiscordBotToken Is null, Pleace Update It BroadcastMessage.cfg"); return; }
                    File.WriteAllText(botTokenFilePath, Config.DiscordBotToken.Value);

                    botProcess = new Process();
                    botProcess.StartInfo.FileName = botExecutablePath;

                    // Make the process window visible
                    botProcess.StartInfo.CreateNoWindow = false;
                    botProcess.StartInfo.UseShellExecute = true;

                    botProcess.Start();

                    Misc.Msg("Bot process started.");
                }
                else
                {
                    Misc.Msg("Bot executable not found at specified path.");
                }
            }
            catch (Exception ex)
            {
                Misc.Msg($"Error starting bot process: {ex.Message}");
            }
        }

        public void StopBot()
        {
            try
            {
                if (botProcess != null && !botProcess.HasExited)
                {
                    botProcess.Kill();
                    botProcess.WaitForExit();
                    Misc.Msg("Bot process stopped.");
                }
            }
            catch (Exception ex)
            {
                Misc.Msg($"Error stopping bot process: {ex.Message}");
            }
        }

        // Method to send a command using named pipes
        public void SendCommand(string command)
        {
            using (var pipeClient = new NamedPipeClientStream(".", "DiscordBotPipe", PipeDirection.Out))
            {
                try
                {
                    // Attempt to connect to the server
                    Misc.Msg("Attempting to connect to the bot process...");
                    pipeClient.Connect(1000); // Try to connect with a timeout

                    // Check if the connection was successful
                    if (pipeClient.IsConnected)
                    {
                        Misc.Msg("Pipe client connected.");

                        using (var writer = new StreamWriter(pipeClient) { AutoFlush = true })
                        {
                            writer.WriteLine(command);
                            Misc.Msg($"Command sent to bot: {command}");

                            // Explicitly flush and close the writer
                            writer.Flush();
                        }

                        // Explicitly close the pipe after sending the command
                        pipeClient.Close();
                        Misc.Msg("Pipe client closed after sending the command.");
                    }
                    else
                    {
                        Misc.Msg("No bot process connected to send command to.");
                    }
                }
                catch (TimeoutException)
                {
                    Misc.Msg("No bot process connected to send command to (timeout).");
                }
                catch (IOException ex)
                {
                    Misc.Msg($"IO error during command sending: {ex.Message}");
                }
                finally
                {
                    // Ensure the pipe client is properly closed if still connected
                    if (pipeClient.IsConnected)
                    {
                        pipeClient.Close();
                        Misc.Msg("Pipe client closed in finally block.");
                    }
                }
            }
        }

        // Method to check for messages from the bot (read from response file)
        public void CheckForResponses()
        {
            if (File.Exists(responseFilePath))
            {
                // Read all lines from the response file
                var lines = File.ReadLines(responseFilePath);
                foreach (var line in lines)
                {
                    // Skip empty lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Find the first colon to split username and message
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > 0 && colonIndex < line.Length - 1)
                    {
                        // Extract the username and the message
                        string username = line.Substring(0, colonIndex).Trim();
                        string message = line.Substring(colonIndex + 1).Trim();

                        // Log the received message in the desired format
                        Misc.Msg($"Received message from {username}: {message}");
                        string username_prefix = $"[DS] {username}";
                        BroadcastInfo.SendChatMessage(username_prefix, message);

                        // Process the received message here if needed
                        // For example, you could invoke some handler or add the message to a queue
                    }
                    else
                    {
                        // Handle the case where no valid colon is found (invalid format)
                        Misc.Msg($"Invalid message format: {line}");
                    }
                }

                // Optionally, clear the response file after reading
                File.WriteAllText(responseFilePath, string.Empty);
            }
        }
    }
}
