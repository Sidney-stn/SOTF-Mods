using Endnight.Utilities;
using Il2CppInterop.Runtime;
using RedLoader;
using SonsSdk;
using UnityEngine;

namespace StoneGate.Objects
{
    internal class CreateGateParent
    {
        // Singleton instance
        private static CreateGateParent _instance;

        // Public accessor for the singleton instance
        public static CreateGateParent Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CreateGateParent();
                }
                return _instance;
            }
        }

        public CreateGateParent()
        {
            // Create the parent object
            StoredParent = new GameObject("StoneGateObjects");
            StoredParent.transform.position = Vector3.zero;
            StoredParent.transform.rotation = Quaternion.identity;
            StoredParent.transform.localScale = Vector3.one;
            StoredParent.SetActive(true);
            StoredParent.DontDestroyOnLoad().HideAndDontSave();
        }

        /// <summary>
        /// The parent object that will store all the gate objects.
        /// </summary>
        public GameObject StoredParent { get; private set; }

        public enum RotateMode : byte
        {
            Vertical = 0,
            Horizontal = 1,
            None = 2
        }

        public static RotateMode RotateModeFromString(string modeString)
        {
            return (RotateMode)Enum.Parse(typeof(RotateMode), modeString, true);
        }

        /// <summary>
        /// Add a door to the gate parent.
        /// KeyPress-> [StoneGateItemMono] Complete() -> HERE -> [StoneGateStoreMono] Init().
        /// This function add StoneGateStoreMono To Parent GameObject (CodeName: StoredParent/GoName: StoneGateObjects).
        /// Then StoneGateStoreMono is Initialized to store all GameObjects that are used in the gate.
        /// </summary>
        /// <param name="rotate"></param>
        /// <param name="otherGameObjects"></param>
        public void AddDoor(GameObject rotate, params GameObject[] otherGameObjects)
        {
            if (rotate == null) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoor] Rotate is null"); return; }
            RotateMode mode = RotateMode.None;
            if (rotate.name.Contains("RockPilar")) { mode = RotateMode.Vertical; }
            else if (rotate.name.Contains("RockBeam")) { mode = RotateMode.Horizontal; }
            if (mode == RotateMode.None) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoor] Rotate is not a valid object"); return; }

            GameObject newGate = new GameObject("StoneGate", Il2CppType.Of<Mono.StoneGateStoreMono>());
            newGate.SetParent(StoredParent.transform);
            Mono.StoneGateStoreMono stoneGateMono = newGate.GetOrAddComponent<Mono.StoneGateStoreMono>();
            if (otherGameObjects.Length > 0)
            {
                Dictionary<string, GameObject> namesOfAllGo = new Dictionary<string, GameObject>();
                foreach (GameObject go in otherGameObjects)
                {
                    if (go == null) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoor] GameObject is null"); continue; }
                    namesOfAllGo.Add(go.name, go);
                }
                HashSet<GameObject> rockBeams = new HashSet<GameObject>(new GameObjectInstanceIDComparer());

                GameObject rockWall = null;
                GameObject extraPillar = null;

                foreach (var item in namesOfAllGo)
                {
                    if (item.Key.Contains("RockBeam")) { rockBeams.Add(item.Value); }
                    else if (item.Key.Contains("RockWall")) { rockWall = item.Value; }
                    else if (item.Key.Contains("RockPilar")) { extraPillar = item.Value; }
                }
                GameObject floorBeam = null;
                GameObject topBeam = null;
                if (rockBeams.Count <= 0)
                {
                    floorBeam = null;
                }
                else if (rockBeams.Count == 1)
                {
                    floorBeam = rockBeams.ToArray()[0];
                }
                else if (rockBeams.Count == 2)
                {
                    // Check witch have the highest Y value
                    float y1 = rockBeams.ToArray()[0].transform.position.y;
                    float y2 = rockBeams.ToArray()[1].transform.position.y;
                    if (y1 > y2)
                    {
                        floorBeam = rockBeams.ToArray()[0];
                        topBeam = rockBeams.ToArray()[1];
                    }
                    else
                    {
                        floorBeam = rockBeams.ToArray()[1];
                        topBeam = rockBeams.ToArray()[0];
                    }
                }

                stoneGateMono.Init(rotate, mode, floorBeam, topBeam, rockWall, extraPillar);
            }
        }

        /// <summary>
        /// Add a door to the gate parent. Used when loading save data.
        /// </summary>
        /// <param name="rotate"></param>
        /// <param name="mode"></param>
        /// <param name="floorBeam"></param>
        /// <param name="topBeam"></param>
        /// <param name="rockWall"></param>
        /// <param name="extraPillar"></param>
        public void AddDoor(GameObject rotate, RotateMode mode, GameObject floorBeam = null, GameObject topBeam = null, GameObject rockWall = null, GameObject extraPillar = null)
        {
            if (rotate == null) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoor] Rotate is null"); return; }
            if (mode == RotateMode.None) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoor] Rotate Mode is not valid"); return; }

            GameObject newGate = new GameObject("StoneGate", Il2CppType.Of<Mono.StoneGateStoreMono>());
            newGate.SetParent(StoredParent.transform);
            Mono.StoneGateStoreMono stoneGateMono = newGate.GetOrAddComponent<Mono.StoneGateStoreMono>();

            stoneGateMono.Init(rotate, mode, floorBeam, topBeam, rockWall, extraPillar);
        }


        /// <summary>
        /// For Multiplayer only, client side.
        /// </summary>
        /// <param name="rotate"></param>
        /// <param name="otherGameObjects"></param>
        public void AddDoorNetworkClient(GameObject rotate, params GameObject[] otherGameObjects)
        {
            if (BoltNetwork.isRunning == false || BoltNetwork.isClient == false) { return; }
            if (rotate == null) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoor] Rotate is null"); return; }
            RotateMode mode = RotateMode.None;
            if (rotate.name.Contains("RockPilar")) { mode = RotateMode.Vertical; }
            else if (rotate.name.Contains("RockBeam")) { mode = RotateMode.Horizontal; }
            if (mode == RotateMode.None) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoor] Rotate is not a valid object"); return; }

            if (otherGameObjects.Length > 0)
            {
                Dictionary<string, GameObject> namesOfAllGo = new Dictionary<string, GameObject>();
                foreach (GameObject go in otherGameObjects)
                {
                    if (go == null) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoor] GameObject is null"); continue; }
                    namesOfAllGo.Add(go.name, go);
                }
                HashSet<GameObject> rockBeams = new HashSet<GameObject>(new GameObjectInstanceIDComparer());

                GameObject rockWall = null;
                GameObject extraPillar = null;

                foreach (var item in namesOfAllGo)
                {
                    if (item.Key.Contains("RockBeam")) { rockBeams.Add(item.Value); }
                    else if (item.Key.Contains("RockWall")) { rockWall = item.Value; }
                    else if (item.Key.Contains("RockPilar")) { extraPillar = item.Value; }
                }
                GameObject floorBeam = null;
                GameObject topBeam = null;
                if (rockBeams.Count <= 0)
                {
                    floorBeam = null;
                }
                else if (rockBeams.Count == 1)
                {
                    floorBeam = rockBeams.ToArray()[0];
                }
                else if (rockBeams.Count == 2)
                {
                    // Check witch have the highest Y value
                    float y1 = rockBeams.ToArray()[0].transform.position.y;
                    float y2 = rockBeams.ToArray()[1].transform.position.y;
                    if (y1 > y2)
                    {
                        floorBeam = rockBeams.ToArray()[0];
                        topBeam = rockBeams.ToArray()[1];
                    }
                    else
                    {
                        floorBeam = rockBeams.ToArray()[1];
                        topBeam = rockBeams.ToArray()[0];
                    }
                }

                string rotateGoName = rotate.name;
                string floorBeamGoName = floorBeam?.name;
                string topBeamGoName = topBeam?.name;
                string rockWallGoName = rockWall?.name;
                string extraPillarGoName = extraPillar?.name;

                // If Any of the GameObjects are null, set them to "NONE"
                if (string.IsNullOrEmpty(floorBeamGoName)) { floorBeamGoName = "NONE"; }
                if (string.IsNullOrEmpty(topBeamGoName)) { topBeamGoName = "NONE"; }
                if (string.IsNullOrEmpty(rockWallGoName)) { rockWallGoName = "NONE"; }
                if (string.IsNullOrEmpty(extraPillarGoName)) { extraPillarGoName = "NONE"; }

                // Raise Event
                Network.ClientEvents.Instance.SendClientEvent(
                    eventType: Network.ClientEvents.ClientEvent.CreateStoneGate,
                    rotateGoName: rotateGoName,
                    mode: mode,
                    floorBeamGoName: floorBeamGoName,
                    topBeamGoName: topBeamGoName,
                    rockWallGoName: rockWallGoName,
                    extraPillarGoName: extraPillarGoName
                );
            }
        }

        public void AddDoorNetworkClient(string rotateGoName, Objects.CreateGateParent.RotateMode mode, string floorBeamGoName, string topBeamGoName, string rockWallGoName, string extraPillarGoName, int childIndex)
        {
            if (BoltNetwork.isRunning == false || BoltNetwork.isClient == false) { return; }
            if (string.IsNullOrEmpty(rotateGoName) || rotateGoName == "NONE") { RLog.Error("[StoneGate] [CreateGateParent] [AddDoorNetworkClient] Rotate String is null"); return; }
            if (mode == RotateMode.None) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoorNetworkClient] Rotate Mode is not valid"); return; }

            // Check, at least 1 of the other string must be valid
            if ((string.IsNullOrEmpty(floorBeamGoName) || floorBeamGoName == "NONE") &&
                (string.IsNullOrEmpty(topBeamGoName) || topBeamGoName == "NONE") &&
                (string.IsNullOrEmpty(rockWallGoName) || rockWallGoName == "NONE") &&
                (string.IsNullOrEmpty(extraPillarGoName) || extraPillarGoName == "NONE")
            )
            {
                RLog.Error("[StoneGate] [CreateGateParent] [AddDoorNetworkClient] At least 1 of the other string must be valid");
                return;
            }

            // Find GameObjects From Name
            GameObject rotateGo = Tools.Gates.FindObjectInSpecificScene(rotateGoName);
            if (rotateGo == null) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoorNetworkClient] Rotate GameObject is null"); return; }
            GameObject floorBeamGo = null;
            GameObject topBeamGo = null;
            GameObject rockWallGo = null;
            GameObject extraPillarGo = null;

            if (string.IsNullOrEmpty(floorBeamGoName) || floorBeamGoName != "NONE")
            {
                floorBeamGo = Tools.Gates.FindObjectInSpecificScene(floorBeamGoName);
            }
            if (string.IsNullOrEmpty(topBeamGoName) || topBeamGoName != "NONE")
            {
                topBeamGo = Tools.Gates.FindObjectInSpecificScene(topBeamGoName);
            }
            if (string.IsNullOrEmpty(rockWallGoName) || rockWallGoName != "NONE")
            {
                rockWallGo = Tools.Gates.FindObjectInSpecificScene(rockWallGoName);
            }
            if (string.IsNullOrEmpty(extraPillarGoName) || extraPillarGoName != "NONE")
            {
                extraPillarGo = Tools.Gates.FindObjectInSpecificScene(extraPillarGoName);
            }

            GameObject newGate = new GameObject("StoneGate", Il2CppType.Of<Mono.StoneGateStoreMono>());
            newGate.SetParent(StoredParent.transform).transform.SetSiblingIndex(childIndex);

            Mono.StoneGateStoreMono stoneGateMono = newGate.GetOrAddComponent<Mono.StoneGateStoreMono>();

            if (stoneGateMono != null)
            {
                stoneGateMono.Init(rotateGo, mode, floorBeamGo, topBeamGo, rockWallGo, extraPillarGo);
            }
        }

        public void AddDoorNetworkHost(GameObject rotate, params GameObject[] otherGameObjects)
        {
            if (BoltNetwork.isRunning == false || BoltNetwork.isServer == false) { return; }

            if (rotate == null) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoor] Rotate is null"); return; }
            RotateMode mode = RotateMode.None;
            if (rotate.name.Contains("RockPilar")) { mode = RotateMode.Vertical; }
            else if (rotate.name.Contains("RockBeam")) { mode = RotateMode.Horizontal; }
            if (mode == RotateMode.None) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoor] Rotate is not a valid object"); return; }

            Mono.StoneGateStoreMono stoneGateMono = null;

            int? childIndex = null;

            GameObject newGate = new GameObject("StoneGate", Il2CppType.Of<Mono.StoneGateStoreMono>());
            newGate.SetParent(StoredParent.transform);
            childIndex = newGate.transform.GetSiblingIndex();
            stoneGateMono = newGate.GetOrAddComponent<Mono.StoneGateStoreMono>();

            if (childIndex == null) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoorNetworkHost] ChildIndex is null"); return; }

            if (otherGameObjects.Length > 0)
            {
                Dictionary<string, GameObject> namesOfAllGo = new Dictionary<string, GameObject>();
                foreach (GameObject go in otherGameObjects)
                {
                    if (go == null) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoor] GameObject is null"); continue; }
                    namesOfAllGo.Add(go.name, go);
                }
                HashSet<GameObject> rockBeams = new HashSet<GameObject>(new GameObjectInstanceIDComparer());

                GameObject rockWall = null;
                GameObject extraPillar = null;

                foreach (var item in namesOfAllGo)
                {
                    if (item.Key.Contains("RockBeam")) { rockBeams.Add(item.Value); }
                    else if (item.Key.Contains("RockWall")) { rockWall = item.Value; }
                    else if (item.Key.Contains("RockPilar")) { extraPillar = item.Value; }
                }
                GameObject floorBeam = null;
                GameObject topBeam = null;
                if (rockBeams.Count <= 0)
                {
                    floorBeam = null;
                }
                else if (rockBeams.Count == 1)
                {
                    floorBeam = rockBeams.ToArray()[0];
                }
                else if (rockBeams.Count == 2)
                {
                    // Check witch have the highest Y value
                    float y1 = rockBeams.ToArray()[0].transform.position.y;
                    float y2 = rockBeams.ToArray()[1].transform.position.y;
                    if (y1 > y2)
                    {
                        floorBeam = rockBeams.ToArray()[0];
                        topBeam = rockBeams.ToArray()[1];
                    }
                    else
                    {
                        floorBeam = rockBeams.ToArray()[1];
                        topBeam = rockBeams.ToArray()[0];
                    }
                }

                if (stoneGateMono != null)
                {
                    stoneGateMono.Init(rotate, mode, floorBeam, topBeam, rockWall, extraPillar);

                    // Raise Event To Clients
                    Network.HostEvents.Instance.SendHostEvent(Network.HostEvents.HostEvent.CreateStoneGate, rotate.name, mode, floorBeam?.name, topBeam?.name, rockWall?.name, extraPillar?.name, childIndex: (int)childIndex);
                }

            }
        }

        /// <summary>
        /// Add a door, used when client event is received.
        /// </summary>
        /// <param name="rotateGoName"></param>
        /// <param name="mode"></param>
        /// <param name="floorBeamGoName"></param>
        /// <param name="topBeamGoName"></param>
        /// <param name="rockWallGoName"></param>
        /// <param name="extraPillarGoName"></param>
        public void AddDoorNetworkHost(string rotateGoName, Objects.CreateGateParent.RotateMode mode, string floorBeamGoName, string topBeamGoName, string rockWallGoName, string extraPillarGoName)
        {
            if (BoltNetwork.isRunning == false || BoltNetwork.isServer == false) { return; }
            if (string.IsNullOrEmpty(rotateGoName) || rotateGoName == "NONE") { RLog.Error("[StoneGate] [CreateGateParent] [AddDoorNetworkHost] Rotate String is null"); return; }
            if (mode == RotateMode.None) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoorNetworkHost] Rotate Mode is not valid"); return; }

            // Check, at least 1 of the other string must be valid
            if ((string.IsNullOrEmpty(floorBeamGoName) || floorBeamGoName == "NONE") &&
                (string.IsNullOrEmpty(topBeamGoName) || topBeamGoName == "NONE") &&
                (string.IsNullOrEmpty(rockWallGoName) || rockWallGoName == "NONE") &&
                (string.IsNullOrEmpty(extraPillarGoName) || extraPillarGoName == "NONE")
            )
            {
                RLog.Error("[StoneGate] [CreateGateParent] [AddDoorNetworkHost] At least 1 of the other string must be valid");
                return;
            }

            // Find GameObjects From Name
            GameObject rotateGo = Tools.Gates.FindObjectInSpecificScene(rotateGoName);
            if (rotateGo == null) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoorNetworkHost] Rotate GameObject is null"); return; }
            GameObject floorBeamGo = null;
            GameObject topBeamGo = null;
            GameObject rockWallGo = null;
            GameObject extraPillarGo = null;

            if (string.IsNullOrEmpty(floorBeamGoName) || floorBeamGoName != "NONE")
            {
                floorBeamGo = Tools.Gates.FindObjectInSpecificScene(floorBeamGoName);
            }
            if (string.IsNullOrEmpty(topBeamGoName) || topBeamGoName != "NONE")
            {
                topBeamGo = Tools.Gates.FindObjectInSpecificScene(topBeamGoName);
            }
            if (string.IsNullOrEmpty(rockWallGoName) || rockWallGoName != "NONE")
            {
                rockWallGo = Tools.Gates.FindObjectInSpecificScene(rockWallGoName);
            }
            if (string.IsNullOrEmpty(extraPillarGoName) || extraPillarGoName != "NONE")
            {
                extraPillarGo = Tools.Gates.FindObjectInSpecificScene(extraPillarGoName);
            }

            GameObject newGate = new GameObject("StoneGate", Il2CppType.Of<Mono.StoneGateStoreMono>());
            newGate.SetParent(StoredParent.transform);
            int? childIndex = null;
            childIndex = newGate.transform.GetSiblingIndex();
            if (childIndex == null) { RLog.Error("[StoneGate] [CreateGateParent] [AddDoorNetworkHost] ChildIndex is null"); return; }

            Mono.StoneGateStoreMono stoneGateMono = newGate.GetOrAddComponent<Mono.StoneGateStoreMono>();

            if (stoneGateMono != null)
            {
                stoneGateMono.Init(rotateGo, mode, floorBeamGo, topBeamGo, rockWallGo, extraPillarGo);

                // Raise Event To Clients
                Network.HostEvents.Instance.SendHostEvent(Network.HostEvents.HostEvent.CreateStoneGate, rotateGoName, mode, floorBeamGoName, topBeamGoName, rockWallGoName, extraPillarGoName, childIndex: (int)childIndex);
            }
        }

        /// <summary>
        /// Destroy a door, used when client event is received.
        /// </summary>
        /// <param name="childIndex"></param>
        public void RemoveDoorNetworkHost(int childIndex)
        {
            if (BoltNetwork.isRunning == false || BoltNetwork.isServer == false) { return; }
            if (childIndex < 0) { RLog.Error("[StoneGate] [CreateGateParent] [DestoryDorrNetworkHost] ChildIndex is invalid"); return; }
            GameObject gate = StoredParent.transform.GetChild(childIndex).gameObject;
            if (gate == null) { RLog.Error("[StoneGate] [CreateGateParent] [DestoryDorrNetworkHost] Gate is null"); return; }
            Mono.StoneGateStoreMono stoneGateMono = gate.GetComponent<Mono.StoneGateStoreMono>();
            if (stoneGateMono == null) { RLog.Error("[StoneGate] [CreateGateParent] [DestoryDorrNetworkHost] StoneGateMono is null"); return; }
            stoneGateMono.DestroyGate(raiseNetwork: true);  // Destroy the gate and raise network event
        }

        /// <summary>
        /// Destroy a door, used when host event is received on client.
        /// </summary>
        /// <param name="childIndex"></param>
        public void RemoveDoorNetworkClient(int childIndex)
        {
            if (BoltNetwork.isRunning == false || BoltNetwork.isClient == false) { return; }
            if (childIndex < 0) { RLog.Error("[StoneGate] [CreateGateParent] [DestoryDorrNetworkClient] ChildIndex is invalid"); return; }
            GameObject gate = StoredParent.transform.GetChild(childIndex).gameObject;
            if (gate == null) { RLog.Error("[StoneGate] [CreateGateParent] [DestoryDorrNetworkClient] Gate is null"); return; }
            Mono.StoneGateStoreMono stoneGateMono = gate.GetComponent<Mono.StoneGateStoreMono>();
            if (stoneGateMono == null) { RLog.Error("[StoneGate] [CreateGateParent] [DestoryDorrNetworkClient] StoneGateMono is null"); return; }
            stoneGateMono.DestroyGate(raiseNetwork: false);  // Destroy the gate and raise network event
        }

        /// <summary>
        /// Open a door, used when client event is received.
        /// </summary>
        /// <param name="childIndex"></param>
        public void OpenDoorNetworkHost(int childIndex)
        {
            if (BoltNetwork.isRunning == false || BoltNetwork.isServer == false) { return; }
            if (childIndex < 0) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkHost] ChildIndex is invalid"); return; }
            GameObject gate = StoredParent.transform.GetChild(childIndex).gameObject;
            if (gate == null) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkHost] Gate is null"); return; }
            Mono.StoneGateStoreMono stoneGateMono = gate.GetComponent<Mono.StoneGateStoreMono>();
            if (stoneGateMono == null) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkHost] StoneGateMono is null"); return; }
            stoneGateMono.OpenGate(raiseNetwork: true);  // Open the gate and raise network event
        }

        /// <summary>
        /// Open a door, used when host event is received on client.
        /// </summary>
        /// <param name="childIndex"></param>
        public void OpenDoorNetworkClient(int childIndex)
        {
            if (BoltNetwork.isRunning == false || BoltNetwork.isClient == false) { return; }
            if (childIndex < 0) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkClient] ChildIndex is invalid"); return; }
            GameObject gate = StoredParent.transform.GetChild(childIndex).gameObject;
            if (gate == null) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkClient] Gate is null"); return; }
            Mono.StoneGateStoreMono stoneGateMono = gate.GetComponent<Mono.StoneGateStoreMono>();
            if (stoneGateMono == null) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkClient] StoneGateMono is null"); return; }
            stoneGateMono.OpenGate(raiseNetwork: false);  // Open the gate and raise network event
        }

        /// <summary>
        /// Close a door, used when client event is received.
        /// </summary>
        /// <param name="childIndex"></param>
        public void CloseDoorNetworkHost(int childIndex)
        {
            if (BoltNetwork.isRunning == false || BoltNetwork.isServer == false) { return; }
            if (childIndex < 0) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkHost] ChildIndex is invalid"); return; }
            GameObject gate = StoredParent.transform.GetChild(childIndex).gameObject;
            if (gate == null) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkHost] Gate is null"); return; }
            Mono.StoneGateStoreMono stoneGateMono = gate.GetComponent<Mono.StoneGateStoreMono>();
            if (stoneGateMono == null) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkHost] StoneGateMono is null"); return; }
            stoneGateMono.CloseGate(raiseNetwork: true);  // Open the gate and raise network event
        }


        /// <summary>
        /// Close a door, used when host event is received on client.
        /// </summary>
        /// <param name="childIndex"></param>
        public void CloseDoorNetworkClient(int childIndex)
        {
            if (BoltNetwork.isRunning == false || BoltNetwork.isClient == false) { return; }
            if (childIndex < 0) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkClient] ChildIndex is invalid"); return; }
            GameObject gate = StoredParent.transform.GetChild(childIndex).gameObject;
            if (gate == null) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkClient] Gate is null"); return; }
            Mono.StoneGateStoreMono stoneGateMono = gate.GetComponent<Mono.StoneGateStoreMono>();
            if (stoneGateMono == null) { RLog.Error("[StoneGate] [CreateGateParent] [OpenDoorNetworkClient] StoneGateMono is null"); return; }
            stoneGateMono.CloseGate(raiseNetwork: false);  // Open the gate and raise network event
        }
    }
}
