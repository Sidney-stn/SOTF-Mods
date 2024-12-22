using RedLoader;
using UnityEngine;
using UnityEngine.UI;

namespace Signs.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class SignController : MonoBehaviour
    {
        public ulong? SteamId = null;
        public string UniqueId = null;

        private Text _line1;
        private Text _line2;
        private Text _line3;
        private Text _line4;

        private void Awake()
        {
            if (_line1 == null) { _line1 = gameObject.transform.FindChild("UI").FindChild("Canvas").FindChild("Line1").GetComponent<Text>(); }
            if (_line2 == null) { _line2 = gameObject.transform.FindChild("UI").FindChild("Canvas").FindChild("Line2").GetComponent<Text>(); }
            if (_line3 == null) { _line3 = gameObject.transform.FindChild("UI").FindChild("Canvas").FindChild("Line3").GetComponent<Text>(); }
            if (_line4 == null) { _line4 = gameObject.transform.FindChild("UI").FindChild("Canvas").FindChild("Line4").GetComponent<Text>(); }
            if (string.IsNullOrEmpty(UniqueId)) { }
        }

        public void SetLineText(int line, string textToDisplay)
        {
            
            Text text = null;
            int? idx = null;
            switch (line)
            {
                case 1:
                    text = _line1;
                    idx = 1;
                    break;
                case 2:
                    text = _line2;
                    idx = 2;
                    break;
                case 3:
                    text = _line3;
                    idx = 3;
                    break;
                case 4:
                    text = _line4;
                    idx = 4;
                    break;
            }
            if (text != null)
            {
                text.text = textToDisplay;
                Misc.Msg($"[SignController] [SetLineText] Set Line: {line} to: {textToDisplay}");
            } else { Misc.Msg($"[SignController] [SetLineText] Failed To Set Line: {line} to: {textToDisplay}. Text == null"); }
            //if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            //{
            //    if (idx != null)
            //    {
            //        (ulong uSteamId, string sSteamId) = Misc.MySteamId();
            //        if (string.IsNullOrEmpty(sSteamId))
            //        {
            //            Misc.Msg("[SignController] [Sendt Update Text Event] Could Not Get My SteamId, Cant Be Sure Who Has The Sign, Skipped");
            //            return;
            //        }
            //        if (idx == 1)
            //        {
            //            SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.UpdateText
            //            {
            //                Sender = sSteamId,
            //                Line1Text = _line1.text,
            //                Line2Text = null,
            //                Line3Text = null,
            //                Line4Text = null,
            //                UniqueId = UniqueId,
            //                ToSteamId = "None"
            //            });
            //        } else if (idx == 2) {
            //            SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.UpdateText
            //            {
            //                Sender = sSteamId,
            //                Line1Text = null,
            //                Line2Text = _line2.text,
            //                Line3Text = null,
            //                Line4Text = null,
            //                UniqueId = UniqueId,
            //                ToSteamId = "None"
            //            });
            //        }
            //        else if (idx == 3)
            //        {
            //            SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.UpdateText
            //            {
            //                Sender = sSteamId,
            //                Line1Text = null,
            //                Line2Text = null,
            //                Line3Text = _line3.text,
            //                Line4Text = null,
            //                UniqueId = UniqueId,
            //                ToSteamId = "None"
            //            });
            //        }
            //        else if (idx == 4)
            //        {
            //            SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.UpdateText
            //            {
            //                Sender = sSteamId,
            //                Line1Text = null,
            //                Line2Text = null,
            //                Line3Text = null,
            //                Line4Text = _line4.text,
            //                UniqueId = UniqueId,
            //                ToSteamId = "None"
            //            });
            //        }
            //    }
            //}
        }

        public void SetAllText(string line1, string line2, string line3, string line4)
        {
            if (!string.IsNullOrEmpty(line1)) { SetLineText(1, line1); }
            if (!string.IsNullOrEmpty(line2)) { SetLineText(2, line2); }
            if (!string.IsNullOrEmpty(line3)) { SetLineText(3, line3); }
            if (!string.IsNullOrEmpty(line4)) { SetLineText(4, line4); }
        }

        public void OpenSignUi()
        {
            if (_line1.text == null) { Misc.Msg("Line1 is null"); return; }
            if (_line2.text == null) { Misc.Msg("Line2 is null"); return; }
            if (_line3.text == null) { Misc.Msg("Line3 is null"); return; }
            if (_line4.text == null) { Misc.Msg("Line4 is null"); return; }
            if (_line1.text == "Press E")
            {
                UI.Setup.SetLineText(1, "");
                UI.Setup.SetLineText(2, "");
                UI.Setup.SetLineText(3, "");
                UI.Setup.SetLineText(4, "");
            }
            else
            {
                UI.Setup.SetLineText(1, _line1.text);
                UI.Setup.SetLineText(2, _line2.text);
                UI.Setup.SetLineText(3, _line3.text);
                UI.Setup.SetLineText(4, _line4.text);
            }
            Prefab.ActiveSign.activeSign = gameObject;
            if (Prefab.ActiveSign.activeSign == null) { Misc.Msg("Active Sign is null after assigning, something is wrong"); return; }
            UI.Setup.OpenUI();
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
    }
}
