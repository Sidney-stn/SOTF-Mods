using Bolt;
using RedLoader;
using SonsSdk;
using System.Collections;
using UnityEngine;
using Color = System.Drawing.Color;

namespace Signs.Network;


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
        if (connection == null)
        {
            Misc.Msg("[CustomEventHandler] [Connected] Connection is null", true);
            return;
        }

        if (BoltNetwork.isServer == false)
        {
            Misc.Msg("[CustomEventHandler] [Connected] Not a server, skipping request", true);
            return;
        }

        Misc.Msg("[CustomEventHandler] [Connected] Player connected, sending info request", true);
        Network.Join.ReciveFromPlayerOnJoinEvent.Instance.RequestInfo(connection);

    }

    public void OnEnterWorld()
    {
        Misc.Msg("[CustomEventHandler] [OnEnterWorld] Player entered world", true);

        if (!BoltNetwork.isRunning)
        {
            Misc.Msg("[CustomEventHandler] [OnEnterWorld] BoltNetwork is not running", true);
            return;
        }

        if (!BoltNetwork.isClient)
        {
            Misc.Msg("[CustomEventHandler] [OnEnterWorld] Not a client, skipping request", true);
            return;
        }

        // Delay the request slightly to ensure everything is initialized
        DelayedRequestInfo().RunCoro();
    }

    private IEnumerator DelayedRequestInfo()
    {
        // Wait a bit to make sure everything is initialized
        yield return new WaitForSeconds(8f);

        try
        {
            if (Network.Join.SyncSignsOnJoin.Instance != null)
            {
                Misc.Msg("[CustomEventHandler] [OnEnterWorld] Sending Request SyncSignsOnJoin", true);
                Network.Join.SyncSignsOnJoin.Instance.RequestInfoFromServer();
            }
            else
            {
                Misc.Msg("[CustomEventHandler] [OnEnterWorld] SyncSignsOnJoin.Instance is null", true);
            }
        }
        catch (System.Exception ex)
        {
            Misc.Msg($"[CustomEventHandler] [OnEnterWorld] Error requesting info: {ex.Message}", true);
        }
    }
}