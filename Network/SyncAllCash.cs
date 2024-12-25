using Currency.LiveData;
using SimpleNetworkEvents;

namespace Currency.Network
{
    internal class SyncAllCash : SimpleEvent<SyncAllCash>
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public Dictionary<string, string> Players { get; set; }
        public Dictionary<string, int> PlayersCurrency { get; set; }
        public string ToPlayerId { get; set; }

        public override void OnReceived()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { return; }
            if (string.IsNullOrEmpty(ToPlayerId)) { Misc.Msg("[SyncAllCash] PlayerId Invalid"); return; }
            if (string.IsNullOrEmpty(SenderId)) { Misc.Msg("[SyncAllCash] SenderId Invalid"); return; }
            if (ToPlayerId == Misc.MySteamId().Item2 || ToPlayerId == "None")
            {
                foreach (var player in Players)
                {
                    int? cash = LiveData.Players.GetPlayerCurrency(LiveData.Players.GetCurrencyType.SteamID, player.Key);
                    if (cash.HasValue && cash != null)
                    {
                        LiveData.Players.AddPlayer(player.Key, player.Value, (int)cash);
                    }
                }
            }
        }
    }

    internal class RequestSyncAllCash : SimpleEvent<RequestSyncAllCash>
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public string ToPlayerId { get; set; }

        public override void OnReceived()
        {
            // For Host Syncing
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer)
            {
                int? cash = LocalPlayerData.GetLocalPlayerCurrency();
                if (cash == null)
                {
                    Misc.Msg("[RequestSyncAllCash] Local player cash is null");
                    return;
                }
                Misc.Msg("[RequestSyncAllCash] Syncing Local Player Cash");
                LiveData.Players.AddPlayer(Misc.MySteamId().Item2, Misc.GetLocalPlayerUsername(), (int)cash);
            }

            // For Client Syncing
            if (Misc.hostMode != Misc.SimpleSaveGameType.MultiplayerClient) { return; }
            if (string.IsNullOrEmpty(ToPlayerId)) { Misc.Msg("[SyncAllCash] PlayerId Invalid"); return; }
            if (string.IsNullOrEmpty(SenderId)) { Misc.Msg("[SyncAllCash] SenderId Invalid"); return; }
            if (ToPlayerId == Misc.MySteamId().Item2 || ToPlayerId == "None")
            {
                Misc.Msg("[RequestSyncAllCash] Sending Single Sync Cash");
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

    internal class SendSingleSyncCash : SimpleEvent<SendSingleSyncCash>
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public int Currency { get; set; }
        public string ToPlayerId { get; set; }

        public override void OnReceived()
        {
            if (Misc.hostMode != Misc.SimpleSaveGameType.Multiplayer) { return; }
            if (string.IsNullOrEmpty(ToPlayerId)) { Misc.Msg("[SyncAllCash] PlayerId Invalid"); return; }
            if (string.IsNullOrEmpty(SenderId)) { Misc.Msg("[SyncAllCash] SenderId Invalid"); return; }
            if (ToPlayerId == Misc.MySteamId().Item2 || ToPlayerId == "None")
            {
                LiveData.Players.AddPlayer(SenderId, SenderName, Currency);
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SyncAllCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    Players = LiveData.Players.GetPlayers(),
                    PlayersCurrency = LiveData.Players.GetPlayersCurrency(),
                    ToPlayerId = "None"
                });
            }
        }
    }
}
