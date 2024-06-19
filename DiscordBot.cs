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
        internal static string fileDir = Path.Join(directory, "BroadcastMessage");


        private Process botProcess;
        //private readonly string botExecutablePath = @"C:\Users\julia\Documents\SOTF MODDING REDLOADER\BroadcastMessageDiscordBotApp\DiscordBotApp\bin\Debug\net6.0\DiscordBotApp.exe"; // Update this path
        private readonly string botExecutablePath = Path.Join(fileDir, "DiscordBotApp.exe");
        private readonly string botTokenFilePath = Path.Join(fileDir, "bot_token.txt");
        private readonly string commandFilePath = Path.Join(fileDir, "bot_command.txt");
        private readonly string responseFilePath = Path.Join(fileDir, "bot_response.txt");

        public void StartBot()
        {
            try
            {
                if (File.Exists(botExecutablePath))
                {
                    // Write the bot token to a file
                    File.WriteAllText(botTokenFilePath, Config.DiscordBotToken.Value);

                    botProcess = new Process();
                    botProcess.StartInfo.FileName = botExecutablePath;
                    botProcess.StartInfo.CreateNoWindow = true;
                    botProcess.StartInfo.UseShellExecute = false;
                    botProcess.Start();

                    Misc.Msg("Bot process started.");

                    // Start a background thread to listen for messages from the bot
                    new Thread(ListenForResponses).Start();
                }
                else
                {
                    Misc.Msg("Bot executable not found.");
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
                    pipeClient.Connect(1000); // Try to connect with a timeout
                    using (var writer = new StreamWriter(pipeClient) { AutoFlush = true })
                    {
                        writer.WriteLine(command);
                        Misc.Msg($"Command sent to bot: {command}");
                    }
                }
                catch (TimeoutException)
                {
                    Misc.Msg("No bot process connected to send command to.");
                }
            }
        }

        // Background method to listen for responses from the bot
        private void ListenForResponses()
        {
            using (var pipeClient = new NamedPipeClientStream(".", "DiscordBotPipe", PipeDirection.In))
            {
                try
                {
                    pipeClient.Connect(1000); // Try to connect with a timeout
                    using (var reader = new StreamReader(pipeClient))
                    {
                        while (pipeClient.IsConnected)
                        {
                            try
                            {
                                string response = reader.ReadLine();
                                if (!string.IsNullOrEmpty(response))
                                {
                                    Misc.Msg($"Received message from bot: {response}");
                                    // Process the received message here
                                }
                            }
                            catch (IOException ex)
                            {
                                Misc.Msg($"Named pipe read error: {ex.Message}");
                                break;
                            }
                        }
                    }
                }
                catch (TimeoutException)
                {
                    Misc.Msg("No bot process connected to receive messages from.");
                }
            }
        }
    }
}
