using Bolt;
using RedLoader;
using Color = UnityEngine.Color;
using UdpKit;
using UdpKit.Protocol;
using UnityEngine;

namespace WirelessSignals.Network.Reciver;

public class ReciverSyncEvent : RelayEventBase<ReciverSyncEvent, ReciverSetter>
{
    public enum ReciverSyncType : byte
    {
        UniqueId = 0,
        IsOn = 1,
        LinkedToTranmitterSwithUniqueId = 2,
        OwnerSteamId = 3,
        LinkedReciverObject = 4,
        ObjectRange = 5,
        RevertOuput = 6,
        LoadedFromSave = 7,
        AllData = 8,
        Position = 9,
        Rotation = 10,
        Transform = 11,
        PlaceOnBoltEntity = 12,
        RemoveFromBoltEntity = 13,
        LinkUiSync = 14,
        SetLoadedIn = 15,
        SetState = 16,  // Reciver Script State Function
        Unlink = 17, // Reciver Script Unlink Function
        Link = 18, // Reciver Script Link Function
        ShowScanLines = 19, // Reciver Script ShowScanLines Function
        LateJoinSync = 20,
    }

    /// For sending from the client
    public void SendClientResponse() { }

    private void UpdateStateInternal(BoltEntity entity, ReciverSyncType type, string toConnectionSteamId = null)
    {
        Misc.Msg($"[ReciverSyncEvent] [UpdateState] Sending {type} to {entity}", true);
        var packet = NewPacket(entity, 128, GlobalTargets.Everyone);  // Still Sent To Everyone Even If sendToConnection Is Not Null
        packet.Packet.WriteByte((byte)type);
        if (toConnectionSteamId != null)
        {
            packet.Packet.WriteString(toConnectionSteamId);
        }
        else
        {
            packet.Packet.WriteString("All");
        }
        switch (type)
        {
            case ReciverSyncType.UniqueId:
                string uniqueId = entity.gameObject.GetComponent<Mono.Reciver>().uniqueId;
                if (string.IsNullOrEmpty(uniqueId))
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] UniqueId is null or empty");
                    packet.Packet.Dispose();
                    return;
                }
                packet.Packet.WriteString(uniqueId);
                break;
            case ReciverSyncType.IsOn:
                bool? isOn = entity.gameObject.GetComponent<Mono.Reciver>().isOn;
                if (isOn == null)
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] IsOn is null");
                    packet.Packet.Dispose();
                    return;
                }
                packet.Packet.WriteBool((bool)isOn);
                break;
            case ReciverSyncType.LinkedToTranmitterSwithUniqueId:
                string linkedToTranmitterSwithUniqueId = entity.gameObject.GetComponent<Mono.Reciver>().linkedToTranmitterSwithUniqueId;
                if (string.IsNullOrEmpty(linkedToTranmitterSwithUniqueId))
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] linkedToTranmitterSwithUniqueId is null or empty");
                    packet.Packet.Dispose();
                    return;
                }
                packet.Packet.WriteString(linkedToTranmitterSwithUniqueId);
                break;
            case ReciverSyncType.OwnerSteamId:
                string ownerSteamId = entity.gameObject.GetComponent<Mono.Reciver>().ownerSteamId;
                if (string.IsNullOrEmpty(ownerSteamId))
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] ownerSteamId is null or empty");
                    packet.Packet.Dispose();
                    return;
                }
                packet.Packet.WriteString(ownerSteamId);
                break;
            case ReciverSyncType.LinkedReciverObject:
                bool linkedReciverObject = entity.gameObject.GetComponent<Mono.Reciver>().IsLinkedReciverObject();
                string linkedReciverObjectName = entity.gameObject.GetComponent<Mono.Reciver>().GetLinkedReciverObjectName();
                if (string.IsNullOrEmpty(linkedReciverObjectName))
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] linkedReciverObjectName is null or empty");
                    packet.Packet.Dispose();
                    return;
                }
                packet.Packet.WriteBool(linkedReciverObject);
                packet.Packet.WriteString(linkedReciverObjectName);
                break;
            case ReciverSyncType.ObjectRange:
                float objectRange = entity.gameObject.GetComponent<Mono.Reciver>().objectRange;
                packet.Packet.WriteFloat(objectRange);
                break;
            case ReciverSyncType.RevertOuput:
                bool revertOuput = entity.gameObject.GetComponent<Mono.Reciver>()._revertOutput;
                packet.Packet.WriteBool(revertOuput);
                break;
            case ReciverSyncType.LoadedFromSave:
                bool? loadedFromSave = entity.gameObject.GetComponent<Mono.Reciver>().loadedFromSave;
                if (loadedFromSave == null)
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] loadedFromSave is null");
                    packet.Packet.Dispose();
                    return;
                }
                packet.Packet.WriteBool((bool)loadedFromSave);
                break;
            case ReciverSyncType.AllData:
                Mono.Reciver reciver = entity.gameObject.GetComponent<Mono.Reciver>();
                if (reciver == null)
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] reciver is null");
                    packet.Packet.Dispose();
                    return;
                }
                if (string.IsNullOrEmpty(reciver.uniqueId))
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] UniqueId is null or empty");
                    packet.Packet.Dispose();
                    return;
                }
                if (reciver.isOn == null)
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] IsOn is null");
                    packet.Packet.Dispose();
                    return;
                }
                
                packet.Packet.WriteString(reciver.uniqueId);  // UniqueId
                packet.Packet.WriteBool((bool)reciver.isOn);  // IsOn
                if (string.IsNullOrEmpty(reciver.linkedToTranmitterSwithUniqueId))  // LinkedToTransmitterSwitchUniqueId
                {
                    packet.Packet.WriteString("None");
                    return;
                }
                else { packet.Packet.WriteString(reciver.linkedToTranmitterSwithUniqueId); }
                if (string.IsNullOrEmpty(reciver.ownerSteamId))  // OwnerSteamId
                {
                    packet.Packet.WriteString("None");
                }
                else { packet.Packet.WriteString(reciver.ownerSteamId); }
                packet.Packet.WriteBool(reciver.IsLinkedReciverObject());  // LinkedReciverObject
                if (string.IsNullOrEmpty(reciver.GetLinkedReciverObjectName()))  // LinkedReciverObjectName
                {
                    packet.Packet.WriteString("None");
                }
                else { packet.Packet.WriteString(reciver.GetLinkedReciverObjectName()); }
                packet.Packet.WriteFloat(reciver.objectRange);  // ObjectRange
                packet.Packet.WriteBool(reciver._revertOutput);  // RevertOuput
                if (reciver.loadedFromSave == null)  // LoadedFromSave
                {
                    packet.Packet.WriteBool(false);
                }
                else { packet.Packet.WriteBool((bool)reciver.loadedFromSave); }

                packet.Packet.WriteBool((bool)reciver.IsLoadedIn());  // LoadedIn

                break;
            case ReciverSyncType.Position:
                Vector3 position = entity.gameObject.transform.position;
                if (position == Vector3.zero)
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] position is zero");
                    packet.Packet.Dispose();
                    return;
                }
                packet.Packet.WriteVector3(position);
                break;
            case ReciverSyncType.Rotation:
                Quaternion rotation = entity.gameObject.transform.rotation;
                packet.Packet.WriteQuaternion(rotation);
                break;
            case ReciverSyncType.Transform:
                Vector3 position1 = entity.gameObject.transform.position;
                Quaternion rotation1 = entity.gameObject.transform.rotation;
                if (position1 == Vector3.zero)
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] position or rotation is zero or identity");
                    packet.Packet.Dispose();
                    return;
                }
                packet.Packet.WriteVector3(position1);
                packet.Packet.WriteQuaternion(rotation1);
                break;
            case ReciverSyncType.PlaceOnBoltEntity:
                packet.Packet.WriteString("PLACE_NETWORK_OWNER_SCRIPT");
                break;
            case ReciverSyncType.RemoveFromBoltEntity:
                packet.Packet.WriteString("REMOVE_NETWORK_OWNER_SCRIPT");
                break;
            case ReciverSyncType.LinkUiSync:
                bool uiVisibleForOwnerOnly = Tools.CreatorSettings.lastState;
                packet.Packet.WriteBool(uiVisibleForOwnerOnly);
                break;
            case ReciverSyncType.SetLoadedIn:
                bool? loadedIn = null;
                try
                {
                    loadedIn = entity.gameObject.GetComponent<Mono.Reciver>().IsLoadedIn();
                }
                catch (System.Exception e)
                {
                    RLog.Error($"[Network] [ReciverSyncEvent] [UpdateState] SetLoadedIn failed: {e}");
                    packet.Packet.Dispose();
                    return;
                }
                if (loadedIn != null)
                {
                    packet.Packet.WriteBool((bool)loadedIn);
                }
                else
                {
                    RLog.Error($"[Network] [ReciverSyncEvent] [UpdateState] SetLoadedIn failed");
                    packet.Packet.Dispose();
                    return;
                }
                break;
            case ReciverSyncType.SetState:
                bool? state = entity.gameObject.GetComponent<Mono.Reciver>().isOn;
                if (state != null)
                {
                    packet.Packet.WriteBool((bool)state);
                } 
                else
                {
                    RLog.Error($"[Network] [ReciverSyncEvent] [UpdateState] SetState failed");
                    packet.Packet.Dispose();
                    return;
                }
                break;
            case ReciverSyncType.Unlink:
                packet.Packet.WriteString("UNLINK");
                break;
            case ReciverSyncType.Link:
                string transmitterUniqueId = entity.gameObject.GetComponent<Mono.Reciver>().linkedToTranmitterSwithUniqueId;
                if (string.IsNullOrEmpty(transmitterUniqueId))
                {
                    packet.Packet.WriteString("None");
                } 
                else
                {
                    packet.Packet.WriteString(transmitterUniqueId);
                }
                break;
            case ReciverSyncType.ShowScanLines:
                bool showScanLines = entity.gameObject.GetComponent<Mono.Reciver>().IsScanLinesShown();
                packet.Packet.WriteBool(showScanLines);
                break;
            case ReciverSyncType.LateJoinSync: // Only Host Can Send This Event
                if (BoltNetwork.isServer == false)
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] LateJoinSync can only be sent by the host");
                    packet.Packet.Dispose();
                    return;
                }
                Mono.Reciver comp = entity.gameObject.GetComponent<Mono.Reciver>();
                if (comp == null)
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] reciver is null");
                    packet.Packet.Dispose();
                    return;
                }
                if (string.IsNullOrEmpty(comp.uniqueId))
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] UniqueId is null or empty");
                    packet.Packet.Dispose();
                    return;
                }
                if (comp.isOn == null)
                {
                    RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] IsOn is null");
                    packet.Packet.Dispose();
                    return;
                }

                packet.Packet.WriteString(comp.uniqueId);  // UniqueId
                packet.Packet.WriteBool((bool)comp.isOn);  // IsOn
                if (string.IsNullOrEmpty(comp.linkedToTranmitterSwithUniqueId))  // LinkedToTransmitterSwitchUniqueId
                {
                    packet.Packet.WriteString("None");
                }
                else { packet.Packet.WriteString(comp.linkedToTranmitterSwithUniqueId); }
                if (string.IsNullOrEmpty(comp.ownerSteamId))  // OwnerSteamId
                {
                    packet.Packet.WriteString("None");
                }
                else { packet.Packet.WriteString(comp.ownerSteamId); }
                packet.Packet.WriteBool(comp.IsLinkedReciverObject());  // LinkedReciverObject
                if (string.IsNullOrEmpty(comp.GetLinkedReciverObjectName()))  // LinkedReciverObjectName
                {
                    packet.Packet.WriteString("None");
                }
                else { packet.Packet.WriteString(comp.GetLinkedReciverObjectName()); }
                packet.Packet.WriteFloat(comp.objectRange);  // ObjectRange
                packet.Packet.WriteBool(comp._revertOutput);  // RevertOuput
                if (comp.loadedFromSave == null)  // LoadedFromSave
                {
                    packet.Packet.WriteBool(false);
                }
                else { packet.Packet.WriteBool((bool)comp.loadedFromSave); }

                packet.Packet.WriteBool((bool)comp.IsLoadedIn());  // LoadedIn

                packet.Packet.WriteBool(comp.IsScanLinesShown());  // ShowScanLines

                var netOwnerComp = entity.gameObject.GetComponent<Mono.NetworkOwner>();
                var placeStructureComp = entity.gameObject.GetComponent<Mono.PlaceStructure>();

                if (netOwnerComp == null && placeStructureComp == null)  // Sync PlaceOnBoltEntity or RemoveFromBoltEntity
                {
                    packet.Packet.WriteString("REMOVE_NETOWNER_AND_PLACESTRUCTURE");  // Remove Both
                }
                else if (netOwnerComp == null && placeStructureComp != null)
                {
                    packet.Packet.WriteString("REMOVE_NETOWNER");  // Remove NetOwner
                }
                else if (netOwnerComp != null && placeStructureComp == null)
                {
                    packet.Packet.WriteString("REMOVE_PLACESTRUCTURE");  // Remove PlaceStructure
                }
                else  // If Both Is != null
                {
                    packet.Packet.WriteString("NONE");  // Run AddNetworkOwnerComp
                }
                Misc.Msg($"[ReciverSyncEvent] [UpdateState] LateJoinSync Sent Object UniqueId: {comp.uniqueId}");
                break;


        }
        Send(packet);
    }



    public static void SendState(BoltEntity entity, ReciverSyncType type)
    {
        Instance.UpdateStateInternal(entity, type);
    }

    public static void SendState(BoltEntity entity, string toConnectionSteamId, ReciverSyncType type)
    {
        Instance.UpdateStateInternal(entity, type, toConnectionSteamId);
    }

    public override string Id => "WirelessSignals_ReciverSyncEvent";
}