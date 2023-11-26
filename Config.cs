using RedLoader;

namespace SimpleNetworkEvents;

internal static class Config
{
    internal static ConfigCategory Category { get; private set; }

    //public static ConfigEntry<bool> SomeEntry { get; private set; }

    internal static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("SimpleNetworkEvents", "SimpleNetworkEvents", "SimpleNetworkEvents.cfg");

        // SomeEntry = Category.CreateEntry(
        //     "some_entry",
        //     true,
        //     "Some entry",
        //     "Some entry that does some stuff.");
    }
}