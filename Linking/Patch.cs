using Construction;
using HarmonyLib;
using UnityEngine;

namespace WirelessSignals.Linking
{
    [HarmonyPatch(typeof(RepairTool))]
    [HarmonyPatch("OnTriggerEnter")]
    [HarmonyPatch(new Type[] { typeof(Collider) })]
    public class RepairToolPatch
    {
        // Dictionary to store last interaction time for each object
        private static Dictionary<string, float> lastInteractionTimes = new Dictionary<string, float>();
        private const float INTERACTION_COOLDOWN = 1f; // Second cooldown

        [HarmonyPrefix]
        public static bool Prefix(Collider other)
        {
            if (other != null && other.transform != null && other.transform.root != null)
            {
                UnityEngine.Debug.Log($"RepairTool triggered with: {other.transform.root.name}");
                Misc.Msg($"RepairTool triggered with: {other.transform.root.name}");

                if (other.transform.root.name.Contains("TransmitterSwitch") || other.transform.root.name.Contains("Reciver") || other.transform.root.name.Contains("TransmitterDetector"))
                {
                    string objectId = other.transform.root.GetInstanceID().ToString();
                    float currentTime = Time.time;

                    // Check if this object has a recorded last interaction time
                    if (lastInteractionTimes.TryGetValue(objectId, out float lastTime))
                    {
                        // If not enough time has passed since last interaction, skip
                        if (currentTime - lastTime < INTERACTION_COOLDOWN)
                        {
                            return false;
                        }
                    }

                    // Update the last interaction time
                    lastInteractionTimes[objectId] = currentTime;
                }

                if (other.transform.root.name.Contains("TransmitterSwitch"))
                {
                    GameObject open = other.transform.root.gameObject;
                    WirelessSignals.linkingCotroller.HitTransmitterSwitch(open);
                    return false; // Skip original method
                }
                else if (other.transform.root.name.Contains("Reciver"))
                {
                    GameObject open = other.transform.root.gameObject;
                    WirelessSignals.linkingCotroller.HitReciver(open);
                    return false; // Skip original method
                }
                else if (other.transform.root.name.Contains("TransmitterDetector"))
                {
                    GameObject open = other.transform.root.gameObject;
                    WirelessSignals.linkingCotroller.HitDetector(open);
                    return false; // Skip original method
                }
            }
            return true; // Continue to original method
        }
    }
}
