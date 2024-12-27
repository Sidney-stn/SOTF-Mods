using RedLoader;
using System.Collections;
using TheForest.Utils;
using UnityEngine;

namespace Banking.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class ATMController : MonoBehaviour
    {
        public string UniqueId = null;

        private bool isCoroutineRunning = false;

        private void Awake()
        {
        }


        public void OpenATMUi()
        {

            Prefab.ActiveATM.activeAtm = gameObject;

            if (Prefab.ActiveATM.activeAtm == null) { Misc.Msg("Active Sign is null after assigning, something is wrong"); if (UI.Setup.messageText != null) { UI.Setup.messageText.text = $"Update Failed"; DoSomethingAfterDelay().RunCoro(); } return; }
            UI.Setup.OpenUI();
            CheckDistance().RunCoro();
        }


        public Vector3 GetPos()
        {
            return gameObject.transform.position;
        }

        public Quaternion GetCurrentRotation()
        {
            return gameObject.transform.rotation;
        }

        private IEnumerator DoSomethingAfterDelay()
        {
            if (isCoroutineRunning) { yield break; }
            isCoroutineRunning = true;
            Misc.Msg("Coroutine started");

            // Wait for 3 seconds
            yield return new WaitForSeconds(3f);

            // Do something here
            Misc.Msg("3 seconds have passed, doing something!");

            // Coroutine will automatically exit after this point
            if (UI.Setup.messageText != null) { UI.Setup.messageText.text = ""; }
            isCoroutineRunning = false;
        }

        private IEnumerator CheckDistance()
        {
            while (true)
            {
                float distance = Vector3.Distance(LocalPlayer.Transform.position, transform.position);
                //Misc.Msg($"Distance: {distance}");
                if (distance > 5f)
                {
                    Prefab.ActiveATM.activeAtm = null;
                    UI.Setup.CloseUI(); // Assuming there's a CloseUI method
                    yield break; // This will end the coroutine
                }
                yield return new WaitForSeconds(1f); // Check every 1 seconds
            }
        }
        
        public void DestroyATM()
        {
            SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RemoveATM
            {
                UniqueId = UniqueId
            });
            // Remove From Lists
            Prefab.ActiveATM.spawnedAtms.Remove(UniqueId);
            Saving.Load.ModdedAtms.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
