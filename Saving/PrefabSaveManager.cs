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


        private void ProcessLoadData(AllPrefabsData data)
        {
            foreach (var managerData in data.ManagerData)
            {
                if (prefabManagers.TryGetValue(managerData.ManagerType, out var manager))
                {
                    // Cast Items back to list of SaveData
                    var items = managerData.Items as List<SaveData>;
                    if (items != null)
                    {
                        foreach (var itemData in items)
                        {
                            try
                            {
                                manager.LoadFromSaveData(itemData);
                            }
                            catch (Exception ex)
                            {
                                Misc.Msg($"[Error] Failed to load prefab: {ex.Message}");
                            }
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
                var items = entry.Value.GetAllSaveData();
                var managerSaveData = new ManagerSaveData
                {
                    ManagerType = entry.Key,
                    Items = entry.Key switch
                    {
                        "TransmitterSwitch" => items.Cast<TransmitterSwitchSaveData>().ToList(),
                        "Receiver" => items.Cast<ReceiverSaveData>().ToList(),
                        "Detector" => items.Cast<TransmitterDetectorSaveData>().ToList(),
                        _ => items
                    }
                };
                saveData.ManagerData.Add(managerSaveData);
            }
            Misc.Msg($"[Saving] Saved {saveData.ManagerData.Count} prefab managers");
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
            public dynamic Items { get; set; }

            public ManagerSaveData()
            {
                // Initialize based on manager type
                Items = new List<SaveData>();
            }
        }

    }
}
