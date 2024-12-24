using RedLoader;
using SonsSdk;

namespace Warps;

public static class Config
{
    internal static ConfigCategory IngameWarpsCategory { get; private set; }
    public static KeybindConfigEntry ToggleMenuKey { get; private set; }
    public static KeybindConfigEntry ExitMenuKey { get; private set; }
    public static ConfigEntry<bool> DebugLoggingIngameWarps { get; private set; }
    public static ConfigEntry<bool> NetworkDebugIngameSign { get; private set; }

    public static void Init()
    {
        IngameWarpsCategory = ConfigSystem.CreateCategory("ingameWarps", "IngameWarps");
        ToggleMenuKey = IngameWarpsCategory.CreateKeybindEntry(
            "menu_key_warps",
            "numpad1",
            "Toggle Menu Key",
            "The key that toggles the Menu (DEFAULT Numpad1).");
        ToggleMenuKey.DefaultValue = "numpad1";
        ToggleMenuKey.Notify(() =>
        {
            UI.Setup.TryOpenUi();
        });

        ExitMenuKey = IngameWarpsCategory.CreateKeybindEntry(
            "menu_exit_key_warps",
            "escape",
            "Toggle Menu Key",
            "Key For Exiting Menu (DEFAULT ESACPE).");
        ExitMenuKey.DefaultValue = "escape";
        ExitMenuKey.Notify(() =>
        {
            UI.Setup.CloseUI();
        });

        DebugLoggingIngameWarps = IngameWarpsCategory.CreateEntry(
            "enable_logging_advanced_warps",
            true,
            "Enable Debug Logs",
            "Enables SimpleNetworkEvents Debug Logs of the game to the console.");
        NetworkDebugIngameSign = IngameWarpsCategory.CreateEntry(
            "enable_logging_advanced_network",
            true,
            "Enable Extra Network Debug Logs",
            "Enables Extra Network Debug Logs of the game to the console.");
    }
}