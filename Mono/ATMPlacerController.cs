using RedLoader;
using UnityEngine;

namespace Banking.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class ATMPlacerController : MonoBehaviour
    {
        public bool isSetupPrefab = false;
        public string UniqueId = null;

        public void DestroyATM()
        {
            SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RemoveATM
            {
                UniqueId = UniqueId
            });

            // Remove From Lists
            Prefab.ATMPlacer.spawnedATMPlacers.Remove(UniqueId);
            Saving.Load.ModdedATMPlacers.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
