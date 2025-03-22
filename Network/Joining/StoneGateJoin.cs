

using Bolt;
using RedLoader;
using SonsSdk;
using UdpKit;
using static StoneGate.Saving.Manager;

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

                var hashSetStoreMono = Tools.Gates.GetAllStoneGateStoreMono();
                if (hashSetStoreMono == null)
                {
                    Misc.Msg("[StoneGateJoin] [ReadMessageServer] hashSetStoreMono is null");
                    SendEmptyResponse(fromConnection);
                    return;
                }
                if (hashSetStoreMono.Count <= 0)
                {
                    Misc.Msg("[StoneGateJoin] [ReadMessageServer] hashSetStoreMono is empty");
                    SendEmptyResponse(fromConnection);
                    return;
                }


                // If collections are valid, send the full sync event
                foreach (var mono in hashSetStoreMono)
                {
                    try
                    {
                        Misc.Msg($"[StoneGateJoin] [ReadMessageServer] Sending StoneGateJoin RESPONSE for gate", true);
                        int childIndex = mono.GetChildIndex();
                        GatesManager.GatesModData gatesModData = mono.GetSaveData();
                        Objects.CreateGateParent.RotateMode mode = Objects.CreateGateParent.RotateModeFromString(gatesModData.Mode);
                        try
                        {
                            Network.HostEvents.Instance.SendHostEvent(HostEvents.HostEvent.CreateStoneGate, gatesModData.RotationGoName, mode, gatesModData.FloorBeamName, gatesModData.TopBeamName, gatesModData.RockWallName, gatesModData.ExtraPillarName, childIndex);
                        }
                        catch (System.Exception ex)
                        {
                            Misc.Msg($"[StoneGateJoin] [ReadMessageServer] Error sending gate: {ex.Message}", true);
                        }
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
                string steamID = Misc.GetLocalPlayerSteamId();
                if (string.IsNullOrEmpty(steamID))
                {
                    RLog.Error("[StoneGate] [StoneGateJoin] [SendServerResponse] SteamID is null or empty", true);
                    return;
                }
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
