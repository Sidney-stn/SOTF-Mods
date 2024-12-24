using UnityEngine;


namespace Warps.Saving
{
    internal class Load
    {
        public static Dictionary<string, Vector3> Warps = new Dictionary<string, Vector3>();
        internal static Queue<Saving.Manager.WarpsManager> deferredLoadQueue = new Queue<Saving.Manager.WarpsManager>();

        internal static void ProcessLoadData(Saving.Manager.WarpsManager obj)
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Loading] Skipped Loading Warps On Multiplayer Client");
                return;
            }
            Warps.Clear();

            Misc.Msg($"[Loading] Warp's From Save: {obj.Warps.Count.ToString()}");
            foreach (var warpData in obj.Warps)
            {
                Misc.Msg("[Loading] Creating New Signs");
                if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
                {
                    Saving.LoadedWarps.loadedWarps.Add(warpData.WarpName, warpData.Position);
                }
                else if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
                {
                    Saving.LoadedWarps.loadedWarps.Add(warpData.WarpName, warpData.Position);
                }
            }
        }
    }
}
