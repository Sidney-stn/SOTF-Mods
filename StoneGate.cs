using Bolt;
using Il2CppInterop.Runtime.Injection;
using RedLoader;
using SonsSdk;
using SUI;

namespace StoneGate;

public class StoneGate : SonsMod
{
    public StoneGate()
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

        // Load assets
        //Assets.Instance.LoadAssets();

        // Registier Classes In Il2cpp
        ClassInjector.RegisterTypeInIl2Cpp<Mono.StoneGateMono>();

        // Register network classes
        Network.Manager.Register();

        //if (Assets.Instance.IsLoaded())
        //{
        //    conveyorBelt = Structure.ConveyorBelt.Instance;
        //    Misc.Msg("ConveyorBelt Set");
        //}
        //else { RLog.Error("Asset Not Loaded"); }
        stoneGate = Structure.StoneGate.Instance;
    }

    protected override void OnSdkInitialized()
    {
        // Do your mod initialization which involves game or sdk references here
        // This is for stuff like UI creation, event registration etc.
        StoneGateUi.Create();

        // Add in-game settings ui for your mod.
        SettingsRegistry.CreateSettings(this, null, typeof(Config));
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.
    }

    internal static Structure.StoneGate stoneGate;
}