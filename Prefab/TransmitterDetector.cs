using Il2CppInterop.Runtime;
using RedLoader;
using SonsSdk;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace WirelessSignals.Prefab
{
    internal class TransmitterDetector : PrefabBase
    {

        internal override void Setup()
        {
            // Get IL2CPP types for your components
            var components = new List<Il2CppSystem.Type>
                {
                    Il2CppType.Of<Mono.TransmitterDetector>(),
                };
            SetupPrefab(Assets.TransmitterDetector, components, configureComponents: ConfigureComponents);
        }

        internal override void ConfigureComponents(GameObject obj)
        {
            // Add wire to the structure
            Transform wirePlacement = obj.transform.GetChild(0).GetChild(22);
            if (wirePlacement == null) { throw new InvalidOperationException("[TransmitterSwitch] [CompleteSetup] WirePlacement Is Null!"); }
            GameObject wire = GameObject.Instantiate(ItemTools.GetHeldPrefab(418).gameObject, wirePlacement);
            HeldItemIdentifier itemIdent = wire.GetComponent<HeldItemIdentifier>();
            GameObject.Destroy(itemIdent);
            wire.transform.localScale = new Vector3(5, 3, 5);
            wire.transform.rotation = Quaternion.Euler(90, 0, 0);
            wire.transform.localPosition = new Vector3(0, 0.16f, 0.1f);

            var mainComponent = obj.GetComponent<Mono.TransmitterDetector>();
            if (mainComponent == null) { throw new InvalidOperationException("[TransmitterDetector] mainComponent Is Null!"); }
            mainComponent.isOn = null;
            mainComponent.uniqueId = null;
            mainComponent.isSetupPrefab = true;

        }

        internal override GameObject Spawn(SpawnParameters parameters)
        {
            Misc.Msg("[Spawn] Spawning TransmitterDetector");
            if (parameters is TransmitterDetectorSpawnParameters transmitterParams)
            {
                Misc.Msg("[Spawn] [TransmitterDetectorSpawnParameters] Parameters Are Correct");
                // Access transmitter-specific parameters
                string uniqueId = transmitterParams.uniqueId;
                if (string.IsNullOrEmpty(uniqueId))
                {
                    Misc.Msg("[Spawn] [TransmitterDetectorSpawnParameters] UniqueId Is Null Or Empty, generating new one");
                    uniqueId = Guid.NewGuid().ToString();  // Generate a new unique ID
                }
                if (DoesUniqueIdExist(uniqueId))
                {
                    Misc.Msg($"[Spawn] [TransmitterDetectorSpawnParameters] UniqueId {uniqueId} Already Exists!");
                    throw new ArgumentException($"[Spawn] [TransmitterDetectorSpawnParameters] UniqueId {uniqueId} Already Exists!");
                }
                Misc.Msg("[Spawn] [TransmitterDetectorSpawnParameters] UniqueId: " + uniqueId);
                if (!LocalPlayer.IsInWorld) { Misc.Msg("[Spawn] [TransmitterDetectorSpawnParameters] LocalPlayer Is Not In World"); throw new InvalidOperationException("[TransmitterDetectorSpawnParameters] LocalPlayer Is Not In World!"); }
                if (gameObjectWithComps == null) { Misc.Msg("[Spawn] [TransmitterDetectorSpawnParameters] Setup GameObject Is Null"); throw new InvalidOperationException("[TransmitterDetectorSpawnParameters] gameObjectWithComps Is Null!"); }
                Vector3 position = transmitterParams.position;
                if (position == Vector3.zero) { Misc.Msg("[Spawn] [TransmitterDetectorSpawnParameters] Invalid Position"); throw new ArgumentException("[TransmitterDetectorSpawnParameters] Invalid Position!"); }
                Quaternion rotation = transmitterParams.rotation;
                if (rotation == Quaternion.identity) { Misc.Msg("[Spawn] [TransmitterDetectorSpawnParameters] Invalid Rotation"); throw new ArgumentException("[TransmitterDetectorSpawnParameters] Invalid Rotation!"); }
                GameObject spawnedObject = GameObject.Instantiate(gameObjectWithComps, position, rotation);
                if (spawnedObject == null) { Misc.Msg("[Spawn] [TransmitterDetectorSpawnParameters] SpawnedObject Is Null"); throw new InvalidOperationException("[TransmitterDetectorSpawnParameters] spawnedObject Is Null!"); }
                Mono.TransmitterDetector controller = spawnedObject.GetComponent<Mono.TransmitterDetector>();
                controller.uniqueId = uniqueId;
                spawnedGameObjects.Add(uniqueId, spawnedObject);

                Misc.Msg("[Spawn] TransmitterDetector Spawned");

                // Raise network event if requested
                if (transmitterParams.raiseNetworkEvent)
                {

                }
                return spawnedObject;
            }
            else
            {
                Misc.Msg("[TransmitterDetectorSpawnParameters] Invalid Parameters");
                throw new ArgumentException("[TransmitterDetectorSpawnParameters] Invalid Parameters!");
            }
        }

        protected override Saving.SaveData CreateSaveDataFromGameObject(GameObject obj)
        {
            var component = obj.GetComponent<Mono.TransmitterDetector>();
            return new TransmitterDetectorSaveData
            {
                UniqueId = component.uniqueId,
                Position = obj.transform.position,
                Rotation = obj.transform.rotation,
                IsOn = component.isOn
            };
        }

        protected override SpawnParameters CreateSpawnParametersFromSaveData(Saving.SaveData data)
        {
            var receiverData = data as TransmitterDetectorSaveData;
            if (receiverData == null)
                throw new ArgumentException("Invalid save data type");

            return new TransmitterDetectorSpawnParameters
            {
                position = receiverData.Position,
                rotation = receiverData.Rotation,
                uniqueId = receiverData.UniqueId,
                isOn = receiverData.IsOn
            };
        }

        protected override void ApplySaveDataToGameObject(GameObject obj, Saving.SaveData data)
        {
            var receiverData = data as TransmitterDetectorSaveData;
            if (receiverData == null) return;

            var component = obj.GetComponent<Mono.TransmitterDetector>();
            if (component != null)
            {
                component.uniqueId = receiverData.UniqueId;
                component.isOn = receiverData.IsOn;
            }
        }

    }

    internal class TransmitterDetectorSpawnParameters : SpawnParameters
    {
        public bool raiseNetworkEvent = false;
        public bool? isOn = null;
        public HashSet<string> linkedUniqueIdsRecivers = null;
    }

    // Base class for specific save data types
    [RegisterTypeInIl2Cpp]
    [Serializable]
    public class TransmitterDetectorSaveData : Saving.SaveData
    {
        public bool? IsOn { get; set; }
        // Add other receiver-specific properties
    }
}
