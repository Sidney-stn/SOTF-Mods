using RedLoader;
using Sons.Inventory;
using SonsSdk;
using System.Collections;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WirelessSignals.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class TransmitterDetector : MonoBehaviour
    {
        public string uniqueId;
        public bool? isOn = false;
        public bool isSetupPrefab;
        public HashSet<string> linkedUniqueIdsRecivers = new HashSet<string>();
        public string ownerSteamId = null;  // If owner is null, then it's public

        public bool foundObject;
        public string foundObjectName;
        private GameObject _lineGameObject;
        private bool addedRepairToolEquip = false;
        private bool addedRepairToolUnequip = false;
        private bool isSearching = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Start()
        {
            if (isSetupPrefab)
            {
                return;
            }
            if (string.IsNullOrEmpty(uniqueId))
            {
                RLog.Warning("[Mono] [TransmitterDetector]: uniqueId is null! Shound Never be");
                SonsTools.ShowMessage("Something went wrong when placing down structure!");
            }

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

            TryFindObject();
            UpdateDebugUi(true);
            if (Debug.VisualData.GetDebugMode())
            {
                SetDebugUi(true);
            }

        }

        private void TryFindObject()
        {
            if (foundObject) { return; }

            Transform transform = gameObject.transform;
            float range = 1f;
            float squareSize = 0.3f;
            int gridDensity = 3;

            // Create LineRenderers if visualization is enabled
            List<LineRenderer> lineRenderers = new List<LineRenderer>();
            if (Config.VisualRayCast.Value)
            {

                GameObject lineContainer = new GameObject("RaycastLines");
                Debug.RayCast.gameObjects.Add(lineContainer);

                //lineContainer.transform.parent = transform;

                // Create a LineRenderer for each ray
                for (int i = 0; i < gridDensity * gridDensity; i++)
                {
                    GameObject lineObj = new GameObject($"RayLine_{i}");
                    lineObj.transform.parent = lineContainer.transform;

                    LineRenderer line = lineObj.AddComponent<LineRenderer>();
                    line.material = WirelessSignals.redMat;
                    line.startWidth = 0.02f;
                    line.endWidth = 0.02f;
                    line.positionCount = 2;

                    lineRenderers.Add(line);
                }
            }
            int lineIndex = 0;
            for (int x = 0; x < gridDensity; x++)
            {
                for (int y = 0; y < gridDensity; y++)
                {
                    float xOffset = (x - (gridDensity - 1) / 2f) * (squareSize / (gridDensity - 1));
                    float yOffset = (y - (gridDensity - 1) / 2f) * (squareSize / (gridDensity - 1));

                    Vector3 rayStart = transform.position +
                                     transform.right * xOffset +
                                     transform.up * yOffset;

                    RaycastHit raycastHit;
                    bool hitSomething = Physics.Raycast(rayStart, transform.forward, out raycastHit, range, LayerMask.GetMask("Default"));

                    // Visualize raycast if enabled
                    if (Config.VisualRayCast.Value)
                    {
                        LineRenderer line = lineRenderers[lineIndex];
                        line.SetPosition(0, rayStart);

                        // Set end position based on whether we hit something
                        if (hitSomething)
                        {
                            line.SetPosition(1, raycastHit.point);
                            line.material = WirelessSignals.blackMat; // Green for hits
                        }
                        else
                        {
                            line.SetPosition(1, rayStart + transform.forward * range);
                            line.material = WirelessSignals.redMat; // Red for misses
                        }
                        lineIndex++;
                    }

                    if (hitSomething &&
                        raycastHit.collider != null &&
                        raycastHit.collider.transform.root != null &&
                        !string.IsNullOrEmpty(raycastHit.collider.transform.root.name))
                    {
                        GameObject open = raycastHit.collider.transform.root.gameObject;
                        if (open == null) // If No Objects Found Enter Scanning Mode
                        {
                            // Enable Scanning Mode
                            ContinuesObjectSearch().RunCoro();
                            Misc.Msg("[Mono] [TransmitterDetector] [TryFindObject] No Objects Found - Scanning Mode Enabled");
                            return;
                        }
                        string name = raycastHit.collider.transform.root.name;
                        if (name.Contains("StorageLogsStructure"))
                        {
                            foundObject = true;
                            foundObjectName = "StorageLogsStructure";
                            isSearching = false;
                            Misc.Msg("[Mono] [TransmitterDetector] [TryFindObject] Found StorageLogsStructure");
                            SonsTools.ShowMessage("Found StorageLogsStructure - Detecting", 5);
                            CheckSimpleStorage(open, 6, "LogLayout").RunCoro();
                            SetupLinkedLines(open.transform.position);
                            return;
                        }
                        else if (name.Contains("StorageLogsLargeStructure"))
                        {
                            foundObject = true;
                            foundObjectName = "StorageLogsLargeStructure";
                            isSearching = false;
                            Misc.Msg("[Mono] [TransmitterDetector] [TryFindObject] Found StorageLogsLargeStructure");
                            SonsTools.ShowMessage("Found StorageLogsLargeStructure - Detecting", 5);
                            CheckSimpleStorage(open, 24, "LogLayout").RunCoro();
                            SetupLinkedLines(open.transform.position);
                            return;
                        }
                        else if (name.Contains("StorageStonesStructure"))
                        {
                            foundObject = true;
                            foundObjectName = "StorageStonesStructure";
                            isSearching = false;
                            Misc.Msg("[Mono] [TransmitterDetector] [TryFindObject] Found StorageStonesStructure");
                            SonsTools.ShowMessage("Found StorageStonesStructure - Detecting", 5);
                            CheckSimpleStorage(open, 28, "StoneLayout").RunCoro();
                            SetupLinkedLines(open.transform.position);
                            return;
                        }
                        else if (name.Contains("StorageSticksStructure"))
                        {
                            foundObject = true;
                            foundObjectName = "StorageSticksStructure";
                            isSearching = false;
                            Misc.Msg("[Mono] [TransmitterDetector] [TryFindObject] Found StorageSticksStructure");
                            SonsTools.ShowMessage("Found StorageSticksStructure - Detecting", 5);
                            CheckSimpleStorage(open, 30, "StickLayout").RunCoro();
                            SetupLinkedLines(open.transform.position);
                            return;
                        }
                        else if (name.Contains("StorageRocksStructure"))
                        {
                            foundObject = true;
                            foundObjectName = "StorageRocksStructure";
                            isSearching = false;
                            Misc.Msg("[Mono] [TransmitterDetector] [TryFindObject] Found StorageRocksStructure");
                            SonsTools.ShowMessage("Found StorageRocksStructure - Detecting", 5);
                            CheckSimpleStorage(open, 17, "RockLayout").RunCoro();
                            SetupLinkedLines(open.transform.position);
                            return;
                        }
                        else if (name.Contains("StorageBonesStructure"))
                        {
                            foundObject = true;
                            foundObjectName = "StorageBonesStructure";
                            isSearching = false;
                            Misc.Msg("[Mono] [TransmitterDetector] [TryFindObject] Found StorageBonesStructure");
                            SonsTools.ShowMessage("Found StorageBonesStructure - Detecting", 5);
                            CheckSimpleStorage(open, 50, "BoneLayout").RunCoro();
                            SetupLinkedLines(open.transform.position);
                            return;
                        }
                        else if (name.Contains("FishTrapStructure"))
                        {
                            foundObject = true;
                            foundObjectName = "FishTrapStructure";
                            isSearching = false;
                            Misc.Msg("[Mono] [TransmitterDetector] [TryFindObject] Found FishTrapStructure");
                            SonsTools.ShowMessage("Found FishTrapStructure - Detecting", 5);
                            CheckSimpleStorage(open, 5, "FishLayout").RunCoro();
                            SetupLinkedLines(open.transform.position);
                            return;
                        } else
                        {
                            Misc.Msg("[Mono] [TransmitterDetector] [TryFindObject] Found Unknown Object");
                            // Enable Scanning Mode
                            ContinuesObjectSearch().RunCoro();
                        }

                    }

                }
            }
        }

        private IEnumerator ContinuesObjectSearch()
        {
            if (isSearching)  // Kill if already searching
            {
                yield break;
            }
            if (!isSearching && !foundObject)  // Only Run If Not Already Searching And Object Is Not Found
            {
                isSearching = true;
            }
            
            if (foundObject)  // Only Run If Object Is Not Found
            {
                yield break;
            }
            yield return new WaitForSeconds(5f);
            while (true)
            {
                if (foundObject)
                {
                    yield break;
                }
                TryFindObject();
                if (Vector3.Distance(LocalPlayer.GameObject.transform.position, transform.position) <= 50f)
                {
                    yield return new WaitForSeconds(30f);
                } else
                {
                    yield return new WaitForSeconds(5f);
                }
                    
            }
        }

        private IEnumerator CheckSimpleStorage(GameObject obj, int maxInStorage, string layoutName)
        {
            int maxLogs = maxInStorage;
            string layoutGroupName = $"{layoutName}Group";
            string itemLayoutGroupName = $"{layoutName}Item";
            GameObject checkObj = obj;
            if (checkObj == null) { 
                Misc.Msg("[Mono] [TransmitterDetector] [CheckSimpleStorage] CheckObj is Null");
                // This will trigger if object is destroyed
                foundObject = false;
                foundObjectName = null;
                if (isOn == true)  // ONLY DISABLE IF NOT ALREADY DISABLED
                {
                    SetState(false);
                }
                Destroy(_lineGameObject);  // Destory LineRenderer if object is destroyed
                ContinuesObjectSearch().RunCoro();
                UpdateDebugUi();
                yield break;
            }
            UpdateDebugUi(true);
            Misc.Msg("[Mono] [TransmitterDetector] [CheckSimpleStorage] Start Checking Item");
            if (!foundObject) { Misc.Msg("[Mono] [TransmitterDetector] [CheckSimpleStorage] Retuned, found object is false"); }
            while (foundObject)
            {
                Misc.Msg("[Mono] [TransmitterDetector] [CheckSimpleStorage] While Loop - Checking Item");
                if (!foundObject) { Misc.Msg("[Mono] [TransmitterDetector] [CheckSimpleStorage] Retuned, found object is false"); yield break; }
                if (checkObj == null) { Misc.Msg("[Mono] [TransmitterDetector] [CheckSimpleStorage] While Loop - CheckObj is Null"); yield break; }
                GameObject itemLayoutGroup = checkObj.transform.FindChild(layoutGroupName).gameObject;
                if (itemLayoutGroup == null) { Misc.Msg("[Mono] [TransmitterDetector] [CheckSimpleStorage] While Loop - LayoutGroup is Null"); yield break; }

                List<Transform> transforms = itemLayoutGroup.transform.GetChildren();
                int activeLogs = 0;
                for (int i = 0; i < transforms.Count; i++)
                {
                    Transform log = transforms[i];
                    if (log == null) { Misc.Msg("[Mono] [TransmitterDetector] [CheckSimpleStorage] While Loop - Log is Null"); yield break; }
                    if (log.gameObject.active)
                    {
                        if (log.gameObject.name.Contains(itemLayoutGroupName))
                        {
                            activeLogs++;
                        }
                    }
                }

                if (activeLogs >= maxLogs)  // If All Logs Are Active - ENABLE SIGNAL
                {
                    if (isOn == false)  // ONLY ENABLE IF NOT ALREADY ENABLED
                    {
                        SetState(true);
                    }
                }
                else  // If Not All Logs Are Active - DISABLE SIGNAL
                {
                    if (isOn == true)  // ONLY DISABLE IF NOT ALREADY DISABLED
                    {
                        SetState(false);
                    }
                }

                if (LocalPlayer.GameObject == null)
                {
                    Misc.Msg("[Mono] [TransmitterDetector] [CheckLogs] While Loop - LocalPlayer.GameObject is Null");
                    yield break;
                }
                if (LocalPlayer.GameObject.transform == null)
                {
                    Misc.Msg("[Mono] [TransmitterDetector] [CheckLogs] While Loop - LocalPlayer.GameObject.transform is Null");
                    yield break;
                }

                // Wait for set time
                float distanceToObj = Vector3.Distance(LocalPlayer.GameObject.transform.position, transform.position);
                if (distanceToObj <= 10f)
                {
                    if (maxLogs < 8)
                    {
                        yield return new WaitForSeconds(1f);  // If Player Is Close To The Structure, This Will Be Called Every Second
                    }
                    else
                    {
                        yield return new WaitForSeconds(3f);  // If Player Is Close To The Structure, This Will Be Called Every 30 Seconds
                    }
                } else if (distanceToObj <= 50f)
                {
                    yield return new WaitForSeconds(10f);  // If Player Is Close To The Structure, This Will Be Called Every 10 Seconds
                }
                else
                {
                    yield return new WaitForSeconds(30f);  // If Player Is Far From The Structure, This Will Be Called Every 30 Seconds
                }

            }
        }

        private void OnItemEquipped(ItemInstance item, int slotIndex)
        {
            if (slotIndex == 422 && _lineGameObject != null)
            {
                Misc.Msg("[TransmitterDetector] [OnItemEquipped] RepairToolInHand Equipped");
                _lineGameObject.SetActive(true);
            }
        }

        private void OnItemUnequipped(ItemInstance item, int slotIndex)
        {
            if (slotIndex == 422 && _lineGameObject != null)
            {
                Misc.Msg("[TransmitterDetector] [OnItemUnequipped] RepairToolInHand Unequipped");
                _lineGameObject.SetActive(false);
            }
        }

        private void SetupLinkedLines(Vector3 attatchedObjectPos)
        {
            Misc.Msg("[TransmitterDetector] [SetupLinkedLines] Setting Up Linked Lines");
            // Subscribing to the OnItemUnequippedEvent
            if (Linking.RepairToolInHand.playerInventoryInstance.OnItemUnequippedEvent != null && !addedRepairToolUnequip)
            {
                Linking.RepairToolInHand.playerInventoryInstance.OnItemUnequippedEvent.AddListener((UnityAction<ItemInstance, int>)OnItemUnequipped);
                addedRepairToolUnequip = true;
            }

            // Subscribing to the OnItemEquippedEvent
            if (Linking.RepairToolInHand.playerInventoryInstance.OnItemEquippedEvent != null && !addedRepairToolEquip)
            {
                Linking.RepairToolInHand.playerInventoryInstance.OnItemEquippedEvent.AddListener((UnityAction<ItemInstance, int>)OnItemEquipped);
                addedRepairToolEquip = true;
            }

            if (Linking.RepairToolInHand.playerInventoryInstance.OnItemEquippedEvent != null && Linking.RepairToolInHand.playerInventoryInstance.OnItemUnequippedEvent != null)
            {
                if (_lineGameObject != null)
                {
                    DestroyImmediate(_lineGameObject);
                }
                _lineGameObject = new GameObject("LineRenderer");
                _lineGameObject.SetParent(transform);

                // Add LineRenderer component
                UnityEngine.LineRenderer line = _lineGameObject.AddComponent<UnityEngine.LineRenderer>();

                // Set material and basic properties
                line.material = WirelessSignals.redMat;
                line.positionCount = 2;
                line.startWidth = 0.07f;
                line.endWidth = 0.07f;

                // Set start and end positions
                line.SetPosition(0, gameObject.transform.position);
                line.SetPosition(1, attatchedObjectPos);

                // Optional quality settings
                line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                line.receiveShadows = false;

                Misc.Msg("[TransmitterDetector] [SetupLinkedLines] LineRenderer Setup Complete");


                // Set LineRenderer visibility
                if (LocalPlayer.Inventory != null && LocalPlayer.Inventory.RightHandItem != null &&
                    LocalPlayer.Inventory.RightHandItem._itemID == 422)
                {
                    _lineGameObject.SetActive(true);
                }
                else
                {
                    _lineGameObject.SetActive(false);
                }
            }
        }

        private float DistanceToPlayer()
        {
            if (LocalPlayer.GameObject == null) { return 0; }
            if (LocalPlayer.GameObject.transform == null) { return 0; }
            return Vector3.Distance(LocalPlayer.GameObject.transform.position, transform.position);
        }

        public void TurnOnLight()
        {
            GameObject lights = transform.FindChild("Light").gameObject;
            if (lights != null)
            {
                lights.SetActive(true);
            }
            else
            {
                Misc.Msg("[TransmitterDetector] [TurnOnLight] Can't Turn On Lights, Null!");
            }
        }
        public void TurnOffLight()
        {
            GameObject lights = transform.FindChild("Light").gameObject;
            if (lights != null)
            {
                lights.SetActive(false);
            }
            else
            {
                Misc.Msg("[TransmitterDetector] [TurnOffLight] Can't Turn Off Lights, Null!");
            }
        }


        public void SetState(bool state)
        {
            Misc.Msg($"[TransmitterDetector] [SetState] Setting State To: {state}");
            isOn = state;
            if (state)
            {
                TurnOnLight();
                Misc.Msg("[TransmitterDetector] [Toggle] Turned On");
            }
            else
            {
                TurnOffLight();
                Misc.Msg("[TransmitterDetector] [Toggle] Turned Off");
            }
            // Update All Recivers Linked
            foreach (var reciverUniqueId in linkedUniqueIdsRecivers)
            {
                var reciver = WirelessSignals.reciver.FindByUniqueId(reciverUniqueId);
                if (reciver)
                {
                    reciver.GetComponent<Reciver>().SetState((bool)isOn);  // Set Reciver State (bool) should always work since the state is set above and can never be null
                    Misc.Msg("[TransmitterDetector] [Toggle] Reciver Updated");
                }
                else
                {
                    Misc.Msg("[TransmitterDetector] [Toggle] Reciver Not Found");
                }
            }
            UpdateDebugUi();
        }

        public void UnlinkReciver(string reciverUniqueId)
        {
            if (linkedUniqueIdsRecivers.Contains(reciverUniqueId))
            {
                linkedUniqueIdsRecivers.Remove(reciverUniqueId);
                Misc.Msg("[TransmitterDetector] [UnlinkReciver] Reciver Unlinked");
            }
            else
            {
                Misc.Msg("[TransmitterDetector] [UnlinkReciver] Reciver Not Found");
            }
            UpdateDebugUi();
        }

        public void LinkReciver(string reciverUniqueId)
        {
            if (!linkedUniqueIdsRecivers.Contains(reciverUniqueId))
            {
                linkedUniqueIdsRecivers.Add(reciverUniqueId);
                Misc.Msg("[TransmitterDetector] [LinkReciver] Reciver Linked");
            }
            else
            {
                Misc.Msg("[TransmitterDetector] [LinkReciver] Reciver Already Linked");
            }
            UpdateDebugUi();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void OnDisable()
        {
            if (_lineGameObject != null)
            {
                Destroy(_lineGameObject);
            }

            // Unsubscribing from the OnItemUnequippedEvent
            if (Linking.RepairToolInHand.playerInventoryInstance.OnItemUnequippedEvent != null)
            {
                Linking.RepairToolInHand.playerInventoryInstance.OnItemUnequippedEvent.RemoveListener((UnityAction<ItemInstance, int>)OnItemUnequipped);
            }

            // Unsubscribing from the OnItemEquippedEvent
            if (Linking.RepairToolInHand.playerInventoryInstance.OnItemEquippedEvent != null)
            {
                Linking.RepairToolInHand.playerInventoryInstance.OnItemEquippedEvent.RemoveListener((UnityAction<ItemInstance, int>)OnItemEquipped);
            }

            // Unlink All Recivers
            foreach (var reciverUniqueId in linkedUniqueIdsRecivers)
            {
                var reciver = WirelessSignals.reciver.FindByUniqueId(reciverUniqueId);
                if (reciver)
                {
                    reciver.GetComponent<Reciver>().SetState(false);
                }
            }

        }

        private void UpdateDebugUi(bool isFirst = false)
        {
            if (!isFirst && !Debug.VisualData.GetDebugMode()) { return; }
            Misc.Msg("[TransmitterDetector] [Mono] [UpdateDebugUi] Updating Debug Ui");

            if (transform == null) { Misc.Msg("[TransmitterDetector] [Mono] [UpdateDebugUi] Main Transfrom == null"); return; }

            GameObject debugUi = transform.FindChild("UI")?.gameObject;
            if (debugUi == null) { Misc.Msg("[TransmitterDetector] [Mono] [UpdateDebugUi] Ui == null"); return; }

            Transform canvas = debugUi.transform.FindChild("Canvas");
            if (canvas == null) { Misc.Msg("[TransmitterDetector] [Mono] [UpdateDebugUi] Canvas == null"); return; }

            Transform textTransform = canvas.FindChild("Text");
            if (textTransform == null) { Misc.Msg("[TransmitterDetector] [Mono] [UpdateDebugUi] TextTransform == null"); return; }

            Text text = textTransform.GetComponent<Text>();
            if (text == null) { Misc.Msg("[TransmitterDetector] [Mono] [UpdateDebugUi] Text Comp == null"); return; }

            if (Debug.VisualData.IsAdvanced())
            {
                text.text = $"IsOn: {(isOn.HasValue ? (isOn.Value ? "True" : "False") : "null")}\r\nUniqueId: {uniqueId}\r\nLinkedUniqueIdsRecivers: {string.Join(", ", linkedUniqueIdsRecivers)}\r\nFoundObject: {foundObject}\r\nFoundObjectName: {foundObjectName}";
            }
            else
            {
                text.text = $"IsOn: {(isOn.HasValue ? (isOn.Value ? "True" : "False") : "null")}\r\nLinked To Count: {linkedUniqueIdsRecivers.Count}\r\nFoundObject: {foundObject}\r\nFoundObjectName: {foundObjectName}";
            }
        }
        

        public void SetDebugUi(bool state)
        {
            GameObject debugUi = transform.FindChild("UI").gameObject;
            if (debugUi == null) { Misc.Msg("[TransmitterDetector] [Mono] [SetDebugUi] Ui == null"); return; }
            if (state)
            {
                UpdateDebugUi();
                debugUi.SetActive(true);
            } else { debugUi.SetActive(false); }
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Quaternion GetRotation()
        {
            return transform.rotation;
        }
    }
}
