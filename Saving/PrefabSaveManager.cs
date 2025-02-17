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
        }

        public void RegisterPrefabManager(string type, PrefabBase manager)
        {
            prefabManagers[type] = manager;
        }

        public string Name => "PrefabSaveManagerWirelessSignals";
        public bool IncludeInPlayerSave => false;

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

            if (Misc.hostMode == Misc.SimpleSaveGameType.NotIngame)
            {
                // Implement deferred loading if needed
                return;
            }

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
