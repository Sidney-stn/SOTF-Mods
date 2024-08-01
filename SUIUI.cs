using static SUI.SUI;

namespace HotKeyCommands
{
    public static class SUIUI
    {
        public static List<string> SuiElements = new List<string>();
        public static void AddSUIElemet(string sUIId)
        {
            if (SuiElements.Contains(sUIId))
            {
                return;
            }
            SuiElements.Add(sUIId);
        }
        public static void RemoveSUIElemet(string sUIId)
        {
            if (SuiElements.Contains(sUIId))
            {
                SuiElements.Remove(sUIId);
            }   
        }

        internal static bool IsPanelActive(string panelName)
        {
            return GetPanel(panelName).Root.activeSelf;
        }
    }
}
