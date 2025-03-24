using HarmonyLib;
using RedLoader;
using Sons.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace LogSledAutoPickup
{
    [HarmonyPatch]
    public class LogSledControllerPatches
    {
        [HarmonyPatch(typeof(LogSledController), "ConnectPlayer")]
        [HarmonyPostfix]
        public static void PostfixConnectPlayer(LogSledController __instance)
        {
            // Get the player info if possible
            if (__instance != null)
            {
                if (Config.Enabled.Value == false)
                {
                    return;
                }
                var alreadyAdded = __instance.gameObject.GetComponent<LogSledAutoPickupMono>(); 
                if (alreadyAdded == null)
                {
                    var sledMono = __instance.gameObject.AddComponent<LogSledAutoPickupMono>();
                    sledMono.enabled = true;
                    sledMono.ShouldProcessTriggers = true;
                }
                else
                {
                    alreadyAdded.enabled = true;
                    alreadyAdded.ShouldProcessTriggers = true;
                }

                if (LogSledAutoPickupUi.panelText.Value == LogSledAutoPickupUi.defaultPanelText)
                {
                    LogSledValues.LastValue = "NONE";
                }

                // Set the active log sled
                LogSledTools.activeLogSled = __instance.gameObject;

                // Enable the UI
                LogSledAutoPickupUi.OpenMainPanel();

            }
            else
            {
                LogSledAutoPickup.Msg("ConnectPlayer Postfix: Player connected to log sled but player info is incomplete");
            }
        }

        [HarmonyPatch(typeof(LogSledController), "DisconnectPlayer")]
        [HarmonyPostfix]
        public static void PostfixDisconnectPlayer(LogSledController __instance)
        {
            if (__instance != null)
            {
                if (Config.Enabled.Value == false)
                {
                    var alreadyAdded = __instance.gameObject.GetComponent<LogSledAutoPickupMono>();
                    if (alreadyAdded != null)
                    {
                        GameObject.Destroy(alreadyAdded);
                    }
                    return;
                }
                // Disable the script
                var sledMono = __instance.gameObject.GetComponent<LogSledAutoPickupMono>();
                sledMono.enabled = false;
                sledMono.ShouldProcessTriggers = false;

                // Clear the active log sled
                LogSledTools.activeLogSled = null;

                // Disable the UI
                LogSledAutoPickupUi.CloseMainPanel();
            }
            else
            {
                LogSledAutoPickup.Msg("DisconnectPlayer Postfix: Player disconnected to log sled but player info is incomplete");
            }
        }
    }
}
