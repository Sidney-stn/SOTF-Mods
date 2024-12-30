using SonsSdk;
using SonsSdk.Attributes;
using SUI;
using TheForest.Utils;
using UnityEngine;

namespace Banking;

public class Banking : SonsMod
{
    public Banking()
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

        // Registering Save System
        var manager = new Saving.Manager(); // ATMS
        SonsSaveTools.Register(manager);

        // Adding Ingame CFG
        SettingsRegistry.CreateSettings(this, null, typeof(Config));

        UI.Setup.SetupUI();
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.

        // Adding Quit Event / Get HostMode Events
        SonsSdk.SdkEvents.OnInWorldUpdate.Subscribe(Misc.CheckHostModeOnWorldUpdate);
        Misc.OnHostModeGotten += Misc.OnHostModeGottenCorrectly;

        // Setup Prefab
        Prefab.ActiveATM.SetupAtmPrefab();
    }

    internal static void OnLeaveWorld()
    {
        Misc.Msg("OnLeaveWorld");

        Misc.OnHostModeGotten -= Misc.OnHostModeGottenCorrectly;
        Misc.dialogManager.QuitGameConfirmDialog.remove_OnOption1Clicked((Il2CppSystem.Action)OnLeaveWorld);

        // Unregistering Network Events
        Network.Manager.UnregisterEvents();
        LiveData.Players.CleanUp();

        // Trigger Event
        API.SubscribableEvents.TriggerOnLeaveWorld();

        GameObject.Destroy(Prefab.ActiveATM.atmWithComps);
        Prefab.ActiveATM.spawnedAtms.Clear();  // Clear Spawned ATMs
        foreach (var atm in Saving.Load.ModdedAtms)
        {
            GameObject.Destroy(atm);
        }
        Prefab.ATMPlacer.spawnedATMPlacers.Clear();  // Clear Spawned ATMPlacers
        foreach (var atm in Saving.Load.ModdedATMPlacers)
        {
            GameObject.Destroy(atm);
        }
        Saving.Load.ModdedAtms.Clear();  // Remove Save Data
        Saving.Load.ModdedATMPlacers.Clear();  // Remove Save Data

        
    }

    [DebugCommand("getcurrency")]
    private void GetCurrency(string args)  // Args = SteamName
    {
        Misc.Msg("Get Currency Command");
        if (string.IsNullOrEmpty(args))
        {
            Misc.Msg("[GetCurrency] No Args");
            return;
        }
        if (args == "all")
        {
            foreach (var player in LiveData.Players.GetPlayers())
            {
                Misc.Msg($"Player: {player.Value} - Currency: {LiveData.Players.GetPlayerCurrency(LiveData.Players.GetCurrencyType.SteamID, player.Key)}");
                SonsTools.ShowMessage($"Player: {player.Value} - Currency: {LiveData.Players.GetPlayerCurrency(LiveData.Players.GetCurrencyType.SteamID, player.Key)}");
            }
            return;
        }
        var currency = LiveData.Players.GetPlayerCurrency(LiveData.Players.GetCurrencyType.PlayerName, args);
        if (currency == null)
        {
            Misc.Msg($"Player: {args} not found");
            SonsTools.ShowMessage($"Player: {args} not found");
            return;
        }
        else
        {
            Misc.Msg($"Player: {args} - Currency: {currency}");
            SonsTools.ShowMessage($"Player: {args} - Currency: {currency}");
        }
    }

    [DebugCommand("addcurrency")]
    private void AddCurrency(string args) // Args = SteamName, Amount
    {
        if (string.IsNullOrEmpty(args))
        {
            Misc.Msg("[AddCurrency] No Args");
            SonsTools.ShowMessage("[AddCurrency] No Args");
            return;
        }
        if (Misc.hostMode != Misc.SimpleSaveGameType.Multiplayer)
        {
            Misc.Msg("[AddCurrency] You Are Not Host Can't Add Cash");
            SonsTools.ShowMessage("[AddCurrency] You Are Not Host Can't Add Cash");
            return;
        }
        var split = args.Split(',');  // Splitting SteamName, Amount
        if (split.Length != 2)
        {
            Misc.Msg("[AddCurrency] Invalid Args");
            SonsTools.ShowMessage("[AddCurrency] Invalid Args");
            return;
        }
        if (int.Parse(split[1]) <= 0)
        {
            Misc.Msg("[RemoveCurrency] Invalid Amount");
            SonsTools.ShowMessage("[RemoveCurrency] Invalid Amount");
            return;
        }

        var allPlayers = LiveData.Players.GetPlayers();
        bool found = false;
        foreach (var player in allPlayers)
        {
            if (player.Value == split[0])
            {
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.AddCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    Currency = int.Parse(split[1]),
                    ToPlayerId = player.Key
                });
                found = true;
                return;
            }
        }
        if (!found)
        {
            Misc.Msg("[AddCurrency] Player Not Found");
            SonsTools.ShowMessage("[AddCurrency] Player Not Found");
        }
    }

    [DebugCommand("removecurrency")]
    private void RemoveCurrency(string args) // Args = SteamName, Amount
    {
        if (string.IsNullOrEmpty(args))
        {
            Misc.Msg("[RemoveCurrency] No Args");
            SonsTools.ShowMessage("[RemoveCurrency] No Args");
            return;
        }
        if (Misc.hostMode != Misc.SimpleSaveGameType.Multiplayer)
        {
            Misc.Msg("[RemoveCurrency] You Are Not Host Can't Add Cash");
            SonsTools.ShowMessage("[RemoveCurrency] You Are Not Host Can't Add Cash");
            return;
        }
        var split = args.Split(',');  // Splitting SteamName, Amount
        if (split.Length != 2)
        {
            Misc.Msg("[RemoveCurrency] Invalid Args");
            SonsTools.ShowMessage("[RemoveCurrency] Invalid Args");
            return;
        }
        if (int.Parse(split[1]) <= 0)
        {
            Misc.Msg("[RemoveCurrency] Invalid Amount");
            SonsTools.ShowMessage("[RemoveCurrency] Invalid Amount");
            return;
        }

        var allPlayers = LiveData.Players.GetPlayers();
        bool found = false;
        foreach (var player in allPlayers)
        {
            if (player.Value == split[0])
            {
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RemoveCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    Currency = int.Parse(split[1]),
                    ToPlayerId = player.Key
                });
                found = true;
                return;
            }
        }
        if (!found)
        {
            Misc.Msg("[RemoveCurrency] Player Not Found");
            SonsTools.ShowMessage("[RemoveCurrency] Player Not Found");
        }
    }

    [DebugCommand("synccurrency")]
    private void SyncCurrency(string args)
    {
        if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
        {
            Misc.Msg("[SyncCurrency] Syncing Cash");
            SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RequestSyncCash
            {
                SenderName = Misc.GetLocalPlayerUsername(),
                SenderId = Misc.MySteamId().Item2,
                RepsondToId = "None"
            });
        }
        else
        {
            Misc.Msg("[SyncCurrency] You Are Not Online Can't Sync Cash");
            SonsTools.ShowMessage("[SyncCurrency] You Are Not Online Can't Sync Cash");
            return;
        }

    }

    [DebugCommand("spawnatm")]
    private void SpawnAtm(string args)
    {
        Misc.Msg("[SpawnAtm] Command");
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
            Misc.Msg("[SpawnAtm] Raycast Hit Nothing");
            SonsTools.ShowMessage("Raycast Hit Nothing", 5);
            return;
        }
        switch (Misc.hostMode)
        {
            case Misc.SimpleSaveGameType.SinglePlayer:
                Misc.Msg("[SpawnAtm] [SinglePlayer] ATM Can't Be Spawned In SinglePlayer");
                SonsTools.ShowMessage("ATM Can't Be Spawned In SinglePlayer", 5);
                break;
            case Misc.SimpleSaveGameType.Multiplayer:
                Misc.Msg("[SpawnAtm] [Multiplayer] Trying To Spawn Atm Multiplayer");
                Prefab.ActiveATM.SpawnATM(raycastHit.point + Vector3.up * 0.1f, LocalPlayer.Transform.rotation);
                SonsTools.ShowMessage("ATM Spawned", 5);
                break;
            case Misc.SimpleSaveGameType.MultiplayerClient:
                Misc.Msg("[SpawnAtm] [MultiplayerClient] ATM Can't Be Spawned As Client For Free");
                SonsTools.ShowMessage("ATM Can't Be Spawned As Client For Free", 5);
                break;
            default:
                Misc.Msg("[SpawnAtm] [Default] Invalid HostMode");
                break;
        }
    }

    [DebugCommand("deleteatm")]
    private void DeleteAtm(string args)
    {
        Misc.Msg("[DeleteAtm] Command");
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
            Misc.Msg("[DeleteAtm] Raycast Hit Nothing");
            SonsTools.ShowMessage("Raycast Hit Nothing", 5);
            return;
        }
        switch (Misc.hostMode)
        {
            case Misc.SimpleSaveGameType.SinglePlayer:
                Misc.Msg("[DeleteAtm] [SinglePlayer] ATM Can't Be Spawned In SinglePlayer");
                SonsTools.ShowMessage("ATM Can't Be Delete In SinglePlayer", 5);
                break;
            case Misc.SimpleSaveGameType.Multiplayer:
                Misc.Msg("[DeleteAtm] [Multiplayer] Trying To Delete Atm Multiplayer");
                // Get Hit Object Root GameObject
                GameObject atm = raycastHit.collider.transform.root.gameObject;
                if (atm.name.Contains("ATM"))
                {
                    if (Prefab.ActiveATM.spawnedAtms.ContainsValue(atm))
                    {
                        string uniqueId = atm.GetComponent<Mono.ATMController>().UniqueId;
                        if (string.IsNullOrEmpty(uniqueId))
                        {
                            Misc.Msg("[DeleteAtm] UniqueId Is Null Or Empty");
                            SonsTools.ShowMessage("UniqueId Is Null Or Empty", 5);
                            return;
                        }
                        Prefab.ActiveATM.spawnedAtms.Remove(uniqueId);
                        Saving.Load.ModdedAtms.Remove(atm);
                        GameObject.Destroy(atm);

                        // Raise Network Event
                        SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RemoveATM
                        {
                            UniqueId = uniqueId
                        });
                    }
                    else
                    {
                        Misc.Msg("[DeleteAtm] ATM Not Found");
                        SonsTools.ShowMessage("ATM Not Found", 5);
                    }
                }
                else
                {
                    Misc.Msg("[DeleteAtm] Hit Object Is Not ATM");
                    SonsTools.ShowMessage("Hit Object Is Not ATM", 5);
                }
                SonsTools.ShowMessage("Deleted ATM", 5);
                break;
            case Misc.SimpleSaveGameType.MultiplayerClient:
                Misc.Msg("[DeleteAtm] [MultiplayerClient] ATM Can't Be Deleted As Client For Free");
                SonsTools.ShowMessage("ATM Can't Be Deleted As Client", 5);
                break;
            default:
                Misc.Msg("[DeleteAtm] [Default] Invalid HostMode");
                break;
        }
    }
}