
using UnityEngine;

namespace Signs.Saving
{
    internal class Track
    {
        public static Dictionary<BoltEntity, GameObject> spawnedSigns = new Dictionary<BoltEntity, GameObject>();

        public static GameObject FindGameObjectByBolt(BoltEntity entity)
        {
            if (spawnedSigns.ContainsKey(entity))
            {
                return spawnedSigns[entity];
            }
            return null;
        }

        public static BoltEntity FindBoltEntityByGameObject(GameObject gameObject)
        {
            foreach (var pair in spawnedSigns)
            {
                if (pair.Value == gameObject)
                {
                    return pair.Key;
                }
            }
            return null;
        }

        public static void AddGameObject(BoltEntity entity, GameObject gameObject)
        {
            if (spawnedSigns.ContainsKey(entity))
            {
                spawnedSigns[entity] = gameObject;
            }
            else
            {
                spawnedSigns.Add(entity, gameObject);
            }
            Misc.Msg($"[Saving.Track] [AddGameObject] Added {entity} to SpawnedSigns Dict");
        }

        public static void RemoveWithBolt(BoltEntity entity)
        {
            if (spawnedSigns.ContainsKey(entity))
            {
                spawnedSigns.Remove(entity);
            }
        }

        public static void RemoveWithGameObject(GameObject gameObject)
        {
            BoltEntity entity = FindBoltEntityByGameObject(gameObject);
            if (entity != null)
            {
                RemoveWithBolt(entity);
            }
        }

        public static void Clear()
        {
            spawnedSigns.Clear();
        }
    }
}
