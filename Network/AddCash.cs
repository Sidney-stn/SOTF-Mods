using SimpleNetworkEvents;

namespace Currency.Network
{
    internal class AddCash : SimpleEvent<AddCash>
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public int Currency { get; set; }
        public string ToPlayerId { get; set; }

        public override void OnReceived()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { return; }
            if (string.IsNullOrEmpty(ToPlayerId)) { Misc.Msg("[AddCash] PlayerId Invalid"); return; }
            if (string.IsNullOrEmpty(SenderId)) { Misc.Msg("[AddCash] SenderId Invalid"); return; }
            if (Currency <= 0) { Misc.Msg("[AddCash] Currency Invalid"); return; }
            if (ToPlayerId == Misc.MySteamId().Item2 || ToPlayerId == "None")
            {
                // Add cash to local player
                LiveData.LocalPlayerData.AddCashToLocalPlayer(Currency);

                // Update MyCash Over Network
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SendSingleSyncCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    Currency = LiveData.LocalPlayerData.GetLocalPlayerCurrency() ?? 0,
                    ToPlayerId = "None"
                });
            }
        }
    }
}
