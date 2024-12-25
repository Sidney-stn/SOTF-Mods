using SimpleNetworkEvents;

namespace Banking.Network
{
    internal class RemoveCash : SimpleEvent<RemoveCash>
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public int Currency { get; set; }
        public string ToPlayerId { get; set; }

        public override void OnReceived()  // Only Host Should Be Able To Remove Cash And Then Sync It
        {
            if (Misc.hostMode != Misc.SimpleSaveGameType.Multiplayer) { return; }
            if (string.IsNullOrEmpty(ToPlayerId)) { Misc.Msg("[RemoveCash] PlayerId Invalid"); return; }
            if (string.IsNullOrEmpty(SenderId)) { Misc.Msg("[RemoveCash] SenderId Invalid"); return; }
            if (Currency <= 0) { Misc.Msg("[RemoveCash] Currency Invalid"); return; }
            if (ToPlayerId == Misc.MySteamId().Item2 || ToPlayerId == "None")
            {
                LiveData.Players.RemoveCashFromPlayer(LiveData.Players.GetCurrencyType.SteamID, SenderId, Currency);

                // Update Cash Over Network
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SyncCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    PlayerName = LiveData.Players.GetPlayers(),
                    PlayerCash = LiveData.Players.GetPlayersCurrency(),
                    ToPlayerId = "None"
                });
            }
        }
    }
}
