using RedLoader;
using Sons.Gui.Input;
using SonsSdk;
using System.Collections;
using TheForest.Utils;
using UnityEngine;

namespace WirelessSignals.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class TransmitterDetector : MonoBehaviour
    {
        public string uniqueId;
        public bool? isOn = false;
        public bool isSetupPrefab;
        public HashSet<string> linkedUniqueIdsRecivers = new HashSet<string>();

        public bool foundObject;
        public string foundObjectName;

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
                        if (open == null) { return; }  // Add A Way For Scanning Here
                        string name = raycastHit.collider.transform.root.name;
                        if (name.Contains("StorageLogsStructure"))
                        {
                            foundObject = true;
                            foundObjectName = "StorageLogsStructure";
                            Misc.Msg("[Mono] [TransmitterDetector] [TryFindObject] Found StorageLogsStructure");
                            SonsTools.ShowMessage("Found StorageLogsStructure - Detecting", 5);
                            CheckLogs(open, 6).RunCoro();
                            return;
                        }
                        else if (name.Contains("StorageLogsLargeStructure"))
                        {
                            foundObject = true;
                            foundObjectName = "StorageLogsLargeStructure";
                            Misc.Msg("[Mono] [TransmitterDetector] [TryFindObject] Found StorageLogsLargeStructure");
                            SonsTools.ShowMessage("Found StorageLogsLargeStructure - Detecting", 5);
                            CheckLogs(open, 24).RunCoro();
                            return;
                        }

                    }
                }
            }
        }

        private IEnumerator CheckLogs(GameObject obj, int maxLogsInStorage)
        {
            int maxLogs = maxLogsInStorage;
            GameObject checkObj = obj;
            if (checkObj == null) { Misc.Msg("[Mono] [TransmitterDetector] [CheckLogs] CheckObj is Null"); yield break; }
            Misc.Msg("[Mono] [TransmitterDetector] [CheckLogs] Start Checking Logs");
            if (!foundObject) { Misc.Msg("[Mono] [TransmitterDetector] [CheckLogs] Retuned, found object is false"); }
            while (foundObject)
            {
                Misc.Msg("[Mono] [TransmitterDetector] [CheckLogs] While Loop - Checking Logs");
                if (!foundObject) { Misc.Msg("[Mono] [TransmitterDetector] [CheckLogs] Retuned, found object is false"); yield break; }
                if (checkObj == null) { Misc.Msg("[Mono] [TransmitterDetector] [CheckLogs] While Loop - CheckObj is Null"); yield break; }
                GameObject logLayoutGroup = checkObj.transform.FindChild("LogLayoutGroup").gameObject;
                if (logLayoutGroup == null) { Misc.Msg("[Mono] [TransmitterDetector] [CheckLogs] While Loop - LogLayoutGroup is Null"); yield break; }

                List<Transform> transforms = logLayoutGroup.transform.GetChildren();
                int activeLogs = 0;
                for (int i = 0; i < transforms.Count; i++)
                {
                    Transform log = transforms[i];
                    if (log == null) { Misc.Msg("[Mono] [TransmitterDetector] [CheckLogs] While Loop - Log is Null"); yield break; }
                    if (log.gameObject.active && log.gameObject.name.Contains("LogLayoutItem"))
                    {
                        activeLogs++;
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
                if (Vector3.Distance(LocalPlayer.GameObject.transform.position, transform.position) <= 50f)
                {
                    if (maxLogs < 8)
                    {
                        yield return new WaitForSeconds(1f);  // If Player Is Close To The Structure, This Will Be Called Every Second
                    } else
                    {
                        yield return new WaitForSeconds(3f);  // If Player Is Close To The Structure, This Will Be Called Every 30 Seconds
                    }
                    
                } 
                else
                {
                    yield return new WaitForSeconds(30f);  // If Player Is Far From The Structure, This Will Be Called Every 30 Seconds
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
        }
    }
}
