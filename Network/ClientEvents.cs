using Bolt;
using RedLoader;
using UdpKit;

namespace StoneGate.Network
{
    internal class ClientEvents : EventBase<ClientEvents>
    {
        public enum ClientEvent : byte
        {
            CreateStoneGate = 0,
            DestroyStoneGate = 1,
            OpenStoneGate = 2,
            CloseStoneGate = 3,
        }

        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
        {
            Misc.Msg("[ClientEvents] [ReadMessageServer] Recived Event", true);
            RecivedEvent(packet, fromConnection);
        }

        private void RecivedEvent(UdpPacket packet, BoltConnection fromConnection)
        {
            var eventType = (ClientEvent)packet.ReadByte();
            switch (eventType)
            {
                case ClientEvent.CreateStoneGate:
                    try
                    {
                        string rotateGoName = packet.ReadString();
                        var mode = (Objects.CreateGateParent.RotateMode)packet.ReadByte();
                        string floorBeamGoName = packet.ReadString();
                        string topBeamGoName = packet.ReadString();
                        string rockWallGoName = packet.ReadString();
                        string extraPillarGoName = packet.ReadString();

                        CreateStoneGate(rotateGoName, mode, floorBeamGoName, topBeamGoName, rockWallGoName, extraPillarGoName);
                    }
                    catch (System.Exception e)
                    {
                        Misc.Msg("[ClientEvents] [RecivedEvent] Error: " + e.Message, true);
                    }
                    break;
                case ClientEvent.DestroyStoneGate:
                    try
                    {
                        int childIndex = packet.ReadInt();
                        DestroyStoneGate(childIndex);
                    }
                    catch (System.Exception e)
                    {
                        Misc.Msg("[ClientEvents] [RecivedEvent] Error: " + e.Message, true);
                    }
                    break;
                case ClientEvent.OpenStoneGate:
                    try
                    {
                        int childIndex2 = packet.ReadInt();
                        OpenStoneGate(childIndex2);
                    }
                    catch (System.Exception e)
                    {
                        Misc.Msg("[ClientEvents] [RecivedEvent] Error: " + e.Message, true);
                    }
                    break;
                case ClientEvent.CloseStoneGate:
                    try
                    {
                        int childIndex3 = packet.ReadInt();
                        CloseStoneGate(childIndex3);
                    }
                    catch (System.Exception e)
                    {
                        Misc.Msg("[ClientEvents] [RecivedEvent] Error: " + e.Message, true);
                    }
                    break;
            }
        }

        private void CreateStoneGate(string rotateGoName, Objects.CreateGateParent.RotateMode mode, string floorBeamGoName, string topBeamGoName, string rockWallGoName, string extraPillarGoName)
        {
            Objects.CreateGateParent.Instance.AddDoorNetworkHost(rotateGoName, mode, floorBeamGoName, topBeamGoName, rockWallGoName, extraPillarGoName);
        }

        private void DestroyStoneGate(int childIndex)
        {
            Objects.CreateGateParent.Instance.RemoveDoorNetworkHost(childIndex);
        }

        private void OpenStoneGate(int childIndex)
        {
            Objects.CreateGateParent.Instance.OpenDoorNetworkHost(childIndex);
        }

        private void CloseStoneGate(int childIndex)
        {
            Objects.CreateGateParent.Instance.CloseDoorNetworkHost(childIndex);
        }

        private bool CheckChildIndex(int? childIndex)
        {
            if (childIndex == null)
            {
                RLog.Error("[StoneGate] [Network] [ClientEvents] [CheckChildIndex] ChildIndex is null");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Send a client event to the server, can send eveything except CreateStoneGate event
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="childIndex"></param>
        public void SendClientEvent(ClientEvent eventType, int? childIndex)
        {
            if (!BoltNetwork.isRunning) { return; }

            var packet = NewPacket(128, GlobalTargets.OnlyServer);
            packet.Packet.WriteByte((byte)eventType);
            switch (eventType)
            {
                case ClientEvent.DestroyStoneGate:
                    if (CheckChildIndex(childIndex) == false) { return; }
                    packet.Packet.WriteInt((int)childIndex);
                    break;
                case ClientEvent.OpenStoneGate:
                    if (CheckChildIndex(childIndex) == false) { return; }
                    packet.Packet.WriteInt((int)childIndex);
                    break;
                case ClientEvent.CloseStoneGate:
                    if (CheckChildIndex(childIndex) == false) { return; }
                    packet.Packet.WriteInt((int)childIndex);
                    break;
            }
            Send(packet);
        }


        /// <summary>
        /// Send a client event to the server, can only send CreateStoneGate event
        /// </summary>
        /// <param name="eventType"></param>
        public void SendClientEvent(ClientEvent eventType, string rotateGoName, Objects.CreateGateParent.RotateMode mode, string floorBeamGoName, string topBeamGoName, string rockWallGoName, string extraPillarGoName)
        {
            if (!BoltNetwork.isRunning) { return; }

            var packet = NewPacket(512, GlobalTargets.OnlyServer);
            packet.Packet.WriteByte((byte)eventType);
            switch (eventType)
            {
                case ClientEvent.CreateStoneGate:
                    if (string.IsNullOrEmpty(rotateGoName))
                    {
                        Misc.Msg("[ClientEvents] [SendClientEvent] RotateGoName is null or empty", true);
                        packet.Packet.Dispose();
                        return;
                    }
                    if (mode == Objects.CreateGateParent.RotateMode.None)
                    {
                        Misc.Msg("[ClientEvents] [SendClientEvent] RotateMode is None", true);
                        packet.Packet.Dispose();
                        return;
                    }
                    packet.Packet.WriteString(rotateGoName);
                    packet.Packet.WriteByte((byte)mode);
                    packet.Packet.WriteString(floorBeamGoName);
                    packet.Packet.WriteString(topBeamGoName);
                    packet.Packet.WriteString(rockWallGoName);
                    packet.Packet.WriteString(extraPillarGoName);
                    break;
                default:
                    Misc.Msg("[ClientEvents] [SendClientEvent] Event type requires a child index", true);
                    return;
            }
            Misc.Msg("[ClientEvents] [SendClientEvent] Sending CreateStoneGate Event", true);
            Send(packet);
        }
    }
}
