using UnityEngine.UI;
using UnityEngine;
using Sons.Gui;

namespace Banking.UI
{
    internal class Setup
    {
        internal static GameObject AddUI = null;
        internal static Text messageText = null;
        internal static Button closeBtn = null;
        internal static Button depositBtn = null;
        internal static Button withdrawBtn = null;
        internal static InputField depositInput = null;
        internal static InputField withdrawInput = null;

        internal static void SetupUI()
        {
            if (AddUI == null)
            {
                Misc.Msg("Setup Ui");
                AddUI = GameObject.Instantiate(Assets.BankingUI);
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
            if (messageText == null) { messageText = AddUI.transform.FindDeepChild("MessageText").GetComponent<Text>(); messageText.text = ""; }  // Message Text
            if (depositBtn == null)
            {
                depositBtn = AddUI.transform.FindDeepChild("DepositButton").GetComponent<Button>();  // Deposit Button
                Action depositAction = () =>
                {
                    FunctionsFromUI.DepositCashFromUi();
                };
                depositBtn.onClick.AddListener(depositAction);
            }
            if (withdrawBtn == null)
            {
                withdrawBtn = AddUI.transform.FindDeepChild("WithdrawButton").GetComponent<Button>();  // Withdraw Button
                Action withdrawAction = () =>
                {
                    FunctionsFromUI.WithdrawCashFromUi();
                };
                withdrawBtn.onClick.AddListener(withdrawAction);
            }
            if (depositInput == null) { depositInput = AddUI.transform.FindDeepChild("DepositAmount").GetComponent<InputField>(); }  // Deposit Input
            if (withdrawInput == null) { withdrawInput = AddUI.transform.FindDeepChild("WithdrawAmount").GetComponent<InputField>(); }  // Withdraw Input
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
                if (Prefab.ActiveATM.activeAtm == null) { return; }
                PauseMenu._instance.Open();
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
            Prefab.ActiveATM.activeAtm = null;
            if (messageText != null)
            {
                messageText.text = "";
            }
        }

        internal static void UpdateUiIfOpen()
        {

        }
    }
}
