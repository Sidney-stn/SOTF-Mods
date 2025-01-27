using Signs.Mono;
using Sons.Gui;
using Sons.Input;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Signs.UI
{
    internal class Setup
    {
        public static GameObject AddUI = null;
        public static InputField line1 = null;
        public static InputField line2 = null;
        public static InputField line3 = null;
        public static InputField line4 = null;
        public static Text messageText = null;
        public static Button updateBtn = null;
        public static Button closeBtn = null;
        public static Sons.Input.InputCursorState inputCursorState = null;
        public static Sons.Input.InputActionMapState inputActionMapState = null;

        public static void SetupUI()
        {
            if (AddUI == null)
            {
                Misc.Msg("Setup Ui");
                AddUI = GameObject.Instantiate(Assets.SignUi);
                AddUI.hideFlags = HideFlags.HideAndDontSave;
                AddUI.SetActive(false);
            }
            if (closeBtn == null)
            {
                closeBtn = AddUI.transform.FindDeepChild("CloseUi").GetComponent<Button>();  // Close UI Button
                Action closeUiAction = () =>
                {
                    CloseUI();
                };
                closeBtn.onClick.AddListener(closeUiAction);
            }
            if (line1 == null) { line1 = AddUI.transform.FindDeepChild("Line1").GetComponent<InputField>(); }  // Line 1 Text
            if (line2 == null) { line2 = AddUI.transform.FindDeepChild("Line2").GetComponent<InputField>(); }  // Line 2 Text
            if (line3 == null) { line3 = AddUI.transform.FindDeepChild("Line3").GetComponent<InputField>(); }  // Line 3 Text
            if (line4 == null) { line4 = AddUI.transform.FindDeepChild("Line4").GetComponent<InputField>(); }  // Line 4 Text
            if (messageText == null) { messageText = AddUI.transform.FindDeepChild("MessageText").GetComponent<Text>(); messageText.text = ""; }  // Message Text

            if (updateBtn == null)
            {
                updateBtn = AddUI.transform.FindDeepChild("UpdateButton").GetComponent<Button>();  // Update Text Button
                Action updateUi = () =>
                {
                    if (Prefab.ActiveSign.activeSign == null) { return; }
                    SignController signController = Prefab.ActiveSign.activeSign.GetComponent<SignController>();
                    signController.SetAllText(line1.text, line2.text, line3.text, line4.text);
                    (ulong steamId, string stringSteamId) = Misc.MySteamId();
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.UpdateText
                    {
                        Sender = stringSteamId,
                        Line1Text = line1.text,
                        Line2Text = line2.text,
                        Line3Text = line3.text,
                        Line4Text = line4.text,
                        UniqueId = signController.UniqueId,
                        ToSteamId = "None"
                    });
                };
                updateBtn.onClick.AddListener(updateUi);
            }
            if (inputCursorState == null)
            {
                inputCursorState = AddUI.AddComponent<InputCursorState>();
                inputCursorState._enabled = true;
                inputCursorState._hardwareCursor = true;
                inputCursorState._priority = 100;
            }
            if (inputActionMapState == null)
            {
                inputActionMapState = AddUI.AddComponent<InputActionMapState>();
                inputActionMapState._applyState = InputState.Console;
            }

            // Set Default Text On Sign
            if (line1 != null && line2 != null && line3 != null && line4 != null && Config.ToggleMenuKey.Value != null && Config.ToggleMenuKey.Value != "")
            {
                line1.text = $"Press {Config.ToggleMenuKey.Value}";
                line2.text = "To Edit";
                line3.text = "Sign";
                line4.text = "";
            }
        }
        public static void UpdateUiOpenKey()
        {
            Misc.Msg("[UpdateUiOpenKey]");
            // Set Default Text On Sign
            if (line1 != null && line2 != null && line3 != null && line4 != null && Config.ToggleMenuKey.Value != null && Config.ToggleMenuKey.Value != "")
            {
                line1.text = $"Press {Config.ToggleMenuKey.Value.ToUpper()}";
                line2.text = "To Edit";
                line3.text = "Sign";
                line4.text = "";
            }
        }
        public static void ToggleUi()  // Not In Use
        {
            if (AddUI != null)
            {
                if (AddUI.active) { AddUI.SetActive(false); }
                if (!AddUI.active) { AddUI.SetActive(true); }
            }
        }
        public static void OpenUI()
        {
            if (AddUI != null)
            {
                if (Prefab.ActiveSign.activeSign == null) { return; }
                if (!AddUI.active) { AddUI.SetActive(true); }
            }
            if (messageText != null)
            {
                messageText.text = "";
            }
        }
        public static void CloseUI()
        {
            if (AddUI != null)
            {
                if (AddUI.active) { AddUI.SetActive(false); }
            }
            Prefab.ActiveSign.activeSign = null;
            if (messageText != null)
            {
                messageText.text = "";
            }
        }
        public static void SetLineText(int line, string textToDisplay)
        {
            if (AddUI != null)
            {
                switch (line)
                {
                    case 1:
                        if (line1 != null)
                        {
                            line1.text = textToDisplay;
                        }
                        break;
                    case 2:
                        if (line2 != null)
                        {
                            line2.text = textToDisplay;
                        }
                        break;
                    case 3:
                        if (line3 != null)
                        {
                            line3.text = textToDisplay;
                        }
                        break;
                    case 4:
                        if (line4 != null)
                        {
                            line4.text = textToDisplay;
                        }
                        break;
                }
            }
        }

        public static void TryOpenUi()
        {
            if (!LocalPlayer.IsInWorld || LocalPlayer.IsInInventory || PauseMenu.IsActive) { return; }
            Transform transform = LocalPlayer._instance._mainCam.transform;
            RaycastHit raycastHit;
            Physics.Raycast(transform.position, transform.forward, out raycastHit, 5f, LayerMask.GetMask(new string[]
            {
                "Default"
            }));
            if (raycastHit.collider == null) { return; }
            if (raycastHit.collider.transform.root == null) { return; }
            if (string.IsNullOrEmpty(raycastHit.collider.transform.root.name)) { return; }
            Misc.Msg($"Hit: {raycastHit.collider.transform.root.name}");
            if (raycastHit.collider.transform.root.name.Contains("Sign"))
            {
                GameObject openSign = raycastHit.collider.transform.root.gameObject;
                SignController signController = openSign.GetComponent<SignController>();
                if (signController != null)
                {
                    Misc.Msg("Opening Sign Ui");
                    signController.OpenSignUi();
                } else { Misc.Msg("SignController is null!"); }

            }
        }
    }
}
