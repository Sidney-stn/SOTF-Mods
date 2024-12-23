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
                itemPlacer.StartPlaceItem();
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
                itemPlacer.StartPlaceItem();

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
                    if (LocalPlayer.IsInInventory || LocalPlayer.IsInMidAction || !LocalPlayer.IsInWorld)
                    {
                        return;
                    }
                    // Get camera transform for raycasting
                    Transform transform = LocalPlayer._instance._mainCam.transform;
                    RaycastHit raycastHit;

                    // Perform raycast
                    bool didHit = Physics.Raycast(
                        transform.position,
                        transform.forward,
                        out raycastHit,
                        5f,
                        LayerMask.GetMask(new string[] { "Terrain", "Default", "Prop" })
                    );

                    // Update position based on raycast
                    if (didHit)
                    {
                        _copiedItem.transform.position = raycastHit.point;
                    }
                    else
                    {
                        // Fallback position if no hit detected
                        float fallbackDistance = 2f;
                        _copiedItem.transform.position = transform.position + (transform.forward * fallbackDistance);
                    }

                    // Handle rotation with Q and E (keeping the existing rotation logic)
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
                        PlaceItem();
                    }
                    if (Input.GetMouseButtonDown(1)) // Right click
                    {
                        CancelPlaceItem();
                    }
                    else if (Input.GetKey(KeyCode.Escape))  // Escape Click
                    {
                        CancelPlaceItem();
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
                Destroy(this);
            }

            public void StartPlaceItem()
            {
                if (itemToPlace != null)
                {
                    LocalPlayer.Inventory.Close();
                    LocalPlayer.Inventory.StashHeldItems(false, true);

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

            private void CancelPlaceItem()
            {
                if (_copiedItem != null)
                {
                    Destroy(_copiedItem);
                    _copiedItem = null;
                    UI.SetupSignPlace.CloseUI();
                    SelfDestruct();
                }
            }
        }
    }
}
