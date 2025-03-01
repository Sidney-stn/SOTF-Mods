using Il2CppInterop.Runtime;
using RedLoader;
using Sons.Gui.Input;
using SonsSdk;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WirelessSignals.Network.Sync;

namespace WirelessSignals.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class NetworkOwner : MonoBehaviour
    {
        public bool isSetupPrefab = true;
        private string _placerSteamId = null;
        private GameObject _ui = null;
        public LinkUiElement LinkUiElement = null;
        public bool fromNetwork = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Start()
        {
            if (isSetupPrefab) { Misc.Msg("[NetworkOwner] [Awake] SetupPrefab"); return; }
            else
            {

                //if (BoltNetwork.isRunning)
                //{
                //    CreateTakeOwnerUi(gameObject, false);
                //    LinkUiElement = UI.LinkUi.CreateLinkUi(gameObject, 2f, Assets.UITakeOwner, null, new Vector3(0, 0, 0));
                //    if (fromNetwork == false)
                //    {
                //        Misc.Msg("[NetworkOwner] [Awake] FromNetwork");
                //        //AddComponentOnRestOfPlayers();
                //        SendAddComponentOnRestOfPlayers().RunCoro();
                //    }
                //}
                //else
                //{
                //    Misc.Msg("[NetworkOwner] [Awake] BoltNetwork is not running");
                //    Destroy(this);
                //}
                WaitForSetup().RunCoro();
            }
            
            
        }

        public void TakeOwnerShip()
        {
            if (isSetupPrefab) { return; }
            _placerSteamId = Misc.GetMySteamId();

            // Check If Any Of My Scripts Exist
            List<Il2CppSystem.Type> componentsToMakeStructureWork = new List<Il2CppSystem.Type>
            {
                Il2CppType.Of<Mono.Reciver>(),
                Il2CppType.Of<Mono.TransmitterDetector>(),
                Il2CppType.Of<Mono.TransmitterSwitch>(),
            };

            // Check If Type Exists, If Not Add It
            if (componentsToMakeStructureWork == null)
            {
                Misc.Msg("[NetworkOwner] [TakeOwnerShip] componentsToMakeStructureWork Is Null");
                DestroyUi();
                return;
            }

            List<Il2CppSystem.Type> foundTypes = new List<Il2CppSystem.Type>();
            foreach (var compType in componentsToMakeStructureWork)
            {
                if (gameObject.GetComponent(compType) != null)
                {
                    foundTypes.Add(compType);
                }
            }
            if (foundTypes.Count == 0)
            {
                Misc.Msg("[NetworkOwner] [TakeOwnerShip] No Components Adding Them");
                if (gameObject.name.ToLower().Contains("reciver"))
                {
                    gameObject.AddComponent<Mono.Reciver>();
                    foundTypes.Add(Il2CppType.Of<Mono.Reciver>());
                }
                else if (gameObject.name.ToLower().Contains("transmitterdetector"))
                {
                    gameObject.AddComponent<Mono.TransmitterDetector>();
                    foundTypes.Add(Il2CppType.Of<Mono.TransmitterDetector>());
                }
                else if (gameObject.name.ToLower().Contains("transmitterswitch"))
                {
                    gameObject.AddComponent<Mono.TransmitterSwitch>();
                    foundTypes.Add(Il2CppType.Of<Mono.TransmitterSwitch>());
                }
                else
                {
                    Misc.Msg("[NetworkOwner] [TakeOwnerShip] No Components Adding ??");
                }

            }
            foreach (var compType in foundTypes)
            {
                if (gameObject.GetComponent(compType) == null)
                {
                    Misc.Msg("[NetworkOwner] [TakeOwnerShip] Component Not Found After Adding");
                    Destroy(_ui);
                    Destroy(LinkUiElement);
                    Destroy(this);
                    return;
                }
                if (compType == Il2CppType.Of<Mono.Reciver>())
                {
                    Misc.Msg("[NetworkOwner] [TakeOwnerShip] Reciver Component Found");
                    Mono.Reciver reciver = gameObject.GetComponent<Mono.Reciver>();
                    if (reciver == null)
                    {
                        Misc.Msg("[NetworkOwner] [TakeOwnerShip] Reciver Component Is Null");
                        DestroyUi();
                    }
                    reciver.OnMultiplayerAssignOwner(_placerSteamId);
                }
                else if (compType == Il2CppType.Of<Mono.TransmitterDetector>())
                {
                    Misc.Msg("[NetworkOwner [TakeOwnerShip] TransmitterDetector Component Found");
                    Mono.TransmitterDetector transmitterDetector = gameObject.GetComponent<Mono.TransmitterDetector>();
                    if (transmitterDetector == null)
                    {
                        Misc.Msg("[NetworkOwner [TakeOwnerShip] TransmitterDetector Component Is Null");
                        DestroyUi();
                        return;
                    }
                    //transmitterDetector.OnMultiplayerAssignOwner(_placerSteamId);  // Not Implemented
                }
                else if (compType == Il2CppType.Of<Mono.TransmitterSwitch>())
                {
                    Misc.Msg("[NetworkOwner] [TakeOwnerShip] TransmitterSwitch Component Found");
                    Mono.TransmitterSwitch transmitterSwitch = gameObject.GetComponent<Mono.TransmitterSwitch>();
                    if (transmitterSwitch == null)
                    {
                        Misc.Msg("[NetworkOwner [TakeOwnerShip] TransmitterSwitch Component Is Null");
                        DestroyUi();
                    }
                    //transmitterSwitch.OnMultiplayerAssignOwner(_placerSteamId);  // Not Implemented
                }
            }
        }

        private void CreateTakeOwnerUi(GameObject obj, bool lower = true)  // Lower Is Height Placement
        {
            Misc.Msg("[NetworkOwner] [CreateTakeOwnerUi] Creating UI");
            // Create Debug Ui
            _ui = new GameObject("OWNER_UI");
            _ui.transform.SetParent(obj.transform);

            GameObject canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(_ui.transform);

            RectTransform canvasT = canvasGo.AddComponent<RectTransform>();
            //canvasT.position = new Vector3(0, 0, 0);
            canvasT.localScale = new Vector3(0.005f, 0.005f, 0.005f);
            canvasT.pivot = new Vector2(0.5f, 0.5f);
            canvasT.anchorMax = new Vector2(0, 0);
            canvasT.anchorMin = new Vector2(0, 0);
            canvasT.rotation = Quaternion.Euler(0, 0, 0);
            canvasT.sizeDelta = new Vector2(1920, 1080);

            canvasGo.layer = 0; // Default
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingLayerName = "Default";
            canvas.sortingOrder = 0;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
            canvas.vertexColorAlwaysGammaSpace = false;

            CanvasScaler canvasScaler = canvasGo.AddComponent<CanvasScaler>();
            canvasScaler.dynamicPixelsPerUnit = 5;
            canvasScaler.referencePixelsPerUnit = 100;

            GraphicRaycaster graphicRaycaster = canvasGo.AddComponent<GraphicRaycaster>();
            graphicRaycaster.ignoreReversedGraphics = true;
            graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            graphicRaycaster.blockingMask = -1;

            GameObject textObj = new GameObject("Text");
            textObj.SetParent(canvasGo.transform);
            RectTransform textT = textObj.AddComponent<RectTransform>();
            textT.position = new Vector3(0, 0, 0);
            textT.localScale = new Vector3(1, 1, 1);
            textT.pivot = new Vector2(0.5f, 0.5f);
            textT.anchorMax = new Vector2(0.5f, 0.5f);
            textT.anchorMin = new Vector2(0.5f, 0.5f);
            textT.rotation = Quaternion.Euler(0, 0, 0);
            textT.sizeDelta = new Vector2(192, 70);

            CanvasRenderer textCanvasRenderer = textObj.AddComponent<CanvasRenderer>();
            textCanvasRenderer.cullTransparentMesh = true;

            Text text = textObj.AddComponent<Text>();
            text.text = "Take OwnerShip";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontStyle = FontStyle.Normal;
            text.fontSize = 18;
            text.lineSpacing = 1;
            text.supportRichText = true;
            text.alignByGeometry = false;
            text.alignment = TextAnchor.UpperCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.resizeTextForBestFit = false;
            text.color = new Color(1, 1, 1, 1);
            text.material = null;
            text.raycastTarget = true;
            text.raycastPadding = new Vector4(0, 0, 0, 0);
            text.maskable = true;

            _ui.transform.localPosition = new Vector3(0, 0.7f, 0);
            if (lower)
            {
                canvasGo.transform.localPosition = new Vector3(0, 0f, 0);
            }
            else { canvasGo.transform.localPosition = new Vector3(0, 0.7f, 0); }

            textObj.transform.localPosition = Vector3.zero;
            text.text = "Take OwnerShip";
            _ui.transform.rotation = gameObject.transform.rotation;
            _ui.SetActive(true);
        }

        private void AddComponentOnRestOfPlayers()
        {
            BoltEntity thisEntity = gameObject.GetComponent<BoltEntity>();
            if (thisEntity == null)
            {
                Misc.Msg("[NetworkOwner] [AddComponentOnRestOfPlayers] BoltEntity Is Null");
                DestroyUi();
                return;
            }
            
            NetworkOwnerSyncEvent.SendState(thisEntity, NetworkOwnerSyncEvent.SyncType.PlaceOnBoltEntity);
        }

        private IEnumerator WaitForSetup()
        {
            if (isSetupPrefab) { yield break; }
            // Wait time in Coroutine before sending
            yield return new WaitForSeconds(1f);
            if (BoltNetwork.isRunning)
            {
                CreateTakeOwnerUi(gameObject, false);
                LinkUiElement = UI.LinkUi.CreateLinkUi(gameObject, 2f, Assets.UITakeOwner, null, new Vector3(0, 0, 0));
                if (fromNetwork == false)
                {
                    Misc.Msg("[NetworkOwner] [Awake] FromNetwork");
                    //AddComponentOnRestOfPlayers();
                    SendAddComponentOnRestOfPlayers().RunCoro();
                }
            }
            else
            {
                Misc.Msg("[NetworkOwner] [Awake] BoltNetwork is not running");
                Destroy(this);
            }
            yield break;
        }

        private IEnumerator SendAddComponentOnRestOfPlayers()
        {
            if (isSetupPrefab) { yield break; }
            // Wait time in Coroutine before sending
            yield return new WaitForSeconds(1f);
            AddComponentOnRestOfPlayers();
        }

        

        public void DestroyUi()
        {
            Destroy(_ui);
            Destroy(LinkUiElement);
            Destroy(this);
        }

        public void OnDisable()
        {
            DestroyUi();
        }
    }
}
