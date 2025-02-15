using SonsSdk;
using SonsSdk.Attributes;
using static TheForest.Player.Actions.GatheringPrefabsDefinition;
using TheForest.Utils;
using UnityEngine.UI;
using UnityEngine;
using WirelessSignals.Prefab;
using SUI;

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

        // Adding Ingame CFG
        SettingsRegistry.CreateSettings(this, null, typeof(Config));
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.

        // Adding Quit Event / Get HostMode Events
        SonsSdk.SdkEvents.OnInWorldUpdate.Subscribe(Misc.CheckHostModeOnWorldUpdate);
        Misc.OnHostModeGotten += Misc.OnHostModeGottenCorrectly;

    }

    internal static void OnLeaveWorld()
    {
        // This is called when the player leaves the world.
        Misc.OnHostModeGotten -= Misc.OnHostModeGottenCorrectly;
        Misc.dialogManager.QuitGameConfirmDialog.remove_OnOption1Clicked((Il2CppSystem.Action)OnLeaveWorld);
    }

    internal static void OnEnterWorld()
    {
        Misc.Msg("OnEnterWorld");
        Misc.Msg("[OnEnterWorld] Creating WirelessTransmitterSwitch");
        transmitterSwitch = new WirelessTransmitterSwitch();
        transmitterSwitch.Setup();
        Misc.Msg("[OnEnterWorld] Complete - Creating WirelessTransmitterSwitch");
        Misc.Msg("[OnEnterWorld] Creating Reciver");
        reciver = new Reciver();
        reciver.Setup();
        Misc.Msg("[OnEnterWorld] Complete - Creating Reciver");

    }

    internal static WirelessTransmitterSwitch transmitterSwitch;
    internal static Reciver reciver;


    [DebugCommand("wireless")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
    private void WirelessCmd(string args)
    {
        Misc.Msg("[WirelessCmd] Command");
        Transform transform = LocalPlayer._instance._mainCam.transform;
        RaycastHit raycastHit;
        Physics.Raycast(transform.position, transform.forward, out raycastHit, 25f, LayerMask.GetMask(new string[]
        {
                "Terrain",
                "Default",
                "Prop"
        }));
        // Check If Raycast Hit Something
        if (raycastHit.collider == null)
        {
            Misc.Msg("[WirelessCmd] Raycast Hit Nothing");
            SonsTools.ShowMessage("Raycast Hit Nothing", 5);
            return;
        }
        switch (args)
        {
            case "spawn1":
                //GameObject.Instantiate(Assets.TransmitterSwitch, raycastHit.point + Vector3.up * 0.1f, LocalPlayer.Transform.rotation);
                Misc.Msg("[WirelessCmd] Spawning - WirelessTransmitterSwitch");
                var parameters = new Prefab.TransmitterSwitchSpawnParameters
                {
                    position = raycastHit.point + Vector3.up * 0.1f,
                    rotation = LocalPlayer.Transform.rotation,
                    uniqueId = null,
                    isOn = false
                };
                Misc.Msg("[WirelessCmd] Spawning - WirelessTransmitterSwitch Parameters Created");
                transmitterSwitch.Spawn(parameters);
                Misc.Msg("[WirelessCmd] Complete - Spawning WirelessTransmitterSwitch");
                break;
            case "spawn2":
                Misc.Msg("[WirelessCmd] Spawning - WirelessTransmitterSwitch");
                var reciverParameters = new Prefab.ReciverSpawnParameters
                {
                    position = raycastHit.point + Vector3.up * 0.1f,
                    rotation = LocalPlayer.Transform.rotation,
                    uniqueId = null,
                    isOn = false
                };
                Misc.Msg("[WirelessCmd] Spawning - Reciver Parameters Created");
                reciver.Spawn(reciverParameters);
                Misc.Msg("[WirelessCmd] Complete - Spawning Reciver");
                break;
            case "rml":
                Misc.Msg("[WirelessCmd] Removing RayCast Lines");
                Debug.RayCast.RemoveLines();
                break;
        }
        
    }
}