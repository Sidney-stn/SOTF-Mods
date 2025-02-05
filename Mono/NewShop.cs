using RedLoader;
using Sons.Crafting.Structures;
using UnityEngine;

namespace Shops.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class NewShop : MonoBehaviour
    {
        public bool IsPlaceHolder = false;

        private void Start()
        {
            if (IsPlaceHolder) { Misc.Msg("[NewShop] Returned, no controller added"); return; }
            // This Only Gets Called When A New Completed Sign Has Been Created
            Misc.Msg("[NewShop] Start");
            Misc.Msg("[NewShop] Deleting Bolt And ScrewStructure");
            ScrewStructure scewStructure = gameObject.GetComponent<ScrewStructure>();
            if (scewStructure != null) { DestroyImmediate(scewStructure); Misc.SuperLog("[NewShop] [Start] ScrewStructure Deleted"); } else { Misc.SuperLog("[NewShop] [Start] ScrewStructure Not Found For Deletion!"); }
            BoltEntity bolt = gameObject.GetComponent<BoltEntity>();
            if (bolt != null)
            {
                if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.Multiplayer || Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.MultiplayerClient)
                {
                    bolt.Entity.Detach();
                }
                DestroyImmediate(bolt);
                Misc.SuperLog("[NewShop] [Start] BoltEntity Deleted");
            }
            else { Misc.SuperLog("[NewShop] [Start] BoltEntity Not Found For Deletion!"); }

            if (gameObject != null)
            {
                Misc.Msg("[NewShop] Adding Components");
                Mono.Shop controller = gameObject.AddComponent<Mono.Shop>();
                Misc.SuperLog("[NewShop] [Start] controller Added");
                Mono.DestroyOnC destroyOnC = gameObject.AddComponent<Mono.DestroyOnC>();
                Misc.SuperLog("[NewShop] [Start] DestroyOnC Added");

                string uniqueId = Guid.NewGuid().ToString();

                controller.UniqueId = uniqueId;
                controller.numberOfSellableItems = 1;
                controller.OwnerName = Banking.API.GetLocalPlayerName();
                controller.OwnerId = Banking.API.GetLocalPlayerId();

                Saving.Load.ModdedShops.Add(gameObject);
                Prefab.SingleShop.spawnedShops.Add(controller.UniqueId, gameObject);

                if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.Multiplayer || Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.MultiplayerClient)
                {
                    Misc.Msg("[NewShop] Spawning On Multiplayer");
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SpawnShop
                    {
                        Vector3Position = Network.CustomSerializable.Vector3ToString(controller.GetPos()),
                        QuaternionRotation = Network.CustomSerializable.QuaternionToString(controller.GetCurrentRotation()),
                        UniqueId = uniqueId,
                        Sender = Banking.API.GetLocalPlayerId(),
                        SenderName = Banking.API.GetLocalPlayerName(),
                        OwnerName = Banking.API.GetLocalPlayerName(),
                        OwnerId = Banking.API.GetLocalPlayerId(),
                        StationPrices = null,
                        StationItems = null,
                        StationQuantities = null,
                        NumberOfSellableItems = 1,
                        ToSteamId = "None"
                    });
                }
            }
            else { Misc.Msg("[OnStructureCompleted] signChild == null"); Misc.SuperLog("[NewShop] [Start] GameObject That NewShop Is Attatched To Is Null!!!!"); }

            Misc.SuperLog("[NewShop] [Start] Destroying NewShop Component");
            Destroy(this);
        }
    }
}
