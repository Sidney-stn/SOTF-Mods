using System;
using System.Diagnostics;
using System.IO;
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

        // Example method to send a command (write command to file)
        public void SendCommand(string command)
        {
            File.WriteAllText(commandFilePath, command);
            Misc.Msg($"Command sent to bot: {command}");
        }

        // Example method to receive messages (read from response file)
        public void ReceiveMessages()
        {
            if (File.Exists(responseFilePath))
            {
                string response = File.ReadAllText(responseFilePath);
                if (!string.IsNullOrEmpty(response))
                {
                    Misc.Msg($"Received message from bot: {response}");
                    // Process the received message here
                }
            }
        }
    }
}
