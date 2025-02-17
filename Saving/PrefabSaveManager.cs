using Newtonsoft.Json.Linq;
using SonsSdk;
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

        public string Name => "PrefabSaveManagerWirelessSignals";
        public bool IncludeInPlayerSave => false;

        public void SetWorldReady()
        {
            isWorldReady = true;
            // Process any queued loads once the world is ready
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
            // Convert based on manager type
            switch (managerType)
            {
                case "TransmitterSwitch":
                    return itemData.ToObject<TransmitterSwitchSaveData>();
                case "Receiver":
                    return itemData.ToObject<ReceiverSaveData>();
                case "Detector":
                    return itemData.ToObject<TransmitterDetectorSaveData>();
                default:
                    return itemData.ToObject<SaveData>();
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
                        manager.LoadFromSaveData(itemData);
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
                // Implement deferred loading
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
