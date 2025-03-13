

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
        }

        public GameObject StoredParent { get; private set; }

        public enum RotateMode
        {
            Vertical,
            Horizontal,
            None
        }

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
                } else if (rockBeams.Count == 1)
                {
                    floorBeam = rockBeams.ToArray()[0];
                } else if (rockBeams.Count == 2)
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
    }
}
