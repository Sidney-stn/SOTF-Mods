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
        private readonly string botExecutablePath = Path.Join(fileDir, "DiscordBotApp.exe");
        private readonly string botTokenFilePath = Path.Join(fileDir, "bot_token.txt");
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
                    // Invisible Window
                    //botProcess.StartInfo.CreateNoWindow = true;
                    //botProcess.StartInfo.UseShellExecute = false;
                    // Make the process window visible
                    botProcess.StartInfo.CreateNoWindow = false;
                    botProcess.StartInfo.UseShellExecute = true;
                    botProcess.Start();

                    Misc.Msg("Bot process started.");
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

        // Method to check for messages from the bot (read from response file)
        public void CheckForResponses()
        {
            if (File.Exists(responseFilePath))
            {
                string response = File.ReadAllText(responseFilePath);
                if (!string.IsNullOrEmpty(response))
                {
                    Misc.Msg($"Received message from bot: {response}");
                    // Process the received message here

                    // Optionally, clear the response file after reading
                    File.WriteAllText(responseFilePath, string.Empty);
                }
            }
        }
    }
}
