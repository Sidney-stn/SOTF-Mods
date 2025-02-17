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
        //public List<GameObject> lineRenderers = new List<GameObject>();
        public Dictionary<string, GameObject> lineRenderers = new Dictionary<string, GameObject>();  // ReciverUnqiueId, LineRenderer
        private Material lineMaterial;
        private UnityEngine.LineRenderer activeLineRenderer = null;
        private bool isRepairToolInHand = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
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

            // If transmitter switch hit is not stored, unlink the reciver
            if (!hitObjects.ContainsKey("TransmitterSwitch"))
            {
                hitObjects.Clear();  // Clear iF it has another Reciver Stored
                hitObjects["Reciver"] = go;  // Store the reciver hit
                // Unlink the reciver
                Mono.Reciver controller = go.GetComponent<Mono.Reciver>();
                if (controller == null) { Misc.Msg("[LineRenderer] [HitReciver]: Reciver controller is null"); return; }
                if (string.IsNullOrEmpty(controller.linkedToTranmitterSwithUniqueId)) { Misc.Msg("[LineRenderer] [HitReciver]: Reciver linkedToTranmitterSwithUniqueId is null"); return; }
                GameObject wirelessTransmitter = WirelessSignals.transmitterSwitch.FindByUniqueId(controller.linkedToTranmitterSwithUniqueId);
                if (wirelessTransmitter == null) // If transmitter is not found by uniqueId Stored, unlink
                {
                    controller.Unlink();  // Unlink Reciver
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver wirelessTransmitter is null"); 
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver unlinked");
                    SonsTools.ShowMessage("Reciver Successfully Unlinked");
                    // Destory LineRenderer
                    if (lineRenderers.ContainsKey(controller.uniqueId))
                    {
                        Destroy(lineRenderers[controller.uniqueId]);
                        lineRenderers.Remove(controller.uniqueId);
                    }
                }
                else  // Unlink Both Reciver And Transmitter
                {
                    controller.Unlink();
                    Mono.TransmitterSwitch transmitterController = wirelessTransmitter.GetComponent<Mono.TransmitterSwitch>();
                    if (transmitterController == null) { Misc.Msg("[LineRenderer] [HitReciver]: Transmitter controller is null"); return; }
                    transmitterController.UnlinkReciver(controller.uniqueId);
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver and Transmitter unlinked");
                    SonsTools.ShowMessage("Reciver Successfully Unlinked");
                    // Destory LineRenderer
                    if (lineRenderers.ContainsKey(controller.uniqueId))
                    {
                        Destroy(lineRenderers[controller.uniqueId]);
                        lineRenderers.Remove(controller.uniqueId);
                    }
                }
                return;
            }
            else if (hitObjects.ContainsKey("TransmitterSwitch"))  // If transmitter switch hit is stored, link the reciver
            {
                // Link the reciver
                Mono.Reciver controller = go.GetComponent<Mono.Reciver>();
                if (controller == null)
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver controller is null");
                    return;
                }
                Mono.TransmitterSwitch transmitterController = hitObjects["TransmitterSwitch"].GetComponent<Mono.TransmitterSwitch>();
                if (transmitterController == null)
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: Transmitter controller is null");
                    return;
                }
                if (transmitterController.linkedUniqueIdsRecivers.Contains(controller.uniqueId))  // If reciver is already linked to transmitter switch
                {
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver Already Linked");
                    SonsTools.ShowMessage("Reciver Already Linked");
                    if (controller.linkedToTranmitterSwithUniqueId != transmitterController.uniqueId)
                    {
                        // Fix Linking In Case Something Have Gone Wrong
                        controller.linkedToTranmitterSwithUniqueId = transmitterController.uniqueId;
                    }
                    //CreateLineRenderer(activeLineRenderer.GetPosition(0), activeLineRenderer.GetPosition(1), true, controller.uniqueId);
                    shouldUpdateRun = false;
                    hitObjects.Clear(); // Clear the stored hit objects
                    DestoryActiveLineRenderer();
                    return;
                } 
                else // Link Reciver
                {
                    shouldUpdateRun = false;
                    CreateLineRenderer(activeLineRenderer.GetPosition(0), activeLineRenderer.GetPosition(1), true, controller.uniqueId);
                    DestoryActiveLineRenderer();
                    transmitterController.LinkReciver(controller.uniqueId);
                    controller.Link(transmitterController.uniqueId);
                    Misc.Msg("[LineRenderer] [HitReciver]: Reciver Linked");
                    SonsTools.ShowMessage("Reciver Successfully Linked - Link Point 2/2");
                }

                hitObjects.Clear(); // Clear the stored hit objects
                hitObjects["Reciver"] = go;  // Store the reciver hit

            }
        }

        public void HitTransmitterSwitch(GameObject go)
        {
            if (go == null)
            {
                Misc.Msg("[LineRenderer] [HitTransmitterSwitch]: GameObject is null");
                return;
            }
            // If nothing is stored, store the transmitter switch hit
            if (!hitObjects.ContainsKey("TransmitterSwitch"))
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
