using UnityEngine;
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

    public static class UnityUi
    {
        public static List<GameObject> UnityElements = new List<GameObject>();
        public static void AddUnityElement(GameObject unityUIGameObject)
        {
            if (unityUIGameObject == null)
            {
                return;
            }
            if (UnityElements.Contains(unityUIGameObject))
            {
                return;
            }
            UnityElements.Add(unityUIGameObject);
        }
        public static void RemoveUnityElement(GameObject unityUIGameObject)
        {
            if (unityUIGameObject == null)
            {
                return;
            }
            if (UnityElements.Contains(unityUIGameObject))
            {
                UnityElements.Remove(unityUIGameObject);
            }
        }

        internal static bool IsPanelActive(GameObject unityUIGameObject)
        {
            if (unityUIGameObject == null)
            {
                return false;
            }
            return unityUIGameObject.active;
        }
    }
}
