using RedLoader;
using SonsSdk.Networking;
using UdpKit;
using UnityEngine;

namespace WirelessSignals.Network.Reciver;

public class ReciverSetter : MonoBehaviour, Packets.IPacketReader
{
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private Material _material;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void SetColor(Color color)
    {
        _material.SetColor(BaseColor, color);
    }

    public void ReadPacket(UdpPacket packet, BoltConnection fromConnection)
    {
        SetColor(packet.ReadColorRGB());
        RLog.Msg(System.Drawing.Color.Gray, "Read packet and set color");
    }
}