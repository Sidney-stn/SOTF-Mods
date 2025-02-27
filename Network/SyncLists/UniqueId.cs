using RedLoader;
using UdpKit;
using UnityEngine;

namespace WirelessSignals.Network.SyncLists
{
    public class UniqueIdSync : EventBase<UniqueIdSync>
    {
        public enum UniqueIdListOptions : byte
        {
            Add = 0,
            Remove = 1,
            Clear = 2,
            SetAll = 3,
            Set = 4,
        }

        public enum UniqueIdListType : byte
        {
            Reciver = 0,
            TransmitterSwitch = 1,
            TransmitterDetector = 2,
            All = 3,
        }

        /// Read message on the client
        protected override void ReadMessageClient(UdpPacket packet, BoltConnection _)  // This is Recived From SendServerResponseToAll
        {

            var toDo = (UniqueIdListOptions)packet.ReadByte();
            var forPrefab = (UniqueIdListType)packet.ReadByte();
            string steamId = packet.ReadString();
            if (string.IsNullOrEmpty(steamId) || steamId == "0") { Misc.Msg("[UniqueIdSync] [ReadMessageClient] SteamId is null or empty", true); return; }
            if (steamId == Misc.GetMySteamId()) { Misc.Msg("[UniqueIdSync] [ReadMessageClient] Skipped Recieving Own Event"); return; }
            switch (toDo)
            {
                case UniqueIdListOptions.Add:
                    switch (forPrefab)
                    {
                        case UniqueIdListType.Reciver or UniqueIdListType.TransmitterSwitch or UniqueIdListType.TransmitterDetector:
                            string id = packet.ReadString();
                            if (string.IsNullOrEmpty(id)) { Misc.Msg("[UniqueIdSync] [ReadMessageClient] Id is null or empty", true); return; }
                            BoltEntity boltEntity = packet.ReadBoltEntity();
                            if (boltEntity == null) { Misc.Msg("[UniqueIdSync] [ReadMessageClient] BoltEntity is null", true); return; }
                            switch (forPrefab)
                            {
                                case UniqueIdListType.Reciver:
                                    WirelessSignals.reciver.AddBoltEntityToDictionary(id, boltEntity);
                                    break;
                                case UniqueIdListType.TransmitterSwitch:
                                    WirelessSignals.transmitterSwitch.AddBoltEntityToDictionary(id, boltEntity);
                                    break;
                                case UniqueIdListType.TransmitterDetector:
                                    WirelessSignals.transmitterDetector.AddBoltEntityToDictionary(id, boltEntity);
                                    break;
                                case UniqueIdListType.All:
                                    break;
                            }
                            break;
                        case UniqueIdListType.All:
                            RLog.Warning("[UniqueIdSync] [ReadMessageClient] All Case for Add Is Invalid!");
                            return;
                    }
                    break;
                case UniqueIdListOptions.Clear:
                    switch (forPrefab)
                    {
                        case UniqueIdListType.Reciver:
                            WirelessSignals.reciver.spawnedGameObjects.Clear();
                            break;
                        case UniqueIdListType.TransmitterSwitch:
                            WirelessSignals.transmitterSwitch.spawnedGameObjects.Clear();
                            break;
                        case UniqueIdListType.TransmitterDetector:
                            WirelessSignals.transmitterDetector.spawnedGameObjects.Clear();
                            break;
                    }
                    break;
                case UniqueIdListOptions.Remove:
                    switch (forPrefab)
                    {
                        case UniqueIdListType.Reciver or UniqueIdListType.TransmitterSwitch or UniqueIdListType.TransmitterDetector:
                            // Sequence: Id
                            string id = packet.ReadString();
                            if (string.IsNullOrEmpty(id)) { Misc.Msg("[UniqueIdSync] [ReadMessageClient] Id is null or empty", true); return; }
                            switch (forPrefab)
                            {
                                case UniqueIdListType.Reciver:
                                    WirelessSignals.reciver.RemoveByUniqueId(id);  // Remove and destroy
                                    break;
                                case UniqueIdListType.TransmitterSwitch:
                                    WirelessSignals.transmitterSwitch.RemoveByUniqueId(id);  // Remove and destroy
                                    break;
                                case UniqueIdListType.TransmitterDetector:
                                    WirelessSignals.transmitterDetector.RemoveByUniqueId(id);  // Remove and destroy
                                    break;
                                default:
                                    RLog.Warning("[Network] [UniqueIdSync] [ReadMessageClient] Invalid UniqueIdListOptions");
                                    break;
                            }
                            break;
                    }
                    break;
                case UniqueIdListOptions.SetAll:
                    break;
            }

        }

        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
        {
            ReadMessageServer(packet, fromConnection);
        }

        /// For sending from the server
        private void SendServerResponse(BoltConnection connection)
        {

        }

        /// For sending from the server
        private void SendServerResponseToAll(UniqueIdListType forPrefab, UniqueIdListOptions toDo, params string[] ids)  // Only Host can send this
        {
            if (!BoltNetwork.isServer) { return; }
            var packet = NewPacket(128, Bolt.GlobalTargets.Everyone);
            packet.Packet.WriteByte((byte)toDo);  // Add, Remove, Clear, Set
            packet.Packet.WriteByte((byte)forPrefab);  // Reciver, TransmitterSwitch, TransmitterDetector, All
            string steamId = Misc.GetMySteamId();
            if (string.IsNullOrEmpty(steamId) || steamId == "0") { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] SteamId is null or empty", true); return; }
            packet.Packet.WriteString(steamId);  // This is the sender steam id
            switch (toDo)
            {
                case UniqueIdListOptions.Add:
                    switch (forPrefab)
                    {
                        case UniqueIdListType.Reciver or UniqueIdListType.TransmitterSwitch or UniqueIdListType.TransmitterDetector:
                            // Sequence: Id, BoltEntity
                            if (ids.Length == 0) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] No Id To Sync", true); return; }
                            if (ids.Length > 1) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] More than one Id To Sync - Not Good", true); return; }
                            if (string.IsNullOrEmpty(ids[0])) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] Id is null or empty", true); return; }
                            packet.Packet.WriteString(ids[0]);
                            BoltEntity boltEntity = null;
                            switch (forPrefab)
                            {
                                case UniqueIdListType.Reciver:
                                    boltEntity = WirelessSignals.reciver.FindBoltEntityByUniqueId(ids[0]);
                                    break;
                                case UniqueIdListType.TransmitterSwitch:
                                    boltEntity = WirelessSignals.transmitterSwitch.FindBoltEntityByUniqueId(ids[0]);
                                    break;
                                case UniqueIdListType.TransmitterDetector:
                                    boltEntity = WirelessSignals.transmitterDetector.FindBoltEntityByUniqueId(ids[0]);
                                    break;
                            }
                            if (boltEntity == null) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] BoltEntity is null", true); return; }
                            packet.Packet.WriteBoltEntity(boltEntity);
                            break;
                        case UniqueIdListType.All:
                            RLog.Warning("[UniqueIdSync] [SendServerResponseToAll] All Case for Add Is Invalid!", true);
                            return;
                    }
                    break;
                case UniqueIdListOptions.Remove:
                    switch (forPrefab)
                    {
                        case UniqueIdListType.Reciver or UniqueIdListType.TransmitterSwitch or UniqueIdListType.TransmitterDetector:
                            // Sequence: Id
                            if (ids.Length == 0) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] No Id To Sync", true); return; }
                            if (ids.Length > 1) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] More than one Id To Sync - Not Good", true); return; }
                            if (string.IsNullOrEmpty(ids[0])) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] Id is null or empty", true); return; }
                            packet.Packet.WriteString(ids[0]);
                            break;
                        case UniqueIdListType.All:
                            RLog.Warning("[UniqueIdSync] [SendServerResponseToAll] All Case for Add Is Invalid!", true);
                            return;
                    }
                    break;
                case UniqueIdListOptions.Clear:
                    packet.Packet.WriteString("Clear");
                    break;
                case UniqueIdListOptions.SetAll:
                    switch (forPrefab)
                    {
                        case UniqueIdListType.Reciver:
                            Dictionary<string, GameObject> spawnedRecivers = WirelessSignals.reciver.spawnedGameObjects;
                            WirelessSignals.reciver.FindBoltEntityByUniqueId("string from dict (key)").networkId.ToString();
                            List<string> keys = new List<string>(spawnedRecivers.Keys);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (BinaryWriter writer = new BinaryWriter(ms))
                                {
                                    // Write the count first
                                    writer.Write(keys.Count);

                                    // Write each string
                                    foreach (string key in keys)
                                    {
                                        writer.Write(key);
                                    }
                                }

                                // Write the entire byte array to the packet
                                packet.Packet.WriteByteArray(ms.ToArray());
                            }
                            break;
                        case UniqueIdListType.TransmitterSwitch:
                            Dictionary<string, GameObject> spawnedDetectors = WirelessSignals.transmitterDetector.spawnedGameObjects;
                            List<string> keysDetector = new List<string>(spawnedDetectors.Keys);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (BinaryWriter writer = new BinaryWriter(ms))
                                {
                                    // Write the count first
                                    writer.Write(keysDetector.Count);

                                    // Write each string
                                    foreach (string key in keysDetector)
                                    {
                                        writer.Write(key);
                                    }
                                }

                                // Write the entire byte array to the packet
                                packet.Packet.WriteByteArray(ms.ToArray());
                            }
                            break;
                        case UniqueIdListType.TransmitterDetector:
                            Dictionary<string, GameObject> spawnedSwitches = WirelessSignals.transmitterSwitch.spawnedGameObjects;
                            List<string> keysSwitch = new List<string>(spawnedSwitches.Keys);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (BinaryWriter writer = new BinaryWriter(ms))
                                {
                                    // Write the count first
                                    writer.Write(keysSwitch.Count);

                                    // Write each string
                                    foreach (string key in keysSwitch)
                                    {
                                        writer.Write(key);
                                    }
                                }

                                // Write the entire byte array to the packet
                                packet.Packet.WriteByteArray(ms.ToArray());
                            }
                            break;
                        case UniqueIdListType.All:
                            Dictionary<string, GameObject> spawnedReciversAll = WirelessSignals.reciver.spawnedGameObjects;
                            Dictionary<string, GameObject> spawnedDetectorsAll = WirelessSignals.transmitterDetector.spawnedGameObjects;
                            Dictionary<string, GameObject> spawnedSwitchesAll = WirelessSignals.transmitterSwitch.spawnedGameObjects;

                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (BinaryWriter writer = new BinaryWriter(ms))
                                {
                                    // Write receivers count and data
                                    writer.Write(spawnedReciversAll.Count);
                                    foreach (string key in spawnedReciversAll.Keys)
                                    {
                                        writer.Write((byte)UniqueIdListType.Reciver);
                                        writer.Write(key);
                                        string networkId = WirelessSignals.reciver.FindBoltEntityByUniqueId(key).networkId.ToString();
                                        if (Tools.BoltIdTool.IsInputStringValid(networkId) == false) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] NetworkId is not valid", true); packet.Packet.Dispose(); return; }
                                        writer.Write(networkId);
                                    }

                                    // Write detector count and data
                                    writer.Write(spawnedDetectorsAll.Count);
                                    foreach (string key in spawnedDetectorsAll.Keys)
                                    {
                                        writer.Write((byte)UniqueIdListType.TransmitterDetector);
                                        writer.Write(key);
                                        string networkId = WirelessSignals.transmitterDetector.FindBoltEntityByUniqueId(key).networkId.ToString();
                                        if (string.IsNullOrEmpty(networkId) || networkId == "0" || networkId.ToLower() == "NetworkID 00-00-00-00-00-00-00-00") { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] NetworkId is null or empty", true); packet.Packet.Dispose(); return; }
                                        writer.Write(networkId);
                                    }

                                    // Write switch count and data
                                    writer.Write(spawnedSwitchesAll.Count);
                                    foreach (string key in spawnedSwitchesAll.Keys)
                                    {
                                        writer.Write((byte)UniqueIdListType.TransmitterSwitch);
                                        writer.Write(key);
                                        string networkId = WirelessSignals.transmitterSwitch.FindBoltEntityByUniqueId(key).networkId.ToString();
                                        if (string.IsNullOrEmpty(networkId) || networkId == "0" || networkId.ToLower() == "NetworkID 00-00-00-00-00-00-00-00") { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] NetworkId is null or empty", true); packet.Packet.Dispose(); return; }
                                        writer.Write(networkId);
                                    }
                                }

                                // Write the entire byte array to the packet
                                packet.Packet.WriteByteArray(ms.ToArray());
                            }
                            break;
                    }
                    break;
                case UniqueIdListOptions.Set:
                    switch (forPrefab)
                    {
                        case UniqueIdListType.Reciver or UniqueIdListType.TransmitterSwitch or UniqueIdListType.TransmitterDetector:
                            // Sequence: Id, BoltEntity
                            if (ids.Length == 0) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] No Id To Sync", true); return; }
                            if (ids.Length > 1) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] More than one Id To Sync - Not Good", true); return; }
                            if (string.IsNullOrEmpty(ids[0])) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] Id is null or empty", true); return; }
                            packet.Packet.WriteString(ids[0]);
                            BoltEntity boltEntity = null;
                            switch (forPrefab)
                            {
                                case UniqueIdListType.Reciver:
                                    boltEntity = WirelessSignals.reciver.FindBoltEntityByUniqueId(ids[0]);
                                    break;
                                case UniqueIdListType.TransmitterSwitch:
                                    boltEntity = WirelessSignals.transmitterSwitch.FindBoltEntityByUniqueId(ids[0]);
                                    break;
                                case UniqueIdListType.TransmitterDetector:
                                    boltEntity = WirelessSignals.transmitterDetector.FindBoltEntityByUniqueId(ids[0]);
                                    break;
                            }
                            if (boltEntity == null) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] BoltEntity is null", true); return; }
                            packet.Packet.WriteBoltEntity(boltEntity);
                            break;
                        case UniqueIdListType.All:
                            RLog.Warning("[UniqueIdSync] [SendServerResponseToAll] All Case for Add Is Invalid!", true);
                            return;
                    }
                    break;
            }
            Misc.Msg($"[UniqueIdSync] [SendServerResponseToAll] UniqueIdListType: {forPrefab}, UniqueIdListOptions: {toDo}, Params Length: {ids.Length}", true);
            Send(packet);
        }

        public void SendInfo(BoltConnection connection) => SendServerResponse(connection);

        public void SendUniqueIdEvent(UniqueIdListType forPrefab, UniqueIdListOptions toDo, params string[] ids) => SendServerResponseToAll(forPrefab, toDo, ids);
    }
}
