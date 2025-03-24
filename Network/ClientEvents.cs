using Bolt;
using RedLoader;
using UdpKit;

namespace BuildingMagnet.Network
{
    internal class ClientEvents : EventBase<ClientEvents>
    {
        public enum ClientEvent : byte
        {
            ServerReleaseControl = 0,
            ServerTakeControl = 1,
        }

        protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
        {
            RLog.Msg("[BuildingMagnet] [ClientEvents] [ReadMessageServer] Recived Event", true);
            RecivedEvent(packet, fromConnection);
        }

        private void RecivedEvent(UdpPacket packet, BoltConnection fromConnection)
        {
            var eventType = (ClientEvent)packet.ReadByte();
            switch (eventType)
            {
                case ClientEvent.ServerReleaseControl:
                    try
                    {
                        Bolt.NetworkId networkId = packet.ReadNetworkId();
                        BoltEntity boltEntity = BoltNetwork.FindEntity(networkId);
                        if (boltEntity == null) { RLog.Msg(ConsoleColor.Red, "[BuildingMagnet] [ClientEvents] [RecivedEvent] [ServerReleaseControl] Could Not Find Bolt Entity"); return; }
                        if (boltEntity.isOwner)
                        {
                            boltEntity.ReleaseControl();
                        }
                        else
                        {
                            RLog.Msg("[BuildingMagnet] [ClientEvents] [RecivedEvent] [ServerReleaseControl] Not Owner", true);
                        }

                    }
                    catch (System.Exception e)
                    {
                        RLog.Msg("[BuildingMagnet] [ClientEvents] [RecivedEvent] Error: " + e.Message, true);
                    }
                    break;
                case ClientEvent.ServerTakeControl:
                    try
                    {
                        Bolt.NetworkId networkId = packet.ReadNetworkId();
                        BoltEntity boltEntity = BoltNetwork.FindEntity(networkId);
                        if (boltEntity == null) { RLog.Msg(ConsoleColor.Red, "[BuildingMagnet] [ClientEvents] [RecivedEvent] [ServerTakeControl] Could Not Find Bolt Entity"); return; }
                        boltEntity.TakeControl();
                    }
                    catch (System.Exception e)
                    {
                        RLog.Msg("[BuildingMagnet] [ClientEvents] [RecivedEvent] Error: " + e.Message, true);
                    }
                    break;

            }
        }


        /// <summary>
        /// Send a client event to the server, can send eveything
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="networkId"></param>
        public void SendClientEvent(ClientEvent eventType, Bolt.NetworkId networkId)
        {
            if (!BoltNetwork.isRunning) { return; }

            var packet = NewPacket(128, GlobalTargets.OnlyServer);
            packet.Packet.WriteByte((byte)eventType);
            switch (eventType)
            {
                case ClientEvent.ServerReleaseControl:
                    packet.Packet.WriteNetworkId(networkId);
                    break;
                case ClientEvent.ServerTakeControl:
                    packet.Packet.WriteNetworkId(networkId);
                    break;
            }
            Send(packet);
        }

    }
}
