
using Il2CppInterop.Runtime.Injection;
using WirelessSignals.Network.Joining;
using WirelessSignals.Network.Reciver;
using WirelessSignals.Network.Sync;
using WirelessSignals.Network.SyncLists;

namespace WirelessSignals.Network
{
    internal class Manager
    {
        private static bool isRegistered = false;
        // Register all network related classes
        internal static void Register()  // OnModInit
        {
            if (isRegistered) { return; }
            isRegistered = true;

            // Reciver
            ReciverSyncEvent.Register();
            ClassInjector.RegisterTypeInIl2Cpp<ReciverSetter>();

            // NetworkOwner
            NetworkOwnerSyncEvent.Register();
            ClassInjector.RegisterTypeInIl2Cpp<NetworkOwnerSetter>();

            JoiningEvent.Register();
            ListInitialSyncEvent.Register();
            UniqueIdSync.Register();

        }

        internal static void RegisterEventHandlers()  // OnGameStart
        {
            CustomEventHandler.Create();
        }
    }
}
