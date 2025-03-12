using RedLoader;
using SonsSdk.Networking;
using UdpKit;
using UnityEngine;

namespace StoneGate.Network
{
    internal class StoneGateSetter : MonoBehaviour, Packets.IPacketReader
    {
        public void ReadPacket(UdpPacket packet, BoltConnection fromConnection)
        {
            if (BoltNetwork.isServer)
            {
                Misc.Msg("[StoneGateSetter] [ReadPacket] Recived packet on server", true);
            }
            else
            {
                Misc.Msg("[StoneGateSetter] [ReadPacket] Recived packet on client", true);
            }
            var type = (StoneGateSyncEvent.StoneGateSyncType)packet.ReadByte();
            string toPlayerSteamId = packet.ReadString();
            if (SonsSdk.Networking.NetUtils.IsDedicatedServer == false && toPlayerSteamId.ToLower() != "all" && toPlayerSteamId != Misc.GetLocalPlayerSteamId())
            {
                Misc.Msg("[ReciverSetter] [ReadPacket] Recived packet not meant for this player", true);
                Misc.Msg($"[ReciverSetter] [ReadPacket] Recived packet meant for: {toPlayerSteamId}", true);
                return;
            }
            switch (type)
            {
                case StoneGateSyncEvent.StoneGateSyncType.OpenGate:
                    if (packet.ReadBool() == false) { return; }
                    GetComponent<Mono.StoneGateMono>().OpenGate(false);
                    break;
                case StoneGateSyncEvent.StoneGateSyncType.CloseGate:
                    if (packet.ReadBool() == true) { return; }
                    GetComponent<Mono.StoneGateMono>().CloseGate(false);
                    break;
            }
        }
    }
}
