using Newtonsoft.Json;

namespace WirelessSignals.Channel
{

    [Serializable]
    public class ChannelData
    {
        public int ChannelId { get; set; }
        public HashSet<string> ActiveTransmitterIds { get; set; }
        public HashSet<string> ConnectedReceiverIds { get; set; }
        public bool IsActive => ActiveTransmitterIds.Count > 0;

        public ChannelData()
        {
            ActiveTransmitterIds = new HashSet<string>();
            ConnectedReceiverIds = new HashSet<string>();
        }

        public ChannelData(int channelId) : this()
        {
            ChannelId = channelId;
        }
    }
}
