using RedLoader;
using UdpKit;

namespace WirelessSignals.Network.Joining
{
    public class JoiningEvent : EventBase<JoiningEvent>
    {
        /// Read message on the client
        protected override void ReadMessageClient(UdpPacket packet, BoltConnection _)
        {
            // Reed the bool state
            var state = packet.ReadBool();
            Misc.Msg($"Received state of OwnerToEdit: {state}", true);
            Config.OwnerToEdit.Value = state;
            Tools.CreatorSettings.lastState = state;

        }

        /// For sending from the server
        private void SendServerResponse(BoltConnection connection)
        {
            Misc.Msg($"Sending state off OwnerToEdit [{Config.OwnerToEdit.Value}]", true);
            var packet = NewPacket(4, connection);
            packet.Packet.WriteBool((bool)Config.OwnerToEdit.Value);
            Send(packet);
        }

        /// For sending from the server
        private void SendServerResponseToAll()
        {
            Misc.Msg($"Sending state off OwnerToEdit [{Config.OwnerToEdit.Value}] To All", true);
            var packet = NewPacket(4, Bolt.GlobalTargets.AllClients);
            packet.Packet.WriteBool((bool)Config.OwnerToEdit.Value);
            Send(packet);
        }

        public void SendInfo(BoltConnection connection) => SendServerResponse(connection);

        public void SendInfoToAll() => SendServerResponseToAll();
    }
}
