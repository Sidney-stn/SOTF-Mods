

using Sons.Gui.Input;
using UnityEngine;

namespace WirelessSignals.Tools
{
    internal class CreatorSettings
    {
        private static bool _lastState = false;
        internal static bool lastState // Updated From Host Owner The Network
        { 
            get { return _lastState; } 
            set 
            {
                Misc.Msg($"[CreatorSettings] [lastState] Set To {value}");
                _lastState = value;
                UpdateStateOfObjecs();
            } 
        }

        public static void OnCloseSettingsForUpdate()
        {
            Misc.Msg("[CreatorSettings] [OnCloseSettingsForUpdate] Called");
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer)
            {
                if (lastState == Config.OwnerToEdit.Value)
                {
                    Misc.Msg("[CreatorSettings] [OnCloseSettingsForUpdate] LastState Not Updated");
                    return;
                }
                lastState = Config.OwnerToEdit.Value;

                UpdateStateOfObjecs();

                // Raise NetWork Event To Sync LastState
            }
        }

        internal static void UpdateStateOfObjecs()
        {
            // No need to update if on singleplayer
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[CreatorSettings] [OnCloseSettingsForUpdate] LastState Updated");
                foreach (var reciver in WirelessSignals.reciver.spawnedGameObjects)  // Reciver Prefab
                {
                    // Reciver Has Ui For Linking. Reciver Trigger Objects On Recived Signal. Example: DefensiveWall
                    // The LinkUi Is Removed Or Added Based On The LastState Note Only In Multiplayer
                    // If LinkUi Is Removed, The Reciver Will Not Be Able To Be Linked To A Recive Trigger
                    // If LinkUi Is Added, The Reciver Will Be Able To Be Linked To A Recive Trigger
                    // Linking Via Hammer Is Still Possible And This Case Needs To Be Handled in LineRenderer.cs

                    Mono.Reciver controller = reciver.Value.GetComponent<Mono.Reciver>();
                    if (controller != null)
                    {
                        if (controller.isSetupPrefab)  // No Need To Update SetupPrefab
                        { continue; }

                        LinkUiElement linkUi = controller._linkUi;
                        if (lastState)  // If you must be owner to edit
                        {
                            if (controller.ownerSteamId == Misc.GetMySteamId())
                            {
                                if (linkUi == null)  // Create LinkUi If You Are The Owner
                                {
                                    controller._linkUi = UI.LinkUi.CreateLinkUi(reciver.Value, 2f, null, Assets.UIAdjust, new Vector3(0, 0f, 0), "screen.take");
                                }  // If Already Added No Need To Add Again
                            }
                            else  // If You Are Not The Owner
                            {
                                if (linkUi != null)  // Destroy LinkUi If You Are Not The Owner
                                {
                                    GameObject.Destroy(linkUi);
                                }  // Do nothing if already removed
                            }
                        }
                        else  // Everyone can edit
                        {
                            if (linkUi == null)  // Create LinkUi So Everyone Can Edit, No Extra Checks
                            {
                                controller._linkUi = UI.LinkUi.CreateLinkUi(reciver.Value, 2f, null, Assets.UIAdjust, new Vector3(0, 0f, 0), "screen.take");
                            }  // If Already Added No Need To Add Again
                        }
                    }
                }
                // More Prefabs Can Be Added Here
            }
            else
            {
                Misc.Msg("[CreatorSettings] [OnCloseSettingsForUpdate] LastState Not Updated, Not In Multiplayer");
            }
        }

        internal static bool IsOwner(string ownerSteamId)
        {
            if (BoltNetwork.isRunning == false) { if (ownerSteamId == null) { return true; } }
            if (ownerSteamId == Misc.GetMySteamId()) { return true; }
            else if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer) { return true; }
            else { return false; }
        }
    }
}
