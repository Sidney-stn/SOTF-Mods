using SonsSdk;
using SonsSdk.Attributes;
using TheForest.Utils;
using UnityEngine.UI;
using UnityEngine;
using WirelessSignals.Prefab;
using SUI;
using TheForest.Items.Inventory;
using WirelessSignals.Linking;
using WirelessSignals.Saving;
using WirelessSignals.Debug;

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
        HarmonyPatchAll = true;
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

        // Setup Structures
        reciverStructure = new Structure.Reciver();  // Reciver Structure
        reciverStructure.Setup();

        transmitterSwitchStructure = new Structure.TransmitterSwitch();  // TransmitterSwitch Structure
        transmitterSwitchStructure.Setup();

        transmitterDetectorStructure = new Structure.TransmitterDetector();  // TransmitterDetector Structure
        transmitterDetectorStructure.Setup();

        // Create and register the save manager
        prefabSaveManager = new PrefabSaveManager();

        // Register with SonsSaveTools
        SonsSaveTools.Register(prefabSaveManager);
        Misc.Msg("[OnSdkInitialized] Complete - Creating PrefabSaveManager");
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

        RepairToolInHand.Deinitialize(LocalPlayer.Inventory);
    }

    internal static void OnEnterWorld()
    {
        Misc.Msg("OnEnterWorld");
        Misc.Msg("[OnEnterWorld] Creating WirelessTransmitterSwitch");
        transmitterSwitch = new WirelessTransmitterSwitch();
        transmitterSwitch.Setup();
        prefabSaveManager.RegisterPrefabManager("TransmitterSwitch", transmitterSwitch);
        Misc.Msg("[OnEnterWorld] Complete - Creating WirelessTransmitterSwitch");
        Misc.Msg("[OnEnterWorld] Creating Reciver");
        reciver = new Reciver();
        reciver.Setup();
        prefabSaveManager.RegisterPrefabManager("Receiver", reciver);
        Misc.Msg("[OnEnterWorld] Complete - Creating Reciver");
        Misc.Msg("[OnEnterWorld] Creating TransmitterDetector");
        transmitterDetector = new TransmitterDetector();
        transmitterDetector.Setup();
        prefabSaveManager.RegisterPrefabManager("Detector", transmitterDetector);
        Misc.Msg("[OnEnterWorld] Complete - Creating TransmitterDetector");


        // Subscribe To UnityAction Listeners OnEqipped And OnUnEqupped
        PlayerInventory playerInventory = LocalPlayer.Inventory;
        RepairToolInHand.Initialize(playerInventory);

        linkingCotroller = LocalPlayer.GameObject.AddComponent<Linking.LineRenderer>();

        // Mark the world as ready for loading
        prefabSaveManager.SetWorldReady();
    }
    // Saving System
    private static PrefabSaveManager prefabSaveManager;

    // Prefabs
    internal static WirelessTransmitterSwitch transmitterSwitch;
    internal static Reciver reciver;
    internal static TransmitterDetector transmitterDetector;

    // Line Renderer for Linking
    internal static Linking.LineRenderer linkingCotroller;

    // Structure For Ingame System
    internal static Structure.Reciver reciverStructure;
    internal static Structure.TransmitterSwitch transmitterSwitchStructure;
    internal static Structure.TransmitterDetector transmitterDetectorStructure;

    // Material For Line Renderer
    internal static Material blackMat = Assets.TransmitterSwitch.transform.GetChild(0).FindChild("Wire (418)").GetChild(0).GetComponent<MeshRenderer>().materials[0];
    internal static Material redMat = Assets.TransmitterSwitch.transform.GetChild(0).GetChild(16).GetChild(0).GetComponent<MeshRenderer>().materials[0];



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
            case "spawn3":
                Misc.Msg("[WirelessCmd] Spawning - TransmitterDetector");
                var transmitterDetectorParameters = new Prefab.TransmitterDetectorSpawnParameters
                {
                    position = raycastHit.point + Vector3.up * 0.1f,
                    rotation = LocalPlayer.Transform.rotation,
                    uniqueId = null,
                    isOn = false
                };
                Misc.Msg("[WirelessCmd] Spawning - TransmitterDetector Parameters Created");
                transmitterDetector.Spawn(transmitterDetectorParameters);
                Misc.Msg("[WirelessCmd] Complete - Spawning TransmitterDetector");
                break;
            case "rml":
                Misc.Msg("[WirelessCmd] Removing RayCast Lines");
                Debug.RayCast.RemoveLines();
                break;
        }
        
    }
}