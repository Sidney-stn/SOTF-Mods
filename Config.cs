using RedLoader;

namespace BroadcastMessage;

public static class Config
{
    public static ConfigCategory Category { get; private set; }

    public static ConfigEntry<bool> EnableFiveSecondPrinting { get; private set; }
    public static ConfigEntry<bool> EnableLogging { get; private set; }

    public static ConfigEntry<bool> CheckNamePrinting { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("BroadcastMessage", "BroadcastMessage", "BroadcastMessage.cfg");

        EnableFiveSecondPrinting = Category.CreateEntry(
            "enable_five_second_printing_broadcast",
            true,
            "Enable Five Second Printing",
            "Enable The Corutine That Runs Every 5 Seconds");

        EnableLogging = Category.CreateEntry(
           "enable_logging_broadcast",
           true,
           "Enable Logging To Console",
           "Enable Logging Of Logging Statements To The Console");

        CheckNamePrinting = Category.CreateEntry(
           "enable_check_name_broadcast",
           true,
           "Enable Logging To Console",
           "Enable Logging Of Logging Statements To The Console");
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}