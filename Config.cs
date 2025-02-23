using RedLoader;
using SonsSdk;

namespace WirelessSignals;

public static class Config
{
    public static ConfigCategory Category { get; private set; }

    public static ConfigEntry<bool> DebugLogging { get; private set; }
    public static ConfigEntry<bool> VisualRayCast { get; private set; }
    public static KeybindConfigEntry CloseUiKey { get; private set; }
    public static KeybindConfigEntry InteractKey { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateCategory("wirelessSignals", "WirelessSignals");

        VisualRayCast = Category.CreateEntry(
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
            "menu_key_wireless",
            "e",
            "Interact Key",
            "Interact Key (DEFAULT E).");
        InteractKey.DefaultValue = "e";
        InteractKey.Notify(() =>
        {
            UI.LinkUi.TryInteractWithUi();
        });

        CloseUiKey = Category.CreateKeybindEntry(
            "esc_key_wireless",
            "escape",
            "Ui Close",
            "Close Open Ui Key (DEFAULT ESC).");
        CloseUiKey.DefaultValue = "e";
        CloseUiKey.Notify(() =>
        {
            if (UI.ReciverUI.UiElement != null)
            {
                if (UI.ReciverUI.UiElement.active)
                {
                    UI.ReciverUI.UiElement.SetActive(false);
                }
            }
        });
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}