using Bolt;
using RedLoader;
using SonsSdk.Networking;
using UdpKit;
using UnityEngine;
using Color = System.Drawing.Color;

namespace SimpleElevator;

public class EventBase<T> : Packets.NetEvent where T : Packets.NetEvent, new()
{
    public static T Instance;
    
    public static void Register()
    {
        Instance = new T();
        Packets.Register(Instance);
        RLog.Msg(Color.GreenYellow, $"Registered {typeof(T).Name}");
    }
    
    public override void Read(UdpPacket packet, BoltConnection fromConnection)
    {
        if (BoltNetwork.isServer)
            ReadMessageServer(packet, fromConnection);
        else
            ReadMessageClient(packet, fromConnection);
    }
    
    /// Read message on the server
    protected virtual void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
    { }

    /// Read message on the client
    protected virtual void ReadMessageClient(UdpPacket packet, BoltConnection fromConnection)
    { }

    public override string Id => typeof(T).FullName;
}

public class RelayEventBase<T, TRelay> : EventBase<T> where T : Packets.NetEvent, new() where TRelay : MonoBehaviour, Packets.IPacketReader
{
    protected override void ReadMessageClient(UdpPacket packet, BoltConnection fromConnection)
    {
        TryRelay<TRelay>(packet, fromConnection);
    }

    protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)  // I Added This Maybe Not Safe
    {
        TryRelay<TRelay>(packet, fromConnection);
    }

    protected Packets.EventPacket NewPacket(BoltEntity entity, int size, GlobalTargets targets)
    {
        var packet = NewPacket(size + 128, targets);
        packet.Packet.WriteBoltEntity(entity);
        return packet;
    }
}