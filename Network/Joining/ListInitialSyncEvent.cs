using Bolt;
using RedLoader;
using System.Diagnostics.Tracing;
using UdpKit;

namespace WirelessSignals.Network.Joining
{
    public class ListInitialSyncEvent : EventBase<ListInitialSyncEvent>
    {
        public bool RecivedJoiningEvent = false;  // For Use In OnEnterWorld() In CustomEventHandler
        public BoltConnection Connection;  // For Use In OnEnterWorld() In CustomEventHandler

        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
        {
            if (packet.ReadBool() == false) { return; }
            Misc.Msg($"Received ListInitialSyncEvent REQUEST from {fromConnection}", true);

            // Reply with the UniqueId Sync Event
            Network.SyncLists.UniqueIdSync.Instance.SendUniqueIdEvent(
                forPrefab: Network.SyncLists.UniqueIdSync.UniqueIdListType.All,
                toDo: Network.SyncLists.UniqueIdSync.UniqueIdListOptions.SetAll,
                toPlayer: Network.SyncLists.UniqueIdSync.UniqueIdTo.BoltConnection,
                conn: fromConnection,
                ids: null
            );
        }

        /// Read message on the client
        protected override void ReadMessageClient(UdpPacket packet, BoltConnection _)
        {
        }

        /// For sending from the server
        private void SendServerResponse()
        {
            Misc.Msg($"Sending ListInitialSyncEvent REQUEST", true);
            var packet = NewPacket(4, GlobalTargets.OnlyServer);
            packet.Packet.WriteBool(true);

            Send(packet);
        }

        public void RequestInfoFromServer() => SendServerResponse();

    }
}
