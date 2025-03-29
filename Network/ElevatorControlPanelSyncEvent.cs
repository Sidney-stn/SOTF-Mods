
using Bolt;
using RedLoader;

namespace SimpleElevator.Network
{
    internal class ElevatorControlPanelSyncEvent : RelayEventBase<ElevatorControlPanelSyncEvent, ElevatorControlPanelSetter>
    {
        public enum ElevatorControlPanelSyncType : byte
        {
            CallElevator = 0,
            Destroy = 1,
        }

        private void UpdateStateInternal(BoltEntity entity, ElevatorControlPanelSyncType type, string toSteamId = null)
        {
            Misc.Msg($"[ElevatorControlPanelSyncEvent] [UpdateStateInternal] Sending {type} to {entity}", true);
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
                case ElevatorControlPanelSyncType.CallElevator:
                    // Add any additional data needed for CallElevator
                    packet.Packet.WriteString("CALL");
                    break;
                case ElevatorControlPanelSyncType.Destroy:
                    // Add any additional data needed for Destroy
                    packet.Packet.WriteString("DESTROY");
                    break;
            }

            Send(packet);
        }

        public static void SendState(BoltEntity entity, ElevatorControlPanelSyncType type)
        {
            Instance.UpdateStateInternal(entity, type);
        }

        public static void SendState(BoltEntity entity, ElevatorControlPanelSyncType type, string toSteamId)
        {
            Instance.UpdateStateInternal(entity, type, toSteamId);
        }

        public override string Id => "SimpleElevator_ElevatorControlPanelSyncEvent";
    }
}
