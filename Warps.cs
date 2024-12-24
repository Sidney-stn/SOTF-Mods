using SonsSdk;
using static TheForest.Player.Actions.GatheringPrefabsDefinition;
using UnityEngine;
using SUI;

namespace Warps;

public class Warps : SonsMod
{
    public Warps()
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
        WarpsUi.Create();

        // Adding Ingame CFG
        SettingsRegistry.CreateSettings(this, null, typeof(Config));

        // Registering Save System
        var manager = new Saving.Manager(); // Signs
        SonsSaveTools.Register(manager);

        UI.Setup.SetupUi();
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.
        // Adding Quit Event / Get HostMode Events
        SonsSdk.SdkEvents.OnInWorldUpdate.Subscribe(Misc.CheckHostModeOnWorldUpdate);
        Misc.OnHostModeGotten += Misc.OnHostModeGottenCorrectly;


        Integrations.HotKeyCommandsIntegration.Setup();
    }

    internal static void OnLeaveWorld()
    {
        // This is called when the player leaves the world.
        Misc.Msg("OnLeaveWorld");
        Misc.OnHostModeGotten -= Misc.OnHostModeGottenCorrectly;
        Misc.dialogManager.QuitGameConfirmDialog.remove_OnOption1Clicked((Il2CppSystem.Action)OnLeaveWorld);
    }
}