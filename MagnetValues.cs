

using RedLoader;

namespace BuildingMagnet
{
    public class MagnetValues
    {
        public static event EventHandler ValueChange;

        private static HashSet<string> _cycleValues = new HashSet<string>
        {
            "NONE",
            "LOG",
            "3/4 LOG",
            "1/2 LOG",
            "1/4 LOG",
            "STONE",
        };

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
                    RLog.Msg($"[BuildingMagnet] [MagnetValues] [LastValue] [Set] Invalid value: {value.ToUpper()}");
                    return;
                }
                _lastValue = value.ToUpper();
                BuildingMagnetUi.panelText.Set(value.ToUpper());
            }
        }

        public static void AddCycleValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            _cycleValues.Add(value.ToUpper());
        }

        public static HashSet<string> GetCycleValues()
        {
            return _cycleValues;
        }

        public static void CycleValue()
        {
            var cycleValues = GetCycleValues().ToList();
            var currentIndex = cycleValues.IndexOf(LastValue);
            if (currentIndex == -1)
            {
                RLog.Msg("[BuildingMagnet] [MagnetValues] [CycleValue] Invalid last value");
                return;
            }
            var nextIndex = currentIndex + 1;
            if (nextIndex >= cycleValues.Count)
            {
                nextIndex = 0;
            }
            LastValue = cycleValues[nextIndex];

            if (LastValue == "NONE")
            {
                BuildingMagnetUi.CloseMainPanel();
            }
            else
            {
                BuildingMagnetUi.OpenMainPanel();
            }
            ValueChange?.Invoke(typeof(MagnetValues), EventArgs.Empty);
        }
    }
}
