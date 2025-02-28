using Bolt;
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

        public enum UniqueIdTo : byte
        {
            All = 0,
            Host = 1,
            Clients = 2,
            BoltConnection = 3,
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
                    switch (forPrefab)
                    {
                        case UniqueIdListType.Reciver:
                            byte[] reciverData = packet.ReadByteArrayWithPrefix();
                            using (MemoryStream ms = new MemoryStream(reciverData))
                            using (BinaryReader reader = new BinaryReader(ms))
                            {
                                int count = reader.ReadInt32();
                                for (int i = 0; i < count; i++)
                                {
                                    string uniqueId = reader.ReadString();
                                    string networkIdStr = reader.ReadString();

                                    if (string.IsNullOrEmpty(uniqueId))
                                    {
                                        Misc.Msg("[UniqueIdSync] [ReadMessageClient] Unique ID is null or empty", true);
                                        continue;
                                    }

                                    if (!Tools.BoltIdTool.IsInputStringValid(networkIdStr))
                                    {
                                        Misc.Msg("[UniqueIdSync] [ReadMessageClient] Network ID is not valid", true);
                                        continue;
                                    }

                                    NetworkId networkId = Tools.BoltIdTool.StringToBoltNetworkId(networkIdStr);
                                    BoltEntity entity = BoltNetwork.FindEntity(networkId);

                                    if (entity == null)
                                    {
                                        Misc.Msg($"[UniqueIdSync] [ReadMessageClient] Could not find entity with NetworkID {networkIdStr}", true);
                                        continue;
                                    }

                                    WirelessSignals.reciver.AddBoltEntityToDictionary(uniqueId, entity);
                                }
                            }
                            break;

                        case UniqueIdListType.TransmitterSwitch:
                            byte[] switchData = packet.ReadByteArrayWithPrefix();
                            using (MemoryStream ms = new MemoryStream(switchData))
                            using (BinaryReader reader = new BinaryReader(ms))
                            {
                                int count = reader.ReadInt32();
                                for (int i = 0; i < count; i++)
                                {
                                    string uniqueId = reader.ReadString();
                                    string networkIdStr = reader.ReadString();

                                    if (string.IsNullOrEmpty(uniqueId))
                                    {
                                        Misc.Msg("[UniqueIdSync] [ReadMessageClient] Unique ID is null or empty", true);
                                        continue;
                                    }
                                    if (Tools.BoltIdTool.IsInputStringValid(networkIdStr) == false)
                                    {
                                        Misc.Msg("[UniqueIdSync] [ReadMessageClient] Network ID is not valid", true);
                                        continue;
                                    }

                                    NetworkId networkId = Tools.BoltIdTool.StringToBoltNetworkId(networkIdStr);
                                    BoltEntity entity = BoltNetwork.FindEntity(networkId);

                                    if (entity == null)
                                    {
                                        Misc.Msg($"[UniqueIdSync] [ReadMessageClient] Could not find entity with NetworkID {networkIdStr}", true);
                                        continue;
                                    }

                                    WirelessSignals.transmitterSwitch.AddBoltEntityToDictionary(uniqueId, entity);
                                }
                            }
                            break;

                        case UniqueIdListType.TransmitterDetector:
                            byte[] detectorData = packet.ReadByteArrayWithPrefix();
                            using (MemoryStream ms = new MemoryStream(detectorData))
                            using (BinaryReader reader = new BinaryReader(ms))
                            {
                                int count = reader.ReadInt32();
                                for (int i = 0; i < count; i++)
                                {
                                    string uniqueId = reader.ReadString();
                                    string networkIdStr = reader.ReadString();

                                    if (string.IsNullOrEmpty(uniqueId))
                                    {
                                        Misc.Msg("[UniqueIdSync] [ReadMessageClient] Unique ID is null or empty", true);
                                        continue;
                                    }

                                    if (Tools.BoltIdTool.IsInputStringValid(networkIdStr) == false)
                                    {
                                        Misc.Msg("[UniqueIdSync] [ReadMessageClient] Network ID is not valid", true);
                                        continue;
                                    }

                                    NetworkId networkId = Tools.BoltIdTool.StringToBoltNetworkId(networkIdStr);
                                    BoltEntity entity = BoltNetwork.FindEntity(networkId);

                                    if (entity == null)
                                    {
                                        Misc.Msg($"[UniqueIdSync] [ReadMessageClient] Could not find entity with NetworkID {networkIdStr}", true);
                                        continue;
                                    }

                                    WirelessSignals.transmitterDetector.AddBoltEntityToDictionary(uniqueId, entity);
                                }
                            }
                            break;

                        case UniqueIdListType.All:
                            byte[] allData = packet.ReadByteArrayWithPrefix();
                            using (MemoryStream ms = new MemoryStream(allData))
                            using (BinaryReader reader = new BinaryReader(ms))
                            {
                                // Read receivers
                                int receiversCount = reader.ReadInt32();
                                for (int i = 0; i < receiversCount; i++)
                                {
                                    byte type = reader.ReadByte();  // Should be UniqueIdListType.Reciver
                                    string uniqueId = reader.ReadString();
                                    string networkIdStr = reader.ReadString();

                                    if (string.IsNullOrEmpty(uniqueId)) continue;
                                    if (!Tools.BoltIdTool.IsInputStringValid(networkIdStr)) continue;

                                    NetworkId networkId = Tools.BoltIdTool.StringToBoltNetworkId(networkIdStr);
                                    BoltEntity entity = BoltNetwork.FindEntity(networkId);

                                    if (entity == null) continue;

                                    WirelessSignals.reciver.AddBoltEntityToDictionary(uniqueId, entity);
                                }

                                // Read detectors
                                int detectorsCount = reader.ReadInt32();
                                for (int i = 0; i < detectorsCount; i++)
                                {
                                    byte type = reader.ReadByte();  // Should be UniqueIdListType.TransmitterDetector
                                    string uniqueId = reader.ReadString();
                                    string networkIdStr = reader.ReadString();

                                    if (string.IsNullOrEmpty(uniqueId)) continue;
                                    if (string.IsNullOrEmpty(networkIdStr) || networkIdStr == "0" ||
                                        networkIdStr.ToLower() == "NetworkID 00-00-00-00-00-00-00-00") continue;

                                    NetworkId networkId = Tools.BoltIdTool.StringToBoltNetworkId(networkIdStr);
                                    BoltEntity entity = BoltNetwork.FindEntity(networkId);

                                    if (entity == null) continue;

                                    WirelessSignals.transmitterDetector.AddBoltEntityToDictionary(uniqueId, entity);
                                }

                                // Read switches
                                int switchesCount = reader.ReadInt32();
                                for (int i = 0; i < switchesCount; i++)
                                {
                                    byte type = reader.ReadByte();  // Should be UniqueIdListType.TransmitterSwitch
                                    string uniqueId = reader.ReadString();
                                    string networkIdStr = reader.ReadString();

                                    if (string.IsNullOrEmpty(uniqueId)) continue;
                                    if (string.IsNullOrEmpty(networkIdStr) || networkIdStr == "0" ||
                                        networkIdStr.ToLower() == "NetworkID 00-00-00-00-00-00-00-00") continue;

                                    NetworkId networkId = Tools.BoltIdTool.StringToBoltNetworkId(networkIdStr);
                                    BoltEntity entity = BoltNetwork.FindEntity(networkId);

                                    if (entity == null) continue;

                                    WirelessSignals.transmitterSwitch.AddBoltEntityToDictionary(uniqueId, entity);
                                }
                            }
                            break;
                    }
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

        /// For sending from the server or client, client is limited
        private void SendUpdateEvent(UniqueIdListType forPrefab, UniqueIdListOptions toDo, UniqueIdTo toPlayer, BoltConnection _ = null, params string[] ids)  // Only Host can send this
        {
            Misc.Msg($"[UniqueIdSync] [SendUpdateEvent] Sending {toDo} {forPrefab} To {toPlayer}");
            bool useConnection = false;
            bool useGlobalTargets = false;
            Bolt.GlobalTargets target = Bolt.GlobalTargets.OnlySelf;
            switch (toPlayer)
            {
                case UniqueIdTo.All:
                    useGlobalTargets = true;
                    target = Bolt.GlobalTargets.Everyone;
                    break;
                case UniqueIdTo.Host:
                    useGlobalTargets = true;
                    target = Bolt.GlobalTargets.OnlyServer;
                    break;
                case UniqueIdTo.Clients:
                    useGlobalTargets = true;
                    target = Bolt.GlobalTargets.AllClients;
                    break;
                case UniqueIdTo.BoltConnection:
                    useConnection = true;
                    break;
            }
            var packet = NewPacket(128, _);
            if (useGlobalTargets && !useConnection)
            {
                packet = NewPacket(128, target);
            }
            else if (useConnection && !useGlobalTargets)
            {
                // Do Nothing
            }
            else
            {
                RLog.Warning("[UniqueIdSync] [SendUpdateEvent] Invalid UniqueIdTo");
                return;
            }

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
                    if (!BoltNetwork.isServer) { return; }
                    switch (forPrefab)
                    {
                        case UniqueIdListType.Reciver:
                            Dictionary<string, GameObject> spawnedRecivers = WirelessSignals.reciver.spawnedGameObjects;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (BinaryWriter writer = new BinaryWriter(ms))
                                {
                                    // Write the count first
                                    writer.Write(spawnedRecivers.Count);

                                    // Write each string and its networkId
                                    foreach (string key in spawnedRecivers.Keys)
                                    {
                                        writer.Write(key);
                                        string networkId = WirelessSignals.reciver.FindBoltEntityByUniqueId(key).networkId.ToString();
                                        if (Tools.BoltIdTool.IsInputStringValid(networkId) == false)
                                        {
                                            Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] NetworkId is not valid", true);
                                            packet.Packet.Dispose();
                                            return;
                                        }
                                        writer.Write(networkId);
                                    }
                                }

                                // Write the entire byte array to the packet
                                packet.Packet.WriteByteArrayLengthPrefixed(ms.ToArray(), (int)ms.Length + 10);  // 10 For Safety
                            }
                            break;

                        case UniqueIdListType.TransmitterSwitch:
                            Dictionary<string, GameObject> spawnedSwitches = WirelessSignals.transmitterSwitch.spawnedGameObjects;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (BinaryWriter writer = new BinaryWriter(ms))
                                {
                                    // Write the count first
                                    writer.Write(spawnedSwitches.Count);

                                    // Write each string and its networkId
                                    foreach (string key in spawnedSwitches.Keys)
                                    {
                                        writer.Write(key);
                                        string networkId = WirelessSignals.transmitterSwitch.FindBoltEntityByUniqueId(key).networkId.ToString();
                                        if (Tools.BoltIdTool.IsInputStringValid(networkId) == false)
                                        {
                                            Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] NetworkId is not valid", true);
                                            packet.Packet.Dispose();
                                            return;
                                        }
                                        writer.Write(networkId);
                                    }
                                }

                                // Write the entire byte array to the packet
                                packet.Packet.WriteByteArrayLengthPrefixed(ms.ToArray(), (int)ms.Length + 10);  // 10 For Safety
                            }
                            break;

                        case UniqueIdListType.TransmitterDetector:
                            Dictionary<string, GameObject> spawnedDetectors = WirelessSignals.transmitterDetector.spawnedGameObjects;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (BinaryWriter writer = new BinaryWriter(ms))
                                {
                                    // Write the count first
                                    writer.Write(spawnedDetectors.Count);

                                    // Write each string and its networkId
                                    foreach (string key in spawnedDetectors.Keys)
                                    {
                                        writer.Write(key);
                                        string networkId = WirelessSignals.transmitterDetector.FindBoltEntityByUniqueId(key).networkId.ToString();
                                        if (Tools.BoltIdTool.IsInputStringValid(networkId) == false)
                                        {
                                            Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] NetworkId is not valid", true);
                                            packet.Packet.Dispose();
                                            return;
                                        }
                                        writer.Write(networkId);
                                    }
                                }

                                // Write the entire byte array to the packet
                                packet.Packet.WriteByteArrayLengthPrefixed(ms.ToArray(), (int)ms.Length + 10);  // 10 For Safety                            }
                                break;
                            }
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
                                        if (Tools.BoltIdTool.IsInputStringValid(networkId) == false) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] NetworkId is not valid", true); packet.Packet.Dispose(); return; }
                                        writer.Write(networkId);
                                    }

                                    // Write switch count and data
                                    writer.Write(spawnedSwitchesAll.Count);
                                    foreach (string key in spawnedSwitchesAll.Keys)
                                    {
                                        writer.Write((byte)UniqueIdListType.TransmitterSwitch);
                                        writer.Write(key);
                                        string networkId = WirelessSignals.transmitterSwitch.FindBoltEntityByUniqueId(key).networkId.ToString();
                                        if (Tools.BoltIdTool.IsInputStringValid(networkId) == false) { Misc.Msg("[UniqueIdSync] [SendServerResponseToAll] NetworkId is not valid", true); packet.Packet.Dispose(); return; }
                                        writer.Write(networkId);
                                    }
                                }

                                // Write the entire byte array to the packet
                                packet.Packet.WriteByteArrayLengthPrefixed(ms.ToArray(), (int)ms.Length + 10);  // 10 For Safety
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
            
            Send(packet);
            Misc.Msg($"[UniqueIdSync] [SendUpdateEvent] Sent {toDo} {forPrefab} To {toPlayer}");
        }

        public void SendInfo(BoltConnection connection) => SendServerResponse(connection);

        public void SendUniqueIdEvent(UniqueIdListType forPrefab, UniqueIdListOptions toDo, UniqueIdTo toPlayer, BoltConnection conn = null, params string[] ids)
        {
            // Check If BoltNetwork Is Running
            if (!BoltNetwork.isRunning) { return; }
            SendUpdateEvent(forPrefab, toDo, toPlayer, conn, ids);
        }
    }
}
