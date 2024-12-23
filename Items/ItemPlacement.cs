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

            private GameObject _pivotObject; // New pivot object for centered rotation

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
                if (_copiedItem != null && _pivotObject != null)
                {
                    if (LocalPlayer.IsInInventory || LocalPlayer.IsInMidAction || !LocalPlayer.IsInWorld)
                    {
                        return;
                    }

                    Transform transform = LocalPlayer._instance._mainCam.transform;
                    RaycastHit raycastHit;

                    bool didHit = Physics.Raycast(
                        transform.position,
                        transform.forward,
                        out raycastHit,
                        5f,
                        LayerMask.GetMask(new string[] { "Terrain", "Default", "Prop", "BasicTrigger" })
                    );

                    Vector3 targetPosition;
                    if (didHit)
                    {
                        // Check if hit point is too close to player
                        float minDistanceFromPlayer = 1f; // Adjust this value as needed
                        float distanceFromPlayer = Vector3.Distance(raycastHit.point, LocalPlayer.Transform.position);

                        if (distanceFromPlayer >= minDistanceFromPlayer)
                        {
                            _pivotObject.transform.position = raycastHit.point;
                        }
                        else
                        {
                            // If too close to player, place at minimum distance in look direction
                            Vector3 forwardDirection = transform.forward;
                            forwardDirection.y = 0; // Optional: keep it at same height
                            forwardDirection.Normalize();
                            _pivotObject.transform.position = LocalPlayer.Transform.position + (forwardDirection * minDistanceFromPlayer);
                        }
                    }
                    else
                    {
                        // If no hit, place at minimum distance in look direction
                        Vector3 forwardDirection = transform.forward;
                        forwardDirection.y = 0; // Optional: keep it at same height
                        forwardDirection.Normalize();
                        float fallbackDistance = 2f;
                        _pivotObject.transform.position = LocalPlayer.Transform.position + (forwardDirection * fallbackDistance);
                    }

                    // Handle rotation with Q and E (now rotating the pivot object)
                    if (Input.GetKey(KeyCode.Q))
                    {
                        _pivotObject.transform.Rotate(Vector3.up, -90f * Time.deltaTime);
                    }
                    else if (Input.GetKey(KeyCode.E))
                    {
                        _pivotObject.transform.Rotate(Vector3.up, 90f * Time.deltaTime);
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        PlaceItem();
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        CancelPlaceItem();
                    }
                    else if (Input.GetKey(KeyCode.Escape))
                    {
                        CancelPlaceItem();
                    }
                }
            }

            private void PlaceItem()
            {
                if (_copiedItem != null && _pivotObject != null)
                {
                    Vector3 finalPosition = _pivotObject.transform.position;
                    Quaternion finalRotation = _pivotObject.transform.rotation;

                    UI.SetupSignPlace.CloseUI();

                    PlaceSignOnGround(finalPosition, finalRotation, 7511, 1);

                    Destroy(_copiedItem);
                    Destroy(_pivotObject);
                    _copiedItem = null;
                    _pivotObject = null;

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

                    // Create a pivot object first
                    _pivotObject = new GameObject("ItemPivot");

                    _copiedItem = Instantiate(itemToPlace);  // Creates Copy Of Item
                    if (_copiedItem == null)
                    {
                        Misc.Msg($"[ItemPlacement] [StartPlaceItem] ItemPlacement.StartPlaceItem: Could not instantiate item with id {itemToPlace}");
                        Destroy(_pivotObject);
                        return;
                    }

                    // Disable all colliders on the copied item
                    Collider[] colliders = _copiedItem.GetComponentsInChildren<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        collider.enabled = false;
                    }

                    // Center the item on its bounds
                    Bounds bounds = CalculateObjectBounds(_copiedItem);
                    _copiedItem.transform.position -= bounds.center - _copiedItem.transform.position;

                    // Make the copied item a child of the pivot
                    _copiedItem.transform.SetParent(_pivotObject.transform, false);

                    // Initial position
                    Transform camTransform = LocalPlayer._instance._mainCam.transform;
                    _pivotObject.transform.position = camTransform.position + camTransform.forward * 2f;
                    _pivotObject.transform.rotation = LocalPlayer.Transform.rotation;

                    // Make sure the copied item doesn't interact with physics
                    Rigidbody[] rigidbodies = _copiedItem.GetComponentsInChildren<Rigidbody>();
                    foreach (Rigidbody rb in rigidbodies)
                    {
                        rb.isKinematic = true;
                        rb.detectCollisions = false;
                    }

                    UI.SetupSignPlace.OpenUI();
                }
            }

            private void CancelPlaceItem()
            {
                if (_copiedItem != null)
                {
                    Destroy(_copiedItem);
                    _copiedItem = null;

                    if (_pivotObject != null)
                    {
                        Destroy(_pivotObject);
                        _pivotObject = null;
                    }
                    SelfDestruct();
                }
                UI.SetupSignPlace.CloseUI();
            }

            private Bounds CalculateObjectBounds(GameObject obj)
            {
                Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

                foreach (Renderer renderer in renderers)
                {
                    bounds.Encapsulate(renderer.bounds);
                }

                return bounds;
            }
        }
    }
}
