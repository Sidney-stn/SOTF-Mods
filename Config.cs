using RedLoader;
using SonsSdk;

namespace StoneGate;

public static class Config
{
    public static ConfigCategory Category { get; private set; }
    public static ConfigEntry<bool> LoggingToConsole { get; private set; }
    public static KeybindConfigEntry PrimaryAction { get; private set; }
    public static KeybindConfigEntry CycleAction { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("StoneGate", "StoneGate", "StoneGate.cfg");

        LoggingToConsole = Category.CreateEntry(
            "stone_gate_logging",
            false,
            "Enable Console Logs",
            "Enable Console Logs To Console");
        LoggingToConsole.DefaultValue = false;

        PrimaryAction = Category.CreateKeybindEntry(
            "stone_gate_primary",
            "<Mouse>/leftButton",
            "Hit Tool Key",
            "Key that makes the Gate Tool useable (DEFAULT Left Click).");
        PrimaryAction.DefaultValue = "<Mouse>/leftButton";
        PrimaryAction.Notify(() =>
        {
            Objects.ActiveItem.OnKeyPress();
        });

        CycleAction = Category.CreateKeybindEntry(
            "stone_gate_cycle",
            "c",
            "Change Tool Mode",
            "Key changes tool mode (DEFAULT C).");
        CycleAction.DefaultValue = "c";
        CycleAction.Notify(() =>
        {
            Objects.UiController.ChangeMode();
        });
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}