
using Bolt;
using RedLoader;

namespace SimpleElevator.Network
{
    internal class ElevatorSyncEvent : RelayEventBase<ElevatorSyncEvent, ElevatorSetter>
    {
        public enum ElevatorSyncType : byte
        {
            MoveUp = 0,
            MoveDown = 1
        }

        private void UpdateStateInternal(BoltEntity entity, ElevatorSyncType type, string toSteamId = null)
        {
            Misc.Msg($"[ElevatorSyncEvent] [UpdateStateInternal] Sending {type} to {entity}", true);
            var packet = NewPacket(entity, 256, GlobalTargets.Everyone);
            packet.Packet.WriteByte((byte)type);
            if (toSteamId != null)
            {
                packet.Packet.WriteString(toSteamId);
            }
            else
            {
                packet.Packet.WriteString("ALL");
            }

            switch (type)
            {
                case ElevatorSyncType.MoveUp:
                    // Add any additional data needed for MoveUp
                    packet.Packet.WriteString("MOVEUP");
                    break;
                case ElevatorSyncType.MoveDown:
                    // Add any additional data needed for MoveDown
                    packet.Packet.WriteString("MOVEDOWN");
                    break;
            }

            Send(packet);
        }

        public static void SendState(BoltEntity entity, ElevatorSyncType type)
        {
            Instance.UpdateStateInternal(entity, type);
        }

        public static void SendState(BoltEntity entity, ElevatorSyncType type, string toSteamId)
        {
            Instance.UpdateStateInternal(entity, type, toSteamId);
        }

        public override string Id => "SimpleElevator_ElevatorSyncEvent";
    }
}
