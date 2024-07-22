using RedLoader;

namespace StructureDamageViewer;

public static class Config
{
    public static ConfigCategory StructureDamageViewerCategory { get; private set; }

    public static ConfigEntry<bool> StructureDamageViewerLogging { get; private set; }

    public static void Init()
    {
        StructureDamageViewerCategory = ConfigSystem.CreateCategory("structureDamageViewerCategory", "StructureDamageViewerCategory");

        StructureDamageViewerLogging = StructureDamageViewerCategory.CreateEntry(
            "logging_structure_damage",
            true,
            "Enable Logging",
            "Enable Logging Statements To Console");
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}