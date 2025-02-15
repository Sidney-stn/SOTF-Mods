
using Newtonsoft.Json;

namespace WirelessSignals.Channel
{
    public class ChannelSystem
    {
        private Dictionary<int, ChannelData> channels;
        public static ChannelSystem Instance { get; private set; }

        public ChannelSystem()
        {
            channels = new Dictionary<int, ChannelData>();
            Instance = this;
        }

        public void RegisterTransmitter(string transmitterId, int channelId)
        {
            if (!channels.ContainsKey(channelId))
            {
                channels[channelId] = new ChannelData(channelId);
            }
        }

        public void RegisterReceiver(string receiverId, int channelId)
        {
            if (!channels.ContainsKey(channelId))
            {
                channels[channelId] = new ChannelData(channelId);
            }
            channels[channelId].ConnectedReceiverIds.Add(receiverId);
        }

        public void SetTransmitterState(string transmitterId, int channelId, bool isActive)
        {
            if (!channels.ContainsKey(channelId)) return;

            var channel = channels[channelId];
            if (isActive)
            {
                channel.ActiveTransmitterIds.Add(transmitterId);
            }
            else
            {
                channel.ActiveTransmitterIds.Remove(transmitterId);
            }
        }

        public bool IsChannelActive(int channelId)
        {
            return channels.ContainsKey(channelId) && channels[channelId].IsActive;
        }

        public void UnregisterTransmitter(string transmitterId, int channelId)
        {
            if (!channels.ContainsKey(channelId)) return;
            channels[channelId].ActiveTransmitterIds.Remove(transmitterId);
        }

        public void UnregisterReceiver(string receiverId, int channelId)
        {
            if (!channels.ContainsKey(channelId)) return;
            channels[channelId].ConnectedReceiverIds.Remove(receiverId);
        }

        // Serialization methods
        public string SerializeToJson()
        {
            // Create a simple serializable format using arrays of strings
            var entries = new List<string>();

            foreach (var kvp in channels)
            {
                // Format: "channelId|transmitter1,transmitter2|receiver1,receiver2"
                string entry = string.Format("{0}|{1}|{2}",
                    kvp.Key,
                    string.Join(",", kvp.Value.ActiveTransmitterIds),
                    string.Join(",", kvp.Value.ConnectedReceiverIds)
                );
                entries.Add(entry);
            }

            // Convert to a simple string array that IL2CPP can handle
            string[] serializedData = entries.ToArray();
            return JsonConvert.SerializeObject((Il2CppSystem.Object)(object)serializedData);
        }

        public static ChannelSystem DeserializeFromJson(string json)
        {
            var system = new ChannelSystem();
            string[] entries = JsonConvert.DeserializeObject<string[]>(json);

            foreach (string entry in entries)
            {
                string[] parts = entry.Split('|');
                if (parts.Length != 3) continue;

                int channelId = int.Parse(parts[0]);
                var channelData = new ChannelData(channelId);

                // Parse active transmitters
                if (!string.IsNullOrEmpty(parts[1]))
                {
                    channelData.ActiveTransmitterIds = new HashSet<string>(parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries));
                }

                // Parse connected receivers
                if (!string.IsNullOrEmpty(parts[2]))
                {
                    channelData.ConnectedReceiverIds = new HashSet<string>(parts[2].Split(',', StringSplitOptions.RemoveEmptyEntries));
                }

                system.channels[channelId] = channelData;
            }

            return system;
        }
    }

}
