using Bolt;
using RedLoader;
using SonsSdk;
using UnityEngine;
using WirelessSignals.Network.Joining;
using Color = System.Drawing.Color;

namespace WirelessSignals;


/// Global event handler to catch some events
/// Here we use it to catch if a player connects

[RegisterTypeInIl2Cpp]
public class CustomEventHandler : GlobalEventListener
{
    public static CustomEventHandler Instance;

    public static void Create()
    {
        if (Instance)
            return;

        Instance = new GameObject("CustomEventHandler").AddComponent<CustomEventHandler>();
    }

    public override void Connected(BoltConnection connection)
    {
        Misc.Msg("[CustomEventHandler] [Connected] Player connected", true);
        //JoiningEvent.Instance.RequestInfo(connection);

        // Send State of OwnerToEdit to the new player
        if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer)
        {
            JoiningEvent.Instance.SendInfo(connection);
            // Set UniqueId Lists Need To Do This Later

        }
    }

    public void OnEnterWorld()
    {
        Misc.Msg("[CustomEventHandler] [OnEnterWorld] Player entered world", true);
        if (BoltNetwork.isRunning && BoltNetwork.isClient)
        {
            Misc.Msg("[CustomEventHandler] [OnEnterWorld] Sending Request ListInitialSyncEvent", true);
            ListInitialSyncEvent.Instance.RequestInfoFromServer();
        }
    }
}