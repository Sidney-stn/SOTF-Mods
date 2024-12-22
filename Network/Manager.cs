

namespace Signs.Network
{
    public class Manager
    {
        public static void RegisterEvents()
        {
            // Network Stuff
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("Registerd Events");
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.JoinedServer>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.SpawnSingeSign>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.UpdateText>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.RemoveSign>();
                SendJoinedServerEvent();
            }
        }
        public static void UnregisterEvents()
        {
            // Network Stuff
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("Unregisterd Events");
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.JoinedServer>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.SpawnSingeSign>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.UpdateText>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.RemoveSign>();
            }
        }

        public static void SendJoinedServerEvent()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Manager] I Joined Server, Sending JoinedServerEvent");
                (ulong steamId, string stringSteamId) = Misc.MySteamId();
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.JoinedServer
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = stringSteamId,

                });
            }
        }
    }
}
