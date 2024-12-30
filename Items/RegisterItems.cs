using RedLoader;
using Sons.Inventory;
using Sons.Items.Core;
using SonsSdk;
using UnityEngine;

namespace Banking.Items
{
    internal class RegisterItems
    {
        internal const int AtmPlacerItemId = 7512;
        internal static GameObject atmPlacer;  // This Is The Placement GameObject. So Not All The Crafting Stuff Shows
        // Assets.ATMInventory Is The Inventory GameObject Shown In Inventory

        internal static void RegisterItem()
        {
            // Create New Object For Inventory And Placement Updates
            if (Assets.ATMPlacer == null) { Misc.Msg("[RegisterItems] ATMPlacer Is Null!"); return; }

            // ATM Placer Placment GameObject
            atmPlacer = GameObject.Instantiate(Assets.ATMPlacer);
            //GameObject.Destroy(atmPlacer.transform.FindChild("Crafting").gameObject);
            //GameObject.Destroy(atmPlacer.transform.FindChild("UI").gameObject);
            GameObject.DestroyImmediate(atmPlacer.transform.FindChild("Crafting").gameObject);
            GameObject.DestroyImmediate(atmPlacer.transform.FindChild("UI").gameObject);
            if (atmPlacer == null) { Misc.Msg("[RegisterItems] ATMPlacer Is Null!"); return; }

            RegisterItemToSotf();
        }

        private static void RegisterItemToSotf()
        {
            if (Config.DebugLoggingIngameBanking.Value) { Misc.Msg("[RegisterItems] RegisterItem()"); }
            if (Assets.ATMInventory == null) { Misc.Msg("[RegisterItems] ATMInventory Is Null!"); return; }
            ItemData itemData = ItemTools.CreateAndRegisterItem(AtmPlacerItemId, "ATMBuilder", 1, null);
            new ItemTools.ItemBuilder(Assets.ATMInventory, itemData, false).AddInventoryItem(new Vector3[]
            {
                new Vector3(0.21f, -0.2f, 1.55f),
                new Vector3(0f, 0f, 0.1f),
                new Vector3(-0.1f, 0f, 0f)

            }).AddIngredientItem(new Vector3[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, 0f, 0f)
            }).AddCraftingResultItem(new Vector3[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, 0f, 0f)
            }).Recipe.AddIngredient(392, 4, false).AddIngredient(403, 2, false).BuildAndAdd();

            ItemData itemData2 = ItemDatabaseManager.ItemById(AtmPlacerItemId);
            if (itemData2 == null) { RLog.Error("[RegisterItems] itemData2 == null"); return; }
            itemData2.PickupPrefab = Prefab.ATMPlacer.atmPlacerWithComps.transform;
            itemData2.PickupBundlePrefab = Prefab.ATMPlacer.atmPlacerWithComps.transform;
            itemData2._pickupCanBeBundled = false;
            itemData2._maxVirtualPickups = 300;
            itemData._maxWorldPickups = 50;
            itemData2._allowFirstLookWhenEquippedFromInventory = false;
            itemData2._dropsOnDeath = true;
            itemData2._alwaysDropOnUnequip = true;
            itemData2._applyRandomForceOnDrop = true;
            itemData2.DropOffsetWorldUp = false;
            itemData2.EquippedAnimVars = ItemDatabaseManager.ItemById(78).EquippedAnimVars;
            itemData2._hasVisualVariant = true;
            itemData2._hasFirstLook = false;
            itemData2.MaxAmount = 1;
            itemData2.UiData._rightClick = ItemUiData.RightClickCommands.None;
            itemData2.UiData._leftClick = ItemUiData.LeftClickCommands.take;
            if (Assets.ATMIcon != null) { itemData2.UiData._icon = Assets.ATMIcon; }

            itemData.SetType(Sons.Items.Core.Types.UniqueItem);

            InventoryLayoutItemGroup item = ItemTools.GetInventoryLayoutItemGroup(AtmPlacerItemId);
            item.gameObject.AddComponent<MyCustomItemInteraction>();
            //item.transform.FindDeepChild("Crafting").gameObject.SetActive(false);
            //item.transform.FindDeepChild("UI").gameObject.SetActive(false);
        }

        [RegisterTypeInIl2Cpp]
        internal class MyCustomItemInteraction : MonoBehaviour
        {
            private InventoryLayoutItemGroup group = null;

            void Start()
            {
                group = ItemTools.GetInventoryLayoutItemGroup(AtmPlacerItemId);
            }
            void Update()
            {
                if (!group._itemData) { return; }
                bool Item(InventoryLayoutItem item) => item.ItemInstance._itemID == AtmPlacerItemId && item.IsHighlighted;
                if (!group.LayoutItems.Exists((Il2CppSystem.Predicate<InventoryLayoutItem>)Item)) { group._itemData = null; return; }

                if (atmPlacer != null)
                {
                    Items.ItemPlacement.StartPlaceMentMode(atmPlacer);
                }

                group._itemData = null;
            }
        }
    }

}
