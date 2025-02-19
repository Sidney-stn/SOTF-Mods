
using Il2CppInterop.Runtime;
using RedLoader;
using TheForest.Utils;
using UnityEngine;

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
                IsOn = component.isOn
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
                isOn = saveData.IsOn
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
        }

    }

    internal class ReciverSpawnParameters : SpawnParameters
    {
        public bool raiseNetworkEvent = false;
        public bool? isOn = null;
    }

    
}
