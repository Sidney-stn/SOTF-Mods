
using Il2CppInterop.Runtime.Injection;
using WirelessSignals.Network.Reciver;

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

            ReciverSyncEvent.Register();
            ClassInjector.RegisterTypeInIl2Cpp<ReciverSetter>();

        }

        internal static void RegisterEventHandlers()  // OnGameStart
        {
            CustomEventHandler.Create();
        }
    }
}
