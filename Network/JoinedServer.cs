using SimpleNetworkEvents;

namespace Currency.Network
{
    internal class JoinedServer : SimpleEvent<JoinedServer>
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public int Currency { get; set; }
        public override void OnReceived()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer)
            {
                Misc.Msg($"[JoinedServer] Player Joined: {SenderId} - {SenderName}");
                LiveData.Players.AddPlayer(SenderId, SenderName, Currency);
                Misc.Msg($"[JoinedServer] Added Player: {SenderId} - {SenderName}");
            }
            else if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg($"[JoinedServer] Player Joined: {SenderId} - {SenderName}");
                LiveData.Players.AddPlayer(SenderId, SenderName, Currency);
                Misc.Msg($"[JoinedServer] Added Player: {SenderId} - {SenderName}");
            }
        }
    }
}
