
using Il2CppInterop.Runtime.Injection;
using WirelessSignals.Network.Joining;
using WirelessSignals.Network.Reciver;
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

            JoiningEvent.Register();
            ListInitialSyncEvent.Register();
            UniqueIdSync.Register();
            RequestReciverSyncEvent.Register();  // Reciver

        }

        internal static void RegisterEventHandlers()  // OnGameStart
        {
            CustomEventHandler.Create();
        }
    }
}
