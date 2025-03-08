using SonsSdk;
using SonsSdk.Attributes;
using SUI;
using System.Diagnostics.Tracing;
using TheForest.Utils;
using UnityEngine;

namespace Signs;

public class Signs : SonsMod
{
    public static Signs Instance;
    public Signs()
    {

        // Uncomment any of these if you need a method to run on a specific update loop.
        //OnUpdateCallback = MyUpdateMethod;
        //OnLateUpdateCallback = MyLateUpdateMethod;
        //OnFixedUpdateCallback = MyFixedUpdateMethod;
        //OnGUICallback = MyGUIMethod;

        // Uncomment this to automatically apply harmony patches in your assembly.
        //HarmonyPatchAll = true;
        Instance = this;
    }

    protected override void OnInitializeMod()
    {
        // Do your early mod initialization which doesn't involve game or sdk references here
        Config.Init();

        Network.Manager.Register();

        if (Tools.DedicatedServer.IsDeticatedServer())
        {
            // Registering Save System
            var manager = new Saving.Manager(); // Signs
            SonsSaveTools.Register(manager);

            Structure.Setup.Crafting();
        }
        
    }

    protected override void OnSdkInitialized()  // Not called on Dedicated Server
    {
        // Do your mod initialization which involves game or sdk references here

        // Adding Ingame CFG
        SettingsRegistry.CreateSettings(this, null, typeof(Config));

        // Registering Save System
        var manager = new Saving.Manager(); // Signs
        SonsSaveTools.Register(manager);

        Structure.Setup.Crafting();

        UI.Setup.SetupUI();
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.

        // Adding Quit Event / Get HostMode Events
        SonsSdk.SdkEvents.OnInWorldUpdate.Subscribe(Misc.CheckHostModeOnWorldUpdate);
        Misc.OnHostModeGotten += Misc.OnHostModeGottenCorrectly;

        Integrations.HotKeyCommandsIntegration.Setup();

        // Register Network Event Handlers
        Network.Manager.RegisterEventHandlers();

        if (BoltNetwork.isRunning && !BoltNetwork.isServer)
        {
            Network.CustomEventHandler.Instance.OnEnterWorld();
        }
    }

    internal static void OnLeaveWorld()
    {
        // This is called when the player leaves the world.
        Misc.Msg("OnLeaveWorld");
        Misc.OnHostModeGotten -= Misc.OnHostModeGottenCorrectly;
        Misc.dialogManager.QuitGameConfirmDialog.remove_OnOption1Clicked((Il2CppSystem.Action)OnLeaveWorld);
        Saving.Track.Clear();
    }

    //[DebugCommand("signs")]
    //private void SignCmd(string args)
    //{
    //    Misc.Msg("Sign Command");
    //    Transform transform = LocalPlayer._instance._mainCam.transform;
    //    RaycastHit raycastHit;
    //    Physics.Raycast(transform.position, transform.forward, out raycastHit, 25f, LayerMask.GetMask(new string[]
    //    {
    //            "Terrain",
    //            "Default",
    //            "Prop"
    //    }));
    //    switch (args.ToLower())
    //    {
    //        case "spawn":
    //            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
    //            {
    //                Prefab.SignPrefab.spawnSignSingePlayer(raycastHit.point + Vector3.up * 0.1f, LocalPlayer.Transform.rotation);
    //            }
    //            else if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
    //            {
    //                Misc.Msg("[SignCmd] [Multiplayer] Trying To Spawn Sign Multiplayer");
    //                Prefab.SignPrefab.spawnSignMultiplayer(raycastHit.point + Vector3.up * 0.1f, LocalPlayer.Transform.rotation, raiseCreateEvent: true);
    //            }
    //            break;
    //        case "sync":
    //            //IngameTools.SyncShopTools.SendSyncEventLookingAt(eventType: SyncShopTools.ShopEventType.Sync);
    //            break;
    //        case "delsave":
    //            Misc.Msg("Clearing Signs");
    //            Saving.Load.ModdedSigns.Clear();
    //            break;
    //        case "removeall":
    //            Misc.Msg("Removing All Signs");
    //            foreach (var sign in Saving.Load.ModdedSigns)
    //            {
    //                GameObject.Destroy(sign);
    //            }
    //            Saving.Load.ModdedSigns.Clear();
    //            break;
    //        default:
    //            break;
    //    }
    //}
}