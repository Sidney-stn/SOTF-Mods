using RedLoader;
using Sons.Input;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.UI;


namespace WirelessSignals.UI
{
    internal class ReciverUI
    {
        public static GameObject UiElement = null;
        public static Text messageText = null;
        public static Button updateBtn = null;
        public static Button closeBtn = null;
        public static Sons.Input.InputCursorState inputCursorState = null;
        public static Sons.Input.InputActionMapState inputActionMapState = null;

        public static GameObject activeReciverPrefab = null;

        // Custom Elements Spesific To This Ui
        public static Text linkedToObjTxt = null;
        public static Slider rangeSlider = null;
        public static Text rangeSliderValueTxt = null;
        public static Dropdown linkElementDropdown = null;
        public static HashSet<string> allowedObjectsInDropdown = new HashSet<string>
        {
            ""
        };  // Only LowerCase Allowed Here
        private static Dictionary<string, string> dropdownValueMapping = new Dictionary<string, string>();

        internal static void SetupUI()
        {
            if (UiElement == null)
            {
                Misc.Msg("Setup Ui");
                if (Assets.ReciverUI == null)
                {
                    RLog.Error("ReciverUI is null!");
                    return;
                }
                UiElement = GameObject.Instantiate(Assets.ReciverUI);
                UiElement.DontDestroyOnLoad().HideAndDontSave();
                UiElement.SetActive(false);
            }

            if (closeBtn == null)
            {
                closeBtn = UiElement.transform.FindDeepChild("CloseUi").GetComponent<Button>();  // Close UI Button
                Action closeUiAction = () =>
                {
                    CloseUI();
                };
                closeBtn.onClick.AddListener(closeUiAction);
            }

            if (messageText == null) { messageText = UiElement.transform.FindDeepChild("MessageText").GetComponent<Text>(); messageText.text = ""; }  // Message Text
            if (linkedToObjTxt == null) { linkedToObjTxt = UiElement.transform.FindDeepChild("LinkedTxt").GetComponent<Text>(); linkedToObjTxt.text = "Linked To Object: Unkown"; }  // Linked To Object Value Text
            if (rangeSlider == null) { rangeSlider = UiElement.transform.FindDeepChild("RangeSlider").GetComponent<Slider>(); }  // Range Slider
            if (rangeSliderValueTxt == null) { rangeSliderValueTxt = UiElement.transform.FindDeepChild("RangeVal").GetComponent<Text>(); rangeSliderValueTxt.text = "?"; }  // Range Slider Value Text
            if (linkElementDropdown == null) { linkElementDropdown = UiElement.transform.FindDeepChild("InRangeDropdown").GetComponent<Dropdown>(); }  // Link Element Dropdown

            if (updateBtn == null)
            {
                updateBtn = UiElement.transform.FindDeepChild("UpdateButton").GetComponent<Button>();  // Update Text Button
                Action updateUi = () =>
                {
                    if (activeReciverPrefab == null) 
                    {
                        Misc.Msg("[ReciverUI] [OnUpdateButtonPress] Could not save, No Active Reciver");
                        CloseUI();
                        SonsTools.ShowMessage("Could not save, No Active Reciver");
                        return;

                    }

                    Mono.Reciver controller = activeReciverPrefab.GetComponent<Mono.Reciver>();
                    //controller._linked

                    // Network
                };
                updateBtn.onClick.AddListener(updateUi);
            }

            if (inputCursorState == null)
            {
                inputCursorState = UiElement.AddComponent<InputCursorState>();
                inputCursorState._enabled = true;
                inputCursorState._hardwareCursor = true;
                inputCursorState._priority = 100;
            }
            if (inputActionMapState == null)
            {
                inputActionMapState = UiElement.AddComponent<InputActionMapState>();
                inputActionMapState._applyState = InputState.Console;
            }

            // Add Mapping Values
            dropdownValueMapping.Add("None", "None");
        }

        internal static void SetToDefault()
        {
            if (UiElement != null)
            {
                if (UiElement.active) { UiElement.SetActive(false); }
            }
            activeReciverPrefab = null;
            if (messageText != null)
            {
                messageText.text = "";
            }
            if (linkedToObjTxt != null)
            {
                linkedToObjTxt.text = "Linked To Object: Unkown";
            }
            if (rangeSlider != null)
            {
                rangeSlider.value = 1;
            }
            if (rangeSliderValueTxt != null)
            {
                rangeSliderValueTxt.text = "?";
            }
            SetDropDownOptionsToNone();
        }

        internal static void SetDropDownOptionsToNone()
        {
            if (linkElementDropdown != null)
            {
                linkElementDropdown.ClearOptions();

                Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> options = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
                options.Add(new Dropdown.OptionData("None"));
                linkElementDropdown.AddOptions(options);
                linkElementDropdown.value = 0;
            }
        }

        internal static void SetUiFromObj()
        {
            if (activeReciverPrefab == null)
            {
                CloseUI();
                return;
            }
            Mono.Reciver controller = activeReciverPrefab.GetComponent<Mono.Reciver>();
            if (controller == null)
            {
                CloseUI();
                return;
            }
            if (linkedToObjTxt != null)  // Set Linked To Object Text (Linked Text)
            {
                linkedToObjTxt.text = $"Linked To Object: {controller.IsLinkedReciverObject()}";
            }
            if (rangeSlider != null)  // Set Slider Value (Range Slider)
            {
                rangeSlider.value = controller.objectRange;
            }
            if (rangeSliderValueTxt != null)  // Set Range Slider Value Text (Range Text)
            {
                rangeSliderValueTxt.text = controller.objectRange.ToString();
            }
            if (linkElementDropdown != null)
            {
                bool isAlreadyLinked = controller.IsLinkedReciverObject();
                List<GameObject> objectsInRange = controller.GetObjectsInRange();
                foreach (GameObject obj in objectsInRange)
                {
                    if (obj == null) { continue; }

                    // Clean up the name by removing whitespace and (Clone)
                    string cleanName = obj.name
                        .Replace("(Clone)", "") // Remove all instances of (Clone)
                        .Trim() // Remove leading and trailing whitespace
                        .ToLower(); // Make it lowercase

                    // Skip if this name is already in the dropdown
                    if (allowedObjectsInDropdown.Contains(cleanName)) { continue; }

                    allowedObjectsInDropdown.Add(cleanName);
                    Dropdown.OptionData option = new Dropdown.OptionData(cleanName);
                    linkElementDropdown.options.Add(option);
                }
                if (isAlreadyLinked)
                {
                    string linkedName = controller.GetLinkedReciverObjectName();
                }
            }
        }

        public static void ToggleUi()  // Not In Use
        {
            if (UiElement != null)
            {
                if (UiElement.active) { UiElement.SetActive(false); }
                if (!UiElement.active) { UiElement.SetActive(true); }
            }
        }
        public static void OpenUI()
        {
            if (UiElement != null)
            {
                if (activeReciverPrefab == null) { return; }
                if (!UiElement.active) { UiElement.SetActive(true); }
            }
            if (messageText != null)
            {
                messageText.text = "";
            }
        }
        public static void CloseUI()
        {
            if (UiElement != null)
            {
                if (UiElement.active) { UiElement.SetActive(false); }
            }
            SetToDefault();
        }

        public static void UpdateUiDataFromObj()
        {
            if (activeReciverPrefab == null)
            {
                CloseUI();
                return;
            } 
        }

    }
}
