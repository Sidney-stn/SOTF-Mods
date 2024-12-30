using SonsSdk;
using UnityEngine;

namespace Banking.Prefab
{
    internal class ATMPlacer
    {
        internal static GameObject atmPlacerWithComps = null;
        internal static Dictionary<string, GameObject> spawnedATMPlacers = new Dictionary<string, GameObject>();
        internal static void SetupPrefab()
        {
            if (atmPlacerWithComps == null)
            {
                if (Assets.ATMPlacer == null) { Misc.Msg("[ATMPlacer] [SetupPrefab] Cant Setup ATM Prefab, ATM Asset is null!"); return; }
                atmPlacerWithComps = GameObject.Instantiate(Assets.ATMPlacer);

                // Add ATMPlacerController
                Mono.ATMPlacerController atmController = atmPlacerWithComps.AddComponent<Mono.ATMPlacerController>();
                atmController.isSetupPrefab = true;
                Mono.DestroyOnC destroyOnC = atmPlacerWithComps.AddComponent<Mono.DestroyOnC>();

                List<Transform> craftingChilds = atmPlacerWithComps.transform.FindChild("Crafting").GetChildren();
                foreach (var child in craftingChilds)
                {
                    child.gameObject.SetActive(false);
                }

                List<Transform> craftingChildsATM = Assets.ATMPlacer.transform.FindChild("Crafting").GetChildren();
                foreach (var child in craftingChilds)
                {
                    child.gameObject.SetActive(false);
                }

                atmPlacerWithComps.SetActive(false);  // Deactive ATMPlacer Prefab
            }
        }
        internal static GameObject PlacePrefab(Vector3 pos, Quaternion rot, bool raiseCreateEvent = false, string uniqueId = null, bool fromNetwork = false)
        {
            if (atmPlacerWithComps == null) { Misc.Msg("[ATMPlacer] [PlacePrefab] Cant Place ATM Prefab, ATM Prefab is null!"); return null; }
            if (Misc.hostMode != Misc.SimpleSaveGameType.Multiplayer && Misc.hostMode != Misc.SimpleSaveGameType.MultiplayerClient) { Misc.Msg("[ATMPlacer] [PlacePrefab] Can't Place ATM Placer"); return null; }

            GameObject atmPlacer = GameObject.Instantiate(atmPlacerWithComps, pos, rot);
            if (atmPlacer == null) { Misc.Msg("[ATMPlacer] [PlacePrefab] Failed To Instantiate ATM Placer Prefab"); return null; }
            atmPlacer.SetActive(true);  // Active ATMPlacer

            // Dobule Deactive Crafting
            List<Transform> craftingChilds = atmPlacer.transform.FindChild("Crafting").GetChildren();
            foreach (var child in craftingChilds)
            {
                child.gameObject.SetActive(false);
            }

            Mono.ATMPlacerController controller = atmPlacer.GetComponent<Mono.ATMPlacerController>();  // Get ATMPlacerController
            controller.isSetupPrefab = false;  // Set isSetupPrefab To False
            controller.spawnedOverNetwork = fromNetwork;  // Set spawnedOverNetwork

            if (uniqueId == null) { controller.UniqueId = Guid.NewGuid().ToString(); }  // Add UniqueId
            else { controller.UniqueId = uniqueId; }  // Use UniqueId If Gotten

            spawnedATMPlacers.Add(controller.UniqueId, atmPlacer);  // Add To Spawned ATM Placers

            // Networking
            if (raiseCreateEvent)
            {
                /* Raise Network Event */
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.ATMPlacer.SpawnATMPlacer
                {
                    Vector3Position = Network.CustomSerializable.Vector3ToString(pos),
                    QuaternionRotation = Network.CustomSerializable.QuaternionToString(rot),
                    UniqueId = controller.UniqueId,
                    Sender = Misc.MySteamId().Item2,
                    SenderName = Misc.GetLocalPlayerUsername(),
                    ToSteamId = "None"
                });
            }

            return atmPlacer;

        }

        internal static GameObject FindByUniqueId(string uniqueId)
        {
            if (spawnedATMPlacers.TryGetValue(uniqueId, out GameObject atmPlacer))
            {
                return atmPlacer;
            }
            else
            {
                Misc.Msg($"Shop with unique ID {uniqueId} not found.");
                return null;
            }
        }

        internal static bool DoesWithUniqueIdExist(string uniqueId)
        {
            if (spawnedATMPlacers.ContainsKey(uniqueId))
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
