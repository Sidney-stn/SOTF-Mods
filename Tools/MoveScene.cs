

using UnityEngine.SceneManagement;
using UnityEngine;
using SonsSdk;


namespace SimpleElevator.Tools
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

            // Check if already in DontDestroyOnLoad scene
            if (objectToMove.scene.name == targetSceneName)
            {
                //if (Settings.logScene)
                //{
                //    Misc.Msg($"[MoveScene] [MoveToScene] {objectToMove.name} is already in Target Scene", true);
                //}
                return true;
            }

            try
            {
                // Instead of trying to find and use the DontDestroyOnLoad scene directly,
                // just use Unity's built-in DontDestroyOnLoad method
                UnityEngine.Object.DontDestroyOnLoad(objectToMove);

                // Additional Unity extensions if needed 
                objectToMove.HideAndDontSave();

                //if (Settings.logScene)
                //{
                //    Misc.Msg($"[MoveScene] [MoveToScene] Successfully moved {objectToMove.name} to DontDestroyOnLoad scene", true);
                //}
                return true;
            }
            catch (Exception ex)
            {
                Misc.Msg($"[MoveScene] [MoveToScene] Error moving {objectToMove.name}: {ex.Message}", true);
                return false;
            }
        }
    }
}
