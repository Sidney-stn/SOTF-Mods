using Endnight.Extensions;
using SonsSdk;
using UnityEngine;

namespace Signs.Saving
{
    internal class Manager : ICustomSaveable<Manager.SignsManager>
    {
        public string Name => "SignsManager";

        // Used to determine if the data should also be saved in multiplayer saves
        public bool IncludeInPlayerSave => false;

        public SignsManager Save()
        {
            Misc.Msg("[Saving] Saving SignsManager");
            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Saving] Only Host Saves");
                return null;
            }
            var saveData = new SignsManager();

            // Signs
            if (Saving.Track.spawnedSigns.Count != 0 || Saving.Track.spawnedSigns != null)
            {
                foreach (var signsGameObject in Saving.Track.spawnedSigns)
                {
                    if (signsGameObject.Value == null) { continue; }
                    Mono.SignController current_obj_controller = signsGameObject.Value.GetComponent<Mono.SignController>();
                    if (current_obj_controller != null)
                    {
                        if (current_obj_controller.GetPos() == Vector3.zero) { continue; }
                        var SignsModData = new SignsManager.SignsModData
                        {
                            Position = current_obj_controller.GetPos(),
                            Rotation = current_obj_controller.GetCurrentRotation(),
                            Line1Text = current_obj_controller.GetLineText(1),
                            Line2Text = current_obj_controller.GetLineText(2),
                            Line3Text = current_obj_controller.GetLineText(3),
                            Line4Text = current_obj_controller.GetLineText(4)
                        };

                        saveData.Signs.Add(SignsModData);
                        Misc.Msg("[Saving] Added Sign To Save List");
                    }
                }
            }
            else { Misc.Msg("[Saving] No Sign found in LST, skipped saving"); }

            return saveData;
        }

        public void Load(SignsManager obj)
        {

            if (Misc.hostMode == Misc.SimpleSaveGameType.NotIngame)
            {
                // Enqueue the load data if host mode is not ready
                Misc.Msg("[Loading] Host mode not ready, deferring load.");
                Saving.Load.deferredLoadQueue.Enqueue(obj);
                return;
            }

            // Process the load data
            Saving.Load.ProcessLoadData(obj);
        }

        public class SignsManager
        {
            public List<SignsModData> Signs = new List<SignsModData>();

            public class SignsModData
            {
                public Vector3 Position;
                public Quaternion Rotation;
                public string Line1Text;
                public string Line2Text;
                public string Line3Text;
                public string Line4Text;
            }

        }
    }
}
