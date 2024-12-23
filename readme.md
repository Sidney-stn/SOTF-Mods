## HotKeyCommands SUI UI Integration Guide
#### Want HotKeyCommands to not trigger commands when using your custom UI? Follow these steps to integrate your UI with HotKeyCommands and ensure it respects your custom elements:

#### Steps:
- Add the following class to your project:
```csharp
internal static class HotKeyCommandsIntegration
    {
        internal static void Setup()
        {
            LoadHotKeyCommandsDllIfFound();
            AddUnityElement(UI.Setup.AddUI);  // Add Custom Unity Ui To HotKeyCommands, so it does not trigger commands while your custom UI is active
            AddUnityElement(UI.SetupSignPlace.AddUI);  // Same as above
            AddSUIElement("YourCustomUI");  // Add Custom SUI Ui To HotKeyCommands, so it does not trigger commands while your custom UI is active
        }

        private static bool alreadyLoaded = false;
        private static Assembly hotKeyCommandsAssembly;
        private static Type suiuiType;
        private static Type unityUiType;

        public static void LoadHotKeyCommandsDllIfFound()
        {
            if (alreadyLoaded)
            {
                Misc.Msg("HotKeyCommands DLL should already be found and loaded, returning");
                return;
            }
            alreadyLoaded = true;

            // Define the path to the DLL
            string executingAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string parentDirectory = Directory.GetParent(executingAssemblyDirectory).FullName;
            string dllPath = Path.Combine(parentDirectory, "Mods", "HotKeyCommands.dll");

            Misc.Msg($"Dll Path: {dllPath}");

            if (File.Exists(dllPath))
            {
                try
                {
                    // Load the DLL
                    hotKeyCommandsAssembly = Assembly.LoadFrom(dllPath);
                    Misc.Msg("HotKeyCommands.dll found and loaded.");

                    // Get the SUIUI type
                    suiuiType = hotKeyCommandsAssembly.GetType("HotKeyCommands.SUIUI");
                    unityUiType = hotKeyCommandsAssembly.GetType("HotKeyCommands.UnityUi");

                    if (suiuiType != null)
                    {
                        Misc.Msg("HotKeyCommands.SUIUI type found.");
                    }
                    else
                    {
                        Misc.Msg("HotKeyCommands.SUIUI type not found.");
                    }

                    if (unityUiType != null)
                    {
                        Misc.Msg("HotKeyCommands.UnityUi type found.");
                    }
                    else
                    {
                        Misc.Msg("HotKeyCommands.UnityUi type not found.");
                    }
                }
                catch (Exception ex)
                {
                    Misc.Msg($"Failed to load HotKeyCommands.dll: {ex.Message}");
                }
            }
            else
            {
                Misc.Msg("HotKeyCommands.dll not found.");
            }
        }

        public static void AddSUIElement(string element)
        {
            if (suiuiType == null)
            {
                Misc.Msg("SUIUI type is not loaded.");
                return;
            }

            try
            {
                MethodInfo addMethod = suiuiType.GetMethod("AddSUIElemet", BindingFlags.Static | BindingFlags.Public);
                if (addMethod != null)
                {
                    addMethod.Invoke(null, new object[] { element });
                    Misc.Msg($"Element '{element}' added.");
                }
                else
                {
                    Misc.Msg("AddSUIElemet method not found.");
                }
            }
            catch (Exception ex)
            {
                Misc.Msg($"Error invoking AddSUIElemet: {ex.Message}");
            }
        }

        public static void RemoveSUIElement(string element)
        {
            if (suiuiType == null)
            {
                Misc.Msg("SUIUI type is not loaded.");
                return;
            }

            try
            {
                MethodInfo removeMethod = suiuiType.GetMethod("RemoveSUIElemet", BindingFlags.Static | BindingFlags.Public);
                if (removeMethod != null)
                {
                    removeMethod.Invoke(null, new object[] { element });
                    Misc.Msg($"Element '{element}' removed.");
                }
                else
                {
                    Misc.Msg("RemoveSUIElemet method not found.");
                }
            }
            catch (Exception ex)
            {
                Misc.Msg($"Error invoking RemoveSUIElemet: {ex.Message}");
            }
        }

        public static void AddUnityElement(GameObject unityElement)
        {
            if (unityUiType == null)
            {
                Misc.Msg("UnityUi type is not loaded.");
                return;
            }

            try
            {
                MethodInfo addMethod = unityUiType.GetMethod("AddUnityElement", BindingFlags.Static | BindingFlags.Public);
                if (addMethod != null)
                {
                    addMethod.Invoke(null, new object[] { unityElement });
                    Misc.Msg($"Unity element added.");
                }
                else
                {
                    Misc.Msg("AddUnityElement method not found.");
                }
            }
            catch (Exception ex)
            {
                Misc.Msg($"Error invoking AddUnityElement: {ex.Message}");
            }
        }

        public static void RemoveUnityElement(GameObject unityElement)
        {
            if (unityUiType == null)
            {
                Misc.Msg("UnityUi type is not loaded.");
                return;
            }

            try
            {
                MethodInfo removeMethod = unityUiType.GetMethod("RemoveUnityElement", BindingFlags.Static | BindingFlags.Public);
                if (removeMethod != null)
                {
                    removeMethod.Invoke(null, new object[] { unityElement });
                    Misc.Msg($"Unity element removed.");
                }
                else
                {
                    Misc.Msg("RemoveUnityElement method not found.");
                }
            }
            catch (Exception ex)
            {
                Misc.Msg($"Error invoking RemoveUnityElement: {ex.Message}");
            }
        }
    }
```

## How to Use

### Register Your UI Elements:
Use AddSUIElement("YOUR UI STRING"); to register your panel with HotKeyCommands. This ensures that HotKeyCommands will not interfere with the UI elements specified.
If use use a custom UI, you can add your UI elements to the HotKeyCommands integration to ensure that HotKeyCommands does not trigger commands while your custom UI is active.
For custom Ui's you need to trigger the

### Example:

AddSUIElement("YourCustomUI"); \
AddUnityElement(MyUiGameObject);


### Add the following line inside your game's OnGameStart() method to initialize the setup:
- `HotKeyCommandsIntegration.Setup();`

This setup will check if the `HotKeyCommands.dll` is available, and if so, it will load it and register the specified UI elements to ensure that HotKeyCommands does not trigger commands while your custom UI is active.

If you encounter any issues or have questions, feel free to reach out