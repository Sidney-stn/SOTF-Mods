using RedLoader;
using Sons.Gui.Input;
using SonsSdk;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace WirelessSignals.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class Reciver : MonoBehaviour
    {
        public string uniqueId;
        public bool? isOn = false;
        public bool isSetupPrefab;
        public string linkedToTranmitterSwithUniqueId = null;
        public string ownerSteamId = null;  // If owner is null, then it's public
        public bool? loadedFromSave = null;

        // Link Ui Menu
        public LinkUiElement _linkUi = null;

        // Settings Menu
        private bool _linkedReciverObject = false;  // To Trigger Cases On Turn On/Off
        private string _linkedReciverObjectName = null;  // Always lowercase
        public float objectRange = 1f;  // Range to detect objects, Physics.OverlapSphere
        private HashSet<GameObject> _objectsInRange = new HashSet<GameObject>(new Tools.GameObjectComparer());  // Objects in range, NOTE: Only Valid Ojbects
        public bool _revertOutput = false;  // Revert output for linked objects

        // Load In Delay. Makes so you do not crash if you try to open the ui on the same frame as the object is created
        private float _loadInDelay = 1f;
        private bool _loadedIn = false;


        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Start()
        {
            if (isSetupPrefab)
            {
                return;
            }
            if (string.IsNullOrEmpty(uniqueId) && !BoltNetwork.isRunning)
            {
                RLog.Warning("[Mono] [Reciver] [Start]: uniqueId is null! Shound Never be");
                SonsTools.ShowMessage("Something went wrong when placing down structure!");
            }

            //if (BoltNetwork.isRunning)
            //{
            //    // Make So We Can Take Ownership
            //    if (BoltNetwork.isServer)
            //    {
            //        Misc.Msg("[Mono] [Reciver] [Start] IsServer");
            //    }
            //    else if (BoltNetwork.isClient)
            //    {
            //        Misc.Msg("[Mono] [Reciver] [Start] IsClient - Only Host Can Place Out Items");
            //        return;
            //    }
            //    else
            //    {
            //        Misc.Msg("[Mono] [Reciver] [Start] IsNotServerOrClient");
            //    }
            //}

            // Light
            GameObject lights = transform.FindChild("Light").gameObject;

            GameObject floorLight = lights.transform.FindChild("Floor").gameObject;
            var floorLightInWorld = floorLight.GetComponentInChildren<Light>();
            floorLightInWorld.intensity = 1000;
            floorLightInWorld.range = 1;

            GameObject bulbLight = lights.transform.FindChild("Bulb").gameObject;
            var bulbLightInWorld = bulbLight.GetComponentInChildren<Light>();
            bulbLightInWorld.intensity = 100000;
            bulbLightInWorld.range = 0.1f;


            if (isOn == true)
            {
                TurnOnLight();
            }
            else
            {
                TurnOffLight();
            }
            UpdateDebugUi(true);
            if (Debug.VisualData.GetDebugMode())
            {
                SetDebugUi(true);
            }

            if (BoltNetwork.isRunning)
            {
                if (ownerSteamId == null)
                {
                    //RLog.Warning("[Mono] [Reciver] [Start] OwnerSteamId Must Be Assigned In Multiplayer");
                    Misc.Msg("[Mono] [Reciver] [Start] OwnerSteamId Must Be Assigned", true);
                }
                return;
            }

            if (_linkUi == null)
            {
                if (Tools.CreatorSettings.lastState)  // If true, means we need to check if we are owner to create link ui
                {
                    if (Tools.CreatorSettings.IsOwner(ownerSteamId))
                    {
                        _linkUi = UI.LinkUi.CreateLinkUi(gameObject, 2f, null, Assets.UIAdjust, new Vector3(0, 0f, 0), "screen.take");
                        Misc.Msg("[Reciver] [Start] CreatorSettings.lastState is true - Created LinkUi");
                    } else
                    {
                        Misc.Msg("[Reciver] [Start] CreatorSettings.lastState is true - Not Owner, No LinkUi");
                    }
                }
                else
                {
                    Misc.Msg("[Reciver] [Start] CreatorSettings.lastState is false - Creating LinkUi [Everyone can change]");
                    _linkUi = UI.LinkUi.CreateLinkUi(gameObject, 2f, null, Assets.UIAdjust, new Vector3(0, 0f, 0), "screen.take");
                }
            }

            LoadInDelay().RunCoro();

        }

        private IEnumerator LoadInDelay()
        {
            yield return new WaitForSeconds(_loadInDelay);
            _loadedIn = true;
            if (BoltNetwork.isRunning)  // Raise Network Event
            {
                var boltEntity = gameObject.GetComponent<BoltEntity>();
                if (boltEntity != null)
                {
                    Network.Reciver.ReciverSyncEvent.SendState(boltEntity, Network.Reciver.ReciverSyncEvent.ReciverSyncType.SetLoadedIn);
                } else
                {
                    RLog.Error("[Reciver] [LoadInDelay] BoltEntity is null, Cant Send LoadedIn Event");
                }
                
            }
            yield break;
        }

        public bool IsLoadedIn()
        {
            return _loadedIn;
        }

        public void SetLoadedInNetwork(bool loadedIn)
        {
            if (BoltNetwork.isRunning)
            {
                _loadedIn = loadedIn;
            }
        }

        private void TurnOnLight()
        {
            GameObject lights = transform.FindChild("Light").gameObject;
            if (lights != null)
            {
                lights.SetActive(true);
            }
            else
            {
                Misc.Msg("[Reciver] [TurnOnLight] Can't Turn On Lights, Null!");
            }
            // Network
        }
        private void TurnOffLight()
        {
            GameObject lights = transform.FindChild("Light").gameObject;
            if (lights != null)
            {
                lights.SetActive(false);
            }
            else
            {
                Misc.Msg("[Reciver] [TurnOffLight] Can't Turn Off Lights, Null!");
            }
            // Network
        }
        public void TestLampLight(bool state)
        {
            GameObject lights = transform.FindChild("Light").gameObject;
            if (lights != null)
            {
                lights.SetActive(state);
            }
            else
            {
                Misc.Msg("[Reciver] [TestLampLight] Can't Turn On Lights, Null!");
            }
        }

        public void SetState(bool state)
        {
            Misc.Msg($"[Reciver] [SetState] Setting State: {state}");
            isOn = state;
            if (state)
            {
                TurnOnLight();
            }
            else
            {
                TurnOffLight();
            }
            if (_linkedReciverObject)
            {
                if (!string.IsNullOrEmpty(_linkedReciverObjectName))
                {
                    if (_linkedReciverObjectName == "defensivewallgate")
                    {
                        UpdateDefenseWallGate(state);
                    }
                }
            }
            if (BoltNetwork.isRunning)
            {
                Network.Reciver.ReciverSyncEvent.SendState(gameObject.GetComponent<BoltEntity>(), Network.Reciver.ReciverSyncEvent.ReciverSyncType.SetState);
            }
            UpdateDebugUi();
        }

        public void SetStateNetwork(bool state)  // On State Change Recived From ReciverSyncEvent
        {
            Misc.Msg($"[Reciver] [SetStateNetwork] Setting State: {state}");
            isOn = state;
            if (state)
            {
                TurnOnLight();
            }
            else
            {
                TurnOffLight();
            }
            if (_linkedReciverObject)
            {
                if (!string.IsNullOrEmpty(_linkedReciverObjectName))
                {
                    if (_linkedReciverObjectName == "defensivewallgate")
                    {
                        UpdateDefenseWallGate(state);
                    }
                }
            }
            UpdateDebugUi();
        }

        public void Unlink(bool runOnNetworkIfMultiplayer = true)
        {
            Misc.Msg("[Reciver] [Unlink] Unlinking Reciver");
            isOn = false;
            TurnOffLight();
            linkedToTranmitterSwithUniqueId = null;
            if (runOnNetworkIfMultiplayer && BoltNetwork.isRunning)
            {
                Network.Reciver.ReciverSyncEvent.SendState(gameObject.GetComponent<BoltEntity>(), Network.Reciver.ReciverSyncEvent.ReciverSyncType.Unlink);
            }
            UpdateDebugUi();
        }

        private void SendLinkNetworkEvent()
        {
            if (BoltNetwork.isRunning)
            {
                Network.Reciver.ReciverSyncEvent.SendState(gameObject.GetComponent<BoltEntity>(), Network.Reciver.ReciverSyncEvent.ReciverSyncType.Link);
            }
        }

        public void Link(string transmitterUniqueId, bool runOnNetworkIfMultiplayer = true)
        {
            Misc.Msg("[Reciver] [Link] Linking Reciver");
            linkedToTranmitterSwithUniqueId = transmitterUniqueId;

            // Check if transmitter is on
            GameObject transmitter = WirelessSignals.transmitterSwitch.FindByUniqueId(transmitterUniqueId);
            // Check if detector is on
            GameObject detector = WirelessSignals.transmitterDetector.FindByUniqueId(transmitterUniqueId);
            if (transmitter == null && detector == null)
            {
                Misc.Msg("[Reciver] [Link] Transmitter is null");
                return;
            }
            if (transmitter != null)
            {
                Mono.TransmitterSwitch transmitterController = transmitter.GetComponent<Mono.TransmitterSwitch>();
                if (transmitterController == null)
                {
                    Misc.Msg("[Reciver] [Link] Transmitter controller is null");
                    return;
                }
                if (transmitterController.isOn == true)  // I do it manually here since SetState should update networking and this does not
                {
                    TurnOnLight();
                    isOn = true;
                    if (runOnNetworkIfMultiplayer) { SendLinkNetworkEvent(); }
                }
                else
                {
                    TurnOffLight();
                    isOn = false;
                    if (runOnNetworkIfMultiplayer) { SendLinkNetworkEvent(); }
                }
            } else if (detector != null)
            {
                Mono.TransmitterDetector detectorController = detector.GetComponent<Mono.TransmitterDetector>();
                if (detectorController == null)
                {
                    Misc.Msg("[Reciver] [Link] Detector controller is null");
                    return;
                }
                if (detectorController.isOn == true)  // I do it manually here since SetState should update networking and this does not
                {
                    TurnOnLight();
                    isOn = true;
                    if (runOnNetworkIfMultiplayer) { SendLinkNetworkEvent(); }
                }
                else
                {
                    TurnOffLight();
                    isOn = false;
                    if (runOnNetworkIfMultiplayer) { SendLinkNetworkEvent(); }
                }
            }

            UpdateDebugUi();
        }

        public void LinkWithOutUpdate(string transmitterUniqueId)
        {
            Misc.Msg("[Reciver] [LinkWithOutUpdate] Linking Reciver");
            linkedToTranmitterSwithUniqueId = transmitterUniqueId;
            UpdateDebugUi();
        }

        private void UpdateDebugUi(bool isFirst = false)
        {
            if (!isFirst && !Debug.VisualData.GetDebugMode()) { return; }
            GameObject debugUi = transform.FindChild("UI").gameObject;
            if (debugUi == null) { return; }
            Text text = debugUi.transform.FindDeepChild("Text").GetComponent<Text>();
            if (text == null) { return; }
            if (Debug.VisualData.IsAdvanced())
            {
                text.text = $"IsOn: {(isOn.HasValue ? (isOn.Value ? "True" : "False") : "null")}\r\nUniqueId: {uniqueId}\r\nLinkedToTranmitterSwithUniqueId: {linkedToTranmitterSwithUniqueId}";
            }
            else
            {
                text.text = $"IsOn: {(isOn.HasValue ? (isOn.Value ? "True" : "False") : "null")}\r\nLinked: {(string.IsNullOrEmpty(linkedToTranmitterSwithUniqueId) ? "False" : "True")}";
            }
        }

        public void SetRevertOutput(bool state, bool runOnNetworkIfMultiplayer = true)
        {
            _revertOutput = state;
            if (runOnNetworkIfMultiplayer && BoltNetwork.isRunning)
            {
                Network.Reciver.ReciverSyncEvent.SendState(gameObject.GetComponent<BoltEntity>(), Network.Reciver.ReciverSyncEvent.ReciverSyncType.RevertOuput);
            }
        }

        public void SetDebugUi(bool state)
        {
            GameObject debugUi = transform.FindChild("UI").gameObject;
            if (state)
            {
                UpdateDebugUi();
                debugUi.SetActive(true);
            }
            else { debugUi.SetActive(false); }
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Quaternion GetRotation()
        {
            return transform.rotation;
        }

        public HashSet<GameObject> GetObjectsInRange()
        {
            _objectsInRange.Clear();

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, objectRange, LayerMask.GetMask(new string[]
            {
                    "Default",
                    "Prop"
            }));
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject != null && hitCollider.gameObject != gameObject)
                {
                    // Verify object can be linked
                    if (hitCollider.transform.root != null
                        && !string.IsNullOrEmpty(hitCollider.transform.root.name))
                    {
                        if (hitCollider.transform.root.gameObject == gameObject)
                        {
                            continue;
                        }
                        string hitName = hitCollider.transform.root.name.ToLower();
                        //Misc.Msg($"Original name: {hitName}");

                        hitName = Regex.Replace(hitName, @"\s+", "");  // Remove spaces
                        //Misc.Msg($"After space removal: {hitName}");

                        hitName = Regex.Replace(hitName, @"\d", "");  // Remove numbers
                        //Misc.Msg($"After number removal: {hitName}");

                        hitName = hitName.Replace("(clone)", "");  // Remove (clone)
                        //Misc.Msg($"Final name: {hitName}");

                        bool success = UI.ReciverUI.CheckIfDictContainsKey(hitName);
                        bool success2 = UI.ReciverUI.CheckComponentDictContainsKey(hitName);
                        if (success && success2)
                        {
                            // Get expected component type from dictionary
                            string expectedComponent = UI.ReciverUI.GetComponentTypeFromDict(hitName);

                            // If no component required (null)
                            if (expectedComponent == null)
                            {
                                Misc.Msg($"[Reciver] [GetObjectsInRange] Found object: {hitName} (no component required)");
                                _objectsInRange.Add(hitCollider.transform.root.gameObject);
                            }
                            else
                            {
                                var componentType = Il2CppSystem.Type.GetType(expectedComponent);
                                // Check if we got a valid type
                                if (componentType != null)
                                {
                                    var component = hitCollider.transform.root.gameObject.GetComponent(componentType);
                                    if (component != null)
                                    {
                                        if (!_objectsInRange.Contains(hitCollider.transform.root.gameObject))
                                        {
                                            Misc.Msg($"[Reciver] [GetObjectsInRange] Found object: {hitName} with required component");
                                            _objectsInRange.Add(hitCollider.transform.root.gameObject);
                                        }
                                    }
                                    else
                                    {
                                        Misc.Msg($"[Reciver] [GetObjectsInRange] Object {hitName} missing required component: {expectedComponent}");
                                    }
                                }
                                else
                                {
                                    Misc.Msg($"[Reciver] [GetObjectsInRange] Failed to get type for component: {expectedComponent}");
                                }
                            }
                        }
                    }
                }
            }
            return _objectsInRange;
        }

        public bool IsLinkedReciverObject()
        {
            return _linkedReciverObject;
        }

        public string GetLinkedReciverObjectName()  // Alyways lowercase
        {
            return _linkedReciverObjectName;
        }

        public void SetLinkedReciverObject(bool state, string name, bool runOnNetworkIfMultiplayer = true)
        {
            _linkedReciverObject = state;
            _linkedReciverObjectName = name;
            if (runOnNetworkIfMultiplayer && BoltNetwork.isRunning)
            {
                Network.Reciver.ReciverSyncEvent.SendState(gameObject.GetComponent<BoltEntity>(), Network.Reciver.ReciverSyncEvent.ReciverSyncType.LinkedReciverObject);
            }
        }

        private void UpdateDefenseWallGate(bool onOff)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, objectRange, LayerMask.GetMask(new string[]
            {
                    "Default",
                    "Prop"
            }));
            bool found = false;
            HashSet<string> hits = new HashSet<string>();
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject != null && hitCollider.gameObject != gameObject)
                {
                    // Verify object can be linked
                    if (hitCollider.transform.root != null
                        && !string.IsNullOrEmpty(hitCollider.transform.root.name))
                    {
                        if (hitCollider.transform.root.gameObject == gameObject)  { continue; }
                        string hitName = hitCollider.transform.root.name.ToLower();
                        hitName = Regex.Replace(hitName, @"\s+", "");  // Remove spaces
                        hitName = Regex.Replace(hitName, @"\d", "");  // Remove numbers
                        hitName = hitName.Replace("(clone)", "");  // Remove (clone)
                        hits.Add(hitName);
                        if (hitName == "defensivewallgate")
                        {
                            found = true;
                            Construction.DefensiveWallGateControl defenseWallGate = hitCollider.transform.root.gameObject.GetComponent<Construction.DefensiveWallGateControl>();
                            if (defenseWallGate != null)
                            {
                                if (!_revertOutput)
                                {
                                    if (onOff)
                                    {
                                        defenseWallGate.OpenGate();
                                    }
                                    else
                                    {
                                        defenseWallGate.CloseGate();
                                    }
                                }
                                else if (_revertOutput)
                                {
                                    if (onOff)
                                    {
                                        defenseWallGate.CloseGate();
                                    }
                                    else
                                    {
                                        defenseWallGate.OpenGate();
                                    }
                                }
                            }
                            else
                            {
                                Misc.Msg("[Reciver] [UpdateDefenseWallGate] defenseWallGate is null");
                            }
                        }
                    }
                }
            }
            if (!found)
            {
                Misc.Msg("[Reciver] [UpdateDefenseWallGate] No defense wall gate found");
                // Log Hits
                string hitsString = string.Join(", ", hits);
                Misc.Msg($"[Reciver] [UpdateDefenseWallGate] Hits: {hitsString}");
            }
        }

        public void ShowScanLines(bool state, bool runOnNetworkIfMultiplayer = true)
        {
            Mono.OverlapSphereVisualizer visualizer = gameObject.GetComponent<Mono.OverlapSphereVisualizer>();
            if (!state)
            {
                if (visualizer != null)
                {
                    Destroy(visualizer);
                }
            }
            else
            {
                if (visualizer == null)
                {
                    gameObject.AddComponent<Mono.OverlapSphereVisualizer>();
                }
            }
        }

        public void SetScanObjectRange(float range, bool runOnNetworkIfMultiplayer = true)
        {
            Mono.OverlapSphereVisualizer visualizer = gameObject.GetComponent<Mono.OverlapSphereVisualizer>();
            if (visualizer != null)
            {
                visualizer.objectRange = range;
            }
            if (runOnNetworkIfMultiplayer && BoltNetwork.isRunning)
            {
                Network.Reciver.ReciverSyncEvent.SendState(gameObject.GetComponent<BoltEntity>(), Network.Reciver.ReciverSyncEvent.ReciverSyncType.ObjectRange);
            }
        }

        public bool IsScanLinesShown()
        {
            Mono.OverlapSphereVisualizer visualizer = gameObject.GetComponent<Mono.OverlapSphereVisualizer>();
            return visualizer != null;
        }

        public void OnMultiplayerAssignOwner(string inputOwnerSteamId)
        {
            if (!BoltNetwork.isRunning) { Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] BoltNetwork is not running"); return; }

            // Check if inputOwnerSteamId is valid
            if (string.IsNullOrEmpty(inputOwnerSteamId))
            {
                Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] inputOwnerSteamId is null or empty", true);
                return;
            }

            ownerSteamId = inputOwnerSteamId;

            // Check if WirelessSignals.reciver and spawnedGameObjects are initialized
            if (WirelessSignals.reciver == null)
            {
                Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] WirelessSignals.reciver is null", true);
                return;
            }

            if (WirelessSignals.reciver.spawnedGameObjects == null)
            {
                Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] WirelessSignals.reciver.spawnedGameObjects is null", true);
                return;
            }

            // Check if uniqueId exists
            if (string.IsNullOrEmpty(uniqueId))
            {
                Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] uniqueId is null, generating a new one", true);
                uniqueId = Guid.NewGuid().ToString();
            }

            // Check if the uniqueId already exists in the dictionary to avoid duplicate keys
            if (WirelessSignals.reciver.spawnedGameObjects.ContainsKey(uniqueId))
            {
                Misc.Msg($"[Reciver] [OnMultiplayerAssignOwner] uniqueId {uniqueId} already exists in dictionary", true);

                // Generate a new uniqueId to avoid conflicts
                string newUniqueId = Guid.NewGuid().ToString();
                Misc.Msg($"[Reciver] [OnMultiplayerAssignOwner] Generated new uniqueId: {newUniqueId}", true);
                uniqueId = newUniqueId;
            }

            try
            {
                // Add to list
                // Check if this uniqueId already exists
                if (WirelessSignals.reciver.spawnedGameObjects.ContainsKey(uniqueId))
                {
                    Misc.Msg($"[Reciver] [OnMultiplayerAssignOwner] uniqueId {uniqueId} already exists, updating entry", true);
                    // Update existing entry
                    WirelessSignals.reciver.spawnedGameObjects[uniqueId] = gameObject;
                }
                else
                {
                    // Add new entry
                    Misc.Msg($"[Reciver] [OnMultiplayerAssignOwner] Adding uniqueId {uniqueId} to dictionary", true);
                    WirelessSignals.reciver.spawnedGameObjects.Add(uniqueId, gameObject);
                }

                if (BoltNetwork.isServer)
                {
                    // Add to network, sync only to clients
                    if (Network.SyncLists.UniqueIdSync.Instance != null)
                    {
                        Network.SyncLists.UniqueIdSync.Instance.SendUniqueIdEvent(
                            forPrefab: Network.SyncLists.UniqueIdSync.UniqueIdListType.Reciver,
                            toDo: Network.SyncLists.UniqueIdSync.UniqueIdListOptions.Add,
                            toPlayer: Network.SyncLists.UniqueIdSync.UniqueIdTo.Clients,
                            conn: null,
                            ids: new string[] { uniqueId }
                        );
                    }
                    else
                    {
                        Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] UniqueIdSync.Instance is null", true);
                    }
                } 
                else if (BoltNetwork.isClient)
                {
                    // Add to network
                    if (Network.SyncLists.UniqueIdSync.Instance != null)
                    {
                        Network.SyncLists.UniqueIdSync.Instance.SendUniqueIdEvent(
                            forPrefab: Network.SyncLists.UniqueIdSync.UniqueIdListType.Reciver,
                            toDo: Network.SyncLists.UniqueIdSync.UniqueIdListOptions.Add,
                            toPlayer: Network.SyncLists.UniqueIdSync.UniqueIdTo.All,
                            conn: null,
                            ids: new string[] { uniqueId }
                        );
                    }
                    else
                    {
                        Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] UniqueIdSync.Instance is null", true);
                    }
                } 
                else
                {
                    Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] Sync List Network Issue", true);
                }

                

            }
            catch (System.Exception ex)
            {
                Misc.Msg($"[Reciver] [OnMultiplayerAssignOwner] Exception: {ex.Message}", true);
                return;
            }

            if (_linkUi == null && !SonsSdk.Networking.NetUtils.IsDedicatedServer)
            {
                if (Tools.CreatorSettings.lastState)  // If true, means we need to check if we are owner to create link ui
                {
                    if (Tools.CreatorSettings.IsOwner(ownerSteamId))
                    {
                        _linkUi = UI.LinkUi.CreateLinkUi(gameObject, 2f, null, Assets.UIAdjust, new Vector3(0, 0f, 0), "screen.take");
                        Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] CreatorSettings.lastState is true - Created LinkUi");
                    }
                    else
                    {
                        Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] CreatorSettings.lastState is true - Not Owner, No LinkUi");
                    }
                }
                else
                {
                    Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] CreatorSettings.lastState is false - Creating LinkUi [Everyone can change]");
                    _linkUi = UI.LinkUi.CreateLinkUi(gameObject, 2f, null, Assets.UIAdjust, new Vector3(0, 0f, 0), "screen.take");
                }
            }

            if (BoltNetwork.isServer || BoltNetwork.isClient)
            {
                BoltEntity bolt = gameObject.GetComponent<BoltEntity>();
                if (bolt != null)
                {
                    // Delete NetworkOwner Component On Rest Of Network
                    Network.Reciver.ReciverSyncEvent.SendState(bolt, Network.Reciver.ReciverSyncEvent.ReciverSyncType.RemoveFromBoltEntity);
                    Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] Sent Sync [RemoveFromBoltEntity] Event", true);

                    // Sync To Network (UniqueId, OwnerSteamId)
                    Network.Reciver.ReciverSyncEvent.SendState(bolt, Network.Reciver.ReciverSyncEvent.ReciverSyncType.UniqueId);
                    Network.Reciver.ReciverSyncEvent.SendState(bolt, Network.Reciver.ReciverSyncEvent.ReciverSyncType.OwnerSteamId);
                    Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] Sent Sync [UniqueId, OwnerSteamId] Event", true);

                    Network.Reciver.ReciverSyncEvent.SendState(bolt, Network.Reciver.ReciverSyncEvent.ReciverSyncType.LinkUiSync);
                    Misc.Msg("[Reciver] [OnMultiplayerAssignOwner] Sent Sync [LinkUiSync] Event", true);
                }
                else
                {
                    RLog.Error("[Reciver] [OnMultiplayerAssignOwner] BoltEntity is null");
                }

                LoadInDelay().RunCoro();  // Set loaded in
            }
            else
            {
                RLog.Error("[Reciver] [OnMultiplayerAssignOwner] Can't assign owner on unknown Network");
            }
        }

        public void SetLinkUi(bool onlyOwner)
        {
            if (_linkUi == null && !SonsSdk.Networking.NetUtils.IsDedicatedServer)
            {
                if (onlyOwner)  // If true, means we need to check if we are owner to create link ui
                {
                    if (Tools.CreatorSettings.IsOwner(ownerSteamId))
                    {
                        _linkUi = UI.LinkUi.CreateLinkUi(gameObject, 2f, null, Assets.UIAdjust, new Vector3(0, 0f, 0), "screen.take");
                        Misc.Msg("[Reciver] [SetLinkUi] CreatorSettings.lastState is true - Created LinkUi");
                    }
                    else
                    {
                        Misc.Msg("[Reciver] [SetLinkUi] CreatorSettings.lastState is true - Not Owner, No LinkUi");
                    }
                }
                else
                {
                    Misc.Msg("[Reciver] [SetLinkUi] CreatorSettings.lastState is false - Creating LinkUi [Everyone can change]");
                    _linkUi = UI.LinkUi.CreateLinkUi(gameObject, 2f, null, Assets.UIAdjust, new Vector3(0, 0f, 0), "screen.take");
                }
            }
        }
    }
}
