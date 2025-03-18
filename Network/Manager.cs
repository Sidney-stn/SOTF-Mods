

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

            Network.ClientEvents.Register();
            Network.HostEvents.Register();

        }

        internal static void RegisterEventHandlers()  // OnGameStart
        {
            CustomEventHandler.Create();
        }
    }
}
