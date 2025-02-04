
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
                //SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.JoinedServer>();
                //SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.SpawnSingeSign>();
                //SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.UpdateText>();
                //SimpleNetworkEvents.EventDispatcher.RegisterEvent<Network.RemoveSign>();
                SendJoinedServerEvent();
            }
        }
        public static void UnregisterEvents()
        {
            // Network Stuff
            if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.Multiplayer || Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("Unregisterd Events");
                //SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.JoinedServer>();
                //SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.SpawnSingeSign>();
                //SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.UpdateText>();
                //SimpleNetworkEvents.EventDispatcher.UnregisterEvent<Network.RemoveSign>();
            }
        }

        public static void SendJoinedServerEvent()
        {
            if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Manager] I Joined Server, Sending JoinedServerEvent");
                //SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.JoinedServer
                //{
                //    SenderName = Banking.API.GetLocalPlayerName(),
                //    SenderId = Banking.API.GetLocalPlayerId(),

                //});
            }
        }
    }
}
