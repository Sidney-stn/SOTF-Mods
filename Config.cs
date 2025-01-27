using RedLoader;
using SonsSdk;
using UnityEngine;

namespace Signs;

public static class Config
{
    internal static ConfigCategory IngameSignCategory { get; private set; }
    public static KeybindConfigEntry ToggleMenuKey { get; private set; }
    public static KeybindConfigEntry ExitMenuKey { get; private set; }
    public static KeybindConfigEntry RotateLeftKey { get; private set; }
    public static KeybindConfigEntry RotateRightKey { get; private set; }
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
            "Exit Menu Key",
            "Key For Exiting Menu (DEFAULT ESACPE).");
        ExitMenuKey.DefaultValue = "escape";
        ExitMenuKey.Notify(() =>
        {
            UI.Setup.CloseUI();
        });

        RotateLeftKey = IngameSignCategory.CreateKeybindEntry(
            "rotate_left_key_signs",
            "q",
            "Rotate Left Key",
            "The key rotates the sign to the left when placing (DEFAULT Q).");
        RotateLeftKey.DefaultValue = "q";

        RotateRightKey = IngameSignCategory.CreateKeybindEntry(
            "rotate_right_key_signs",
            "e",
            "Rotate Right Key",
            "The key rotates the sign to the right when placing (DEFAULT E).");
        RotateRightKey.DefaultValue = "e";

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
        UI.Setup.UpdateUiOpenKey();

        // Update Placement Keys
        if (RotateLeftKey.Value != null && RotateRightKey.Value != null && ExitMenuKey.Value != null)
        {
            Misc.Msg("Update Placement Keys");
            Items.ItemPlacement.rotateLeftKey = Items.ItemPlacement.TryParseKeyCode(RotateLeftKey.Value, KeyCode.Q);
            Items.ItemPlacement.rotateRightKey = Items.ItemPlacement.TryParseKeyCode(RotateRightKey.Value, KeyCode.E);
            Items.ItemPlacement.cancelPlacementKey = Items.ItemPlacement.TryParseKeyCode(ExitMenuKey.Value, KeyCode.Escape);
            UI.SetupSignPlace.UpdateKeysInUI(RotateLeftKey.Value.ToUpper(), RotateRightKey.Value.ToUpper());
        }
        else
        {
            // Set Default Placement Keys
            Misc.Msg("Set Default Placement Keys");
            Items.ItemPlacement.rotateLeftKey = KeyCode.Q;
            Items.ItemPlacement.rotateRightKey = KeyCode.E;
            Items.ItemPlacement.cancelPlacementKey = KeyCode.Escape;
            UI.SetupSignPlace.UpdateKeysInUI("Q", "E");
        }

    }
}