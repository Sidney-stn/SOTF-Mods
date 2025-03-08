
using Bolt;
using RedLoader;
using SonsSdk;
using TheForest.Items.Craft;
using UdpKit;

namespace Signs.Network.Join
{
    internal class SyncSignsOnJoin : EventBase<SyncSignsOnJoin>
    {

        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
        {
            Misc.Msg($"Received SyncSignsOnJoin REQUEST from {fromConnection}", true);

            string requestType = packet.ReadString();
            if (requestType != "REQUEST_SYNC")
            {
                RLog.Error($"[SyncSignsOnJoin] [ReadMessageServer] Invalid request type: {requestType}");
                return;
            }
            Misc.Msg($"[SyncSignsOnJoin] [ReadMessageServer] Received: {requestType}", true);

            try
            {
                // Check if Network classes are properly initialized
                if (Network.Join.SyncSignsOnJoin.Instance == null)
                {
                    Misc.Msg("[SyncSignsOnJoin] [ReadMessageServer] UniqueIdSync.Instance is null", true);
                    SendEmptyResponse(fromConnection);
                    return;
                }

                string toPlayerSteamId = packet.ReadString();
                if (string.IsNullOrEmpty(toPlayerSteamId))
                {
                    Misc.Msg("[SyncSignsOnJoin] [ReadMessageServer] toPlayerSteamId is null or empty", true);
                    SendEmptyResponse(fromConnection);
                    return;
                }

                // Check if the collections are valid before sending
                bool collectionsValid = true;

                if (Saving.Track.spawnedSigns == null)
                {
                    Misc.Msg("[SyncSignsOnJoin] [ReadMessageServer] Saving.Track.spawnedSigns Is Null", true);
                    collectionsValid = false;
                }
                else
                {
                    Misc.Msg($"[SyncSignsOnJoin] [ReadMessageServer] Signs items count: {Saving.Track.spawnedSigns.Count}", true);
                }

                if (!collectionsValid)
                {
                    Misc.Msg("[SyncSignsOnJoin] [ReadMessageServer] One or more collections is invalid, not sending sync data", true);
                    // Send a small valid response instead
                    SendEmptyResponse(fromConnection);
                    return;
                }


                // If collections are valid, send the full sync event
                foreach (var item in Saving.Track.spawnedSigns)
                {
                    var boltEntity = item.Key;
                    if (boltEntity == null)
                    {
                        Misc.Msg($"[SyncSignsOnJoin] [ReadMessageServer] BoltEntity is null for {item.Value.name}", true);
                        continue;
                    }
                    var signSetter = boltEntity.GetComponent<Network.SignSetter>();
                    if (signSetter == null)
                    {
                        Misc.Msg($"[SyncSignsOnJoin] [ReadMessageServer] SignSetter is null for {item.Value.name}", true);
                        continue;
                    }
                    Network.SignSyncEvent.SendState(boltEntity, SignSyncEvent.SignSyncType.SetTextAll, toPlayerSteamId);
                }
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[SyncSignsOnJoin] [ReadMessageServer] Error handling request: {ex.Message}", true);
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
                Misc.Msg("[SyncSignsOnJoin] [SendEmptyResponse] Sent empty response instead", true);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[SyncSignsOnJoin] [SendEmptyResponse] Error sending empty response: {ex.Message}", true);
            }
        }

        // Read message on the client
        protected override void ReadMessageClient(UdpPacket packet, BoltConnection _)
        {
            // This method would handle responses if needed
            if (packet.ReadBool() == false)
            {
                if (packet.ReadString() == "EMPTY_SYNC_RESPONSE")
                {
                    Misc.Msg($"[SyncSignsOnJoin] [ReadMessageClient] Received SyncSignsOnJoin RESPONSE: Empty response received", true);
                    return;
                }
                else
                {
                    Misc.Msg($"[SyncSignsOnJoin] [ReadMessageClient] Received SyncSignsOnJoin RESPONSE: Something went wrong!", true);
                    SonsTools.ShowMessage("Something went wrong while syncing the initial data with server, please rejoin", 5f);
                    return;
                }
            }
        }

        private void SendServerResponse()
        {
            Misc.Msg($"Sending SyncSignsOnJoin REQUEST", true);
            try
            {
                // Check if BoltNetwork is running before sending
                if (!BoltNetwork.isRunning)
                {
                    Misc.Msg("[SyncSignsOnJoin] [SendServerResponse] BoltNetwork is not running", true);
                    return;
                }

                // Check if we have a valid server to send to
                if (BoltNetwork.server == null)
                {
                    Misc.Msg("[SyncSignsOnJoin] [SendServerResponse] BoltNetwork.server is null", true);
                    return;
                }
                if (BoltNetwork.isServer)
                {
                    Misc.Msg("[SyncSignsOnJoin] [SendServerResponse] Not a client, skipping request", true);
                    return;
                }

                var packet = NewPacket(64, GlobalTargets.OnlyServer);
                packet.Packet.WriteString("REQUEST_SYNC");
                packet.Packet.WriteString(Misc.MySteamId().Item2);
                Send(packet);

                Misc.Msg("[SyncSignsOnJoin] [SendServerResponse] Successfully sent request to server", true);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[SyncSignsOnJoin] [SendServerResponse] Error sending request: {ex.Message}", true);
            }
        }

        public void RequestInfoFromServer() => SendServerResponse();

        public override string Id => "Signs_SyncSignsOnJoin";
    }
}
