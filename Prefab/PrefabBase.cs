using SonsSdk;
using UnityEngine;

namespace WirelessSignals.Prefab
{
    internal abstract class PrefabBase
    {
        internal virtual GameObject gameObjectWithComps { get; set; }
        public virtual Dictionary<string, GameObject> spawnedGameObjects { get; set; } = new Dictionary<string, GameObject>(); // UniqueId, GameObject

        internal virtual GameObject SetupPrefab(GameObject goToInstantiate, List<Il2CppSystem.Type> componentsToAdd, Action<GameObject> configureComponents = null)  // Returns Complete Prefab Thas Setup
        {
            if (goToInstantiate == null) { throw new ArgumentNullException("[SetupPrefab] goToInstantiate Is Null!"); }

            gameObjectWithComps = GameObject.Instantiate(goToInstantiate);
            gameObjectWithComps.HideAndDontSave().DontDestroyOnLoad();

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

        internal abstract GameObject Spawn(SpawnParameters parameters);

        internal virtual bool DoesUniqueIdExist(string uniqueId)
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

        internal virtual GameObject FindByUniqueId(string uniqueId)
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
        internal abstract void Setup();

        // Abstract methods for save/load operations
        protected abstract object CreateSaveDataFromGameObject(GameObject obj);
        protected abstract SpawnParameters CreateSpawnParametersFromSaveData(object data);
        protected abstract void ApplySaveDataToGameObject(GameObject obj, object data);

        internal virtual List<object> GetAllSaveData()
        {
            var allSaveData = new List<object>();
            foreach (var obj in spawnedGameObjects.Values)
            {
                if (obj != null)
                {
                    allSaveData.Add(CreateSaveDataFromGameObject(obj));
                }
            }
            return allSaveData;
        }

        internal virtual void LoadFromSaveData(object data)
        {
            if (data == null) return;

            var parameters = CreateSpawnParametersFromSaveData(data);
            var spawnedObject = Spawn(parameters);

            if (spawnedObject != null)
            {
                ApplySaveDataToGameObject(spawnedObject, data);
            }
        }
    }
}
