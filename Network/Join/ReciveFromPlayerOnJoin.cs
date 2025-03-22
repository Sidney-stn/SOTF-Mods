using Bolt;
using RedLoader;
using Sons.Multiplayer;
using System.Diagnostics.Tracing;
using UdpKit;

namespace Signs.Network.Join
{
    internal class ReciveFromPlayerOnJoinEvent : EventBase<ReciveFromPlayerOnJoinEvent>
    {
        public enum MessageType : byte
        {
            InfoPayload = 1,
            RequestInfo = 2
        }

        // Read message on the server
        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
        {
            if (!BoltNetwork.isServer)
            {
                Misc.Msg($"[JoinEvent] [ReadMessageServer] Recived¨[ReadMessageServer] message on client, returning", true);
                return;
            }
            var messageType = (MessageType)packet.ReadByte();
            switch (messageType)
            {
                case MessageType.InfoPayload:
                    // read the info the client has sent us
                    var modVersion = packet.ReadString();

                    if (Signs.Instance.Manifest.Version != modVersion)
                    {
                        KickPlayer(fromConnection);
                    }
                        
                    break;
                default:
                    Misc.Msg($"[JoinEvent] [ReadMessageServer] Message type not handled on server!", true);
                    break;
            }
        }

        // Read message on the client
        protected override void ReadMessageClient(UdpPacket packet, BoltConnection _)
        {
            var messageType = (MessageType)packet.ReadByte();
            switch (messageType)
            {
                case MessageType.RequestInfo:
                    Misc.Msg($"[JoinEvent] [ReadMessageClient] Sending response to client", true);
                    SendClientResponse();
                    break;
                default:
                    Misc.Msg($"[JoinEvent] [ReadMessageClient] Message type not handled on client!", true);
                    break;
            }
        }

        // For sending from the server
        private void SendServerResponse(MessageType message, BoltConnection connection)
        {
            Misc.Msg($"[JoinEvent] [SendServerResponse] Sending Request Packet to client", true);
            var packet = NewPacket(1, connection);
            packet.Packet.WriteByte((byte)message);
            Send(packet);
        }

        public void RequestInfo(BoltConnection connection) => SendServerResponse(MessageType.RequestInfo, connection);

        // For sending from the client
        public void SendClientResponse()
        {
            Misc.Msg($"[JoinEvent] [SendClientResponse] Sending response to client", true);
            var packet = NewPacket(5 + Signs.Instance.Manifest.Version.Length + 2 /* sizeof(byte + int + string) */, GlobalTargets.OnlyServer);
            packet.Packet.WriteByte((byte)MessageType.InfoPayload);

            packet.Packet.WriteString(Signs.Instance.Manifest.Version);

            Send(packet);
        }

        private void KickPlayer(BoltConnection connection)
        {
            Misc.Msg($"[JoinEvent] [KickPlayer] Kicking player for outdated mod version", true);
            connection.Disconnect(new CoopKickToken { Banned = false, KickMessage = "OUTDATED_VERSION_OF_SIGNS_MOD" }.Cast<IProtocolToken>());
        }

        public override string Id => "Signs_ReciveFromPlayerOnJoinEvent";
    }
}
