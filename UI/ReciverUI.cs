using Il2CppInterop.Runtime;
using RedLoader;
using Sons.Input;
using SonsSdk;
using System.Text.RegularExpressions;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.Events;
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
        public static Toggle showScanLine = null;
        public static Toggle revertOutput = null;
        public static HashSet<string> allowedObjectsInDropdown = new HashSet<string>
        {
            "DefensiveWallGate".ToLower()
        };  // Only LowerCase Allowed Here
        private static Dictionary<string, string> dropdownValueMapping = new Dictionary<string, string>();  // Gameobject Name Without Clone And Space (CleanName), DisplayName
        public static Dictionary<string, string> componentRequirements = new Dictionary<string, string>
        {
            { "defensivewallgate", "Construction.DefensiveWallGateControl" },
            //{ "otheritem", null },  // Example of item with no required component
            // Add other objects...
        };


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
            if (linkElementDropdown == null) { linkElementDropdown = UiElement.transform.FindDeepChild("InRangeDropdown").GetComponent<Dropdown>(); linkElementDropdown.ClearOptions(); }  // Link Element Dropdown
            if (rangeSlider != null)
            {
                rangeSlider.onValueChanged.RemoveAllListeners();

                // Create an Il2CppSystem.Collections.Generic.List for the UnityEvent
                var callbacks = new Il2CppSystem.Collections.Generic.List<UnityEngine.Events.UnityAction<float>>();

                // Create the callback
                UnityEngine.Events.UnityAction<float> callback = DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<float>>(
                    (float val) =>
                    {
                        if (activeReciverPrefab == null) { return; }
                        Mono.Reciver controller = activeReciverPrefab.GetComponent<Mono.Reciver>();
                        if (controller == null) { return; }
                        // Round the val to max 2 decimals
                        val = Mathf.Round(val * 100f) / 100f;
                        controller.objectRange = val;  // Set Range On Reciver
                        rangeSliderValueTxt.text = val.ToString();  // Update Range Text
                    });

                callbacks.Add(callback);
                rangeSlider.onValueChanged.AddListener(callback);
            }

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
                    if (controller == null)
                    {
                        Misc.Msg("[ReciverUI] [OnUpdateButtonPress] Could not save, No Reciver Component Found");
                        SonsTools.ShowMessage("Could not save, No Reciver Component Found");
                        return;
                    }
                    if (showScanLine != null)  // Set Scan Lines On Save
                    {
                        controller.ShowScanLines(showScanLine.isOn);
                        controller.SetScanObjectRange(controller.objectRange);
                    }
                    if (revertOutput != null)  // Set Revert Output On Save
                    {
                        controller._revertOutput = revertOutput.isOn;
                    }

                    string selectedVal = linkElementDropdown.options[linkElementDropdown.value].text;
                    if (selectedVal == "None")
                    {
                        controller.SetLinkedReciverObject(false, null);
                        SetUiFromObj();
                        Misc.Msg("[ReciverUI] [OnUpdateButtonPress] Unlinked Object");
                        SonsTools.ShowMessage("Unlinked Object");
                        //Misc.Msg("[ReciverUI] [OnUpdateButtonPress] Could not save, No Object Selected");
                        //SonsTools.ShowMessage("Could not save, No Object Selected");
                        // This Case Needs Updated, Float Value Does Change, but rescan should be done
                        return;
                    }

                    
                    
                    string itemName = GetOriginalName(selectedVal);
                    if (string.IsNullOrEmpty(itemName))
                    {
                        Misc.Msg("[ReciverUI] [OnUpdateButtonPress] Could not save, Item Name Is Null Or Empty");
                        SonsTools.ShowMessage("Could not save, Item Name Is Null Or Empty");
                    }
                    controller.SetLinkedReciverObject(true, itemName);

                    // Update UI
                    SetUiFromObj();

                    // Network
                };
                updateBtn.onClick.AddListener(updateUi);
            }

            
            if (inputCursorState == null)  // For Locking Cursor when UI is Open
            {
                inputCursorState = UiElement.AddComponent<InputCursorState>();
                inputCursorState._enabled = true;
                inputCursorState._hardwareCursor = true;
                inputCursorState._priority = 100;
            }
            if (inputActionMapState == null)  // For Locking Input When UI is Open
            {
                inputActionMapState = UiElement.AddComponent<InputActionMapState>();
                inputActionMapState._applyState = InputState.Console;
            }

            if (showScanLine == null)  // Scan Lines Hor And Vert Direction On Reciver For Showing Scan Area
            {
                showScanLine = UiElement.transform.FindDeepChild("ShowLinesButton").GetComponent<Toggle>();
                showScanLine.onValueChanged.RemoveAllListeners();

                // Set Toggle To Default False
                showScanLine.isOn = false;

                // Create an Il2CppSystem.Collections.Generic.List for the UnityEvent
                var callbacks = new Il2CppSystem.Collections.Generic.List<UnityEngine.Events.UnityAction<bool>>();

                // Create the callback
                UnityEngine.Events.UnityAction<bool> callback = DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<bool>>(
                    (bool val) =>
                    {
                        if (activeReciverPrefab == null) { return; }
                        Mono.Reciver controller = activeReciverPrefab.GetComponent<Mono.Reciver>();
                        if (controller == null) { return; }
                        controller.ShowScanLines(val);
                    });

                callbacks.Add(callback);
                showScanLine.onValueChanged.AddListener(callback);
            }

            if (revertOutput == null)
            {
                revertOutput = UiElement.transform.FindDeepChild("RevertOutputToggle").GetComponent<Toggle>();
                revertOutput.onValueChanged.RemoveAllListeners();
                // Set Toggle To Default False
                revertOutput.isOn = false;
                // Create an Il2CppSystem.Collections.Generic.List for the UnityEvent
                var callbacks = new Il2CppSystem.Collections.Generic.List<UnityEngine.Events.UnityAction<bool>>();
                // Create the callback
                UnityEngine.Events.UnityAction<bool> callback = DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<bool>>(
                    (bool val) =>
                    {
                        if (activeReciverPrefab == null) { return; }
                        Mono.Reciver controller = activeReciverPrefab.GetComponent<Mono.Reciver>();
                        if (controller == null) { return; }
                        controller._revertOutput = val;
                    });
                callbacks.Add(callback);
                revertOutput.onValueChanged.AddListener(callback);
            }

            // Add Mapping Values
            dropdownValueMapping.Add("None", "None");
            dropdownValueMapping.Add("DefensiveWallGate".ToLower(), "Wall Gate");
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
            if (showScanLine != null)
            {
                showScanLine.isOn = false;
            }
            if (revertOutput != null)
            {
                revertOutput.isOn = false;
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

        internal static void AddNoneToDropDown()
        {
            if (linkElementDropdown != null)
            {
                // Check if None is already in the dropdown
                foreach (Dropdown.OptionData option in linkElementDropdown.options)
                {
                    if (option.text == "None") { return; }
                }
                Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> options = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
                options.Add(new Dropdown.OptionData("None"));
                linkElementDropdown.AddOptions(options);
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
            if (linkElementDropdown != null)  // Set Dropdown Options (Link Element Dropdown)
            {
                if (linkElementDropdown.options.Count > 0)
                {
                    linkElementDropdown.ClearOptions();
                }
                AddNoneToDropDown();
                bool isAlreadyLinked = controller.IsLinkedReciverObject();
                HashSet<GameObject> objectsInRange = controller.GetObjectsInRange();
                foreach (GameObject obj in objectsInRange)
                {
                    if (obj == null) { continue; }

                    // Clean up the name by removing whitespace and (Clone)
                    string cleanName = obj.name.ToLower(); // Make it lowercase

                    cleanName = Regex.Replace(cleanName, @"\s+", "");  // Remove spaces
                    cleanName = Regex.Replace(cleanName, @"\d", "");  // Remove numbers
                    cleanName = cleanName.Replace("(clone)", "");  // Remove (clone)

                    // Skip if this name isn't in our mapping or is already in dropdown
                    if (!dropdownValueMapping.ContainsKey(cleanName)) {
                        Misc.Msg($"[ReciverUI] [SetUiFromObj] Skipping {cleanName} because it's not in the mapping");
                        continue;
                    }
                    if (allowedObjectsInDropdown.Contains(dropdownValueMapping[cleanName])) {
                        Misc.Msg($"[ReciverUI] [SetUiFromObj] Skipping {cleanName} because it's already in the dropdown");
                        continue;
                    }

                    //allowedObjectsInDropdown.Add(dropdownValueMapping[cleanName]);  // Not sure why i added this here
                    
                    Dropdown.OptionData option = new Dropdown.OptionData(dropdownValueMapping[cleanName]);
                    linkElementDropdown.options.Add(option);
                }
                if (isAlreadyLinked)
                {
                    string linkedName = controller.GetLinkedReciverObjectName();
                    // Clean the linked name the same way as we did for other names
                    string cleanLinkedName = linkedName
                        .Replace("(Clone)", "")
                        .Trim()
                        .ToLower();

                    // Only proceed if we have a mapping for this name
                    if (dropdownValueMapping.ContainsKey(cleanLinkedName))
                    {
                        // Get the display name from the mapping
                        string displayName = dropdownValueMapping[cleanLinkedName];

                        // Find the index by looping through options
                        for (int i = 0; i < linkElementDropdown.options.Count; i++)
                        {
                            if (linkElementDropdown.options[i].text == displayName)
                            {
                                linkElementDropdown.value = i;
                                linkElementDropdown.RefreshShownValue();
                                break;
                            }
                        }
                    }
                }
                if (objectsInRange.Count == 0)
                {
                    SetDropDownOptionsToNone();
                }
            }
            if (showScanLine != null)  // Set Scan Lines Toggle (Show Scan Lines)
            {
                showScanLine.isOn = controller.IsScanLinesShown();
            }
            if (revertOutput != null)  // Set Revert Output Toggle (Revert Output)
            {
                revertOutput.isOn = controller._revertOutput;
            }
        }

        //string displayName = linkElementDropdown.options[linkElementDropdown.value].text;
        //string originalName = GetOriginalName(displayName);

        // To get original name from display name, you'll need to search the dictionary
        public static string GetOriginalName(string displayName)
        {
            // Look for the key (original name) that maps to this display name
            foreach (var pair in dropdownValueMapping)
            {
                if (pair.Value == displayName)
                    return pair.Key;
            }
            return null; // or return displayName if no mapping found
        }

        public static bool CheckIfDictContainsKey(string key)
        {
            return dropdownValueMapping.ContainsKey(key);
        }

        public static void ToggleUi()  // Not In Use, Needs Rework To Make It Usable
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
                SetUiFromObj();
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

        public static bool CheckComponentDictContainsKey(string key)
        {
            return componentRequirements.ContainsKey(key);
        }

        public static string GetComponentTypeFromDict(string key)
        {
            if (componentRequirements.TryGetValue(key, out string componentType))
            {
                return componentType;  // Will return null if no component required
            }
            return null;
        }

    }
}
