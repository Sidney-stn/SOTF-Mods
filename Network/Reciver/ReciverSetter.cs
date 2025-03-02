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

    private void AddNetworkOwnerComp()
    {
        var comp = gameObject.GetComponent<Mono.NetworkOwner>();
        if (comp == null)
        {
            if (SonsSdk.Networking.NetUtils.IsDedicatedServer)
            {
                Misc.Msg("[ReciverSyncEvent] [AddNetworkOwnerComp] Skip Adding Comp - Recieved On DedicatedServer", true);
                return;
            }
            var addedComp = gameObject.AddComponent<Mono.NetworkOwner>();
            addedComp.fromNetwork = true;
            addedComp.isSetupPrefab = false;
            if (addedComp.CheckIfSettingsWereSetCorrectly() == false)
            {
                RLog.Warning("[ReciverSyncEvent] [AddNetworkOwnerComp] Settings Were Not Set Correctly, even tho it was set, trying agian");
                addedComp.FixSettings();
                return;
            }
            var getToCheck = gameObject.GetComponent<Mono.NetworkOwner>();
            if (getToCheck != null)
            {
                if (getToCheck.CheckIfSettingsWereSetCorrectly() == false)
                {
                    RLog.Error("[ReciverSyncEvent] [AddNetworkOwnerComp] Settings Were Not Set Correctly - Deleting");
                    DestroyImmediate(getToCheck);
                    return;
                }
                Misc.Msg("[ReciverSyncEvent] [AddNetworkOwnerComp] Added NetworkOwner Component", true);
            } else
            {
                RLog.Error("[ReciverSyncEvent] [AddNetworkOwnerComp] Failed To Add NetworkOwner Component");
            }

            // Add Reciver Controller
            var reciver = gameObject.GetComponent<Mono.Reciver>();
            if (reciver != null)
            {
                Misc.Msg("[ReciverSyncEvent] [AddNetworkOwnerComp] Reciver Component Already Exists, Setting State", true);
                reciver.ownerSteamId = null;
                reciver.uniqueId = null;

            } else
            {
                Misc.Msg("[ReciverSyncEvent] [AddNetworkOwnerComp] Reciver Component Does Not Exist, Adding", true);
                var reciverController = gameObject.AddComponent<Mono.Reciver>();
                reciverController.uniqueId = null;
                reciverController.ownerSteamId = null;
            }
            
        }
        else
        {
            Misc.Msg("[ReciverSyncEvent] [AddNetworkOwnerComp] NetworkOwner Component Already Exists", true);
        }
    }

    private void RemoveNetworkOwnerComp()
    {
        if (SonsSdk.Networking.NetUtils.IsDedicatedServer)
        {
            Misc.Msg("[ReciverSyncEvent] [ReadPacket] [RemoveFromBoltEntity] Skip Removing Comp - Recieved On DedicatedServer", true);
            return;
        }
        var comp = gameObject.GetComponent<Mono.NetworkOwner>();
        if (comp != null)
        {
            comp.DestroyUi();
            DestroyImmediate(comp);
            Misc.Msg("[ReciverSyncEvent] [RemoveNetworkOwnerComp] Removed NetworkOwner Component", true);
        }
        var comp2 = gameObject.GetComponent<Mono.PlaceStructure>();
        if (comp2 != null)
        {
            DestroyImmediate(comp2);
            Misc.Msg("[ReciverSyncEvent] [RemoveNetworkOwnerComp] Removed PlaceStructure Component", true);
        }
    }

    private void LinkUiForOwnerOrAll(bool onlyOwner)
    {
        var reciver = gameObject.GetComponent<Mono.Reciver>();
        if (reciver != null)
        {
            Misc.Msg("[ReciverSyncEvent] [LinkUiForOwnerOrAll] Linked UI", true);
            reciver.SetLinkUi(onlyOwner);
        } 
        else
        {
            RLog.Error("[ReciverSyncEvent] [LinkUiForOwnerOrAll] Reciver Component is null");
        }
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
            case ReciverSyncType.PlaceOnBoltEntity:
                string fromNetwork = packet.ReadString();
                if (fromNetwork == "PLACE_NETWORK_OWNER_SCRIPT")
                {
                    AddNetworkOwnerComp();
                    Misc.Msg("[ReciverSetter] [ReadPacket] [PlaceOnBoltEntity] Added NetworkOwner Component", true);
                }
                else
                {
                    Misc.Msg($"[ReciverSetter] [ReadPacket] [PlaceOnBoltEntity] Unknown string: {fromNetwork}", true);
                }
                break;
            case ReciverSyncType.RemoveFromBoltEntity:
                string fromNetworkRemove = packet.ReadString();
                if (fromNetworkRemove == "REMOVE_NETWORK_OWNER_SCRIPT")
                {
                    RemoveNetworkOwnerComp();
                    Misc.Msg("[ReciverSetter] [ReadPacket] [RemoveFromBoltEntity] Removed NetworkOwner Component", true);
                }
                else
                {
                    Misc.Msg($"[ReciverSetter] [ReadPacket] [RemoveFromBoltEntity] Unknown string: {fromNetworkRemove}", true);
                }
                break;
            case ReciverSyncType.LinkUiSync:
                LinkUiForOwnerOrAll(packet.ReadBool());
                break;
        }
    }
}