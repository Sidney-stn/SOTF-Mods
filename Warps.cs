using SonsSdk;
using static TheForest.Player.Actions.GatheringPrefabsDefinition;
using UnityEngine;
using SUI;
using SonsSdk.Attributes;
using TheForest.Utils;
using static Il2CppMono.Math.BigInteger;

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

        Saving.LoadedWarps.loadedWarps.Clear();
        //Saving.Load.Warps.Clear();
    }

    [DebugCommand("addwarp")]
    private void AddWarp(string args)
    {
        Misc.Msg("Add Warp Command");
        if (string.IsNullOrEmpty(args))
        {
            Misc.Msg("No Warp Name Provided");
            SonsTools.ShowMessage("No Warp Name Provided");
            return;
        }

        if (Saving.LoadedWarps.loadedWarps.ContainsKey(args))
        {
            Misc.Msg("Warp Name Already Exists");
            SonsTools.ShowMessage("Warp Name Already Exists");
            return;
        }
        else
        {
            Saving.LoadedWarps.loadedWarps.Add(args, LocalPlayer.Transform.position);
            Misc.Msg("Warp Added");
            SonsTools.ShowMessage($"Warp [{args}] Added");

            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient || Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer)
            {
                // Add Warp Over Network
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.AddWarp
                {
                    WarpName = args,
                    Vector3String = Network.CustomSerializable.Vector3ToString(LocalPlayer.Transform.position),
                    Sender = Misc.MySteamId().Item2,
                    SenderName = Misc.GetLocalPlayerUsername(),
                    ToSteamId = "None"
                });
            }
        }
    }

    [DebugCommand("removewarp")]
    private void RemoveWarp(string args)
    {
        Misc.Msg("Remove Warp Command");
        if (string.IsNullOrEmpty(args))
        {
            Misc.Msg("No Warp Name Provided");
            SonsTools.ShowMessage("No Warp Name Provided");
            return;
        }
        if (!Saving.LoadedWarps.loadedWarps.ContainsKey(args))
        {
            Misc.Msg("Warp Name Does Not Exist");
            SonsTools.ShowMessage("Warp Name Does Not Exist");
            return;
        }
        else
        {
            Saving.LoadedWarps.loadedWarps.Remove(args);
            Misc.Msg("Warp Removed");
            SonsTools.ShowMessage($"Warp [{args}] Removed");

            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient || Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer)
            {
                // Remove Warp Over Network
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.DeleteWarp
                {
                    WarpName = args
                });
            }
        }
    }
    [DebugCommand("testwarps")]
    private void TestWarp(string args)
    {
        for (int i = 0; i < 9; i++)
        {
            Saving.LoadedWarps.loadedWarps.Add($"TestWarp{i}", LocalPlayer.Transform.position);
        }
    }
}