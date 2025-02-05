using SimpleNetworkEvents;
using UnityEngine;


namespace Shops.Network
{
    internal class RemoveShop : SimpleEvent<RemoveShop>
    {
        public string UniqueId { get; set; }

        public override void OnReceived()
        {
            Misc.NetLog("Recived Network Remove Shop Event");
            if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.SinglePlayer)
            {
                Misc.NetLog("[RemoveShop] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                return;
            }

            Misc.NetLog($"[RemoveShop] [OnReceived()] Removing Prefab From Network Event");

            if (Prefab.SingleShop.DoesShopWithUniqueIdExist(UniqueId))
            {
                GameObject Shop = Prefab.SingleShop.FindShopByUniqueId(UniqueId);
                if (Shop != null)
                {
                    Prefab.SingleShop.spawnedShops.Remove(UniqueId);
                    Saving.Load.ModdedShops.Remove(Shop);
                    UnityEngine.Object.Destroy(Shop);
                }
                else
                {
                    Misc.NetLog($"[RemoveShop] [OnReceived()] Shop With UniqueId: {UniqueId} Does Not Exist, Cant Remove");
                }
            }
            else
            {
                Misc.NetLog($"[RemoveShop] [OnReceived()] Shop With UniqueId: {UniqueId} Does Not Exist, Cant Remove");
            }

        }
    }
}
