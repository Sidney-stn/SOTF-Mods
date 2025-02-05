using RedLoader;
using Shops.Mono;
using Sons.Gui;
using TheForest.Utils;
using UnityEngine;

namespace Shops.Prefab
{
    internal class SingleShop
    {
        internal static GameObject gameObjectWithComps;
        public static Dictionary<string, GameObject> spawnedShops = new Dictionary<string, GameObject>();

        internal static void SetupPrefab()
        {
            // Setup Prefab
            if (gameObjectWithComps == null)
            {
                if (Assets.SingleShop == null) { Misc.Msg("Cant Setup Prefab, Asset is null!"); return; }
                gameObjectWithComps = GameObject.Instantiate(Assets.SingleShop);
                gameObjectWithComps.hideFlags = HideFlags.HideAndDontSave;
                Shop shop = gameObjectWithComps.AddComponent<Shop>();
                shop.isSetupPrefab = true;
                DestroyOnC destroyOnC = gameObjectWithComps.AddComponent<DestroyOnC>();
            }
        }

        internal static GameObject Spawn(Vector3 pos, Quaternion rot, string ownerName = null, string ownerId = null, string uniqueId = null, List<int> StationPrices = null, List<int> StationItems = null, List<int> StationQuantities = null, bool raiseNetworkEvent = false)
        {
            if (!string.IsNullOrEmpty(uniqueId) && !string.IsNullOrWhiteSpace(uniqueId))
            {
                bool doesShopAlreadyExsist = Prefab.SingleShop.DoesShopWithUniqueIdExist(uniqueId);
                if (doesShopAlreadyExsist)
                {
                    RLog.Warning($"Shop with unique ID {uniqueId} already exists. Skipping spawn.");
                    return null;
                }
            }
            if (!LocalPlayer.IsInWorld) { RLog.Error("Unable to spawn shop, player not in world"); return null; }
            if (gameObjectWithComps == null) { RLog.Error("Unable to spawn shop, prefab is not set up"); return null; }
            if (pos == Vector3.zero) { RLog.Error("Unable to spawn shop, position is zero"); return null; }
            if (rot == Quaternion.identity) { RLog.Error("Unable to spawn shop, rotation is zero"); return null; }
            GameObject gameObject = GameObject.Instantiate(gameObjectWithComps, pos, rot);
            Shop mono = gameObject.GetComponent<Shop>();
            if (mono == null) { RLog.Error("Unable to spawn shop, shop component is missing"); GameObject.Destroy(gameObject); return null; }
            if (string.IsNullOrEmpty(uniqueId) || string.IsNullOrWhiteSpace(uniqueId))
            {
                // Generate New Id. In case of new shop
                Misc.Msg("[Spawning] Generated New Id");
                string generatedUniqueId = Guid.NewGuid().ToString();
                if (Prefab.SingleShop.DoesShopWithUniqueIdExist(generatedUniqueId))
                {
                    Misc.Msg("[Spawning] Generated Id Already Exists, Generating New One");
                    uniqueId = Guid.NewGuid().ToString();
                } else {uniqueId = generatedUniqueId; }
                
            }
            mono.UniqueId = uniqueId;
            if (!string.IsNullOrEmpty(ownerId))
            {
                mono.OwnerId = ownerId;
            }
            else
            {
                mono.OwnerId = Banking.API.GetLocalPlayerId();
            }
            if (!string.IsNullOrEmpty(ownerName))
            {
                mono.OwnerName = ownerName;
            }
            else
            {
                mono.OwnerName = Banking.API.GetLocalPlayerName();
            }
            mono.numberOfSellableItems = 1;  // Default For SingleShop Prefab (This Prefab)

            // Shop Prefab And Shopprefab Ui should be setup in Shop Awake Method (Shop.cs) after declerad here
            if (StationPrices != null) { mono.StationPrices = StationPrices; }  // Read Point Above
            if (StationItems != null) { mono.StationItems = StationItems; }  // Read Point Above
            if (StationQuantities != null) { mono.StationQuantities = StationQuantities; }  // Read Point Above

            Saving.Load.ModdedShops.Add(gameObject);
            spawnedShops.Add(uniqueId, gameObject);

            // Network Event
            if (raiseNetworkEvent)
            {
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SpawnShop
                {
                    Vector3Position = Network.CustomSerializable.Vector3ToString(pos),
                    QuaternionRotation = Network.CustomSerializable.QuaternionToString(rot),
                    UniqueId = uniqueId,
                    Sender = Banking.API.GetLocalPlayerId(),
                    SenderName = Banking.API.GetLocalPlayerName(),
                    OwnerName = Banking.API.GetLocalPlayerName(),
                    OwnerId = Banking.API.GetLocalPlayerId(),
                    StationPrices = StationPrices,
                    StationItems = StationItems,
                    StationQuantities = StationQuantities,
                    NumberOfSellableItems = 1,
                    ToSteamId = "None"
                });
            }

            return gameObject;
        }

        internal static void TryOpenUi()
        {
            if (!LocalPlayer.IsInWorld || LocalPlayer.IsInInventory || PauseMenu.IsActive) { return; }
            Transform transform = LocalPlayer._instance._mainCam.transform;
            RaycastHit raycastHit;
            Physics.Raycast(transform.position, transform.forward, out raycastHit, 5f, LayerMask.GetMask(new string[]
            {
                "Default"
            }));
            if (raycastHit.collider == null) { return; }
            if (raycastHit.collider.transform.root == null) { return; }
            if (string.IsNullOrEmpty(raycastHit.collider.transform.root.name)) { return; }
            //Misc.Msg($"Hit: {raycastHit.collider.transform.root.name}");
            if (raycastHit.collider.transform.root.name.Contains("Shop"))
            {
                GameObject open = raycastHit.collider.transform.root.gameObject;
                Shop controller = open.GetComponent<Shop>();
                if (controller != null)
                {
                    controller.OnInteractButtonPressed();
                }
                else { Misc.Msg("Controller is null!"); }

            }
        }

        internal static void PriceAdjust(bool increase)
        {
            if (!LocalPlayer.IsInWorld || LocalPlayer.IsInInventory || PauseMenu.IsActive) { return; }
            Transform transform = LocalPlayer._instance._mainCam.transform;
            RaycastHit raycastHit;
            Physics.Raycast(transform.position, transform.forward, out raycastHit, 5f, LayerMask.GetMask(new string[]
            {
                "Default"
            }));
            if (raycastHit.collider == null) { return; }
            if (raycastHit.collider.transform.root == null) { return; }
            if (string.IsNullOrEmpty(raycastHit.collider.transform.root.name)) { return; }
            //Misc.Msg($"Hit: {raycastHit.collider.transform.root.name}");
            if (raycastHit.collider.transform.root.name.Contains("Shop"))
            {
                GameObject open = raycastHit.collider.transform.root.gameObject;
                Shop controller = open.GetComponent<Shop>();
                if (controller != null)
                {
                    controller.AdjustPriceKey(increase);
                }
                else { Misc.Msg("Controller is null!"); }

            }
        }

        public static GameObject FindShopByUniqueId(string uniqueId)
        {
            if (spawnedShops.TryGetValue(uniqueId, out GameObject sign))
            {
                return sign;
            }
            else
            {
                Misc.Msg($"Shop with unique ID {uniqueId} not found.");
                return null;
            }
        }

        public static bool DoesShopWithUniqueIdExist(string uniqueId)
        {
            if (spawnedShops.ContainsKey(uniqueId))
            {
                return true;
            }
            else
            {
                Misc.Msg($"Shop with unique ID {uniqueId} not found.");
                return false;
            }
        }
    }
}
