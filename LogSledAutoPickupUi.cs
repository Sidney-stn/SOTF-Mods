using RedLoader;
using SUI;
using UnityEngine;
using static SUI.SUI;

namespace LogSledAutoPickup
{
    internal class LogSledAutoPickupUi
    {
        public const string AutoPickupPanel = "AutoPickupPanel";
        public static Observable<string> panelText = new("UNKNOWN");
        public const string defaultPanelText = "UNKNOWN";
        public static void Create()
        {
            var panel = RegisterNewPanel(AutoPickupPanel, false)
                .Anchor(AnchorType.BottomRight)
                .Background(Color.black)
                .Size(120, 60)
                .Position(-360, 100)
                .OverrideSorting(100);

            CloseMainPanel();

            var mainContainer = SContainer
                .Dock(EDockType.Fill)
                //.Background(Color.red, EBackground.RoundedStandard)
                .OverrideSorting(101);

            panel.Add(mainContainer);

            var text = SLabel.Text("UNKNOWN")
                .FontColor(Color.white)
                .Font(EFont.RobotoRegular)
                .FontSize(26)
                .Position(0, 0)
                .HFill()
                .VFill()
                .Bind(panelText);
            text.SetParent(mainContainer);


            LogSledAutoPickup.Msg("AutoPickupPanel Created");
            //OpenMainPanel();  // For testing purposes
        }

        internal static void OpenMainPanel()
        {
            TogglePanel(AutoPickupPanel, true);
        }

        internal static void CloseMainPanel()
        {
            TogglePanel(AutoPickupPanel, false);
        }

        internal static void ToggleMainPanel()
        {
            TogglePanel(AutoPickupPanel);
        }

        
    }
}
