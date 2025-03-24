using Bolt;
using RedLoader;
using Sons.Inventory;
using Sons.Items.Core;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.UI;


namespace LogSledAutoPickup
{
    [RegisterTypeInIl2Cpp]
    internal class LogSledAutoPickupMono : MonoBehaviour
    {
        // Add the boolean flag to control trigger processing
        internal bool ShouldProcessTriggers = true;

        internal GameObject StoneLayoutGroup;
        internal ItemInstanceManager StoneItemInstanceManager;
        internal InWorldLayoutItemGroup StoneInWorldLayoutItemGroup;

        internal GameObject LogLayoutGroup;
        internal ItemInstanceManager LogItemInstanceManager;
        internal InWorldLayoutItemGroup LogInWorldLayoutItemGroup;

        private string activeItem = "NONE";  // Same As CycleValues HashSet

        private bool isSmallLogSled = false;

        //private int layerMask = LayerMask.GetMask(new string[] { "PickUp" });

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Runs anyway")]
        private void Start()
        {
            //layerMask = LayerMask.GetMask(new string[] { "PickUp" });

            if (gameObject.transform.root.name.Contains("BasicLogSledStructure"))
            {
                LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [Start] Small Log Sled Detected");
                isSmallLogSled = true;
            }

            if (StoneLayoutGroup == null && !isSmallLogSled) { StoneLayoutGroup = gameObject.transform.FindChild("StoneStorage").FindChild("StoneLayoutGroup").gameObject; }
            if (LogLayoutGroup == null) { LogLayoutGroup = gameObject.transform.FindChild("LogStorage").FindChild("LogLayoutGroup").gameObject; }
            if (StoneItemInstanceManager == null && !isSmallLogSled) { StoneItemInstanceManager = StoneLayoutGroup.GetComponent<ItemInstanceManager>(); }
            if (LogItemInstanceManager == null) { LogItemInstanceManager = LogLayoutGroup.GetComponent<ItemInstanceManager>(); }
            if (StoneInWorldLayoutItemGroup == null && !isSmallLogSled) { StoneInWorldLayoutItemGroup = StoneLayoutGroup.GetComponent<InWorldLayoutItemGroup>(); }
            if (LogInWorldLayoutItemGroup == null) { LogInWorldLayoutItemGroup = LogLayoutGroup.GetComponent<InWorldLayoutItemGroup>(); }

            // Find Active Item
            FindActiveItem();

            // Initialize the flag
            ShouldProcessTriggers = true;
        }


        /// <summary>
        /// Only 1 Item Can Be Active At A Time, Updates On Change
        /// </summary>
        private void FindActiveItem()
        {
            if (isSmallLogSled) { activeItem = "LOGS"; return; }  // Small Log Sleds Only Hold Logs
            if (StoneItemInstanceManager == null) { StoneItemInstanceManager = StoneLayoutGroup.GetComponent<ItemInstanceManager>(); }
            if (LogItemInstanceManager == null) { LogItemInstanceManager = LogLayoutGroup.GetComponent<ItemInstanceManager>(); }
            // Check If Still Null
            if (StoneItemInstanceManager == null) { RLog.Error("[LogSledAutoPickup] [LogSledAutoPickupMono] [FindActiveItem] StoneItemInstanceManager Is Null"); }
            if (LogItemInstanceManager == null) { RLog.Error("[LogSledAutoPickup] [LogSledAutoPickupMono] [FindActiveItem] LogItemInstanceManager Is Null"); }

            if (StoneItemInstanceManager != null)
            {
                if (StoneItemInstanceManager.GetItemCount(640) >= 1)
                {
                    activeItem = "STONES";  // Same As CycleValues HashSet
                    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [FindActiveItem] Active Item Is Now: {activeItem}");
                    return;
                }
            } 
            else
            {
                RLog.Error("[LogSledAutoPickup] [LogSledAutoPickupMono] [FindActiveItem] StoneItemInstanceManager Is Null");
            }
            if (LogItemInstanceManager != null)
            {
                if (LogItemInstanceManager.GetItemCount(78) >= 1)
                {
                    activeItem = "LOGS";  // Same As CycleValues HashSet
                    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [FindActiveItem] Active Item Is Now: {activeItem}");
                    return;
                }
            } 
            else
            {
                RLog.Error("[LogSledAutoPickup] [LogSledAutoPickupMono] [FindActiveItem] LogItemInstanceManager Is Null");
            }
            activeItem = "NONE";  // Same As CycleValues HashSet
            LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [FindActiveItem] Active Item Is Now: {activeItem}");
        }


        /// <summary>
        /// Add Item To Sled, Via Item Id, Creates Item Instance If Null, Returns False If Item Cannot Be Added
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemInstance"></param>
        /// <param name="count"></param>
        private bool AddItemToSled(int itemId, ItemInstance itemInstance = null, int count = 1)
        {
            ItemInstance instance = itemInstance;
            if (isSmallLogSled)
            {
                if (itemId == 640)
                {
                    LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [AddItemToSled] [SmallSled] Cannot Add Stones, Small Log Sleds Only Hold Logs");
                    return false;
                } 
                else if (itemId == 78)
                {
                    // Check If Sled Is Full
                    if (LogInWorldLayoutItemGroup.GetCurrentCount() >= LogInWorldLayoutItemGroup.MaxAmount)
                    {
                        LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [AddItemToSled] [SmallSled] Cannot Add Logs, Sled Is Full");
                        return false;
                    }
                    if (instance == null)
                    {
                        instance = new ItemInstance(itemId);
                    }
                    LogInWorldLayoutItemGroup.TryAddItem(instance, itemId, count);
                    return true;
                }
                return false;
            }
            if (itemId == 640)
            {
                // Check If Logs Are In Sled, Cannot Add Stones If Logs Are In Sled
                if (StoneInWorldLayoutItemGroup.GetCurrentCount() <= 0 && LogInWorldLayoutItemGroup.GetCurrentCount() > 0)
                {
                    LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [AddItemToSled] Cannot Add Stones, Logs Already In Sled");
                    return false;
                }
                // Check If Sled Is Full
                if (StoneInWorldLayoutItemGroup.GetCurrentCount() >= StoneInWorldLayoutItemGroup.MaxAmount)
                {
                    LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [AddItemToSled] Cannot Add Stones, Sled Is Full");
                    return false;
                }

                if (instance == null)
                {
                    instance = new ItemInstance(itemId);
                }
                StoneInWorldLayoutItemGroup.TryAddItem(instance, itemId, count);
                return true;
            }
            else if (itemId == 78)
            {
                // Check If Stones Are In Sled, Cannot Add Logs If Stones Are In Sled
                if (LogInWorldLayoutItemGroup.GetCurrentCount() <= 0 && StoneInWorldLayoutItemGroup.GetCurrentCount() > 0)
                {
                    LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [AddItemToSled] Cannot Add Logs, Stones Already In Sled");
                    return false;
                }
                // Check If Sled Is Full
                if (LogInWorldLayoutItemGroup.GetCurrentCount() >= LogInWorldLayoutItemGroup.MaxAmount)
                {
                    LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [AddItemToSled] Cannot Add Stones, Sled Is Full");
                    return false;
                }
                if (instance == null)
                {
                    instance = new ItemInstance(itemId);
                }
                LogInWorldLayoutItemGroup.TryAddItem(instance, itemId, count);
                return true;
            }
            return false;
        }

        private void AddItemToSled(ItemInstance itemInstance = null, int count = 1)
        {
            // NOT UPDATED TO SMALL LOG SLED
            FindActiveItem();
            ItemInstance instance = itemInstance;
            if (activeItem == "STONES")
            {
                if (StoneInWorldLayoutItemGroup.GetCurrentCount() <= 0 && LogInWorldLayoutItemGroup.GetCurrentCount() > 0)
                {
                    LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [AddItemToSled] Cannot Add Stones, Logs Already In Sled");
                    return;
                }
                if (instance == null)
                {
                    instance = new ItemInstance(640);
                }
                StoneInWorldLayoutItemGroup.TryAddItem(instance, 640, count);
            }
            else if (activeItem == "LOGS")
            {
                if (LogInWorldLayoutItemGroup.GetCurrentCount() <= 0 && StoneInWorldLayoutItemGroup.GetCurrentCount() > 0)
                {
                    LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [AddItemToSled] Cannot Add Logs, Stones Already In Sled");
                    return;
                }
                if (instance == null)
                {
                    instance = new ItemInstance(78);
                }
                LogInWorldLayoutItemGroup.TryAddItem(instance, 78, count);
            }
        }

        private void RemoveItemFromSled(int itemId, int count = 1)
        {
            if (itemId == 640)
            {
                int amoutOfRemovedItems = 0;
                bool removedAllItems = true;
                for (int i = 0; i < count; i++)
                {
                    if (StoneInWorldLayoutItemGroup.GetCurrentCount() >= 1)
                    {
                        StoneInWorldLayoutItemGroup.RemoveItem(true);
                        amoutOfRemovedItems++;
                    }
                    else
                    {
                        removedAllItems = false;
                    }
                }
                if (removedAllItems)
                {
                    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [RemoveItemFromSled] Removed {amoutOfRemovedItems} Stones");
                }
                else
                {
                    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [RemoveItemFromSled] Removed {amoutOfRemovedItems} Stones, {count - amoutOfRemovedItems} Stones Could Not Be Removed");
                }
            }
            else if (itemId == 78)
            {
                int amoutOfRemovedItems = 0;
                bool removedAllItems = true;
                for (int i = 0; i < count; i++)
                {
                    if (LogInWorldLayoutItemGroup.GetCurrentCount() >= 1)
                    {
                        LogInWorldLayoutItemGroup.RemoveItem(true);
                        amoutOfRemovedItems++;
                    }
                    else
                    {
                        removedAllItems = false;
                    }
                }
                if (removedAllItems)
                {
                    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [RemoveItemFromSled] Removed {amoutOfRemovedItems} Logs");
                }
                else
                {
                    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [RemoveItemFromSled] Removed {amoutOfRemovedItems} Logs, {count - amoutOfRemovedItems} Logs Could Not Be Removed");
                }
            }
        }

        /// <summary>
        /// Remove Item From Sled,
        /// Uses Active Item for removal (ItemSelected via Cycle System)
        /// </summary>
        /// <param name="count">Amount To Remove</param>
        private void RemoveItemFromSled(int count = 1)
        {
            FindActiveItem();
            if (activeItem == "STONES")
            {
                int amoutOfRemovedItems = 0;
                bool removedAllItems = true;
                for (int i = 0; i < count; i++)
                {
                    if (StoneInWorldLayoutItemGroup.GetCurrentCount() >= 1)
                    {
                        StoneInWorldLayoutItemGroup.RemoveItem(true);
                        amoutOfRemovedItems++;
                    } 
                    else
                    {
                        removedAllItems = false;
                    }
                }
                if (removedAllItems)
                {
                    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [RemoveItemFromSled] Removed {amoutOfRemovedItems} Stones");
                }
                else
                {
                    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [RemoveItemFromSled] Removed {amoutOfRemovedItems} Stones, {count - amoutOfRemovedItems} Stones Could Not Be Removed");
                }
            }
            else if (activeItem == "LOGS")
            {
                int amoutOfRemovedItems = 0;
                bool removedAllItems = true;
                for (int i = 0; i < count; i++)
                {
                    if (LogInWorldLayoutItemGroup.GetCurrentCount() >= 1)
                    {
                        LogInWorldLayoutItemGroup.RemoveItem(true);
                        amoutOfRemovedItems++;
                    }
                    else
                    {
                        removedAllItems = false;
                    }
                }
                if (removedAllItems)
                {
                    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [RemoveItemFromSled] Removed {amoutOfRemovedItems} Logs");
                }
                else
                {
                    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [RemoveItemFromSled] Removed {amoutOfRemovedItems} Logs, {count - amoutOfRemovedItems} Logs Could Not Be Removed");
                }
            }
        }

        private void DestroyPickupFromGround(Sons.Gameplay.PickUp pickUp)
        {
            pickUp.AutoRemoveForGameMode();
        }

        private void DestroyPickupFromGroundMultiplayer(BoltEntity boltEntity)
        {
            DestroyPickUp destroyPickUp;
            if (boltEntity.source == null)
            {
                destroyPickUp = DestroyPickUp.Create(GlobalTargets.OnlySelf);
                LogSledAutoPickup.Msg("IN: DestroyPickUp.Create(GlobalTargets.OnlySelf)");
            }
            else
            {
                destroyPickUp = DestroyPickUp.Create(boltEntity.source);
                LogSledAutoPickup.Msg("IN: DestroyPickUp.Create(kinghtVEntity.source)");
            }
            destroyPickUp.PickUpPlayer = LocalPlayer.Entity;
            destroyPickUp.PickUpEntity = boltEntity;
            destroyPickUp.ItemId = 626;
            destroyPickUp.SibblingId = -1;
            destroyPickUp.FakeDrop = false;
            destroyPickUp.Send();
        }

        private void DestroyPickupFromGround(GameObject go)
        {
            if (go == null)
            {
                RLog.Warning("[LogSledAutoPickup] [LogSledAutoPickupMono] [DestroyPickupFromGround] Cannot Destory Pickup GameObject Is Null");
                return;
            }
            if (BoltNetwork.isRunning)
            {
                BoltEntity boltEntity = go.gameObject.GetComponent<BoltEntity>();
                if (boltEntity != null)
                {
                    DestroyPickupFromGroundMultiplayer(boltEntity);
                }
                else
                {
                    RLog.Warning("[LogSledAutoPickup] [LogSledAutoPickupMono] [OnTriggerEnter] BoltEntity Not Found - Failed To Delete Item");
                }
            } else
            {
                GameObject.Destroy(go);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Runs Anyway On Collider Enter")]
        void OnTriggerEnter(Collider other)
        {
            //LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [OnTriggerEnter] Trigger Detected: {other.gameObject.name}");
            //if (other.gameObject.name.Contains("Stone") || other.gameObject.name.Contains("Log"))
            //{
            //    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [OnTriggerEnter] Trigger Detected - CONTAINS NAME: {other.gameObject.name}");
            //    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [OnTriggerEnter] LAYER OF GAMEOBJECT: {other.gameObject.layer}, LayerMask Pickup: {layerMask}");
            //}
            if (!ShouldProcessTriggers || !this.enabled)
            {
                return;
            }
            if (other.gameObject.layer == 28 || other.gameObject.layer == 21)  // Pickup Layer DOES NOT WORK THE WAY I TRIED
            {
                //LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [OnTriggerEnter] Trigger Detected: {other.gameObject.name}");
                var item = other.gameObject.GetComponent<Sons.Gameplay.PickUp>();
                if (item != null)
                {
                    if (item._itemId == 640 || item._itemId == 78)
                    {
                        LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [OnTriggerEnter] Pickup Detected: {item._itemId}, ActiveItem: {activeItem}, LastValue: {LogSledValues.LastValue}");
                        FindActiveItem();
                        if (activeItem == "NONE" && LogSledValues.LastValue != "NONE")
                        {
                            // Check If No Items In Sled
                            if (StoneInWorldLayoutItemGroup.GetCurrentCount() <= 0 && LogInWorldLayoutItemGroup.GetCurrentCount() <= 0)
                            {
                                if (LogSledValues.LastValue == "STONES" && item._itemId == 640)
                                {
                                    LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [OnTriggerEnter] Adding To Empty Sled");
                                    bool success = AddItemToSled(item._itemId, item.ItemInstance);
                                    if (success)
                                    {
                                        DestroyPickupFromGround(item);
                                    }
                                }
                                else if (LogSledValues.LastValue == "LOGS" && item._itemId == 78)
                                {
                                    LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [OnTriggerEnter] Adding To Empty Sled");
                                    bool success = AddItemToSled(item._itemId, item.ItemInstance);
                                    if (success)
                                    {
                                        DestroyPickupFromGround(other.transform.root.gameObject);
                                    }
                                }
                            }
                            else
                            {
                                LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [OnTriggerEnter] Cannot Pickup, Items Already In Sled");
                            }
                        }
                        else if (LogSledValues.LastValue != "NONE" && activeItem != "NONE")
                        {
                            // Check If Item In Sled Matches Active Item
                            if (activeItem == "STONES" && LogSledValues.LastValue == "STONES" && item._itemId == 640)
                            {
                                LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [OnTriggerEnter] Trying To Add Item To Sled");
                                bool success = AddItemToSled(item._itemId, item.ItemInstance);
                                if (success)
                                {
                                    DestroyPickupFromGround(item);
                                }
                            }
                            else if (activeItem == "LOGS" && LogSledValues.LastValue == "LOGS" && item._itemId == 78)
                            {
                                LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledAutoPickupMono] [OnTriggerEnter] Trying To Add Item To Sled");
                                bool success = AddItemToSled(item._itemId, item.ItemInstance);
                                if (success)
                                {
                                    DestroyPickupFromGround(other.transform.root.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }

        //void OnCollisionEnter(Collision collision)
        //{
        //    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledAutoPickupMono] [OnCollisionEnter] Collision Detected: {collision.gameObject.name}");
        //}
    }
}
