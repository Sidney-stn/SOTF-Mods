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

        public static KeyCode rotateLeftKey = TryParseKeyCode(Config.RotateLeftKey.Value, KeyCode.Q);
        public static KeyCode rotateRightKey = TryParseKeyCode(Config.RotateRightKey.Value, KeyCode.E);
        public static KeyCode cancelPlacementKey = TryParseKeyCode(Config.ExitMenuKey.Value, KeyCode.Escape);


        internal static KeyCode TryParseKeyCode(string value, KeyCode defaultValue)
        {
            try
            {
                
                string formattedValue = value.ToLower();
                if (string.IsNullOrEmpty(formattedValue)) { return defaultValue; }

                // Handle numpad keys
                if (formattedValue.StartsWith("numpad") || formattedValue.StartsWith("keypad"))
                {
                    string numPart = formattedValue.Replace("numpad", "").Replace("keypad", "");
                    return (KeyCode)Enum.Parse(typeof(KeyCode), "Keypad" + numPart);
                }

                // Handle special cases
                switch (formattedValue)
                {
                    case "escape":
                        return KeyCode.Escape;
                    case "space":
                        return KeyCode.Space;
                    case "return":
                        return KeyCode.Return;
                    case "backspace":
                        return KeyCode.Backspace;
                    case "delete":
                        return KeyCode.Delete;
                    case "tab":
                        return KeyCode.Tab;
                    case "capslock":
                        return KeyCode.CapsLock;
                    // Add any other special cases here
                    default:
                        // For single character keys, use uppercase
                        return (KeyCode)Enum.Parse(typeof(KeyCode), value.ToUpper());
                }
            }
            catch
            {
                Misc.Msg($"Failed to parse KeyCode '{value}', using default: {defaultValue}");
                return defaultValue;
            }
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
                        targetPosition = raycastHit.point;

                        // Check minimum distance from player
                        float minDistanceFromPlayer = 1f;
                        float distanceFromPlayer = Vector3.Distance(targetPosition, LocalPlayer.Transform.position);

                        if (distanceFromPlayer < minDistanceFromPlayer)
                        {
                            Vector3 directionFromPlayer = (targetPosition - LocalPlayer.Transform.position).normalized;
                            targetPosition = LocalPlayer.Transform.position + (directionFromPlayer * minDistanceFromPlayer);
                        }
                    }
                    else
                    {
                        // Clean fallback when no hit
                        Vector3 forwardDirection = transform.forward;
                        forwardDirection.y = 0;
                        forwardDirection.Normalize();
                        float fallbackDistance = 2f;
                        targetPosition = LocalPlayer.Transform.position + (forwardDirection * fallbackDistance);
                    }

                    // Direct position update instead of interpolation
                    _pivotObject.transform.position = targetPosition;

                    // Handle rotation with Q and E (now rotating the pivot object)
                    if (Input.GetKey(rotateRightKey))
                    {
                        _pivotObject.transform.Rotate(Vector3.up, -90f * Time.deltaTime);
                    }
                    else if (Input.GetKey(rotateLeftKey))
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
                    else if (Input.GetKey(cancelPlacementKey))
                    {
                        CancelPlaceItem();
                    }
                }
            }

            private void PlaceItem()
            {
                if (_copiedItem != null && _pivotObject != null)
                {
                    // Calculate the final position by moving back from the preview's center to where the corner should be
                    Vector3 finalPosition = _copiedItem.transform.position;
                    Quaternion finalRotation = _copiedItem.transform.rotation;

                    // Close the UI first
                    UI.SetupSignPlace.CloseUI();

                    // Place the actual object
                    PlaceSignOnGround(finalPosition, finalRotation, 7511, 1);

                    // Clean up
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
