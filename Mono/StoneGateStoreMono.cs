using RedLoader;
using Sons.Gui.Input;
using StoneGate.Objects;
using System.Collections;
using UnityEngine;

namespace StoneGate.Mono
{
    internal class StoneGateStoreMono : MonoBehaviour
    {

        private GameObject _rotateGo;  // Rotate GameObject
        private Objects.CreateGateParent.RotateMode _rotateMode;  // Mode For Rotation of Rotate GameObject
        private GameObject _floorBeam;
        private GameObject _topBeam;
        private GameObject _rockWall;
        private GameObject _extraPillar;

        private HashSet<GameObject> objectsToRotate = new HashSet<GameObject>(new Objects.GameObjectInstanceIDComparer());
        private Dictionary<string, GameObject> namesOfAllGo = new Dictionary<string, GameObject>();

        //// THINGS WITH ROTATION
        // Store original rotations for each object
        private Dictionary<int, Quaternion> originalRotations = new Dictionary<int, Quaternion>();

        // Animation parameters
        private bool isAnimating = false;
        private float animationDuration = 1.0f; // Duration of the gate animation in seconds
        //// THINGS WITH ROTATION END

        public LinkUiElement LinkUiElement { get; private set; }

        private bool _gateOpen = false;

        public void Init(GameObject rotationGo,
            Objects.CreateGateParent.RotateMode mode,
            GameObject floorBeam = null,
            GameObject topBeam = null,
            GameObject rockWall = null,
            GameObject extraPillar = null
            )
        {
            if (rotationGo == null) { RLog.Error("[StoneGate] [StoneGateStoreMono] [Init] rotationGo is null"); Destroy(gameObject); }
            if (mode == Objects.CreateGateParent.RotateMode.None) { RLog.Error("[StoneGate] [StoneGateStoreMono] [Init] mode is None"); Destroy(gameObject); }
            if (Testing.Settings.logExtraStoneGateStoreMono) { Misc.Msg($"[StoneGate] [StoneGateStoreMono] [Init] Rotation GameObject: {rotationGo.name} Mode: {mode}"); }
            if (floorBeam == null && topBeam == null && rockWall == null && extraPillar == null) { RLog.Error("[StoneGate] [StoneGateStoreMono] [Init] All GameObjects are null"); Destroy(gameObject); }
            _rotateGo = rotationGo;
            _rotateMode = mode;
            _floorBeam = floorBeam;
            _topBeam = topBeam;
            _rockWall = rockWall;
            _extraPillar = extraPillar;
            if (floorBeam != null)
            {
                objectsToRotate.Add(floorBeam);
                namesOfAllGo.Add(floorBeam.name, floorBeam);
                originalRotations[floorBeam.GetInstanceID()] = floorBeam.transform.rotation;
            }
            if (topBeam != null)
            {
                objectsToRotate.Add(topBeam);
                namesOfAllGo.Add(topBeam.name, topBeam);
                originalRotations[topBeam.GetInstanceID()] = topBeam.transform.rotation;
            }
            if (rockWall != null)
            {
                objectsToRotate.Add(rockWall);
                namesOfAllGo.Add(rockWall.name, rockWall);
                originalRotations[rockWall.GetInstanceID()] = rockWall.transform.rotation;
            }
            if (extraPillar != null)
            {
                objectsToRotate.Add(extraPillar);
                namesOfAllGo.Add(extraPillar.name, extraPillar);
                originalRotations[extraPillar.GetInstanceID()] = extraPillar.transform.rotation;
            }
                
            namesOfAllGo.Add(rotationGo.name, rotationGo);

            if (Testing.Settings.logExtraStoneGateStoreMono) { Misc.Msg($"[StoneGate] [StoneGateStoreMono] [Init] Added {objectsToRotate.Count} GameObjects to objectsToRotate"); }

            if (LinkUiElement == null)
            {
                if (Testing.Settings.logExtraStoneGateStoreMono) { Misc.Msg($"[StoneGate] [StoneGateStoreMono] [Init] Creating LinkUI"); }
                LinkUiElement = Tools.LinkUi.CreateLinkUi(rotationGo, 2f, null, StoneGate.stoneGateOpenCloseIcon, null);
            }
            else
            {
                if (Testing.Settings.logExtraStoneGateStoreMono) { Misc.Msg($"[StoneGate] [StoneGateStoreMono] [Init] LinkUI already exists"); }
            }
        }

        public bool IsGateOpen()
        {
            return _gateOpen;
        }

        public void ToggleGate(bool raiseNetwork = true)
        {
            if (_gateOpen)
            {
                CloseGate(raiseNetwork);
            }
            else
            {
                OpenGate(raiseNetwork);
            }
        }

        public void OpenGate(bool raiseNetwork = true)
        {
            if (_gateOpen || isAnimating)
            {
                return;
            }

            _gateOpen = true;

            if (raiseNetwork)
            {
                // Send network event to open the gate for all clients
                BoltEntity entity = Track.FindEntity(gameObject);
                if (entity != null)
                {
                    Network.StoneGateSyncEvent.SendState(entity, Network.StoneGateSyncEvent.StoneGateSyncType.OpenGate);
                }
            }

            // Start the gate opening animation
            AnimateGate(true).RunCoro();
        }

        public void CloseGate(bool raiseNetwork = true)
        {
            if (!_gateOpen || isAnimating)
            {
                return;
            }

            _gateOpen = false;

            if (raiseNetwork)
            {
                // Send network event to close the gate for all clients
                BoltEntity entity = Track.FindEntity(gameObject);
                if (entity != null)
                {
                    Network.StoneGateSyncEvent.SendState(entity, Network.StoneGateSyncEvent.StoneGateSyncType.CloseGate);
                }
            }

            // Start the gate closing animation
            AnimateGate(false).RunCoro();
        }

        private IEnumerator AnimateGate(bool opening)
        {
            isAnimating = true;

            float startTime = Time.time;
            float elapsedTime = 0f;

            // Get the rotation axis based on the gate type
            Vector3 rotationAxis = GetRotationAxis();

            // Calculate start and end rotations for each object
            Dictionary<int, Quaternion> startRotations = new Dictionary<int, Quaternion>();
            Dictionary<int, Quaternion> endRotations = new Dictionary<int, Quaternion>();

            foreach (GameObject obj in objectsToRotate)
            {
                int id = obj.GetInstanceID();

                // Starting rotation is the current rotation
                startRotations[id] = obj.transform.rotation;

                // For opening, we rotate from current to 90 degrees
                // For closing, we rotate from current back to the original position
                if (opening)
                {
                    // Calculate target rotation (90 degrees around the axis)
                    Quaternion additionalRotation = Quaternion.AngleAxis(90f, rotationAxis);
                    endRotations[id] = startRotations[id] * additionalRotation;
                }
                else
                {
                    // Return to original rotation
                    endRotations[id] = originalRotations[id];
                }
            }

            // Animate the rotation
            while (elapsedTime < animationDuration)
            {
                elapsedTime = Time.time - startTime;
                float t = Mathf.Clamp01(elapsedTime / animationDuration);

                // Apply a smooth easing function
                float smoothT = SmoothStep(0f, 1f, t);

                foreach (GameObject obj in objectsToRotate)
                {
                    int id = obj.GetInstanceID();

                    // Smoothly interpolate between start and end rotations
                    obj.transform.rotation = Quaternion.Slerp(
                        startRotations[id],
                        endRotations[id],
                        smoothT
                    );
                }

                yield return null;
            }

            // Ensure all objects are at their final rotation
            foreach (GameObject obj in objectsToRotate)
            {
                int id = obj.GetInstanceID();
                obj.transform.rotation = endRotations[id];

                // If we're closing, update the stored original rotations
                if (!opening)
                {
                    originalRotations[id] = obj.transform.rotation;
                }
            }

            isAnimating = false;
        }

        private Vector3 GetRotationAxis()
        {
            if (_rotateMode == Objects.CreateGateParent.RotateMode.Vertical)
            {
                // For vertical rotation (like a door hinge), rotate around the Y axis
                return _rotateGo.transform.up;
            }
            else // RotateMode.Horizontal
            {
                // For horizontal rotation (like a drawbridge), rotate around the X axis
                return _rotateGo.transform.right;
            }
        }

        // Helper smoothing function for animation
        private float SmoothStep(float edge0, float edge1, float x)
        {
            // Scale and clamp x to 0..1 range
            x = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
            // Evaluate polynomial
            return x * x * (3 - 2 * x);
        }
    }
}
