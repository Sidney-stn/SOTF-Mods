using RedLoader;
using Sons.Gui;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;

namespace StoneGate;

public static class Config
{
    public static ConfigCategory Category { get; private set; }
    public static ConfigEntry<bool> LoggingToConsole { get; private set; }
    public static KeybindConfigEntry PrimaryAction { get; private set; }
    public static KeybindConfigEntry CycleAction { get; private set; }
    public static KeybindConfigEntry FinishAction { get; private set; }

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

        FinishAction = Category.CreateKeybindEntry(
            "stone_gate_finish",
            "e",
            "Open-Close Door / Finish Gate Key",
            "Open-Close Door /Finish Gate Key (DEFAULT E) NOTE: Finish UI Key Does Not Update, but key works");
        FinishAction.DefaultValue = "e";
        FinishAction.Notify(() =>
        {
            if (LocalPlayer.IsInWorld == false || LocalPlayer.IsInInventory || PauseMenu.IsActive || LocalPlayer.InWater) { return; }
            if (Objects.ActiveItem.active != null)
            {
                Objects.ActiveItem.active.Complete();
                if (Testing.Settings.logOnFinishOpenCloseDoorKey)
                {
                    Misc.Msg($"[Config] [FinishAction] [Notify] ActiveItem.active.Complete()");
                }
            } 
            else
            {
                // Check LINKUI
                GameObject storedParent = Objects.CreateGateParent.Instance.StoredParent;
                if (storedParent == null) { if (Testing.Settings.logOnFinishOpenCloseDoorKey) { Misc.Msg($"[Config] [CheckLinkui] [Notify] StoredParent IS NULL"); } return; }
                List<Transform> tempChildTransforms = storedParent.GetChildren();
                if (Testing.Settings.logOnFinishOpenCloseDoorKey) { Misc.Msg($"[Config] [CheckLinkui] [Notify] TempChildTransforms.Count: {tempChildTransforms.Count}"); }
                foreach (Transform t in tempChildTransforms)
                {
                    var comp = t.GetComponent<Mono.StoneGateStoreMono>();
                    if (comp != null)
                    {
                        if (comp.LinkUiElement != null)
                        {
                            if (comp.LinkUiElement.IsActive)
                            {
                                if (Testing.Settings.logOnFinishOpenCloseDoorKey) { Misc.Msg($"[Config] [CheckLinkui] [Notify] LinkUiElement.IsActive"); }
                                // Open Or Close Gate Code
                                comp.ToggleGate();
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (Testing.Settings.logOnFinishOpenCloseDoorKey) { Misc.Msg($"[Config] [CheckLinkui] [Notify] StoneGateStoreMono comp IS NULL"); }
                    }
                }
            }
        });
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }

    public static HashSet<string> allowedHits = new HashSet<string>()
        {
            "RockWall",
            "RockPilar",
            "RockBeam"
        };
}