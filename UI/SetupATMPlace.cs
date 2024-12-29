using UnityEngine;

namespace Banking.UI
{
    internal class SetupATMPlace
    {
        internal static GameObject AddUI = null;

        internal static void SetupUi()
        {
            if (AddUI == null)
            {
                Misc.Msg("Setup ATMPlaceUi");
                AddUI = GameObject.Instantiate(Assets.ATMPlaceUI);
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
