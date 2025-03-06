using Bolt;
using RedLoader;
using SonsSdk.Networking;
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
            Misc.Msg("[UniqueIdSync] [ReadMessageClient] Recived Event", true);
            RecivedEvent(packet, _, isServer: false);
        }

        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
        {
            Misc.Msg("[UniqueIdSync] [ReadMessageServer] Recived Event", true);
            RecivedEvent(packet, fromConnection, isServer: true);
        }

        /// For sending from the server
        private void SendServerResponse(BoltConnection connection)
        {

        }

        private void RecivedEvent(UdpPacket packet, BoltConnection fromConnection, bool isServer = false)
        {
            var toDo = (UniqueIdListOptions)packet.ReadByte();
            var forPrefab = (UniqueIdListType)packet.ReadByte();
            string steamId = packet.ReadString();
            if (string.IsNullOrEmpty(steamId) || steamId == "0") { Misc.Msg("[UniqueIdSync] [ReadMessageClient] SteamId is null or empty", true); return; }
            if (steamId == Misc.GetMySteamId()) { Misc.Msg("[UniqueIdSync] [ReadMessageClient] Skipped Recieving Own Event", true); return; }
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
                            try
                            {
                                byte[] allData = packet.ReadByteArrayWithPrefix();
                                if (allData == null || allData.Length == 0)
                                {
                                    Misc.Msg("[UniqueIdSync] [ReadMessageClient] Received empty or null allData", true);
                                    return;
                                }

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
                                        if (!Tools.BoltIdTool.IsInputStringValid(uniqueId)) continue;

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
                            }
                            catch (System.Exception ex)
                            {
                                Misc.Msg($"[UniqueIdSync] [ReadMessageClient] Error processing All data: {ex.Message}", true);
                            }
                            break;
                    }
                    break;
            }
        }

        /// For sending from the server or client, client is limited
        private void SendUpdateEvent(UniqueIdListType forPrefab, UniqueIdListOptions toDo, UniqueIdTo toPlayer, BoltConnection connection = null, params string[] ids)  // Only Host can send this
        {
            Misc.Msg($"[UniqueIdSync] [SendUpdateEvent] Sending {toDo} {forPrefab} To {toPlayer}", true);

            // Determine packet target
            Packets.EventPacket packet;
            // Use a much larger buffer size for SetAll operations
            int bufferSize = (toDo == UniqueIdListOptions.SetAll) ? 16384 : 1024;
            if (toPlayer == UniqueIdTo.BoltConnection && connection != null)
            {
                packet = NewPacket(bufferSize, connection);
            }
            else if (toPlayer == UniqueIdTo.All)
            {
                packet = NewPacket(bufferSize, GlobalTargets.Everyone);
            }
            else if (toPlayer == UniqueIdTo.Host)
            {
                packet = NewPacket(bufferSize, GlobalTargets.OnlyServer);
            }
            else if (toPlayer == UniqueIdTo.Clients)
            {
                packet = NewPacket(bufferSize, GlobalTargets.AllClients);
            }
            else
            {
                RLog.Warning("[UniqueIdSync] [SendUpdateEvent] Invalid UniqueIdTo or missing connection");
                return;
            }

            // Write header information
            packet.Packet.WriteByte((byte)toDo);  // Add, Remove, Clear, Set
            packet.Packet.WriteByte((byte)forPrefab);  // Reciver, TransmitterSwitch, TransmitterDetector, All
            string steamId = Misc.GetMySteamId();
            if (string.IsNullOrEmpty(steamId) || steamId == "0")
            {
                Misc.Msg("[UniqueIdSync] [SendUpdateEvent] SteamId is null or empty", true);
                packet.Packet.Dispose();
                return;
            }
            packet.Packet.WriteString(steamId);  // This is the sender steam id

            try
            {
                switch (toDo)
                {
                    case UniqueIdListOptions.Add:
                        HandleAddOperation(forPrefab, ids, packet);
                        break;
                    case UniqueIdListOptions.Remove:
                        HandleRemoveOperation(forPrefab, ids, packet);
                        break;
                    case UniqueIdListOptions.Clear:
                        packet.Packet.WriteString("Clear");
                        break;
                    case UniqueIdListOptions.SetAll:
                        if (!BoltNetwork.isServer)
                        {
                            packet.Packet.Dispose();
                            return;
                        }
                        HandleSetAllOperation(forPrefab, packet);
                        break;
                    case UniqueIdListOptions.Set:
                        HandleSetOperation(forPrefab, ids, packet);
                        break;
                }

                Send(packet);
                Misc.Msg($"[UniqueIdSync] [SendUpdateEvent] Sent {toDo} {forPrefab} To {toPlayer}", true);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[UniqueIdSync] [SendUpdateEvent] Error: {ex.Message}", true);
                packet.Packet.Dispose();
            }
        }

        private void HandleAddOperation(UniqueIdListType forPrefab, string[] ids, Packets.EventPacket packet)
        {
            if (forPrefab == UniqueIdListType.All)
            {
                RLog.Warning("[UniqueIdSync] [HandleAddOperation] All Case for Add Is Invalid!");
                packet.Packet.Dispose();
                return;
            }

            if (ids == null || ids.Length == 0)
            {
                Misc.Msg("[UniqueIdSync] [HandleAddOperation] No Id To Sync", true);
                packet.Packet.Dispose();
                return;
            }

            if (ids.Length > 1)
            {
                Misc.Msg("[UniqueIdSync] [HandleAddOperation] More than one Id To Sync - Not Good", true);
                packet.Packet.Dispose();
                return;
            }

            if (string.IsNullOrEmpty(ids[0]))
            {
                Misc.Msg("[UniqueIdSync] [HandleAddOperation] Id is null or empty", true);
                packet.Packet.Dispose();
                return;
            }

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

            if (boltEntity == null)
            {
                Misc.Msg("[UniqueIdSync] [HandleAddOperation] BoltEntity is null", true);
                packet.Packet.Dispose();
                return;
            }

            packet.Packet.WriteBoltEntity(boltEntity);
        }

        private void HandleRemoveOperation(UniqueIdListType forPrefab, string[] ids, Packets.EventPacket packet)
        {
            if (forPrefab == UniqueIdListType.All)
            {
                RLog.Warning("[UniqueIdSync] [HandleRemoveOperation] All Case for Remove Is Invalid!");
                packet.Packet.Dispose();
                return;
            }

            if (ids == null || ids.Length == 0)
            {
                Misc.Msg("[UniqueIdSync] [HandleRemoveOperation] No Id To Sync", true);
                packet.Packet.Dispose();
                return;
            }

            if (ids.Length > 1)
            {
                Misc.Msg("[UniqueIdSync] [HandleRemoveOperation] More than one Id To Sync - Not Good", true);
                packet.Packet.Dispose();
                return;
            }

            if (string.IsNullOrEmpty(ids[0]))
            {
                Misc.Msg("[UniqueIdSync] [HandleRemoveOperation] Id is null or empty", true);
                packet.Packet.Dispose();
                return;
            }

            packet.Packet.WriteString(ids[0]);
        }

        private void HandleSetOperation(UniqueIdListType forPrefab, string[] ids, Packets.EventPacket packet)
        {
            if (forPrefab == UniqueIdListType.All)
            {
                RLog.Warning("[UniqueIdSync] [HandleSetOperation] All Case for Set Is Invalid!");
                packet.Packet.Dispose();
                return;
            }

            if (ids == null || ids.Length == 0)
            {
                Misc.Msg("[UniqueIdSync] [HandleSetOperation] No Id To Sync", true);
                packet.Packet.Dispose();
                return;
            }

            if (ids.Length > 1)
            {
                Misc.Msg("[UniqueIdSync] [HandleSetOperation] More than one Id To Sync - Not Good", true);
                packet.Packet.Dispose();
                return;
            }

            if (string.IsNullOrEmpty(ids[0]))
            {
                Misc.Msg("[UniqueIdSync] [HandleSetOperation] Id is null or empty", true);
                packet.Packet.Dispose();
                return;
            }

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

            if (boltEntity == null)
            {
                Misc.Msg("[UniqueIdSync] [HandleSetOperation] BoltEntity is null", true);
                packet.Packet.Dispose();
                return;
            }

            packet.Packet.WriteBoltEntity(boltEntity);
        }

        private void HandleSetAllOperation(UniqueIdListType forPrefab, Packets.EventPacket packet)
        {
            try
            {
                switch (forPrefab)
                {
                    case UniqueIdListType.Reciver:
                        SerializeRecivers(packet);
                        break;
                    case UniqueIdListType.TransmitterSwitch:
                        SerializeSwitches(packet);
                        break;
                    case UniqueIdListType.TransmitterDetector:
                        SerializeDetectors(packet);
                        break;
                    case UniqueIdListType.All:
                        SerializeAll(packet);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[UniqueIdSync] [HandleSetAllOperation] Error: {ex.Message}", true);
                packet.Packet.Dispose();
                throw;
            }
        }

        private void SerializeRecivers(Packets.EventPacket packet)
        {
            var spawnedRecivers = WirelessSignals.reciver.spawnedGameObjects;
            if (spawnedRecivers == null)
            {
                Misc.Msg("[UniqueIdSync] [SerializeRecivers] spawnedRecivers is null", true);
                packet.Packet.WriteByteArrayLengthPrefixed(new byte[0], 10);
                return;
            }

            // Create memory stream
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);

            try
            {
                writer.Write(spawnedRecivers.Count);

                foreach (var kvp in spawnedRecivers)
                {
                    string key = kvp.Key;
                    GameObject go = kvp.Value;

                    if (string.IsNullOrEmpty(key) || go == null)
                    {
                        continue;
                    }

                    BoltEntity entity = go.GetComponent<BoltEntity>();
                    if (entity == null)
                    {
                        continue;
                    }

                    string networkId = entity.networkId.ToString();
                    if (!Tools.BoltIdTool.IsInputStringValid(networkId))
                    {
                        continue;
                    }

                    writer.Write(key);
                    writer.Write(networkId);
                }

                // Get the byte array before closing the stream
                byte[] dataBytes = ms.ToArray();

                // Write to packet
                packet.Packet.WriteByteArrayLengthPrefixed(dataBytes, dataBytes.Length + 10);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[UniqueIdSync] [SerializeRecivers] Error: {ex.Message}", true);
                // Write empty array on error
                packet.Packet.WriteByteArrayLengthPrefixed(new byte[0], 10);
            }
            finally
            {
                // Clean up resources
                writer.Dispose();
                ms.Dispose();
            }
        }

        private void SerializeSwitches(Packets.EventPacket packet)
        {
            var spawnedSwitches = WirelessSignals.transmitterSwitch.spawnedGameObjects;
            if (spawnedSwitches == null)
            {
                Misc.Msg("[UniqueIdSync] [SerializeSwitches] spawnedSwitches is null", true);
                packet.Packet.WriteByteArrayLengthPrefixed(new byte[0], 10);
                return;
            }

            // Create memory stream
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);

            try
            {
                writer.Write(spawnedSwitches.Count);

                foreach (var kvp in spawnedSwitches)
                {
                    string key = kvp.Key;
                    GameObject go = kvp.Value;

                    if (string.IsNullOrEmpty(key) || go == null)
                    {
                        continue;
                    }

                    BoltEntity entity = go.GetComponent<BoltEntity>();
                    if (entity == null)
                    {
                        continue;
                    }

                    string networkId = entity.networkId.ToString();
                    if (!Tools.BoltIdTool.IsInputStringValid(networkId))
                    {
                        continue;
                    }

                    writer.Write(key);
                    writer.Write(networkId);
                }

                // Get the byte array before closing the stream
                byte[] dataBytes = ms.ToArray();

                // Write to packet
                packet.Packet.WriteByteArrayLengthPrefixed(dataBytes, dataBytes.Length + 10);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[UniqueIdSync] [SerializeSwitches] Error: {ex.Message}", true);
                // Write empty array on error
                packet.Packet.WriteByteArrayLengthPrefixed(new byte[0], 10);
            }
            finally
            {
                // Clean up resources
                writer.Dispose();
                ms.Dispose();
            }
        }

        private void SerializeDetectors(Packets.EventPacket packet)
        {
            var spawnedDetectors = WirelessSignals.transmitterDetector.spawnedGameObjects;
            if (spawnedDetectors == null)
            {
                Misc.Msg("[UniqueIdSync] [SerializeDetectors] spawnedDetectors is null", true);
                packet.Packet.WriteByteArrayLengthPrefixed(new byte[0], 10);
                return;
            }

            // Create memory stream
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);

            try
            {
                writer.Write(spawnedDetectors.Count);

                foreach (var kvp in spawnedDetectors)
                {
                    string key = kvp.Key;
                    GameObject go = kvp.Value;

                    if (string.IsNullOrEmpty(key) || go == null)
                    {
                        continue;
                    }

                    BoltEntity entity = go.GetComponent<BoltEntity>();
                    if (entity == null)
                    {
                        continue;
                    }

                    string networkId = entity.networkId.ToString();
                    if (!Tools.BoltIdTool.IsInputStringValid(networkId))
                    {
                        continue;
                    }

                    writer.Write(key);
                    writer.Write(networkId);
                }

                // Get the byte array before closing the stream
                byte[] dataBytes = ms.ToArray();

                // Write to packet
                packet.Packet.WriteByteArrayLengthPrefixed(dataBytes, dataBytes.Length + 10);
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[UniqueIdSync] [SerializeDetectors] Error: {ex.Message}", true);
                // Write empty array on error
                packet.Packet.WriteByteArrayLengthPrefixed(new byte[0], 10);
            }
            finally
            {
                // Clean up resources
                writer.Dispose();
                ms.Dispose();
            }
        }

        private void SerializeAll(Packets.EventPacket packet)
        {
            // Add additional safeguards for null reference checks
            if (WirelessSignals.reciver == null)
            {
                Misc.Msg("[UniqueIdSync] [SerializeAll] WirelessSignals.reciver is null", true);
                packet.Packet.WriteByteArrayLengthPrefixed(new byte[0], 10);
                return;
            }

            if (WirelessSignals.transmitterDetector == null)
            {
                Misc.Msg("[UniqueIdSync] [SerializeAll] WirelessSignals.transmitterDetector is null", true);
                packet.Packet.WriteByteArrayLengthPrefixed(new byte[0], 10);
                return;
            }

            if (WirelessSignals.transmitterSwitch == null)
            {
                Misc.Msg("[UniqueIdSync] [SerializeAll] WirelessSignals.transmitterSwitch is null", true);
                packet.Packet.WriteByteArrayLengthPrefixed(new byte[0], 10);
                return;
            }

            var spawnedReciversAll = WirelessSignals.reciver.spawnedGameObjects;
            var spawnedDetectorsAll = WirelessSignals.transmitterDetector.spawnedGameObjects;
            var spawnedSwitchesAll = WirelessSignals.transmitterSwitch.spawnedGameObjects;

            if (spawnedReciversAll == null || spawnedDetectorsAll == null || spawnedSwitchesAll == null)
            {
                Misc.Msg("[UniqueIdSync] [SerializeAll] One or more collections is null", true);
                packet.Packet.WriteByteArrayLengthPrefixed(new byte[0], 10);
                return;
            }

            // Create memory stream
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);

            try
            {
                // Write receivers
                int validReceivers = CountValidEntities(spawnedReciversAll);
                writer.Write(validReceivers);

                foreach (var kvp in spawnedReciversAll)
                {
                    WriteEntityIfValid(writer, kvp.Key, kvp.Value, (byte)UniqueIdListType.Reciver);
                }

                // Write detectors
                int validDetectors = CountValidEntities(spawnedDetectorsAll);
                writer.Write(validDetectors);

                foreach (var kvp in spawnedDetectorsAll)
                {
                    WriteEntityIfValid(writer, kvp.Key, kvp.Value, (byte)UniqueIdListType.TransmitterDetector);
                }

                // Write switches
                int validSwitches = CountValidEntities(spawnedSwitchesAll);
                writer.Write(validSwitches);

                foreach (var kvp in spawnedSwitchesAll)
                {
                    WriteEntityIfValid(writer, kvp.Key, kvp.Value, (byte)UniqueIdListType.TransmitterSwitch);
                }

                // Get the byte array before closing the stream
                byte[] dataBytes = ms.ToArray();

                // Make sure we don't exceed the size limits
                if (dataBytes.Length > 16000)
                {
                    Misc.Msg($"[UniqueIdSync] [SerializeAll] Warning: Data size ({dataBytes.Length} bytes) is very large, truncating", true);
                    byte[] truncated = new byte[16000];
                    Array.Copy(dataBytes, truncated, 16000);
                    dataBytes = truncated;
                }

                // Write to packet
                // Write to packet with safe maxLength
                packet.Packet.WriteByteArrayLengthPrefixed(dataBytes, System.Math.Min(dataBytes.Length + 100, 16300));
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[UniqueIdSync] [SerializeAll] Error: {ex.Message}", true);
                // Write empty array on error
                packet.Packet.WriteByteArrayLengthPrefixed(new byte[0], 10);
            }
            finally
            {
                // Clean up resources
                writer.Dispose();
                ms.Dispose();
            }
        }

        private int CountValidEntities(Dictionary<string, GameObject> collection)
        {
            int count = 0;

            foreach (var kvp in collection)
            {
                string key = kvp.Key;
                GameObject go = kvp.Value;

                if (string.IsNullOrEmpty(key) || go == null)
                {
                    continue;
                }

                BoltEntity entity = go.GetComponent<BoltEntity>();
                if (entity == null)
                {
                    continue;
                }

                string networkId = entity.networkId.ToString();
                if (!Tools.BoltIdTool.IsInputStringValid(networkId))
                {
                    continue;
                }

                count++;
            }

            return count;
        }

        private bool WriteEntityIfValid(BinaryWriter writer, string key, GameObject go, byte type)
        {
            try
            {
                if (string.IsNullOrEmpty(key) || go == null)
                {
                    return false;
                }

                BoltEntity entity = go.GetComponent<BoltEntity>();
                if (entity == null)
                {
                    return false;
                }

                // Guard against potential exceptions with networkId
                NetworkId networkId;
                try
                {
                    networkId = entity.networkId;
                }
                catch (System.Exception ex)
                {
                    Misc.Msg($"[UniqueIdSync] [WriteEntityIfValid] Error getting networkId: {ex.Message}", true);
                    return false;
                }

                string networkIdStr = networkId.ToString();
                if (!Tools.BoltIdTool.IsInputStringValid(networkIdStr))
                {
                    return false;
                }

                writer.Write(type);
                writer.Write(key);
                writer.Write(networkIdStr);
                return true;
            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[UniqueIdSync] [WriteEntityIfValid] Error: {ex.Message}", true);
                return false;
            }
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
