using Il2CppInterop.Runtime;
using RedLoader;
using TheForest.Utils;
using UnityEngine;

namespace WirelessSignals.Prefab
{
    internal class WirelessTransmitterSwitch : PrefabBase
    {

        internal override void Setup()
        {
            // Get IL2CPP types for your components
            var components = new List<Il2CppSystem.Type>
                {
                    Il2CppType.Of<Mono.TransmitterSwitch>(),
                };
            SetupPrefab(Assets.TransmitterSwitch, components, configureComponents: ConfigureComponents);
        }

        internal override void ConfigureComponents(GameObject obj)
        {
            var mainComponent = obj.GetComponent<Mono.TransmitterSwitch>();
            if (mainComponent == null) { throw new InvalidOperationException("[TransmitterSwitch] mainComponent Is Null!"); }
            mainComponent.isOn = null;
            mainComponent.uniqueId = null;
            mainComponent.isSetupPrefab = true;

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
                IsOn = component.isOn
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
                isOn = saveData.IsOn
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
        }
    }

    internal class TransmitterSwitchSpawnParameters : SpawnParameters
    {
        public bool raiseNetworkEvent = false;
        public bool? isOn = null;
        public HashSet<string> linkedUniqueIdsRecivers = null;
    }

    
}
