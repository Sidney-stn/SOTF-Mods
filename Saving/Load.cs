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
                //if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
                //{
                //    GameObject sign = Prefab.SignPrefab.spawnSignSingePlayer(modData.Position, modData.Rotation, false, modData.Line1Text, modData.Line2Text, modData.Line3Text, modData.Line4Text, modData.UniqueId);
                //}
                //else if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
                //{
                //    GameObject sign = Prefab.SignPrefab.spawnSignSingePlayer(modData.Position, modData.Rotation, false, modData.Line1Text, modData.Line2Text, modData.Line3Text, modData.Line4Text, modData.UniqueId);
                //}
            }
        }
    }
}
