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
        comp.SetLinkedReciverObject(linkedReciverObject, linkedReciverObjectName, runOnNetworkIfMultiplayer: false);
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
        comp.SetScanObjectRange(objectRange, false);
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
        comp.SetRevertOutput(revertOuput, runOnNetworkIfMultiplayer: false);
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

    private void SetAllData(string uniqueId, bool isOn, string linkedToTranmitterSwithUniqueId, string ownerSteamId, bool linkedReciverObject, string linkedReciverObjectName, float objectRange, bool revertOuput, bool loadedFromSave, bool loadedIn)
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
        comp.uniqueId = uniqueId.ToLower() == "none" ? string.Empty : uniqueId;
        comp.isOn = isOn;
        comp.linkedToTranmitterSwithUniqueId = linkedToTranmitterSwithUniqueId.ToLower() == "none" ? string.Empty : linkedToTranmitterSwithUniqueId;
        comp.ownerSteamId = ownerSteamId.ToLower() == "none" ? string.Empty : ownerSteamId;
        comp.SetLinkedReciverObject(linkedReciverObject, linkedReciverObjectName.ToLower() == "none" ? string.Empty : linkedReciverObjectName);
        comp.objectRange = objectRange;
        comp.SetRevertOutput(revertOuput, runOnNetworkIfMultiplayer: false);
        comp.loadedFromSave = loadedFromSave;
        comp.SetLoadedInNetwork(loadedIn);
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

    private void SetLoadedIn(bool loadedIn)
    {
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.SetLoadedInNetwork(loadedIn);
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived LoadedIn: {loadedIn}", true);
    }

    private void SetStateNetwork(bool state)
    {
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.SetStateNetwork(state);
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived State: {state}", true);
    }

    private void Unlink()
    {
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.Unlink(runOnNetworkIfMultiplayer: false);  // Dont Want Infinite Loop
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Unlinked", true);
    }

    private void Link(string transmitterUniqueId)
    {
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        if (transmitterUniqueId == "None")  // If Null "None" will be sent
        {
            comp.Link(null, runOnNetworkIfMultiplayer: false); // Dont Want Infinite Loop
        }
        else
        {
            comp.Link(transmitterUniqueId, runOnNetworkIfMultiplayer: false); // Dont Want Infinite Loop
        }

        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Linked", true);
    }

    private void ShowScanLines(bool state)
    {
        var comp = gameObject.GetComponent<Mono.Reciver>();
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.ShowScanLines(state, runOnNetworkIfMultiplayer: false);
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] ShowScanLines", true);
    }

    private void LateJoinSync(
        string uniqueId,
        bool isOn,
        string linkedToTranmitterSwithUniqueId,
        string ownerSteamId,
        bool linkedReciverObject,
        string linkedReciverObjectName,
        float objectRange,
        bool revertOuput,
        bool loadedFromSave,
        bool showScanLines,
        bool loadedIn,
        string syncComp  // Sync PlaceOnBoltEntity or RemoveFromBoltEntity
        )
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
            comp = gameObject.AddComponent<Mono.Reciver>();
        }
        if (comp == null)
        {
            RLog.Error("[Network] [ReciverSyncEvent] [UpdateState] Reciver Component is null");
            return;
        }
        comp.uniqueId = uniqueId.ToLower() == "none" ? string.Empty : uniqueId;
        comp.isOn = isOn;
        comp.linkedToTranmitterSwithUniqueId = linkedToTranmitterSwithUniqueId.ToLower() == "none" ? string.Empty : linkedToTranmitterSwithUniqueId;
        comp.ownerSteamId = ownerSteamId.ToLower() == "none" ? string.Empty : ownerSteamId;
        comp.SetLinkedReciverObject(linkedReciverObject, linkedReciverObjectName.ToLower() == "none" ? string.Empty : linkedReciverObjectName, runOnNetworkIfMultiplayer: false);
        comp.objectRange = objectRange;
        comp.SetRevertOutput(revertOuput, runOnNetworkIfMultiplayer: false);
        comp.loadedFromSave = loadedFromSave;
        comp.SetLoadedInNetwork(loadedIn);
        comp.ShowScanLines(showScanLines, runOnNetworkIfMultiplayer: false);
        comp.SetScanObjectRange(objectRange, runOnNetworkIfMultiplayer: false);
        comp.SetLinkUi(Tools.CreatorSettings.lastState);
        if (string.IsNullOrEmpty(syncComp))
        {
            RLog.Warning("[Network] [ReciverSyncEvent] [UpdateState] syncComp is null or empty, Some items will not sync correctly");
        } 
        else
        {
            switch (syncComp)
            {
                case "REMOVE_NETOWNER_AND_PLACESTRUCTURE":
                    RemoveNetworkOwnerComp();
                    break;
                case "REMOVE_NETOWNER":
                    var netOwner = gameObject.GetComponent<Mono.NetworkOwner>();
                    if (netOwner != null)
                    {
                        netOwner.DestroyUi();
                        DestroyImmediate(netOwner);
                        Misc.Msg("[ReciverSetter] [ReadPacket] [LateJoinSync] Removed NetworkOwner Component", true);
                    }
                    break;
                case "REMOVE_PLACESTRUCTURE":
                    var placeStructure = gameObject.GetComponent<Mono.PlaceStructure>();
                    if (placeStructure != null)
                    {
                        DestroyImmediate(placeStructure);
                        Misc.Msg("[ReciverSetter] [ReadPacket] [LateJoinSync] Removed PlaceStructure Component", true);
                    }
                    break;
                case "NONE":
                    AddNetworkOwnerComp();
                    break;
            }
        }
        
        Misc.Msg($"[Network] [ReciverSyncEvent] [UpdateState] Recived LateJoinSync: UniqueId: " +
            $"{uniqueId} IsOn: {isOn} linkedToTranmitterSwithUniqueId: {linkedToTranmitterSwithUniqueId}" +
            $" ownerSteamId: {ownerSteamId} linkedReciverObject: {linkedReciverObject} linkedReciverObjectName:" +
            $" {linkedReciverObjectName} objectRange: {objectRange} RevertOuput: {revertOuput}" +
            $" LoadedFromSave: {loadedFromSave}" + $"ShowScanLines: {showScanLines}" + $"LoadedIn: {loadedIn}", true);
    }


    public void ReadPacket(UdpPacket packet, BoltConnection fromConnection)
    {
        if (BoltNetwork.isServer)
        {
            Misc.Msg("[ReciverSetter] [ReadPacket] Recived packet on server", true);
        } else
        {
            Misc.Msg("[ReciverSetter] [ReadPacket] Recived packet on client", true);
        }
        var type = (ReciverSyncEvent.ReciverSyncType)packet.ReadByte();
        string toPlayerSteamId = packet.ReadString();
        if (toPlayerSteamId.ToLower() != "all" && toPlayerSteamId != Misc.GetMySteamId())
        {
            Misc.Msg("[ReciverSetter] [ReadPacket] Recived packet not meant for this player", true);
            Misc.Msg($"[ReciverSetter] [ReadPacket] Recived packet meant for: {toPlayerSteamId}", true);
            return;
        }
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
                SetLoadedFromSave(packet.ReadBool());
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
                    loadedFromSave: packet.ReadBool(),
                    loadedIn: packet.ReadBool()
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
            case ReciverSyncType.SetLoadedIn:
                SetLoadedIn(packet.ReadBool());
                break;
            case ReciverSyncType.SetState:
                SetStateNetwork(packet.ReadBool());
                break;
            case ReciverSyncType.Unlink:
                string fromNetworkUnlink = packet.ReadString();
                if (fromNetworkUnlink == "UNLINK")
                {
                    Unlink();
                    Misc.Msg("[ReciverSetter] [ReadPacket] [Unlink] Unlinked", true);
                }
                else
                {
                    Misc.Msg($"[ReciverSetter] [ReadPacket] [Unlink] Unknown string: {fromNetworkUnlink}", true);
                }
                break;
            case ReciverSyncType.Link:
                Link(packet.ReadString());
                break;
            case ReciverSyncType.ShowScanLines:
                ShowScanLines(packet.ReadBool());
                break;
            case ReciverSyncType.LateJoinSync:
                if (BoltNetwork.isServer)
                {
                    Misc.Msg("[ReciverSetter] [ReadPacket] [LateJoinSync] Recived LateJoinSync on server, skipping", true);
                    return;
                }
                LateJoinSync(
                    uniqueId: packet.ReadString(),
                    isOn: packet.ReadBool(),
                    linkedToTranmitterSwithUniqueId: packet.ReadString(),
                    ownerSteamId: packet.ReadString(),
                    linkedReciverObject: packet.ReadBool(),
                    linkedReciverObjectName: packet.ReadString(),
                    objectRange: packet.ReadFloat(),
                    revertOuput: packet.ReadBool(),
                    loadedFromSave: packet.ReadBool(),
                    loadedIn: packet.ReadBool(),
                    showScanLines: packet.ReadBool(),
                    syncComp: packet.ReadString()
                );
                break;
            default:
                RLog.Error($"[ReciverSetter] [ReadPacket] Unknown packet type: {type}");
                break;
        }
    }
}