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
                return null;
            }
            var saveData = new BankingManager();

            // Atms
            if (Saving.Load.ModdedAtms.Count != 0 || Saving.Load.ModdedAtms != null)
            {
                foreach (var signsGameObject in Saving.Load.ModdedAtms)
                {
                    if (signsGameObject == null) { continue; }
                    Mono.ATMController current_obj_controller = signsGameObject.GetComponent<Mono.ATMController>();
                    if (current_obj_controller != null)
                    {
                        if (current_obj_controller.UniqueId.IsNullOrWhitespace() || current_obj_controller.UniqueId == null)
                        {
                            // Generate New Id
                            Misc.Msg("[Saving] Generated New Id For Sign");
                            current_obj_controller.UniqueId = Guid.NewGuid().ToString();
                        }
                        if (current_obj_controller.UniqueId == "0")
                        {
                            Misc.Msg("[Saving] UniqueId == 0. Skipping Saving Of Sign");
                            continue;
                        }
                        var SignsModData = new BankingManager.SignsModData
                        {
                            UniqueId = current_obj_controller.UniqueId,
                            Position = current_obj_controller.GetPos(),
                            Rotation = current_obj_controller.GetCurrentRotation(),
                        };

                        saveData.Atms.Add(SignsModData);
                        Misc.Msg("[Saving] Added Sign To Save List");
                    }
                }
            }
            else { Misc.Msg("[Saving] No Sign found in LST, skipped saving"); }

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
            public List<SignsModData> Atms = new List<SignsModData>();

            public class SignsModData
            {
                public string UniqueId;
                public Vector3 Position;
                public Quaternion Rotation;
            }

        }
    }
}
