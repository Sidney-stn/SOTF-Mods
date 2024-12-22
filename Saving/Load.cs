using UnityEngine;

namespace Signs.Saving
{
    internal class Load
    {
        public static List<GameObject> ModdedSigns = new List<GameObject>();
        internal static Queue<Saving.Manager.SignsManager> deferredLoadQueue = new Queue<Saving.Manager.SignsManager>();

        internal static void ProcessLoadData(Saving.Manager.SignsManager obj)
        {

            // Signs Prefab
            ModdedSigns.Clear();
            Misc.Msg($"[Loading] Signs From Save: {obj.Signs.Count.ToString()}");
            foreach (var signsData in obj.Signs)
            {
                Misc.Msg("[Loading] Creating New Signs");
                if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
                {
                    GameObject sign = Prefab.SignPrefab.spawnSignSingePlayer(signsData.Position, signsData.Rotation, false, signsData.Line1Text, signsData.Line2Text, signsData.Line3Text, signsData.Line4Text, signsData.UniqueId);
                }
                else if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
                {
                    GameObject sign = Prefab.SignPrefab.spawnSignMultiplayer(signsData.Position, signsData.Rotation, signsData.UniqueId);
                }
            }
        }
    }
}
