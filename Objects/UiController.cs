

using RedLoader;
using Sons.Gui;
using TheForest.Utils;

namespace StoneGate.Objects
{
    internal static class UiController
    {
        private static HashSet<string> allowedModes = new HashSet<string>()
        {
            "MARK",
            "ROTATE",
            "DELETE"
        };

        public static void ChangeMode()
        {
            if (LocalPlayer.IsInWorld == false || LocalPlayer.IsInInventory || PauseMenu.IsActive || LocalPlayer.InWater) { return; }
            if (StoneGateUi.panelText.Value == "UNKOWN")
            {
                StoneGateUi.panelText.Value = "MARK";
            }
            else if (StoneGateUi.panelText.Value == "MARK")
            {
                StoneGateUi.panelText.Value = "ROTATE";
            }
            else if (StoneGateUi.panelText.Value == "ROTATE")
            {
                StoneGateUi.panelText.Value = "DELETE";
            }
            else if (StoneGateUi.panelText.Value == "DELETE")
            {
                StoneGateUi.panelText.Value = "MARK";
            }
            else
            {
                StoneGateUi.panelText.Value = "MARK";
            }

            if (StoneGateUi.panelText.Value == "MARK" || StoneGateUi.panelText.Value == "ROTATE")
            {
                if (StoneGate.StoneGateToolUI == null) { RLog.Error("[StoneGate] [UiController] [ChangeMode] StoneGateToolUI is null"); return; }
                StoneGate.StoneGateToolUI.SetActive(true);
            } 
            else if (StoneGateUi.panelText.Value == "DELETE")
            {
                if (StoneGate.StoneGateToolUI == null) { RLog.Error("[StoneGate] [UiController] [ChangeMode] StoneGateToolUI is null"); return; }
                StoneGate.StoneGateToolUI.SetActive(false);
            }
        }

        public static void SetMode(string mode)
        {
            if (allowedModes.Contains(mode))
            {
                StoneGateUi.panelText.Value = mode;
            }
        }

        public static string GetMode()
        {
            return StoneGateUi.panelText.Value;
        }

        public static HashSet<string> GetAllowedModes()
        {
            return allowedModes;
        }

        public static bool IsValidMode(string mode)
        {
            return allowedModes.Contains(mode);
        }
    }
}
