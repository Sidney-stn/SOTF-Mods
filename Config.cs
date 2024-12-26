using RedLoader;
using Sons.Gui;
using SonsSdk;

namespace Banking;

public static class Config
{
    internal static ConfigCategory IngameBankingCategory { get; private set; }
    public static ConfigEntry<bool> DebugLoggingIngameBanking { get; private set; }
    public static ConfigEntry<bool> NetworkDebugIngameBanking { get; private set; }
    public static KeybindConfigEntry ToggleMenuKey { get; private set; }
    public static KeybindConfigEntry ExitMenuKey { get; private set; }

    public static void Init()
    {
        IngameBankingCategory = ConfigSystem.CreateCategory("ingameBanking", "IngameBanking");

        DebugLoggingIngameBanking = IngameBankingCategory.CreateEntry(
            "enable_logging_advanced_ingameBanking",
            true,
            "Enable Debug Logs",
            "Enables SimpleNetworkEvents Debug Logs of the game to the console.");
        NetworkDebugIngameBanking = IngameBankingCategory.CreateEntry(
            "enable_logging_advanced_network_ingameBanking",
            true,
            "Enable Extra Network Debug Logs",
            "Enables Extra Network Debug Logs of the game to the console.");

        ToggleMenuKey = IngameBankingCategory.CreateKeybindEntry(
            "menu_key_banking",
            "e",
            "Toggle Menu Key",
            "The key that toggles the Menu (DEFAULT E).");
        ToggleMenuKey.DefaultValue = "e";
        ToggleMenuKey.Notify(() =>
        {
            if (!UI.Setup.IsUiOpen())
            {
                UI.FunctionsFromUI.TryOpenUi();
            }
            else
            {
                UI.Setup.CloseUI();
                if (PauseMenu._instance != null)
                {
                    PauseMenu._instance.Close();
                }
            }
        });

        ExitMenuKey = IngameBankingCategory.CreateKeybindEntry(
            "menu_exit_key_banking",
            "escape",
            "Toggle Menu Key",
            "Key For Exiting Menu (DEFAULT ESACPE).");
        ExitMenuKey.DefaultValue = "escape";
        ExitMenuKey.Notify(() =>
        {
            UI.Setup.CloseUI();
        });
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}