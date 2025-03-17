using SonsSdk;
using TheForest.Utils;

namespace StoneGate.Saving
{
    internal class Manager : ICustomSaveable<Manager.GatesManager>
    {
        public string Name => "GatesManager";

        // Used to determine if the data should also be saved in multiplayer saves
        public bool IncludeInPlayerSave => false;
        public GatesManager Save()
        {
            if (BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                if (Testing.Settings.logSavingSystem)
                    Misc.Msg("[Saving] Skipped Saving StoneGates On Multiplayer Client");
                return null;
            }

            var saveData = new GatesManager();

            // Get All Stored Gates Mones
            var mono = Tools.Gates.GetAllStoneGateStoreMono();
            if (mono == null)
            {
                if (Testing.Settings.logSavingSystem)
                    Misc.Msg("[Saving] No StoneGateStoreMono Found");
                return null;
            }
            if (mono.Count == 0)
            {
                if (Testing.Settings.logSavingSystem)
                    Misc.Msg("[Saving] No StoneGateStoreMono Found");
                return null;
            }
            if (Testing.Settings.logSavingSystem)
                Misc.Msg($"[Saving] Found {mono.Count} StoneGateStoreMono");

            int savedGates = 0;
            foreach (var controller in mono)
            {
                if (controller == null)
                {
                    if (Testing.Settings.logSavingSystem)
                        Misc.Msg("[Saving] Controller IS NULL");
                    continue;
                }

                saveData.Gates.Add(controller.GetSaveData());
                savedGates++;
            }
            if (Testing.Settings.logSavingSystem)
                Misc.Msg($"[Saving] Saved {savedGates} StoneGates");

            return saveData;
        }

        public void Load(GatesManager obj)
        {
            if (!LocalPlayer.IsInWorld)
            {
                // Enqueue the load data if host mode is not ready
                Misc.Msg("[Loading] Host mode not ready, deferring load.");
                Saving.Load.deferredLoadQueue.Enqueue(obj);
                return;
            }

            // Process the load data
            Saving.Load.ProcessLoadData(obj);
        }

        public class GatesManager
        {
            public List<GatesModData> Gates = new List<GatesModData>();

            public class GatesModData
            {
                public string Mode;
                public string FloorBeamName;
                public string TopBeamName;
                public string RockWallName;
                public string ExtraPillarName;
                public string RotationGoName;
            }

        }
    }
}
