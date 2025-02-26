using Bolt;
using RedLoader;
using Color = UnityEngine.Color;
using UdpKit;

namespace WirelessSignals.Network.Reciver;

public class ReciverSyncEvent : RelayEventBase<ReciverSyncEvent, ReciverSetter>
{
    /// Read message on the server
    protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection) { }

    /// For sending from the client
    public void SendClientResponse() { }

    private void SendColorInternal(BoltEntity entity, Color color)
    {
        RLog.Msg(ConsoleColor.Gray, "Sending packet");
        Misc.Msg($"Sending color {color}", true);
        var packet = NewPacket(entity, 0, GlobalTargets.AllClients);
        packet.Packet.WriteColorRGB(color);
        Send(packet);
    }

    public static void SendColor(BoltEntity entity, Color color)
    {
        Instance.SendColorInternal(entity, color);
    }

    public override string Id => "WirelessSignals_ReciverSyncEvent";
}