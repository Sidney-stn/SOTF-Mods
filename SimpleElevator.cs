using Bolt;
using Il2CppInterop.Runtime.Injection;
using RedLoader;
using SonsSdk;
using SonsSdk.Attributes;
using SUI;
using TheForest.Utils;
using UnityEngine;


namespace SimpleElevator;

public class SimpleElevator : SonsMod, IOnAfterSpawnReceiver
{
    public SimpleElevator()
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

    public static SimpleElevator Instance;
    internal Structure.Elevator ElevatorInstance;
    internal Structure.ElevatorControlPanel ElevatorControlPanelInstace;

    protected override void OnInitializeMod()
    {
        if (Testing.DedicatedServer.IsDeticatedServer())
        {
            RLog.Msg(System.ConsoleColor.Blue, "[SimpleElevator] [DEDICATED SERVER] OnInitializeMod");
        }
        // Do your early mod initialization which doesn't involve game or sdk references here
        Config.Init();

        // Load assets
        Assets.Instance.LoadAssets();

        // Registier Classes In Il2cpp
        ClassInjector.RegisterTypeInIl2Cpp<Mono.ScrollMono>();
        ClassInjector.RegisterTypeInIl2Cpp<Mono.ElevatorMono>();
        ClassInjector.RegisterTypeInIl2Cpp<Mono.ElevatorControlPanelMono>();
        ClassInjector.RegisterTypeInIl2Cpp<Network.ElevatorSetter>();
        ClassInjector.RegisterTypeInIl2Cpp<Network.ElevatorControlPanelSetter>();

        // Register network classes
        Network.Manager.Register();

        if (Assets.Instance.IsLoaded())
        {
            Misc.Msg("Assets Loaded");
        } else
        {
            RLog.Error("[SimpleElevator] Assets Not Loaded");
        }

        if (Testing.DedicatedServer.IsDeticatedServer())
        {
            // Registering Save System
            //var manager = new Saving.Manager(); // Signs
            //SonsSaveTools.Register(manager);

            SdkEvents.OnGameActivated.Subscribe(OnFirstGameActivation, unsubscribeOnFirstInvocation: true);

            ElevatorInstance = Structure.Elevator.Instance;
            ElevatorControlPanelInstace = Structure.ElevatorControlPanel.Instance;

        }
    }

    protected override void OnSdkInitialized()
    {
        if (Testing.DedicatedServer.IsDeticatedServer())
        {
            // This Never Runs On Dedicated Server
            RLog.Msg(System.ConsoleColor.Blue, "[SimpleElevator] [DEDICATED SERVER] OnSdkInitialized");  // This never runs on dedicated server
        }
        // Do your mod initialization which involves game or sdk references here
        // This is for stuff like UI creation, event registration etc.
        SimpleElevatorUi.Create();

        // Add in-game settings ui for your mod.
        SettingsRegistry.CreateSettings(this, null, typeof(Config));

        SdkEvents.OnGameActivated.Subscribe(OnFirstGameActivation, unsubscribeOnFirstInvocation: true);


        // Registering Save System
        //var manager = new Saving.Manager();
        //SonsSaveTools.Register(manager);

        ElevatorInstance = Structure.Elevator.Instance;
        ElevatorControlPanelInstace = Structure.ElevatorControlPanel.Instance;
    }

    protected override void OnGameStart()
    {
        if (Testing.DedicatedServer.IsDeticatedServer())
        {
            RLog.Msg(System.ConsoleColor.Blue, "[StoneGate] [DEDICATED SERVER] OnGameStart");
        }
        // Register Network Event Handlers
        Network.Manager.RegisterEventHandlers();
    }

    private void OnFirstGameActivation()
    {
        if (Testing.DedicatedServer.IsDeticatedServer())
        {
            RLog.Msg(System.ConsoleColor.Blue, "[StoneGate] [DEDICATED SERVER] OnFirstGameActivation");
        }
    }

    public void OnAfterSpawn()
    {
        LocalPlayer._instance.gameObject.AddComponent<Mono.ScrollMono>();
        if (BoltNetwork.isRunning && BoltNetwork.isClient)
        {
            if (Settings.logSavingSystem)
                Misc.Msg("[Loading] Skipped Loading StoneGates On Multiplayer Client");
            return;
        }

        // Process all deferred load data, if host
        //while (Saving.Load.deferredLoadQueue.Count > 0)
        //{
        //    var obj = Saving.Load.deferredLoadQueue.Dequeue();
        //    Saving.Load.ProcessLoadData(obj);
        //}

    }
}