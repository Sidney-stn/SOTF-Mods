
using Il2CppInterop.Runtime;
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
                Misc.Msg("[Spawn] [RaciverSpawnParameters] Parameters Are Correct");
                // Access transmitter-specific parameters
                string uniqueId = reciverParams.uniqueId;
                if (string.IsNullOrEmpty(uniqueId))
                {
                    Misc.Msg("[Spawn] [RaciverSpawnParameters] UniqueId Is Null Or Empty, generating new one");
                    uniqueId = Guid.NewGuid().ToString();  // Generate a new unique ID
                }
                if (DoesUniqueIdExist(uniqueId))
                {
                    Misc.Msg($"[Spawn] [RaciverSpawnParameters] UniqueId {uniqueId} Already Exists!");
                    throw new ArgumentException($"[Spawn] [RaciverSpawnParameters] UniqueId {uniqueId} Already Exists!");
                }
                Misc.Msg("[Spawn] [RaciverSpawnParameters] UniqueId: " + uniqueId);
                if (!LocalPlayer.IsInWorld) { Misc.Msg("[Spawn] [RaciverSpawnParameters] LocalPlayer Is Not In World"); throw new InvalidOperationException("[RaciverSpawnParameters] LocalPlayer Is Not In World!"); }
                if (gameObjectWithComps == null) { Misc.Msg("[Spawn] [RaciverSpawnParameters] Setup GameObject Is Null"); throw new InvalidOperationException("[RaciverSpawnParameters] gameObjectWithComps Is Null!"); }
                Vector3 position = reciverParams.position;
                if (position == Vector3.zero) { Misc.Msg("[Spawn] [RaciverSpawnParameters] Invalid Position"); throw new ArgumentException("[RaciverSpawnParameters] Invalid Position!"); }
                Quaternion rotation = reciverParams.rotation;
                if (rotation == Quaternion.identity) { Misc.Msg("[Spawn] [RaciverSpawnParameters] Invalid Rotation"); throw new ArgumentException("[RaciverSpawnParameters] Invalid Rotation!"); }
                GameObject spawnedObject = GameObject.Instantiate(gameObjectWithComps, position, rotation);
                if (spawnedObject == null) { Misc.Msg("[Spawn] [RaciverSpawnParameters] SpawnedObject Is Null"); throw new InvalidOperationException("[RaciverSpawnParameters] spawnedObject Is Null!"); }
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
                Misc.Msg("[RaciverSpawnParameters] Invalid Parameters");
                throw new ArgumentException("[RaciverSpawnParameters] Invalid Parameters!");
            }
        }

    }

    internal class ReciverSpawnParameters : SpawnParameters
    {
        public bool raiseNetworkEvent = false;
        public bool? isOn = null;
    }
}
