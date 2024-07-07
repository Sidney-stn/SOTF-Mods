using RedLoader;
using SonsSdk;
using SUI;
using TheForest;
using UnityEngine;

namespace HotKeyCommands;

public class HotKeyCommands : SonsMod
{
    public HotKeyCommands()
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
        HotKeyCommandsUi.Create();

        // Adding Ingame CFG
        SettingsRegistry.CreateSettings(this, null, typeof(Config));
    }

    protected override void OnGameStart()
    {
        // Find the first object of type MyComponent in the scene
        DebugConsole foundDebugConsole = GameObject.FindObjectOfType<DebugConsole>();

        if (foundDebugConsole != null)
        {
            debugConsole = foundDebugConsole;
        }
        else
        {
            RLog.Msg("No GameObject with MyComponent found.");
        }
    }

    public static DebugConsole debugConsole = null;
}