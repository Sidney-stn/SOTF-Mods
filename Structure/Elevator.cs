using Il2CppInterop.Runtime;
using UnityEngine;


namespace SimpleElevator.Structure 
{
    internal class Elevator : StructureBase
    {
        // Singleton instance
        private static Elevator _instance;

        // Private constructor to prevent direct instantiation
        private Elevator()
        {
            // Initialization code if needed
            StructureId = 7511110;
            BlueprintName = "Elevator";
            RegisterInBook = true;
            BookPage = Assets.Instance.ElevatorBookPage;
            AddComponents = new List<Il2CppSystem.Type> { Il2CppType.Of<Mono.ElevatorMono>() };
            BoltSetterComponent = Il2CppType.Of<Network.ElevatorSetter>();
            RegisterStructure = true;
            AddGrassAndSnow = true;
            GrassSize = new Vector3(2f, 1f, 2.5f);
            GrassLocalPos = new Vector3(2f, 1f, 2.5f);
            SnowSize = new Vector3(2f, 3f, 1f);
            SnowLocalPos = new Vector3(0f, 0f, -1f);
            MaxPlacementAngle = null;
            SetupStructure(Assets.Instance.Elevator);
        }

        // Public accessor for the singleton instance
        public static Elevator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Elevator();
                }
                return _instance;
            }
        }
    }
}
