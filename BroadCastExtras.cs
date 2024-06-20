using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using TheForest.Utils;
using Sons.Gameplay.GameSetup;
using Sons.Multiplayer;
using Sons.Gui;

namespace BroadcastMessage
{
    internal class BroadCastExtras
    {
        public static ModalDialogManager dialogManager;

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

        public static void AddOnQuitFromMultiplayer()
        {
            if (hostMode == SimpleSaveGameType.Multiplayer && !GameServerManager.IsDedicatedServer)
            {
                BroadCastExtras.dialogManager.QuitGameConfirmDialog.AddOnOption1ClickedCallback((Il2CppSystem.Action)BroadcastMessage.OnLeaveWorld);
                Misc.Msg("Added OnLeaveWorld");
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

        public static void CheckHostModeOnWorldUpdate()
        {
            if (LocalPlayer.IsInWorld)
            {
                if (hostMode != SimpleSaveGameType.NotIngame)
                {
                    BroadCastEvents.OnHostModeGotten?.Invoke(typeof(BroadCastExtras), EventArgs.Empty);
                }
            }
        }

        public static void OnHostModeGottenCorrectly(object sender, EventArgs e)
        {
            SonsSdk.SdkEvents.OnInWorldUpdate.Unsubscribe(BroadCastExtras.CheckHostModeOnWorldUpdate);
            if (!GameServerManager.IsDedicatedServer && BroadCastExtras.hostMode == SimpleSaveGameType.Multiplayer)
            {
                BroadcastMessage.isDedicated = false;  // Sets Static Vars In BroadcastMessage. Used for fixing quit errors
                BroadcastMessage.saveTypeGotten = SimpleSaveGameType.Multiplayer;  // Sets Static Vars In BroadcastMessage. Used for fixing quit errors

                dialogManager = BroadCastExtras.FindObjectInSpecificScene().GetComponent<ModalDialogManager>();
                if (dialogManager != null)
                {
                    Misc.Msg("Dialog Manager Found");
                }
                else
                {
                    Misc.Msg("Dialog Manager is NOT Found!");
                }
                Misc.Msg("Running From MultiplayerHost");
                BroadcastInfo.SetAndActivateBotManager();
                BroadcastInfo.InitilizeMonoBehavior();
                BroadCastExtras.AddOnQuitFromMultiplayer();
            }
            else if (GameServerManager.IsDedicatedServer)
            {
                BroadcastMessage.isDedicated = true;  // Sets Static Vars In BroadcastMessage. Used for fixing quit errors
                BroadcastMessage.saveTypeGotten = SimpleSaveGameType.Multiplayer;  // Sets Static Vars In BroadcastMessage. Used for fixing quit errors
                Misc.Msg("Running From Dedicated Server");
                BroadcastInfo.SetAndActivateBotManager();
                BroadcastInfo.InitilizeMonoBehavior();
            }
            else { BroadcastMessage.saveTypeGotten = SimpleSaveGameType.SinglePlayer; }

            Misc.ErrorMsg($"IsDedicatedServer: {GameServerManager.IsDedicatedServer}, HostMode: {BroadCastExtras.hostMode}");
        }

    }
}
