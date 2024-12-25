
using UnityEngine;

namespace Banking.Prefab
{
    internal class ActiveATM
    {
        internal static GameObject activeAtm = null;  // For Using ATM In World. Checks If ATM Is Close And If Looking At And Such

        public static GameObject atmWithComps;
        public static Dictionary<string, GameObject> spawnedAtms = new Dictionary<string, GameObject>();

        public static void SetupAtmPrefab()
        {
            if (atmWithComps == null)
            {
                if (Assets.ATM == null) { Misc.Msg("Cant Setup Sign Prefab, Sign Asset is null!"); return; }
                atmWithComps = GameObject.Instantiate(Assets.ATM);
                Mono.ATMController signMono = atmWithComps.AddComponent<Mono.ATMController>();
                Mono.DestroyOnC destroyOnC = atmWithComps.AddComponent<Mono.DestroyOnC>();
            }
        }

        internal static GameObject SpawnATM(Vector3 pos, Quaternion rot, string uniqueId = null)
        {
            string setUniqueId = uniqueId;
            if (uniqueId == null)
            {
                setUniqueId = Guid.NewGuid().ToString();
            }
            if (atmWithComps != null)
            {
                Misc.Msg("ATM prefab is not Setup");
                return null;
            }
            GameObject newAtm = GameObject.Instantiate(atmWithComps, pos, rot);
            if (newAtm == null)
            {
                Misc.Msg("Failed To Instantiate ATM Prefab");
                return null;
            }

            spawnedAtms.Add(setUniqueId, newAtm); // UniqueId, GameObject
            Saving.Load.ModdedAtms.Add(newAtm); // Add To Save List
            return null;
        }

        internal static GameObject FindShopByUniqueId(string uniqueId)
        {
            if (spawnedAtms.TryGetValue(uniqueId, out GameObject sign))
            {
                return sign;
            }
            else
            {
                Misc.Msg($"Shop with unique ID {uniqueId} not found.");
                return null;
            }
        }

        internal static bool DoesShopWithUniqueIdExist(string uniqueId)
        {
            if (spawnedAtms.ContainsKey(uniqueId))
            {
                return true;
            }
            else
            {
                Misc.Msg($"Shop with unique ID {uniqueId} not found.");
                return false;
            }
        }
    }
}
