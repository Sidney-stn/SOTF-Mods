using UnityEngine;
using System.Collections;
using RedLoader.Unity.IL2CPP.Utils.Collections;
using TheForest.Utils;
using RedLoader;

namespace BuildingMagnet
{
    [RegisterTypeInIl2Cpp]
    internal class BuildingMagnetMono : MonoBehaviour
    {
        private float _moveSpeed = 5f;            // Speed at which objects move to player
        private LayerMask _defaultLayerMask;      // Default layer mask when mode is ALL or NONE
        private int _maxObjectsToAttract = 10;    // Maximum number of objects to attract at once

        private List<GameObject> _currentlyAttractedObjects = new List<GameObject>();
        private Dictionary<GameObject, Coroutine> _activeMovementCoroutines = new Dictionary<GameObject, Coroutine>();

        public bool IsEnabled
        {
            get
            {
                return Config.Enabled.Value;
            }
        }

        public string Mode
        {
            get
            {
                return MagnetValues.LastValue;
            }
        }

        private Dictionary<string, List<string>> _modeLinkedObject = new Dictionary<string, List<string>>  // Mode, <GameObjectName, LayerName>
        {
            {"NONE", new List<string> { null, null } },
            {"LOG", new List<string> { "LogPickup(Clone)", "Prop" } },
            {"3/4 LOG", new List<string> { "LogQuarterX3Pickup(Clone)", "Prop" } },
            {"1/2 LOG", new List<string> { "LogQuarterX2Pickup(Clone)", "Prop" } },
            {"1/4 LOG", new List<string> { "LogQuarterX1Pickup(Clone)", "Prop" } },
            {"STONE", new List<string> { "StonePickup(Clone)", "PickUp" } },
            {"PLANK", new List<string> { "LogPlankPickup(Clone)", "PickUp" } },
            {"3/4 PLANK", new List<string> { "LogPlankQuarterX3Pickup(Clone)", "PickUp" } },
            {"1/2 PLANK", new List<string> { "LogPlankQuarterX2Pickup(Clone)", "PickUp" } },
            {"1/4 PLANK", new List<string> { "LogPlankQuarterX1Pickup(Clone)", "PickUp" } },
            {"ALL", new List<string> { null, null } }
        };

        private HashSet<string> _scanForGoName = new HashSet<string>();

        /// <summary>
        /// Add a mode linked object to the dictionary, if the mode already exists it will be overwritten.
        /// Mode, <GameObjectName, LayerName>
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="gameObjectName"></param>
        /// <param name="layerName"></param>
        public void AddModeLinkedObject(string mode, string gameObjectName, string layerName)
        {
            if (_modeLinkedObject.ContainsKey(mode))
            {
                _modeLinkedObject[mode] = new List<string> { gameObjectName, layerName };
            }
            else
            {
                _modeLinkedObject.Add(mode, new List<string> { gameObjectName, layerName });
            }
        }

        private void Start()
        {
            MagnetValues.ValueChange += OnModeChange;

            // Initialize the scan for game object names hash set
            OnModeChange(this, EventArgs.Empty);
        }

        private void OnModeChange(object sender, EventArgs e)
        {
            _scanForGoName.Clear();

            if (Mode == "ALL")
            {
                // Add all game object names for ALL mode
                foreach (var entry in _modeLinkedObject)
                {
                    if (entry.Key != "NONE" && entry.Key != "ALL" && entry.Value[0] != null)
                    {
                        _scanForGoName.Add(entry.Value[0]);
                    }
                }
            }
            else if (_modeLinkedObject.ContainsKey(Mode) && _modeLinkedObject[Mode][0] != null)
            {
                _scanForGoName.Add(_modeLinkedObject[Mode][0]);
            }
        }

        private void Update()
        {
            if (!IsEnabled || Mode == "NONE")
            {
                return;
            }

            PerformSphereCast();
        }

        private void PerformSphereCast()
        {
            // Get the layer mask based on current mode
            LayerMask layerMask = GetLayerMaskForCurrentMode();

            // Perform sphere cast
            Collider[] hitColliders = Physics.OverlapSphere(LocalPlayer.Transform.position, Config.MagnetRange.Value, layerMask);

            // Filter and process hits
            foreach (var hitCollider in hitColliders)
            {
                GameObject hitObject = hitCollider.gameObject;

                // Check if we should attract this object based on its name
                if (ShouldAttractObject(hitObject) && !_currentlyAttractedObjects.Contains(hitObject))
                {
                    // Add to currently attracted objects, but respect the maximum limit
                    if (_currentlyAttractedObjects.Count < _maxObjectsToAttract)
                    {
                        _currentlyAttractedObjects.Add(hitObject);

                        // Start smoothly moving the object to the player
                        if (_activeMovementCoroutines.ContainsKey(hitObject))
                        {
                            StopCoroutine(_activeMovementCoroutines[hitObject]);
                        }

                        Coroutine movementCoroutine = StartCoroutine(MoveObjectToPlayerSmoothly(hitObject).WrapToIl2Cpp());
                        _activeMovementCoroutines[hitObject] = movementCoroutine;
                    }
                }
            }

            // Clean up list of attracted objects (remove any that have been destroyed)
            _currentlyAttractedObjects.RemoveAll(item => item == null);
        }

        private bool ShouldAttractObject(GameObject obj)
        {
            if (Mode == "ALL")
            {
                // In ALL mode, check if the object name is in any mode's linked objects
                foreach (string goName in _scanForGoName)
                {
                    if (obj.name == goName)
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (_modeLinkedObject.ContainsKey(Mode))
            {
                string targetName = _modeLinkedObject[Mode][0];
                return obj.name == targetName;
            }

            return false;
        }

        private LayerMask GetLayerMaskForCurrentMode()
        {
            if (Mode == "NONE")
            {
                return 0; // No layers (nothing will be detected)
            }

            if (Mode == "ALL")
            {
                // For ALL mode, we need to create a mask that includes all the layers used in the dictionary
                LayerMask mask = _defaultLayerMask;

                foreach (var entry in _modeLinkedObject)
                {
                    if (entry.Key != "NONE" && entry.Key != "ALL" && entry.Value[1] != null)
                    {
                        int layerIndex = LayerMask.NameToLayer(entry.Value[1]);
                        if (layerIndex >= 0)
                        {
                            mask |= (1 << layerIndex);
                        }
                    }
                }

                return mask;
            }
            else if (_modeLinkedObject.ContainsKey(Mode) && _modeLinkedObject[Mode][1] != null)
            {
                string layerName = _modeLinkedObject[Mode][1];
                int layerIndex = LayerMask.NameToLayer(layerName);

                if (layerIndex >= 0)
                {
                    return (1 << layerIndex);
                }
            }

            return _defaultLayerMask; // Fallback to default layer mask
        }

        private IEnumerator MoveObjectToPlayerSmoothly(GameObject obj)
        {
            // Check if the object has a rigidbody and disable it temporarily
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            bool hadRigidbody = false;
            bool wasKinematic = false;

            if (rb != null)
            {
                hadRigidbody = true;
                wasKinematic = rb.isKinematic;
                rb.isKinematic = true;
            }

            // Move object smoothly to player
            float startTime = Time.time;
            Vector3 startPosition = obj.transform.position;

            while (obj != null)
            {
                float journeyLength = Vector3.Distance(obj.transform.position, LocalPlayer.Transform.position);

                // If we're close enough, break out of the loop
                if (journeyLength < 1f)
                {
                    break;
                }

                // Calculate movement
                float distCovered = (Time.time - startTime) * _moveSpeed;
                float fractionOfJourney = distCovered / journeyLength;
                obj.transform.position = Vector3.Lerp(
                    obj.transform.position,
                    LocalPlayer.Transform.position,
                    fractionOfJourney * Time.deltaTime * 10f
                );

                yield return null;
            }

            // Clean up
            if (obj != null)
            {
                // Final position adjustment
                obj.transform.position = LocalPlayer.Transform.position;

                // Restore rigidbody state if needed
                if (hadRigidbody && rb != null)
                {
                    rb.isKinematic = wasKinematic;
                }

                // Remove from currently attracted objects
                _currentlyAttractedObjects.Remove(obj);
                _activeMovementCoroutines.Remove(obj);
            }
        }

        private void OnDestroy()
        {
            MagnetValues.ValueChange -= OnModeChange;

            // Stop all coroutines
            foreach (var coroutine in _activeMovementCoroutines.Values)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }

            _activeMovementCoroutines.Clear();
            _currentlyAttractedObjects.Clear();
        }

        // Optional: Visual debugging
        private void OnDrawGizmosSelected()
        {
            if (LocalPlayer.Transform.position != null || LocalPlayer.Transform.position != Vector3.zero)
            {
                // Draw the magnet radius
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(LocalPlayer.Transform.position, Config.MagnetRange.Value);
            }
        }
    }
}