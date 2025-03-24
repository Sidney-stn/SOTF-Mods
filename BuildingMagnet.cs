using Endnight.Utilities;
using SonsSdk;
using SonsSdk.Attributes;
using TheForest.Utils;
using UnityEngine;

namespace BuildingMagnet;

public class BuildingMagnet : SonsMod, IOnAfterSpawnReceiver
{
    public BuildingMagnet()
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
        Network.Manager.Register();
    }

    protected override void OnSdkInitialized()
    {
        // Do your mod initialization which involves game or sdk references here
        // This is for stuff like UI creation, event registration etc.
        if (IsDeticatedServer()) { return; }
        BuildingMagnetUi.Create();

        // Add in-game settings ui for your mod.
        // SettingsRegistry.CreateSettings(this, null, typeof(Config));
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.
        
    }

    public void OnAfterSpawn()
    {
        if (IsDeticatedServer()) { return; }
        LocalPlayer.GameObject.GetOrAddComponent<BuildingMagnetMono>();
    }

    public static bool isItemUnlocked = true;


    public static bool IsDeticatedServer()
    {
        string dataPath = Application.dataPath;

        // sotfPath Are 1 Level Up From The DataPath
        string sotfPath = Directory.GetParent(dataPath).FullName;

        // SonsOfTheForestDS.exe
        string sotfDs = Path.Combine(sotfPath, "SonsOfTheForestDS.exe");

        // Check If The File Exists
        if (File.Exists(sotfDs))
        {
            return true;
        }
        return false;
    }
}