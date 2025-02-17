using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SonsSdk;
using Unity.Baselib.LowLevel;
using UnityEngine;
using WirelessSignals.Prefab;
using WirelessSignals.Saving.WirelessSignals.Saving;

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

        private SaveData ConvertToCorrectType(string json, string managerType)
        {
            try
            {
                switch (managerType)
                {
                    case "TransmitterSwitch":
                        return JsonConvert.DeserializeObject<TransmitterSwitchSaveData>(json);
                    case "Receiver":
                        return JsonConvert.DeserializeObject<ReceiverSaveData>(json);
                    case "Detector":
                        return JsonConvert.DeserializeObject<TransmitterDetectorSaveData>(json);
                    default:
                        return JsonConvert.DeserializeObject<SaveData>(json);
                }
            }
            catch (Exception ex)
            {
                Misc.Msg($"[Error] Failed to convert save data: {ex.Message}");
                return null;
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
                            var serializableData = SerializableSaveData.FromSaveData(itemData);
                            string jsonString = JsonConvert.SerializeObject(serializableData);
                            SaveData typedData;

                            switch (managerData.ManagerType)
                            {
                                case "TransmitterSwitch":
                                    var switchData = JsonConvert.DeserializeObject<SerializableTransmitterSwitchData>(jsonString);
                                    typedData = switchData.ToSaveData();
                                    break;
                                // Add cases for Receiver and Detector
                                default:
                                    var baseData = JsonConvert.DeserializeObject<SerializableSaveData>(jsonString);
                                    typedData = baseData.ToSaveData();
                                    break;
                            }

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
                try
                {
                    var items = entry.Value.GetAllSaveData();
                    var managerSaveData = new ManagerSaveData
                    {
                        ManagerType = entry.Key,
                        Items = items
                    };
                    saveData.ManagerData.Add(managerSaveData);
                }
                catch (Exception ex)
                {
                    Misc.Msg($"[Error] Failed to save prefab manager {entry.Key}: {ex.Message}");
                }
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
