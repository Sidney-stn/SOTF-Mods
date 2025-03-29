using Construction;
using HarmonyLib;
using UnityEngine;

namespace SimpleElevator.Tools
{
    [HarmonyPatch(typeof(RepairTool))]
    [HarmonyPatch("OnTriggerEnter")]
    [HarmonyPatch(new Type[] { typeof(Collider) })]
    internal class Destroy
    {
        [HarmonyPrefix]
        public static bool Prefix(Collider other)
        {
            if (other != null && other.transform != null && other.transform.root != null)
            {
                if (other.transform.root.name.Contains("EControlPanel"))
                {
                    GameObject go = other.transform.root.gameObject;
                    if (BoltNetwork.isRunning == false)
                    {
                        GameObject.Destroy(go);
                    } else if (BoltNetwork.isRunning && BoltNetwork.isServer)
                    {
                        GameObject.Destroy(go);
                    } else if (BoltNetwork.isRunning && BoltNetwork.isClient)
                    {
                        var mono = go.GetComponent<Mono.ElevatorControlPanelMono>();
                        mono.RaiseDestoryNetwork();
                    }
                    return false; // Skip original method
                }
                else if (other.transform.root.name.Contains("MainElevator"))
                {
                    GameObject go = other.transform.root.gameObject;
                    if (BoltNetwork.isRunning == false)
                    {
                        GameObject.Destroy(go);
                    }
                    else if (BoltNetwork.isRunning && BoltNetwork.isServer)
                    {
                        GameObject.Destroy(go);
                    }
                    else if (BoltNetwork.isRunning && BoltNetwork.isClient)
                    {
                        var mono = go.GetComponent<Mono.ElevatorMono>();
                        mono.RaiseDestoryNetwork();
                    }
                    return false; // Skip original method
                }
            }
            return true; // Continue to original method
        }
    }
}
