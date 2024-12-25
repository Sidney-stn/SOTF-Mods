using SimpleNetworkEvents;

namespace Banking.Network
{
    internal class SyncCash : SimpleEvent<SyncCash>  // Everyone Recives This Event
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public Dictionary<string, string> PlayerName { get; set; } // PlayerSteamID, PlayerName
        public Dictionary<string, int> PlayerCash { get; set; } // PlayerSteamID, PlayerCash
        public string ToPlayerId { get; set; }

        public override void OnReceived()
        {
            if (Misc.hostMode != Misc.SimpleSaveGameType.Multiplayer && Misc.hostMode != Misc.SimpleSaveGameType.MultiplayerClient) { return; }
            if (string.IsNullOrEmpty(ToPlayerId)) { Misc.Msg("[AddCash] PlayerId Invalid"); return; }
            if (string.IsNullOrEmpty(SenderId)) { Misc.Msg("[AddCash] SenderId Invalid"); return; }
            if (ToPlayerId == Misc.MySteamId().Item2 || ToPlayerId == "None")
            {
                Misc.Msg($"[SyncCash] Recived Updated Cash And Player Values");
                
                LiveData.Players.UpdatePlayersAndCash(PlayerName, PlayerCash);

                UI.Setup.UpdateUiIfOpen();
            }
        }
    }

    internal class RequestSyncCash : SimpleEvent<RequestSyncCash>  // Can be sent from client to host to request a sync
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public string RepsondToId { get; set; }

        public override void OnReceived()
        {
            if (Misc.hostMode != Misc.SimpleSaveGameType.Multiplayer) { return; }
            if (string.IsNullOrEmpty(RepsondToId)) { Misc.Msg("[AddCash] PlayerId Invalid"); return; }
            if (string.IsNullOrEmpty(SenderId)) { Misc.Msg("[AddCash] SenderId Invalid"); return; }
            if (RepsondToId == "None")
            {
                Misc.Msg($"[SyncCash] Recived Sync Request");
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SyncCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    PlayerName = LiveData.Players.GetPlayers(),
                    PlayerCash = LiveData.Players.GetPlayersCurrency(),
                    ToPlayerId = "None"
                });

            }
            else
            {
                Misc.Msg($"[SyncCash] Recived Sync Request");
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SyncCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    PlayerName = LiveData.Players.GetPlayers(),
                    PlayerCash = LiveData.Players.GetPlayersCurrency(),
                    ToPlayerId = RepsondToId
                });
            }
        }
    }
}
