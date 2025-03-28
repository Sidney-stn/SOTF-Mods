
using Sons.Gui.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleElevator.Mono
{
    internal class ElevatorMono : MonoBehaviour
    {
        public bool isSetupPrefab = false;
        public GameObject MoveGo { get; private set; }
        public Text MoveText { get; private set; }
        public GameObject UPGo { get; private set; }
        public Text UPText { get; private set; }
        public GameObject DOWNGo { get; private set; }
        public Text DOWNText { get; private set; }
        public GameObject ErrorTextGo { get; private set; }
        public Text ErrorText { get; private set; }

        private string _upOrDown = "";
        public string UpOrDown
        {
            get
            {
                return _upOrDown;
            }
            set
            {
                _upOrDown = value;
                if (value == "UP" || value == "DOWN")
                {
                    if (UPGo != null) { UPGo.SetActive(value == "UP"); }
                    if (DOWNGo != null) { DOWNGo.SetActive(value == "DOWN"); }
                }
                else if (value == "NONE")
                {
                    if (UPGo != null) { UPGo.SetActive(false); }
                    if (DOWNGo != null) { DOWNGo.SetActive(false); }
                }
            }
        }

        private void Awake()
        {
            if (isSetupPrefab) { return; }
            if (transform.position == Vector3.zero) { isSetupPrefab = true; return; }
            if (MoveGo == null)
            {
                MoveGo = gameObject.transform.GetChild(3).FindChild("MoveGo").gameObject;
                if (MoveGo != null)
                {
                    MoveText = MoveGo.GetComponent<Text>();
                    MoveGo.SetActive(true);
                }
            }
            if (UPGo == null)
            {
                UPGo = gameObject.transform.GetChild(3).FindChild("UP").gameObject;
                if (UPGo != null)
                {
                    UPText = UPGo.GetComponent<Text>();
                    UPGo.SetActive(false);
                }
            }
            if (DOWNGo == null)
            {
                DOWNGo = gameObject.transform.GetChild(3).FindChild("DOWN").gameObject;
                if (DOWNGo != null)
                {
                    DOWNText = DOWNGo.GetComponent<Text>();
                    DOWNGo.SetActive(false);
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
                LinkUi = Tools.LinkUi.CreateLinkUi(gameObject.transform.GetChild(4).gameObject, 2f, null, Assets.Instance.LinkUiIcon, null);
            }
            Destroy(GetComponent<Collider>());  // I Removed Here since it get added from something i dont have in unity
            Objects.Track.Elevators.Add(gameObject);
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
            if (MoveGo != null) { MoveGo.SetActive(false); }
            if (UPGo != null) { UPGo.SetActive(false); }
            if (DOWNGo != null) { DOWNGo.SetActive(false); }
            UpOrDown = "NONE";
            ErrorTextGo.SetActive(true);
            yield return new WaitForSeconds(seconds);
            ErrorTextGo.SetActive(false);
            if (MoveGo != null) { MoveGo.SetActive(true); }
        }

        public void MoveUp(bool raiseNetwork = true)
        {
            if (BoltNetwork.isRunning == false)
            {

            }
            else if (BoltNetwork.isServer)
            {

            }
            else if (BoltNetwork.isClient)
            {

            }
        }

        public void MoveDown(bool raiseNetwork = true)
        {
            if (BoltNetwork.isRunning == false)
            {

            }
            else if (BoltNetwork.isServer)
            {

            }
            else if (BoltNetwork.isClient)
            {

            }
        }

        public void InvokePrimaryAction()
        {
            if (UpOrDown == "UP")
            {
                MoveUp();
            }
            else if (UpOrDown == "DOWN")
            {
                MoveDown();
            }
            else
            {
                ShowError("No\nDirection\nSelected");
            }
        }

        public void OnScrollUp() { UpOrDown = "UP"; }
        public void OnScrollDown() { UpOrDown = "DOWN"; }

        private void OnDestroy()
        {
            Objects.Track.Elevators.Remove(gameObject);
        }
    }
}
