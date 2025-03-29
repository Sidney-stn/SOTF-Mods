using Sons.Gui.Input;
using UnityEngine;

namespace SimpleElevator.Mono
{
    internal class ElevatorControlPanelMono : MonoBehaviour
    {
        public bool isSetupPrefab = false;
        public LinkUiElement LinkUi { get; private set; }

        // Reference to the closest elevator
        private GameObject closestElevator;
        private float maxElevatorSearchDistance = 20f;

        private void Start()
        {
            if (isSetupPrefab) { return; }
            if (LinkUi == null)
            {
                LinkUi = Tools.LinkUi.CreateLinkUi(gameObject.transform.GetChild(1).gameObject, 1.5f, null, Assets.Instance.LinkUiIcon, null);
            }
            Objects.Track.ElevatorControlPanels.Add(gameObject);
        }

        public void InvokePrimaryAction()
        {
            // Find the closest elevator to call
            FindClosestElevator();

            if (closestElevator == null)
            {
                // If we're in network play, send a call event to server
                if (BoltNetwork.isRunning && BoltNetwork.isClient)
                {
                    BoltEntity entity = GetComponent<BoltEntity>();
                    if (entity != null)
                    {
                        if (entity.isAttached == false)
                        {
                            Misc.Msg("[ElevatorControlPanelMono] Entity is not attached", true);
                            return;
                        }
                        Misc.Msg("[ElevatorControlPanelMono] Sending call elevator event to server", true);
                        Network.ElevatorControlPanelSyncEvent.SendState(entity, Network.ElevatorControlPanelSyncEvent.ElevatorControlPanelSyncType.CallElevator);
                    }
                }
                return;
            }

            // Get the elevator's component
            ElevatorMono elevatorMono = closestElevator.GetComponent<ElevatorMono>();
            if (elevatorMono == null)
            {
                Misc.Msg("[ElevatorControlPanelMono] Found elevator but no ElevatorMono component", true);
                return;
            }

            // Calculate if we need to move the elevator up or down to this panel
            if (transform.position.y > elevatorMono.transform.position.y)
            {
                // Control panel is above elevator - move up
                Misc.Msg("[ElevatorControlPanelMono] Calling elevator to move up to this panel", true);
                elevatorMono.MoveUp();
            }
            else if (transform.position.y < elevatorMono.transform.position.y)
            {
                // Control panel is below elevator - move down
                Misc.Msg("[ElevatorControlPanelMono] Calling elevator to move down to this panel", true);
                elevatorMono.MoveDown();
            }
            else
            {
                // Elevator is already at this level
                Misc.Msg("[ElevatorControlPanelMono] Elevator is already at this level", true);
                // Optionally: elevatorMono.ReturnToGroundFloor();
            }
        }

        private void FindClosestElevator()
        {
            closestElevator = null;
            float closestDistance = maxElevatorSearchDistance;

            foreach (GameObject elevator in Objects.Track.Elevators)
            {
                if (elevator != null)
                {
                    float distance = Vector3.Distance(transform.position, elevator.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestElevator = elevator;
                    }
                }
            }

            if (closestElevator != null)
            {
                Misc.Msg($"[ElevatorControlPanelMono] Found closest elevator at distance {closestDistance}", true);
            }
            else
            {
                Misc.Msg("[ElevatorControlPanelMono] No elevator found within range", true);
            }
        }

        private void OnDestroy()
        {
            Objects.Track.ElevatorControlPanels.Remove(gameObject);
        }
    }
}