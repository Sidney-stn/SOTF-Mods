
using UnityEngine;

namespace BuildingMagnet
{
    internal class BuildingMagnetMono : MonoBehaviour
    {
        public bool IsEnabled
        {
            get
            {
                return Config.Enabled.Value;
            }
        }

        public string Mode
        {
            get
            {
                return MagnetValues.LastValue;
            }
        }

        private Dictionary<string, List<string>> _modeLinkedObject = new Dictionary<string, List<string>>  // Mode, <GameObjectName, LayerName>
        {
            {"NONE", new List<string> { null, null } },
            {"LOG", new List<string> { "LogPickup(Clone)", "Prop" } },
            {"3/4 LOG", new List<string> { "LogQuarterX3Pickup(Clone)", "Prop" } },
            {"1/2 LOG", new List<string> { "LogQuarterX2Pickup(Clone)", "Prop" } },
            {"1/4 LOG", new List<string> { "LogQuarterX1Pickup(Clone)", "Prop" } },
            {"STONE", new List<string> { "StonePickup(Clone)", "PickUp" } },
            {"PLANK", new List<string> { "LogPlankPickup(Clone)", "PickUp" } },
            {"3/4 PLANK", new List<string> { "LogPlankQuarterX3Pickup(Clone)", "PickUp" } },
            {"1/2 PLANK", new List<string> { "LogPlankQuarterX2Pickup(Clone)", "PickUp" } },
            {"1/4 PLANK", new List<string> { "LogPlankQuarterX1Pickup(Clone)", "PickUp" } },
            {"ALL", new List<string> { null, null } }
        };

        private HashSet<string> _scanForGoName = new HashSet<string>();

        /// <summary>
        /// Add a mode linked object to the dictionary, if the mode already exists it will be overwritten.
        /// Mode, <GameObjectName, LayerName>
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="gameObjectName"></param>
        /// <param name="layerName"></param>
        public void AddModeLinkedObject(string mode, string gameObjectName, string layerName)
        {
            if (_modeLinkedObject.ContainsKey(mode))
            {
                _modeLinkedObject[mode] = new List<string> { gameObjectName, layerName };
            }
            else
            {
                _modeLinkedObject.Add(mode, new List<string> { gameObjectName, layerName });
            }
        }

        private void Start()
        {
            MagnetValues.ValueChange += OnModeChange;
        }

        private void OnModeChange(object sender, EventArgs e)
        {
            _scanForGoName.Clear();
            if (_modeLinkedObject.ContainsKey(Mode))
            {
                _scanForGoName.Add(_modeLinkedObject[Mode][0]);
            }
        }

        private void OnDestroy()
        {
            MagnetValues.ValueChange -= OnModeChange;
        }
    }
}
