using RedLoader;
using SonsSdk;

namespace LogSledAutoPickup;

public static class Config
{
    public static ConfigCategory Category { get; private set; }
    public static KeybindConfigEntry CyclePickup { get; private set; }
    public static ConfigEntry<bool> Enabled { get; private set; }
    public static ConfigEntry<bool> EnableLogging { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("LogSledAutoPickup", "LogSledAutoPickup", "LogSledAutoPickup.cfg");

        CyclePickup = Category.CreateKeybindEntry(
            "logsled_pickup_cycle_key",
            "c",
            "Cycle Between Pickups",
            "Switch between witch item the logsled should auto pickup (DEFAULT C).");
        CyclePickup.DefaultValue = "c";
        CyclePickup.Notify(() =>
        {
            if (Enabled.Value == false)
            {
                return;
            }
            LogSledTools.CycleValue();
        });
        Enabled = Category.CreateEntry(
            "logsled_pickup_enable",
            true,
            "Enable Mod",
            "If Checked Mod Is Enabled (DEFAULT CHECKED)");
        Enabled.DefaultValue = true;

        EnableLogging = Category.CreateEntry(
            "logsled_pickup_logging_console",
            false,
            "Enable Logging To Console",
            "If Checked Logging To Console Is Enabled (DEFAULT OFF)");
        EnableLogging.DefaultValue = false;
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
        // Check if the mod is enabled or disabled

    }
}