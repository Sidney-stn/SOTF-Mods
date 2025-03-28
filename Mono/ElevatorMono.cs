
using Sons.Gui.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleElevator.Mono
{
    internal class ElevatorMono : MonoBehaviour
    {
        public bool isSetupPrefab = false;
        public GameObject CallTextGo { get; private set; }
        public Text CallText { get; private set; }
        public GameObject GotoGo { get; private set; }
        public Text GotoText { get; private set; }
        public GameObject FloorNumberGo { get; private set; }
        public Text FloorNumberText { get; private set; }
        public GameObject FloorTextGo { get; private set; }
        public Text FloorText { get; private set; }
        public GameObject ErrorTextGo { get; private set; }
        public Text ErrorText { get; private set; }

        private void Awake()
        {
            if (isSetupPrefab) { return; }
            if (transform.position == Vector3.zero) { isSetupPrefab = true; return; }
            if (CallTextGo == null)  // Not in use, hide it.
            {
                CallTextGo = gameObject.transform.GetChild(3).FindChild("Call").gameObject;
                if (CallTextGo != null)
                {
                    CallText = CallTextGo.GetComponent<Text>();
                    CallTextGo.SetActive(false);  // Disable the CallTextGo. [NOT IN USE]
                }
            }
            if (GotoGo == null)
            {
                GotoGo = gameObject.transform.GetChild(3).FindChild("Goto").gameObject;
                if (GotoGo != null)
                {
                    GotoText = GotoGo.GetComponent<Text>();
                    GotoGo.SetActive(true);
                }
            }
            if (FloorNumberGo == null)
            {
                FloorNumberGo = gameObject.transform.GetChild(3).FindChild("Number").gameObject;
                if (FloorNumberGo != null)
                {
                    FloorNumberText = FloorNumberGo.GetComponent<Text>();
                    if (FloorNumberText != null)
                    {
                        FloorNumberText.text = "0";
                    }
                    FloorNumberGo.SetActive(true);
                }
            }
            if (FloorTextGo == null)
            {
                FloorTextGo = gameObject.transform.GetChild(3).FindChild("Floor").gameObject;
                if (FloorTextGo != null)
                {
                    FloorText = FloorTextGo.GetComponent<Text>();
                    FloorTextGo.SetActive(true);
                }
            }
            if (ErrorTextGo == null)
            {
                ErrorTextGo = gameObject.transform.GetChild(3).FindChild("Error").gameObject;
                if (ErrorTextGo != null)
                {
                    ErrorText = ErrorTextGo.GetComponent<Text>();
                    ErrorTextGo.SetActive(false);  // Hide By Default
                }
            }
        }

        public LinkUiElement LinkUi { get; private set; }

        private void Start()
        {
            if (isSetupPrefab) { return; }
            if (LinkUi == null)
            {
                LinkUi = Tools.LinkUi.CreateLinkUi(gameObject, 2f, null, Assets.Instance.LinkUiIcon, null);
            }
        }


        public void ShowError(string error, float seconds = 3f)
        {
            if (ErrorTextGo == null) { return; }
            if (ErrorText == null) { return; }
            showError(error, seconds);
        }

        private IEnumerator showError(string error, float seconds)
        {
            if (ErrorTextGo == null) { yield break; }
            if (ErrorText == null) { yield break; }
            ErrorText.text = error;
            if (GotoGo != null) { GotoGo.SetActive(false); }
            if (FloorNumberGo != null) { FloorNumberGo.SetActive(false); }
            if (FloorTextGo != null) { FloorTextGo.SetActive(false); }
            ErrorTextGo.SetActive(true);
            yield return new WaitForSeconds(seconds);
            ErrorTextGo.SetActive(false);
            if (GotoGo != null) { GotoGo.SetActive(true); }
            if (FloorNumberGo != null) { FloorNumberGo.SetActive(true); }
            if (FloorTextGo != null) { FloorTextGo.SetActive(true); }
        }
    }
}
