using SimpleNetworkEvents;
using UnityEngine;

namespace Shops.Network
{
    internal class SpawnShop : SimpleEvent<SpawnShop>
    {
        public string Vector3Position { get; set; }
        public string QuaternionRotation { get; set; }
        public string UniqueId { get; set; }
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string OwnerName { get; set; }
        public string OwnerId { get; set; }
        public List<int> StationPrices { get; set; }
        public List<int> StationItems { get; set; }
        public List<int> StationQuantities { get; set; }
        public int NumberOfSellableItems { get; set; }
        public string ToSteamId { get; set; }


        public override void OnReceived()
        {
            Misc.NetLog("Recived Network SpawnShop Event");
            if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.SinglePlayer)
            {
                Misc.NetLog("[SpawnShop] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                return;
            }
            if (Prefab.SingleShop.gameObjectWithComps == null)
            {
                Misc.NetLog("[SpawnShop] [OnReceived()] Shop Prefab has not been created yet, skipped");
                return;
            }
            if (string.IsNullOrEmpty(Banking.API.GetLocalPlayerId()))
            {
                Misc.NetLog("[SpawnShop] [OnReceived()] Could Not Get My SteamId, Cant Be Sure Who Has The Shop, Skipped");
                return;
            }
            else
            {
                if (Sender == Banking.API.GetLocalPlayerId())
                {
                    Misc.NetLog("[SpawnShop] [OnReceived()] Not Creating Shop Over Network When Its From My SteamID, skipped");
                    return;
                }
            }
            if (ToSteamId != null && ToSteamId != "None")
            {
                if (ToSteamId != Banking.API.GetLocalPlayerId())
                {
                    Misc.NetLog("[SpawnShop] [OnReceived()] Not Creating Shop Over Network When Its Not For Me, skipped");
                    return;
                }
            }
            // Extra Shop Sesific Checks
            if (string.IsNullOrEmpty(OwnerName))
            {
                Misc.NetLog("[SpawnShop] [OnReceived()] OwnerName Is Null Or Empty, skipped");
                return;
            }
            if (string.IsNullOrEmpty(OwnerId))
            {
                Misc.NetLog("[SpawnShop] [OnReceived()] OwnerId Is Null Or Empty, skipped");
                return;
            }
            if (string.IsNullOrEmpty(UniqueId))
            {
                Misc.NetLog("[SpawnShop] [OnReceived()] UniqueId Is Null Or Empty, skipped");
                return;
            }

            Misc.NetLog($"[SpawnShop] [OnReceived()] Spawning Prefab From Network Event");

            Misc.NetLog($"[SpawnShop] [OnReceived()] Spawn Shop To STRING Pos: {Vector3Position}, STRING Rot: {QuaternionRotation}");
            Vector3 pos = Network.CustomSerializable.Vector3FromString(Vector3Position);
            Quaternion rot = Network.CustomSerializable.QuaternionFromString(QuaternionRotation);
            Misc.NetLog($"[SpawnShop] [OnReceived()] Spawn Shop To Pos: {pos}, Rot: {rot}");

            if (StationPrices != null && StationItems != null && StationQuantities != null && NumberOfSellableItems == 1)
            {
                Misc.NetLog("[SpawnShop] [OnReceived()] Setting Station Prices, Items, Quantities");
                Prefab.SingleShop.Spawn(pos, rot, OwnerName, OwnerId, UniqueId, StationPrices, StationItems, StationQuantities);
            }
            else
            {
                if (NumberOfSellableItems == 1)
                {
                    Misc.NetLog("[SpawnShop] [OnReceived()] Spawned Shop Without Lists");
                    Prefab.SingleShop.Spawn(pos, rot, OwnerName, OwnerId, UniqueId);
                }
                
            }
            

        }
    }
}
