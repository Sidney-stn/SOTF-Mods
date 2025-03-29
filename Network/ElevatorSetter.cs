using SonsSdk.Networking;
using UdpKit;
using UnityEngine;

namespace SimpleElevator.Network
{
    internal class ElevatorSetter : MonoBehaviour, Packets.IPacketReader
    {
        public void ReadPacket(UdpPacket packet, BoltConnection fromConnection)
        {
            if (BoltNetwork.isServer)
            {
                Misc.Msg("[ElevatorSetter] [ReadPacket] Received packet on server", true);
            }
            else
            {
                Misc.Msg("[ElevatorSetter] [ReadPacket] Received packet on client", true);
            }

            // Read the entity from the packet
            BoltEntity entity = packet.ReadBoltEntity();
            if (entity == null)
            {
                Misc.Msg("[ElevatorSetter] [ReadPacket] Entity is null", true);
                return;
            }

            // Read the sync type
            var type = (ElevatorSyncEvent.ElevatorSyncType)packet.ReadByte();

            // Read the target player steam ID
            string toPlayerSteamId = packet.ReadString();
            if (SonsSdk.Networking.NetUtils.IsDedicatedServer == false)
            {
                if (toPlayerSteamId.ToLower() != "all" && toPlayerSteamId != Misc.SteamId())
                {
                    Misc.Msg("[ElevatorSetter] [ReadPacket] Received packet not meant for this player", true);
                    Misc.Msg($"[ElevatorSetter] [ReadPacket] Received packet meant for: {toPlayerSteamId}", true);
                    return;
                }
            }

            // Get the ElevatorMono component
            Mono.ElevatorMono elevatorMono = entity.GetComponent<Mono.ElevatorMono>();
            if (elevatorMono == null)
            {
                Misc.Msg("[ElevatorSetter] [ReadPacket] ElevatorMono component is null", true);
                return;
            }

            // Read additional data (we don't use this currently, but it's part of the format)
            string actionData = packet.ReadString();

            // Process the event based on if we're server or client
            if (BoltNetwork.isServer)
            {
                // If we're the server and received a request from a client
                switch (type)
                {
                    case ElevatorSyncEvent.ElevatorSyncType.MoveUp:
                        Misc.Msg("[ElevatorSetter] [ReadPacket] Server executing MoveUp command", true);
                        elevatorMono.MoveUp(true); // Execute the movement and broadcast to all clients
                        break;
                    case ElevatorSyncEvent.ElevatorSyncType.MoveDown:
                        Misc.Msg("[ElevatorSetter] [ReadPacket] Server executing MoveDown command", true);
                        elevatorMono.MoveDown(true); // Execute the movement and broadcast to all clients
                        break;
                    default:
                        Misc.Msg("[ElevatorSetter] [ReadPacket] Unknown command type", true);
                        break;
                }
            }
            else if (BoltNetwork.isClient)
            {
                // If we're a client and received an update from the server
                switch (type)
                {
                    case ElevatorSyncEvent.ElevatorSyncType.MoveUp:
                        Misc.Msg("[ElevatorSetter] [ReadPacket] Client applying MoveUp from server", true);
                        // Check if there's a control panel above
                        RaycastHit[] hits = Physics.BoxCastAll(
                            elevatorMono.transform.position,
                            new Vector3(2f, 2f, 2f),
                            Vector3.up,
                            Quaternion.identity,
                            20f
                        );

                        GameObject closestControlPanel = null;
                        float closestDistance = float.MaxValue;

                        foreach (RaycastHit hit in hits)
                        {
                            if (hit.transform.gameObject.name.Contains("EControlPanel"))
                            {
                                float distance = Vector3.Distance(elevatorMono.transform.position, hit.transform.position);
                                if (distance < closestDistance && hit.transform.position.y > elevatorMono.transform.position.y)
                                {
                                    closestDistance = distance;
                                    closestControlPanel = hit.transform.gameObject;
                                }
                            }
                        }

                        if (closestControlPanel != null)
                        {
                            // Apply the visual movement locally without sending network events
                            Vector3 targetPosition = new Vector3(
                                elevatorMono.transform.position.x,
                                closestControlPanel.transform.position.y - 1f,
                                elevatorMono.transform.position.z
                            );
                            elevatorMono.ClientSetTarget(closestControlPanel, targetPosition);
                        }
                        else
                        {
                            Misc.Msg("[ElevatorSetter] [ReadPacket] Client couldn't find control panel above", true);
                        }
                        break;
                    case ElevatorSyncEvent.ElevatorSyncType.MoveDown:
                        Misc.Msg("[ElevatorSetter] [ReadPacket] Client applying MoveDown from server", true);
                        // Check if there's a control panel below
                        RaycastHit[] hitsDown = Physics.BoxCastAll(
                            elevatorMono.transform.position,
                            new Vector3(2f, 2f, 2f),
                            Vector3.down,
                            Quaternion.identity,
                            20f
                        );

                        GameObject closestControlPanelDown = null;
                        float closestDistanceDown = float.MaxValue;

                        foreach (RaycastHit hit in hitsDown)
                        {
                            if (hit.transform.gameObject.name.Contains("EControlPanel"))
                            {
                                float distance = Vector3.Distance(elevatorMono.transform.position, hit.transform.position);
                                if (distance < closestDistanceDown && hit.transform.position.y < elevatorMono.transform.position.y)
                                {
                                    closestDistanceDown = distance;
                                    closestControlPanelDown = hit.transform.gameObject;
                                }
                            }
                        }

                        if (closestControlPanelDown != null)
                        {
                            // Apply the visual movement locally without sending network events
                            Vector3 targetPosition = new Vector3(
                                elevatorMono.transform.position.x,
                                closestControlPanelDown.transform.position.y + 1f,
                                elevatorMono.transform.position.z
                            );
                            elevatorMono.ClientSetTarget(closestControlPanelDown, targetPosition);
                        }
                        else
                        {
                            Misc.Msg("[ElevatorSetter] [ReadPacket] Client couldn't find control panel below", true);
                        }
                        break;
                    default:
                        Misc.Msg("[ElevatorSetter] [ReadPacket] Unknown command type", true);
                        break;
                }
            }
        }
    }
}