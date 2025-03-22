using RedLoader;
using SonsSdk;
using StoneGate.Objects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StoneGate.Tools
{
    internal static class Gates
    {
        public static Scene? scene = null;

        /// <summary>
        /// For keeping track of all under gameobjects used in stonegate.
        /// Example RockPilar1, if its in the list no other gate can use this object if found in here
        /// </summary>
        public static HashSet<GameObject> ocupiedObjects = new HashSet<GameObject>(new Objects.GameObjectInstanceIDComparer());

        /// <summary>
        /// Check if the GameObject is ocupied, all ocupied GameObjects are stored in a HashSet
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool IsObjectOcupied(GameObject go)
        {
            if (ocupiedObjects.Contains(go))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the StoneGate GameObject. Gotten from GameObject that is linked to StoneGate. Example: RockPilar1 -> StoneGate
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static GameObject GetLinkedStoneGate(GameObject go)
        {
            HashSet<Mono.StoneGateStoreMono> allControllers = GetAllStoneGateStoreMono();
            foreach (Mono.StoneGateStoreMono controller in allControllers)
            {
                if (controller == null) { if (Testing.Settings.logToolsGates) { Misc.Msg($"[Tools] [Gates] [GetLinkedStoneGate] Controller IS NULL"); } continue; }

                Dictionary<string, GameObject> allAddedGameObjects = controller.GetNamesAndGameObjects();

                foreach (KeyValuePair<string, GameObject> kvp in allAddedGameObjects)
                {
                    if (kvp.Value == go)
                    {
                        if (Testing.Settings.logToolsGates) { Misc.Msg($"[Tools] [Gates] [GetLinkedStoneGate] Found StoneGate Linked to GameObject: {go.name}"); }
                        return controller.gameObject;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get the StoneGate GameObject. Gotten from of name of looking GameObject. Example: RockPilar1 -> StoneGate
        /// </summary>
        /// <param name="goName"></param>
        /// <returns></returns>
        public static GameObject GetLinkedStoneGate(string goName)
        {
            HashSet<Mono.StoneGateStoreMono> allControllers = GetAllStoneGateStoreMono();
            foreach (Mono.StoneGateStoreMono controller in allControllers)
            {
                if (controller == null) { if (Testing.Settings.logToolsGates) { Misc.Msg($"[Tools] [Gates] [GetLinkedStoneGate] Controller IS NULL"); } continue; }

                Dictionary<string, GameObject> allAddedGameObjects = controller.GetNamesAndGameObjects();

                foreach (KeyValuePair<string, GameObject> kvp in allAddedGameObjects)
                {
                    if (kvp.Value.name == goName)
                    {
                        if (Testing.Settings.logToolsGates) { Misc.Msg($"[Tools] [Gates] [GetLinkedStoneGate] Found StoneGate Linked to GameObject: {goName}"); }
                        return controller.gameObject;
                    }
                }
            }
            return null;
        }

        public static Mono.StoneGateStoreMono GetLinkedStoneGateController(GameObject go)
        {
            GameObject linkedGo = GetLinkedStoneGate(go);
            return linkedGo.GetComponent<Mono.StoneGateStoreMono>();
        }

        public static Mono.StoneGateStoreMono GetLinkedStoneGateController(string goName)
        {
            GameObject linkedGo = GetLinkedStoneGate(goName);
            return linkedGo.GetComponent<Mono.StoneGateStoreMono>();
        }


        /// <summary>
        /// Check if the GameObject has a StoneGate linked to it
        /// </summary>
        /// <param name="rootGo"></param>
        /// <returns>true or false</returns>
        public static bool DoesGoHaveStoneGateLinked(GameObject rootGo)
        {
            string rootName = rootGo.name;
            if (Config.allowedHits.Any(prefix => rootName.StartsWith(prefix)))
            {
                HashSet<Mono.StoneGateStoreMono> allControllers = GetAllStoneGateStoreMono();
                foreach (Mono.StoneGateStoreMono controller in allControllers)
                {
                    if (controller == null) { if (Testing.Settings.logToolsGates) { Misc.Msg($"[Tools] [Gates] [DoesGoHaveStoneGateLinked] Controller IS NULL"); } continue; }
                    {
                        Dictionary<string, GameObject> allAddedGameObjects = controller.GetNamesAndGameObjects();
                        foreach (KeyValuePair<string, GameObject> kvp in allAddedGameObjects)
                        {
                            if (kvp.Value == rootGo)
                            {
                                if (Testing.Settings.logToolsGates) { Misc.Msg($"[Tools] [Gates] [DoesGoHaveStoneGateLinked] Found StoneGate Linked to GameObject: {rootGo.name}"); }
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static HashSet<Mono.StoneGateStoreMono> GetAllStoneGateStoreMono()
        {
            HashSet<Mono.StoneGateStoreMono> allStoneGateStoreMono = new HashSet<Mono.StoneGateStoreMono>();

            GameObject parentOfAllStoneGates = CreateGateParent.Instance.StoredParent;
            if (parentOfAllStoneGates == null) { if (Testing.Settings.logToolsGates) { Misc.Msg($"[Tools] [Gates] [GetAllStoneGateStoreMono] StoredParent IS NULL"); } return allStoneGateStoreMono; }

            List<Transform> tempChildTransforms = parentOfAllStoneGates.GetChildren();

            if (Testing.Settings.logToolsGates) { Misc.Msg($"[Tools] [Gates] [GetAllStoneGateStoreMono] TempChildTransforms.Count: {tempChildTransforms.Count}"); }

            foreach (Transform t in tempChildTransforms)
            {
                var comp = t.GetComponent<Mono.StoneGateStoreMono>();
                if (comp != null)
                {
                    allStoneGateStoreMono.Add(comp);
                }
                else
                {
                    if (Testing.Settings.logToolsGates) { Misc.Msg($"[Tools] [Gates] [GetAllStoneGateStoreMono] StoneGateStoreMono comp IS NULL"); }
                }
            }
            if (Testing.Settings.logToolsGates) { Misc.Msg($"[Tools] [Gates] [GetAllStoneGateStoreMono] AllStoneGateStoreMono.Count: {allStoneGateStoreMono.Count}"); }
            return allStoneGateStoreMono;
        }

        public static GameObject FindObjectInSpecificScene(string objectName, string sceneName = "BlankScene")
        {
            if (string.IsNullOrEmpty(objectName))
            {
                Misc.Msg("[Tools] [Tools] [FindObjectInSpecificScene] ObjectName is null or empty");
                return null;
            }
            if (string.IsNullOrEmpty(sceneName))
            {
                Misc.Msg("[Tools] [Tools] [FindObjectInSpecificScene] SceneName is null or empty");
                return null;
            }
            if (scene == null)
            {
                scene = SceneManager.GetSceneByName(sceneName);
            }
            Scene notNull = scene ?? SceneManager.GetSceneByName(sceneName);

            // Check if the scene is valid and loaded
            if (notNull.IsValid() && notNull.isLoaded)
            {
                // Get all root GameObjects in the scene
                GameObject[] rootGameObjects = notNull.GetRootGameObjects();

                // Iterate through the root GameObjects to find the one with the specified name
                foreach (GameObject go in rootGameObjects)
                {
                    if (go.name == objectName)
                    {
                        return go;
                    }
                }
            }
            else
            {
                Misc.Msg("Scene is not valid or not loaded: " + sceneName);
            }

            // Return null if the GameObject was not found
            return null;
        }

        public static void LoadAllSaveData(Saving.Manager.GatesManager obj)
        {
            if (BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                if (Testing.Settings.logSavingSystem)
                    Misc.Msg("[Loading] Skipped Loading StoneGates On Multiplayer Client");
                return;
            }
            if (Testing.Settings.logSavingSystem)
                Misc.Msg($"[Loading] Gates From Save: {obj.Gates.Count.ToString()}");
            foreach (var gatesData in obj.Gates)
            {
                if (Testing.Settings.logSavingSystem)
                    Misc.Msg("[Loading] Creating New Gates");
                LoadIndivudalSaveData(gatesData);
            }
        }

        public static void LoadIndivudalSaveData(Saving.Manager.GatesManager.GatesModData obj)
        {
            if (BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                if (Testing.Settings.logSavingSystem)
                    Misc.Msg("[Loading] Skipped Loading StoneGates On Multiplayer Client");
                return;
            }
            if (Testing.Settings.logSavingSystem) { Misc.Msg($"[Loading] Gates From Save: {obj.ToString()}"); }

            // Check if if gameobject is null or empty
            bool floorBeamIsNull = string.IsNullOrEmpty(obj.FloorBeamName);
            bool topBeamIsNull = string.IsNullOrEmpty(obj.TopBeamName);
            bool rockWallIsNull = string.IsNullOrEmpty(obj.RockWallName);
            bool extraPillarIsNull = string.IsNullOrEmpty(obj.ExtraPillarName);
            bool rotateIsNull = string.IsNullOrEmpty(obj.RotationGoName);

            // Mode
            Objects.CreateGateParent.RotateMode mode = Objects.CreateGateParent.RotateModeFromString(obj.Mode);

            GameObject floorBeam = null;
            GameObject topBeam = null;
            GameObject rockWall = null;
            GameObject extraPillar = null;
            GameObject rotate = null;

            if (floorBeamIsNull == false)
            {
                floorBeam = FindObjectInSpecificScene(obj.FloorBeamName);
                if (Testing.Settings.superLogSavingSystem)
                {
                    if (floorBeam == null)
                    {
                        Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] FloorBeam is null, can't reconstruct gate");
                    }
                    else
                    {
                        Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] FloorBeam is not null, can reconstruct gate");
                    }
                }
                if (topBeamIsNull == false)
                {
                    topBeam = FindObjectInSpecificScene(obj.TopBeamName);
                    if (Testing.Settings.superLogSavingSystem)
                    {
                        if (topBeam == null)
                        {
                            Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] TopBeam is null, can't reconstruct gate");
                        }
                        else
                        {
                            Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] TopBeam is not null, can reconstruct gate");
                        }
                    }
                }
                if (rockWallIsNull == false)
                {
                    rockWall = FindObjectInSpecificScene(obj.RockWallName);
                    if (Testing.Settings.superLogSavingSystem)
                    {
                        if (rockWall == null)
                        {
                            Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] RockWall is null, can't reconstruct gate");
                        }
                        else
                        {
                            Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] RockWall is not null, can reconstruct gate");
                        }
                    }
                }
                if (extraPillarIsNull == false)
                {
                    extraPillar = FindObjectInSpecificScene(obj.ExtraPillarName);
                    if (Testing.Settings.superLogSavingSystem)
                    {
                        if (extraPillar == null)
                        {
                            Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] ExtraPillar is null, can't reconstruct gate");
                        }
                        else
                        {
                            Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] ExtraPillar is not null, can reconstruct gate");
                        }
                    }
                }
                if (rotateIsNull == false)
                {
                    rotate = FindObjectInSpecificScene(obj.RotationGoName);
                    if (Testing.Settings.superLogSavingSystem)
                    {
                        if (rotate == null)
                        {
                            Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] Main Rotate is null, can't reconstruct gate");
                        }
                        else
                        {
                            Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] Main Rotate is not null, can reconstruct gate");
                        }
                    }
                }

                if (rotate == null) { Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] Main Rotate is null, can't reconstruct gate"); return; }

                int notNullCount = 0;
                if (floorBeam != null) { notNullCount++; }
                if (topBeam != null) { notNullCount++; }
                if (rockWall != null) { notNullCount++; }
                if (extraPillar != null) { notNullCount++; }


                if (notNullCount <= 0)
                {
                    Misc.Msg("[Tools] [Gates] [LoadIndivudalSaveData] No GameObjects found, can't reconstruct gate");
                    return;
                }

                // Use AddDoor to create the gate
                try
                {
                    CreateGateParent.Instance.AddDoor(rotate, mode, floorBeam, topBeam, rockWall, extraPillar);
                } catch (Exception e)
                {
                    RLog.Error($"[Tools] [Gates] [LoadIndivudalSaveData] Failed To Add Door To GateParent. Error: ${e}");
                }
                
            }
        }
    }
}
