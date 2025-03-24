

using Il2CppInterop.Runtime.Injection;

namespace BuildingMagnet.Network
{
    public class Manager
    {
        private static bool isRegistered = false;
        // Register all network related classes
        internal static void Register()  // OnModInit
        {
            if (isRegistered) { return; }
            isRegistered = true;

            Network.ClientEvents.Register();

        }
    }
}
