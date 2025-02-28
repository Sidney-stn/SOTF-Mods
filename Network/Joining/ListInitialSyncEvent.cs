using Bolt;
using RedLoader;
using SonsSdk;
using UdpKit;

namespace WirelessSignals.Network.Joining
{
    public class ListInitialSyncEvent : EventBase<ListInitialSyncEvent>
    {
        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
        {
            if (packet.ReadBool() == false)
            {
                return;
            }

            Misc.Msg($"Received ListInitialSyncEvent REQUEST from {fromConnection}", true);

            try
            {
                // Check if the collections are valid before sending
                bool collectionsValid = true;

                if (WirelessSignals.reciver == null || WirelessSignals.reciver.spawnedGameObjects == null)
                {
                    Misc.Msg("[ListInitialSyncEvent] [ReadMessageServer] WirelessSignals.reciver or its spawnedGameObjects is null", true);
                    collectionsValid = false;
                }

                if (WirelessSignals.transmitterSwitch == null || WirelessSignals.transmitterSwitch.spawnedGameObjects == null)
                {
                    Misc.Msg("[ListInitialSyncEvent] [ReadMessageServer] WirelessSignals.transmitterSwitch or its spawnedGameObjects is null", true);
                    collectionsValid = false;
                }

                if (WirelessSignals.transmitterDetector == null || WirelessSignals.transmitterDetector.spawnedGameObjects == null)
                {
                    Misc.Msg("[ListInitialSyncEvent] [ReadMessageServer] WirelessSignals.transmitterDetector or its spawnedGameObjects is null", true);
                    collectionsValid = false;
                }

                if (!collectionsValid)
                {
                    Misc.Msg("[ListInitialSyncEvent] [ReadMessageServer] One or more collections is invalid, not sending sync data", true);
                    // Send a small valid response instead
                    SendEmptyResponse(fromConnection);
                    return;
                }

                // If collections are valid, send the full sync event
                Network.SyncLists.UniqueIdSync.Instance.SendUniqueIdEvent(
                    forPrefab: Network.SyncLists.UniqueIdSync.UniqueIdListType.All,
                    toDo: Network.SyncLists.UniqueIdSync.UniqueIdListOptions.SetAll,
                    toPlayer: Network.SyncLists.UniqueIdSync.UniqueIdTo.BoltConnection,
                    conn: fromConnection,
                    ids: null
                );
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[ListInitialSyncEvent] [ReadMessageServer] Error handling request: {ex.Message}", true);
                // Send a small valid response instead
                SendEmptyResponse(fromConnection);
            }
        }

        private void SendEmptyResponse(BoltConnection connection)
        {
            try
            {
                // Send a minimal valid response
                var packet = NewPacket(4, connection);
                packet.Packet.WriteBool(false);
                Send(packet);
                Misc.Msg("[ListInitialSyncEvent] [SendEmptyResponse] Sent empty response instead", true);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[ListInitialSyncEvent] [SendEmptyResponse] Error sending empty response: {ex.Message}", true);
            }
        }

        /// Read message on the client
        protected override void ReadMessageClient(UdpPacket packet, BoltConnection _)
        {
            // This method would handle responses if needed
            if (packet.ReadBool() == false)
            {
                Misc.Msg($"[ListInitialSyncEvent] [ReadMessageClient] Received ListInitialSyncEvent RESPONSE: Something went wrong!", true);
                SonsTools.ShowMessage("Something went wrong while syncing the initial data with server, please rejoin", 5f);
                return;
            }
        }

        /// For sending from the server
        private void SendServerResponse()
        {
            Misc.Msg($"Sending ListInitialSyncEvent REQUEST", true);
            try
            {
                var packet = NewPacket(4, GlobalTargets.OnlyServer);
                packet.Packet.WriteBool(true);
                Send(packet);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[ListInitialSyncEvent] [SendServerResponse] Error sending request: {ex.Message}", true);
            }
        }

        public void RequestInfoFromServer() => SendServerResponse();
    }
}