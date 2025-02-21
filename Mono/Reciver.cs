using RedLoader;
using SonsSdk;
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
    }
}
