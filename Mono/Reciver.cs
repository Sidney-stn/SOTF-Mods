using RedLoader;
using UnityEngine;

namespace WirelessSignals.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class Reciver : MonoBehaviour
    {
        internal string uniqueId;
        internal bool? isOn = false;
        internal bool isSetupPrefab;
        internal string linkedToTranmitterSwithUniqueId = null;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Start()
        {
            if (isSetupPrefab)
            {
                return;
            }
            if (uniqueId == null)
            {
                Misc.Msg("Reciver: uniqueId is null! Shound Never be");
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
        }

        public void Unlink()
        {
            Misc.Msg("[Reciver] [Unlink] Unlinking Reciver");
            isOn = false;
            TurnOffLight();
            linkedToTranmitterSwithUniqueId = null;
        }

        public void Link(string transmitterUniqueId)
        {
            Misc.Msg("[Reciver] [Link] Linking Reciver");
            linkedToTranmitterSwithUniqueId = transmitterUniqueId;

            // Check if transmitter is on
            GameObject transmitter = WirelessSignals.transmitterSwitch.FindByUniqueId(transmitterUniqueId);
            if (transmitter == null)
            {
                Misc.Msg("[Reciver] [Link] Transmitter is null");
                return;
            }
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
        }
    }
}
