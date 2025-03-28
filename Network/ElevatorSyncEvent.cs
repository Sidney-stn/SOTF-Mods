
using Bolt;
using RedLoader;

namespace SimpleElevator.Network
{
    internal class ElevatorSyncEvent : RelayEventBase<ElevatorSyncEvent, ElevatorSetter>
    {
        public enum ElevatorSyncType : byte
        {

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
                default:
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
