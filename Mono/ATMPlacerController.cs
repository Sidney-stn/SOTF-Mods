using RedLoader;
using Sons.Gui.Input;
using SonsSdk;
using System.Collections;
using TheForest.Items.Special;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Banking.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class ATMPlacerController : MonoBehaviour
    {
        public bool isSetupPrefab = false;
        public string UniqueId = null;
        private List<Transform> craftingChilds = null;
        public LinkUiElement placeItem;
        private Dictionary<int, List<GameObject>> unplacedItems = new Dictionary<int, List<GameObject>>(); // <ItemId, GameObject>
        private HeldOnlyItemController itemController;  // For Placing Log Into Compresser in Update()
        private bool isAllItemsAdded = false;

        private bool isCoroutineRunning = false;  // For Coroutine. Check If Coroutine Is Running

        private Text frontTextToCoutdown = null;  // For Countdown Text And Front Text

        // For Networking
        public bool spawnedOverNetwork = false;

        // For Networking And Countdown
        public float countdown = 600f; // For Setting Coutdown Time

        // For Networking And Countdown Over Network
        public float remainingTime = 0f;  // For Remaining Time


        private void Start()
        {
            if (isSetupPrefab) { return; }  // Not Needed If SetupPrefab
            if (UniqueId == null)  // UniqueId Should Never Be Null
            {
                Misc.Msg("[ATMPlacerController] [Start()] UniqueId Is Null! I Should never be Null");
            }

            if (craftingChilds == null)
            {
                craftingChilds = gameObject.transform.FindChild("Crafting").GetChildren();
            }

            if (placeItem == null && !spawnedOverNetwork)
            {
                GameObject uiPlacement = gameObject.transform.FindChild("UI").gameObject;
                if (uiPlacement != null)
                {
                    placeItem = CreateLinkUi(uiPlacement, 2f, null, Assets.ATMIcon, null);
                }
            }

            if (frontTextToCoutdown == null)
            {
                frontTextToCoutdown = gameObject.transform.FindDeepChild("Line1").GetComponent<Text>();
                frontTextToCoutdown.text = "";
            }

            if (itemController == null && !spawnedOverNetwork) { itemController = LocalPlayer.Transform.GetComponentInChildren<HeldOnlyItemController>(); }

            GetUnplacedItems();
            UpdateUiIcon();
        }

        private void GetUnplacedItems()
        {
            if (craftingChilds == null)
            {
                Misc.Msg("[ATMPlacerController] [GetUnplacedItems] Crafting Childs Is Null Or SpawnedOverNetwork True");
                return;
            }
            foreach (var child in craftingChilds)
            {
                if (child.gameObject != null && child.gameObject.name.Contains("(") && child.gameObject.name.Contains(")"))
                {
                    if (child.gameObject.active) { continue; }  // Return If Active (Already Built / Added The Item)

                    if (Mono.Helpers.ExtractNumberFromString(child.gameObject.name, out int number))
                    {
                        // If this itemId doesn't exist in dictionary yet, create new list
                        if (!unplacedItems.ContainsKey(number))
                        {
                            unplacedItems[number] = new List<GameObject>();
                        }
                        // Add GameObject to the list for this itemId
                        unplacedItems[number].Add(child.gameObject);
                    }
                }
            }
        }

        private void UpdateUiIcon()
        {
            if (spawnedOverNetwork) { return; }
            if (placeItem != null)
            {
                int firstElementId = unplacedItems.Keys.First();
                placeItem._texture = ItemTools.GetIcon(firstElementId).icon;
                placeItem.enabled = false;
                placeItem.enabled = true;
            }
            else
            {
                Misc.Msg("[ATMPlacerController] [UpdateUiIcon] placeItem Is Null");
            }
        }

        public void OnTryPlaceItem()  // Try To Add Item
        {
            if (spawnedOverNetwork) { return; }
            var firstItemId = unplacedItems.Keys.First();  // Get first itemId
            var gameObjectsList = unplacedItems[firstItemId];  // Get list of GameObjects for this id
            var showItemOnAdded = gameObjectsList[0];  // Get first GameObject from list

            if (firstItemId == 0 || showItemOnAdded == null) { return; }

            if (firstItemId == 78)  // If Log
            {
                if (itemController._heldItemId == 78 && itemController._heldCount >= 1)
                {
                    itemController.PutDown(false, false, false, null, 78, 0);
                }
                else
                {
                    Misc.Msg("[ATMPlacerController] [OnTryPlaceItem] You do not have the required item: " + firstItemId);
                    SonsTools.ShowMessage("You do not have the required item", 2);
                    return;
                }
            }
            else
            {
                if (LocalPlayer.Inventory.AmountOf(firstItemId) < 1)
                {
                    Misc.Msg("[ATMPlacerController] [OnTryPlaceItem] You do not have the required item: " + firstItemId);
                    SonsTools.ShowMessage("You do not have the required item", 2);
                    return;
                }
                LocalPlayer.Inventory.RemoveItem(firstItemId, 1, false, false);
            }

            showItemOnAdded.SetActive(true);

            // Remove the used GameObject from the list
            gameObjectsList.RemoveAt(0);

            // If list is empty, remove the entire itemId entry
            if (gameObjectsList.Count == 0)
            {
                unplacedItems.Remove(firstItemId);
            }

            // Check If Dictionary Is Empty
            if (unplacedItems.Count == 0)
            {
                isAllItemsAdded = true;
                GameObject.Destroy(placeItem);
                StartBuilding();
                return;
            }

            OnItemSuccessAdded();

            UpdateUiIcon();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && !spawnedOverNetwork)
            {
                if (placeItem != null)  // Check If LinkUi Still Exists
                {
                    if (placeItem.IsActive)  // Check If LinkUi Is Active
                    {
                        if (!isAllItemsAdded) { OnTryPlaceItem(); }  // Check If All Items Are Added
                    }
                } else { Misc.Msg("[ATMPlacerController] [Update] placeItem Is Null"); }
                
            }
        }

        private void StartBuilding() // Start Building
        {
            if (isCoroutineRunning || spawnedOverNetwork) { return; }
            BuildATM().RunCoro();
        }


        private IEnumerator BuildATM()
        {
            isCoroutineRunning = true;
            Misc.Msg("[ATMPlacerController] [BuildATM] Coroutine Started");

            remainingTime = countdown;

            while (remainingTime > 0)
            {
                // Update the text with the remaining time (rounded to integers)
                frontTextToCoutdown.text = $"Building ATM... {Mathf.Ceil(remainingTime)}s";

                // Wait for one second
                yield return new WaitForSeconds(1f);

                // Decrease the remaining time
                remainingTime -= 1f;
            }

            frontTextToCoutdown.text = "Building ATM... Done!";
            Misc.Msg("[ATMPlacerController] [BuildATM] Coroutine Finished");

            // Build ATM On Same Position
            Prefab.ActiveATM.SpawnATM(gameObject.transform.position, gameObject.transform.rotation);
            isCoroutineRunning = false;
            SonsTools.ShowMessage("ATM Built!", 5);

            // Remove ATM Placer
            DestroyATM();
        }

        public void StartNetworkCountdown(float duration)
        {
            if (isCoroutineRunning) { return; }
            ShowNetworkCountdown(duration).RunCoro();
        }

        private IEnumerator ShowNetworkCountdown(float duration)
        {
            isCoroutineRunning = true;
            Misc.Msg("[ATMPlacerController] [ShowNetworkCountdown] Coroutine Started");

            float remainingTime = duration;

            while (remainingTime > 0)
            {
                // Update the text with the remaining time
                frontTextToCoutdown.text = $"Processing... {Mathf.Ceil(remainingTime)}s";

                yield return new WaitForSeconds(1f);
                remainingTime -= 1f;
            }

            frontTextToCoutdown.text = "Processing... Complete!";
            Misc.Msg("[ATMPlacerController] [ShowNetworkCountdown] Coroutine Finished");
            isCoroutineRunning = false;
        }

        public void DestroyATM()
        {
            SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.ATMPlacer.RemoveATMPlacer
            {
                UniqueId = UniqueId
            });

            // Remove From Lists
            Prefab.ATMPlacer.spawnedATMPlacers.Remove(UniqueId);
            Saving.Load.ModdedATMPlacers.Remove(gameObject);
            Destroy(gameObject);
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

        public void SetFrontText(string text)
        {
            if (frontTextToCoutdown != null)
            {
                frontTextToCoutdown.text = text;
            }
        }

        public Dictionary<int, int> GetAddedObjects()  // Get Added Objects <ItemId, Quanity>
        {
            Dictionary<int, int> addedObjects = new Dictionary<int, int>();
            foreach (var item in unplacedItems)
            {
                addedObjects.Add(item.Key, item.Value.Count);
            }
            return addedObjects;
        }

        public void SetAddedObjects(Dictionary<int, int> addedObjects)  // Set Added Objects <ItemId, Quanity>
        {
            foreach (var item in addedObjects)  // Loop Through Added Objects
            {
                int itemId = item.Key;
                int quanity = item.Value;
                foreach (var item2 in unplacedItems)
                {
                    if (item2.Key == itemId)  // Check If ItemId Matches Any ItemId In UnplacedItems
                    {
                        List<GameObject> undAddedGameObjects = item2.Value;
                        undAddedGameObjects.First().SetActive(true);  // Set Active
                        undAddedGameObjects.Remove(undAddedGameObjects.First());  // Remove From List
                    }
                }
            }
        }

        private void OnItemSuccessAdded()  // Send Network Event
        {
            SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.ATMPlacer.UpdateATMPlacer
            {
                UniqueId = UniqueId,
                RecivedAddedItems = GetAddedObjects(),
                RemainingTime = remainingTime,
                Sender = Misc.MySteamId().Item2,
                SenderName = Misc.GetLocalPlayerUsername(),
                ToSteamId = "None"
            });
        }

        public Vector3 GetPos()
        {
            return gameObject.transform.position;
        }

        public Quaternion GetCurrentRotation()
        {
            return gameObject.transform.rotation;
        }
    }
}
