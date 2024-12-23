using RedLoader;
using RedLoader.NativeUtils;
using RedLoader.Utils;
using Sons.Inventory;
using Sons.Items.Core;
using SonsSdk;
using UnityEngine;
using UnityEngine.InputSystem;


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
            itemData2.UiData._rightClick = ItemUiData.RightClickCommands.None;
            itemData2.UiData._leftClick = ItemUiData.LeftClickCommands.take;
            //itemData2.UiData._icon = ICON

            //itemData.SetType(Sons.Items.Core.Types.Equippable, Sons.Items.Core.Types.Droppable, Sons.Items.Core.Types.CraftingMaterial, Sons.Items.Core.Types.Craftable);
            //itemData.SetType(Sons.Items.Core.Types.Equippable);
            //itemData.SetType(Sons.Items.Core.Types.Droppable);
            itemData.SetType(Sons.Items.Core.Types.Edible);

            GameObject inventoryLayoutItemGroup = ItemTools.GetInventoryLayoutItemGroup(7511).gameObject;
            InventoryLayoutItemGroup inventoryLayoutItemGroup2 = inventoryLayoutItemGroup.AddComponent<InventoryLayoutItemGroup>();

            // Hook method for your specific instance
            var hook = new Hook<OnInteractWithItem>(
                typeof(InventoryLayoutItemGroup).GetMethod("OnInteractWithItem"),
                (originalMethod, instance, context) => {
                    // Check if this is our specific instance
                    if (instance == inventoryLayoutItemGroup2)
                    {
                        // Your code here when the specific instance is interacted with
                        Debug.Log("Our specific item was interacted with!");
                    }

                    // Call original method
                    originalMethod(instance, context);
                }
            );

            // Define the delegate for the hook
            delegate void OnInteractWithItem(InventoryLayoutItemGroup instance, InputAction.CallbackContext context);




            //HeldOnlyItemController controller = LocalPlayer.Transform.GetComponentInChildren<HeldOnlyItemController>();
            //HeldOnlyItemController.ControllerItemData controllerItem = new HeldOnlyItemController.ControllerItemData();
            //controllerItem._animType = HeldOnlyItemController.ControllerItemData.AnimTypes.Log;
            //controllerItem._dropPositionOffset = new Vector3(0, 0, 0);
            //controllerItem._dropSpawnDelay = 0;
            //controllerItem._itemCache = itemData2;
            //controllerItem._itemId = 7511;
            //controllerItem._itemLength = 4;
            //controllerItem._itemMaxLength = 5;
            //controllerItem._itemThickness = 0.5f;
            //controllerItem._useDropOffset = true;
            //controllerItem._held = controller._heldOnlyItems[0].Held;
            //controller._heldOnlyItems.AddItem(controllerItem); // Number 10 in list so [10]


            // SetupCustomInteraction();
            //SetupCustomInteraction();


        }
        //public static void SetupCustomInteraction()
        //{
        //    Misc.Msg("Starting Custom Interaction Setup");

        //    // First register with ClassInjector
        //    try
        //    {
        //        ClassInjector.RegisterTypeInIl2Cpp<CustomItemInteractionHandler>();
        //        Misc.Msg("Registered class with IL2CPP");
        //    }
        //    catch (Exception e)
        //    {
        //        Misc.Msg($"Failed to register with IL2CPP: {e}");
        //        return;
        //    }

        //    // Then inject the interface
        //    try
        //    {
        //        InjectorEx.Inject<ICustomInventoryItemInteraction>(typeof(CustomItemInteractionHandler));
        //        Misc.Msg("Injected interface implementation");
        //    }
        //    catch (Exception e)
        //    {
        //        Misc.Msg($"Failed to inject interface: {e}");
        //        return;
        //    }

        //    // Finally add component to GameObject
        //    try
        //    {
        //        GameObject inventoryLayoutItemGroup = ItemTools.GetInventoryLayoutItemGroup(7511).gameObject;
        //        var handler = inventoryLayoutItemGroup.AddComponent<CustomItemInteractionHandler>();
        //        Misc.Msg("Added component successfully");
        //    }
        //    catch (Exception e)
        //    {
        //        Misc.Msg($"Failed to add component: {e}");
        //    }
        //}
    }

    //public class CustomItemInteractionHandler : MonoBehaviour
    //{
    //    // Need to add constructor for IL2CPP
    //    public CustomItemInteractionHandler(IntPtr ptr) : base(ptr) { }

    //    public bool TryPerformAction(ItemInstance itemInstance, bool isUnique)
    //    {
    //        Misc.Msg($"Custom interaction with item: {itemInstance.Data.Id}");
    //        return true;
    //    }
    //}

}
