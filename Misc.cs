using RedLoader;
using Sons.Gui;
using TheForest.Utils;
using UnityEngine;
using Sons.Gameplay.GameSetup;
using UnityEngine.SceneManagement;

namespace StructureDamageViewer
{
    internal class Misc
    {
        internal static void Msg(string msg)
        {
            if (Config.StructureDamageViewerLogging.Value)
            {
                RLog.Msg(msg);
            }
        }

        // On Host Mode Gotten Event
        public static EventHandler OnHostModeGotten;

        // Stores DamageMonoes
        internal static List<DamageMono> damageMonos = new List<DamageMono>();

        // Stores Instance
        internal static LocalPlayerTrackMono localPlayerTrackMono = null;

        // DialogManager For Getting Quit Button Press
        public static ModalDialogManager dialogManager;

        public static void CheckHostModeOnWorldUpdate()
        {
            if (LocalPlayer.IsInWorld)
            {
                if (hostMode != SimpleSaveGameType.NotIngame)
                {
                    OnHostModeGotten?.Invoke(typeof(Misc), EventArgs.Empty);
                }
            }
        }

        public enum SimpleSaveGameType
        {
            SinglePlayer,
            Multiplayer,
            MultiplayerClient,
            NotIngame,
        }

        public static SimpleSaveGameType? hostMode
        {
            get { return GetHostMode(); }
        }

        private static SimpleSaveGameType? GetHostMode()
        {
            if (!LocalPlayer.IsInWorld) { return SimpleSaveGameType.NotIngame; }
            var saveType = GameSetupManager.GetSaveGameType();
            switch (saveType)
            {
                case Sons.Save.SaveGameType.SinglePlayer:
                    return SimpleSaveGameType.SinglePlayer;
                case Sons.Save.SaveGameType.Multiplayer:
                    return SimpleSaveGameType.Multiplayer;
                case Sons.Save.SaveGameType.MultiplayerClient:
                    return SimpleSaveGameType.MultiplayerClient;
            }
            return SimpleSaveGameType.NotIngame;
        }

        public static void AddOnQuitWorld()
        {
            dialogManager.QuitGameConfirmDialog.AddOnOption1ClickedCallback((Il2CppSystem.Action)StructureDamageViewer.OnLeaveWorld);
            Misc.Msg("Added OnLeaveWorld Event");

        }

        public static void OnHostModeGottenCorrectly(object sender, EventArgs e)
        {
            Misc.Msg($"[MISC] [OnHostModeGottenCorrectly] HostMode: {Misc.hostMode}");
            SonsSdk.SdkEvents.OnInWorldUpdate.Unsubscribe(Misc.CheckHostModeOnWorldUpdate);

            dialogManager = FindObjectInSpecificScene().GetComponent<ModalDialogManager>();
            if (dialogManager != null)
            {
                Misc.Msg("Dialog Manager Found");
            }
            else
            {
                Misc.Msg("Dialog Manager is NOT Found!");
            }
            AddOnQuitWorld();
        }

        public static GameObject FindObjectInSpecificScene(string sceneName = "SonsMain", string objectName = "ModalDialogManager") // ModalDialogManager as Standard
        {
            // Get the scene by its name
            Scene scene = SceneManager.GetSceneByName(sceneName);

            // Check if the scene is valid and loaded
            if (scene.IsValid() && scene.isLoaded)
            {
                // Get all root GameObjects in the scene
                GameObject[] rootGameObjects = scene.GetRootGameObjects();

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
