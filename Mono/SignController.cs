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
                text.text = textToDisplay;
                Misc.Msg($"[SignController] [SetLineText] Set Line: {line} to: {textToDisplay}");
            } else { Misc.Msg($"[SignController] [SetLineText] Failed To Set Line: {line} to: {textToDisplay}. Text == null"); }
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
