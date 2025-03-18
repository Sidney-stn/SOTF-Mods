using Bolt;
using RedLoader;
using UdpKit;

namespace StoneGate.Network
{
    internal class HostEvents : EventBase<HostEvents>
    {
        public enum HostEvent : byte
        {
            CreateStoneGate = 0,
            DestroyStoneGate = 1,
            OpenStoneGate = 2,
            CloseStoneGate = 3,
        }

        protected override void ReadMessageClient(UdpPacket packet, BoltConnection fromConnection)
        {
            Misc.Msg("[HostEvents] [ReadMessageClient] Recived Event", true);
            RecivedEvent(packet, fromConnection);
        }

        private void RecivedEvent(UdpPacket packet, BoltConnection fromConnection)
        {
            var eventType = (HostEvent)packet.ReadByte();
            switch (eventType)
            {
                case HostEvent.CreateStoneGate:
                    try
                    {
                        string rotateGoName = packet.ReadString();
                        var mode = (Objects.CreateGateParent.RotateMode)packet.ReadByte();
                        string floorBeamGoName = packet.ReadString();
                        string topBeamGoName = packet.ReadString();
                        string rockWallGoName = packet.ReadString();
                        string extraPillarGoName = packet.ReadString();
                        int childIndex = packet.ReadInt();

                        CreateStoneGate(rotateGoName, mode, floorBeamGoName, topBeamGoName, rockWallGoName, extraPillarGoName, childIndex);
                    }
                    catch (System.Exception e)
                    {
                        Misc.Msg("[HostEvents] [RecivedEvent] Error: " + e.Message, true);
                    }
                    break;
                case HostEvent.DestroyStoneGate:
                    try
                    {
                        int childIndex = packet.ReadInt();
                        DestroyStoneGate(childIndex);
                    }
                    catch (System.Exception e)
                    {
                        Misc.Msg("[HostEvents] [RecivedEvent] Error: " + e.Message, true);
                    }
                    break;
                case HostEvent.OpenStoneGate:
                    try
                    {
                        int childIndex2 = packet.ReadInt();
                        OpenStoneGate(childIndex2);
                    }
                    catch (System.Exception e)
                    {
                        Misc.Msg("[HostEvents] [RecivedEvent] Error: " + e.Message, true);
                    }
                    break;
                case HostEvent.CloseStoneGate:
                    try
                    {
                        int childIndex3 = packet.ReadInt();
                        CloseStoneGate(childIndex3);
                    }
                    catch (System.Exception e)
                    {
                        Misc.Msg("[HostEvents] [RecivedEvent] Error: " + e.Message, true);
                    }
                    break;
            }
        }

        private void CreateStoneGate(string rotateGoName, Objects.CreateGateParent.RotateMode mode, string floorBeamGoName, string topBeamGoName, string rockWallGoName, string extraPillarGoName, int childIndex)
        {
            Objects.CreateGateParent.Instance.AddDoorNetworkClient(rotateGoName, mode, floorBeamGoName, topBeamGoName, rockWallGoName, extraPillarGoName, childIndex);
        }

        private void DestroyStoneGate(int childIndex)
        {
            Objects.CreateGateParent.Instance.RemoveDoorNetworkClient(childIndex);
        }

        private void OpenStoneGate(int childIndex)
        {
            Objects.CreateGateParent.Instance.OpenDoorNetworkClient(childIndex);
        }

        private void CloseStoneGate(int childIndex)
        {
            Objects.CreateGateParent.Instance.CloseDoorNetworkClient(childIndex);
        }

        private bool CheckChildIndex(int? childIndex)
        {
            if (childIndex == null)
            {
                RLog.Error("[StoneGate] [Network] [HostEvents] [CheckChildIndex] ChildIndex is null");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Send a host event to the all clients, can send eveything except CreateStoneGate event
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="childIndex"></param>
        public void SendHostEvent(HostEvent eventType, int? childIndex)
        {
            if (!BoltNetwork.isRunning) { return; }

            var packet = NewPacket(128, GlobalTargets.AllClients);
            packet.Packet.WriteByte((byte)eventType);
            switch (eventType)
            {
                case HostEvent.DestroyStoneGate:
                    if (CheckChildIndex(childIndex) == false) { return; }
                    packet.Packet.WriteInt((int)childIndex);
                    break;
                case HostEvent.OpenStoneGate:
                    if (CheckChildIndex(childIndex) == false) { return; }
                    packet.Packet.WriteInt((int)childIndex);
                    break;
                case HostEvent.CloseStoneGate:
                    if (CheckChildIndex(childIndex) == false) { return; }
                    packet.Packet.WriteInt((int)childIndex);
                    break;
            }
            Send(packet);
        }


        /// <summary>
        /// Send a host event to all clients, can only send CreateStoneGate event
        /// </summary>
        /// <param name="eventType"></param>
        public void SendHostEvent(HostEvent eventType, string rotateGoName, Objects.CreateGateParent.RotateMode mode, string floorBeamGoName, string topBeamGoName, string rockWallGoName, string extraPillarGoName, int childIndex)
        {
            if (!BoltNetwork.isRunning) { return; }

            var packet = NewPacket(512, GlobalTargets.AllClients);
            packet.Packet.WriteByte((byte)eventType);
            switch (eventType)
            {
                case HostEvent.CreateStoneGate:
                    if (string.IsNullOrEmpty(rotateGoName))
                    {
                        Misc.Msg("[HostEvents] [SendHostEvent] RotateGoName is null or empty", true);
                        packet.Packet.Dispose();
                        return;
                    }
                    if (mode == Objects.CreateGateParent.RotateMode.None)
                    {
                        Misc.Msg("[HostEvents] [SendHostEvent] RotateMode is None", true);
                        packet.Packet.Dispose();
                        return;
                    }
                    packet.Packet.WriteString(rotateGoName);
                    packet.Packet.WriteByte((byte)mode);
                    packet.Packet.WriteString(floorBeamGoName);
                    packet.Packet.WriteString(topBeamGoName);
                    packet.Packet.WriteString(rockWallGoName);
                    packet.Packet.WriteString(extraPillarGoName);
                    packet.Packet.WriteInt(childIndex);
                    break;
                default:
                    Misc.Msg("[HostEvents] [SendHostEvent] Event type requires a child index", true);
                    return;
            }
            Misc.Msg("[HostEvents] [SendHostEvent] Sending CreateStoneGate Event", true);
            Send(packet);
        }
    }
}
