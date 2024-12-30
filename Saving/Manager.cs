using Endnight.Extensions;
using SonsSdk;
using UnityEngine;

namespace Banking.Saving
{
    internal class Manager : ICustomSaveable<Manager.BankingManager>
    {
        public string Name => "BankingManager";

        // Used to determine if the data should also be saved in multiplayer saves
        public bool IncludeInPlayerSave => false;

        public BankingManager Save()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Saving] Only Host Saves");
                return new BankingManager();
            }
            var saveData = new BankingManager();

            // Atms
            if (Saving.Load.ModdedAtms != null && Saving.Load.ModdedAtms.Count != 0)
            {
                foreach (var atmGameObject in Saving.Load.ModdedAtms)
                {
                    if (atmGameObject == null) { continue; }
                    Mono.ATMController current_obj_controller = atmGameObject.GetComponent<Mono.ATMController>();
                    if (current_obj_controller != null)
                    {
                        if (current_obj_controller.UniqueId.IsNullOrWhitespace() || current_obj_controller.UniqueId == null)
                        {
                            // Generate New Id
                            Misc.Msg("[Saving] Generated New Id For ATM");
                            current_obj_controller.UniqueId = Guid.NewGuid().ToString();
                        }
                        if (current_obj_controller.UniqueId == "0")
                        {
                            Misc.Msg("[Saving] UniqueId == 0. Skipping Saving Of ATM");
                            continue;
                        }
                        var atmModData = new BankingManager.ATMModData
                        {
                            UniqueId = current_obj_controller.UniqueId,
                            Position = current_obj_controller.GetPos(),
                            Rotation = current_obj_controller.GetCurrentRotation(),
                        };

                        saveData.Atms.Add(atmModData);
                        Misc.Msg("[Saving] Added ATM To Save List");
                    }
                }
            }
            else { Misc.Msg("[Saving] No ATM found in LST, skipped saving"); }

            // ATM Placers
            if (Saving.Load.ModdedATMPlacers != null && Saving.Load.ModdedATMPlacers.Count != 0)
            {
                foreach (var atmPlacerGameObject in Saving.Load.ModdedATMPlacers)
                {
                    if (atmPlacerGameObject == null) { continue; }
                    Mono.ATMPlacerController current_obj_controller = atmPlacerGameObject.GetComponent<Mono.ATMPlacerController>();
                    if (current_obj_controller != null)
                    {
                        if (current_obj_controller.UniqueId.IsNullOrWhitespace() || current_obj_controller.UniqueId == null)
                        {
                            // Generate New Id
                            Misc.Msg("[Saving] Generated New Id For ATM Placer");
                            current_obj_controller.UniqueId = Guid.NewGuid().ToString();
                        }
                        if (current_obj_controller.UniqueId == "0")
                        {
                            Misc.Msg("[Saving] UniqueId == 0. Skipping Saving Of ATM Placer");
                            continue;
                        }
                        var atmPlacerModData = new BankingManager.ATMPlacerModData
                        {
                            UniqueId = current_obj_controller.UniqueId,
                            Position = current_obj_controller.GetPos(),
                            Rotation = current_obj_controller.GetCurrentRotation(),
                            ATMPlacerData = current_obj_controller.GetAddedObjects()
                        };

                        saveData.ATMPlacers.Add(atmPlacerModData);
                        Misc.Msg("[Saving] Added ATM Placer To Save List");
                    }
                }
            }
            else { Misc.Msg("[Saving] No ATM Placer found in LST, skipped saving"); }


            saveData.SavedPlayers = LiveData.Players.GetPlayers();
            saveData.SavedPlayersCurrency = LiveData.Players.GetPlayersCurrency();

            return saveData;
        }

        public void Load(BankingManager obj)
        {

            if (Misc.hostMode == Misc.SimpleSaveGameType.NotIngame)
            {
                // Enqueue the load data if host mode is not ready
                Misc.Msg("[Loading] Host mode not ready, deferring load.");
                Saving.Load.deferredLoadQueue.Enqueue(obj);
                return;
            }

            // Process the load data
            Saving.Load.ProcessLoadData(obj);
        }

        public class BankingManager
        {
            public Dictionary<string, string> SavedPlayers = new Dictionary<string, string>();  // PlayerSteamID, PlayerName
            public Dictionary<string, int> SavedPlayersCurrency = new Dictionary<string, int>();  // PlayerSteamID, PlayerCash

            public List<ATMModData> Atms = new List<ATMModData>();
            public List<ATMPlacerModData> ATMPlacers = new List<ATMPlacerModData>();

            public class ATMModData
            {
                public string UniqueId;
                public Vector3 Position;
                public Quaternion Rotation;
            }

            public class ATMPlacerModData
            {
                public string UniqueId;
                public Vector3 Position;
                public Quaternion Rotation;
                public Dictionary<int, int> ATMPlacerData;
            }
        }
    }
}
