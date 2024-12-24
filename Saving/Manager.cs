using Endnight.Extensions;
using SonsSdk;
using UnityEngine;

namespace Warps.Saving
{
    internal class Manager : ICustomSaveable<Manager.WarpsManager>
    {
        public string Name => "WarpsManager";

        // Used to determine if the data should also be saved in multiplayer saves
        public bool IncludeInPlayerSave => false;

        public WarpsManager Save()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Saving] Only Host/Singleplayer Saves");
                return null;
            }
            var saveData = new WarpsManager();

            // Warps
            if (Saving.LoadedWarps.loadedWarps.Count != 0 || Saving.LoadedWarps.loadedWarps != null)
            {
                foreach (var signsGameObject in Saving.LoadedWarps.loadedWarps)
                {
                    if (signsGameObject.Key == null || signsGameObject.Value == Vector3.zero) { continue; }  // Skip Invalid Warps

                    var WarpsModData = new WarpsManager.WarpsModData
                    {
                        WarpName = signsGameObject.Key,
                        Position = signsGameObject.Value
                    };

                    saveData.Warps.Add(WarpsModData);
                    Misc.Msg("[Saving] Added Warp To Save List");
                }
            }
            else { Misc.Msg("[Saving] No Warp found in LST, skipped saving"); }

            return saveData;
        }

        public void Load(WarpsManager obj)
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

        public class WarpsManager
        {
            public List<WarpsModData> Warps = new List<WarpsModData>();

            public class WarpsModData
            {
                public string WarpName;
                public Vector3 Position;
            }

        }
    }
}
