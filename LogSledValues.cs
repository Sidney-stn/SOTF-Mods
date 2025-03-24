

using RedLoader;
using UnityEngine;

namespace LogSledAutoPickup
{
    internal static class LogSledValues
    {
        private static HashSet<string> _cycleValues = new HashSet<string> { "NONE", "LOGS", "STONES" };

        internal static HashSet<string> GetCycleValues()
        {
            return _cycleValues;
        }
        internal static void AddCycleValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            _cycleValues.Add(value.ToUpper());
        }

        private static string _lastValue;
        internal static string LastValue
        {
            get
            {
                if (_lastValue == null)
                {
                    return "NONE";
                }
                return _lastValue;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                if (!_cycleValues.Contains(value.ToUpper()))
                {
                    LogSledAutoPickup.Msg($"[LogSledAutoPickup] [LogSledValues] [LastValue] [Set] Invalid value: {value.ToUpper()}");
                    return;
                }
                _lastValue = value.ToUpper();
                LogSledAutoPickupUi.panelText.Set(value.ToUpper());
            }
        }
    }

    internal static class LogSledTools
    {
        internal static GameObject activeLogSled = null;
        private static bool IsLogSledActive()
        {
            if (activeLogSled == null)
            {
                LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledTools] [IsLogSledActive] No active log sled found");
                return false;
            }
            else
            {
                var logSledController = activeLogSled.GetComponent<LogSledAutoPickupMono>();
                if (logSledController == null)
                {
                    LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledTools] [IsLogSledActive] No LogSledAutoPickupMono found on active log sled");
                    return false;
                }
                else if (logSledController.enabled == false)
                {
                    LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledTools] [IsLogSledActive] LogSledAutoPickupMono is disabled on active log sled");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        internal static void CycleValue()
        {
            if (IsLogSledActive() == false) { return; }
            var cycleValues = LogSledValues.GetCycleValues().ToList();
            var currentIndex = cycleValues.IndexOf(LogSledValues.LastValue);
            if (currentIndex == -1)
            {
                LogSledAutoPickup.Msg("[LogSledAutoPickup] [LogSledTools] [CycleValue] Invalid last value");
                return;
            }
            var nextIndex = currentIndex + 1;
            if (nextIndex >= cycleValues.Count)
            {
                nextIndex = 0;
            }
            LogSledValues.LastValue = cycleValues[nextIndex];
        }
    }
}
