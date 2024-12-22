using RedLoader;
using SonsSdk;

namespace Signs;

public static class Config
{
    internal static ConfigCategory IngameSignCategory { get; private set; }
    public static KeybindConfigEntry ToggleMenuKey { get; private set; }
    public static KeybindConfigEntry ExitMenuKey { get; private set; }
    public static ConfigEntry<bool> DebugLoggingIngameSign { get; private set; }
    public static ConfigEntry<bool> NetworkDebugIngameSign { get; private set; }

    public static void Init()
    {
        IngameSignCategory = ConfigSystem.CreateCategory("ingameSign", "IngameSign");
        ToggleMenuKey = IngameSignCategory.CreateKeybindEntry(
            "menu_key",
            "e",
            "Toggle Menu Key",
            "The key that toggles the Menu (DEFAULT E).");
        ToggleMenuKey.DefaultValue = "e";
        ToggleMenuKey.Notify(() =>
        {
            UI.Setup.TryOpenUi();
        });

        ExitMenuKey = IngameSignCategory.CreateKeybindEntry(
            "menu_exit_key",
            "escape",
            "Toggle Menu Key",
            "Key For Exiting Menu (DEFAULT ESACPE).");
        ExitMenuKey.DefaultValue = "escape";
        ExitMenuKey.Notify(() =>
        {
            UI.Setup.CloseUI();
        });

        DebugLoggingIngameSign = IngameSignCategory.CreateEntry(
            "enable_logging_advanced_ingameshop",
            true,
            "Enable Debug Logs",
            "Enables SimpleNetworkEvents Debug Logs of the game to the console.");
        NetworkDebugIngameSign = IngameSignCategory.CreateEntry(
            "enable_logging_advanced_network",
            true,
            "Enable Extra Network Debug Logs",
            "Enables Extra Network Debug Logs of the game to the console.");
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}