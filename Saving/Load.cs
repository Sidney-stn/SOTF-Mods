

using RedLoader;
using SonsSdk;
using System.Numerics;
using UnityEngine;

namespace SimpleElevator.Saving
{
    internal class Load
    {
        internal static Queue<Saving.Manager.ElevatorManager> deferredLoadQueue = new Queue<Saving.Manager.ElevatorManager>();

        /// <summary>
        /// Process the deferred load queue, this is used to load the gates after the world has been loaded
        /// </summary>
        /// <param name="obj"></param>
        internal static void ProcessLoadData(Saving.Manager.ElevatorManager obj)
        {
            if (BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                if (Settings.logSavingSystem)
                    Misc.Msg("[Loading] Skipped Loading StoneGates On Multiplayer Client");
                return;
            }
            if (Settings.logSavingSystem)
            {
                // Check if the gates data is null
                if (obj == null)
                {
                    Misc.Msg("[Loading] ElevatorManager IS NULL");
                    return;
                }
                Misc.Msg($"[Loading] Elevators From Save: {obj.Elevators.Count.ToString()}");
                Misc.Msg($"[Loading] Control Panels From Save: {obj.ControlPanels.Count.ToString()}");
            }

            foreach (var elevator in obj.Elevators)
            {
                if (elevator == null)
                {
                    RLog.Error("[SimpleElevator] [Loading] Elevator IS NULL");
                }
                if (BoltNetwork.isRunning && BoltNetwork.isClient)
                {
                    if (Settings.logSavingSystem)
                        Misc.Msg("[Loading] Skipped Loading Elevator On Multiplayer Client");
                    return;
                }
                else if (BoltNetwork.isRunning && BoltNetwork.isServer)
                {
                    if (Settings.logSavingSystem)
                        Misc.Msg("[Loading] Loading Elevator On Multiplayer Server");
                    GameObject elevatorGo = BoltNetwork.Instantiate(SimpleElevator.Instance.ElevatorInstance.SetupGameObject, elevator.Position, elevator.Rotation);
                    if (elevatorGo == null)
                    {
                        RLog.Error("[SimpleElevator] [Loading] ElevatorGo IS NULL");
                        SonsTools.ShowMessage("Something went worng when loading in elevators");
                    }
                    //elevatorGo.hideFlags = HideFlags.None;
                }
                else if (!BoltNetwork.isRunning)
                {
                    if (Settings.logSavingSystem)
                        Misc.Msg("[Loading] Loading Elevator On Singleplayer");
                    GameObject elevatorGo = GameObject.Instantiate(SimpleElevator.Instance.ElevatorInstance.SetupGameObject, elevator.Position, elevator.Rotation);
                    if (elevatorGo == null)
                    {
                        RLog.Error("[SimpleElevator] [Loading] ElevatorGo IS NULL");
                        SonsTools.ShowMessage("Something went worng when loading in elevators");
                    }
                    //elevatorGo.hideFlags = HideFlags.None;
                }
            }

            foreach (var controlPanel in obj.ControlPanels)
            {
                if (controlPanel == null)
                {
                    RLog.Error("[SimpleElevator] [Loading] ControlPanel IS NULL");
                }
                if (BoltNetwork.isRunning && BoltNetwork.isClient)
                {
                    if (Settings.logSavingSystem)
                        Misc.Msg("[Loading] Skipped Loading ControlPanel On Multiplayer Client");
                    return;
                }
                else if (BoltNetwork.isRunning && BoltNetwork.isServer)
                {
                    if (Settings.logSavingSystem)
                        Misc.Msg("[Loading] Loading ControlPanel On Multiplayer Server");
                    if (SimpleElevator.Instance.ElevatorControlPanelInstace.SetupGameObject == null)
                    {
                        RLog.Error("[SimpleElevator] [Loading] ElevatorControlPanelInstace IS NULL");
                    }
                    GameObject controlPanelGo = BoltNetwork.Instantiate(SimpleElevator.Instance.ElevatorControlPanelInstace.SetupGameObject, controlPanel.Position, controlPanel.Rotation);
                    if (controlPanelGo == null)
                    {
                        RLog.Error("[SimpleElevator] [Loading] ControlPanelGo IS NULL");
                        SonsTools.ShowMessage("Something went worng when loading in control panels");
                    }
                    //controlPanelGo.hideFlags = HideFlags.None;
                }
                else if (!BoltNetwork.isRunning)
                {
                    if (Settings.logSavingSystem)
                        Misc.Msg("[Loading] Loading ControlPanel On Singleplayer");
                    if (SimpleElevator.Instance.ElevatorControlPanelInstace.SetupGameObject == null)
                    {
                        RLog.Error("[SimpleElevator] [Loading] ElevatorControlPanelInstace IS NULL");
                    }
                    GameObject controlPanelGo = GameObject.Instantiate(SimpleElevator.Instance.ElevatorControlPanelInstace.SetupGameObject, controlPanel.Position, controlPanel.Rotation);
                    if (controlPanelGo == null)
                    {
                        RLog.Error("[SimpleElevator] [Loading] ControlPanelGo IS NULL");
                        SonsTools.ShowMessage("Something went worng when loading in control panels");
                    }
                    //controlPanelGo.hideFlags = HideFlags.None;
                }
            }
        }
    }
}
