using Bolt;
using RedLoader;


namespace StoneGate.Network
{
    internal class StoneGateSyncEvent : RelayEventBase<StoneGateSyncEvent, StoneGateSetter>
    {
        public enum StoneGateSyncType : byte
        {
            OpenGate = 0,
            CloseGate = 1,
        }

        private void UpdateStateInternal(BoltEntity entity, StoneGateSyncType type, string toSteamId = null)
        {
            Misc.Msg($"[ConveyorSyncEvent] [UpdateStateInternal] Sending {type} to {entity}", true);
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
                case StoneGateSyncType.OpenGate:
                    packet.Packet.WriteBool(true);
                    break;
                case StoneGateSyncType.CloseGate:
                    packet.Packet.WriteBool(false);
                    break;

            }

            Send(packet);
        }

        public static void SendState(BoltEntity entity, StoneGateSyncType type)
        {
            Instance.UpdateStateInternal(entity, type);
        }

        public static void SendState(BoltEntity entity, StoneGateSyncType type, string toSteamId)
        {
            Instance.UpdateStateInternal(entity, type, toSteamId);
        }

        public override string Id => "StoneGate_StoneGateSyncEvent";
    }
}
