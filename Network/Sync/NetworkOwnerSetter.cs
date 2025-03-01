using RedLoader;
using SonsSdk.Networking;
using UdpKit;
using UnityEngine;

namespace WirelessSignals.Network.Sync
{
    public class NetworkOwnerSetter : MonoBehaviour, Packets.IPacketReader
    {

        private void AddNetworkOwnerComp()
        {
            var comp = gameObject.GetComponent<Mono.NetworkOwner>();
            if (comp == null)
            {
                if (SonsSdk.Networking.NetUtils.IsDedicatedServer)
                {
                    Misc.Msg("[NetworkOwnerSetter] [AddNetworkOwnerComp] Skip Adding Comp - Recieved On DedicatedServer", true);
                    return;
                }
                var addedComp = gameObject.AddComponent<Mono.NetworkOwner>().fromNetwork = true;
                Misc.Msg("[NetworkOwnerSetter] [AddNetworkOwnerComp] Added NetworkOwner Component", true);
            } else
            {
                Misc.Msg("[NetworkOwnerSetter] [AddNetworkOwnerComp] NetworkOwner Component Already Exists", true);
            }
        }

        private void RemoveNetworkOwnerComp()
        {
            if (SonsSdk.Networking.NetUtils.IsDedicatedServer)
            {
                Misc.Msg("[NetworkOwnerSetter] [ReadPacket] [RemoveFromBoltEntity] Skip Removing Comp - Recieved On DedicatedServer", true);
                return;
            }
            var comp = gameObject.GetComponent<Mono.NetworkOwner>();
            if (comp != null)
            {
                DestroyImmediate(comp);
                Misc.Msg("[NetworkOwnerSetter] [RemoveNetworkOwnerComp] Removed NetworkOwner Component", true);
            }
        }

        public void ReadPacket(UdpPacket packet, BoltConnection fromConnection)
        {
            var selectedCase = (Network.Sync.NetworkOwnerSyncEvent.SyncType)packet.ReadByte();
            Misc.Msg($"[NetworkOwnerSetter] [ReadPacket] Recived {selectedCase}: From Connection", true);
            switch (selectedCase)
            {
                case Network.Sync.NetworkOwnerSyncEvent.SyncType.PlaceOnBoltEntity:
                    string fromNetwork = packet.ReadString();
                    if (fromNetwork == "PLACE_NETWORK_OWNER_SCRIPT")
                    {
                        AddNetworkOwnerComp();
                        Misc.Msg("[NetworkOwnerSetter] [ReadPacket] [PlaceOnBoltEntity] Added NetworkOwner Component", true);
                    }
                    else
                    {
                        Misc.Msg($"[NetworkOwnerSetter] [ReadPacket] [PlaceOnBoltEntity] Unknown string: {fromNetwork}", true);
                    }
                    break;
                case NetworkOwnerSyncEvent.SyncType.RemoveFromBoltEntity:
                    string fromNetworkRemove = packet.ReadString();
                    if (fromNetworkRemove == "REMOVE_NETWORK_OWNER_SCRIPT")
                    {
                        RemoveNetworkOwnerComp();
                        Misc.Msg("[NetworkOwnerSetter] [ReadPacket] [RemoveFromBoltEntity] Removed NetworkOwner Component", true);
                    }
                    else
                    {
                        Misc.Msg($"[NetworkOwnerSetter] [ReadPacket] [RemoveFromBoltEntity] Unknown string: {fromNetworkRemove}", true);
                    }
                    break;
            }

        }
    }
}
