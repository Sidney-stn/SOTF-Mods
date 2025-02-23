using RedLoader;
using Sons.Gui.Input;
using SonsSdk;
using UnityEngine;
using UnityEngine.UI;

namespace WirelessSignals.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class TransmitterSwitch : MonoBehaviour
    {
        public string uniqueId;
        public bool? isOn = false;
        public bool isSetupPrefab;
        private Animator _animController = null;
        private LinkUiElement _linkUi = null;
        public HashSet<string> linkedUniqueIdsRecivers = new HashSet<string>();
        public string ownerSteamId = null;  // If owner is null, then it's public

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Start()
        {
            if (isSetupPrefab)
            {
                return;
            }
            if (string.IsNullOrEmpty(uniqueId))
            {
                RLog.Warning("[Mono] [TransmitterSwitch]: uniqueId is null! Shound Never be");
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

            // Animator
            if (_animController == null)
            {
                _animController = gameObject.transform.FindChild("Operation").GetChild(0).GetComponent<Animator>();
                if (_animController == null)
                {
                    Misc.Msg("TransmitterSwitch: _animController is null! Should never be");
                }
            }

            // Link UI
            if (_linkUi == null)
            {
                _linkUi = UI.LinkUi.CreateLinkUi(gameObject.transform.FindChild("Operation").GetChild(0).gameObject, 2f, null, Assets.UIOnOff, new Vector3(0, 0f, 0), "screen.take");
            }

            if (isOn == true)
            {
                TurnOnLight();
                if (_animController != null) { _animController.SetTrigger("TurnOn"); }
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

        public void TurnOnLight()
        {
            GameObject lights = transform.FindChild("Light").gameObject;
            if (lights != null)
            {
                lights.SetActive(true);
            }
            else
            {
                Misc.Msg("[TransmitterSwitch] [TurnOnLight] Can't Turn On Lights, Null!");
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
                Misc.Msg("[TransmitterSwitch] [TurnOffLight] Can't Turn Off Lights, Null!");
            }
        }

        public void Toggle()
        {
            Misc.Msg("[TransmitterSwitch] [Toggle] Toggling");
            // Check if LinkUi is enabled
            if (_linkUi == null) { Misc.Msg("[TransmitterSwitch] [Toggle] LinkUiElement Is Null!"); return; }
            if (_linkUi.IsActive)
            {
                if (isOn == true)
                {
                    TurnOffLight();
                    if (_animController != null) { _animController.SetTrigger("TurnOff"); }
                    isOn = false;
                    Misc.Msg("[TransmitterSwitch] [Toggle] Turned Off");

                }
                else
                {
                    TurnOnLight();
                    if (_animController != null) { _animController.SetTrigger("TurnOn"); }
                    isOn = true;
                    Misc.Msg("[TransmitterSwitch] [Toggle] Turned On");
                }
                // Update All Recivers Linked
                foreach (var reciverUniqueId in linkedUniqueIdsRecivers)
                {
                    var reciver = WirelessSignals.reciver.FindByUniqueId(reciverUniqueId);
                    if (reciver)
                    {
                        reciver.GetComponent<Reciver>().SetState((bool)isOn);  // Set Reciver State (bool) should always work since the state is set above and can never be null
                        Misc.Msg("[TransmitterSwitch] [Toggle] Reciver Updated");
                    }
                    else
                    {
                        Misc.Msg("[TransmitterSwitch] [Toggle] Reciver Not Found");
                    }
                }
            }
            UpdateDebugUi();
        }

        public void UnlinkReciver(string reciverUniqueId)
        {
            if (linkedUniqueIdsRecivers.Contains(reciverUniqueId))
            {
                linkedUniqueIdsRecivers.Remove(reciverUniqueId);
                Misc.Msg("[TransmitterSwitch] [UnlinkReciver] Reciver Unlinked");
            }
            else
            {
                Misc.Msg("[TransmitterSwitch] [UnlinkReciver] Reciver Not Found");
            }
            UpdateDebugUi();
        }

        public void LinkReciver(string reciverUniqueId)
        {
            if (!linkedUniqueIdsRecivers.Contains(reciverUniqueId))
            {
                linkedUniqueIdsRecivers.Add(reciverUniqueId);
                Misc.Msg("[TransmitterSwitch] [LinkReciver] Reciver Linked");
            }
            else
            {
                Misc.Msg("[TransmitterSwitch] [LinkReciver] Reciver Already Linked");
            }
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
                text.text = $"IsOn: {(isOn.HasValue ? (isOn.Value ? "True" : "False") : "null")}\r\nUniqueId: {uniqueId}\r\nLinkedUniqueIdsRecivers: {string.Join(", ", linkedUniqueIdsRecivers.ToList())}";
            }
            else
            {
                text.text = $"IsOn: {(isOn.HasValue ? (isOn.Value ? "True" : "False") : "null")}\r\nLinked To Count: {linkedUniqueIdsRecivers.Count}";
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
