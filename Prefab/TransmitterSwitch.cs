
using Il2CppInterop.Runtime;
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
                if (rotation == Quaternion.identity) { Misc.Msg("[Spawn] [TransmitterSpawnParameters] Invalid Rotation"); throw new ArgumentException("[TransmitterSpawnParameters] Invalid Rotation!"); }
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

    }

    internal class TransmitterSwitchSpawnParameters : SpawnParameters
    {
        public bool raiseNetworkEvent = false;
        public bool? isOn = null;
        public HashSet<string> linkedUniqueIdsRecivers = null;
    }
}
