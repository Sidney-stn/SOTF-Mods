using RedLoader;
using SonsSdk;

namespace SimpleElevator;

public static class Config
{
    public static ConfigCategory Category { get; private set; }

    public static KeybindConfigEntry PrimaryAction { get; private set; }
    public static ConfigEntry<bool> LoggingToConsole { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("SimpleElevator", "SimpleElevator", "SimpleElevator.cfg");

        PrimaryAction = Category.CreateKeybindEntry(
            "simple_elevator_enter",
            "e",
            "Confirm Key",
            "Confirm Key Changing Floors, Calling Elevator (DEFAULT E).");
        PrimaryAction.DefaultValue = "e";
        PrimaryAction.Notify(() =>
        {
            Objects.Actions.OnPrimaryAction();
        });

        LoggingToConsole = Category.CreateEntry(
            "simple_elevator_logging",
            true,
            "Log To Console",
            "Log Into RedLoader Console");
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}