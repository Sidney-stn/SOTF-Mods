

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
                StoneGateUi.panelText.Value = "MARK";
            }
            else
            {
                StoneGateUi.panelText.Value = "MARK";
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
