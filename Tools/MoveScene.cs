

using UnityEngine.SceneManagement;
using UnityEngine;
using SonsSdk;

namespace StoneGate.Tools
{
    internal static class MoveScene
    {
        public const string targetSceneName = "DontDestroyOnLoad";

        public static bool MoveToScene(GameObject objectToMove)
        {
            if (objectToMove == null)
            {
                Misc.Msg($"[MoveScene] [MoveToScene] Cannot move null object to scene", true);
                return false;
            }

            // Skip if already in correct scene
            if (objectToMove.scene.name == targetSceneName)
            {
                if (Testing.Settings.logScene)
                {
                    Misc.Msg($"[MoveScene] [MoveToScene] {objectToMove.name} is already in Target Scene", true);
                }
                return true;
            }

            // Get the target scene
            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            if (targetScene.IsValid())
            {
                try
                {
                    SceneManager.MoveGameObjectToScene(objectToMove, targetScene);
                    objectToMove.DontDestroyOnLoad().HideAndDontSave();

                    if (Testing.Settings.logScene)
                    {
                        Misc.Msg($"[MoveScene] [MoveToScene] Successfully moved {objectToMove.name} to {targetSceneName}", true);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Misc.Msg($"[MoveScene] [MoveToScene] Error moving {objectToMove.name}: {ex.Message}", true);
                    return false;
                }
            }
            else
            {
                Misc.Msg($"[MoveScene] [MoveToScene] Target Scene {targetSceneName} is invalid", true);

                // Try to create the DontDestroyOnLoad scene by using GameObject.DontDestroyOnLoad
                try
                {
                    GameObject tempObject = new GameObject("TempSceneCreator");
                    GameObject.DontDestroyOnLoad(tempObject);

                    // Try again
                    targetScene = SceneManager.GetSceneByName(targetSceneName);
                    if (targetScene.IsValid())
                    {
                        SceneManager.MoveGameObjectToScene(objectToMove, targetScene);
                        GameObject.Destroy(tempObject); // Clean up

                        if (Testing.Settings.logScene)
                        {
                            Misc.Msg($"[MoveScene] [MoveToScene] Successfully moved {objectToMove.name} to {targetSceneName} after creating scene", true);
                        }
                        return true;
                    }

                    GameObject.Destroy(tempObject); // Clean up
                }
                catch (Exception ex)
                {
                    Misc.Msg($"[MoveScene] [MoveToScene] Failed to create DontDestroyOnLoad scene: {ex.Message}", true);
                }

                return false;
            }
        }
    }
}
