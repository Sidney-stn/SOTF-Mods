using Endnight.Types;
using Sons.Gui.Input;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;

namespace Shops.Testing
{
    internal static class Tools
    {
        static GameObject GetLookingAtShop()
        {
            Transform transform = LocalPlayer._instance._mainCam.transform;
            RaycastHit raycastHit;
            Physics.Raycast(transform.position, transform.forward, out raycastHit, 5f, LayerMask.GetMask(new string[]
            {
                "Default"
            }));
            if (raycastHit.collider == null) { return null; }
            if (raycastHit.collider.transform.root == null) { return null; }
            if (string.IsNullOrEmpty(raycastHit.collider.transform.root.name)) { return null; }
            if (raycastHit.collider.transform.root.name.Contains("Shop"))
            {
                GameObject open = raycastHit.collider.transform.root.gameObject;
                return open;

            }
            return null;
        }
        static string GetShopOwnerName()
        {
            GameObject shop = GetLookingAtShop();
            if (shop == null) { return null; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                return controller.OwnerName;
            }
            return null;
        }
        static string GetShopOwnerName(GameObject shop)
        {
            if (shop == null) { return null; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                return controller.OwnerName;
            }
            return null;
        }
        static void SetShopOwnerName(string name)
        {
            GameObject shop = GetLookingAtShop();
            if (shop == null) { return; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                controller.SetOwnerText(name);
            }
        }
        static void SetShopOwnerName(GameObject shop, string name)
        {
            if (shop == null) { return; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                controller.SetOwnerText(name);
            }
        }
        static string GetShopUnqiueId()
        {
            GameObject shop = GetLookingAtShop();
            if (shop == null) { return null; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                return controller.UniqueId;
            }
            return null;
        }
        static string GetShopUnqiueId(GameObject shop)
        {
            if (shop == null) { return null; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                return controller.UniqueId;
            }
            return null;
        }
        static void SetShopUnqiueId(string uniqueId)
        {
            GameObject shop = GetLookingAtShop();
            if (shop == null) { return; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                controller.UniqueId = uniqueId;
            }
        }
        static void SetShopUnqiueId(GameObject shop, string uniqueId)
        {
            if (shop == null) { return; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                controller.UniqueId = uniqueId;
            }
        }
        static string GetShopOwnerId()
        {
            GameObject shop = GetLookingAtShop();
            if (shop == null) { return null; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                return controller.OwnerId;
            }
            return null;
        }
        static string GetShopOwnerId(GameObject shop)
        {
            if (shop == null) { return null; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                return controller.OwnerId;
            }
            return null;
        }
        static void SetShopOwnerId(string ownerId)
        {
            GameObject shop = GetLookingAtShop();
            if (shop == null) { return; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                controller.OwnerId = ownerId;
            }
        }
        static void SetShopOwnerId(GameObject shop, string ownerId)
        {
            if (shop == null) { return; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                controller.OwnerId = ownerId;
            }
        }

        static Mono.Shop GetShopController()
        {
            GameObject shop = GetLookingAtShop();
            if (shop == null) { return null; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                return controller;
            }
            return null;
        }
        static Mono.Shop GetShopController(GameObject shop)
        {
            if (shop == null) { return null; }
            Mono.Shop controller = shop.GetComponent<Mono.Shop>();
            if (controller != null)
            {
                return controller;
            }
            return null;
        }

        static GameObject SpawnShop()
        {
            Transform transform = LocalPlayer._instance._mainCam.transform;
            RaycastHit raycastHit;
            Physics.Raycast(transform.position, transform.forward, out raycastHit, 25f, LayerMask.GetMask(new string[]
            {
            "Terrain",
            "Default"
            }));
            // Check If Raycast Hit Something
            if (raycastHit.collider == null)
            {
                Misc.Msg("[SpawnShop] Raycast Hit Nothing");
                SonsTools.ShowMessage("Raycast Hit Nothing", 5);
                return null;
            }
            return Prefab.SingleShop.Spawn(
                pos: raycastHit.point + Vector3.up * 0.1f,
                rot: LocalPlayer.Transform.rotation,
                ownerName: Banking.API.GetLocalPlayerName(),
                ownerId: Banking.API.GetLocalPlayerId()
                );
        }
        static GameObject SpawnShopPlayerMode()
        {
            Transform transform = LocalPlayer._instance._mainCam.transform;
            RaycastHit raycastHit;
            Physics.Raycast(transform.position, transform.forward, out raycastHit, 25f, LayerMask.GetMask(new string[]
            {
            "Terrain",
            "Default"
            }));
            // Check If Raycast Hit Something
            if (raycastHit.collider == null)
            {
                Misc.Msg("[SpawnShop] Raycast Hit Nothing");
                SonsTools.ShowMessage("Raycast Hit Nothing", 5);
                return null;
            }
            return Prefab.SingleShop.Spawn(
                pos: raycastHit.point + Vector3.up * 0.1f,
                rot: LocalPlayer.Transform.rotation,
                ownerName: $"Test-{Banking.API.GetLocalPlayerName()}",
                ownerId: $"Test-{Banking.API.GetLocalPlayerId()}"
                );
        }

        public enum ShopMode
        {
            Player,
            Owner
        }

        static bool SwapShopMode(ShopMode mode)  // Bool Is For Success
        {
            GameObject go = GetLookingAtShop();
            if (go == null) { return false; }
            return SwapShopMode(go, mode);
        }
        static bool SwapShopMode(GameObject go, ShopMode mode)  // Bool Is For Success
        {
            if (go == null) { return false; }
            Mono.Shop controller = go.GetComponent<Mono.Shop>();
            if (controller == null) { return false; }

            // Get All Stations and add them to the list
            List<GameObject> stations = new List<GameObject>();
            for (int i = 0; i < controller.numberOfSellableItems; i++)
            {
                Misc.Msg($"[Shop] Looking for station {i + 1}");
                GameObject station = go.transform.FindChild("BuyStation").FindChild($"{i + 1}").gameObject;
                if (station != null)
                {
                    stations.Add(station);
                    Misc.Msg($"[Shop] Added station {i + 1}");
                }
                else { Misc.Msg("[Shop] [Start()] Station Not Found"); return false; }
            }

            switch (mode)
            {
                case ShopMode.Player:
                    controller.SetOwner($"Test-{controller.OwnerName}", $"Test-{controller.OwnerId}");  // Set OwnerName And Id
                    foreach (var station in stations)
                    {
                        // Check If PlayerMode LinkUI Already Exists, if not create it
                        GameObject uiPlacement = station.transform.FindChild("LinkUI").gameObject;
                        if (uiPlacement != null)
                        {
                            // Check If LinkUI Already Exists
                            LinkUiElement linkUiElement = uiPlacement.GetComponent<LinkUiElement>();
                            if (linkUiElement == null)
                            {
                                // Creates Buy Icon UI Element And Attaches It To The Station LinkUI Place
                                LinkUiElement buyUi = controller.CreateLinkUi(uiPlacement, 1f, null, Assets.BuyIcon, new Vector3(0, 0, 0));
                            }
                        }
                        // Check If AdminMode LinkUI Already Exists, if it does destroy it
                        GameObject uiPlacementAdmin = station.transform.FindChild("AdminLinkUI").gameObject;
                        if (uiPlacementAdmin != null)
                        {
                            // Check If LinkUI Already Exists
                            LinkUiElement linkUiElement = uiPlacementAdmin.GetComponent<LinkUiElement>();
                            if (linkUiElement != null)
                            {
                                GameObject.Destroy(linkUiElement); // Destroy Admin Adjust UI Element
                            }
                        }
                    }  // Generate Buy Icon UI Element, Destroy Admin Adjust UI Element
                    GameObject addItemPlacement = go.transform.FindChild("Admin").FindChild("Add").gameObject;
                    if (addItemPlacement != null)  // Check If Add Item UI Element Exists, If It Does Destroy It
                    {
                        LinkUiElement linkUiElement = addItemPlacement.GetComponent<LinkUiElement>();
                        if (linkUiElement != null)
                        {
                            GameObject.Destroy(linkUiElement);  // Destroy Add Item UI Element
                        }
                    }
                    return true;
                    break;
                case ShopMode.Owner:
                    controller.SetOwner(Banking.API.GetLocalPlayerName(), Banking.API.GetLocalPlayerId());  // Set OwnerName And Id
                    foreach (var station in stations)
                    {
                        GameObject uiPlacement = station.transform.FindChild("LinkUI").gameObject;
                        if (uiPlacement != null)
                        {
                            // Check If LinkUI Already Exists
                            LinkUiElement linkUiElement = uiPlacement.GetComponent<LinkUiElement>();
                            if (linkUiElement != null)
                            {
                                GameObject.Destroy(linkUiElement);  // Destroy Buy Icon UI Element
                            }
                        }

                        GameObject uiPlacementAdmin = station.transform.FindChild("AdminLinkUI").gameObject;
                        if (uiPlacementAdmin != null)
                        {
                            // Check If LinkUI Already Exists
                            LinkUiElement linkUiElement = uiPlacementAdmin.GetComponent<LinkUiElement>();
                            if (linkUiElement == null)
                            {
                                // Creates Admin Adjust UI Element And Attaches It To The Station AdminLinkUI Place
                                LinkUiElement buyUi = controller.CreateLinkUi(uiPlacementAdmin, 1f, null, Assets.AdminTakeIcon, new Vector3(0, 0, 0));
                            }
                        }
                    }  // Destroy Buy Icon UI Element, Generate Admin Adjust UI Element
                    GameObject addItemPlacementt = go.transform.FindChild("Admin").FindChild("Add").gameObject;
                    if (addItemPlacementt != null)  // Check If Add Item UI Element Exists, If It Does Create It
                    {
                        LinkUiElement linkUiElement = addItemPlacementt.GetComponent<LinkUiElement>();
                        if (linkUiElement == null)
                        {
                            LinkUiElement addItemUi = controller.CreateLinkUi(addItemPlacementt, 1.5f, null, Assets.DepositIcon, null);
                        }
                    }
                    return true;
                    break;
            }
            return false;
        }
    }
}
