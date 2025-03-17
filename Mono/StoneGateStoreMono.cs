using RedLoader;
using Sons.Gui.Input;
using StoneGate.Objects;
using System.Collections;
using UnityEngine;
using static StoneGate.Saving.Manager;

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
        // Store original positions and rotations for each object
        private Dictionary<int, Vector3> originalPositions = new Dictionary<int, Vector3>();
        private Dictionary<int, Quaternion> originalRotations = new Dictionary<int, Quaternion>();

        // Animation parameters
        private bool isAnimating = false;
        private float animationDuration = 1.0f; // Duration of the gate animation in seconds
        private float rotationAngle = 90f; // Default rotation angle
        private int rotationDirection = 1; // 1 for positive rotation, -1 for negative
        //// THINGS WITH ROTATION END

        public LinkUiElement LinkUiElement { get; private set; }

        private bool _gateOpen = false;

        /// <summary>
        /// Initialize the StoneGateStoreMono with the required GameObjects and Mode
        /// </summary>
        /// <param name="rotationGo"></param>
        /// <param name="mode"></param>
        /// <param name="floorBeam"></param>
        /// <param name="topBeam"></param>
        /// <param name="rockWall"></param>
        /// <param name="extraPillar"></param>
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
            Tools.Gates.ocupiedObjects.Add(rotationGo);
            if (floorBeam != null)
            {
                objectsToRotate.Add(floorBeam);
                namesOfAllGo.Add(floorBeam.name, floorBeam);
                originalRotations[floorBeam.GetInstanceID()] = floorBeam.transform.rotation;
                originalPositions[floorBeam.GetInstanceID()] = floorBeam.transform.position;
                Tools.Gates.ocupiedObjects.Add(floorBeam);
            }
            if (topBeam != null)
            {
                objectsToRotate.Add(topBeam);
                namesOfAllGo.Add(topBeam.name, topBeam);
                originalRotations[topBeam.GetInstanceID()] = topBeam.transform.rotation;
                originalPositions[topBeam.GetInstanceID()] = topBeam.transform.position;
                Tools.Gates.ocupiedObjects.Add(topBeam);
            }
            if (rockWall != null)
            {
                objectsToRotate.Add(rockWall);
                namesOfAllGo.Add(rockWall.name, rockWall);
                originalRotations[rockWall.GetInstanceID()] = rockWall.transform.rotation;
                originalPositions[rockWall.GetInstanceID()] = rockWall.transform.position;
                Tools.Gates.ocupiedObjects.Add(rockWall);
            }
            if (extraPillar != null)
            {
                objectsToRotate.Add(extraPillar);
                namesOfAllGo.Add(extraPillar.name, extraPillar);
                originalRotations[extraPillar.GetInstanceID()] = extraPillar.transform.rotation;
                originalPositions[extraPillar.GetInstanceID()] = extraPillar.transform.position;
                Tools.Gates.ocupiedObjects.Add(extraPillar);
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

            // Determine the direction to rotate based on player position
            DetermineRotationDirection();

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

            // Get the pivot point (center of the rotation gameobject)
            Vector3 pivotPoint = _rotateGo.transform.position;

            // Store current positions and rotations
            Dictionary<int, Vector3> currentPositions = new Dictionary<int, Vector3>();
            Dictionary<int, Quaternion> currentRotations = new Dictionary<int, Quaternion>();

            foreach (GameObject obj in objectsToRotate)
            {
                int id = obj.GetInstanceID();
                currentPositions[id] = obj.transform.position;
                currentRotations[id] = obj.transform.rotation;
            }

            // Log for debugging
            if (Testing.Settings.logExtraStoneGateStoreMono)
            {
                Misc.Msg($"[StoneGate] [AnimateGate] Starting gate animation: opening={opening}, objects={objectsToRotate.Count}");
                Misc.Msg($"[StoneGate] [AnimateGate] Pivot point: {pivotPoint}, Axis: {rotationAxis}");
            }

            // Animate the rotation around the pivot
            while (elapsedTime < animationDuration)
            {
                elapsedTime = Time.time - startTime;
                float t = Mathf.Clamp01(elapsedTime / animationDuration);

                // Apply a smooth easing function
                float smoothT = SmoothStep(0f, 1f, t);

                foreach (GameObject obj in objectsToRotate)
                {
                    int id = obj.GetInstanceID();

                    if (opening)
                    {
                        // For opening: rotate from current position to 90 degrees around pivot
                        Vector3 relativePos = originalPositions[id] - pivotPoint;
                        // Apply rotation direction to the angle
                        Quaternion rotation = Quaternion.AngleAxis(rotationAngle * rotationDirection * smoothT, rotationAxis);

                        // Calculate the new position by rotating around pivot
                        Vector3 newPos = pivotPoint + rotation * relativePos;

                        // Calculate the new rotation
                        Quaternion newRot = rotation * originalRotations[id];

                        // Apply
                        obj.transform.position = newPos;
                        obj.transform.rotation = newRot;
                    }
                    else
                    {
                        // For closing: rotate from current position back to original position
                        // Lerp directly from current position to original position
                        obj.transform.position = Vector3.Lerp(
                            currentPositions[id],
                            originalPositions[id],
                            smoothT
                        );

                        obj.transform.rotation = Quaternion.Slerp(
                            currentRotations[id],
                            originalRotations[id],
                            smoothT
                        );
                    }
                }

                yield return null;
            }

            // Final positioning to ensure perfect alignment
            foreach (GameObject obj in objectsToRotate)
            {
                int id = obj.GetInstanceID();

                if (opening)
                {
                    // Calculate final open position and rotation
                    Vector3 relativePos = originalPositions[id] - pivotPoint;
                    // Apply rotation direction to the angle
                    Quaternion finalRotation = Quaternion.AngleAxis(rotationAngle * rotationDirection, rotationAxis);

                    Vector3 finalPos = pivotPoint + finalRotation * relativePos;
                    Quaternion finalRot = finalRotation * originalRotations[id];

                    obj.transform.position = finalPos;
                    obj.transform.rotation = finalRot;
                }
                else
                {
                    // Return to exact original position and rotation
                    obj.transform.position = originalPositions[id];
                    obj.transform.rotation = originalRotations[id];
                }
            }

            isAnimating = false;

            if (Testing.Settings.logExtraStoneGateStoreMono)
            {
                Misc.Msg($"[StoneGate] [AnimateGate] Completed gate animation: opening={opening}");
            }
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

        private void DetermineRotationDirection()
        {
            // Get player camera transform
            Transform cameraTransform = TheForest.Utils.LocalPlayer._instance._mainCam.transform;

            // Get player's forward direction (camera direction)
            Vector3 playerForward = cameraTransform.forward;

            // Get gate position (using rotation object as reference)
            Vector3 gatePosition = _rotateGo.transform.position;

            // Get the forward direction of the gate
            Vector3 gateForward = _rotateGo.transform.forward;

            // For vertical rotation (like a door):
            if (_rotateMode == Objects.CreateGateParent.RotateMode.Vertical)
            {
                // If player is looking in the same general direction as the gate's forward (dot product > 0),
                // rotate the gate in the opposite direction of the camera's forward
                if (Vector3.Dot(gateForward, playerForward) > 0)
                {
                    rotationDirection = -1; // Rotate clockwise (negative angle)
                }
                else
                {
                    rotationDirection = 1; // Rotate counter-clockwise (positive angle)
                }
            }
            // For horizontal rotation (like a drawbridge):
            else
            {
                // For horizontal gates, we need to determine if the player is looking more up or down at the gate
                // Get the gate's up vector
                Vector3 gateUp = _rotateGo.transform.up;

                // If player is looking more downward toward the gate
                if (Vector3.Dot(gateUp, playerForward) < 0)
                {
                    rotationDirection = -1; // Rotate downward
                }
                else
                {
                    rotationDirection = 1; // Rotate upward
                }
            }

            if (Testing.Settings.logExtraStoneGateStoreMono)
            {
                Misc.Msg($"[StoneGate] [DetermineRotationDirection] Camera forward: {playerForward}, Gate forward: {gateForward}");
                Misc.Msg($"[StoneGate] [DetermineRotationDirection] Rotation direction: {rotationDirection}");
            }
        }

        /// <summary>
        /// Get the names of all GameObjects associated with this gate
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, GameObject> GetNamesAndGameObjects()
        {
            return namesOfAllGo;
        }


        /// <summary>
        /// Destroy the gate and all associated GameObjects
        /// </summary>
        /// <param name="raiseNetwork"></param>
        public void DestroyGate(bool raiseNetwork = true)
        {
            if (LinkUiElement != null)
            {
                Destroy(LinkUiElement);
            }
            foreach (GameObject obj in objectsToRotate)
            {
                Tools.Gates.ocupiedObjects.Remove(obj);
            }
            Tools.Gates.ocupiedObjects.Remove(_rotateGo);
            Destroy(gameObject);
        }

        /// <summary>
        /// Find the name of a GameObject from the dictionary of names and GameObjects (namesOfAllGo)
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private string FindNameFromGameObject(GameObject go)
        {
            foreach (KeyValuePair<string, GameObject> kvp in namesOfAllGo)
            {
                if (kvp.Value == go)
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        public GatesManager.GatesModData GetSaveData()
        {
            // Always Save The Gate In Closed State, Regardless Of The Current State
            GatesManager.GatesModData data = new GatesManager.GatesModData
            {
                Mode = _rotateMode.ToString(),
                FloorBeamName = FindNameFromGameObject(_floorBeam),
                TopBeamName = FindNameFromGameObject(_topBeam),
                RockWallName = FindNameFromGameObject(_rockWall),
                ExtraPillarName = FindNameFromGameObject(_extraPillar),
                RotationGoName = FindNameFromGameObject(_rotateGo)
            };
            return data;
        }
    }
}
