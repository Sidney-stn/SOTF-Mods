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
            if (isSetupPrefab) { return; }
            if (UniqueId == null)
            {
                Misc.Msg("[Shop] [Start()] UniqueId Is Null! I Should never be Null");
            }
            if (ownerShopText == null)
            {
                ownerShopText = gameObject.transform.FindChild("UI").FindDeepChild("OwnerText").GetComponent<Text>();
            }

            for (int i = 0; i < numberOfSellableItems; i++)
            {
                GameObject station = gameObject.transform.FindChild("BuyStation").FindChild($"{i + 1}").gameObject;
                if (station != null)
                {
                    stations.Add(station);
                } else { Misc.Msg("[Shop] [Start()] Station Not Found"); return; }

            }
            int stationIndex = 0;
            foreach (var station in stations)
            {
                stationIndex++;
                if (OwnerId != Banking.API.GetLocalPlayerId())
                {
                    GameObject uiPlacement = station.transform.FindChild("LinkUI").gameObject;
                    if (uiPlacement == null)
                    {
                        Misc.Msg("[Shop] [Start()] UI Placement Not Found");
                        return;
                    }
                    LinkUiElement buyUi = CreateLinkUi(uiPlacement, 2f, null, Assets.BuyIcon, null);
                }
                SetQuantityUi(stationIndex, StationQuantities[stationIndex - 1]);
                SetPriceUi(stationIndex, StationPrices[stationIndex - 1]);
                SetStationItem(stationIndex, StationItems[stationIndex - 1]);

            }
            if (OwnerId == Banking.API.GetLocalPlayerId())
            {
                Misc.Msg("Owner Of The Shop - Adding Owner UI");
                GameObject addItemPlacement = gameObject.transform.FindChild("Admin").FindChild("Add").gameObject;
                if (addItemPlacement == null)
                {
                    Misc.Msg("[Shop] [Start()] Add Item Placement Not Found");
                    return;
                }
                LinkUiElement addItemUi = CreateLinkUi(addItemPlacement, 2f, null, Assets.DepositIcon, null);
            }

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
            if (itemId == 0)
            {
                var existingPreview1 = itemSlotOnStation.transform.GetChild(0).gameObject;
                if (existingPreview1 != null)
                {
                    DestroyImmediate(existingPreview1);
                }
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
            if (itemData.HeldPrefab == null)
            {
                RLog.Warning("[Shop] [SetStationItem()] Item HeldPrefab Not Found");
                return;
            }
            GameObject previewItem = itemData.HeldPrefab.gameObject;
            if (previewItem == null)
            {
                Misc.Msg("[Shop] [SetStationItem()] Preview Item Not Found");
                return;
            }
            var existingPreview = itemSlotOnStation.transform.GetChild(0).gameObject;
            if (existingPreview != null)
            {
                DestroyImmediate(existingPreview);
            }
            GameObject newPreview = Instantiate(previewItem, itemSlotOnStation.transform);
            if (newPreview == null)
            {
                Misc.Msg("[Shop] [SetStationItem()] New Preview Not Found");
                return;
            }
            newPreview.SetActive(true);
            newPreview.SetName("PreviewItem");
        }

        public void SetOwner(string ownerName, string ownerId)
        {
            OwnerName = ownerName;
            OwnerId = ownerId;
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

        private string FindActiveLinkUi()
        {
            if (OwnerId == Banking.API.GetLocalPlayerId())  // If the player is the owner of the shop
            {
                // Only Option is to add an item
                GameObject addItemPlacement = gameObject.transform.FindChild("Admin").FindChild("Add").gameObject;
                if (addItemPlacement != null)
                {
                    LinkUiElement linkUi = addItemPlacement.GetComponent<LinkUiElement>();
                    if (linkUi != null)
                    {
                        if (linkUi.IsActive)
                        {
                            return "AddItem";
                        }
                    }
                    else
                    {
                        return null;

                    }
                }
                return null;
            }
            else
            {
                // Find Witch Station is active
                int stationIndex = 0;
                bool found = false;
                int stationIndexFound = 0;
                foreach (var station in stations)
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
                                found = true;
                                stationIndexFound = stationIndex;
                                break;
                            }
                        }
                    }
                }
                if (!found)
                {
                    return null;
                }
                return $"{stationIndexFound}";
            }
        }

        public void OnInteractButtonPressed()
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
            switch (activeLinkUi.ToLower())
            {
                case "additem":
                    Misc.Msg("Add Item");
                    IHeldOnlyItemController heldItem = LocalPlayer.Inventory.HeldOnlyItemController;
                    if (heldItem != null)
                    {
                        if (heldItem.HeldItem == null)
                        {
                            Misc.Msg("[AddItemFromHand] Held Item Is Null;");
                            return;
                        }
                        int heldItemId = heldItem.HeldItem._itemID;
                        if (heldItemId == 0)
                        {
                            Misc.Msg("[AddItemFromHand] Held Item ID Is 0;");
                            return;
                        }
                        if (heldItem.Amount == 0)
                        {
                            Misc.Msg("[AddItemFromHand] Held Item Amount Is 0;");
                            return;
                        }

                        if (StationItems.Contains(heldItemId))  // Check IF The Item Is Already Added To The Shop
                        {
                            int index = StationItems.IndexOf(heldItemId);  // Find the index of the item in the list
                            heldItem.PutDown(false, false, false);  // Remove the item from the hand
                            int stationNumber = index + 1;  // Station Number is the index + 1

                            StationQuantities[index] = StationQuantities[index] + 1;  // Add 1 to the quantity

                            // Add Sync Network Event
                        } else  // If no items are found
                        {
                            // Check if the shop is full
                            if (StationItems.Count >= numberOfSellableItems)
                            {
                                Misc.Msg("[AddItemFromHand] Shop Is Full;");
                                SonsTools.ShowMessage("The shop is full, you can't add more items to it");
                                return;
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
            }
        }
    }
}
