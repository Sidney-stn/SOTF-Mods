
using Il2CppInterop.Runtime;
using UnityEngine;

namespace SimpleElevator.Structure
{
    internal class ElevatorControlPanel : StructureBase
    {
        // Singleton instance
        private static ElevatorControlPanel _instance;

        // Private constructor to prevent direct instantiation
        private ElevatorControlPanel()
        {
            // Initialization code if needed
            StructureId = 751155;
            BlueprintName = "ElevatorControlPanel";
            RegisterInBook = true;
            BookPage = Assets.Instance.ElevatorControlPanelBookPage; // Assets.Instance.ConveyorBeltBookPage;
            AddComponents = new List<Il2CppSystem.Type> { Il2CppType.Of<Mono.ElevatorControlPanelMono>() };
            BoltSetterComponent = Il2CppType.Of<Network.ElevatorControlPanelSetter>();
            RegisterStructure = true;
            AddGrassAndSnow = true;
            GrassSize = new Vector3(0.4f, 0.2f, 1.3f);
            SnowSize = new Vector3(0.7f, 1, 0.2f);
            MaxPlacementAngle = null;
            SetupStructure(Assets.Instance.ElevatorControlPanel);
        }

        // Public accessor for the singleton instance
        public static ElevatorControlPanel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ElevatorControlPanel();
                }
                return _instance;
            }
        }
    }
}
