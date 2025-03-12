

using Il2CppInterop.Runtime.Injection;

namespace StoneGate.Network
{
    public class Manager
    {
        private static bool isRegistered = false;
        // Register all network related classes
        internal static void Register()  // OnModInit
        {
            if (isRegistered) { return; }
            isRegistered = true;

            Network.Joining.StoneGateJoin.Register();

            // ConveyorSyncEvent, StoneGateSetter
            Network.StoneGateSyncEvent.Register();
            ClassInjector.RegisterTypeInIl2Cpp<Network.StoneGateSetter>();

        }

        internal static void RegisterEventHandlers()  // OnGameStart
        {
            CustomEventHandler.Create();
        }
    }
}
