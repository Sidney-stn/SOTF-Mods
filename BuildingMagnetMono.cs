using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TheForest.Utils;
using RedLoader.Unity.IL2CPP.Utils.Collections;
using RedLoader;

namespace BuildingMagnet
{
    [RegisterTypeInIl2Cpp]
    internal class BuildingMagnetMono : MonoBehaviour
    {
        private float _moveSpeed = 5f;            // Speed at which objects move to player
        private LayerMask _defaultLayerMask;      // Default layer mask when mode is ALL or NONE
        private int _maxObjectsToAttract = 10;    // Maximum number of objects to attract at once

        private MagnetTracking _magnetTracking = new MagnetTracking();

        public bool IsEnabled
        {
            get
            {
                if (BuildingMagnet.isItemUnlocked == true && Config.Enabled.Value == true)
                {
                    return true;
                }
                else if (BuildingMagnet.isItemUnlocked == false && Config.Enabled.Value == true)
                {
                    return false;
                }
                else if (BuildingMagnet.isItemUnlocked == true && Config.Enabled.Value == false)
                {
                    return false;
                }
                else
                {
                    return false;
                }
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
            // Release all currently attracted objects
            ReleaseAllAttractedObjects();

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

        // Helper method to release all currently attracted objects
        private void ReleaseAllAttractedObjects()
        {
            // Stop all active movement coroutines
            foreach (var obj in _magnetTracking.GetAllObjects())
            {
                if (obj != null)
                {
                    Coroutine coroutine = _magnetTracking.GetCoroutine(obj);
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }

                    // Restore normal physics
                    Rigidbody rb = obj.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                    }
                }
            }

            // Clear the tracking
            _magnetTracking.Clear();
        }

        private void Update()
        {
            if (!IsEnabled || Mode == "NONE")
            {
                return;
            }
            if (!LocalPlayer.IsInWorld || LocalPlayer.InWater || LocalPlayer.IsInCaves || LocalPlayer.IsInInventory || LocalPlayer.IsInGolfCart || LocalPlayer.IsGliding || LocalPlayer.IsConstructing || LocalPlayer.IsInMidAction)
            {
                ReleaseAllAttractedObjects();
            }

            PerformSphereCast();
        }

        private void PerformSphereCast()
        {
            // Get the layer mask based on current mode
            LayerMask layerMask = GetLayerMaskForCurrentMode();

            // Perform sphere cast
            Collider[] hitColliders = Physics.OverlapSphere(LocalPlayer.Transform.position, Config.MagnetRange.Value, layerMask);

            // Clean up any null references
            _magnetTracking.CleanupNullReferences();

            // Filter and process hits
            foreach (var hitCollider in hitColliders)
            {
                GameObject hitObject = hitCollider.gameObject;

                // Check if we should attract this object based on its name
                if (ShouldAttractObject(hitObject) && !_magnetTracking.IsTracking(hitObject))
                {
                    // Add to currently attracted objects, but respect the maximum limit
                    if (_magnetTracking.Count < _maxObjectsToAttract)
                    {
                        _magnetTracking.AddObject(hitObject);

                        // Stop existing coroutine if there is one
                        Coroutine existingCoroutine = _magnetTracking.GetCoroutine(hitObject);
                        if (existingCoroutine != null)
                        {
                            StopCoroutine(existingCoroutine);
                        }

                        // Start smoothly moving the object to the player
                        Coroutine movementCoroutine = StartCoroutine(MoveObjectToPlayerSmoothly(hitObject).WrapToIl2Cpp());
                        _magnetTracking.SetCoroutine(hitObject, movementCoroutine);
                    }
                }
            }
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
            if (obj == null)
                yield break;

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

            // Get the player layer as a layer mask
            int playerLayerIndex = LayerMask.NameToLayer("Player");
            LayerMask playerLayerMask = 1 << playerLayerIndex;

            // Get all colliders on the object
            Collider[] colliders;
            if (obj.name == "StonePickup(Clone)")
            {
                // For stones, include all child colliders
                colliders = obj.GetComponentsInChildren<Collider>();
                RLog.Msg($"[BuildingMagnet] Using GetComponentsInChildren for Stone: {colliders.Length} colliders found");
            }
            else
            {
                // For other objects, use the original approach
                colliders = obj.GetComponents<Collider>();
            }
            Dictionary<Collider, LayerMask> originalExcludeLayers = new Dictionary<Collider, LayerMask>();

            // Get player colliders for direct collision ignoring
            Collider[] playerColliders = LocalPlayer.GameObject.GetComponentsInChildren<Collider>(true);
            List<(Collider, Collider, bool)> ignoredCollisionPairs = new List<(Collider, Collider, bool)>();

            // Apply double protection:
            // 1. Exclude player layer in collider.excludeLayers
            // 2. Use Physics.IgnoreCollision for specific collider pairs
            foreach (Collider collider in colliders)
            {
                if (collider != null)
                {
                    // Store and set excludeLayers
                    originalExcludeLayers[collider] = collider.excludeLayers;
                    collider.excludeLayers |= playerLayerMask;

                    // Also use direct Physics.IgnoreCollision as a backup
                    foreach (Collider playerCollider in playerColliders)
                    {
                        if (playerCollider != null)
                        {
                            bool wasIgnoring = Physics.GetIgnoreCollision(collider, playerCollider);
                            Physics.IgnoreCollision(collider, playerCollider, true);
                            ignoredCollisionPairs.Add((collider, playerCollider, wasIgnoring));
                        }
                    }
                }
            }

            // Log to verify this is happening
            RLog.Msg($"[BuildingMagnet] Applied collision exclusion to {colliders.Length} colliders for {obj.name}");

            // Move object smoothly to player
            float startTime = Time.time;
            Vector3 startPosition = obj.transform.position;

            BoltEntity boltEntity = null;
            if (BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                boltEntity = obj.GetComponent<BoltEntity>();

                while (obj != null)
                {
                    // Verify exclusion is still applied (extra safety check)
                    foreach (Collider collider in colliders)
                    {
                        if (collider != null && (collider.excludeLayers & playerLayerMask) != playerLayerMask)
                        {
                            RLog.Msg($"[BuildingMagnet] Warning: Player layer exclusion was lost, reapplying to {obj.name}");
                            collider.excludeLayers |= playerLayerMask;
                        }
                    }

                    float journeyLength = Vector3.Distance(obj.transform.position, LocalPlayer.Transform.position);

                    // Calculate movement
                    float distCovered = (Time.time - startTime) * _moveSpeed;
                    float fractionOfJourney = distCovered / journeyLength;
                    if (boltEntity == null)
                    {
                        obj.transform.position = Vector3.Lerp(
                        obj.transform.position,
                        LocalPlayer.Transform.position,
                        fractionOfJourney * Time.deltaTime * 10f
                        );
                    } else
                    {
                        if (boltEntity.hasControl == false)
                        {
                            boltEntity.TakeControl();
                            RLog.Msg($"[BuildingMagnet] Took control of {obj.name}");
                            if (boltEntity.hasControl == false)
                            {
                                RLog.Msg(ConsoleColor.DarkYellow, $"[BuildingMagnet] Failed to take control of {obj.name}");
                                // Restore to original condition
                                if (hadRigidbody && rb != null)
                                {
                                    rb.isKinematic = wasKinematic;
                                }
                                foreach (Collider collider in colliders)
                                {
                                    if (collider != null && originalExcludeLayers.TryGetValue(collider, out LayerMask original))
                                    {
                                        collider.excludeLayers = original;
                                    }
                                }
                                foreach (var (objCollider, playerCollider, wasIgnoring) in ignoredCollisionPairs)
                                {
                                    if (objCollider != null && playerCollider != null)
                                    {
                                        Physics.IgnoreCollision(objCollider, playerCollider, wasIgnoring);
                                    }
                                }
                                _magnetTracking.RemoveObject(obj);
                                yield break;
                            }
                        }
                        boltEntity.transform.position = Vector3.Lerp(
                        obj.transform.position,
                        LocalPlayer.Transform.position,
                        fractionOfJourney * Time.deltaTime * 10f
                        );
                    }
                    


                    yield return null;
                }

                // Clean up
                if (obj != null)
                {
                    // Final position adjustment
                    obj.transform.position = LocalPlayer.Transform.position;

                    // Restore original exclude layers
                    foreach (Collider collider in colliders)
                    {
                        if (collider != null && originalExcludeLayers.TryGetValue(collider, out LayerMask original))
                        {
                            collider.excludeLayers = original;
                        }
                    }

                    // Restore original collision ignore states
                    foreach (var (objCollider, playerCollider, wasIgnoring) in ignoredCollisionPairs)
                    {
                        if (objCollider != null && playerCollider != null)
                        {
                            Physics.IgnoreCollision(objCollider, playerCollider, wasIgnoring);
                        }
                    }

                    // Restore rigidbody state if needed
                    if (hadRigidbody && rb != null)
                    {
                        rb.isKinematic = wasKinematic;
                    }

                    RLog.Msg($"[BuildingMagnet] Restored collision settings for {obj.name}");
                }

                // Remove from tracking collections
                _magnetTracking.RemoveObject(obj);
            }
        }

        private void OnDestroy()
        {
            MagnetValues.ValueChange -= OnModeChange;

            // Stop all coroutines using the MagnetTracking class
            foreach (var obj in _magnetTracking.GetAllObjects())
            {
                if (obj != null)
                {
                    Coroutine coroutine = _magnetTracking.GetCoroutine(obj);
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                }
            }

            // Clear the tracking collections
            _magnetTracking.Clear();
        }

        // Optional: Visual debugging
        private void OnDrawGizmosSelected()
        {
            if (LocalPlayer.Transform.position != Vector3.zero)
            {
                // Draw the magnet radius
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(LocalPlayer.Transform.position, Config.MagnetRange.Value);
            }
        }
    }
}