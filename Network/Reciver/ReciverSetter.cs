using RedLoader;
using SonsSdk.Networking;
using UdpKit;
using UnityEngine;

namespace WirelessSignals.Network.Reciver;

public class ReciverSetter : MonoBehaviour, Packets.IPacketReader
{

    public void SendLampState(bool onOff)
    {
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSetter] [SendLampState] Reciver component not found");
            return;
        }
        comp.TestLampLight(onOff);
    }

    public void ReadPacket(UdpPacket packet, BoltConnection fromConnection)
    {
        bool state = packet.ReadBool();
        Misc.Msg($"Received state of Lamp: {state}", true);
        SendLampState(state);
        
    }
}