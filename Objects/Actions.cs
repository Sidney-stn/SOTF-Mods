using Sons.Gui;
using System;
using TheForest.Utils;


namespace SimpleElevator.Objects
{
    internal class Actions
    {
        internal static void OnPrimaryAction()
        {
            if (!LocalPlayer.IsInWorld || LocalPlayer.IsInInventory || LocalPlayer.IsInGolfCart || LocalPlayer.IsInCaves || LocalPlayer.IsInMidAction || LocalPlayer.IsConstructing || LocalPlayer.IsGliding || LocalPlayer.IsConstructing || PauseMenu.IsActive) { return; }
            bool found = false;
            if (Objects.Track.Elevators.Count > 0 )
            {
                foreach (var elevator in Objects.Track.Elevators)
                {
                    if (elevator == null) { continue; }
                    var component = elevator.GetComponent<Mono.ElevatorMono>();
                    if (component == null) { continue; }
                    if (component.isSetupPrefab) { continue; }
                    if (component.LinkUi == null) { continue; }
                    if (component.LinkUi.IsActive)
                    {
                        found = true;
                        component.InvokePrimaryAction();  // Call the primary action.
                        return;
                    }
                }
            }
            if (found == false && Objects.Track.ElevatorControlPanels.Count > 0)
            {
                foreach (var controlPanel in Objects.Track.ElevatorControlPanels)
                {
                    if (controlPanel == null) { continue; }
                    var component = controlPanel.GetComponent<Mono.ElevatorControlPanelMono>();
                    if (component == null) { continue; }
                    if (component.isSetupPrefab) { continue; }
                    if (component.LinkUi == null) { continue; }
                    if (component.LinkUi.IsActive)
                    {
                        found = true;
                        component.InvokePrimaryAction();  // Call the primary action.
                        return;
                    }
                }
            }
        }

        internal static void OnScrollUp()
        {
            if (!LocalPlayer.IsInWorld || LocalPlayer.IsInInventory || LocalPlayer.IsInGolfCart || LocalPlayer.IsInCaves || LocalPlayer.IsInMidAction || LocalPlayer.IsConstructing || LocalPlayer.IsGliding || LocalPlayer.IsConstructing || PauseMenu.IsActive) { return; }
            if (Objects.Track.Elevators.Count > 0)
            {
                foreach (var elevator in Objects.Track.Elevators)
                {
                    if (elevator == null) { continue; }
                    var component = elevator.GetComponent<Mono.ElevatorMono>();
                    if (component == null) { continue; }
                    if (component.isSetupPrefab) { continue; }
                    if (component.LinkUi == null) { continue; }
                    if (component.LinkUi.IsActive)
                    {
                        component.OnScrollUp();
                        return;
                    }
                }
            }
        }
        internal static void OnScrollDown()
        {
            if (!LocalPlayer.IsInWorld || LocalPlayer.IsInInventory || LocalPlayer.IsInGolfCart || LocalPlayer.IsInCaves || LocalPlayer.IsInMidAction || LocalPlayer.IsConstructing || LocalPlayer.IsGliding || LocalPlayer.IsConstructing || PauseMenu.IsActive) { return; }
            if (Objects.Track.Elevators.Count > 0)
            {
                foreach (var elevator in Objects.Track.Elevators)
                {
                    if (elevator == null) { continue; }
                    var component = elevator.GetComponent<Mono.ElevatorMono>();
                    if (component == null) { continue; }
                    if (component.isSetupPrefab) { continue; }
                    if (component.LinkUi == null) { continue; }
                    if (component.LinkUi.IsActive)
                    {
                        component.OnScrollDown();
                        return;
                    }
                }
            }
        }
    }
}
