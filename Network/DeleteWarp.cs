using SimpleNetworkEvents;
using UnityEngine;

namespace Warps.Network
{
    internal class DeleteWarp : SimpleEvent<DeleteWarp>
    {
        public string WarpName { get; set; }

        public override void OnReceived()
        {
            Misc.Msg("Recived Network Remove Warp Event");
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
            {
                Misc.Msg("[DeleteWarp] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                return;
            }

            if (Config.NetworkDebugIngameSign.Value) { Misc.Msg($"[DeleteWarp] [OnReceived()] Removing Warp From Network Event"); }

            if (string.IsNullOrEmpty(WarpName))
            {
                Misc.Msg($"[DeleteWarp] [OnReceived()] WarpName Is Null Or Empty");
                return;
            }

            if (Saving.LoadedWarps.loadedWarps.ContainsKey(WarpName))
            {
                Saving.LoadedWarps.loadedWarps.Remove(WarpName);
                Misc.Msg($"[DeleteWarp] [OnReceived()] Removed Warp With Name: {WarpName}");
                if (UI.Setup.IsUiOpen())
                {
                    UI.Setup.CloseUI();
                    UI.Setup.TryOpenUi();
                }
            }
            else
            {
                Misc.Msg($"[DeleteWarp] [OnReceived()] Warp With Name: {WarpName} Does Not Exist, Cant Remove");
            }

        }
    }
}
