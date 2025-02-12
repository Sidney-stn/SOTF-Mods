using UnityEngine;

namespace WirelessSignals.Prefab
{
    internal abstract class PrefabBase
    {
        internal virtual GameObject SetupPrefab(GameObject goToInstantiate, List<Il2CppSystem.Type> componentsToAdd, Action<GameObject> configureComponents = null)  // Returns Complete Prefab Thas Setup
        {
            if (goToInstantiate == null) { throw new ArgumentNullException("[SetupPrefab] goToInstantiate Is Null!"); }

            GameObject gameObjectWithComps = GameObject.Instantiate(goToInstantiate);
            gameObjectWithComps.hideFlags = HideFlags.HideAndDontSave;

            // Add components using IL2CPP types
            if (componentsToAdd != null)
            {
                foreach (var componentType in componentsToAdd)
                {
                    gameObjectWithComps.AddComponent(componentType);
                }
            }

            // Configure components if provided
            configureComponents?.Invoke(gameObjectWithComps);

            return gameObjectWithComps;

        }

        internal abstract void ConfigureComponents(GameObject obj);

        internal abstract void Spawn(Vector3 pos, Quaternion rot);

        internal virtual bool DoesUniqueIdExist(Dictionary<string, GameObject> spawnedGameObjects, string uniqueId)
        {
            if (spawnedGameObjects.ContainsKey(uniqueId))
            {
                return true;
            }
            else
            {
                Misc.Msg($"GameObject with unique ID {uniqueId} not found.");
                return false;
            }
        }

        internal virtual GameObject FindByUniqueId(Dictionary<string, GameObject> spawnedGameObjects, string uniqueId)
        {
            if (spawnedGameObjects.TryGetValue(uniqueId, out GameObject sign))
            {
                return sign;
            }
            else
            {
                Misc.Msg($"GameObject with unique ID {uniqueId} not found.");
                return null;
            }
        }
    }
}
