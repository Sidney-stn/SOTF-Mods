using RedLoader;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;



namespace Signs.Items
{
    public class ItemPlacement
    {
        public enum PrefabType
        {
            Held,
            Prop,
            Pickup
        }

        public static GameObject PlaceSignOnGround(Vector3 position, Quaternion rotation, int itemId, int itemQuantity, bool shouldRemoveFromInventory = true)
        {
            GameObject sign = null;
            int amountOfItem = LocalPlayer.Inventory.AmountOf(7511);
            if (shouldRemoveFromInventory)
            {
                if (amountOfItem < itemQuantity)
                {
                    Misc.Msg($"[ItemPlacement] [PlaceSignOnGround] ItemPlacement.PlaceSignOnGround: Not enough items in inventory to place sign. Required: {itemQuantity}, Available: {amountOfItem}");
                    return null;
                }
                LocalPlayer.Inventory.RemoveItem(7511, amountOfItem);
            }
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
            {
                sign = Prefab.SignPrefab.spawnSignSingePlayer(position, rotation);
            }
            else if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                sign = Prefab.SignPrefab.spawnSignMultiplayer(position, rotation, raiseCreateEvent: true);
            }
            return sign;
        }

        public static void StartPlaceMentMode(GameObject itemToPlace)  // Only Needs Input GameObject
        {
            if (itemToPlace != null)
            {
                ItemPlacer itemPlacer = LocalPlayer.GameObject.AddComponent<ItemPlacer>();
                itemPlacer.itemToPlace = itemToPlace;
            }
            else
            {
                Misc.Msg($"[ItemPlacement] [StartPlaceMentMode] ItemPlacement.StartPlaceMentMode: Could not find item with id {itemToPlace}");
            }
        }

        public static void StartPlaceMentMode(int itemIdToPlace, PrefabType prefabType)  // Only Needs Input ItemId Item Needs To Be Registered In ItemDb
        {
            GameObject itemToPlace = null;
            switch (prefabType)
            {
                case PrefabType.Held:
                    itemToPlace = ItemTools.GetHeldPrefab(itemIdToPlace).gameObject;
                    break;
                case PrefabType.Prop:
                    itemToPlace = ItemTools.GetPropPrefab(itemIdToPlace).gameObject;
                    break;
                case PrefabType.Pickup:
                    itemToPlace = ItemTools.GetPickupPrefab(itemIdToPlace).gameObject;
                    break;
            }
            if (itemToPlace != null)
            {
                ItemPlacer itemPlacer = LocalPlayer.GameObject.AddComponent<ItemPlacer>();
                itemPlacer.itemToPlace = itemToPlace;
  
            }
            else
            {
                Misc.Msg($"[ItemPlacement] [StartPlaceMentMode] ItemPlacement.StartPlaceMentMode: Could not find item with id {itemIdToPlace}");
            }
        }

        [RegisterTypeInIl2Cpp]
        public class ItemPlacer : MonoBehaviour
        {
            public static ItemPlacer Instance { get; private set; }

            public GameObject itemToPlace;
            private GameObject _copiedItem;

            private void Awake()
            {
                if (Instance == null)
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }

            private void Start()
            {

            }
            private void Update()
            {
                if (_copiedItem != null)
                {
                    // Update position to hover in front of player
                    Vector3 playerPosition = LocalPlayer.Transform.position;
                    Vector3 playerForward = LocalPlayer.Transform.forward;
                    float hoverDistance = 2f; // Adjust this value to change how far in front it hovers

                    // Calculate new position in front of player
                    Vector3 targetPosition = playerPosition + (playerForward * hoverDistance);
                    _copiedItem.transform.position = targetPosition;

                    // Handle rotation with Q and E
                    if (Input.GetKey(KeyCode.Q))
                    {
                        _copiedItem.transform.Rotate(Vector3.up, -90f * Time.deltaTime); // Rotate left
                    }
                    else if (Input.GetKey(KeyCode.E))
                    {
                        _copiedItem.transform.Rotate(Vector3.up, 90f * Time.deltaTime); // Rotate right
                    }

                    // Handle placement with left click
                    if (Input.GetMouseButtonDown(0)) // Left click
                    {
                        // Place the object and clean up
                        PlaceItem();
                    }
                }
            }

            private void PlaceItem()
            {
                if (_copiedItem != null)
                {
                    // Get the current position and rotation of the preview item
                    Vector3 finalPosition = _copiedItem.transform.position;
                    Quaternion finalRotation = _copiedItem.transform.rotation;

                    // Close the UI first
                    UI.SetupSignPlace.CloseUI();

                    // Place the actual object using the static method
                    PlaceSignOnGround(finalPosition, finalRotation, 7511, 1); // Adjust itemId and quantity as needed

                    // Clean up
                    Destroy(_copiedItem);
                    _copiedItem = null;

                    // Self destruct the MonoBehaviour
                    SelfDestruct();
                }
            }

            private void OnDestroy()
            {
                if (Instance == this)
                {
                    Instance = null;
                }
            }

            public void SelfDestruct()
            {
                if (Instance == this)
                {
                    Instance = null;
                }
                Destroy(gameObject);
            }

            public void StartPlaceItem()
            {
                if (itemToPlace != null)
                {
                    _copiedItem = Instantiate(itemToPlace);  // Creates Copy Of Item
                    if (_copiedItem == null)
                    {
                        Misc.Msg($"[ItemPlacement] [StartPlaceItem] ItemPlacement.StartPlaceItem: Could not instantiate item with id {itemToPlace}");
                        return;
                    }
                    _copiedItem.transform.position = LocalPlayer.Transform.position + LocalPlayer.Transform.forward;  // Places Item In Front Of Player
                    _copiedItem.transform.rotation = LocalPlayer.Transform.rotation;  // Rotates Item To Face Player

                    UI.SetupSignPlace.OpenUI();
                }
            }
        }
    }
}
