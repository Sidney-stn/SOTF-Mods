using Il2CppInterop.Runtime;
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

        //LoadUnityExplorerDllIfFound();
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
            Msg("No GameObject with DebugConsole found.");
        }

        LoadUnityExplorerDllIfFound();
    }

    internal static bool alreadyLoaded = false;

    public static void LoadUnityExplorerDllIfFound()
    {
        if (alreadyLoaded)
        {
            Msg("UnityExplorer Dll Should Already Be Found And Loaded, returning");
            return;
        }
        alreadyLoaded = true;

        // Get the directory of the executing assembly
        string executingAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // Get the parent directory
        string parentDirectory = Directory.GetParent(executingAssemblyDirectory).FullName;

        // Combine to get the path to the Mods directory
        string dllPath = Path.Combine(parentDirectory, "Mods", "UnityExplorer.dll");

        Msg($"Dll Path: {dllPath}");

        if (File.Exists(dllPath))
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Msg("UnityExplorer.dll found and loaded.");

                // Use the ClassSearch method to find the UnityExplorer.UI.UIManager type
                Type uiManagerType = ClassSearch("UnityExplorer.UI.UIManager");
                if (uiManagerType != null)
                {
                    Msg("UnityExplorer.UI.UIManager type found.");

                    // Check the value of the static property or field
                    bool showMenu = CheckShowMenuProperty(uiManagerType);
                    Msg($"UnityExplorer.UI.UIManager.ShowMenu is {showMenu}");
                }
                else
                {
                    Msg("UnityExplorer.UI.UIManager type not found.");
                }
            }
            catch (Exception ex)
            {
                Msg($"Failed to load UnityExplorer.dll: {ex.Message}");
            }
        }
        else
        {
            Msg("UnityExplorer.dll not found.");
        }
    }

    private static bool CheckShowMenuProperty(Type uiManagerType)
    {
        try
        {
            // Attempt to get the property first
            PropertyInfo showMenuProperty = uiManagerType.GetProperty("ShowMenu", BindingFlags.Static | BindingFlags.Public);
            if (showMenuProperty != null)
            {
                return (bool)showMenuProperty.GetValue(null);
            }

            // If the property is not found, attempt to get the field
            FieldInfo showMenuField = uiManagerType.GetField("ShowMenu", BindingFlags.Static | BindingFlags.Public);
            if (showMenuField != null)
            {
                return (bool)showMenuField.GetValue(null);
            }

            Msg("ShowMenu property or field not found.");
        }
        catch (Exception ex)
        {
            Msg($"Error checking ShowMenu property: {ex.Message}");
        }

        return false;
    }

    private static Type ClassSearch(string input)
    {
        Msg($"Starting ClassSearch with input: {input}");

        string nameFilter = "";
        if (!string.IsNullOrEmpty(input))
            nameFilter = input;

        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in asm.GetTypes())
            {
                if (!string.IsNullOrEmpty(nameFilter) && type.FullName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
                {
                    Msg($"Found matching type: {type.FullName}");
                    return type;
                }
            }
        }

        Msg("No matching type found.");
        return null;
    }


    internal static void Msg(string msg)
    {
        if (Config.LoggingHotKeyCommands.Value)
        {
            RLog.Msg(msg);
        }
    }

    public static DebugConsole debugConsole = null;
}