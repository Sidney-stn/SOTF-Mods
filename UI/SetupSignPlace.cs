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
            if (AddUI != null)
            {
            }
        }
        internal static void UpdateKeysInUI(string rotateLeftKey, string rotateRightKey)
        {
            if (AddUI != null)
            {
                AddUI.transform.FindDeepChild("RotateText").GetComponent<UnityEngine.UI.Text>().text = $"{rotateLeftKey} / {rotateRightKey}";
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
