

using Bolt;
using RedLoader;
using SonsSdk;
using UdpKit;

namespace StoneGate.Network.Joining
{
    internal class StoneGateJoin : EventBase<StoneGateJoin>
    {
        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
        {
            Misc.Msg($"Received StoneGateJoin REQUEST from {fromConnection}", true);

            string requestType = packet.ReadString();
            if (requestType != "REQUEST_SYNC")
            {
                RLog.Error($"[StoneGateJoin] [ReadMessageServer] Invalid request type: {requestType}");
                return;
            }
            Misc.Msg($"[StoneGateJoin] [ReadMessageServer] Received: {requestType}", true);

            try
            {
                // Check if Network classes are properly initialized
                if (Network.Joining.StoneGateJoin.Instance == null)
                {
                    Misc.Msg("[StoneGateJoin] [ReadMessageServer] UniqueIdSync.Instance is null", true);
                    SendEmptyResponse(fromConnection);
                    return;
                }

                string toPlayerSteamId = packet.ReadString();
                if (string.IsNullOrEmpty(toPlayerSteamId))
                {
                    Misc.Msg("[StoneGateJoin] [ReadMessageServer] toPlayerSteamId is null or empty", true);
                    SendEmptyResponse(fromConnection);
                    return;
                }

                // Check if the collections are valid before sending
                bool collectionsValid = true;

                if (Objects.Track.spawendStoneGates == null)
                {
                    Misc.Msg("[StoneGateJoin] [ReadMessageServer] Objects.Track.spawendStoneGates Is Null", true);
                    collectionsValid = false;
                }
                else
                {
                    Misc.Msg($"[StoneGateJoin] [ReadMessageServer] StoneGates items count: {Objects.Track.spawendStoneGates.Count}", true);
                }

                if (!collectionsValid)
                {
                    Misc.Msg("[StoneGateJoin] [ReadMessageServer] One or more collections is invalid, not sending sync data", true);
                    // Send a small valid response instead
                    SendEmptyResponse(fromConnection);
                    return;
                }


                // If collections are valid, send the full sync event
                foreach (var item in Objects.Track.spawendStoneGates)
                {
                    try
                    {
                        Misc.Msg($"[StoneGateJoin] Processing item: Key={item.Key}, Value={item.Value}", true);

                        var boltEntity = item.Key;
                        if (boltEntity == null)
                        {
                            Misc.Msg($"[StoneGateJoin] [ReadMessageServer] BoltEntity is null for {item.Value?.name ?? "unknown"}", true);
                            continue;
                        }

                        Misc.Msg($"[StoneGateJoin] BoltEntity valid, getting StoneGateSetter component", true);
                        var StoneGateSetter = boltEntity.GetComponent<Network.StoneGateSetter>();
                        if (StoneGateSetter == null)
                        {
                            Misc.Msg($"[StoneGateJoin] [ReadMessageServer] StoneGateSetter is null for {item.Value?.name ?? "unknown"}", true);
                            continue;
                        }

                        Misc.Msg($"[StoneGateJoin] StoneGateSetter valid, calling SendState", true);
                        if (item.Value.GetComponent<Mono.StoneGateMono>().IsGateOpen())
                        {
                            Network.StoneGateSyncEvent.SendState(boltEntity, StoneGateSyncEvent.StoneGateSyncType.OpenGate, toPlayerSteamId);
                        }
                        else
                        {
                            Network.StoneGateSyncEvent.SendState(boltEntity, StoneGateSyncEvent.StoneGateSyncType.CloseGate, toPlayerSteamId);
                        }
                        Misc.Msg($"[StoneGateJoin] SendState completed successfully", true);
                    }
                    catch (System.Exception ex)
                    {
                        Misc.Msg($"[StoneGateJoin] Error processing item: {ex.Message}\n{ex.StackTrace}", true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[StoneGateJoin] [ReadMessageServer] Error handling request: {ex.Message}", true);
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
                Misc.Msg("[StoneGateJoin] [SendEmptyResponse] Sent empty response instead", true);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[StoneGateJoin] [SendEmptyResponse] Error sending empty response: {ex.Message}", true);
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
                    Misc.Msg($"[StoneGateJoin] [ReadMessageClient] Received StoneGateJoin RESPONSE: Empty response received", true);
                    return;
                }
                else
                {
                    Misc.Msg($"[StoneGateJoin] [ReadMessageClient] Received StoneGateJoin RESPONSE: Something went wrong!", true);
                    SonsTools.ShowMessage("Something went wrong while syncing the initial data with server, please rejoin", 5f);
                    return;
                }
            }
        }

        private void SendServerResponse()
        {
            Misc.Msg($"Sending StoneGateJoin REQUEST", true);
            try
            {
                // Check if BoltNetwork is running before sending
                if (!BoltNetwork.isRunning)
                {
                    Misc.Msg("[StoneGateJoin] [SendServerResponse] BoltNetwork is not running", true);
                    return;
                }

                // Check if we have a valid server to send to
                if (BoltNetwork.server == null)
                {
                    Misc.Msg("[StoneGateJoin] [SendServerResponse] BoltNetwork.server is null", true);
                    return;
                }
                if (BoltNetwork.isServer)
                {
                    Misc.Msg("[StoneGateJoin] [SendServerResponse] Not a client, skipping request", true);
                    return;
                }

                var packet = NewPacket(64, GlobalTargets.OnlyServer);
                packet.Packet.WriteString("REQUEST_SYNC");
                packet.Packet.WriteString(Misc.GetLocalPlayerSteamId());
                Send(packet);

                Misc.Msg("[StoneGateJoin] [SendServerResponse] Successfully sent request to server", true);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[StoneGateJoin] [SendServerResponse] Error sending request: {ex.Message}", true);
            }
        }

        public void RequestInfoFromServer() => SendServerResponse();

        public override string Id => "StoneGate_StoneGateJoin";
    }
}
