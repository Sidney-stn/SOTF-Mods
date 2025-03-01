using Bolt;
using RedLoader;
using UdpKit;
using WirelessSignals.Network.Reciver;

namespace WirelessSignals.Network.Sync
{
    public class NetworkOwnerSyncEvent : RelayEventBase<NetworkOwnerSyncEvent, ReciverSetter>
    {
        public enum SyncType : byte
        {
            PlaceOnBoltEntity = 0,
            RemoveFromBoltEntity = 1
        }

        /// Read message on the server
        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection) { }

        /// For sending from the client
        public void SendClientResponse() { }

        private void UpdateStateInternal(BoltEntity entity, SyncType type)
        {
            Misc.Msg($"Sending {type} to {entity}", true);
            var packet = NewPacket(entity, 128, GlobalTargets.AllClients);
            packet.Packet.WriteByte((byte)type);
            switch (type)
            {
                case SyncType.PlaceOnBoltEntity:
                    packet.Packet.WriteString("PLACE_NETWORK_OWNER_SCRIPT");
                    break;
                case SyncType.RemoveFromBoltEntity:
                    packet.Packet.WriteString("REMOVE_NETWORK_OWNER_SCRIPT");
                    break;
            }
            
            Send(packet);
        }

        public static void SendState(BoltEntity entity, SyncType type)
        {
            Instance.UpdateStateInternal(entity, type);
        }

        public override string Id => "WirelessSignals_NetworkOwnerSyncEvent";
    }
}
