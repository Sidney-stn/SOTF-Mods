using SonsSdk;
using SonsSdk.Attributes;
using SUI;
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
        Prefab.ActiveATM.spawnedAtms.Clear();
        foreach (var atm in Saving.Load.ModdedAtms)
        {
            GameObject.Destroy(atm);
        }
        Saving.Load.ModdedAtms.Clear();
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
}