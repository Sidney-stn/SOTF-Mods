using SonsSdk;
using UnityEngine;

namespace Shops.Saving
{
    internal class Manager : ICustomSaveable<Manager.ShopsManager>
    {
        public string Name => "ShopsManager";

        // Used to determine if the data should also be saved in multiplayer saves
        public bool IncludeInPlayerSave => false;

        public ShopsManager Save()
        {
            if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Saving] Only Host Saves");
                return null;
            }
            var saveData = new ShopsManager();

            // Shops
            if (Saving.Load.ModdedShops.Count != 0 || Saving.Load.ModdedShops != null)
            {
                foreach (var savingGameObject in Saving.Load.ModdedShops)
                {
                    if (savingGameObject == null) { continue; }
                    Mono.Shop current_obj_controller = savingGameObject.GetComponent<Mono.Shop>();
                    if (current_obj_controller != null)
                    {
                        if (string.IsNullOrEmpty(current_obj_controller.UniqueId) || string.IsNullOrWhiteSpace(current_obj_controller.UniqueId))
                        {
                            // Generate New Id
                            Misc.Msg("[Saving] Generated New Id");
                            current_obj_controller.UniqueId = Guid.NewGuid().ToString();
                        }
                        if (current_obj_controller.UniqueId == "0")
                        {
                            Misc.Msg("[Saving] UniqueId == 0. Skipping Saving");
                            continue;
                        }
                        var ShopsModData = new ShopsManager.ShopsModData
                        {
                            UniqueId = current_obj_controller.UniqueId,
                            Position = current_obj_controller.GetPos(),
                            Rotation = current_obj_controller.GetCurrentRotation(),
                            OwnerName = current_obj_controller.OwnerName,
                            OwnerId = current_obj_controller.OwnerId,
                            Prices = current_obj_controller.StationPrices,
                            Items = current_obj_controller.StationItems,
                            Quanteties = current_obj_controller.StationQuantities,
                            NumberOfSellableItems = current_obj_controller.numberOfSellableItems
                        };

                        saveData.Shops.Add(ShopsModData);
                        Misc.Msg("[Saving] Added Shops To Save List");
                    }
                }
            }
            else { Misc.Msg("[Saving] No Shop found in LST, skipped saving"); }

            return saveData;
        }

        public void Load(ShopsManager obj)
        {

            if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.NotIngame)
            {
                // Enqueue the load data if host mode is not ready
                Misc.Msg("[Loading] Host mode not ready, deferring load.");
                Saving.Load.deferredLoadQueue.Enqueue(obj);
                return;
            }

            // Process the load data
            Saving.Load.ProcessLoadData(obj);
        }

        public class ShopsManager
        {
            public List<ShopsModData> Shops = new List<ShopsModData>();

            public class ShopsModData
            {
                public string UniqueId;
                public Vector3 Position;
                public Quaternion Rotation;
                public string OwnerName;
                public string OwnerId;
                public List<int> Prices;
                public List<int> Items;
                public List<int> Quanteties;
                public int NumberOfSellableItems;
            }

        }
    }
}
