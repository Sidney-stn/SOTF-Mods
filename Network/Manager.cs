

namespace Currency.Network
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
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.AddCash>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.RemoveCash>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.SyncAllCash>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.RequestSyncAllCash>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.SendSingleSyncCash>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.RemovePlayer>();
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
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.AddCash>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.RemoveCash>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.SyncAllCash>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.RequestSyncAllCash>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.SendSingleSyncCash>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.RemovePlayer>();
            }
        }

        public static void SendJoinedServerEvent()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Manager] [SendJoinedServerEvent] I Joined Server, Sending JoinedServerEvent");
                (ulong steamId, string stringSteamId) = Misc.MySteamId();
                int? cash = LiveData.LocalPlayerData.GetLocalPlayerCurrency();
                if (cash == null || cash < 0)
                {
                    Misc.Msg("[Manager] [SendJoinedServerEvent] Local player cash is null Or Less Than 0");
                    return;
                }
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.JoinedServer
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = stringSteamId,
                    Currency = (int)cash

                });
            }
        }

        public static void SendLeaveServerEvent()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Manager] [SendLeaveServerEvent] I Leaved Server, Sending LeavedServerEvent");
                (ulong steamId, string stringSteamId) = Misc.MySteamId();
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RemovePlayer
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = stringSteamId,
                });
            }
        }

    }
}
