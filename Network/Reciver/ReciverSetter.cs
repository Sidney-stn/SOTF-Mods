using Bolt;
using RedLoader;
using SonsSdk.Networking;
using UdpKit;
using UnityEngine;
using static WirelessSignals.Network.Reciver.ReciverSyncEvent;

namespace WirelessSignals.Network.Reciver;

public class ReciverSetter : MonoBehaviour, Packets.IPacketReader
{

    private void SetUniqueId(string uniqueId)
    {
        if (string.IsNullOrEmpty(uniqueId))
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] UniqueId is null or empty");
            return;
        }
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.uniqueId = uniqueId;
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived UniqueId: {uniqueId}", true);
    }

    private void SetIsOn(bool isOn)
    {
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.isOn = isOn;
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived IsOn: {isOn}", true);
    }

    private void SetLinkedToTranmitterSwithUniqueId(string linkedToTranmitterSwithUniqueId)
    {
        if (string.IsNullOrEmpty(linkedToTranmitterSwithUniqueId))
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] linkedToTranmitterSwithUniqueId is null or empty");
            return;
        }
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.linkedToTranmitterSwithUniqueId = linkedToTranmitterSwithUniqueId;
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived linkedToTranmitterSwithUniqueId: {linkedToTranmitterSwithUniqueId}", true);
    }

    private void SetOwnerSteamId(string ownerSteamId)
    {
        if (string.IsNullOrEmpty(ownerSteamId))
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] ownerSteamId is null or empty");
            return;
        }
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.ownerSteamId = ownerSteamId;
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived ownerSteamId: {ownerSteamId}", true);
    }

    private void SetLinkedReciverObject(bool linkedReciverObject, string linkedReciverObjectName)
    {
        if (string.IsNullOrEmpty(linkedReciverObjectName) && linkedReciverObject == true)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] linkedReciverObjectName is null or empty - When linkedReciverObject is TRUE!");
            return;
        }
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.SetLinkedReciverObject(linkedReciverObject, linkedReciverObjectName);
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived linkedReciverObject: {linkedReciverObject} linkedReciverObjectName: {linkedReciverObjectName}", true);
    }

    private void SetObjectRange(float objectRange)
    {
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.objectRange = objectRange;
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived objectRange: {objectRange}", true);
    }

    private void SetRevertOuput(bool revertOuput)
    {
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp._revertOutput = revertOuput;
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived RevertOuput: {revertOuput}", true);
    }

    private void SetLoadedFromSave(bool loadedFromSave)
    {
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.loadedFromSave = loadedFromSave;
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived LoadedFromSave: {loadedFromSave}", true);
    }

    private void SetAllData(string uniqueId, bool isOn, string linkedToTranmitterSwithUniqueId, string ownerSteamId, bool linkedReciverObject, string linkedReciverObjectName, float objectRange, bool revertOuput, bool loadedFromSave)
    {
        if (string.IsNullOrEmpty(uniqueId))
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] UniqueId is null or empty");
            return;
        }
        if (string.IsNullOrEmpty(linkedToTranmitterSwithUniqueId))
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] linkedToTranmitterSwithUniqueId is null or empty");
            return;
        }
        if (string.IsNullOrEmpty(ownerSteamId))
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] ownerSteamId is null or empty");
            return;
        }
        if (string.IsNullOrEmpty(linkedReciverObjectName))
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] linkedReciverObjectName is null or empty");
            return;
        }
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.uniqueId = uniqueId == "None" ? string.Empty : uniqueId;
        comp.isOn = isOn;
        comp.linkedToTranmitterSwithUniqueId = linkedToTranmitterSwithUniqueId == "None" ? string.Empty : linkedToTranmitterSwithUniqueId;
        comp.ownerSteamId = ownerSteamId == "None" ? string.Empty : ownerSteamId;
        comp.SetLinkedReciverObject(linkedReciverObject, linkedReciverObjectName == "None" ? string.Empty : linkedReciverObjectName);
        comp.objectRange = objectRange;
        comp._revertOutput = revertOuput;
        comp.loadedFromSave = loadedFromSave;
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived All Data: UniqueId: " +
            $"{uniqueId} IsOn: {isOn} linkedToTranmitterSwithUniqueId: {linkedToTranmitterSwithUniqueId}" +
            $" ownerSteamId: {ownerSteamId} linkedReciverObject: {linkedReciverObject} linkedReciverObjectName:" +
            $" {linkedReciverObjectName} objectRange: {objectRange} RevertOuput: {revertOuput}" +
            $" LoadedFromSave: {loadedFromSave}", true);
    }

    private void SetPosition(Vector3 position)
    {
        if (position == Vector3.zero)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] position is zero");
            return;
        }
        gameObject.transform.position = position;
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived Position: {position}", true);
    }

    private void SetRotation(Quaternion rotation)
    {
        gameObject.transform.rotation = rotation;
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived Rotation: {rotation}", true);
    }

    private void SetTransform(Vector3 position, Quaternion rotation)
    {
        if (position == Vector3.zero)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] position is zero");
            return;
        }
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived Transform: Position: {position} Rotation: {rotation}", true);
    }


    public void ReadPacket(UdpPacket packet, BoltConnection fromConnection)
    {
        var type = (ReciverSyncEvent.ReciverSyncType)packet.ReadByte();
        switch (type)
        {
            case ReciverSyncType.UniqueId:
                SetUniqueId(packet.ReadString());
                break;
            case ReciverSyncType.IsOn:
                SetIsOn(packet.ReadBool());
                break;
            case ReciverSyncType.LinkedToTranmitterSwithUniqueId:
                SetLinkedToTranmitterSwithUniqueId(packet.ReadString());
                break;
            case ReciverSyncType.OwnerSteamId:
                SetOwnerSteamId(packet.ReadString());
                break;
            case ReciverSyncType.LinkedReciverObject:
                SetLinkedReciverObject(packet.ReadBool(), packet.ReadString());
                break;
            case ReciverSyncType.ObjectRange:
                SetObjectRange(packet.ReadFloat());
                break;
            case ReciverSyncType.RevertOuput:
                SetRevertOuput(packet.ReadBool());
                break;
            case ReciverSyncType.LoadedFromSave:
                SetRevertOuput(packet.ReadBool());
                break;
            case ReciverSyncType.AllData:
                SetAllData(
                    uniqueId: packet.ReadString(),
                    isOn: packet.ReadBool(),
                    linkedToTranmitterSwithUniqueId: packet.ReadString(),
                    ownerSteamId: packet.ReadString(),
                    linkedReciverObject: packet.ReadBool(),
                    linkedReciverObjectName: packet.ReadString(),
                    objectRange: packet.ReadFloat(),
                    revertOuput: packet.ReadBool(),
                    loadedFromSave: packet.ReadBool()
                );
                break;
            case ReciverSyncType.Position:
                SetPosition(packet.ReadVector3());
                break;
            case ReciverSyncType.Rotation:
                SetRotation(packet.ReadQuaternion());
                break;
            case ReciverSyncType.Transform:
                SetTransform(packet.ReadVector3(), packet.ReadQuaternion());
                break;
        }
    }
}