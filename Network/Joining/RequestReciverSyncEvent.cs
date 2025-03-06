using Bolt;
using RedLoader;
using SonsSdk;
using UdpKit;

namespace WirelessSignals.Network.Joining
{
    internal class RequestReciverSyncEvent : EventBase<RequestReciverSyncEvent>
    {

        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
        {
            Misc.Msg($"Received RequestReciverSyncEvent REQUEST from {fromConnection}", true);

            string requestType = packet.ReadString();
            if (requestType != "REQUEST_SYNC")
            {
                RLog.Error($"[RequestReciverSyncEvent] [ReadMessageServer] Invalid request type: {requestType}");
                return;
            }

            try
            {
                // Check if Network classes are properly initialized
                if (Network.Joining.RequestReciverSyncEvent.Instance == null)
                {
                    Misc.Msg("[RequestReciverSyncEvent] [ReadMessageServer] UniqueIdSync.Instance is null", true);
                    SendEmptyResponse(fromConnection);
                    return;
                }

                string toPlayerSteamId = packet.ReadString();
                if (string.IsNullOrEmpty(toPlayerSteamId))
                {
                    Misc.Msg("[RequestReciverSyncEvent] [ReadMessageServer] toPlayerSteamId is null or empty", true);
                    SendEmptyResponse(fromConnection);
                    return;
                }

                // Check if the collections are valid before sending
                bool collectionsValid = true;

                if (WirelessSignals.reciver == null || WirelessSignals.reciver.spawnedGameObjects == null)
                {
                    Misc.Msg("[RequestReciverSyncEvent] [ReadMessageServer] WirelessSignals.reciver or its spawnedGameObjects is null", true);
                    collectionsValid = false;
                }
                else
                {
                    Misc.Msg($"[RequestReciverSyncEvent] [ReadMessageServer] Reciver items count: {WirelessSignals.reciver.spawnedGameObjects.Count}", true);
                }

                if (!collectionsValid)
                {
                    Misc.Msg("[RequestReciverSyncEvent] [ReadMessageServer] One or more collections is invalid, not sending sync data", true);
                    // Send a small valid response instead
                    SendEmptyResponse(fromConnection);
                    return;
                }

                // Check if we have any data to send - if all collections are empty, just send an empty response
                if (WirelessSignals.reciver.spawnedGameObjects.Count == 0)
                {
                    Misc.Msg("[RequestReciverSyncEvent] [ReadMessageServer] All collections are empty, sending empty response", true);
                    SendEmptyResponse(fromConnection);
                    return;
                }

                // If collections are valid, send the full sync event
                foreach (var item in WirelessSignals.reciver.spawnedGameObjects)
                {
                    var boltEntity = item.Value.GetComponent<BoltEntity>();
                    if (boltEntity == null)
                    {
                        Misc.Msg($"[RequestReciverSyncEvent] [ReadMessageServer] BoltEntity is null for {item.Key}", true);
                        continue;
                    }
                    var reciverSetter = boltEntity.GetComponent<Network.Reciver.ReciverSetter>();
                    if (reciverSetter == null)
                    {
                        Misc.Msg($"[RequestReciverSyncEvent] [ReadMessageServer] ReciverSetter is null for {item.Key}", true);
                        continue;
                    }
                    Network.Reciver.ReciverSyncEvent.SendState(boltEntity, toPlayerSteamId, Reciver.ReciverSyncEvent.ReciverSyncType.LateJoinSync);
                }
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[RequestReciverSyncEvent] [ReadMessageServer] Error handling request: {ex.Message}", true);
                // Send a small valid response instead
                SendEmptyResponse(fromConnection);
            }
        }

        // Send From Server
        private void SendEmptyResponse(BoltConnection connection)
        {
            try
            {
                // Send a minimal valid response with clear information
                var packet = NewPacket(32, connection);
                packet.Packet.WriteBool(false);  // Flag indicating this is an empty response
                packet.Packet.WriteString("EMPTY_SYNC_RESPONSE");  // Clear identifier

                Send(packet);
                Misc.Msg("[RequestReciverSyncEvent] [SendEmptyResponse] Sent empty response instead", true);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[RequestReciverSyncEvent] [SendEmptyResponse] Error sending empty response: {ex.Message}", true);
            }
        }

        /// Read message on the client
        protected override void ReadMessageClient(UdpPacket packet, BoltConnection _)
        {
            // This method would handle responses if needed
            if (packet.ReadBool() == false)
            {
                if (packet.ReadString() == "EMPTY_SYNC_RESPONSE")
                {
                    Misc.Msg($"[RequestReciverSyncEvent] [ReadMessageClient] Received RequestReciverSyncEvent RESPONSE: Empty response received", true);
                    return;
                }
                else
                {
                    Misc.Msg($"[RequestReciverSyncEvent] [ReadMessageClient] Received RequestReciverSyncEvent RESPONSE: Something went wrong!", true);
                    SonsTools.ShowMessage("Something went wrong while syncing the initial data with server, please rejoin", 5f);
                    return;
                }
            }
        }

        private void SendServerResponse()
        {
            Misc.Msg($"Sending RequestReciverSyncEvent REQUEST", true);
            try
            {
                // Check if BoltNetwork is running before sending
                if (!BoltNetwork.isRunning)
                {
                    Misc.Msg("[RequestReciverSyncEvent] [SendServerResponse] BoltNetwork is not running", true);
                    return;
                }

                // Check if we have a valid server to send to
                if (BoltNetwork.server == null)
                {
                    Misc.Msg("[RequestReciverSyncEvent] [SendServerResponse] BoltNetwork.server is null", true);
                    return;
                }

                var packet = NewPacket(64, GlobalTargets.OnlyServer);
                packet.Packet.WriteString("REQUEST_SYNC");
                packet.Packet.WriteString(Misc.GetMySteamId());
                Send(packet);

                Misc.Msg("[RequestReciverSyncEvent] [SendServerResponse] Successfully sent request to server", true);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[RequestReciverSyncEvent] [SendServerResponse] Error sending request: {ex.Message}", true);
            }
        }

        public void RequestInfoFromServer() => SendServerResponse();
    }
}
