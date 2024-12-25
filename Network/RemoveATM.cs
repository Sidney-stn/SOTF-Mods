using SimpleNetworkEvents;
using UnityEngine;

namespace Banking.Network
{
    internal class RemoveATM : SimpleEvent<RemoveATM>
    {
        public string UniqueId { get; set; }

        public override void OnReceived()
        {
            Misc.Msg("Recived Network Remove Sign Event");
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
            {
                Misc.Msg("[RemoveATM] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                return;
            }
            if (Misc.hostMode == Misc.SimpleSaveGameType.NotIngame)
            {
                Misc.Msg("[RemoveATM] [OnReceived()] Skipped Reciving Network Event Since Not Ingame");
                return;
            }

            if (Config.NetworkDebugIngameBanking.Value) { Misc.Msg($"[RemoveATM] [OnReceived()] Removing Prefab From Network Event"); }

            if (Prefab.ActiveATM.DoesShopWithUniqueIdExist(UniqueId))
            {
                GameObject atm = Prefab.ActiveATM.FindShopByUniqueId(UniqueId);
                if (atm != null)
                {
                    Prefab.ActiveATM.spawnedAtms.Remove(UniqueId);
                    Saving.Load.ModdedAtms.Remove(atm);
                    UnityEngine.Object.Destroy(atm);
                }
                else
                {
                    Misc.Msg($"[RemoveATM] [OnReceived()] Sign With UniqueId: {UniqueId} Does Not Exist, Cant Remove");
                }
            }
            else
            {
                Misc.Msg($"[RemoveATM] [OnReceived()] Sign With UniqueId: {UniqueId} Does Not Exist, Cant Remove");
            }

        }
    }
}
