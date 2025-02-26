using Il2CppInterop.Runtime;
using SonsSdk;
using SonsSdk.Networking;
using UnityEngine;

namespace WirelessSignals.Prefab
{
    internal abstract class PrefabBase
    {
        internal virtual GameObject gameObjectWithComps { get; set; }
        public virtual Dictionary<string, GameObject> spawnedGameObjects { get; set; } = new Dictionary<string, GameObject>(); // UniqueId, GameObject
        internal virtual int structureId { get; set; }

        internal virtual GameObject SetupPrefab(GameObject goToInstantiate, List<Il2CppSystem.Type> componentsToAdd, Action<GameObject> configureComponents = null, bool addGrassAndSnowRemover = false)  // Returns Complete Prefab Thas Setup
        {
            if (goToInstantiate == null) { throw new ArgumentNullException("[SetupPrefab] goToInstantiate Is Null!"); }

            gameObjectWithComps = GameObject.Instantiate(goToInstantiate);
            gameObjectWithComps.HideAndDontSave().DontDestroyOnLoad();

            // Add components using IL2CPP types
            if (componentsToAdd != null)
            {
                foreach (var componentType in componentsToAdd)
                {
                    // First Check If Its BoltEntity Component
                    if (componentType == Il2CppType.Of<BoltEntity>())
                    {
                        if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
                        {
                            if (structureId != 0)
                            {
                                BoltEntity boltEntity = gameObjectWithComps.AddComponent<BoltEntity>();
                                boltEntity.Init(structureId, BoltFactories.RigidbodyState);
                                Misc.Msg("[PrefabBase] [SetupPrefab] BoltEntity Component Added In Multiplayer Mode");
                                //EntityManager.RegisterPrefab(boltEntity);
                                continue;
                            }
                            else
                            {
                                Misc.Msg("[PrefabBase] [SetupPrefab] StructureId Is 0, Can't Add BoltEntity Component In Multiplayer Mode");
                                continue;
                            }

                        }
                        else
                        {
                            Misc.Msg("[PrefabBase] [SetupPrefab] BoltEntity Component Not Added In SinglePlayer Mode");
                            continue;
                        }
                    }
                    else
                    {
                        Misc.Msg($"[PrefabBase] [SetupPrefab] Adding Component {componentType}");
                        gameObjectWithComps.AddComponent(componentType);
                    }
                }
            }

            // Configure components if provided
            configureComponents?.Invoke(gameObjectWithComps);

            if (addGrassAndSnowRemover)
            {
                CleanGrassAndSnow();
            }

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

        internal abstract List<object> GetAllSaveData();

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

        internal virtual void CleanGrassAndSnow(GameObject addToObj = null)
        {
            if (addToObj == null) { addToObj = gameObjectWithComps; }
            if (gameObjectWithComps == null) { Misc.Msg("[PrefabBase] [CleanGrassAndSnow] Can't add to null object"); return; }
            Bolt.PrefabId prefabId = BoltPrefabs.StorageFirewoodStructure;  // Find StorageFirewoodStructure PrefabId for copying GameObjects
            Bolt.PrefabId nullId = new Bolt.PrefabId(0);
            if (prefabId == nullId) { Misc.Msg("[PrefabBase] [CleanGrassAndSnow] Can't find prefabId"); return; }
            GameObject obj = Bolt.PrefabDatabase.Find(prefabId);  // Find StorageFirewoodStructure GameObject for copying GameObjects
            if (obj == null) { Misc.Msg("[PrefabBase] [CleanGrassAndSnow] Can't find prefabId"); return; }
            GameObject grassRemover = obj.transform.FindChild("GrassRemover").gameObject;  // Find GrassRemover GameObject
            if (grassRemover != null)
            {
                GameObject grassRemoverCopy = GameObject.Instantiate(grassRemover);  // Copy GrassRemover GameObject
                grassRemoverCopy.SetParent(addToObj.transform);  // Set GrassRemover GameObject as child of addToObj
            }
            else
            {
                Misc.Msg("[PrefabBase] [CleanGrassAndSnow] Can't find GrassRemover"); return;
            }
            GameObject snowRemover = obj.transform.FindChild("SnowRemover").gameObject;  // Find SnowRemover GameObject
            if (snowRemover != null)
            {
                GameObject snowRemoverCopy = GameObject.Instantiate(snowRemover);  // Copy SnowRemover GameObject
                snowRemoverCopy.SetParent(addToObj.transform);  // Set SnowRemover GameObject as child of addToObj
            }
            else
            {
                Misc.Msg("[PrefabBase] [CleanGrassAndSnow] Can't find SnowRemover"); return;
            }
            GameObject structureEnvironmentCleaner = obj.transform.FindChild("StructureEnvironmentCleaner").gameObject;  // Find StructureEnvironmentCleaner GameObject
            if (structureEnvironmentCleaner != null)
            {
                GameObject structureEnvironmentCleanerCopy = GameObject.Instantiate(structureEnvironmentCleaner);  // Copy StructureEnvironmentCleaner GameObject
                structureEnvironmentCleanerCopy.SetParent(addToObj.transform);  // Set StructureEnvironmentCleaner GameObject as child of addToObj
            }
            else
            {
                Misc.Msg("[PrefabBase] [CleanGrassAndSnow] Can't find StructureEnvironmentCleaner"); return;
            }
        }
    }
}
