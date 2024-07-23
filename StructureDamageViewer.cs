using SonsSdk;
using SUI;
using TheForest.Utils;
using UnityEngine;

namespace StructureDamageViewer;

public class StructureDamageViewer : SonsMod
{
    public StructureDamageViewer()
    {

        // Uncomment any of these if you need a method to run on a specific update loop.
        //OnUpdateCallback = OnUpdate;
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
        StructureDamageViewerUi.Create();

        // Adding Ingame CFG
        SettingsRegistry.CreateSettings(this, null, typeof(Config));
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.
        Misc.localPlayerTrackMono = LocalPlayer.GameObject.AddComponent<LocalPlayerTrackMono>();
        SonsSdk.SdkEvents.OnInWorldUpdate.Subscribe(Misc.CheckHostModeOnWorldUpdate);
        Misc.OnHostModeGotten += Misc.OnHostModeGottenCorrectly;

    }

    internal static void OnLeaveWorld()
    {
        Misc.Msg("OnLeaveWorld");
        Misc.OnHostModeGotten -= Misc.OnHostModeGottenCorrectly;
        Misc.Msg($"List DamageMonos Count: {Misc.damageMonos.Count}");

        foreach (DamageMono mono in Misc.damageMonos)
        {
            Misc.Msg("[OnLeaveWorld] Foreach: DamageMono mono in Misc.damageMonos");
            mono.StopCorutineCustom();
            Misc.Msg("Successfully stopped Coro");
            GameObject.Destroy(mono);
            Misc.Msg("[OnLeaveWorld] Destroyed DamageMono");
        }

        if (Misc.localPlayerTrackMono != null)
        {
            Misc.localPlayerTrackMono.StopCorutineCustom();
            GameObject.Destroy(Misc.localPlayerTrackMono);
            Misc.Msg("Destroyed LocalPlayerTrackMono");
            Misc.localPlayerTrackMono = null;
        }
        else
        {
            Misc.Msg("localPlayerTrackMono is null, nothing to stop or destroy.");
        }

        Misc.damageMonos.Clear();
        Misc.dialogManager.QuitGameConfirmDialog.remove_OnOption1Clicked((Il2CppSystem.Action)StructureDamageViewer.OnLeaveWorld);
    }

}