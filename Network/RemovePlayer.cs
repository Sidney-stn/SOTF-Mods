using SimpleNetworkEvents;

namespace Currency.Network
{
    internal class RemovePlayer : SimpleEvent<RemovePlayer>
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public override void OnReceived()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer)
            {
                Misc.Msg($"[RemovePlayer] Player Left: {SenderId} - {SenderName}");
                LiveData.Players.RemovePlayer(SenderId);
                Misc.Msg($"[RemovePlayer] Removed Player: {SenderId} - {SenderName}");
            }
            else if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg($"[RemovePlayer] Player Left: {SenderId} - {SenderName}");
                LiveData.Players.RemovePlayer(SenderId);
                Misc.Msg($"[RemovePlayer] Removed Player: {SenderId} - {SenderName}");
            }
        }
    }
}
