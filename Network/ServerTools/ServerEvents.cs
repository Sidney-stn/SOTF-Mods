using Bolt;
using RedLoader;
using UdpKit;

namespace SimpleElevator.Network.ServerTools
{
    internal class ServerEvents : EventBase<ServerEvents>
    {
        public enum ServerEvent : byte
        {
            CheckNull = 0,
        }

        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
        {
            Misc.Msg("[ServerEvents] [ReadMessageServer] Recived Event", true);
            RecivedEvent(packet, fromConnection);
        }

        private void CheckNull()
        {
            if (Assets.Instance.Elevator == null)
            {
                RLog.Error("[SimpleElevator] [ServerEvents] [CheckNull] Elevator is null");
            }
            if (Assets.Instance.ElevatorControlPanel == null)
            {
                RLog.Error("[SimpleElevator] [ServerEvents] [CheckNull] ElevatorControlPanel is null");
            }
            if (Assets.Instance.ElevatorBookPage == null)
            {
                RLog.Error("[SimpleElevator] [ServerEvents] [CheckNull] ElevatorBookPage is null");
            }
            if (Assets.Instance.ElevatorControlPanelBookPage == null)
            {
                RLog.Error("[SimpleElevator] [ServerEvents] [CheckNull] ElevatorControlPanelBookPage is null");
            }
            if (Assets.Instance.LinkUiIcon == null)
            {
                RLog.Error("[SimpleElevator] [ServerEvents] [CheckNull] LinkUiIcon is null");
            }
            if (SimpleElevator.Instance.ElevatorInstance == null)
            {
                RLog.Error("[SimpleElevator] [ServerEvents] [CheckNull] ElevatorInstance is null");
            }
            if (SimpleElevator.Instance.ElevatorControlPanelInstace == null)
            {
                RLog.Error("[SimpleElevator] [ServerEvents] [CheckNull] ElevatorControlPanelInstace is null");
            }
        }

        private void RecivedEvent(UdpPacket packet, BoltConnection fromConnection)
        {
            var eventType = (ServerEvent)packet.ReadByte();
            switch (eventType)
            {
                case ServerEvent.CheckNull:
                    try
                    {
                        if (packet.ReadString() == "CHECK_NULL")
                        {
                            Misc.Msg("[ServerEvents] [RecivedEvent] Recived CheckNull", true);
                            CheckNull();
                        }
                    }
                    catch (System.Exception e)
                    {
                        Misc.Msg("[ServerEvents] [RecivedEvent] Error: " + e.Message, true);
                    }
                    break;

            }
        }
        public void SendServerEvent(ServerEvent eventType)
        {
            if (!BoltNetwork.isRunning) { return; }

            var packet = NewPacket(128, GlobalTargets.OnlyServer);
            packet.Packet.WriteByte((byte)eventType);
            switch (eventType)
            {
                case ServerEvent.CheckNull:
                    packet.Packet.WriteString("CHECK_NULL");
                    break;
            }
            Send(packet);
        }
    }
}
