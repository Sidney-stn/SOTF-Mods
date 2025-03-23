using HarmonyLib;
using RedLoader;
using Sons.Gui;
using Sons.Inventory;
using Sons.Multiplayer.Client;
using StoneGate.Structure;
using TheForest.Items.Inventory;
using UnityEngine;

namespace StoneGate
{
    [HarmonyPatch(typeof(CoopPlayerRemoteSetup))]
    [HarmonyPatch("TryAddHeldItemWeaponMods")]
    [HarmonyPatch(new Type[] { typeof(ItemInstance) })]
    internal class MultiplayerFix
    {
        [HarmonyPrefix]
        public static bool PrefixOne(ItemInstance itemInstance)
        {
            if (itemInstance == null)
            {
                RLog.Error("[MultiplayerFix] [PrefixOne] itemInstance is null");
                return false;  // Skip the original method
            }
            if (itemInstance.Data == null)
            {
                RLog.Error("[MultiplayerFix] [PrefixOne] itemInstance.Data is null");
                return false;  // Skip the original method
            }
            if (itemInstance.Data.Id == 0)
            {
                RLog.Error("[MultiplayerFix] [PrefixOne] itemInstance.Data.Id is null");
                return false;  // Skip the original method
            }
            if (Testing.Settings.logPatches == true)
            {
                Misc.Msg($"[MultiplayerFix] [PrefixOne] itemInstance.Data.Id: {itemInstance.Data.Id}");
            }
            //if (itemInstance.Data.Id == StoneGate.ToolItemId)
            //{
            //    Misc.Msg("[MultiplayerFix] [PrefixOne] Skipping StoneGate Tool, Multiplayer");
            //    return false;  // Skip the original method
            //}
            return true;  // Call the original method
        }
    }

    [HarmonyPatch(typeof(CoopPlayerRemoteSetup))]
    [HarmonyPatch("AddHeldItem")]
    [HarmonyPatch(new Type[] { typeof(int) })]
    internal class MultiplayerFixThree
    {
        [HarmonyPrefix]
        public static bool PrefixThree(int eachItem)
        {
            if (eachItem == 0)
            {
                RLog.Error("[MultiplayerFix] [PrefixThree] ItemId is 0");
                return false;  // Skip the original method
            }
            if (Testing.Settings.logPatches == true)
            {
                Misc.Msg($"[MultiplayerFix] [PrefixThree] ItemId: {eachItem}");
            }
            //if (eachItem == StoneGate.ToolItemId)
            //{
            //    Misc.Msg("[MultiplayerFix] [PrefixThree] Skipping StoneGate Tool AddItem, Multiplayer");
            //    return false;  // Skip the original method
            //}
            return true;  // Call the original method
        }
    }

    [HarmonyPatch(typeof(UiElement))]
    [HarmonyPatch("GetHash")]
    [HarmonyPatch(new Type[] { typeof(string) })]
    internal class MultiplayerFixTwo
    {
        [HarmonyPrefix]
        public static bool PrefixTwo(string id)
        {
            if (id == null)
            {
                RLog.Error("[MultiplayerFix] [PrefixTwo] id is null");
                return false;  // Skip the original method
            }
            if (Testing.Settings.logPatches == true)
            {
                Misc.Msg($"[MultiplayerFix] [PrefixTwo] id: {id}");
            }
            //if (id == "StoneGateTool" || id == "Stone Gate Creator")
            //{
            //    Misc.Msg("[MultiplayerFix] [PrefixTwo] Skipping StoneGate Tool, Multiplayer");
            //    return false;  // Skip the original method
            //}
            return true;  // Call the original method
        }
    }

    //[HarmonyPatch(typeof(PlayerInventory))]
    //[HarmonyPatch("Open")]
    //internal class MultiplayerFixFour
    //{
    //    [HarmonyPrefix]
    //    public static bool PrefixFour()
    //    {
    //        Misc.Msg("PlayerInventory Open");
    //        Misc.Msg("Missing Refs:");
    //        Debug.ItemRefs.LogMissingRefs();
    //        Misc.Msg("All Refs:");
    //        Debug.ItemRefs.LogAllRefs();
    //        Misc.Msg("Scenes:");
    //        Debug.ItemRefs.LogSceneOfRefs();

    //        return true;  // Call the original method
    //    }
    //}

}
