//using SonsSdk;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.Baselib.LowLevel;
//using UnityEngine;
//using WirelessSignals.Prefab;

//namespace WirelessSignals.Saving
//{
//    internal class Test
//    {
//        [Serializable]
//        public class SaveData
//        {
//            public string UniqueId { get; set; }
//            public Vector3 Position { get; set; }
//            public Quaternion Rotation { get; set; }
//        }

//        // Base class for specific save data types
//        [Serializable]
//        public class ReceiverSaveData : SaveData
//        {
//            public bool? IsOn { get; set; }
//            // Add other receiver-specific properties
//        }

//        // Modified PrefabBase to handle save/load operations
//        internal abstract class PrefabBase
//        {
//            internal virtual GameObject gameObjectWithComps { get; set; }
//            public virtual Dictionary<string, GameObject> spawnedGameObjects { get; set; } = new Dictionary<string, GameObject>();

//            // Abstract methods for save/load operations
//            protected abstract SaveData CreateSaveDataFromGameObject(GameObject obj);
//            protected abstract SpawnParameters CreateSpawnParametersFromSaveData(SaveData data);
//            protected abstract void ApplySaveDataToGameObject(GameObject obj, SaveData data);

//            internal virtual List<SaveData> GetAllSaveData()
//            {
//                var allSaveData = new List<SaveData>();
//                foreach (var obj in spawnedGameObjects.Values)
//                {
//                    if (obj != null)
//                    {
//                        allSaveData.Add(CreateSaveDataFromGameObject(obj));
//                    }
//                }
//                return allSaveData;
//            }

//            internal virtual void LoadFromSaveData(SaveData data)
//            {
//                if (data == null) return;

//                var parameters = CreateSpawnParametersFromSaveData(data);
//                var spawnedObject = Spawn(parameters);

//                if (spawnedObject != null)
//                {
//                    ApplySaveDataToGameObject(spawnedObject, data);
//                }
//            }

//            // Your existing PrefabBase methods...
//            internal abstract void Setup();
//            internal abstract void ConfigureComponents(GameObject obj);
//            internal abstract GameObject Spawn(SpawnParameters parameters);
//        }

//        // Example implementation for Receiver
//        internal class Receiver : PrefabBase
//        {
//            protected override SaveData CreateSaveDataFromGameObject(GameObject obj)
//            {
//                var component = obj.GetComponent<Mono.Reciver>();
//                return new ReceiverSaveData
//                {
//                    UniqueId = component.uniqueId,
//                    Position = obj.transform.position,
//                    Rotation = obj.transform.rotation,
//                    IsOn = component.isOn
//                };
//            }

//            protected override SpawnParameters CreateSpawnParametersFromSaveData(SaveData data)
//            {
//                var receiverData = data as ReceiverSaveData;
//                if (receiverData == null)
//                    throw new ArgumentException("Invalid save data type");

//                return new Prefab.ReciverSpawnParameters
//                {
//                    position = receiverData.Position,
//                    rotation = receiverData.Rotation,
//                    uniqueId = receiverData.UniqueId,
//                    isOn = receiverData.IsOn
//                };
//            }

//            protected override void ApplySaveDataToGameObject(GameObject obj, SaveData data)
//            {
//                var receiverData = data as ReceiverSaveData;
//                if (receiverData == null) return;

//                var component = obj.GetComponent<Mono.Reciver>();
//                if (component != null)
//                {
//                    component.uniqueId = receiverData.UniqueId;
//                    component.isOn = receiverData.IsOn;
//                }
//            }

//            // Rest of your receiver implementation...
//        }

//        // Unified save manager for all prefab-based objects
//        internal class PrefabSaveManager : ICustomSaveable<PrefabSaveManager.AllPrefabsData>
//        {
//            private readonly Dictionary<string, PrefabBase> prefabManagers;

//            public PrefabSaveManager()
//            {
//                prefabManagers = new Dictionary<string, PrefabBase>();
//            }

//            public void RegisterPrefabManager(string type, PrefabBase manager)
//            {
//                prefabManagers[type] = manager;
//            }

//            public string Name => "PrefabSaveManager";
//            public bool IncludeInPlayerSave => false;

//            public AllPrefabsData Save()
//            {
//                if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
//                {
//                    Misc.Msg("[Saving] Only Host Saves");
//                    return null;
//                }

//                var saveData = new AllPrefabsData();

//                foreach (var entry in prefabManagers)
//                {
//                    var managerSaveData = new ManagerSaveData
//                    {
//                        ManagerType = entry.Key,
//                        Items = entry.Value.GetAllSaveData()
//                    };
//                    saveData.ManagerData.Add(managerSaveData);
//                }

//                return saveData;
//            }

//            public void Load(AllPrefabsData data)
//            {
//                if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
//                {
//                    Misc.Msg("[Loading] Skipped Loading Objects On Multiplayer Client");
//                    return;
//                }

//                if (Misc.hostMode == Misc.SimpleSaveGameType.NotIngame)
//                {
//                    // Implement deferred loading if needed
//                    return;
//                }

//                foreach (var managerData in data.ManagerData)
//                {
//                    if (prefabManagers.TryGetValue(managerData.ManagerType, out var manager))
//                    {
//                        foreach (var itemData in managerData.Items)
//                        {
//                            manager.LoadFromSaveData(itemData);
//                        }
//                    }
//                }
//            }

//            [Serializable]
//            public class AllPrefabsData
//            {
//                public List<ManagerSaveData> ManagerData { get; set; } = new List<ManagerSaveData>();
//            }

//            [Serializable]
//            public class ManagerSaveData
//            {
//                public string ManagerType { get; set; }
//                public List<SaveData> Items { get; set; } = new List<SaveData>();
//            }
//        }
//    }
//}
