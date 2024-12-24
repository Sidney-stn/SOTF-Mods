using Sons.Gui;
using SonsSdk;
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

                Button closeBtn = AddUI.transform.FindDeepChild("CloseUi")?.GetComponent<Button>();
                if (closeBtn != null)
                {
                    Action closeUiAction = () =>
                    {
                        CloseUI();
                        if (PauseMenu._instance != null)
                        {
                            PauseMenu._instance.Close();
                        }
                    };
                    closeBtn.onClick.AddListener(closeUiAction);
                }
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
            if (PauseMenu._instance.CanBeOpened()) { PauseMenu._instance.Open(); }
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

            GameObject allWarpsParent = AddUI.transform.FindDeepChild("AllWarps").gameObject;
            if (allWarpsParent == null)
            {
                Misc.Msg("[DeleteAllWarpsUI] [Error] AllWarps gameObject is null");
                return;
            }

            if (allWarpsParent.transform.childCount > 0)
            {
                foreach (Transform child in allWarpsParent.transform.GetChildren())
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
            if (AddUI == null)
            {
                Misc.Msg("[AddAllWarpsUI] [Error] AddUI is null");
                return;
            }

            // Find ItemContainer1 first with safety check
            var itemContainer = AddUI.transform.FindDeepChild("ItemContainer1");
            if (itemContainer == null)
            {
                Misc.Msg("[AddAllWarpsUI] [Error] ItemContainer1 not found");
                return;
            }

            // Find or create AllWarps parent
            GameObject _allWarpsParent = null;
            var existingAllWarps = AddUI.transform.FindDeepChild("AllWarps");
            if (existingAllWarps != null)
            {
                _allWarpsParent = existingAllWarps.gameObject;
            }
            else
            {
                _allWarpsParent = new GameObject("AllWarps");
                _allWarpsParent.transform.SetParent(itemContainer, false); // false to keep world position
            }

            // Find WarpTemplate with safety check
            var templateTransform = AddUI.transform.FindDeepChild("WarpTemplate");
            if (templateTransform == null)
            {
                Misc.Msg("[AddAllWarpsUI] [Error] WarpTemplate not found");
                return;
            }
            GameObject warpTemplate = templateTransform.gameObject;

            int index = 0;
            foreach (var warp in Saving.LoadedWarps.loadedWarps)
            {
                if (string.IsNullOrEmpty(warp.Key)) { continue; }
                if (warp.Value == Vector3.zero) { continue; }

                GameObject newWarp = GameObject.Instantiate(warpTemplate, _allWarpsParent.transform);
                if (newWarp == null)
                {
                    Misc.Msg("[AddAllWarpsUI] Instantiated GameObject is Null!");
                    continue;
                }

                newWarp.SetActive(true);

                newWarp.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 68f * index);

                var textComponent = newWarp.transform.FindChild("Text");
                if (textComponent == null)
                {
                    Misc.Msg("[AddAllWarpsUI] Text component not found");
                    continue;
                }
                textComponent.GetComponent<Text>().text = warp.Key;

                Button acceptBtn = newWarp.transform.FindDeepChild("accept")?.GetComponent<Button>();
                if (acceptBtn != null)
                {
                    Action acceptUiAction = () =>
                    {
                        Misc.Msg($"Warping to {warp.Key}");
                        SonsTools.ShowMessage($"Warping to {warp.Key}");
                        CloseUI();
                        if (PauseMenu._instance != null)
                        {
                            PauseMenu._instance.Close();
                        }

                        Player.MoveLocalPlayer(warp.Value);
                    };
                    acceptBtn.onClick.AddListener(acceptUiAction);
                }

                Button deleteBtn = newWarp.transform.FindDeepChild("delete")?.GetComponent<Button>();
                if (deleteBtn != null)
                {
                    Action deleteUiAction = () =>
                    {
                        DeleteSelectedWarp(warpName: warp.Key);
                    };
                    deleteBtn.onClick.AddListener(deleteUiAction);
                }
                index++;
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
                    // Maybe Add Permission Check Here
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.DeleteWarp
                    {
                        WarpName = warpName
                    });
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
