using RedLoader;
using Sons.Gui;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;

namespace WirelessSignals.Linking
{
    [RegisterTypeInIl2Cpp]
    internal class LineRenderer : MonoBehaviour
    {
        public bool shouldUpdateRun = false;
        public Dictionary<string, GameObject> hitObjects = new Dictionary<string, GameObject>();  // Last Hit Object
        public Dictionary<string, GameObject> lineRenderers = new Dictionary<string, GameObject>();  // ReciverUnqiueId, LineRenderer
        private Material lineMaterial;
        private UnityEngine.LineRenderer activeLineRenderer = null;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Most Likley to be used later")]
        private bool isRepairToolInHand = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Awake Do Get Called When Script Is Added To Go")]
        private void Awake()
        {
            if (lineMaterial != null) { return; }
            lineMaterial = Assets.TransmitterSwitch.transform.GetChild(0).GetChild(16).GetChild(0).GetComponent<MeshRenderer>().materials[0];
        }

        public void HitReciver(GameObject go)
        {
            if (go == null)
            {
                Misc.Msg("[LineRenderer] [HitReciver]: GameObject is null");
                return;
            }

            // If neither transmitter switch nor detector hit is stored, unlink the receiver
            if (!hitObjects.ContainsKey("TransmitterSwitch") && !hitObjects.ContainsKey("TransmitterDetector"))
            {
                // If CreatorSettings.lastState Is True, Only Owner Can Change The Linking
                if (Tools.CreatorSettings.lastState && !Tools.CreatorSettings.IsOwner(go.GetComponent<Mono.Reciver>().ownerSteamId))
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: You Must Be The Owner To Link");
                    SonsTools.ShowMessage("You Must Be The Owner Edit Linking");
                    hitObjects.Clear();
                    return;
                }

                hitObjects.Clear();  // Clear if it has another Receiver Stored
                hitObjects["Reciver"] = go;  // Store the receiver hit

                // Unlink the receiver
                Mono.Reciver controller = go.GetComponent<Mono.Reciver>();
                if (controller == null) { Misc.Msg("[LineRenderer] [HitReciver]: Reciver controller is null"); return; }
                if (string.IsNullOrEmpty(controller.linkedToTranmitterSwithUniqueId)) { 
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver linkedToTranmitterSwithUniqueId is null");
                    SonsTools.ShowMessage("Reciver Aready Unlinked");
                    return;
                }

                // Try to find the linked transmitter (either switch or detector)
                GameObject wirelessTransmitter = WirelessSignals.transmitterSwitch.FindByUniqueId(controller.linkedToTranmitterSwithUniqueId);
                GameObject wirelessDetector = WirelessSignals.transmitterDetector.FindByUniqueId(controller.linkedToTranmitterSwithUniqueId);

                if (wirelessTransmitter == null && wirelessDetector == null) // If neither transmitter is found by uniqueId Stored, unlink
                {
                    controller.Unlink();  // Unlink Receiver
                    Misc.Msg("[LineRenderer] [HitReciver]: No linked transmitter or detector found");
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver unlinked");
                    SonsTools.ShowMessage("Receiver Successfully Unlinked");
                    // Destroy LineRenderer
                    if (lineRenderers.ContainsKey(controller.uniqueId))
                    {
                        Destroy(lineRenderers[controller.uniqueId]);
                        lineRenderers.Remove(controller.uniqueId);
                    }
                }
                else if (wirelessTransmitter != null)  // Unlink from TransmitterSwitch
                {
                    controller.Unlink();
                    Mono.TransmitterSwitch transmitterController = wirelessTransmitter.GetComponent<Mono.TransmitterSwitch>();
                    if (transmitterController == null) { Misc.Msg("[LineRenderer] [HitReciver]: Transmitter controller is null"); return; }
                    transmitterController.UnlinkReciver(controller.uniqueId);
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver and Transmitter Switch unlinked");
                    SonsTools.ShowMessage("Receiver Successfully Unlinked from Transmitter");
                    // Destroy LineRenderer
                    if (lineRenderers.ContainsKey(controller.uniqueId))
                    {
                        Destroy(lineRenderers[controller.uniqueId]);
                        lineRenderers.Remove(controller.uniqueId);
                    }
                }
                else if (wirelessDetector != null)  // Unlink from TransmitterDetector
                {
                    controller.Unlink();
                    Mono.TransmitterDetector detectorController = wirelessDetector.GetComponent<Mono.TransmitterDetector>();
                    if (detectorController == null) { Misc.Msg("[LineRenderer] [HitReciver]: Detector controller is null"); return; }
                    detectorController.UnlinkReciver(controller.uniqueId);
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver and Detector unlinked");
                    SonsTools.ShowMessage("Receiver Successfully Unlinked from Detector");
                    // Destroy LineRenderer
                    if (lineRenderers.ContainsKey(controller.uniqueId))
                    {
                        Destroy(lineRenderers[controller.uniqueId]);
                        lineRenderers.Remove(controller.uniqueId);
                    }
                }
                return;
            }
            else if (hitObjects.ContainsKey("TransmitterSwitch"))  // If transmitter switch hit is stored, link the receiver
            {
                // Link the receiver
                Mono.Reciver controller = go.GetComponent<Mono.Reciver>();
                if (controller == null)
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver controller is null");
                    return;
                }
                // If CreatorSettings.lastState Is True, Only Owner Can Change The Linking
                if (Tools.CreatorSettings.lastState && !Tools.CreatorSettings.IsOwner(controller.ownerSteamId))
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: You Must Be The Owner To Link");
                    SonsTools.ShowMessage("You Must Be The Owner Edit Linking");
                    hitObjects.Clear();
                    DestoryActiveLineRenderer();
                    shouldUpdateRun = false;
                    return;
                }

                // Check If Reciver Is Already Linked
                if (!string.IsNullOrEmpty(controller.linkedToTranmitterSwithUniqueId))
                {
                    shouldUpdateRun = false;
                    hitObjects.Clear(); // Clear the stored hit objects
                    DestoryActiveLineRenderer();
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver Already Linked");
                    SonsTools.ShowMessage("Receiver Already Linked");
                    return;
                }

                Mono.TransmitterSwitch transmitterController = hitObjects["TransmitterSwitch"].GetComponent<Mono.TransmitterSwitch>();
                if (transmitterController == null)
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: Transmitter controller is null");
                    return;
                }
                // If CreatorSettings.lastState Is True, Only Owner Can Change The Linking
                if (Tools.CreatorSettings.lastState && !Tools.CreatorSettings.IsOwner(transmitterController.ownerSteamId))
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: You Must Be The Owner To Link");
                    SonsTools.ShowMessage("You Must Be The Owner Edit Linking");
                    hitObjects.Clear();
                    shouldUpdateRun = false;
                    DestoryActiveLineRenderer();
                    return;
                }
                if (transmitterController.linkedUniqueIdsRecivers.Contains(controller.uniqueId))  // If receiver is already linked to transmitter switch
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver Already Linked");
                    SonsTools.ShowMessage("Receiver Already Linked");
                    if (controller.linkedToTranmitterSwithUniqueId != transmitterController.uniqueId)
                    {
                        // Fix Linking In Case Something Have Gone Wrong
                        controller.linkedToTranmitterSwithUniqueId = transmitterController.uniqueId;
                    }
                    shouldUpdateRun = false;
                    hitObjects.Clear(); // Clear the stored hit objects
                    DestoryActiveLineRenderer();
                    return;
                }
                else // Link Receiver, Case Cant Be Triggerd If Not LastState Is False or Owner is true
                {
                    shouldUpdateRun = false;
                    CreateLineRenderer(activeLineRenderer.GetPosition(0), activeLineRenderer.GetPosition(1), true, controller.uniqueId);
                    DestoryActiveLineRenderer();
                    transmitterController.LinkReciver(controller.uniqueId);
                    controller.Link(transmitterController.uniqueId);
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver Linked");
                    SonsTools.ShowMessage("Receiver Successfully Linked - Link Point 2/2");
                }

                hitObjects.Clear(); // Clear the stored hit objects
                hitObjects["Reciver"] = go;  // Store the receiver hit
            }
            else if (hitObjects.ContainsKey("TransmitterDetector"))  // If transmitter detector hit is stored, link the receiver
            {
                // Link the receiver
                Mono.Reciver controller = go.GetComponent<Mono.Reciver>();

                if (controller == null)
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver controller is null");
                    hitObjects.Clear(); // Clear the stored hit objects
                    hitObjects["Reciver"] = go;  // Store the receiver hit
                    return;
                }

                // If CreatorSettings.lastState Is True, Only Owner Can Change The Linking
                if (Tools.CreatorSettings.lastState && !Tools.CreatorSettings.IsOwner(controller.ownerSteamId))
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: You Must Be The Owner To Link");
                    SonsTools.ShowMessage("You Must Be The Owner Edit Linking");
                    hitObjects.Clear();
                    DestoryActiveLineRenderer();
                    shouldUpdateRun = false;
                    return;
                }

                if (string.IsNullOrEmpty(controller.uniqueId))
                {
                    RLog.Warning("[LineRenderer] [HitReciver]: Reciver UniqueId is null");
                    hitObjects.Clear(); // Clear the stored hit objects
                    hitObjects["Reciver"] = go;  // Store the receiver hit
                    return;
                }

                // Check If Reciver Is Already Linked
                if (!string.IsNullOrEmpty(controller.linkedToTranmitterSwithUniqueId))
                {
                    shouldUpdateRun = false;
                    hitObjects.Clear(); // Clear the stored hit objects
                    DestoryActiveLineRenderer();
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver Already Linked");
                    SonsTools.ShowMessage("Receiver Already Linked");
                    return;
                }

                Mono.TransmitterDetector transmitterController = hitObjects["TransmitterDetector"].GetComponent<Mono.TransmitterDetector>();
                if (transmitterController == null)
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: TransmitterDetector controller is null");
                    return;
                }
                // If CreatorSettings.lastState Is True, Only Owner Can Change The Linking
                if (Tools.CreatorSettings.lastState && !Tools.CreatorSettings.IsOwner(transmitterController.ownerSteamId))
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: You Must Be The Owner To Link");
                    SonsTools.ShowMessage("You Must Be The Owner Edit Linking");
                    hitObjects.Clear();
                    DestoryActiveLineRenderer();
                    shouldUpdateRun = false;
                    return;
                }

                if (transmitterController.linkedUniqueIdsRecivers.Contains(controller.uniqueId))  // If receiver is already linked to transmitter detector
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver Already Linked");
                    SonsTools.ShowMessage("Receiver Already Linked");
                    if (controller.linkedToTranmitterSwithUniqueId != transmitterController.uniqueId)
                    {
                        // Fix Linking In Case Something Have Gone Wrong
                        //controller.linkedToTranmitterSwithUniqueId = transmitterController.uniqueId;
                        controller.LinkWithOutUpdate(transmitterController.uniqueId);
                    }
                    shouldUpdateRun = false;
                    hitObjects.Clear(); // Clear the stored hit objects
                    DestoryActiveLineRenderer();
                    return;
                }
                else // Link Receiver
                {
                    shouldUpdateRun = false;
                    CreateLineRenderer(activeLineRenderer.GetPosition(0), activeLineRenderer.GetPosition(1), true, controller.uniqueId);
                    DestoryActiveLineRenderer();
                    transmitterController.LinkReciver(controller.uniqueId);
                    controller.Link(transmitterController.uniqueId);
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver Linked to Detector");
                    SonsTools.ShowMessage("Receiver Successfully Linked to Detector - Link Point 2/2");
                }

                hitObjects.Clear(); // Clear the stored hit objects
                hitObjects["Reciver"] = go;  // Store the receiver hit
            }
        }

        public void HitTransmitterSwitch(GameObject go)
        {
            if (go == null)
            {
                Misc.Msg("[LineRenderer] [HitTransmitterSwitch]: GameObject is null");
                return;
            }
            // If CreatorSettings.lastState Is True, Only Owner Can Change The Linking
            if (Tools.CreatorSettings.lastState && !Tools.CreatorSettings.IsOwner(go.GetComponent<Mono.TransmitterSwitch>().ownerSteamId))
            {
                Misc.Msg("[LineRenderer] [HitTransmitterSwitch]: You Must Be The Owner To Link");
                SonsTools.ShowMessage("You Must Be The Owner Edit Linking");
                hitObjects.Clear();
                return;
            }
            // If nothing is stored, store the transmitter switch hit
            if (!hitObjects.ContainsKey("TransmitterSwitch") && !hitObjects.ContainsKey("TransmitterDetector"))
            {
                hitObjects.Clear();  // Clear iF it has another Transmitter Switch Stored
                hitObjects["TransmitterSwitch"] = go;  // Store the transmitter switch hit
                Misc.Msg("[LineRenderer] [HitTransmitterSwitch]: Transmitter Switch Hit - Link Point 1/2");
                SonsTools.ShowMessage("Transmitter Switch Hit - Link Point 1/2");
                CreateActiveLineRenderer(go.transform.position);  // Create Active Line Renderer
                shouldUpdateRun = true;
                return;
            }
            else if (hitObjects.ContainsKey("TransmitterSwitch"))  // If transmitter switch hit is stored, stop the linking
            {
                Misc.Msg("[LineRenderer] [HitTransmitterSwitch]: Transmitter Switch Hit - Stopped Linking");
                SonsTools.ShowMessage("Transmitter Switch Hit - Stopped Linking");
                shouldUpdateRun = false;
                DestoryActiveLineRenderer();
                hitObjects.Clear(); // Clear the stored hit objects
            } else if (hitObjects.ContainsKey("TransmitterDetector"))  // If transmitter detector hit is stored, stop the linking
            {
                Misc.Msg("[LineRenderer] [HitTransmitterSwitch]: Transmitter Detector Hit - Stopped Linking");
                SonsTools.ShowMessage("Transmitter Detector Hit - Stopped Linking");
                shouldUpdateRun = false;
                DestoryActiveLineRenderer();
                hitObjects.Clear(); // Clear the stored hit objects
            }
        }

        public void HitDetector(GameObject go)
        {
            if (go == null)
            {
                Misc.Msg("[LineRenderer] [HitDetector]: GameObject is null");
                return;
            }
            // If CreatorSettings.lastState Is True, Only Owner Can Change The Linking
            if (Tools.CreatorSettings.lastState && !Tools.CreatorSettings.IsOwner(go.GetComponent<Mono.TransmitterDetector>().ownerSteamId))
            {
                Misc.Msg("[LineRenderer] [HitDetector]: You Must Be The Owner To Link");
                SonsTools.ShowMessage("You Must Be The Owner Edit Linking");
                hitObjects.Clear();
                return;
            }
            // If nothing is stored, store the transmitter switch hit
            if (!hitObjects.ContainsKey("TransmitterDetector") && !hitObjects.ContainsKey("TransmitterSwitch"))
            {
                hitObjects.Clear();  // Clear iF it has another Detector Stored
                hitObjects["TransmitterDetector"] = go;  // Store the detector hit
                Misc.Msg("[LineRenderer] [HitDetector]: Detector Hit - Link Point 1/2");
                SonsTools.ShowMessage("Detector Hit - Link Point 1/2");
                CreateActiveLineRenderer(go.transform.position);  // Create Active Line Renderer
                shouldUpdateRun = true;
                return;
            }
            else if (hitObjects.ContainsKey("TransmitterDetector"))  // If detector hit is stored, stop the linking
            {
                Misc.Msg("[LineRenderer] [HitDetector]: Detector Hit - Stopped Linking");
                SonsTools.ShowMessage("Detector Hit - Stopped Linking");
                shouldUpdateRun = false;
                DestoryActiveLineRenderer();
                hitObjects.Clear(); // Clear the stored hit objects
            }
            else if (hitObjects.ContainsKey("TransmitterSwitch"))  // If transmitter switch hit is stored, stop the linking
            {
                Misc.Msg("[LineRenderer] [HitDetector]: Transmitter Switch Hit - Stopped Linking");
                SonsTools.ShowMessage("Transmitter Switch Hit - Stopped Linking");
                shouldUpdateRun = false;
                DestoryActiveLineRenderer();
                hitObjects.Clear(); // Clear the stored hit objects

            }
        }

        public void CreateLineRenderer(Vector3 start, Vector3 end, bool visible = false, string linkedReciverUniqueId = null)
        {
            // Create new GameObject for the line
            GameObject lineObject = new GameObject("SignalLine");

            // Add LineRenderer component
            UnityEngine.LineRenderer line = lineObject.AddComponent<UnityEngine.LineRenderer>();

            // Set material and basic properties
            line.material = lineMaterial;
            line.positionCount = 2;
            line.startWidth = 0.05f;
            line.endWidth = 0.05f;

            // Set start and end positions
            line.SetPosition(0, start);
            line.SetPosition(1, end);

            // Optional quality settings
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;

            // Add To List
            //lineRenderers.Add(lineObject);
            if (linkedReciverUniqueId != null)
            {
                lineRenderers[linkedReciverUniqueId] = lineObject;
            }

            // Set LineRenderer visibility
            lineObject.SetActive(visible);
        }

        public Dictionary<string, List<Vector3>> GetLinkedLinesForSaving()  // Reciver Object Id, List Pos1 Pos2
        {
            Dictionary<string, List<Vector3>> linkedLines = new Dictionary<string, List<Vector3>>();
            foreach (var line in lineRenderers)
            {
                List<Vector3> positions = new List<Vector3>();
                positions.Add(line.Value.GetComponent<UnityEngine.LineRenderer>().GetPosition(0));
                positions.Add(line.Value.GetComponent<UnityEngine.LineRenderer>().GetPosition(1));
                linkedLines.Add(line.Key, positions);
            }
            return linkedLines;
        }

        public void LoadLinkedLines(Dictionary<string, List<Vector3>> linkedLines)
        {
            bool visible = false;
            if (LocalPlayer.Inventory != null && LocalPlayer.Inventory.RightHandItem != null)
            {
                if (LocalPlayer.Inventory.RightHandItem._itemID == 422)
                {
                    visible = true;
                }
            }
            foreach (var line in linkedLines)
            {
                
                CreateLineRenderer(line.Value[0], line.Value[1], visible, line.Key);
            }
        }

        public void HideLineRenderers()
        {
            //foreach (var line in lineRenderers)
            //{
            //    line.SetActive(false);
            //}
            foreach (var line in lineRenderers)
            {
                line.Value.SetActive(false);
            }
        }

        public void ShowLineRenderers()
        {
            //foreach (var line in lineRenderers)
            //{
            //    line.SetActive(true);
            //}
            foreach (var line in lineRenderers)
            {
                line.Value.SetActive(true);
            }
        }

        public void DestoryActiveLineRenderer()
        {
            if (activeLineRenderer == null) { return; }
            Destroy(activeLineRenderer.gameObject);
        }

        public void ChangeActiveLineRenderer(Vector3 end)
        {
            if (activeLineRenderer == null) { return; }
            activeLineRenderer.SetPosition(1, end);
        }

        public void CreateActiveLineRenderer(Vector3 startPos)
        {
            activeLineRenderer = new GameObject("ActiveLine").AddComponent<UnityEngine.LineRenderer>();
            activeLineRenderer.material = lineMaterial;
            activeLineRenderer.positionCount = 2;
            activeLineRenderer.startWidth = 0.05f;
            activeLineRenderer.endWidth = 0.05f;
            activeLineRenderer.SetPosition(0, startPos);
            activeLineRenderer.SetPosition(0, LocalPlayer._instance._mainCam.transform.position + LocalPlayer._instance._mainCam.transform.forward * 1f);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Update()
        {
            if (!shouldUpdateRun) { return; }
            if (activeLineRenderer == null) { return; }
            if (!LocalPlayer.IsInWorld || LocalPlayer.InWater || LocalPlayer.IsInInventory || PauseMenu.IsActive) { return; }
            ChangeActiveLineRenderer(LocalPlayer._instance._mainCam.transform.position + LocalPlayer._instance._mainCam.transform.forward * 1f);
        }

        public void RepairToolInHand(bool inHand)
        {
            if (inHand)
            {
                isRepairToolInHand = true;
                ShowLineRenderers();
            }
            else
            {
                isRepairToolInHand = false;
                HideLineRenderers();
                DestoryActiveLineRenderer();
                shouldUpdateRun = false;
                hitObjects.Clear();
            }
        }
    }
}
