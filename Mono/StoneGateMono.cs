using UnityEngine;

namespace StoneGate.Mono
{
    internal class StoneGateMono : MonoBehaviour
    {
        public bool isSetupPrefab = false;
        private bool _gateOpen = false;

        private void Start()
        {
            if (isSetupPrefab == true) { return; }
            if (gameObject.transform.position == Vector3.zero) { isSetupPrefab = true; return; }

            // Register In Saving System
            Objects.Track.spawendStoneGates.Add(gameObject.GetComponent<BoltEntity>(), gameObject);
            Misc.Msg($"[StoneGateMono] [Start] Added {gameObject.name} to Track.spawendStoneGates");
        }

        public bool IsGateOpen()
        {
            return _gateOpen;
        }

        public void OpenGate(bool raiseNetwork = true)
        {
            if (_gateOpen)
            {
                return;
            }
            _gateOpen = true;
            // Open gate
        }

        public void CloseGate(bool raiseNetwork = true)
        {
            if (!_gateOpen)
            {
                return;
            }
            _gateOpen = false;
            // Close gate
        }
    }
}
