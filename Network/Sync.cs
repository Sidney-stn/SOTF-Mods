using SimpleNetworkEvents;
using UnityEngine;

namespace Shops.Network
{
    internal class Sync
    {
        internal class RequestSpawnShop : SimpleEvent<RequestSpawnShop>
        {
            public string UniqueId { get; set; }  // If Null, Spawn All Shops
            public string Sender { get; set; }
            public string ToSteamId { get; set; }


            public override void OnReceived()
            {
                Misc.NetLog("Recived Network SyncShop Event");
                if (Banking.API.GetHostMode() != Banking.API.SimpleSaveGameType.Multiplayer)
                {
                    Misc.NetLog("[RequestSyncShop] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                    return;
                }
                if (Prefab.SingleShop.gameObjectWithComps == null)
                {
                    Misc.NetLog("[RequestSyncShop] [OnReceived()] Shop Prefab has not been created yet, skipped");
                    return;
                }
                if (string.IsNullOrEmpty(Banking.API.GetLocalPlayerId()))
                {
                    Misc.NetLog("[RequestSyncShop] [OnReceived()] Could Not Get My SteamId, Cant Be Sure Who Has The Shop, Skipped");
                    return;
                }
                else
                {
                    if (Sender == Banking.API.GetLocalPlayerId())
                    {
                        Misc.NetLog("[RequestSyncShop] [OnReceived()] Not Creating Shop Over Network When Its From My SteamID, skipped");
                        return;
                    }
                }
                if (ToSteamId != null && ToSteamId != "None")
                {
                    if (ToSteamId != Banking.API.GetLocalPlayerId())
                    {
                        Misc.NetLog("[RequestSyncShop] [OnReceived()] Not Creating Shop Over Network When Its Not For Me, skipped");
                        return;
                    }
                }
                // Extra Shop Sesific Checks
                

                Misc.NetLog($"[RequestSyncShop] [OnReceived()] Syncing Prefab From Network Event");
                if (string.IsNullOrEmpty(UniqueId) || UniqueId == "0")
                {
                    Misc.NetLog("[RequestSyncShop] [OnReceived()] UniqueId Is Null Or Empty, syncing all shops");
                    if (Saving.Load.ModdedShops.Count > 0)
                    {
                        if (Sender != Banking.API.GetLocalPlayerId())
                        {
                            Misc.Msg("[RequestSyncShop] Sending All Shops Sync Event");
                            foreach (var shop in Saving.Load.ModdedShops)
                            {
                                Mono.Shop controller = shop.GetComponent<Mono.Shop>();
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
                                        ToSteamId = Sender
                                    });
                                }
                                else
                                {
                                    Misc.Msg("[RequestSyncShop] UniqueId is Null/Empty Witch It Shoud Never Be!");
                                }
                            }
                        }
                        else { Misc.Msg("[RequestSyncShop] Got Joined Server Event From MySelf, while being host, Skipped"); }
                    }
                } 
                else
                {
                    // Check If Shop Exists
                    if (Prefab.SingleShop.DoesShopWithUniqueIdExist(UniqueId))
                    {
                        GameObject prefab = Prefab.SingleShop.FindShopByUniqueId(UniqueId);
                        if (prefab != null)
                        {
                            Mono.Shop mono = prefab.GetComponent<Mono.Shop>();
                            if (mono != null)
                            {
                                // Send SpawnShop Event To Sender
                                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SpawnShop
                                {
                                    Vector3Position = Network.CustomSerializable.Vector3ToString(mono.GetPos()),
                                    QuaternionRotation = Network.CustomSerializable.QuaternionToString(mono.GetCurrentRotation()),
                                    UniqueId = mono.UniqueId,
                                    Sender = Banking.API.GetLocalPlayerId(),
                                    SenderName = Banking.API.GetLocalPlayerName(),
                                    OwnerName = mono.OwnerName,
                                    OwnerId = mono.OwnerId,
                                    StationPrices = mono.StationPrices,
                                    StationItems = mono.StationItems,
                                    StationQuantities = mono.StationQuantities,
                                    NumberOfSellableItems = mono.numberOfSellableItems,
                                    ToSteamId = Sender
                                });
                            }
                        }
                    } 
                    else
                    {
                        // If shop does not exist on host pc, send remove shop event to all clients
                        // Cloud be that somehow and extra shop id had been made without syncing
                        SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RemoveShop
                        {
                            UniqueId = UniqueId,
                        });
                        // When That Event Is Recived, The Shop Will Be Removed From All Clients And Host

                    }
                }
            }
        }
        internal class SyncShop : SimpleEvent<SyncShop>
        {
            public string UniqueId { get; set; }
            public string Sender { get; set; }
            public string SenderName { get; set; }
            public string OwnerName { get; set; }
            public string OwnerId { get; set; }
            public List<int> StationPrices { get; set; }
            public List<int> StationItems { get; set; }
            public List<int> StationQuantities { get; set; }
            public string ToSteamId { get; set; }


            public override void OnReceived()
            {
                Misc.NetLog("Recived Network SyncShop Event");
                if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.SinglePlayer)
                {
                    Misc.NetLog("[SyncShop] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                    return;
                }
                if (Prefab.SingleShop.gameObjectWithComps == null)
                {
                    Misc.NetLog("[SyncShop] [OnReceived()] Shop Prefab has not been created yet, skipped");
                    return;
                }
                if (string.IsNullOrEmpty(Banking.API.GetLocalPlayerId()))
                {
                    Misc.NetLog("[SyncShop] [OnReceived()] Could Not Get My SteamId, Cant Be Sure Who Has The Shop, Skipped");
                    return;
                }
                else
                {
                    if (Sender == Banking.API.GetLocalPlayerId())
                    {
                        Misc.NetLog("[SyncShop] [OnReceived()] Not Creating Shop Over Network When Its From My SteamID, skipped");
                        return;
                    }
                }
                if (ToSteamId != null && ToSteamId != "None")
                {
                    if (ToSteamId != Banking.API.GetLocalPlayerId())
                    {
                        Misc.NetLog("[SyncShop] [OnReceived()] Not Creating Shop Over Network When Its Not For Me, skipped");
                        return;
                    }
                }
                // Extra Shop Sesific Checks
                if (string.IsNullOrEmpty(OwnerName))
                {
                    Misc.NetLog("[SyncShop] [OnReceived()] OwnerName Is Null Or Empty, skipped");
                    return;
                }
                if (string.IsNullOrEmpty(OwnerId))
                {
                    Misc.NetLog("[SyncShop] [OnReceived()] OwnerId Is Null Or Empty, skipped");
                    return;
                }
                if (string.IsNullOrEmpty(UniqueId) || UniqueId == "0")
                {
                    Misc.NetLog("[SyncShop] [OnReceived()] UniqueId Is Null Or Empty, skipped");
                    return;
                }

                Misc.NetLog($"[SyncShop] [OnReceived()] Syncing Prefab From Network Event");
                // Check If Shop Exists
                if (Prefab.SingleShop.DoesShopWithUniqueIdExist(UniqueId))
                {
                    GameObject prefab = Prefab.SingleShop.FindShopByUniqueId(UniqueId);
                    if (prefab != null)
                    {
                        Mono.Shop mono = prefab.GetComponent<Mono.Shop>();
                        if (mono != null)
                        {
                            mono.UpdateAll(StationPrices, StationItems, StationQuantities, OwnerName, OwnerId);
                        }
                    }
                } 
                else
                {
                    // If Shop Does Not Exist, Send Request SpawnShop Event (Only Host Recives/Replies To It)
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.Sync.RequestSpawnShop
                    {
                        UniqueId = UniqueId,
                        Sender = Banking.API.GetLocalPlayerId(),
                        ToSteamId = "None"
                    });
                }

            }
        }
        internal class SyncShopList : SimpleEvent<SyncShopList>
        {
            public string UniqueId { get; set; }
            public string Sender { get; set; }
            public string SenderName { get; set; }
            public List<int> StationPrices { get; set; }
            public List<int> StationItems { get; set; }
            public List<int> StationQuantities { get; set; }
            public string ToSteamId { get; set; }


            public override void OnReceived()
            {
                Misc.NetLog("Recived Network SyncShopList Event");
                if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.SinglePlayer)
                {
                    Misc.NetLog("[SyncShopList] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                    return;
                }
                if (Prefab.SingleShop.gameObjectWithComps == null)
                {
                    Misc.NetLog("[SyncShopList] [OnReceived()] Shop Prefab has not been created yet, skipped");
                    return;
                }
                if (string.IsNullOrEmpty(Banking.API.GetLocalPlayerId()))
                {
                    Misc.NetLog("[SyncShopList] [OnReceived()] Could Not Get My SteamId, Cant Be Sure Who Has The Shop, Skipped");
                    return;
                }
                else
                {
                    if (Sender == Banking.API.GetLocalPlayerId())
                    {
                        Misc.NetLog("[SyncShopList] [OnReceived()] Not Creating Shop Over Network When Its From My SteamID, skipped");
                        return;
                    }
                }
                if (ToSteamId != null && ToSteamId != "None")
                {
                    if (ToSteamId != Banking.API.GetLocalPlayerId())
                    {
                        Misc.NetLog("[SyncShopList] [OnReceived()] Not Creating Shop Over Network When Its Not For Me, skipped");
                        return;
                    }
                }
                // Extra Shop Sesific Checks
                if (string.IsNullOrEmpty(UniqueId) || UniqueId == "0")
                {
                    Misc.NetLog("[SyncShopList] [OnReceived()] UniqueId Is Null Or Empty, skipped");
                    return;
                }

                Misc.NetLog($"[SyncShopList] [OnReceived()] Syncing Prefab From Network Event");
                // Check If Shop Exists
                if (Prefab.SingleShop.DoesShopWithUniqueIdExist(UniqueId))
                {
                    GameObject prefab = Prefab.SingleShop.FindShopByUniqueId(UniqueId);
                    if (prefab != null)
                    {
                        Mono.Shop mono = prefab.GetComponent<Mono.Shop>();
                        if (mono != null)
                        {
                            if (StationPrices != null && StationItems != null && StationQuantities != null)
                            {
                                mono.UpdateAllLists(StationPrices, StationItems, StationQuantities);
                            } else if (StationItems != null && StationQuantities != null)
                            {
                                mono.UpdateStationItems(StationItems);
                                mono.UpdateStationQuantities(StationQuantities);
                            } else if (StationPrices != null)
                            {
                                mono.UpdateStationPrices(StationPrices);
                            }
                        }
                    }
                }
                else
                {
                    // If Shop Does Not Exist, Send Request SpawnShop Event (Only Host Recives/Replies To It)
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.Sync.RequestSpawnShop
                    {
                        UniqueId = UniqueId,
                        Sender = Banking.API.GetLocalPlayerId(),
                        ToSteamId = "None"
                    });
                }

            }
        }
    }
}
