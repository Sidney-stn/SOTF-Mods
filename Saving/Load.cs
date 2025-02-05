using UnityEngine;

namespace Shops.Saving
{
    internal class Load
    {
        public static List<GameObject> ModdedShops = new List<GameObject>();
        internal static Queue<Saving.Manager.ShopsManager> deferredLoadQueue = new Queue<Saving.Manager.ShopsManager>();

        internal static void ProcessLoadData(Saving.Manager.ShopsManager obj)
        {
            if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Loading] Skipped Loading Object On Multiplayer Client");
                return;
            }
            // Shop Prefab
            ModdedShops.Clear();
            Misc.Msg($"[Loading] Shop From Save: {obj.Shops.Count.ToString()}");
            foreach (var modData in obj.Shops)
            {
                Misc.Msg("[Loading] Creating New Shops");
                Prefab.SingleShop.Spawn(
                    pos: modData.Position,
                    rot: modData.Rotation,
                    ownerName: modData.OwnerName,
                    ownerId: modData.OwnerId,
                    uniqueId: modData.UniqueId,
                    StationPrices: modData.Prices,
                    StationItems: modData.Items,
                    StationQuantities: modData.Quanteties
                    );
            }
        }
    }
}
