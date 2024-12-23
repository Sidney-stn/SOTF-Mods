using RedLoader;
using Sons.Inventory;
using Sons.Items.Core;
using SonsSdk;
using UnityEngine;

namespace Signs.Items
{
    public class RegisterItems
    {
        public const int SignItemId = 7511;

        public static void RegisterItem()
        {
            if (Config.DebugLoggingIngameSign.Value) { Misc.Msg("[RegisterItems] RegisterItem()"); }
            ItemData itemData = ItemTools.CreateAndRegisterItem(SignItemId, "Sign", 1, null);
            new ItemTools.ItemBuilder(Assets.SignObj, itemData, false).AddInventoryItem(new Vector3[]
            {
                new Vector3(0.21f, -0.2f, 1.55f),
                new Vector3(0f, 0f, 0.1f),
                new Vector3(-0.1f, 0f, 0f)  // If this is for Scale use: 0,7 0,7 0,7 And 0,21 -0,1 1,55 for localPos

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
            }).Recipe.AddIngredient(392, 12, false).BuildAndAdd();

            ItemData itemData2 = ItemDatabaseManager.ItemById(SignItemId);
            if (itemData2 == null) { RLog.Error("CompactLogPickup RegisterItem: itemData2 == null"); return; }
            itemData2.PickupPrefab = Prefab.SignPrefab.signWithComps.transform;
            itemData2.PickupBundlePrefab = Prefab.SignPrefab.signWithComps.transform;
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
            //itemData2.UiData._icon = ICON

            itemData.SetType(Sons.Items.Core.Types.UniqueItem);

            ItemTools.GetInventoryLayoutItemGroup(SignItemId).gameObject.AddComponent<MyCustomItemInteraction>();

        }

        [RegisterTypeInIl2Cpp]
        public class MyCustomItemInteraction : MonoBehaviour
        {
            void Update()
            {
                var group = ItemTools.GetInventoryLayoutItemGroup(SignItemId);
                if (!group._itemData) { return; }
                bool Item(InventoryLayoutItem item) => item.ItemInstance._itemID == SignItemId && item.IsHighlighted;
                if (!group.LayoutItems.Exists((Il2CppSystem.Predicate<InventoryLayoutItem>)Item)) { group._itemData = null; return; }

                if (Assets.SignObj != null)
                {
                    Items.ItemPlacement.StartPlaceMentMode(Assets.SignObj);
                }

                group._itemData = null;
            }
        }
    }

}
