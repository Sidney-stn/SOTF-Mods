using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SonsSdk;
using UnityEngine;
using WirelessSignals.Prefab;

namespace WirelessSignals.Saving
{
    internal class PrefabSaveManager : ICustomSaveable<PrefabSaveManager.AllPrefabsData>
    {
        private readonly Dictionary<string, PrefabBase> prefabManagers;
        private static Queue<AllPrefabsData> deferredLoadQueue = new Queue<AllPrefabsData>();
        private static bool isWorldReady = false;

        public PrefabSaveManager()
        {
            prefabManagers = new Dictionary<string, PrefabBase>();
            isWorldReady = false;
        }

        public void RegisterPrefabManager(string type, PrefabBase manager)
        {
            prefabManagers[type] = manager;

            // If we have deferred data and all managers are registered, process it
            if (deferredLoadQueue.Count > 0 && isWorldReady)
            {
                ProcessDeferredLoads();
            }
        }

        public string Name => "PrefabSaveManager";
        public bool IncludeInPlayerSave => false;

        public void SetWorldReady()
        {
            isWorldReady = true;
            ProcessDeferredLoads();
        }

        private void ProcessDeferredLoads()
        {
            while (deferredLoadQueue.Count > 0)
            {
                var data = deferredLoadQueue.Dequeue();
                ProcessLoadData(data);
            }
        }

        private SaveData ConvertSaveData(JObject itemData, string managerType)
        {
            try
            {
                // Create base properties
                var baseData = new SaveData
                {
                    UniqueId = itemData["UniqueId"]?.ToString(),
                    Position = new Vector3(
                        float.Parse(itemData["Position"]["x"].ToString()),
                        float.Parse(itemData["Position"]["y"].ToString()),
                        float.Parse(itemData["Position"]["z"].ToString())
                    ),
                    Rotation = new Quaternion(
                        float.Parse(itemData["Rotation"]["x"].ToString()),
                        float.Parse(itemData["Rotation"]["y"].ToString()),
                        float.Parse(itemData["Rotation"]["z"].ToString()),
                        float.Parse(itemData["Rotation"]["w"].ToString())
                    )
                };

                // Then create the appropriate save data type
                switch (managerType)
                {
                    case "TransmitterSwitch":
                        return new TransmitterSwitchSaveData
                        {
                            UniqueId = baseData.UniqueId,
                            Position = baseData.Position,
                            Rotation = baseData.Rotation,
                            IsOn = itemData["IsOn"] != null ? bool.Parse(itemData["IsOn"].ToString()) : null
                        };
                    case "Receiver":
                        return new ReceiverSaveData
                        {
                            UniqueId = baseData.UniqueId,
                            Position = baseData.Position,
                            Rotation = baseData.Rotation,
                            IsOn = itemData["IsOn"] != null ? bool.Parse(itemData["IsOn"].ToString()) : null
                        };
                    case "Detector":
                        return new TransmitterDetectorSaveData
                        {
                            UniqueId = baseData.UniqueId,
                            Position = baseData.Position,
                            Rotation = baseData.Rotation,
                            IsOn = itemData["IsOn"] != null ? bool.Parse(itemData["IsOn"].ToString()) : null
                        };
                    default:
                        return baseData;
                }
            }
            catch (Exception ex)
            {
                Misc.Msg($"[Error] Failed to convert save data: {ex.Message}");
                throw;
            }
        }

        private void ProcessLoadData(AllPrefabsData data)
        {
            foreach (var managerData in data.ManagerData)
            {
                if (prefabManagers.TryGetValue(managerData.ManagerType, out var manager))
                {
                    foreach (var itemData in managerData.Items)
                    {
                        try
                        {
                            var json = JsonConvert.SerializeObject(itemData);
                            var jObject = JObject.Parse(json);
                            var typedData = ConvertSaveData(jObject, managerData.ManagerType);
                            manager.LoadFromSaveData(typedData);
                        }
                        catch (Exception ex)
                        {
                            Misc.Msg($"[Error] Failed to load prefab: {ex.Message}");
                        }
                    }
                }
            }
        }

        public AllPrefabsData Save()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Saving] Only Host Saves");
                return null;
            }

            var saveData = new AllPrefabsData();

            foreach (var entry in prefabManagers)
            {
                var managerSaveData = new ManagerSaveData
                {
                    ManagerType = entry.Key,
                    Items = entry.Value.GetAllSaveData()
                };
                saveData.ManagerData.Add(managerSaveData);
            }

            return saveData;
        }

        public void Load(AllPrefabsData data)
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Loading] Skipped Loading Objects On Multiplayer Client");
                return;
            }

            if (!isWorldReady || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame)
            {
                Misc.Msg("[Loading] World not ready, deferring load");
                deferredLoadQueue.Enqueue(data);
                return;
            }

            ProcessLoadData(data);
        }

        [Serializable]
        public class AllPrefabsData
        {
            public List<ManagerSaveData> ManagerData { get; set; } = new List<ManagerSaveData>();
        }

        [Serializable]
        public class ManagerSaveData
        {
            public string ManagerType { get; set; }
            public List<SaveData> Items { get; set; } = new List<SaveData>();
        }

    }
}
