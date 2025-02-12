using RedLoader;

namespace WirelessSignals;

public static class Config
{
    public static ConfigCategory Category { get; private set; }

    public static ConfigEntry<bool> DebugLogging { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateCategory("wirelessSignals", "WirelessSignals");

        DebugLogging = Category.CreateEntry(
            "enable_logging_wireless",
            false,
            "Enable Debug Logs",
            "Enables Debug Logs of the game to the console.");
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}