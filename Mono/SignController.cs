using RedLoader;
using System.Collections;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Signs.Mono
{
    //[RegisterTypeInIl2Cpp]
    internal class SignController : MonoBehaviour
    {
        public bool isSetupPrefab = false;

        private Text _line1;
        private Text _line2;
        private Text _line3;
        private Text _line4;
        private string _line1Dedicated;
        private string _line2Dedicated;
        private string _line3Dedicated;
        private string _line4Dedicated;

        private bool isCoroutineRunning = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Awake()
        {
            if (isSetupPrefab) { return; }
            if (_line1 == null) { _line1 = gameObject.transform.FindChild("UI").FindChild("Canvas").FindChild("Line1").GetComponent<Text>(); }
            if (_line2 == null) { _line2 = gameObject.transform.FindChild("UI").FindChild("Canvas").FindChild("Line2").GetComponent<Text>(); }
            if (_line3 == null) { _line3 = gameObject.transform.FindChild("UI").FindChild("Canvas").FindChild("Line3").GetComponent<Text>(); }
            if (_line4 == null) { _line4 = gameObject.transform.FindChild("UI").FindChild("Canvas").FindChild("Line4").GetComponent<Text>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Start()
        {
            // Add For Saving
            if (isSetupPrefab) { return; }
            if (BoltNetwork.isRunning && BoltNetwork.isServer || Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
            {
                if (gameObject.transform.position == Vector3.zero)
                {
                    Misc.Msg("[SignController] [Start] Position is zero, should be setupPrefab!");
                    isSetupPrefab = true;
                    return;
                }
                var boltEntity = gameObject.GetComponent<BoltEntity>();
                if (boltEntity == null) { RLog.Warning("[SignController] [Start] BoltEntity is null"); return; }
                Saving.Track.AddGameObject(boltEntity, gameObject);
            }
            var screwStucture = gameObject.GetComponent<Sons.Crafting.Structures.ScrewStructure>();
            if (screwStucture != null)
            {
                DestroyImmediate(screwStucture);
                Misc.Msg("[SignController] [Start] ScrewStructure Delete");
            }
            //if (Tools.DedicatedServer.IsDeticatedServer())
            //{
            //    // Create Fake Text To Store Date
            //    if (_line1 == null) { _line1 = gameObject.AddComponent<Text>(); }
            //    if (_line2 == null) { _line2 = gameObject.AddComponent<Text>(); }
            //    if (_line3 == null) { _line3 = gameObject.AddComponent<Text>(); }
            //    if (_line4 == null) { _line4 = gameObject.AddComponent<Text>(); }
            //    Misc.Msg("[SignController] [Start] Dedicated Server Mode");
            //}
        }

        public void SetLineText(int line, string textToDisplay, bool raiseNetwork = true)
        {
            
            Text text = null;
            switch (line)
            {
                case 1:
                    text = _line1;
                    break;
                case 2:
                    text = _line2;
                    break;
                case 3:
                    text = _line3;
                    break;
                case 4:
                    text = _line4;
                    break;
            }
            if (text != null)
            {
                text.text = ClipString(textToDisplay);
                Misc.Msg($"[SignController] [SetLineText] Set Line: {line} to: {textToDisplay}");
                if (UI.Setup.messageText != null) { UI.Setup.messageText.text = $"Text Updated"; DoSomethingAfterDelay().RunCoro(); }
                    
            } else if (text == null && SonsSdk.Networking.NetUtils.IsDedicatedServer || text == null && Tools.DedicatedServer.IsDeticatedServer())
            {
                switch (line)
                {
                    case 1:
                        _line1Dedicated = ClipString(textToDisplay);
                        break;
                    case 2:
                        _line2Dedicated = ClipString(textToDisplay);
                        break;
                    case 3:
                        _line3Dedicated = ClipString(textToDisplay);
                        break;
                    case 4:
                        _line4Dedicated = ClipString(textToDisplay);
                        break;
                }
            }
            else
            { Misc.Msg($"[SignController] [SetLineText] Failed To Set Line: {line} to: {textToDisplay}. Text == null"); if (UI.Setup.messageText != null) { UI.Setup.messageText.text = $"Update Failed"; DoSomethingAfterDelay().RunCoro(); } }
            if (raiseNetwork && BoltNetwork.isRunning)
            {
                Misc.Msg($"[SignController] [SetLineText] Raising Network Event");
                switch (line)
                {
                    case 1:
                        Network.SignSyncEvent.SendState(GetComponent<BoltEntity>(), Network.SignSyncEvent.SignSyncType.SetTextLine1);
                        break;
                    case 2:
                        Network.SignSyncEvent.SendState(GetComponent<BoltEntity>(), Network.SignSyncEvent.SignSyncType.SetTextLine2);
                        break;
                    case 3:
                        Network.SignSyncEvent.SendState(GetComponent<BoltEntity>(), Network.SignSyncEvent.SignSyncType.SetTextLine3);
                        break;
                    case 4:
                        Network.SignSyncEvent.SendState(GetComponent<BoltEntity>(), Network.SignSyncEvent.SignSyncType.SetTextLine4);
                        break;
                }
                
            }
        }

        public void SetAllText(string line1, string line2, string line3, string line4, bool raiseNetwork = true)
        {
            if (line1 != null) { SetLineText(1, ClipString(line1), false); }
            if (line2 != null) { SetLineText(2, ClipString(line2), false); }
            if (line3 != null) { SetLineText(3, ClipString(line3), false); }
            if (line4 != null) { SetLineText(4, ClipString(line4), false); }

            if (UI.Setup.messageText != null) { UI.Setup.messageText.text = $"Text Updated"; DoSomethingAfterDelay().RunCoro(); }

            if (raiseNetwork && BoltNetwork.isRunning)
            {
                Misc.Msg($"[SignController] [SetLineText] Raising Network Event");
                Network.SignSyncEvent.SendState(GetComponent<BoltEntity>(), Network.SignSyncEvent.SignSyncType.SetTextAll);
            }

        }

        private string ClipString(string input)
        {
            return input?.Length > 20 ? input[..20] : input;
        }

        public void OpenSignUi()
        {
            if (_line1.text == null) { Misc.Msg("Line1 is null"); return; }
            if (_line2.text == null) { Misc.Msg("Line2 is null"); return; }
            if (_line3.text == null) { Misc.Msg("Line3 is null"); return; }
            if (_line4.text == null) { Misc.Msg("Line4 is null"); return; }

            UI.Setup.SetLineText(1, _line1.text);
            UI.Setup.SetLineText(2, _line2.text);
            UI.Setup.SetLineText(3, _line3.text);
            UI.Setup.SetLineText(4, _line4.text);

            Prefab.ActiveSign.activeSign = gameObject;

            if (Prefab.ActiveSign.activeSign == null) { Misc.Msg("Active Sign is null after assigning, something is wrong"); if (UI.Setup.messageText != null) { UI.Setup.messageText.text = $"Update Failed"; DoSomethingAfterDelay().RunCoro(); } return; }
            UI.Setup.OpenUI();
            CheckDistance().RunCoro();
        }

        public string GetLineText(int line)
        {
            Text text = null;
            switch (line)
            {
                case 1:
                    text = _line1;
                    break;
                case 2:
                    text = _line2;
                    break;
                case 3:
                    text = _line3;
                    break;
                case 4:
                    text = _line4;
                    break;
            }
            if (text != null)
            {
                return text.text;
            } 
            else if (text == null && Tools.DedicatedServer.IsDeticatedServer() || text == null && SonsSdk.Networking.NetUtils.IsDedicatedServer)
            {
                switch (line)
                {
                    case 1:
                        return _line1Dedicated;
                    case 2:
                        return _line2Dedicated;
                    case 3:
                        return _line3Dedicated;
                    case 4:
                        return _line4Dedicated;
                }
            }
            return null;
        }

        public Vector3 GetPos()
        {
            return gameObject.transform.position;
        }

        public Quaternion GetCurrentRotation()
        {
            return gameObject.transform.rotation;
        }

        private IEnumerator DoSomethingAfterDelay()
        {
            if (isCoroutineRunning) { yield break; }
            isCoroutineRunning = true;
            Misc.Msg("Coroutine started");

            // Wait for 3 seconds
            yield return new WaitForSeconds(3f);

            // Do something here
            Misc.Msg("3 seconds have passed, doing something!");

            // Coroutine will automatically exit after this point
            if (UI.Setup.messageText != null) { UI.Setup.messageText.text = ""; }
            isCoroutineRunning = false;
        }

        private IEnumerator CheckDistance()
        {
            while (true)
            {
                float distance = Vector3.Distance(LocalPlayer.Transform.position, transform.position);
                //Misc.Msg($"Distance: {distance}");
                if (distance > 5f)
                {
                    Prefab.ActiveSign.activeSign = null;
                    UI.Setup.CloseUI(); // Assuming there's a CloseUI method
                    yield break; // This will end the coroutine
                }
                yield return new WaitForSeconds(1f); // Check every 1 seconds
            }
        }
    }
}
