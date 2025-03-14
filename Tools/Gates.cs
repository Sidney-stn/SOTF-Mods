using SonsSdk;
using StoneGate.Objects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StoneGate.Tools
{
    internal static class Gates
    {
        public static Scene? scene = null;
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
                Misc.Msg("ObjectName is null or empty");
                return null;
            }
            if (string.IsNullOrEmpty(sceneName))
            {
                Misc.Msg("SceneName is null or empty");
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
    }
}
