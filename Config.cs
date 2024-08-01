using RedLoader;

namespace SimpleNetworkEvents;

internal static class Config
{
    internal static ConfigCategory SimpleNetworkEventsCategory { get; private set; }

    public static ConfigEntry<bool> DebugLoggingSimpleNetworkEvents { get; private set; }

    internal static void Init()
    {
        SimpleNetworkEventsCategory = ConfigSystem.CreateCategory("simpleNetworkEvents", "SimpleNetworkEvents");

        DebugLoggingSimpleNetworkEvents = SimpleNetworkEventsCategory.CreateEntry(
            "enable_logging_advanced_network_events",
            false,
            "Enable Debug Logs",
            "Enables SimpleNetworkEvents Debug Logs of the game to the console.");
    }
}