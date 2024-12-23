using HarmonyLib;
using RedLoader;
using RedLoader.Utils;
using Sons.Inventory;
using Sons.Items.Core;
using SonsSdk;
using TheForest.Items.Special;
using TheForest.Utils;
using UnityEngine;

namespace Signs.Items
{
    public class RegisterItems
    {
        public static void RegisterItem()
        {
            if (Config.DebugLoggingIngameSign.Value) { Misc.Msg("[RegisterItems] RegisterItem()"); }
            ItemData itemData = ItemTools.CreateAndRegisterItem(7511, "Sign", 1, null);
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

            ItemData itemData2 = ItemDatabaseManager.ItemById(7511);
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

            //Sons.Items.Core.AnimatorVariables[] animatorVariables;
            //animatorVariables.AddItem<AnimatorVariables.logHeld>();
            itemData2.EquippedAnimVars = ItemDatabaseManager.ItemById(78).EquippedAnimVars;
            itemData2._hasVisualVariant = true;
            itemData2._hasFirstLook = false;
            itemData2.MaxAmount = 1;
            //itemData.SetType(Sons.Items.Core.Types.Equippable, Sons.Items.Core.Types.Droppable, Sons.Items.Core.Types.CraftingMaterial, Sons.Items.Core.Types.Craftable);
            //itemData.SetType(Sons.Items.Core.Types.Equippable);
            itemData.SetType(Sons.Items.Core.Types.Droppable);

            HeldOnlyItemController controller = LocalPlayer.Transform.GetComponentInChildren<HeldOnlyItemController>();
            HeldOnlyItemController.ControllerItemData controllerItem = new HeldOnlyItemController.ControllerItemData();
            controllerItem._animType = HeldOnlyItemController.ControllerItemData.AnimTypes.Log;
            controllerItem._dropPositionOffset = new Vector3(0, 0, 0);
            controllerItem._dropSpawnDelay = 0;
            controllerItem._itemCache = itemData2;
            controllerItem._itemId = 7511;
            controllerItem._itemLength = 4;
            controllerItem._itemMaxLength = 5;
            controllerItem._itemThickness = 0.5f;
            controllerItem._useDropOffset = true;
            controllerItem._held = controller._heldOnlyItems[0].Held;
            controller._heldOnlyItems.AddItem(controllerItem); // Number 10 in list so [10]


            // SetupCustomInteraction();
            SetupCustomInteraction();


        }
        public static void SetupCustomInteraction()
        {
            Misc.Msg("Injecting Custom Interaction");
            // Register our class to implement ICustomInventoryItemInteraction
            InjectorEx.Inject<ICustomInventoryItemInteraction>(typeof(CustomItemInteractionHandler));

            // Now we can add it to the GameObject
            GameObject inventoryLayoutItemGroup = ItemTools.GetInventoryLayoutItemGroup(7511).gameObject;
            var handler = inventoryLayoutItemGroup.AddComponent<CustomItemInteractionHandler>();
            var handler2 = inventoryLayoutItemGroup.transform.GetParent().gameObject.AddComponent<CustomItemInteractionHandler>();
        }
    }


    [RegisterTypeInIl2Cpp]
    public class CustomItemInteractionHandler : MonoBehaviour
    {
        public bool TryPerformAction(ItemInstance itemInstance, bool isUnique)
        {
            Misc.Msg($"Custom interaction with item: {itemInstance.Data.Id}");
            return true;
        }
    }

}
