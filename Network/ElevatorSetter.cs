using SonsSdk.Networking;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

namespace SimpleElevator.Network
{
    internal class ElevatorSetter : MonoBehaviour, Packets.IPacketReader
    {
        // Configurable values for elevator
        private float controlPanelOffset = 0f; // Offset from control panel
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
            Mono.ElevatorMono elevatorMono = GetComponent<Mono.ElevatorMono>();
            if (elevatorMono == null)
            {
                Misc.Msg("[ElevatorSetter] [ReadPacket] ElevatorMono component is null", true);
                return;
            }

            // Read additional data (we don't use this currently, but it's part of the format)
            string actionData = packet.ReadString();

            // Define the layer mask for the OverlapBox
            int layerMask = LayerMask.GetMask(new string[]
            {
                //"Terrain",
                "Default",
                //"Prop"
            });

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
                    case ElevatorSyncEvent.ElevatorSyncType.Destroy:
                        Misc.Msg("[ElevatorSetter] [ReadPacket] Server executing Destroy command", true);
                        BoltNetwork.Destroy(gameObject); // Execute the destruction and broadcast to all clients
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
                        // Find the closest control panel above
                        GameObject closestControlPanel = FindClosestControlPanel(elevatorMono.transform.position, Vector3.up, 20f, layerMask);

                        if (closestControlPanel != null)
                        {
                            // Apply the visual movement locally without sending network events
                            Vector3 targetPosition = new Vector3(
                                elevatorMono.transform.position.x,
                                closestControlPanel.transform.position.y - controlPanelOffset,
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
                        // Find the closest control panel below
                        GameObject closestControlPanelDown = FindClosestControlPanel(elevatorMono.transform.position, Vector3.down, 20f, layerMask);

                        if (closestControlPanelDown != null)
                        {
                            // Apply the visual movement locally without sending network events
                            Vector3 targetPosition = new Vector3(
                                elevatorMono.transform.position.x,
                                closestControlPanelDown.transform.position.y + controlPanelOffset,
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

        // Helper method to find all control panels in a specific direction
        private List<GameObject> FindControlPanels(Vector3 startPosition, Vector3 direction, float maxDistance, int layerMask)
        {
            List<GameObject> controlPanels = new List<GameObject>();
            Vector3 halfExtents = new Vector3(2f, 2f, 2f);
            int checkPoints = 4; // Number of check points along the path

            for (int i = 1; i <= checkPoints; i++)
            {
                float distanceFraction = (float)i / checkPoints;
                Vector3 checkPosition = startPosition + (direction * maxDistance * distanceFraction);

                // Use OverlapBox to find all colliders at this position
                Collider[] colliders = Physics.OverlapBox(
                    checkPosition,
                    halfExtents,
                    Quaternion.identity,
                    layerMask
                );

                foreach (Collider collider in colliders)
                {
                    if (Settings.logRaycastHit)
                    {
                        Misc.Msg($"[ElevatorSetter] [FindControlPanels] Found collider: {collider.gameObject.name}", true);
                    }
                        if (collider.transform.root.gameObject.name.Contains("EControlPanel"))
                    {
                        if (!controlPanels.Contains(collider.gameObject))
                        {
                            controlPanels.Add(collider.gameObject);
                        }
                    }
                }
            }

            return controlPanels;
        }

        // Helper method to find the closest control panel in a specific direction
        private GameObject FindClosestControlPanel(Vector3 startPosition, Vector3 direction, float maxDistance, int layerMask)
        {
            List<GameObject> controlPanels = FindControlPanels(startPosition, direction, maxDistance, layerMask);

            // Find the closest control panel in the specified direction
            GameObject closestControlPanel = null;
            float closestDistance = float.MaxValue;

            foreach (GameObject panel in controlPanels)
            {
                float distance = Vector3.Distance(startPosition, panel.transform.position);
                bool isCorrectDirection = direction.y > 0 ?
                    panel.transform.position.y > startPosition.y :
                    panel.transform.position.y < startPosition.y;

                if (distance < closestDistance && isCorrectDirection)
                {
                    closestDistance = distance;
                    closestControlPanel = panel;
                }
            }

            return closestControlPanel;
        }
    }
}