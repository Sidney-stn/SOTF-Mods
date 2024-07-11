using RedLoader;
using SonsSdk;
using SUI;
using System.Reflection;
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

        LoadUnityExplorerDllIfFound();
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
            RLog.Msg("No GameObject with DebugConsole found.");
        }
    }

    public static void LoadUnityExplorerDllIfFound()
    {
        // Get the directory of the executing assembly
        string executingAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // Get the parent directory
        string parentDirectory = Directory.GetParent(executingAssemblyDirectory).FullName;

        // Combine to get the path to the Mods directory
        string dllPath = Path.Combine(parentDirectory, "Mods", "UnityExplorer.dll");

        RLog.Msg($"Dll Path: {dllPath}");

        if (File.Exists(dllPath))
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                RLog.Msg("UnityExplorer.dll found and loaded.");

                UnityExplorer.Ui.UIManager foundDebugConsole = GameObject.FindObjectOfType<UnityExplorer.Ui.UIManager>();
            }
            catch (Exception ex)
            {
                RLog.Msg($"Failed to load UnityExplorer.dll: {ex.Message}");
            }
        }
        else
        {
            RLog.Msg("UnityExplorer.dll not found.");
        }
    }

    public static DebugConsole debugConsole = null;
}