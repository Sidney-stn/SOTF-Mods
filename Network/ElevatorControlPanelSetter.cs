using SonsSdk.Networking;
using UdpKit;
using UnityEngine;

namespace SimpleElevator.Network
{
    internal class ElevatorControlPanelSetter : MonoBehaviour, Packets.IPacketReader
    {
        public void ReadPacket(UdpPacket packet, BoltConnection fromConnection)
        {
            if (BoltNetwork.isServer)
            {
                Misc.Msg("[ElevatorControlPanelSetter] [ReadPacket] Recived packet on server", true);
            }
            else
            {
                Misc.Msg("[ElevatorControlPanelSetter] [ReadPacket] Recived packet on client", true);
            }
            var type = (ElevatorSyncEvent.ElevatorSyncType)packet.ReadByte();
            string toPlayerSteamId = packet.ReadString();
            if (SonsSdk.Networking.NetUtils.IsDedicatedServer == false)
            {
                if (toPlayerSteamId.ToLower() != "all" && toPlayerSteamId != Misc.SteamId())
                {
                    Misc.Msg("[ElevatorControlPanelSetter] [ReadPacket] Recived packet not meant for this player", true);
                    Misc.Msg($"[ElevatorControlPanelSetter] [ReadPacket] Recived packet meant for: {toPlayerSteamId}", true);
                    return;
                }
            }
            
            switch (type)
            {
                default:
                    break;
            }
        }
    }
}
