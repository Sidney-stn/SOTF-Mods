
using Il2CppInterop.Runtime;
using RedLoader;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace WirelessSignals.Prefab
{
    internal class Reciver : PrefabBase
    {

        internal override void Setup()
        {
            // Get IL2CPP types for your components
            var components = new List<Il2CppSystem.Type>
                {
                    Il2CppType.Of<Mono.Reciver>(),
                    //Il2CppType.Of<Construction.DefensiveWallGateControl>(),
                };
            SetupPrefab(Assets.Reciver, components, configureComponents: ConfigureComponents);
        }

        internal override void ConfigureComponents(GameObject obj)
        {
            var mainComponent = obj.GetComponent<Mono.Reciver>();
            if (mainComponent == null) { throw new InvalidOperationException("[Reciver] mainComponent Is Null!"); }
            mainComponent.isOn = null;
            mainComponent.uniqueId = null;
            mainComponent.isSetupPrefab = true;

            CreateDebugUi(obj);            
        }

        internal static void CreateDebugUi(GameObject obj, bool lower = false)
        {
            // Create Debug Ui
            GameObject visualUi = new GameObject("UI");
            visualUi.transform.SetParent(obj.transform);

            GameObject canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(visualUi.transform);

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
            text.text = "IsOn: Value\r\nUniqueId: Value\r\nLinkedToTranmitterSwithUniqueId: Value";
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

            visualUi.transform.localPosition = new Vector3(0, 0.7f, 0);
            if (lower)
            {
                canvasGo.transform.localPosition = new Vector3(0, 0f, 0);
            } else { canvasGo.transform.localPosition = new Vector3(0, 0.7f, 0); }
                
            textObj.transform.localPosition = Vector3.zero;

            visualUi.SetActive(false);
        }

        internal override GameObject Spawn(SpawnParameters parameters)
        {
            Misc.Msg("[Spawn] Spawning Reciver");
            if (parameters is ReciverSpawnParameters reciverParams)
            {
                Misc.Msg("[Spawn] [ReciverSpawnParameters] Parameters Are Correct");
                // Access transmitter-specific parameters
                string uniqueId = reciverParams.uniqueId;
                if (string.IsNullOrEmpty(uniqueId))
                {
                    Misc.Msg("[Spawn] [ReciverSpawnParameters] UniqueId Is Null Or Empty, generating new one");
                    uniqueId = Guid.NewGuid().ToString();  // Generate a new unique ID
                }
                if (DoesUniqueIdExist(uniqueId))
                {
                    Misc.Msg($"[Spawn] [ReciverSpawnParameters] UniqueId {uniqueId} Already Exists!");
                    throw new ArgumentException($"[Spawn] [ReciverSpawnParameters] UniqueId {uniqueId} Already Exists!");
                }
                Misc.Msg("[Spawn] [ReciverSpawnParameters] UniqueId: " + uniqueId);
                if (!LocalPlayer.IsInWorld) { Misc.Msg("[Spawn] [ReciverSpawnParameters] LocalPlayer Is Not In World"); throw new InvalidOperationException("[ReciverSpawnParameters] LocalPlayer Is Not In World!"); }
                if (gameObjectWithComps == null) { Misc.Msg("[Spawn] [ReciverSpawnParameters] Setup GameObject Is Null"); throw new InvalidOperationException("[ReciverSpawnParameters] gameObjectWithComps Is Null!"); }
                Vector3 position = reciverParams.position;
                if (position == Vector3.zero) { Misc.Msg("[Spawn] [ReciverSpawnParameters] Invalid Position"); throw new ArgumentException("[ReciverSpawnParameters] Invalid Position!"); }
                Quaternion rotation = reciverParams.rotation;
                GameObject spawnedObject = GameObject.Instantiate(gameObjectWithComps, position, rotation);
                if (spawnedObject == null) { Misc.Msg("[Spawn] [ReciverSpawnParameters] SpawnedObject Is Null"); throw new InvalidOperationException("[ReciverSpawnParameters] spawnedObject Is Null!"); }
                Mono.Reciver controller = spawnedObject.GetComponent<Mono.Reciver>();
                controller.uniqueId = uniqueId;

                spawnedGameObjects.Add(uniqueId, spawnedObject);

                Misc.Msg("[Spawn] Reciver Spawned");

                // Raise network event if requested
                if (reciverParams.raiseNetworkEvent)
                {

                }
                return spawnedObject;
            }
            else
            {
                Misc.Msg("[ReciverSpawnParameters] Invalid Parameters");
                throw new ArgumentException("[ReciverSpawnParameters] Invalid Parameters!");
            }
        }

        protected override object CreateSaveDataFromGameObject(GameObject obj)
        {
            var component = obj.GetComponent<Mono.Reciver>();
            string uniqueId = component.uniqueId;
            if (string.IsNullOrEmpty(uniqueId)) { 
                Misc.Msg("[CreateSaveDataFromGameObject] [Reciver] Skipped Saving - UniqueId is null");
                return null;
            }
            Vector3 position = component.GetPosition();
            if (position == Vector3.zero) { Misc.Msg("[CreateSaveDataFromGameObject] [Reciver] Skipped Saving - Invalid Position"); return null; }
            Quaternion rotation = component.GetRotation();
            bool? isOn = component.isOn;
            return new ReceiverSaveData
            {
                UniqueId = uniqueId,
                Position = obj.transform.position,
                Rotation = obj.transform.rotation,
                IsOn = component.isOn,
                LinkedToTranmitterSwithUniqueId = component.linkedToTranmitterSwithUniqueId,
                LinkedReciverObject = component.IsLinkedReciverObject(),
                LinkedReciverObjectName = component.GetLinkedReciverObjectName()
            };
        }

        protected override SpawnParameters CreateSpawnParametersFromSaveData(object data)
        {
            var saveData = data as ReceiverSaveData;
            if (saveData == null)
            {
                Misc.Msg($"[Error] Expected ReceiverSaveData but got {data?.GetType()}");
                throw new ArgumentException("Invalid save data type");
            }

            Misc.Msg($"[CreateSpawnParametersFromSaveData] [Receiver] Creating Spawn Parameters From Loaded Save Data. UniqueId: {saveData.UniqueId} Position: {saveData.Position}");

            return new ReciverSpawnParameters
            {
                position = saveData.Position,
                rotation = saveData.Rotation,
                uniqueId = saveData.UniqueId,
                isOn = saveData.IsOn,
                linkedToTranmitterSwithUniqueId = saveData.LinkedToTranmitterSwithUniqueId,
                linkedReciverObject = saveData.LinkedReciverObject,
                linkedReciverObjectName = saveData.LinkedReciverObjectName
            };
        }

        protected override void ApplySaveDataToGameObject(GameObject obj, object data)
        {
            var saveData = data as ReceiverSaveData;
            if (saveData == null) return;

            var component = obj.GetComponent<Mono.Reciver>();
            if (component != null)
            {
                component.uniqueId = saveData.UniqueId;
                component.isOn = saveData.IsOn;
                component.linkedToTranmitterSwithUniqueId = saveData.LinkedToTranmitterSwithUniqueId;
                component.SetLinkedReciverObject(saveData.LinkedReciverObject, saveData.LinkedReciverObjectName);
            }
        }

        internal override List<object> GetAllSaveData()
        {
            var allSaveData = new List<ReceiverSaveData>();
            foreach (var obj in spawnedGameObjects.Values)
            {
                if (obj != null)
                {
                    allSaveData.Add(CreateSaveDataFromGameObject(obj) as ReceiverSaveData);
                }
            }
            return allSaveData.Cast<object>().ToList();
        }

        // Base class for specific save data types
        [RegisterTypeInIl2Cpp]
        [Serializable]
        public class ReceiverSaveData : Il2CppSystem.Object
        {
            public string UniqueId;
            public Vector3 Position;
            public Quaternion Rotation;
            public bool? IsOn;
            public string LinkedToTranmitterSwithUniqueId;
            public bool LinkedReciverObject;
            public string LinkedReciverObjectName;
        }

    }

    internal class ReciverSpawnParameters : SpawnParameters
    {
        public bool raiseNetworkEvent = false;
        public bool? isOn = null;
        public string linkedToTranmitterSwithUniqueId = null;
        public bool linkedReciverObject = false;
        public string linkedReciverObjectName = null;
    }

    
}
