using Sons.Inventory;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine.Events;


namespace WirelessSignals.Linking
{
    public class RepairToolInHand
    { 
        internal static PlayerInventory playerInventoryInstance;
        public static void Initialize(PlayerInventory playerInventory)
        {
            // Subscribing to the OnItemUnequippedEvent
            if (playerInventory.OnItemUnequippedEvent != null)
            {
                playerInventory.OnItemUnequippedEvent.AddListener((UnityAction<ItemInstance, int>)OnItemUnequipped);
            }

            // Subscribing to the OnItemEquippedEvent
            if (playerInventory.OnItemEquippedEvent != null)
            {
                playerInventory.OnItemEquippedEvent.AddListener((UnityAction<ItemInstance, int>)OnItemEquipped);
            }

            playerInventoryInstance = playerInventory;
        }

        public static void Deinitialize(PlayerInventory playerInventory)
        {
            // Unsubscribing from the OnItemUnequippedEvent
            if (playerInventory.OnItemUnequippedEvent != null)
            {
                playerInventory.OnItemUnequippedEvent.RemoveListener((UnityAction<ItemInstance, int>)OnItemUnequipped);
            }

            // Unsubscribing from the OnItemEquippedEvent
            if (playerInventory.OnItemEquippedEvent != null)
            {
                playerInventory.OnItemEquippedEvent.RemoveListener((UnityAction<ItemInstance, int>)OnItemEquipped);
            }

            playerInventoryInstance = null;
        }

        public static void OnItemUnequipped(ItemInstance item, int slotIndex)
        {
            // Your logic for when an item is unequipped
            //Misc.Msg($"Item {item.Data.name} unequipped from slot {slotIndex}");
            if (slotIndex == 422)
            {
                Misc.Msg("[RepairToolInHand] [OnItemUnequipped]");
                if (WirelessSignals.linkingCotroller == null)
                {
                    Misc.Msg("[RepairToolInHand] [OnItemUnequipped] [WirelessSignals.linkingCotroller is null]");
                    return;
                }
                WirelessSignals.linkingCotroller.RepairToolInHand(false);
            }
        }

        public static void OnItemEquipped(ItemInstance item, int slotIndex)
        {
            // Your logic for when an item is equipped
            //Misc.Msg($"Item {item.Data.name} equipped to slot {slotIndex}");
            if (slotIndex == 422)
            {
                Misc.Msg("[RepairToolInHand] [OnItemEquipped]");
                if (WirelessSignals.linkingCotroller == null)
                {
                    Misc.Msg("[RepairToolInHand] [OnItemEquipped] [WirelessSignals.linkingCotroller is null]");
                    return;
                }
                WirelessSignals.linkingCotroller.RepairToolInHand(true);
            }
        }
    }
}
