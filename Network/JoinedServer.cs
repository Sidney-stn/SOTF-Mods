using SimpleNetworkEvents;
using UnityEngine;


namespace Shops.Network
{
    internal class JoinedServer : SimpleEvent<JoinedServer>
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public override void OnReceived()
        {
            if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.Multiplayer)  // Only Host Should Respond The JoinedServer Event
            {
                Misc.Msg("[JoinedServer] Player Joined, Sending Get All Shops Event");
                if (Saving.Load.ModdedShops.Count > 0)
                {
                    if (SenderId != Banking.API.GetLocalPlayerId())
                    {
                        Misc.Msg("[JoinedServer] Player Joined, Sending All Shops Sync Event");
                        foreach (var sign in Saving.Load.ModdedShops)
                        {
                            Mono.Shop controller = sign.GetComponent<Mono.Shop>();
                            string uniqueId = controller.UniqueId;
                            string vector3Position = Network.CustomSerializable.Vector3ToString((Vector3)controller.GetPos());
                            string rotation = Network.CustomSerializable.QuaternionToString((Quaternion)controller.GetCurrentRotation());
                            string ownerName = controller.OwnerName;
                            string ownerId = controller.OwnerId;
                            List<int> stationPrices = controller.StationPrices;
                            List<int> stationItems = controller.StationItems;
                            List<int> stationQuantities = controller.StationQuantities;

                            if (!string.IsNullOrEmpty(uniqueId))
                            {
                                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SpawnShop
                                {
                                    Vector3Position = vector3Position,
                                    QuaternionRotation = rotation,
                                    UniqueId = uniqueId,
                                    Sender = Banking.API.GetLocalPlayerId(),
                                    SenderName = Banking.API.GetLocalPlayerName(),
                                    OwnerName = ownerName,
                                    OwnerId = ownerId,
                                    StationPrices = stationPrices,
                                    StationItems = stationItems,
                                    StationQuantities = stationQuantities,
                                    NumberOfSellableItems = 1,
                                    ToSteamId = "None"
                                });
                            }
                            else
                            {
                                Misc.Msg("[JoinedServer] UniqueId is Null/Empty Witch It Shoud Never Be!");
                            }
                        }
                    }
                    else { Misc.Msg("[JoinedServer] Got Joined Server Event From MySelf, while being host, Skipped"); }
                }
            }
        }
    }
}
