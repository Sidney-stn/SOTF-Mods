using SimpleNetworkEvents;
using UnityEngine;


namespace Signs.Network
{
    internal class RemoveSign : SimpleEvent<RemoveSign>
    {
        public string UniqueId { get; set; }


        public override void OnReceived()
        {
            Misc.Msg("Recived Network Remove Sign Event");
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
            {
                Misc.Msg("[RemoveSign] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                return;
            }

            if (Config.NetworkDebugIngameSign.Value) { Misc.Msg($"[RemoveSign] [OnReceived()] Removing Prefab From Network Event"); }

            if (Prefab.SignPrefab.DoesShopWithUniqueIdExist(UniqueId))
            {
                GameObject sign = Prefab.SignPrefab.FindShopByUniqueId(UniqueId);
                if (sign != null)
                {
                    Prefab.SignPrefab.spawnedSigns.Remove(UniqueId);
                    Saving.Load.ModdedSigns.Remove(sign);
                    UnityEngine.Object.Destroy(sign);
                }
                else
                {
                    Misc.Msg($"[RemoveSign] [OnReceived()] Sign With UniqueId: {UniqueId} Does Not Exist, Cant Remove");
                }
            }
            else
            {
                Misc.Msg($"[RemoveSign] [OnReceived()] Sign With UniqueId: {UniqueId} Does Not Exist, Cant Remove");
            }

        }
    }
}
