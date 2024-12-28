using RedLoader;
using Sons.Gui.Input;
using System.Collections;
using TheForest.Utils;
using UnityEngine;

namespace Banking.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class ATMController : MonoBehaviour
    {
        public bool isSetupPrefab = false;
        public string UniqueId = null;

        private bool isCoroutineRunning = false;

        private LinkUiElement openUi;

        private void Awake()
        {
            
        }

        private void Start()
        {
            if (isSetupPrefab) { return; }
            if (UniqueId == null)
            {
                Misc.Msg("[ATMController] [Start()] UniqueId Is Null! I Should never be Null");
            }

            GameObject uiPlacement = gameObject.transform.FindChild("UI").gameObject;
            if (uiPlacement == null)
            {
                Misc.Msg("UI Placement Not Found");
                return;
            }
            openUi = CreateLinkUi(uiPlacement, 2f, null, Assets.ATMIcon, null);
        }


        public void OpenATMUi()
        {
            if (openUi != null) { 
                if (!openUi.IsActive)
                {
                    return;
                }
            }
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

        private LinkUiElement CreateLinkUi(GameObject toAddLinkUiOn, float maxDistance, Texture? texture, Texture2D? texture2D, Vector3? worldSpaceOffset, string elementId = "screen.take")
        {
            Vector3 _worldOffset = worldSpaceOffset ?? new Vector3(0, (float)0.2, 0);
            LinkUiElement linkUiAdd = toAddLinkUiOn.AddComponent<LinkUiElement>();
            linkUiAdd._applyMaterial = false;
            linkUiAdd._applyText = false;
            linkUiAdd._applyTexture = true;
            if (texture != null)
            {
                linkUiAdd._texture = texture;
            }
            else if (texture2D != null)
            {
                linkUiAdd._texture = texture2D;
            }
            linkUiAdd._maxDistance = maxDistance;
            linkUiAdd._worldSpaceOffset = _worldOffset;
            linkUiAdd._uiElementId = elementId; // "screen.take", "screen.use", "screen.takeAndUse", "PickUps"
            linkUiAdd.enabled = false;
            linkUiAdd.enabled = true;
            return linkUiAdd;
        }
    }
}
