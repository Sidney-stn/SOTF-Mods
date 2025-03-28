
using Sons.Gui.Input;
using UnityEngine;

namespace SimpleElevator.Mono
{
    internal class ElevatorControlPanelMono : MonoBehaviour
    {
        public bool isSetupPrefab = false;
        public LinkUiElement LinkUi { get; private set; }

        private void Start()
        {
            if (isSetupPrefab) { return; }
            if (LinkUi == null)
            {
                LinkUi = Tools.LinkUi.CreateLinkUi(gameObject.transform.GetChild(1).gameObject, 2f, null, Assets.Instance.LinkUiIcon, null);
            }
            Objects.Track.ElevatorControlPanels.Add(gameObject);
        }

        public void InvokePrimaryAction()
        {

        }

        private void OnDestroy()
        {
            Objects.Track.ElevatorControlPanels.Remove(gameObject);
        }
    }
}
