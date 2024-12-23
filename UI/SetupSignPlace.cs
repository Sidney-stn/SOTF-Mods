using Sons.Gui;
using UnityEngine;

namespace Signs.UI
{
    public class SetupSignPlace
    {
        internal static GameObject AddUI = null;

        internal static void SetupUi()
        {
            if (AddUI == null)
            {
                Misc.Msg("Setup SignPlaceUi");
                AddUI = GameObject.Instantiate(Assets.SignPlaceUI);
                AddUI.hideFlags = HideFlags.HideAndDontSave;
                AddUI.SetActive(false);
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
                PauseMenu._instance.Open();
                if (!AddUI.active) { AddUI.SetActive(true); }
            }
        }
        public static void CloseUI()
        {
            if (AddUI != null)
            {
                if (AddUI.active) { AddUI.SetActive(false); }
            }

        }
    }
}
