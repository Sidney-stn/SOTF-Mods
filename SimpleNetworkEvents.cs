using Sons.Gui;
using SonsSdk;
using UnityEngine;

namespace SimpleNetworkEvents;

public class SimpleNetworkEvents : SonsMod
{
    public SimpleNetworkEvents()
    {

        // Uncomment any of these if you need a method to run on a specific update loop.
        //OnUpdateCallback = MyUpdateMethod;
        //OnLateUpdateCallback = MyLateUpdateMethod;
        //OnFixedUpdateCallback = MyFixedUpdateMethod;
        //OnGUICallback = MyGUIMethod;

        // Uncomment this to automatically apply harmony patches in your assembly.
        //HarmonyPatchAll = true;
    }

    protected override void OnInitializeMod()
    {
        // Do your early mod initialization which doesn't involve game or sdk references here
        Config.Init();
    }

    protected override void OnSdkInitialized()
    {
        // Do your mod initialization which involves game or sdk references here
        // This is for stuff like UI creation, event registration etc.
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.

        // Adding Quit Event
        Misc.Msg("[OnGameStart] Added Custom Quit Event");
        PauseMenu.add_OnQuitEvent((Il2CppSystem.Action)Quitting);

        // Adding CustomGlobalEventListener to Component of GameObject
        customGlobalEventListener = new GameObject("CustomGlobalEventListenerGameObject");
        customGlobalEventListener.AddComponent<CustomGlobalEventListener>();
    }

    private void Quitting()
    {
        // Removing CustomGlobalEventListener from GameObject
        CustomGlobalEventListener component = customGlobalEventListener.GetComponent<CustomGlobalEventListener>();
        component.RemoveGlobalEventListener();
        component.CleanUpAndDestoy();
        UnityEngine.Object.Destroy(customGlobalEventListener);

        // Removig Quit Event
        PauseMenu.remove_OnQuitEvent((Il2CppSystem.Action)Quitting);
    }

    internal static GameObject customGlobalEventListener;
}