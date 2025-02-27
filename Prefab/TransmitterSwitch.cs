using Il2CppInterop.Runtime;
using RedLoader;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace WirelessSignals.Prefab
{
    internal class WirelessTransmitterSwitch : PrefabBase
    {

        internal override void Setup()
        {
            // Get IL2CPP types for your components
            var components = new List<Il2CppSystem.Type>  // If Adding BoltEntity, Add It Last
                {
                    Il2CppType.Of<Mono.TransmitterSwitch>(),
                    Il2CppType.Of<BoltEntity>(),
                };
            SetupPrefab(Assets.TransmitterSwitch, components, configureComponents: ConfigureComponents, addGrassAndSnowRemover: true);
        }

        internal override void ConfigureComponents(GameObject obj)
        {
            var mainComponent = obj.GetComponent<Mono.TransmitterSwitch>();
            if (mainComponent == null) { throw new InvalidOperationException("[TransmitterSwitch] mainComponent Is Null!"); }
            mainComponent.isOn = null;
            mainComponent.uniqueId = null;
            mainComponent.isSetupPrefab = true;

            CreateDebugUi(obj);

            // Add Sound Component To GameObject -> Sound Not Loaded At This Point
            var soundPlayer = obj.AddComponent<SoundPlayer>();
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
            text.text = "IsOn: Value\r\nUniqueId: Value\r\nLinkedUniqueIdsRecivers: Value";
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
            }
            else { canvasGo.transform.localPosition = new Vector3(0, 0.7f, 0); }
            textObj.transform.localPosition = Vector3.zero;

            visualUi.SetActive(false);
        }

        internal override GameObject Spawn(SpawnParameters parameters)
        {
            Misc.Msg("[Spawn] Spawning TransmitterSwitch");
            if (parameters is TransmitterSwitchSpawnParameters transmitterParams)
            {
                Misc.Msg("[Spawn] [TransmitterSpawnParameters] Parameters Are Correct");
                // Access transmitter-specific parameters
                string uniqueId = transmitterParams.uniqueId;
                if (string.IsNullOrEmpty(uniqueId))
                {
                    Misc.Msg("[Spawn] [TransmitterSpawnParameters] UniqueId Is Null Or Empty, generating new one");
                    uniqueId = Guid.NewGuid().ToString();  // Generate a new unique ID
                }
                if (DoesUniqueIdExist(uniqueId))
                {
                    Misc.Msg($"[Spawn] [TransmitterSpawnParameters] UniqueId {uniqueId} Already Exists!");
                    throw new ArgumentException($"[Spawn] [TransmitterSpawnParameters] UniqueId {uniqueId} Already Exists!");
                }
                Misc.Msg("[Spawn] [TransmitterSpawnParameters] UniqueId: " + uniqueId);
                if (!LocalPlayer.IsInWorld) { Misc.Msg("[Spawn] [TransmitterSpawnParameters] LocalPlayer Is Not In World"); throw new InvalidOperationException("[TransmitterSpawnParameters] LocalPlayer Is Not In World!"); }
                if (gameObjectWithComps == null) { Misc.Msg("[Spawn] [TransmitterSpawnParameters] Setup GameObject Is Null"); throw new InvalidOperationException("[TransmitterSpawnParameters] gameObjectWithComps Is Null!"); }
                Vector3 position = transmitterParams.position;
                if (position == Vector3.zero) { Misc.Msg("[Spawn] [TransmitterSpawnParameters] Invalid Position"); throw new ArgumentException("[TransmitterSpawnParameters] Invalid Position!"); }
                Quaternion rotation = transmitterParams.rotation;
                GameObject spawnedObject = GameObject.Instantiate(gameObjectWithComps, position, rotation);
                if (spawnedObject == null) { Misc.Msg("[Spawn] [TransmitterSpawnParameters] SpawnedObject Is Null"); throw new InvalidOperationException("[TransmitterSpawnParameters] spawnedObject Is Null!"); }
                Mono.TransmitterSwitch controller = spawnedObject.GetComponent<Mono.TransmitterSwitch>();
                controller.uniqueId = uniqueId;
                spawnedGameObjects.Add(uniqueId, spawnedObject);

                Misc.Msg("[Spawn] TransmitterSwitch Spawned");

                // Raise network event if requested
                if (transmitterParams.raiseNetworkEvent)
                {

                }
                return spawnedObject;
            }
            else
            {
                Misc.Msg("[TransmitterSpawnParameters] Invalid Parameters");
                throw new ArgumentException("[TransmitterSpawnParameters] Invalid Parameters!");
            }
        }

        protected override object CreateSaveDataFromGameObject(GameObject obj)
        {
            var component = obj.GetComponent<Mono.TransmitterSwitch>();
            string uniqueId = component.uniqueId;
            if (string.IsNullOrEmpty(uniqueId))
            {
                Misc.Msg("[CreateSaveDataFromGameObject] [TransmitterSwitch] Skipped Saving - UniqueId is null");
                return null;
            }
            Vector3 position = component.GetPosition();
            if (position == Vector3.zero) { Misc.Msg("[CreateSaveDataFromGameObject] [TransmitterSwitch] Skipped Saving - Invalid Position"); return null; }
            Quaternion rotation = component.GetRotation();
            bool? isOn = component.isOn;

            return new TransmitterSwitchSaveData
            {
                UniqueId = component.uniqueId,
                Position = obj.transform.position,
                Rotation = obj.transform.rotation,
                IsOn = component.isOn,
                LinkedUniqueIdsRecivers = component.linkedUniqueIdsRecivers
            };
        }

        protected override SpawnParameters CreateSpawnParametersFromSaveData(object data)
        {
            var saveData = data as TransmitterSwitchSaveData;
            if (saveData == null)
            {
                Misc.Msg($"[Error] Expected TransmitterSwitch but got {data?.GetType()}");
                throw new ArgumentException("Invalid save data type");
            }
            Misc.Msg($"[CreateSpawnParametersFromSaveData] [TransmitterDetector] Creating Spawn Parameters From Loaded Save Data. UniqueId: {saveData.UniqueId} Position: {saveData.Position}");

            return new TransmitterSwitchSpawnParameters
            {
                position = saveData.Position,
                rotation = saveData.Rotation,
                uniqueId = saveData.UniqueId,
                isOn = saveData.IsOn,
                linkedUniqueIdsRecivers = saveData.LinkedUniqueIdsRecivers,
                ownerSteamId = saveData.OwnerSteamId
            };
        }

        protected override void ApplySaveDataToGameObject(GameObject obj, object data)
        {
            var saveData = data as TransmitterSwitchSaveData;
            if (saveData == null) return;

            var component = obj.GetComponent<Mono.TransmitterSwitch>();
            if (component != null)
            {
                component.uniqueId = saveData.UniqueId;
                component.isOn = saveData.IsOn;
                component.linkedUniqueIdsRecivers = saveData.LinkedUniqueIdsRecivers;
                component.ownerSteamId = saveData.OwnerSteamId;
            }
        }

        internal override List<object> GetAllSaveData()
        {
            var allSaveData = new List<TransmitterSwitchSaveData>();
            foreach (var obj in spawnedGameObjects.Values)
            {
                if (obj != null)
                {
                    allSaveData.Add(CreateSaveDataFromGameObject(obj) as TransmitterSwitchSaveData);
                }
            }
            return allSaveData.Cast<object>().ToList();
        }

        [RegisterTypeInIl2Cpp]
        [Serializable]
        public class TransmitterSwitchSaveData : Il2CppSystem.Object
        {
            public string UniqueId;
            public Vector3 Position;
            public Quaternion Rotation;
            public bool? IsOn;
            public HashSet<string> LinkedUniqueIdsRecivers;
            public string OwnerSteamId;
        }
    }

    internal class TransmitterSwitchSpawnParameters : SpawnParameters
    {
        public bool raiseNetworkEvent = false;
        public bool? isOn = null;
        public HashSet<string> linkedUniqueIdsRecivers = null;
        public string ownerSteamId = null;
    }

    
}
