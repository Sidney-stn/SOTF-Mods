

using UnityEngine;

namespace SimpleElevator.Objects
{
    internal static class Track
    {
        internal static HashSet<GameObject> Elevators = new HashSet<GameObject>();
        internal static HashSet<GameObject> ElevatorControlPanels = new HashSet<GameObject>();
    }
}
