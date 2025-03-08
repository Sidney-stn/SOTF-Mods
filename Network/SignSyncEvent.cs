
using Bolt;
using RedLoader;

namespace Signs.Network
{
    internal class SignSyncEvent : RelayEventBase<SignSyncEvent, SignSetter>
    {
        public enum SignSyncType : byte
        {
            SetTextAll = 0,
            SetTextLine1 = 1,
            SetTextLine2 = 2,
            SetTextLine3 = 3,
            SetTextLine4 = 4,
            Destroy = 5
        }

        private void UpdateStateInternal(BoltEntity entity, SignSyncType type, string toSteamId = null)
        {
            Misc.Msg($"[SignSyncEvent] [UpdateStateInternal] Sending {type} to {entity}", true);
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
                case SignSyncType.SetTextAll:
                    var signContoller = entity.gameObject.GetComponent<Mono.SignController>();
                    if (signContoller == null)
                    {
                        RLog.Warning($"[SignSyncEvent] [UpdateStateInternal] SignController is null");
                        packet.Packet.Dispose();
                        return;
                    }
                    string line1 = signContoller.GetLineText(1);
                    string line2 = signContoller.GetLineText(2);
                    string line3 = signContoller.GetLineText(3);
                    string line4 = signContoller.GetLineText(4);

                    if (string.IsNullOrEmpty(line1)) { line1 = "NONE"; }
                    if (string.IsNullOrEmpty(line2)) { line2 = "NONE"; }
                    if (string.IsNullOrEmpty(line3)) { line3 = "NONE"; }
                    if (string.IsNullOrEmpty(line4)) { line4 = "NONE"; }
                    packet.Packet.WriteString(line1);
                    packet.Packet.WriteString(line2);
                    packet.Packet.WriteString(line3);
                    packet.Packet.WriteString(line4);
                    break;
                case SignSyncType.SetTextLine1:
                    var signContoller1 = entity.gameObject.GetComponent<Mono.SignController>();
                    if (signContoller1 == null)
                    {
                        RLog.Warning($"[SignSyncEvent] [UpdateStateInternal] SignController is null");
                        packet.Packet.Dispose();
                        return;
                    }
                    string line1_1 = signContoller1.GetLineText(1);
                    if (string.IsNullOrEmpty(line1_1)) { line1_1 = "NONE"; }
                    packet.Packet.WriteString(line1_1);
                    break;
                case SignSyncType.SetTextLine2:
                    var signContoller2 = entity.gameObject.GetComponent<Mono.SignController>();
                    if (signContoller2 == null)
                    {
                        RLog.Warning($"[SignSyncEvent] [UpdateStateInternal] SignController is null");
                        packet.Packet.Dispose();
                        return;
                    }
                    string line2_2 = signContoller2.GetLineText(2);
                    if (string.IsNullOrEmpty(line2_2)) { line2_2 = "NONE"; }
                    packet.Packet.WriteString(line2_2);
                    break;
                case SignSyncType.SetTextLine3:
                    var signContoller3 = entity.gameObject.GetComponent<Mono.SignController>();
                    if (signContoller3 == null)
                    {
                        RLog.Warning($"[SignSyncEvent] [UpdateStateInternal] SignController is null");
                        packet.Packet.Dispose();
                        return;
                    }
                    string line3_3 = signContoller3.GetLineText(3);
                    if (string.IsNullOrEmpty(line3_3)) { line3_3 = "NONE"; }
                    packet.Packet.WriteString(line3_3);
                    break;
                case SignSyncType.SetTextLine4:
                    var signContoller4 = entity.gameObject.GetComponent<Mono.SignController>();
                    if (signContoller4 == null)
                    {
                        RLog.Warning($"[SignSyncEvent] [UpdateStateInternal] SignController is null");
                        packet.Packet.Dispose();
                        return;
                    }
                    string line4_4 = signContoller4.GetLineText(4);
                    if (string.IsNullOrEmpty(line4_4)) { line4_4 = "NONE"; }
                    packet.Packet.WriteString(line4_4);
                    break;
                case SignSyncType.Destroy:
                    packet.Packet.WriteString("DESTROY");
                    break;
            }

            Send(packet);
        }

        public static void SendState(BoltEntity entity, SignSyncType type)
        {
            Instance.UpdateStateInternal(entity, type);
        }

        public static void SendState(BoltEntity entity, SignSyncType type, string toSteamId)
        {
            Instance.UpdateStateInternal(entity, type, toSteamId);
        }

        public override string Id => "Signs_SignSyncEvent";
    }
}
