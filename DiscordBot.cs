using System;
using System.Diagnostics;
using System.IO;
using System.Threading;


namespace BroadcastMessage
{
    public class DiscordBotManager
    {
        private Process botProcess;
        private readonly string botExecutablePath = @"C:\Users\julia\Documents\SOTF MODDING REDLOADER\BroadcastMessageDiscordBotApp\DiscordBotApp\bin\Debug\net6.0\DiscordBotApp.exe"; // Update this path

        public void StartBot()
        {
            try
            {
                if (File.Exists(botExecutablePath))
                {
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

        // Example method to send a command (for demo purposes, uses file-based IPC)
        public void SendCommand(string command)
        {
            File.WriteAllText("bot_command.txt", command);
            Misc.Msg($"Command sent to bot: {command}");
        }
    }
}
