using RedLoader;
using SonsSdk;

namespace WirelessSignals;

public static class Config
{
    public static ConfigCategory Category { get; private set; }

    public static ConfigEntry<bool> DebugLogging { get; private set; }
    public static ConfigEntry<bool> VisualRayCast { get; private set; }

    public static KeybindConfigEntry InteractKey { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateCategory("wirelessSignals", "WirelessSignals");

        DebugLogging = Category.CreateEntry(
            "enable_visual_raycast_wireless",
            false,
            "Enable Visual Raycast",
            "Shows RayCast Lines");

        DebugLogging = Category.CreateEntry(
            "enable_logging_wireless",
            false,
            "Enable Debug Logs",
            "Enables Debug Logs of the game to the console.");

        InteractKey = Category.CreateKeybindEntry(
            "menu_key_shops",
            "e",
            "Interact Key",
            "Interact Key (DEFAULT E).");
        InteractKey.DefaultValue = "e";
        InteractKey.Notify(() =>
        {
            UI.LinkUi.TryInteractWithUi();
        });
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}