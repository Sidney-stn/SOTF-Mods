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

        public static GameObject PlaceObjectOnGround(GameObject itemToPlaceOnGround, Vector3 position, Quaternion rotation, int itemId, int itemQuantity, bool shouldRemoveFromInventory = true)
        {
            // This is a placeholder for the actual implementation
            return null;
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
                    _copiedItem.transform.position = LocalPlayer.Transform.position + LocalPlayer.Transform.forward;  // Places Item In Front Of Player
                    _copiedItem.transform.rotation = LocalPlayer.Transform.rotation;  // Rotates Item To Face Player

                    UI.SetupSignPlace.OpenUI();
                }
            }
        }
    }
}
