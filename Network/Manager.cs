

using Il2CppInterop.Runtime.Injection;

namespace SimpleElevator.Network
{
    public class Manager
    {
        private static bool isRegistered = false;
        // Register all network related classes
        internal static void Register()  // OnModInit
        {
            if (isRegistered) { return; }
            isRegistered = true;


            ElevatorSyncEvent.Register();
            ElevatorControlPanelSyncEvent.Register();
            
            Network.ServerTools.ServerEvents.Register();

        }

        internal static void RegisterEventHandlers()  // OnGameStart
        {
            //CustomEventHandler.Create();
        }
    }
}
