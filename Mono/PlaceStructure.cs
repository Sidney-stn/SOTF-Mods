using RedLoader;
using Sons.Crafting.Structures;
using SonsSdk;
using UnityEngine;


namespace WirelessSignals.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class PlaceStructure : MonoBehaviour
    {
        public bool isSetupPrefab;
        public string structureName;
        public bool destroyBoltEntity;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Start()
        {
            if (isSetupPrefab) { return; }

            Misc.Msg("[PlaceStructure] Start");
            if (structureName == null)
            {
                if (gameObject.name.ToLower().Contains("reciver"))
                {
                    structureName = "Reciver";
                }
                else if (gameObject.name.ToLower().Contains("transmitterswitch"))
                {
                    structureName = "TransmitterSwitch";
                }
                else if (gameObject.name.ToLower().Contains("transmitterdetector"))
                {
                    structureName = "TransmitterDetector";
                }
            }
            Misc.Msg("[PlaceStructure] Deleting Bolt And ScrewStructure");
            ScrewStructure scewStructure = gameObject.GetComponent<ScrewStructure>();
            if (scewStructure != null) { DestroyImmediate(scewStructure); Misc.Msg("[PlaceStructure] [Start] ScrewStructure Deleted"); } else { Misc.Msg("[PlaceStructure] [Start] ScrewStructure Not Found For Deletion!"); }
            BoltEntity bolt = gameObject.GetComponent<BoltEntity>();
            if (bolt != null)
            {
                Misc.Msg("[PlaceStructure] [Start] BoltEntity Found For Deletion");
                if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
                {
                    if (destroyBoltEntity)
                    {
                        bolt.Entity.Detach();
                    }
                }
                if (destroyBoltEntity)
                {
                    DestroyImmediate(bolt);
                }
                
                Misc.Msg("[PlaceStructure] [Start] BoltEntity Deleted");
            }
            else { Misc.Msg("[PlaceStructure] [Start] BoltEntity Not Found For Deletion!"); }

            if (gameObject != null)
            {
                if (string.IsNullOrEmpty(structureName)) { RLog.Error("[PlaceStructure] [Start] StructureName Is Null Or Empty!"); return; }

                Misc.Msg("[PlaceStructure] Adding Components");
                switch (structureName.ToLower())  // These names are the same as bluprintName inhereted from StructureBase
                {
                    case "reciver":
                        Misc.Msg("[PlaceStructure] [Start] Reciver Started Adding Component");
                        string uniqueId = Guid.NewGuid().ToString();
                        if (string.IsNullOrEmpty(uniqueId)) { Misc.Msg("[PlaceStructure] [Start] uniqueId Is Null Or Empty!"); return; }
                        Mono.Reciver reciverController = gameObject.AddComponent<Mono.Reciver>();
                        reciverController.uniqueId = uniqueId;
                        reciverController.ownerSteamId = Misc.GetMySteamId();
                        reciverController.loadedFromSave = false;
                        WirelessSignals.reciver.spawnedGameObjects.Add(uniqueId, gameObject);
                        Misc.Msg($"[PlaceStructure] [Start] Reciver Set UniqueId {uniqueId}");

                        // Setup Debug UI
                        Prefab.Reciver.CreateDebugUi(gameObject, true);

                        // Setup Grass And Snow Remover
                        WirelessSignals.reciver.CleanGrassAndSnow(gameObject);

                        // Network

                        break;
                    case "transmitterswitch":
                        Misc.Msg("[PlaceStructure] [Start] TransmitterSwitch Started Adding Component");
                        string uniqueId1 = Guid.NewGuid().ToString();
                        if (string.IsNullOrEmpty(uniqueId1)) { Misc.Msg("[PlaceStructure] [Start] uniqueId Is Null Or Empty!"); return; }
                        Mono.TransmitterSwitch transmitterController = gameObject.AddComponent<Mono.TransmitterSwitch>();
                        transmitterController.uniqueId = uniqueId1;
                        transmitterController.ownerSteamId = Misc.GetMySteamId();
                        WirelessSignals.transmitterSwitch.spawnedGameObjects.Add(uniqueId1, gameObject);
                        Misc.Msg($"[PlaceStructure] [Start] TransmitterSwitch Set UniqueId {uniqueId1}");

                        // Setup Debug UI
                        Prefab.WirelessTransmitterSwitch.CreateDebugUi(gameObject, true);

                        // Setup Grass And Snow Remover
                        WirelessSignals.transmitterSwitch.CleanGrassAndSnow(gameObject);

                        // Add Sound To GameObject
                        var soundPlayer = gameObject.AddComponent<SoundPlayer>();

                        // Network

                        break;
                    case "transmitterdetector":
                        Misc.Msg("[PlaceStructure] [Start] TransmitterDetector Started Adding Component");
                        string uniqueId2 = Guid.NewGuid().ToString();
                        if (string.IsNullOrEmpty(uniqueId2)) { Misc.Msg("[PlaceStructure] [Start] uniqueId Is Null Or Empty!"); return; }
                        Mono.TransmitterDetector transmitterDetectorController = gameObject.AddComponent<Mono.TransmitterDetector>();
                        transmitterDetectorController.uniqueId = uniqueId2;
                        transmitterDetectorController.ownerSteamId = Misc.GetMySteamId();
                        WirelessSignals.transmitterDetector.spawnedGameObjects.Add(uniqueId2, gameObject);
                        Misc.Msg($"[PlaceStructure] [Start] TransmitterDetector Set UniqueId {uniqueId2}");

                        // Setup Debug UI
                        Prefab.TransmitterDetector.CreateDebugUi(gameObject, true);

                        // Setup Grass And Snow Remover
                        WirelessSignals.transmitterDetector.CleanGrassAndSnow(gameObject);

                        // Network

                        break;
                    default:
                        Misc.Msg($"[PlaceStructure] [Start] StructureName {structureName} Not Found!");
                        break;
                }
            }
            else { Misc.Msg("[PlaceStructure] [Start] GameObject That PlaceStructure Ccomponent Is Attatched To Is Null!!!!"); }

            Misc.Msg("[PlaceStructure] [Start] Destroying PlaceStructure Component");
            Destroy(this);
        }
    }
}
