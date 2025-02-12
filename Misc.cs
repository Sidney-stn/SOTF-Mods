using RedLoader;
using Sons.Gameplay.GameSetup;
using Sons.Gui;
using TheForest.Utils;
using UnityEngine.SceneManagement;
using UnityEngine;
using Bolt;
using Scene = UnityEngine.SceneManagement.Scene;
using Steamworks;
using Sons.Multiplayer;

namespace WirelessSignals
{
    internal class Misc
    {
        internal static void Msg(string msg)
        {
            if (Config.DebugLogging.Value)
            {
                RLog.Msg(msg);
            }

        }


        // On Host Mode Gotten Event
        public static EventHandler OnHostModeGotten;

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
            dialogManager.QuitGameConfirmDialog.AddOnOption1ClickedCallback((Il2CppSystem.Action)WirelessSignals.OnLeaveWorld);  // Quit World Confirm Button Press
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
            //Network.Manager.RegisterEvents();

            //// Process all deferred load data
            //while (Saving.Load.deferredLoadQueue.Count > 0)
            //{
            //    var obj = Saving.Load.deferredLoadQueue.Dequeue();
            //    Saving.Load.ProcessLoadData(obj);
            //}

            //// Add Host Player To System
            //LiveData.Host.AddHostPlayerToSystem();

            //// Trigger Event For Host Mode Gotten
            //API.SubscribableEvents.TriggerOnJoinWorld();
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

        public static string GetLocalPlayerUsername()
        {
            if (Misc.GetHostMode() == SimpleSaveGameType.SinglePlayer) { return "SmokyAce"; }
            if (LocalPlayer.Entity.Entity.NetworkId == new NetworkId(0)) { Misc.Msg("Error GetLocalPlayerUsername"); return null; }
            var playerSetup = GameObject.FindObjectsOfType<BoltPlayerSetup>()
                                        .FirstOrDefault(bps => bps._entity._entity.NetworkId == LocalPlayer.Entity.Entity.NetworkId);
            if (playerSetup != null)
            {
                string username = playerSetup.state.name;
                return username;
            }
            return null;
        }

        public static (ulong, string) MySteamId()
        {
            ulong? mySteamId = MultiplayerUtilities.GetSteamId(LocalPlayer.Entity);
            if (mySteamId == null || mySteamId == 0)
            {
                Steamworks.CSteamID SteamID = SteamUser.GetSteamID();
                string SteamIDString = SteamID.ToString();
                ulong resultSteamID;
                if (ulong.TryParse(SteamIDString, out resultSteamID))
                {
                    return (resultSteamID, SteamIDString);
                }
                else { Misc.Msg("[MySteamId()] Failed To Get MySteamId! Returned null"); return (0, null); }
            }
            else
            {
                return ((ulong)mySteamId, mySteamId.ToString());
            }
        }
    }
}
