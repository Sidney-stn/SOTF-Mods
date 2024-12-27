
using Sons.Gui;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;

namespace Banking.UI
{
    internal class FunctionsFromUI
    {
        internal static void DepositCashFromUi()
        {
            if (UI.Setup.depositInput == null) { 
                Misc.Msg("[FunctionsFromUI] [DepositCashFromUi] InputField Is Invalid");
                SonsTools.ShowMessage("Ops! Something went wrong!", 5);
                if (UI.Setup.messageText != null) { UI.Setup.messageText.text = "Ops! Something went wrong!"; }
                return;
            }
            if (UI.Setup.depositInput.text == null)
            {
                Misc.Msg("[FunctionsFromUI] [DepositCashFromUi] InputField Text Is Invalid");
                SonsTools.ShowMessage("Ops! Something went wrong!", 5);
                if (UI.Setup.messageText != null) { UI.Setup.messageText.text = "Ops! Something went wrong!"; }
                return;
            }
            // Try Parse String To Int For Deposit Amount
            if (!int.TryParse(UI.Setup.depositInput.text, out int depositAmount))
            {
                Misc.Msg("[FunctionsFromUI] [DepositCashFromUi] InputField Text Is Invalid");
                SonsTools.ShowMessage("Only Numbers Accepted!", 5);
                if (UI.Setup.messageText != null) { UI.Setup.messageText.text = "Only Numbers Accepted!"; }
                return;
            }
            else
            {
                if (depositAmount <= 0) { 
                    Misc.Msg("[FunctionsFromUI] [DepositCashFromUi] INVALID Deposit Amount Is Less Than 0 or = 0");
                    if (UI.Setup.messageText != null) { UI.Setup.messageText.text = "Only Positive Accepted!"; }
                    return;
                }
                Misc.Msg($"[FunctionsFromUI] [DepositCashFromUi] Deposit Amount: {depositAmount}");

                // Check If Player Has Enough Cash
                if (LiveData.LocalPlayerData.GetLocalPlayerCurrency() < depositAmount)
                {
                    Misc.Msg("[FunctionsFromUI] [DepositCashFromUi] Not Enough Cash To Deposit");
                    SonsTools.ShowMessage("Not Enough Cash To Deposit", 5);
                    if (UI.Setup.messageText != null) { UI.Setup.messageText.text = "Not Enough Cash To Deposit"; }
                    return;
                }

                // Remove Cash From Player Via Network
                LiveData.LocalPlayerData.RemoveCashFromLocalPlayer(depositAmount);

                // Add Cash To Player Via Network
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.AddCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    Currency = depositAmount,
                    ToPlayerId = "None"
                });

            }
            // Deposit Cash
            if (UI.Setup.messageText != null) { UI.Setup.messageText.text = "Deposited Cash"; }
        }
        internal static void WithdrawCashFromUi()
        {
            if (UI.Setup.withdrawInput == null)
            {
                Misc.Msg("[FunctionsFromUI] [WithdrawCashFromUi] InputField Is Invalid");
                SonsTools.ShowMessage("Ops! Something went wrong!", 5);
                if (UI.Setup.messageText != null) { UI.Setup.messageText.text = "Ops! Something went wrong!"; }
                return;
            }

            if (UI.Setup.withdrawInput.text == null)
            {
                Misc.Msg("[FunctionsFromUI] [WithdrawCashFromUi] InputField Text Is Invalid");
                SonsTools.ShowMessage("Ops! Something went wrong!", 5);
                if (UI.Setup.messageText != null) { UI.Setup.messageText.text = "Ops! Something went wrong!"; }
                return;
            }

            // Try Parse String To Int For Withdraw Amount
            if (!int.TryParse(UI.Setup.withdrawInput.text, out int withdrawAmount))
            {
                Misc.Msg("[FunctionsFromUI] [WithdrawCashFromUi] InputField Text Is Invalid");
                SonsTools.ShowMessage("Only Numbers Accepted!", 5);
                if (UI.Setup.messageText != null) { UI.Setup.messageText.text = "Only Numbers Accepted!"; }
                return;
            }
            else
            {
                if (withdrawAmount <= 0)
                {
                    Misc.Msg("[FunctionsFromUI] [DepositCashFromUi] INVALID Deposit Amount Is Less Than or = 0");
                    if (UI.Setup.messageText != null) { UI.Setup.messageText.text = "Only Positive Accepted!"; }
                    return;
                }
                Misc.Msg($"[FunctionsFromUI] [WithdrawCashFromUi] Withdraw Amount: {withdrawAmount}");

                // Add Cash To Player Via Network
                LiveData.LocalPlayerData.AddCashToLocalPlayer(withdrawAmount);

                // Remove Cash From Player Via Network
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RemoveCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    Currency = withdrawAmount,
                    ToPlayerId = "None"
                });
            }

            // Withdraw Cash
            if (UI.Setup.messageText != null) { UI.Setup.messageText.text = "Withdrew Cash"; }
        }

        internal static void TryOpenUi()
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
            if (raycastHit.collider.transform.root.name.Contains("ATM"))
            {
                GameObject openAtm = raycastHit.collider.transform.root.gameObject;
                Mono.ATMController aTMController = openAtm.GetComponent<Mono.ATMController>();
                if (aTMController != null)
                {
                    Misc.Msg("Opening Sign Ui");
                    aTMController.OpenATMUi();
                }
                else { Misc.Msg("ATMController is null!"); }

            }
        }
    }
}
