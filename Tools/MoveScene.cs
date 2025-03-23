

using UnityEngine.SceneManagement;
using UnityEngine;
using SonsSdk;

namespace StoneGate.Tools
{
    internal static class MoveScene
    {
        public const string targetSceneName = "DontDestroyOnLoad";

        public static void MoveToScene(GameObject objectToMove)
        {
            if (objectToMove.scene.name == targetSceneName)
            {
                if (Testing.Settings.logScene)
                {
                    Misc.Msg($"[MoveScene] [MoveToScene] objectToMove is already in Target Scene", true);
                }
                return;
            }
            // Get the target scene
            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            if (targetScene.IsValid())
            {
                if (Testing.Settings.logScene)
                {
                    Misc.Msg($"[MoveScene] [MoveToScene] Target Scene is valid", true);
                }
                SceneManager.MoveGameObjectToScene(objectToMove, targetScene);
                objectToMove.DontDestroyOnLoad().HideAndDontSave();
            }
            else
            {
                Misc.Msg($"[MoveScene] [MoveToScene] Target Scene is invalid", true);
            }

        }
    }
}
