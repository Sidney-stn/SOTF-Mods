using SonsSdk;
using SonsSdk.Attributes;
using TheForest.Utils;
using UnityEngine;
using SUI;

namespace Shops;

public class Shops : SonsMod
{
    public Shops()
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

        // Registering Save System
        var manager = new Saving.Manager(); // Shops
        SonsSaveTools.Register(manager);

        Structure.Setup.Crafting();

        // Subscribe To Event
        Banking.API.SubscribableEvents.OnPlayerJoin += OnPlayerJoin;  // When a player joins the game in MP
        Banking.API.SubscribableEvents.OnCashChange += OnCashChange;  // When a player's cash changes on the network
        Banking.API.SubscribableEvents.OnLeaveWorld += OnLeaveWorld;  // When a THE LOCALPLAYER leaves the world
        Banking.API.SubscribableEvents.OnJoinWorld += OnJoinedWorld;  // When a THE LOCALPLAYER joins the world
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.
        Prefab.SingleShop.SetupPrefab();
    }

    internal static void OnJoinedWorld()
    {
        // This is called when the player joins a world.

        Network.Manager.RegisterEvents();


        Misc.Msg("[Loading] Processing deferred load.");

        // Process all deferred load data
        while (Saving.Load.deferredLoadQueue.Count > 0)
        {
            var obj = Saving.Load.deferredLoadQueue.Dequeue();
            Saving.Load.ProcessLoadData(obj);
        }
    }

    internal static void OnLeaveWorld()
    {
        // This is called when the player leaves a world.
        GameObject.Destroy(Prefab.SingleShop.gameObjectWithComps);
        Prefab.SingleShop.spawnedShops.Clear();
        foreach (var sign in Saving.Load.ModdedShops)
        {
            GameObject.Destroy(sign);
        }
        Saving.Load.ModdedShops.Clear();

        Network.Manager.UnregisterEvents();

        // Unsubscribe To Event
        Banking.API.SubscribableEvents.OnPlayerJoin -= OnPlayerJoin;
        Banking.API.SubscribableEvents.OnCashChange -= OnCashChange;
        Banking.API.SubscribableEvents.OnLeaveWorld -= OnLeaveWorld;
        Banking.API.SubscribableEvents.OnJoinWorld -= OnJoinedWorld;
    }

    internal static void OnPlayerJoin()
    {
        // This is called when a player joins the game in MP.
    }

    internal static void OnCashChange()
    {
        // This is called when a player's cash changes on the network.
    }

    [DebugCommand("shops")]
    private void ShopCmd(string args)
    {
        Misc.Msg("Shop Command");
        Transform transform = LocalPlayer._instance._mainCam.transform;
        RaycastHit raycastHit;
        Physics.Raycast(transform.position, transform.forward, out raycastHit, 25f, LayerMask.GetMask(new string[]
        {
                "Terrain",
                "Default",
                "Prop"
        }));
        switch (args.ToLower())
        {
            case "spawn":
                GameObject go = GameObject.Instantiate(Prefab.SingleShop.gameObjectWithComps);
                go.transform.position = raycastHit.point + Vector3.up * 0.1f;
                go.transform.rotation = LocalPlayer.Transform.rotation;
                Mono.Shop mono = go.GetComponent<Mono.Shop>();
                mono.UniqueId = Guid.NewGuid().ToString();
                mono.OwnerId = Banking.API.GetLocalPlayerId();
                mono.OwnerName = Banking.API.GetLocalPlayerName();
                mono.numberOfSellableItems = 1;
                Misc.Msg($"Set OwnerId To: {mono.OwnerId}");
                break;
            case "sync":
                break;
            case "delsave":
                break;
            case "removeall":
                break;
            default:
                break;
        }
    }
}