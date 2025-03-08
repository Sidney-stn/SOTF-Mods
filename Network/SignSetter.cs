using SonsSdk.Networking;
using UdpKit;
using UnityEngine;

namespace Signs.Network
{
    internal class SignSetter : MonoBehaviour, Packets.IPacketReader
    {
        public void ReadPacket(UdpPacket packet, BoltConnection fromConnection)
        {
            if (BoltNetwork.isServer)
            {
                Misc.Msg("[ReciverSetter] [ReadPacket] Recived packet on server", true);
            }
            else
            {
                Misc.Msg("[ReciverSetter] [ReadPacket] Recived packet on client", true);
            }
            var type = (SignSyncEvent.SignSyncType)packet.ReadByte();
            string toPlayerSteamId = packet.ReadString();
            if (SonsSdk.Networking.NetUtils.IsDedicatedServer == false && toPlayerSteamId.ToLower() != "all" && toPlayerSteamId != Misc.MySteamId().Item2)
            {
                Misc.Msg("[ReciverSetter] [ReadPacket] Recived packet not meant for this player", true);
                Misc.Msg($"[ReciverSetter] [ReadPacket] Recived packet meant for: {toPlayerSteamId}", true);
                return;
            }
            switch (type)
            {
                case SignSyncEvent.SignSyncType.SetTextAll:
                    var signContoller = GetComponent<Mono.SignController>();
                    if (signContoller == null)
                    {
                        Misc.Msg("[ReciverSetter] [ReadPacket] SignController is null", true);
                        return;
                    }
                    string line1 = packet.ReadString();
                    string line2 = packet.ReadString();
                    string line3 = packet.ReadString();
                    string line4 = packet.ReadString();
                    signContoller.SetLineText(1, line1.ToLower() == "none" ? "" : line1, false);
                    signContoller.SetLineText(2, line2.ToLower() == "none" ? "" : line2, false);
                    signContoller.SetLineText(3, line3.ToLower() == "none" ? "" : line3, false);
                    signContoller.SetLineText(4, line4.ToLower() == "none" ? "" : line4, false);
                    break;
                case SignSyncEvent.SignSyncType.SetTextLine1:
                    var signContoller1 = GetComponent<Mono.SignController>();
                    if (signContoller1 == null)
                    {
                        Misc.Msg("[ReciverSetter] [ReadPacket] SignController is null", true);
                        return;
                    }
                    string line1_1 = packet.ReadString();
                    signContoller1.SetLineText(1, line1_1.ToLower() == "none" ? "" : line1_1, false);
                    break;
                case SignSyncEvent.SignSyncType.SetTextLine2:
                    var signContoller2 = GetComponent<Mono.SignController>();
                    if (signContoller2 == null)
                    {
                        Misc.Msg("[ReciverSetter] [ReadPacket] SignController is null", true);
                        return;
                    }
                    string line2_1 = packet.ReadString();
                    signContoller2.SetLineText(2, line2_1.ToLower() == "none" ? "" : line2_1, false);
                    break;
                case SignSyncEvent.SignSyncType.SetTextLine3:
                    var signContoller3 = GetComponent<Mono.SignController>();
                    if (signContoller3 == null)
                    {
                        Misc.Msg("[ReciverSetter] [ReadPacket] SignController is null", true);
                        return;
                    }
                    string line3_1 = packet.ReadString();
                    signContoller3.SetLineText(3, line3_1.ToLower() == "none" ? "" : line3_1, false);
                    break;
                case SignSyncEvent.SignSyncType.SetTextLine4:
                    var signContoller4 = GetComponent<Mono.SignController>();
                    if (signContoller4 == null)
                    {
                        Misc.Msg("[ReciverSetter] [ReadPacket] SignController is null", true);
                        return;
                    }
                    string line4_1 = packet.ReadString();
                    signContoller4.SetLineText(4, line4_1.ToLower() == "none" ? "" : line4_1, false);
                    break;
                case SignSyncEvent.SignSyncType.Destroy:
                    Saving.Track.RemoveWithGameObject(gameObject);  // Remove the object from the save list
                    if (BoltNetwork.isRunning && BoltNetwork.isServer)
                    {
                        BoltNetwork.Destroy(gameObject);  // Destroy the object on all clients
                        Misc.Msg("[ReciverSetter] [ReadPacket] Destroying object on all clients", true);
                    }
                    else if (!BoltNetwork.isRunning)
                    {
                        Destroy(gameObject);  // Destroy the object on the client
                        Misc.Msg("[ReciverSetter] [ReadPacket] Destroying object on client", true);
                    }
                    break;
            }
        }
    }
}
