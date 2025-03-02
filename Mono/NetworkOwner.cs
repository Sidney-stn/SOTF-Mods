using Il2CppInterop.Runtime;
using RedLoader;
using Sons.Gui.Input;
using SonsSdk;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

        private string _typeOfObject = null;
        private bool _started = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Start()
        {
            if (isSetupPrefab) { Misc.Msg("[NetworkOwner] [Start] SetupPrefab"); return; }
            else
            {
                WaitForSetup().RunCoro();
            }
        }

        public bool CheckIfSettingsWereSetCorrectly()  // Only used in AddNetworkOwnerComp
        {
            if (isSetupPrefab)
            {
                return false;
            }
            if (!fromNetwork)
            {
                return false;
            }
            return true;
        }

        public void FixSettings()  // Only used in AddNetworkOwnerComp
        {
            isSetupPrefab = false;
            fromNetwork = true;
            _started = false;
            WaitForSetup().RunCoro();
        }

        public void TakeOwnerShip()
        {
            if (isSetupPrefab) { return; }
            if (SonsSdk.Networking.NetUtils.IsDedicatedServer)
            {
                Misc.Msg("[NetworkOwner] [TakeOwnerShip] Skip Adding Comp - Recieved On DedicatedServer", true);
                return;
            }
            Misc.Msg("[NetworkOwner] [TakeOwnerShip] Trying To Take Ownership");

            if (LinkUiElement == null) { Misc.Msg("[NetworkOwner] [TakeOwnerShip] Can't Take OwnerShip, LinkUi Is Null"); return; }
            if (!LinkUiElement.IsActive)
            {
                Misc.Msg("[NetworkOwner] [TakeOwnerShip] Can't Take OwnerShip, LinkUi Is Not Active");
                return;
            }

            _placerSteamId = Misc.GetMySteamId();
            if (string.IsNullOrEmpty(_placerSteamId))
            {
                Misc.Msg("[NetworkOwner] [TakeOwnerShip] Failed to get valid SteamID");
                return;
            }

            Misc.Msg("[NetworkOwner] [TakeOwnerShip] No Components Adding Them");
            if (gameObject.name.ToLower().Contains("reciver"))
            {
                try
                {
                    var type = Il2CppType.Of<Mono.Reciver>();
                    if (gameObject.GetComponent(type) == null)
                    {
                        // Check if WirelessSignals.reciver is properly initialized before assigning ownership
                        if (WirelessSignals.reciver == null || WirelessSignals.reciver.spawnedGameObjects == null)
                        {
                            Misc.Msg("[NetworkOwner] [TakeOwnerShip] WirelessSignals.reciver not initialized", true);
                            SonsTools.ShowMessage("Cannot take ownership at this time. Please try again later.", 3f);
                            return;
                        }

                        dynamic addedComp = gameObject.AddComponent(type);
                        addedComp.OnMultiplayerAssignOwner(_placerSteamId);
                        DestroyUi();
                    }
                    else
                    {
                        dynamic addedComp = gameObject.GetComponent(type);
                        addedComp.OnMultiplayerAssignOwner(_placerSteamId);
                        DestroyUi();
                        Misc.Msg("[NetworkOwner] [TakeOwnerShip] Reciver Component Already Exists");
                    }
                }
                catch (System.Exception ex)
                {
                    Misc.Msg($"[NetworkOwner] [TakeOwnerShip] Error: {ex.Message}", true);
                    SonsTools.ShowMessage("Failed to take ownership. Please try again later.", 3f);
                }
            }
            else if (gameObject.name.ToLower().Contains("transmitterdetector"))
            {
                var type = Il2CppType.Of<Mono.TransmitterDetector>();
                if (gameObject.GetComponent(type) == null)
                {
                    dynamic addedComp = gameObject.AddComponent(type);
                    //addedComp.OnMultiplayerAssignOwner(_placerSteamId);  // Not Implemented
                    DestroyUi();
                }
                else
                {
                    dynamic addedComp = gameObject.GetComponent(type);
                    //addedComp.OnMultiplayerAssignOwner(_placerSteamId);  // Not Implemented
                    DestroyUi();
                    Misc.Msg("[NetworkOwner [TakeOwnerShip] TransmitterDetector Component Already Exists");
                }
            }
            else if (gameObject.name.ToLower().Contains("transmitterswitch"))
            {
                var type = Il2CppType.Of<Mono.TransmitterSwitch>();
                if (gameObject.GetComponent(type) == null)
                {
                    dynamic addedComp = gameObject.AddComponent(type);
                    //addedComp.OnMultiplayerAssignOwner(_placerSteamId);  // Not Implemented
                    DestroyUi();
                }
                else
                {
                    dynamic addedComp = gameObject.GetComponent(type);
                    //addedComp.OnMultiplayerAssignOwner(_placerSteamId);  // Not Implemented
                    DestroyUi();
                    Misc.Msg("[NetworkOwner [TakeOwnerShip] TransmitterSwitch Component Already Exists");
                }
            }
            else
            {
                Misc.Msg("[NetworkOwner] [TakeOwnerShip] Unknown object type");
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
                Misc.Msg("[NetworkOwner] [AddComponentOnRestOfPlayers] BoltEntity Is Null", true);
                DestroyUi();
                return;
            }
            switch (_typeOfObject)
            {
                case null:
                    Misc.Msg("[NetworkOwner] [AddComponentOnRestOfPlayers] _typeOfObject Is Null", true);
                    DestroyUi();
                    return;
                case "Reciver":
                    Network.Reciver.ReciverSyncEvent.SendState(thisEntity, Network.Reciver.ReciverSyncEvent.ReciverSyncType.PlaceOnBoltEntity);
                    break;
            }
        }

        private IEnumerator WaitForSetup()
        {
            if (isSetupPrefab) { yield break; }
            _started = true;
            // Wait time in Coroutine before sending
            yield return new WaitForSeconds(1f);
            if (BoltNetwork.isRunning)
            {
                Misc.Msg("[NetworkOwner] [TakeOwnerShip] No Components Adding Them");
                if (gameObject.name.ToLower().Contains("reciver"))
                {
                    _typeOfObject = "Reciver";
                }
                else if (gameObject.name.ToLower().Contains("transmitterdetector"))
                {
                    _typeOfObject = "TransmitterDetector";
                }
                else if (gameObject.name.ToLower().Contains("transmitterswitch"))
                {
                    _typeOfObject = "TransmitterSwitch";
                }
                else
                {
                    Misc.Msg("[NetworkOwner] [TakeOwnerShip] No Components Adding ??");
                }
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