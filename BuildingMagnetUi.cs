namespace BuildingMagnet;

using SUI;
using UnityEngine;
using static SUI.SUI;

public class BuildingMagnetUi
{
    public const string MagnetPanel = "MagnetPanel";
    public static Observable<string> panelText = new("NONE");
    public const string defaultPanelText = "NONE";
    public static void Create()
    {
        var panel = RegisterNewPanel(MagnetPanel, false)
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

        var text = SLabel.Text("NONE")
            .FontColor(Color.white)
            .Font(EFont.RobotoRegular)
            .FontSize(26)
            .Position(0, 0)
            .HFill()
            .VFill()
            .Bind(panelText);
        text.SetParent(mainContainer);

    }

    internal static void OpenMainPanel()
    {
        TogglePanel(MagnetPanel, true);
    }

    internal static void CloseMainPanel()
    {
        TogglePanel(MagnetPanel, false);
    }

    internal static void ToggleMainPanel()
    {
        TogglePanel(MagnetPanel);
    }
}