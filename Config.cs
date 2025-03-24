using RedLoader;
using SonsSdk;

namespace BuildingMagnet;

public static class Config
{
    public static ConfigCategory Category { get; private set; }

    public static KeybindConfigEntry CyclePickup { get; private set; }
    public static ConfigEntry<bool> Enabled { get; private set; }
    public static ConfigEntry<float> MagnetRange { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("BuildingMagnet", "BuildingMagnet", "BuildingMagnet.cfg");

        CyclePickup = Category.CreateKeybindEntry(
            "build_magnet_cycle_key",
            "c",
            "Cycle Between What To Magnet",
            "Switch between witch item to magnet (DEFAULT C).");
        CyclePickup.DefaultValue = "c";
        CyclePickup.Notify(() =>
        {
            if (Enabled.Value == false)
            {
                return;
            }
            MagnetValues.CycleValue();
        });
        MagnetRange = Category.CreateEntry(
            "build_magnet_range",
            10f,
            "Magnet Range",
            "The range of the magnet (DEFAULT 10).");
        MagnetRange.SetRange(0f, 20f);
        MagnetRange.DefaultValue = 10f;
        Enabled = Category.CreateEntry(
            "building_magnet_enabled",
            true,
            "Enable Mod",
            "If Checked Mod Is Enabled (DEFAULT CHECKED)");
        Enabled.DefaultValue = true;
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}