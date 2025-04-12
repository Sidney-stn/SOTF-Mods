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
                Misc.Msg("[ElevatorControlPanelSetter] [ReadPacket] Received packet on server", true);
            }
            else
            {
                Misc.Msg("[ElevatorControlPanelSetter] [ReadPacket] Received packet on client", true);
            }

            // Read the sync type
            var type = (ElevatorControlPanelSyncEvent.ElevatorControlPanelSyncType)packet.ReadByte();

            // Get the ElevatorControlPanelMono component
            Mono.ElevatorControlPanelMono controlPanelMono = GetComponent<Mono.ElevatorControlPanelMono>();
            if (controlPanelMono == null)
            {
                Misc.Msg("[ElevatorControlPanelSetter] [ReadPacket] ElevatorControlPanelMono component is null", true);
                return;
            }

            // Read additional data (we don't use this currently, but it's part of the format)
            string actionData = packet.ReadString();

            // Process the event based on if we're server or client
            switch (type)
            {
                case ElevatorControlPanelSyncEvent.ElevatorControlPanelSyncType.CallElevator:
                    if (BoltNetwork.isServer)
                    {
                        Misc.Msg("[ElevatorControlPanelSetter] [ReadPacket] Server received CallElevator command", true);
                        // Find the nearest elevator and tell it to move to this control panel
                        // This would be implemented in a real scenario
                    }
                    else if (BoltNetwork.isClient)
                    {
                        Misc.Msg("[ElevatorControlPanelSetter] [ReadPacket] Client received CallElevator command", true);
                        // Update UI or other visual elements to show the elevator has been called
                    }
                    break;
                case ElevatorControlPanelSyncEvent.ElevatorControlPanelSyncType.Destroy:
                    if (BoltNetwork.isServer)
                    {
                        Misc.Msg("[ElevatorControlPanelSetter] [ReadPacket] Server received Destroy command", true);
                        // Destroy the control panel
                        BoltNetwork.Destroy(gameObject);
                    }
                    break;
                default:
                    Misc.Msg("[ElevatorControlPanelSetter] [ReadPacket] Unknown command type", true);
                    break;
            }
        }
    }
}