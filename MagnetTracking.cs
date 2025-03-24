using RedLoader;
using UnityEngine;

namespace BuildingMagnet
{
    /// <summary>
    /// Handles tracking of attracted objects and their movement coroutines
    /// using instance IDs for stable references
    /// </summary>
    public class MagnetTracking
    {
        // Maps instance IDs to GameObject references
        private Dictionary<int, GameObject> _trackedObjects = new Dictionary<int, GameObject>();

        // Maps instance IDs to active coroutines
        private Dictionary<int, Coroutine> _movementCoroutines = new Dictionary<int, Coroutine>();

        /// <summary>
        /// Adds an object to the tracking collection
        /// </summary>
        /// <param name="obj">The GameObject to track</param>
        /// <returns>True if object was added, false if already tracked</returns>
        public bool AddObject(GameObject obj)
        {
            if (obj == null)
                return false;

            int instanceId = obj.GetInstanceID();

            if (_trackedObjects.ContainsKey(instanceId))
                return false;

            _trackedObjects.Add(instanceId, obj);
            if (BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                BoltEntity boltEntity = obj.GetComponent<BoltEntity>();
                if (boltEntity.isOwner == false && !boltEntity.hasControl)
                {
                    Network.ClientEvents.Instance.SendClientEvent(Network.ClientEvents.ClientEvent.ServerReleaseControl, boltEntity.networkId);
                } else if (boltEntity.isOwner)
                {
                    boltEntity.TakeControl();
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if an object is currently being tracked
        /// </summary>
        /// <param name="obj">The GameObject to check</param>
        /// <returns>True if the object is being tracked</returns>
        public bool IsTracking(GameObject obj)
        {
            if (obj == null)
                return false;

            return _trackedObjects.ContainsKey(obj.GetInstanceID());
        }

        /// <summary>
        /// Adds or updates a movement coroutine for a tracked object
        /// </summary>
        /// <param name="obj">The GameObject the coroutine is moving</param>
        /// <param name="coroutine">The active movement coroutine</param>
        /// <returns>True if added/updated successfully</returns>
        public bool SetCoroutine(GameObject obj, Coroutine coroutine)
        {
            if (obj == null || coroutine == null)
                return false;

            int instanceId = obj.GetInstanceID();

            // Ensure object is being tracked
            if (!_trackedObjects.ContainsKey(instanceId))
            {
                RLog.Msg($"[BuildingMagnet] Attempted to set coroutine for untracked object ID: {instanceId}");
                return false;
            }

            // Update or add coroutine
            _movementCoroutines[instanceId] = coroutine;
            return true;
        }

        /// <summary>
        /// Gets the coroutine associated with an object
        /// </summary>
        /// <param name="obj">The GameObject</param>
        /// <returns>The coroutine or null if not found</returns>
        public Coroutine GetCoroutine(GameObject obj)
        {
            if (obj == null)
                return null;

            int instanceId = obj.GetInstanceID();

            if (_movementCoroutines.TryGetValue(instanceId, out Coroutine coroutine))
                return coroutine;

            return null;
        }

        /// <summary>
        /// Removes an object and its associated coroutine from tracking
        /// </summary>
        /// <param name="obj">The GameObject to remove</param>
        /// <returns>True if the object was removed</returns>
        public bool RemoveObject(GameObject obj)
        {
            if (obj == null)
                return false;

            int instanceId = obj.GetInstanceID();

            bool objectRemoved = _trackedObjects.Remove(instanceId);
            _movementCoroutines.Remove(instanceId);

            if (BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                BoltEntity boltEntity = obj.GetComponent<BoltEntity>();
                boltEntity.ReleaseControl();
                Network.ClientEvents.Instance.SendClientEvent(Network.ClientEvents.ClientEvent.ServerTakeControl, boltEntity.networkId);
            }

            return objectRemoved;
        }

        /// <summary>
        /// Gets all currently tracked GameObjects
        /// </summary>
        /// <returns>List of all tracked GameObjects</returns>
        public List<GameObject> GetAllObjects()
        {
            List<GameObject> result = new List<GameObject>();

            foreach (var obj in _trackedObjects.Values)
            {
                if (obj != null)
                    result.Add(obj);
            }

            return result;
        }

        /// <summary>
        /// Gets the count of objects currently being tracked
        /// </summary>
        public int Count => _trackedObjects.Count;

        /// <summary>
        /// Cleans up any null references from the tracking collections
        /// </summary>
        /// <returns>Number of references that were cleaned up</returns>
        public int CleanupNullReferences()
        {
            List<int> keysToRemove = new List<int>();

            foreach (var kvp in _trackedObjects)
            {
                if (kvp.Value == null)
                    keysToRemove.Add(kvp.Key);
            }

            foreach (int key in keysToRemove)
            {
                _trackedObjects.Remove(key);
                _movementCoroutines.Remove(key);
            }

            return keysToRemove.Count;
        }

        /// <summary>
        /// Clears all tracked objects and coroutines
        /// </summary>
        public void Clear()
        {
            _trackedObjects.Clear();
            _movementCoroutines.Clear();
        }
    }
}
