using SimpleNetworkEvents;

namespace Warps.Network
{
    internal class AddWarp : SimpleEvent<AddWarp>
    {
        public string WarpName { get; set; }
        public string Vector3String { get; set; }
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string ToSteamId { get; set; }

        public override void OnReceived()
        {
            Misc.Msg("Recived Network Add Warp Event");
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
            {
                Misc.Msg("[AddWarp] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                return;
            }

            if (Sender == Misc.MySteamId().Item2)
            {
                Misc.Msg("[AddWarp] [OnReceived()] Got Add Warp Event From MySelf, Skipped");
                return;
            }

            if (!string.IsNullOrEmpty(ToSteamId) && ToSteamId != "None")
            {
                if (ToSteamId != Misc.MySteamId().Item2)
                {
                    Misc.Msg("[AddWarp] [OnReceived()] Got Add Warp Event From Another Player, But Not For Me, Skipped");
                    return;
                }
            }

            if (Config.NetworkDebugIngameSign.Value) { Misc.Msg($"[AddWarp] [OnReceived()] Adding Warp From Network Event"); }

            if (string.IsNullOrEmpty(WarpName) || string.IsNullOrEmpty(Vector3String))
            {
                Misc.Msg($"[AddWarp] [OnReceived()] WarpName Or Vector3String Is Null Or Empty");
                return;
            }

            if (!Saving.LoadedWarps.loadedWarps.ContainsKey(WarpName))
            {
                Saving.LoadedWarps.loadedWarps.Add(WarpName, CustomSerializable.Vector3FromString(Vector3String));
                Misc.Msg($"[AddWarp] [OnReceived()] Removed Warp With Name: {WarpName}");
                if (UI.Setup.IsUiOpen())
                {
                    UI.Setup.CloseUI();
                    UI.Setup.TryOpenUi();
                }
            }
            else
            {
                Misc.Msg($"[AddWarp] [OnReceived()] Warp With Name: {WarpName} Does Exist, Cant Add");
            }

        }
    }
}
