using RedLoader;
using SonsSdk;

namespace Shops;

public static class Config
{
    internal static ConfigCategory IngameShopsCategory { get; private set; }
    public static ConfigEntry<bool> DebugLoggingIngameShops { get; private set; }
    public static ConfigEntry<bool> NetworkDebugIngameShops { get; private set; }
    public static KeybindConfigEntry ToggleMenuKey { get; private set; }

    internal static void Init()
    {
        IngameShopsCategory = ConfigSystem.CreateCategory("ingameShops", "IngameShops");

        DebugLoggingIngameShops = IngameShopsCategory.CreateEntry(
            "enable_logging_advanced_ingameShops",
            true,
            "Enable Debug Logs",
            "Enables SimpleNetworkEvents Debug Logs of the game to the console.");
        NetworkDebugIngameShops = IngameShopsCategory.CreateEntry(
            "enable_logging_advanced_network_ingameShops",
            true,
            "Enable Extra Network Debug Logs",
            "Enables Extra Network Debug Logs of the game to the console.");

        ToggleMenuKey = IngameShopsCategory.CreateKeybindEntry(
            "menu_key_shops",
            "e",
            "Interact With Shop Key",
            "The key that Interact with the shop (DEFAULT E).");
        ToggleMenuKey.DefaultValue = "e";
        ToggleMenuKey.Notify(() =>
        {
            Prefab.SingeShop.TryOpenUi();
        });

    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}