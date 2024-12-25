using RedLoader;

namespace Currency;

public static class Config
{
    internal static ConfigCategory IngameCurrencyCategory { get; private set; }
    public static ConfigEntry<bool> DebugLoggingIngameCurrency { get; private set; }
    public static ConfigEntry<bool> NetworkDebugIngameCurrency { get; private set; }

    public static void Init()
    {
        IngameCurrencyCategory = ConfigSystem.CreateCategory("ingameCurrency", "IngameCurrency");

        DebugLoggingIngameCurrency = IngameCurrencyCategory.CreateEntry(
            "enable_logging_advanced_ingameCurrency",
            true,
            "Enable Debug Logs",
            "Enables SimpleNetworkEvents Debug Logs of the game to the console.");
        NetworkDebugIngameCurrency = IngameCurrencyCategory.CreateEntry(
            "enable_logging_advanced_network_ingameCurrency",
            true,
            "Enable Extra Network Debug Logs",
            "Enables Extra Network Debug Logs of the game to the console.");
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}