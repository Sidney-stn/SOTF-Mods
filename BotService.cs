using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadcastMessage
{
    public class BotService
    {
        private DiscordBotManager _botManager;

        public BotService()
        {
            // Initialize the DiscordBotManager without needing to pass a token
            _botManager = new DiscordBotManager();

            // Subscribe to the OnMessageReceived event
            _botManager.OnMessageReceived += HandleMessageReceived;
        }

        // Method to start the bot without awaiting directly
        public void StartBot()
        {
            Task.Run(() => _botManager.StartBotAsync());
        }

        // Method to stop the bot without awaiting directly
        public void StopBot()
        {
            Task.Run(() => _botManager.StopBotAsync());
        }

        // Method to send a message to a specific channel without awaiting directly
        public void SendMessage(ulong channelId, string message)
        {
            Task.Run(() => _botManager.SendMessageToChannel(channelId, message));
        }

        // Event handler for received messages
        private void HandleMessageReceived(SocketMessage message)
        {
            Misc.Msg($"[Event Handler] Message received from {message.Author.Username}: {message.Content}");

            // Add more logic here to handle the message, e.g., parsing commands
        }
    }
}
