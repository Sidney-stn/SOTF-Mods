
namespace Shops.Network
{
    internal class Manager
    {
        public static void RegisterEvents()
        {
            // Network Stuff
            if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.Multiplayer || Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("Registerd Events");
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.JoinedServer>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.SpawnShop>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.RemoveShop>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.Sync.RequestSpawnShop>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.Sync.SyncShop>();
                SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.Sync.SyncShopList>();
                SendJoinedServerEvent();
            }
        }
        public static void UnregisterEvents()
        {
            // Network Stuff
            if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.Multiplayer || Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("Unregisterd Events");
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.JoinedServer>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.SpawnShop>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.RemoveShop>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.Sync.RequestSpawnShop>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.Sync.SyncShop>();
                SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.Sync.SyncShopList>();
            }
        }

        public static void SendJoinedServerEvent()
        {
            if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Manager] I Joined Server, Sending JoinedServerEvent");
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.JoinedServer
                {
                    SenderName = Banking.API.GetLocalPlayerName(),
                    SenderId = Banking.API.GetLocalPlayerId(),
                });
            }
        }
    }
}
