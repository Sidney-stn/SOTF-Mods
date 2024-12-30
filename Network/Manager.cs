
namespace Banking.Network
{
    internal class Manager
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
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.SyncCash>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.RequestSyncCash>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.RemoveATM>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.SpawnATM>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.ATMPlacer.SpawnATMPlacer>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.ATMPlacer.RemoveATMPlacer>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.ATMPlacer.UpdateATMPlacer>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.ATMPlacer.RequestUpdateATMPlacer>();
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
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.SyncCash>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.RequestSyncCash>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.RemoveATM>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.SpawnATM>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.ATMPlacer.SpawnATMPlacer>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.ATMPlacer.RemoveATMPlacer>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.ATMPlacer.UpdateATMPlacer>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.ATMPlacer.RequestUpdateATMPlacer>();
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

                });
            }
        }
    }
}
