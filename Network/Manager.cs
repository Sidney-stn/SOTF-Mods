

using Il2CppInterop.Runtime.Injection;

namespace Signs.Network
{
    public class Manager
    {
        private static bool isRegistered = false;
        // Register all network related classes
        internal static void Register()  // OnModInit
        {
            if (isRegistered) { return; }
            isRegistered = true;

            Network.Join.ReciveFromPlayerOnJoinEvent.Register();
            Network.Join.SyncSignsOnJoin.Register();

            //// SignSyncEvent
            Network.SignSyncEvent.Register();
            ClassInjector.RegisterTypeInIl2Cpp<Network.SignSetter>();

            ClassInjector.RegisterTypeInIl2Cpp<Mono.SignController>();
            ClassInjector.RegisterTypeInIl2Cpp<Mono.DestroyOnC>();

            if (Tools.DedicatedServer.IsDeticatedServer())
            {
            }

        }

        internal static void RegisterEventHandlers()  // OnGameStart
        {
            CustomEventHandler.Create();
        }
    }
}
