using RedLoader;

namespace StoneGate;

public static class Config
{
    public static ConfigCategory Category { get; private set; }
    public static ConfigEntry<bool> LoggingToConsole { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("StoneGate", "StoneGate", "StoneGate.cfg");

        LoggingToConsole = Category.CreateEntry(
            "stone_gate_logging",
            false,
            "Enable Console Logs",
            "Enable Console Logs To Console");
        LoggingToConsole.DefaultValue = false;
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}