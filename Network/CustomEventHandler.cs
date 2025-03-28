using Bolt;
using RedLoader;
using SonsSdk;
using System.Collections;
using UnityEngine;
using Color = System.Drawing.Color;

namespace SimpleElevator;


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

        Misc.Msg("[CustomEventHandler] [Connected] Player connected", true);

        // Send State of OwnerToEdit to the new player
        if (BoltNetwork.isRunning && BoltNetwork.isServer)
        {
            try
            {
                //if (JoiningEvent.Instance != null)
                //{
                //    JoiningEvent.Instance.SendInfo(connection);
                //    Misc.Msg("[CustomEventHandler] [Connected] Sent owner to edit info", true);
                //}
                //else
                //{
                //    Misc.Msg("[CustomEventHandler] [Connected] JoiningEvent.Instance is null", true);
                //}
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[CustomEventHandler] [Connected] Error sending joining info: {ex.Message}", true);
            }
        }
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
        yield return new WaitForSeconds(2f);

        try
        {
            //if (ListInitialSyncEvent.Instance != null)
            //{
            //    Misc.Msg("[CustomEventHandler] [OnEnterWorld] Sending Request ListInitialSyncEvent", true);
            //    ListInitialSyncEvent.Instance.RequestInfoFromServer();
            //}
            //else
            //{
            //    Misc.Msg("[CustomEventHandler] [OnEnterWorld] ListInitialSyncEvent.Instance is null", true);
            //}
        }
        catch (System.Exception ex)
        {
            Misc.Msg($"[CustomEventHandler] [OnEnterWorld] Error requesting info: {ex.Message}", true);
        }
    }
}