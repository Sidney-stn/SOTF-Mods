using RedLoader;
using Sons.Gui.Input;
using Sons.Items.Core;
using SonsSdk;
using TheForest.Items.Special;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Shops.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class Shop : MonoBehaviour
    {
        public string UniqueId = null;
        public bool isSetupPrefab = false;
        public string OwnerName = null;
        public string OwnerId = null;
        public int numberOfSellableItems = 0;  // How many items are in the shop. NEEDS TO BE SET

        private Text ownerShopText = null;
        private List<GameObject> stations = new List<GameObject>();

        public List<int> StationPrices = new List<int>();
        public List<int> StationItems = new List<int>();
        public List<int> StationQuantities = new List<int>();

        private void Start()
        {
            Misc.Msg("[Shop] Starting Start()");
            if (isSetupPrefab) { return; }
            if (UniqueId == null)
            {
                Misc.Msg("[Shop] [Start()] UniqueId Is Null! I Should never be Null");
            }
            Misc.Msg("[Shop] Finding owner text");
            if (ownerShopText == null)
            {
                ownerShopText = gameObject.transform.FindChild("UI").FindDeepChild("OwnerText").GetComponent<Text>();
                if (ownerShopText == null)
                {
                    Misc.Msg("[Shop] Failed to find owner text");
                    return;  // Add this return if ownerShopText is critical
                }
                Misc.Msg("[Shop] Found owner text");
            }

            Misc.Msg("[Shop] Starting station setup");
            for (int i = 0; i < numberOfSellableItems; i++)
            {
                Misc.Msg($"[Shop] Looking for station {i + 1}");
                GameObject station = gameObject.transform.FindChild("BuyStation").FindChild($"{i + 1}").gameObject;
                if (station != null)
                {
                    stations.Add(station);
                    Misc.Msg($"[Shop] Added station {i + 1}");
                } else { Misc.Msg("[Shop] [Start()] Station Not Found"); return; }

            }
            Misc.Msg("[Shop] Starting station setup loop");
            int stationIndex = 0;
            foreach (var station in stations)
            {
                stationIndex++;
                Misc.Msg($"[Shop] Processing station {stationIndex}");
                if (OwnerId != Banking.API.GetLocalPlayerId())
                {
                    // Creates Buy Icon UI Element And Attaches It To The Station LinkUI Place
                    GameObject uiPlacement = station.transform.FindChild("LinkUI").gameObject;
                    if (uiPlacement == null)
                    {
                        Misc.Msg("[Shop] [Start()] UI Placement Not Found");
                        return;
                    }
                    LinkUiElement buyUi = CreateLinkUi(uiPlacement, 1f, null, Assets.BuyIcon, new Vector3(0, 0, 0));
                } else
                {
                    // Creates Admin Adjust UI Element And Attaches It To The Station AdminLinkUI Place
                    GameObject uiPlacement = station.transform.FindChild("AdminLinkUI").gameObject;
                    if (uiPlacement == null)
                    {
                        Misc.Msg("[Shop] [Start()] AdminLinkUI Placement Not Found");
                        return;
                    }
                    LinkUiElement buyUi = CreateLinkUi(uiPlacement, 1f, null, Assets.AdminTakeIcon, new Vector3(0, 0, 0));
                }
                // Check if the lists have enough elements before accessing them
                try
                {
                    // Check if the lists have enough elements before accessing them
                    int quantity = StationQuantities.Count >= stationIndex ? StationQuantities[stationIndex - 1] : 0;
                    int price = StationPrices.Count >= stationIndex ? StationPrices[stationIndex - 1] : 0;
                    int itemId = StationItems.Count >= stationIndex ? StationItems[stationIndex - 1] : 0;

                    SetQuantityUi(stationIndex, quantity);
                    SetPriceUi(stationIndex, price);
                    SetStationItem(stationIndex, itemId);
                }
                catch (Exception e)
                {
                    Misc.Msg($"[Shop] Error processing station UI: {e.Message}");
                    return;  // Remove this if you want to continue despite errors
                }

            }
            Misc.Msg("[Shop] Finished stations, starting owner setup");
            if (string.IsNullOrEmpty(OwnerId))
            {
                RLog.Warning("[Shop] [Start()] OwnerId Is Null when it never should be");
            }
            Misc.Msg($"Owner: {OwnerId} LocalPlayer: {Banking.API.GetLocalPlayerId()}");
            if (OwnerId == Banking.API.GetLocalPlayerId())
            {
                Misc.Msg("Owner Of The Shop - Adding Owner UI");
                GameObject addItemPlacement = gameObject.transform.FindChild("Admin").FindChild("Add").gameObject;
                if (addItemPlacement == null)
                {
                    Misc.Msg("[Shop] [Start()] Add Item Placement Not Found");
                    return;
                }
                LinkUiElement addItemUi = CreateLinkUi(addItemPlacement, 1f, null, Assets.DepositIcon, null);
            }

            //SetOwnerText("TestName");

            SetOwner(OwnerName, OwnerId);

        }

        private void SetPriceUi(int stationId, int price)
        {
            int StationId = stationId - 1;
            GameObject station = stations[StationId];
            if (station == null)
            {
                Misc.Msg("[Shop] [SetPriceUi()] Station Not Found");
                return;
            }
            GameObject quantityText = station.transform.FindDeepChild("Price").gameObject;
            if (quantityText == null)
            {
                Misc.Msg("[Shop] [SetPriceUi()] Price Text Not Found");
                return;
            }
            if (price == 0)
            {
                quantityText.GetComponent<Text>().text = $"Price: Free";
                return;
            }
            quantityText.GetComponent<Text>().text = $"Price: {price} $";
        }

        private void SetQuantityUi(int stationId, int quantity)
        {
            int StationId = stationId - 1;
            GameObject station = stations[StationId];
            if (station == null)
            {
                Misc.Msg("[Shop] [SetQuantityUi()] Station Not Found");
                return;
            }
            GameObject quantityText = station.transform.FindDeepChild("Quantity").gameObject;
            if (quantityText == null)
            {
                Misc.Msg("[Shop] [SetQuantityUi()] Quantity Text Not Found");
                return;
            }
            if (quantity == 0)
            {
                quantityText.GetComponent<Text>().text = $"Quantity: Empty";
                return;
            }
            quantityText.GetComponent<Text>().text = $"Quantity: {quantity}";
        }

        private GameObject GetStationItem(int stationId)
        {
            int StationId = stationId - 1;
            GameObject station = stations[StationId];
            if (station == null)
            {
                Misc.Msg("[Shop] [GetStationItem()] Station Not Found");
                return null;
            }
            GameObject itemSlotOnStation = station.transform.FindChild("Item").gameObject;
            if (itemSlotOnStation == null)
            {
                Misc.Msg("[Shop] [GetStationItem()] Item Slot Not Found");
                return null;
            }
            GameObject previewItem = itemSlotOnStation.transform.FindChild("PreviewItem").gameObject;
            if (previewItem == null)
            {
                Misc.Msg("[Shop] [GetStationItem()] Preview Item Not Found");
                return null;
            }
            return previewItem;
        }
        private void SetStationItem(int stationId, int itemId)
        {
            int StationId = stationId - 1;
            GameObject station = stations[StationId];
            if (station == null)
            {
                Misc.Msg("[Shop] [SetStationItem()] Station Not Found");
                return;
            }
            GameObject itemSlotOnStation = station.transform.FindChild("Item").gameObject;
            if (itemSlotOnStation == null)
            {
                Misc.Msg("[Shop] [SetStationItem()] Item Slot Not Found");
                return;
            }
            Vector3 pos = itemSlotOnStation.transform.localPosition;
            itemSlotOnStation.transform.localPosition = new Vector3(pos.x, 0.04f, pos.z);

            // First check if there's an existing preview and remove it
            if (itemSlotOnStation.transform.childCount > 0)
            {
                for (int i = itemSlotOnStation.transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(itemSlotOnStation.transform.GetChild(i).gameObject);
                }
            }

            if (itemId == 0)
            {
                Misc.Msg("[Shop] [SetStationItem()] Item ID Is 0, Cleared PreviewItem");
                return;
            }
            string itemName = ItemDatabaseManager.ItemById(itemId).name;
            if (string.IsNullOrEmpty(itemName))
            {
                RLog.Warning("[Shop] [SetStationItem()] Item Name Not Found");
                return;
            }
            var itemData = ItemDatabaseManager.ItemById(itemId);
            if (itemData == null)
            {
                RLog.Warning("[Shop] [SetStationItem()] Item Data Not Found");
                return;
            }
            if (itemData.HeldPrefab == null && itemData.PickupPrefab == null)
            {
                RLog.Warning("[Shop] [SetStationItem()] Item HeldPrefab && PickupPrefabNot Found");
                return;
            }
            GameObject previewItem = null;
            bool isPickup = false;
            if (itemData.HeldPrefab != null)
            {
                previewItem = itemData.HeldPrefab.gameObject;
            }
            if (itemData.HeldPrefab == null && itemData.PickupPrefab != null)
            {
                previewItem = itemData.PickupPrefab.gameObject;
                isPickup = true;
            }
            if (previewItem == null)
            {
                Misc.Msg("[Shop] [SetStationItem()] Preview Item Not Found");
                return;
            }
            GameObject newPreview = Instantiate(previewItem, itemSlotOnStation.transform);
            if (newPreview == null)
            {
                Misc.Msg("[Shop] [SetStationItem()] New Preview Not Found");
                return;
            }
            switch (itemId)  // Some Items Like Stones Needs Rescaling On HeldPrefab
            {
                case 640:  // Stone
                    newPreview.transform.localScale = new Vector3(0.6f, 0.5f, 1f);
                    break;
            }
            if (isPickup)
            {
                Vector3 scale = Vector3.one;
                switch (itemId)
                {
                    case 78:  // Log
                        scale = new Vector3(0.2f, 0.2f, 0.2f);
                        // Disable Physics and Gravity
                        var rb = newPreview.GetComponent<Rigidbody>();
                        if (rb != null) { rb.useGravity = false; rb.isKinematic = true; }
                        // Remove BoltEntity
                        var be = newPreview.GetComponent<BoltEntity>();
                        if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.Multiplayer || Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.MultiplayerClient)
                        {
                            if (be != null) { be.Entity.Detach(); }
                        }
                        if (be != null) { DestroyImmediate(be); }
                        RemoveChildren(newPreview, true);
                        Misc.Msg("[Shop] [SetStationItem()] Log");
                        break;
                    case 395:  // Log Plank
                        scale = new Vector3(0.2f, 0.2f, 0.2f);

                        // Disable Physics and Gravity
                        var rib = newPreview.GetComponent<Rigidbody>();
                        if (rib != null) { rib.useGravity = false; rib.isKinematic = true; }

                        RemoveChildren(newPreview, false, true, "LogHalfAModel");
                        Misc.Msg("[Shop] [SetStationItem()] Log Plank");
                        break;
                    case 640:  // Stone [ONLY IF STONE PICKUP PREFAB IS USED (SO ATM IT SHOULD NOT)]
                        scale = new Vector3(0.6f, 0.5f, 1f);
                        Misc.Msg("[Shop] [SetStationItem()] Stone");
                        break;
                    default:
                        scale = new Vector3(0.5f, 0.5f, 0.5f);
                        // Remove the bolt entity
                        var bEntity = newPreview.GetComponent<BoltEntity>();
                        if (Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.Multiplayer || Banking.API.GetHostMode() == Banking.API.SimpleSaveGameType.MultiplayerClient)
                        {
                            if (bEntity != null) { bEntity.Entity.Detach(); }
                        }
                        if (bEntity != null) { DestroyImmediate(bEntity); }

                        // Disable Physics and Gravity
                        var rbo = newPreview.GetComponent<Rigidbody>();
                        if (rbo != null) { rbo.useGravity = false; rbo.isKinematic = true; }
                        break;
                }
                newPreview.transform.localScale = scale;

                
            }
            newPreview.SetActive(true);
            newPreview.SetName("PreviewItem");
        }

        private void RemoveChildren(GameObject go, bool dontDetroyRenderer, bool dontDestroyCustomName = false, string dontDestory = "")
        {
            List<Transform> list = go.transform.GetChildren();
            for (int i = 0; i < list.Count; i++)
            {
                if (dontDetroyRenderer)
                {
                    if (!list[i].gameObject.name.Contains("Renderer"))
                    {
                        if (list[i].gameObject != null)
                        {
                            DestroyImmediate(list[i].gameObject);
                        }
                    }
                }
                if (dontDestroyCustomName)
                {
                    if (!string.IsNullOrEmpty(dontDestory))
                    {
                        if (!list[i].gameObject.name.Contains(dontDestory))
                        {
                            if (list[i].gameObject != null)
                            {
                                DestroyImmediate(list[i].gameObject);
                            }
                        }
                    }
                }
            }
        }

        public void SetOwner(string ownerName, string ownerId)
        {
            OwnerName = ownerName;
            OwnerId = ownerId;
            ownerShopText.text = $"{ownerName}\nShop";
        }

        private void SetOwnerText(string ownerName)
        {
            ownerShopText.text = $"{ownerName}\nShop";
        }

        private LinkUiElement CreateLinkUi(GameObject toAddLinkUiOn, float maxDistance, Texture? texture, Texture2D? texture2D, Vector3? worldSpaceOffset, string elementId = "screen.take")
        {
            Vector3 _worldOffset = worldSpaceOffset ?? new Vector3(0, (float)0.2, 0);
            LinkUiElement linkUiAdd = toAddLinkUiOn.AddComponent<LinkUiElement>();
            linkUiAdd._applyMaterial = false;
            linkUiAdd._applyText = false;
            linkUiAdd._applyTexture = true;
            if (texture != null)
            {
                linkUiAdd._texture = texture;
            }
            else if (texture2D != null)
            {
                linkUiAdd._texture = texture2D;
            }
            linkUiAdd._maxDistance = maxDistance;
            linkUiAdd._worldSpaceOffset = _worldOffset;
            linkUiAdd._uiElementId = elementId; // "screen.take", "screen.use", "screen.takeAndUse", "PickUps"
            linkUiAdd.enabled = false;
            linkUiAdd.enabled = true;
            return linkUiAdd;
        }

        public void UpdateStationLists(List<int> prices, List<int> items, List<int> quantity)
        {
            StationPrices = prices;
            StationItems = items;
            StationQuantities = quantity;
            RefreshStationUi();
        }

        private void RefreshStationUi()  // Refreshes the UI of the stations, when data is updated
        {
            int stationIndex = 0;
            foreach (var station in stations)
            {
                stationIndex++;
                SetQuantityUi(stationIndex, StationQuantities[stationIndex - 1]);
                SetPriceUi(stationIndex, StationPrices[stationIndex - 1]);
                SetStationItem(stationIndex, StationItems[stationIndex - 1]);

            }
        }

        private void RefreshStationQuantityUi(int stationIndex)
        {
            SetQuantityUi(stationIndex, StationQuantities[stationIndex - 1]);
        }

        private int? GetActiveAdminLinkUi()  // int? = StationIndex
        {
            int stationIndex = 0;
            foreach (var station in stations)  // Loops Thru Stations
            {
                stationIndex++;
                GameObject uiPlacement = station.transform.FindChild("AdminLinkUI").gameObject;
                if (uiPlacement != null)
                {
                    LinkUiElement linkUi = uiPlacement.GetComponent<LinkUiElement>();
                    if (linkUi != null)
                    {
                        if (linkUi.IsActive)
                        {
                            return stationIndex;
                        }
                    }
                }
            }
            return null;
        }

        private int? GetActiveLinkUi()  // int? = StationIndex
        {
            int stationIndex = 0;
            foreach (var station in stations)  // Loops Thru Stations
            {
                stationIndex++;
                GameObject uiPlacement = station.transform.FindChild("LinkUI").gameObject;
                if (uiPlacement != null)
                {
                    LinkUiElement linkUi = uiPlacement.GetComponent<LinkUiElement>();
                    if (linkUi != null)
                    {
                        if (linkUi.IsActive)
                        {
                            return stationIndex;
                        }
                    }
                }
            }
            return null;
        }

        private bool IsAddItemUiActive()
        {
            GameObject addItemPlacement = gameObject.transform.FindChild("Admin").FindChild("Add").gameObject;
            if (addItemPlacement != null)
            {
                LinkUiElement linkUi = addItemPlacement.GetComponent<LinkUiElement>();
                if (linkUi != null)
                {
                    if (linkUi.IsActive)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string FindActiveLinkUi()
        {
            if (OwnerId == Banking.API.GetLocalPlayerId())  // If the player is the owner of the shop
            {
                if (IsAddItemUiActive())  // If the Add Item UI is active
                {
                    return "AddItem";
                }
                // Check If AdminLinkUI Is Active (For Setting Price/And Taking Back Item)
                int? activeAdminLinkUi = GetActiveAdminLinkUi();
                if (activeAdminLinkUi != null)
                {
                    return $"Admin,{activeAdminLinkUi}";
                }
            }
            else
            {
                // Check If Buy LinkUI Is Active
                int? activeLinkUi = GetActiveLinkUi();
                if (activeLinkUi != null)
                {
                    return $"Buy,{activeLinkUi}";
                }
            }
            return null;
        }

        public void OnInteractButtonPressed()  // Called When Interection Button Is Pressed From Settings And Shop Component Is Found Via RayCast
        {
            Misc.Msg("Interact Button Pressed");
            string activeLinkUi = FindActiveLinkUi();
            if (string.IsNullOrEmpty(activeLinkUi))
            {
                Misc.Msg("No Link Ui Active");
                return;
            }
            else
            {
                Misc.Msg($"Active Link Ui: {activeLinkUi}");
            }
            int? stationNumberActive = null;
            if (activeLinkUi.Contains(",")) {
                string[] activeLinkUiSplit = activeLinkUi.Split(',');
                activeLinkUi = activeLinkUiSplit[0];
                if (int.TryParse(activeLinkUiSplit[1], out int converted)) {
                    stationNumberActive = converted;
                }
                
            }
            switch (activeLinkUi)
            {
                case "AddItem":
                    Misc.Msg("Add Item");
                    if (LocalPlayer.Inventory.RightHandItem != null)  // If Normal Item In Hand e.g. Stick
                    {
                        int itemId = LocalPlayer.Inventory.RightHandItem._itemID;
                        if (itemId != 0)
                        {
                            if (LocalPlayer.Inventory.AmountOf(itemId) >= 1)
                            {
                                LocalPlayer.Inventory.RemoveItem(itemId, 1, false, false, false, null, true);

                                if (StationItems.Contains(itemId))  // Check IF The Item Is Already Added To The Shop
                                {
                                    int index = StationItems.IndexOf(itemId);  // Find the index of the item in the list
                                    int stationNumber = index + 1;  // Station Number is the index + 1

                                    StationQuantities[index] = (StationQuantities[index] + 1);  // Add 1 to the quantity

                                    // Add Sync Network Event
                                    RefreshStationUi();  // Refresh the UI

                                    return;
                                }
                                else  // If no items are found
                                {
                                    // Check if the shop is full
                                    if (StationItems.Count >= numberOfSellableItems)
                                    {
                                        // Check If Any StationItems has itemid 0, if no items are set to 0 then the shop is full
                                        if (StationItems.Contains(0))
                                        {
                                            // Override itemid 0 to new item. (ItemId = 0 is no item added)
                                            StationItems[StationItems.IndexOf(0)] = itemId;  // Add item to the first empty slot
                                                                                             // IndexOf 0 does not need to be in the same place here so we need to get the index of the item
                                            int index = StationItems.IndexOf(itemId);  // Find the index of the item in the list
                                            StationQuantities[index] = 1;  // Add the quantity to the list
                                            StationPrices[index] = 0;  // Add the price to the list (Adjusted Later On Scrolling)

                                            RefreshStationUi();

                                            return;
                                        } 
                                        else
                                        {
                                            // Shop Is Full
                                            Misc.Msg("[AddItemFromHand] Shop Is Full;");
                                            SonsTools.ShowMessage("The shop is full, you can't add more items to it");
                                            return;
                                        }
                                    }
                                    // Add the item to the shop
                                    StationItems.Add(itemId);  // Add the item to the list
                                    StationQuantities.Add(1);  // Add the quantity to the list
                                    StationPrices.Add(0);  // Add the price to the list (Adjusted Later On Scrolling)

                                    RefreshStationUi();  // Refresh the UI

                                    // Add Sync Network Event

                                    return;
                                }
                            }
                        }
                    }

                    // Check For Log Or Stone etc..
                    IHeldOnlyItemController heldItem = LocalPlayer.Inventory.HeldOnlyItemController;
                    if (heldItem != null)
                    {
                        if (heldItem.HeldItem == null)
                        {
                            Misc.Msg("[AddItemFromHand] Held Item Is Null");
                            return;
                        }
                        if (!heldItem.HasItem)
                        {
                            Misc.Msg("[AddItemFromHand] Held Item Is Empty");
                            return;
                        }
                        int heldItemId = heldItem.HeldItem._itemID;
                        if (heldItemId == 0)
                        {
                            Misc.Msg("[AddItemFromHand] Held Item ID Is 0");
                            return;
                        }
                        Misc.Msg("HeldItem: " + heldItemId);
                        if (heldItem.Amount == 0)
                        {
                            Misc.Msg("[AddItemFromHand] Held Item Amount Is 0");
                            return;
                        }

                        if (StationItems.Contains(heldItemId))  // Check IF The Item Is Already Added To The Shop
                        {
                            int index = StationItems.IndexOf(heldItemId);  // Find the index of the item in the list
                            heldItem.PutDown(false, false, false);  // Remove the item from the hand
                            int stationNumber = index + 1;  // Station Number is the index + 1

                            StationQuantities[index] = StationQuantities[index] + 1;  // Add 1 to the quantity

                            RefreshStationUi();  // Refresh the UI

                            // Add Sync Network Event

                        } else  // If no items are found
                        {
                            // Check if the shop is full
                            if (StationItems.Count >= numberOfSellableItems)
                            {
                                // Check If Any StationItems has itemid 0, if no items are set to 0 then the shop is full
                                if (StationItems.Contains(0))
                                {
                                    // Override itemid 0 to new item. (ItemId = 0 is no item added)
                                    StationItems[StationItems.IndexOf(0)] = heldItemId;  // Add item to the first empty slot
                                                                                     // IndexOf 0 does not need to be in the same place here so we need to get the index of the item
                                    int index = StationItems.IndexOf(heldItemId);  // Find the index of the item in the list
                                    StationQuantities[index] = 1;  // Add the quantity to the list
                                    StationPrices[index] = 0;  // Add the price to the list (Adjusted Later On Scrolling)

                                    // Remove Item From Hand
                                    heldItem.PutDown(false, false, false);

                                    RefreshStationUi();

                                    return;
                                }
                                else
                                {
                                    // Shop Is Full
                                    Misc.Msg("[AddItemFromHand] Shop Is Full;");
                                    SonsTools.ShowMessage("The shop is full, you can't add more items to it");
                                    return;
                                }
                            }
                            // Add the item to the shop
                            StationItems.Add(heldItemId);  // Add the item to the list
                            StationQuantities.Add(1);  // Add the quantity to the list
                            StationPrices.Add(0);  // Add the price to the list (Adjusted Later On Scrolling)

                            RefreshStationUi();  // Refresh the UI

                            // Add Sync Network Event
                        }

                    }
                    break;
                case "Buy":
                    Misc.Msg($"Buy Item, StationNumber: {stationNumberActive}");
                    break;
                case "Admin":
                    Misc.Msg($"Admin Item, StationNumber: {stationNumberActive}");
                    if (stationNumberActive == null || stationNumberActive == 0)
                    {
                        Misc.Msg("[Shop] [OnInteractButtonPressed()] StationNumberActive Is Null");
                        SonsTools.ShowMessage("Something went wrong, please try agian");
                        return;
                    }
                    int stationListPlace = (int)stationNumberActive - 1;
                    // Check If there is any quantity of the item
                    if (StationQuantities[stationListPlace] == 0)
                    {
                        Misc.Msg("[Shop] [OnInteractButtonPressed()] No Quantity Of The Item");
                        SonsTools.ShowMessage("There is no item added to take");
                        return;
                    }
                    // Check If Item Is Inventory Item Or Held Item
                    if (StationItems[stationListPlace] == 0) { 
                        Misc.Msg("[Shop] No Item To Take");
                        SonsTools.ShowMessage("No item to take");
                    }
                    int maxAmountInventory = LocalPlayer.Inventory.GetMaxAmountOf(StationItems[stationListPlace]);
                    if (maxAmountInventory > 0)
                    {
                        // Check If The Player Has Enough Space In Inventory
                        int currentInventoryAmount = LocalPlayer.Inventory.AmountOf(StationItems[stationListPlace]);
                        if (currentInventoryAmount >= maxAmountInventory)
                        {
                            Misc.Msg("[Shop] [OnInteractButtonPressed()] Inventory Full");
                            SonsTools.ShowMessage("Your inventory is full, you can't take more of this item");
                            return;
                        }
                        // Check IF Quantity Is More Than 1
                        if (StationQuantities[stationListPlace] > 1)
                        {
                            // Add The Item To The Inventory And Remove It From The Shop
                            LocalPlayer.Inventory.AddItem(StationItems[stationListPlace], 1, true);  // Add the item to the inventory
                            StationQuantities[stationListPlace] = (StationQuantities[stationListPlace] - 1);  // Remove 1 from the quantity
                            RefreshStationQuantityUi((int)stationNumberActive);
                        }
                        else if (StationQuantities[stationListPlace] == 1)
                        {
                            // In Case Of Last Item [We want to do extra stuff]
                            LocalPlayer.Inventory.AddItem(StationItems[stationListPlace], 1, true);  // Add the item to the inventory
                            StationQuantities[stationListPlace] = (StationQuantities[stationListPlace] - 1);  // Remove 1 from the quantity

                            // Set StationItem To itemId 0 so item will be removed on refresh of ui
                            StationItems[stationListPlace] = 0;

                            RefreshStationUi();  // Refresh the UI
                        }
                        else
                        {
                            // In Case Quantity Is 0 or below
                            Misc.Msg("[Shop] [OnInteractButtonPressed()] Quantity Is 0 or below");
                            SonsTools.ShowMessage("There is no item to take");
                            return;
                        }

                    }
                    else
                    {
                        // In Case The Item Is Not An Inventory Item
                        IHeldOnlyItemController heldController = LocalPlayer.Inventory.HeldOnlyItemController;
                        if (heldController != null)
                        {
                            if (heldController.HasItem)
                            {
                                Misc.Msg("[Shop] [OnInteractButtonPressed()] HeldController Has Item");
                                SonsTools.ShowMessage("You can't take this item, drop item in hand");
                                return;
                            }
                            if (StationQuantities[stationListPlace] > 1)
                            {
                                heldController.Lift(StationItems[stationListPlace], null);  // Lift the item / Add it to the hand
                                StationQuantities[stationListPlace] = (StationQuantities[stationListPlace] - 1);  // Remove 1 from the quantity
                                RefreshStationQuantityUi((int)stationNumberActive);
                            } else if (StationQuantities[stationListPlace] == 1)
                            {
                                heldController.Lift(StationItems[stationListPlace], null);  // Lift the item / Add it to the hand
                                StationQuantities[stationListPlace] = (StationQuantities[stationListPlace] - 1);  // Remove 1 from the quantity

                                // Set StationItem To itemId 0 so item will be removed on refresh of ui
                                StationItems[stationListPlace] = 0;

                                RefreshStationUi();  // Refresh the UI
                            }
                            else
                            {
                                // In Case Quantity Is 0 or below
                                Misc.Msg("[Shop] [OnInteractButtonPressed()] Quantity Is 0 or below");
                                SonsTools.ShowMessage("There is no item to take");
                                return;
                            }

                        }
                        else
                        {
                            Misc.Msg("[Shop] [OnInteractButtonPressed()] HeldController Is Null");
                            SonsTools.ShowMessage("You can't take this item");
                            return;
                        }
                    }

                    break;
            }
        }
    }
}
