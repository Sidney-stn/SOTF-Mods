using RedLoader;
using Sons.Gui.Input;
using SonsSdk;
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
