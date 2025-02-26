using Bolt;
using RedLoader;
using UnityEngine;
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
        
        Instance = new GameObject("EventTestEventHandler").AddComponent<CustomEventHandler>();
    }
    
    public override void Connected(BoltConnection connection)
    {
        Misc.Msg("[CustomEventHandler] [Connected] Player connected", true);
        //JoiningEvent.Instance.RequestInfo(connection);
    }
}