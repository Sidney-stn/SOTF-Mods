using SimpleElevator.Structure;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;

namespace SimpleElevator.Saving
{
    internal class Manager : ICustomSaveable<Manager.ElevatorManager>
    {
        public string Name => "ElevatorManager";

        // Used to determine if the data should also be saved in multiplayer saves
        public bool IncludeInPlayerSave => false;
        public ElevatorManager Save()
        {
            if (BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                return null;
            }

            var saveData = new ElevatorManager();

            int savedElevators = 0;
            // Get All Stored Elevators
            var elevators = Objects.Track.Elevators;
            if (elevators == null)
            {
                if (Settings.logSavingSystem)
                    Misc.Msg("[Saving] No Elevators Found");
            }
            else if (elevators.Count > 0)
            {
                if (Settings.logSavingSystem)
                    Misc.Msg($"[Saving] Found {elevators.Count} Elevators");
                foreach (var elevator in elevators)
                {
                    if (elevator == null)
                    {
                        if (Settings.logSavingSystem)
                            Misc.Msg("[Saving] Elevator IS NULL");
                        continue;
                    }
                    var elevatorManager = elevator.GetComponent<Mono.ElevatorMono>();
                    if (elevatorManager == null)
                    {
                        if (Settings.logSavingSystem)
                            Misc.Msg("[Saving] ElevatorMono IS NULL");
                        continue;
                    }
                    if (elevatorManager.isSetupPrefab == true)
                    {
                        if (Settings.logSavingSystem)
                            Misc.Msg("[Saving] ElevatorMono isSetupPrefab IS TRUE");
                        continue;
                    }
                    if (elevator.transform.position == Vector3.zero)
                    {
                        if (Settings.logSavingSystem)
                            Misc.Msg("[Saving] Elevator Position IS ZERO");
                        continue;
                    }
                    saveData.Elevators.Add(new ElevatorManager.ElevatorModData
                    {
                        Position = elevator.transform.position,
                        Rotation = elevator.transform.rotation
                    });
                    savedElevators++;
                }
            }

            int savedPanels = 0;
            // Get All Stored Elevators
            var panels = Objects.Track.ElevatorControlPanels;
            if (panels == null)
            {
                if (Settings.logSavingSystem)
                    Misc.Msg("[Saving] No Panels Found");
            }
            else if (panels.Count > 0)
            {
                if (Settings.logSavingSystem)
                    Misc.Msg($"[Saving] Found {panels.Count} Panels");
                foreach (var panel in panels)
                {
                    if (panel == null)
                    {
                        if (Settings.logSavingSystem)
                            Misc.Msg("[Saving] Panels IS NULL");
                        continue;
                    }
                    var elevatorPanel = panel.GetComponent<Mono.ElevatorControlPanelMono>();
                    if (elevatorPanel == null)
                    {
                        if (Settings.logSavingSystem)
                            Misc.Msg("[Saving] ElevatorControlPanelMono IS NULL");
                        continue;
                    }
                    if (elevatorPanel.isSetupPrefab == true)
                    {
                        if (Settings.logSavingSystem)
                            Misc.Msg("[Saving] ElevatorControlPanelMono isSetupPrefab IS TRUE");
                        continue;
                    }
                    if (panel.transform.position == Vector3.zero)
                    {
                        if (Settings.logSavingSystem)
                            Misc.Msg("[Saving] Elevator Position IS ZERO");
                        continue;
                    }
                    saveData.ControlPanels.Add(new ElevatorManager.ElevatorModData
                    {
                        Position = panel.transform.position,
                        Rotation = panel.transform.rotation
                    });
                    savedPanels++;
                }
            }
            Misc.Msg($"[Saving] Saved {savedElevators} Elevators");
            Misc.Msg($"[Saving] Saved {savedPanels} Panels");

            return saveData;
        }

        public void Load(ElevatorManager obj)
        {
            if (!LocalPlayer.IsInWorld)
            {
                // Enqueue the load data if host mode is not ready
                Misc.Msg("[Loading] Host mode not ready, deferring load.");
                Saving.Load.deferredLoadQueue.Enqueue(obj);
                return;
            }

            // Process the load data
            Saving.Load.ProcessLoadData(obj);
        }

        public class ElevatorManager
        {
            public List<ElevatorModData> Elevators = new List<ElevatorModData>();
            public List<ElevatorModData> ControlPanels = new List<ElevatorModData>();

            public class ElevatorModData
            {
                public Vector3 Position;
                public Quaternion Rotation;
            }

        }
    }
}
