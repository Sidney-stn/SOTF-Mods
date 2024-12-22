using SimpleNetworkEvents;
using UnityEngine;


namespace Signs.Network
{
    internal class UpdateText : SimpleEvent<UpdateText>
    {
        public string UniqueId { get; set; }
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string Line1Text { get; set; }
        public string Line2Text { get; set; }
        public string Line3Text { get; set; }
        public string Line4Text { get; set; }
        public string ToSteamId { get; set; }

        public override void OnReceived()
        {
            Misc.Msg("Recived Network Update Text On Sign Event");
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
            {
                Misc.Msg("[UpdateTextSign] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                return;
            }
            if (Assets.SignObj == null)
            {
                Misc.Msg("[UpdateTextSign] [OnReceived()] Sign Prefab has not been created yet, skipped");
                return;
            }
            (ulong uSteamId, string sSteamId) = Misc.MySteamId();
            if (string.IsNullOrEmpty(sSteamId))
            {
                Misc.Msg("[UpdateTextSign] [OnReceived()] Could Not Get My SteamId, Cant Be Sure Who Has The Sign, Skipped");
                return;
            }
            else
            {
                if (Sender == sSteamId)
                {
                    Misc.Msg("[UpdateTextSign] [OnReceived()] Not Updating Text Sign Over Network When Its From My SteamID, skipped");
                    return;
                }
            }
            if (ToSteamId != null && ToSteamId != "None")
            {
                if (ToSteamId != sSteamId)
                {
                    Misc.Msg("[UpdateTextSign] [OnReceived()] Not Updating Text Sign Over Network When Its Not For Me, skipped");
                    return;
                }

            }

            if (Config.NetworkDebugIngameSign.Value) { Misc.Msg($"[UpdateTextSign] [OnReceived()] Updating Text Over Network Event"); }

            ulong resultSteamID;
            if (!ulong.TryParse(Sender, out resultSteamID)) { Misc.Msg($"Failed To Parse SenderId: {Sender} To String"); return; }

            // Update the text on the sign
            if (Prefab.SignPrefab.DoesShopWithUniqueIdExist(UniqueId))
            {
                Mono.SignController signController = Prefab.SignPrefab.FindShopByUniqueId(UniqueId).GetComponent<Mono.SignController>();
                if (signController != null) {
                    if (Line1Text != null) signController.SetLineText(1, Line1Text);
                    if (Line2Text != null) signController.SetLineText(2, Line2Text);
                    if (Line3Text != null) signController.SetLineText(3, Line3Text);
                    if (Line4Text != null) signController.SetLineText(4, Line4Text);
                }
            }

        }
    }
}
