using Sons.Gui;
using SonsSdk;
using SUI;
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
        // Adding Ingame CFG
        SettingsRegistry.CreateSettings(this, null, typeof(Config));
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.

        // Adding Quit Event
        SonsSdk.SdkEvents.OnInWorldUpdate.Subscribe(Misc.CheckHostModeOnWorldUpdate);
        Misc.OnHostModeGotten += Misc.OnHostModeGottenCorrectly;

        // Adding CustomGlobalEventListener to Component of GameObject
        customGlobalEventListener = new GameObject("CustomGlobalEventListenerGameObject");
        customGlobalEventListener.AddComponent<CustomGlobalEventListener>();
    }

    internal static void OnLeaveWorld()
    {
        Misc.Msg("OnLeaveWorld");
        Misc.OnHostModeGotten -= Misc.OnHostModeGottenCorrectly;
        Misc.dialogManager.QuitGameConfirmDialog.remove_OnOption1Clicked((Il2CppSystem.Action)OnLeaveWorld);

        // Removing CustomGlobalEventListener from GameObject
        CustomGlobalEventListener component = customGlobalEventListener.GetComponent<CustomGlobalEventListener>();
        component.RemoveGlobalEventListener();
        component.CleanUpAndDestoy();
        UnityEngine.Object.Destroy(customGlobalEventListener);
    }

    internal static GameObject customGlobalEventListener;
}