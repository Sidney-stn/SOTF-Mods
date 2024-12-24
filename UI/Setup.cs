using UnityEngine;
using UnityEngine.UI;

namespace Warps.UI
{
    internal class Setup
    {
        internal static GameObject AddUI = null;
        public static Text messageText = null;

        internal static void SetupUi()
        {
            if (AddUI == null)
            {
                Misc.Msg("Setup WarpsUi");
                AddUI = GameObject.Instantiate(Assets.WarpsUI);
                AddUI.hideFlags = HideFlags.HideAndDontSave;
                AddUI.SetActive(false);

                GameObject warpTemplate = AddUI.transform.FindDeepChild("WarpTemplate").gameObject;
                warpTemplate.SetActive(false);

                if (messageText == null) { messageText = AddUI.transform.FindDeepChild("MessageText").GetComponent<Text>(); messageText.text = ""; }  // Message Text
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

        public static bool IsUiOpen()
        {
            if (AddUI != null)
            {
                return AddUI.active;
            }
            return false;
        }

        private static void DeleteAllWarpsUI()
        {
            if (AddUI == null)
            {
                Misc.Msg("[DeleteAllWarpsUI] [Error] AddUI is null");
                return;
            }

            var allWarpsTransform = AddUI.transform.FindDeepChild("AllWarps");
            if (allWarpsTransform == null)
            {
                Misc.Msg("[DeleteAllWarpsUI] [Info] AllWarps transform not found");
                return;
            }

            GameObject allWarpsParent = allWarpsTransform.gameObject;
            if (allWarpsParent == null)
            {
                Misc.Msg("[DeleteAllWarpsUI] [Error] AllWarps gameObject is null");
                return;
            }

            if (allWarpsParent.transform.childCount > 0)
            {
                foreach (Transform child in allWarpsParent.transform)
                {
                    if (child != null)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }
        }

        private static void AddAllWarpsUI()
        {
            GameObject _allWarpsParent = AddUI.transform.FindDeepChild("AllWarps").gameObject;
            if (_allWarpsParent == null)  // Creates AllWarps Parent If Not Found
            {
                _allWarpsParent = new GameObject("AllWarps");
                _allWarpsParent.transform.SetParent(AddUI.transform.FindDeepChild("ItemContainer1"));
            }

            GameObject warpTemplate = AddUI.transform.FindDeepChild("WarpTemplate").gameObject;
            foreach (var warp in Saving.LoadedWarps.loadedWarps)
            {
                if (string.IsNullOrEmpty(warp.Key)) { continue; }
                if (warp.Value == Vector3.zero) { continue; }

                GameObject newWarp = GameObject.Instantiate(warpTemplate, _allWarpsParent.transform);
                if (newWarp == null) { Misc.Msg("[AddAllWarpsUI] Instantiated GameObject is Null!"); continue; }

                newWarp.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 68f);

                newWarp.transform.FindChild("Text").GetComponent<Text>().text = warp.Key;

                Button acceptBtn = newWarp.transform.FindDeepChild("accept").GetComponent<Button>();  // Accept Warp Button
                if (acceptBtn != null)
                {
                    Action acceptUiAction = () =>
                    {
                        Player.MoveLocalPlayer(warp.Value);
                    };
                    acceptBtn.onClick.AddListener(acceptUiAction);
                }

                Button deleteBtn = newWarp.transform.FindDeepChild("delete").GetComponent<Button>();  // Accept Warp Button
                if (deleteBtn != null)
                {
                    Action deleteUiAction = () =>
                    {
                        DeleteSelectedWarp(warpName: warp.Key);
                    };
                    deleteBtn.onClick.AddListener(deleteUiAction);
                }
            }
        }

        private static void DeleteSelectedWarp(string warpName)
        {
            if (string.IsNullOrEmpty(warpName)) { return; }
            if (Saving.LoadedWarps.loadedWarps.ContainsKey(warpName))
            {
                if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient || Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer)
                {
                    // Delete Warp Over Network
                }
                Saving.LoadedWarps.loadedWarps.Remove(warpName);
                CloseUI();
                TryOpenUi();
            }
        }

        public static void TryOpenUi()
        {
            if (AddUI == null)
            {
                SetupUi();
            }

            DeleteAllWarpsUI();
            AddAllWarpsUI();
            OpenUI();
        }
    }
}
