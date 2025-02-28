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

        [Serializable]
        public class AllPrefabsData
        {
            public ManagerSaveData SaveData = new ManagerSaveData();
        }

        [Serializable]
        public class ManagerSaveData
        {
            public List<Prefab.Reciver.ReceiverSaveData> ReceiverItems = new List<Prefab.Reciver.ReceiverSaveData>();
            public List<Prefab.WirelessTransmitterSwitch.TransmitterSwitchSaveData> SwitchItems = new List<Prefab.WirelessTransmitterSwitch.TransmitterSwitchSaveData>();
            public List<Prefab.TransmitterDetector.TransmitterDetectorSaveData> DetectorItems = new List<Prefab.TransmitterDetector.TransmitterDetectorSaveData>();

            // For Saving Lines
            public Dictionary<string, List<Vector3>> LinkedLines = new Dictionary<string, List<Vector3>>();
        }

        private void ProcessLoadData(AllPrefabsData data)
        {
            // On Host This Works Perfect, On Client I Want To Skip This (On Client now i get Object Referecne error)
            // Check If data is null
            if (data == null)
            {
                Misc.Msg("[Loading] Data Is Null");
                return;
            }
            if (prefabManagers.TryGetValue("Receiver", out var receiverManager))
            {
                foreach (var item in data.SaveData.ReceiverItems)
                {
                    try
                    {
                        receiverManager.LoadFromSaveData(item);
                    }
                    catch (Exception ex)
                    {
                        Misc.Msg($"[Error] Failed to load receiver: {ex.Message}");
                    }
                }
            }

            if (prefabManagers.TryGetValue("TransmitterSwitch", out var switchManager))
            {
                foreach (var item in data.SaveData.SwitchItems)
                {
                    try
                    {
                        switchManager.LoadFromSaveData(item);
                    }
                    catch (Exception ex)
                    {
                        Misc.Msg($"[Error] Failed to load switch: {ex.Message}");
                    }
                }
            }

            if (prefabManagers.TryGetValue("Detector", out var detectorManager))
            {
                foreach (var item in data.SaveData.DetectorItems)
                {
                    try
                    {
                        detectorManager.LoadFromSaveData(item);
                    }
                    catch (Exception ex)
                    {
                        Misc.Msg($"[Error] Failed to load detector: {ex.Message}");
                    }
                }
            }

            // Load Linked Lines
            if (WirelessSignals.linkingCotroller != null)
            {
                WirelessSignals.linkingCotroller.LoadLinkedLines(data.SaveData.LinkedLines);
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

            if (prefabManagers.TryGetValue("Receiver", out var receiverManager))
            {
                saveData.SaveData.ReceiverItems = receiverManager.GetAllSaveData()
                    .Cast<Reciver.ReceiverSaveData>().ToList();

                // Loop through all recivers and log their uniqueId, position, and isOn
                foreach (var item in saveData.SaveData.ReceiverItems)
                {
                    Misc.Msg($"[Saving] Receiver: {item.UniqueId} {item.Position} {item.IsOn}");
                }
            }

            if (prefabManagers.TryGetValue("TransmitterSwitch", out var switchManager))
            {
                saveData.SaveData.SwitchItems = switchManager.GetAllSaveData()
                    .Cast<WirelessTransmitterSwitch.TransmitterSwitchSaveData>().ToList();
            }

            if (prefabManagers.TryGetValue("Detector", out var detectorManager))
            {
                saveData.SaveData.DetectorItems = detectorManager.GetAllSaveData()
                    .Cast<TransmitterDetector.TransmitterDetectorSaveData>().ToList();
            }

            // Log Save Data Prefabs Count Saved
            Misc.Msg($"[Saving] Saved {saveData.SaveData.ReceiverItems.Count} Receiver, {saveData.SaveData.SwitchItems.Count} Switch, {saveData.SaveData.DetectorItems.Count} Detector");

            // Save Linked Lines
            if (WirelessSignals.linkingCotroller != null)
            {
                saveData.SaveData.LinkedLines = WirelessSignals.linkingCotroller.GetLinkedLinesForSaving();
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

    }
}
