using RedLoader;
using Sons.Gui.Input;
using SonsSdk;
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

        // Link Ui Menu
        public LinkUiElement _linkUi = null;

        // Settings Menu
        private bool _linkedReciverObject = false;  // To Trigger Cases On Turn On/Off
        private string _linkedReciverObjectName = null;  // Always lowercase
        public float objectRange = 1f;  // Range to detect objects, Physics.OverlapSphere
        private HashSet<GameObject> _objectsInRange = new HashSet<GameObject>(new Tools.GameObjectComparer());  // Objects in range, NOTE: Only Valid Ojbects
        public bool _revertOutput = false;  // Revert output for linked objects


        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Start()
        {
            if (isSetupPrefab)
            {
                return;
            }
            if (string.IsNullOrEmpty(uniqueId))
            {
                RLog.Warning("[Mono] [Reciver]: uniqueId is null! Shound Never be");
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
            UpdateDebugUi(true);
            if (Debug.VisualData.GetDebugMode())
            {
                SetDebugUi(true);
            }

            if (_linkUi == null)
            {
                _linkUi = UI.LinkUi.CreateLinkUi(gameObject, 2f, null, Assets.UIAdjust, new Vector3(0, 0f, 0), "screen.take");
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
            UpdateDebugUi();
        }

        public void Unlink()
        {
            Misc.Msg("[Reciver] [Unlink] Unlinking Reciver");
            isOn = false;
            TurnOffLight();
            linkedToTranmitterSwithUniqueId = null;
            UpdateDebugUi();
        }

        public void Link(string transmitterUniqueId)
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
                }
                else
                {
                    TurnOffLight();
                    isOn = false;
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
                }
                else
                {
                    TurnOffLight();
                    isOn = false;
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

        public void SetLinkedReciverObject(bool state, string name)
        {
            _linkedReciverObject = state;
            _linkedReciverObjectName = name;
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

        public void ShowScanLines(bool state)
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

        public void SetScanObjectRange(float range)
        {
            Mono.OverlapSphereVisualizer visualizer = gameObject.GetComponent<Mono.OverlapSphereVisualizer>();
            if (visualizer != null)
            {
                visualizer.objectRange = range;
            }
        }

        public bool IsScanLinesShown()
        {
            Mono.OverlapSphereVisualizer visualizer = gameObject.GetComponent<Mono.OverlapSphereVisualizer>();
            return visualizer != null;
        }
    }
}
