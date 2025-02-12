using SonsSdk;
using SonsSdk.Attributes;
using static TheForest.Player.Actions.GatheringPrefabsDefinition;
using TheForest.Utils;
using UnityEngine.UI;
using UnityEngine;
using WirelessSignals.Prefab;

namespace WirelessSignals;

public class WirelessSignals : SonsMod
{
    public WirelessSignals()
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
        WirelessSignalsUi.Create();

        // Add in-game settings ui for your mod.
        // SettingsRegistry.CreateSettings(this, null, typeof(Config));
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.

    }

    internal static void OnLeaveWorld()
    {
        // This is called when the player leaves the world.
        Misc.OnHostModeGotten -= Misc.OnHostModeGottenCorrectly;
        Misc.dialogManager.QuitGameConfirmDialog.remove_OnOption1Clicked((Il2CppSystem.Action)OnLeaveWorld);
    }

    [DebugCommand("wireless")]
    private void WirelessCmd(string args)
    {
        Misc.Msg("[WirelessCmd] Command");
        Transform transform = LocalPlayer._instance._mainCam.transform;
        RaycastHit raycastHit;
        Physics.Raycast(transform.position, transform.forward, out raycastHit, 25f, LayerMask.GetMask(new string[]
        {
            "Terrain",
            "Default"
        }));
        // Check If Raycast Hit Something
        if (raycastHit.collider == null)
        {
            Misc.Msg("[WirelessCmd] Raycast Hit Nothing");
            SonsTools.ShowMessage("Raycast Hit Nothing", 5);
            return;
        }
        GameObject.Instantiate(Assets.TransmitterSwitch, raycastHit.point + Vector3.up * 0.1f, LocalPlayer.Transform.rotation);
    }
}