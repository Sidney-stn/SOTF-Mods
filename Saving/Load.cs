using UnityEngine;

namespace Banking.Saving
{
    internal class Load
    {
        public static List<GameObject> ModdedAtms = new List<GameObject>();
        public static List<GameObject> ModdedATMPlacers = new List<GameObject>();
        
        internal static Queue<Saving.Manager.BankingManager> deferredLoadQueue = new Queue<Saving.Manager.BankingManager>();

        internal static void ProcessLoadData(Saving.Manager.BankingManager obj)
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[Loading] Skipped Loading Atms On Multiplayer Client");
                return;
            }
            // Atms Prefab
            ModdedAtms.Clear();
            Misc.Msg($"[Loading] Atms From Save: {obj.Atms.Count.ToString()}");
            foreach (var atmData in obj.Atms)
            {
                Misc.Msg("[Loading] Creating New Atms");
                if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
                {
                    Prefab.ActiveATM.SpawnATM(atmData.Position, atmData.Rotation, atmData.UniqueId);
                }
            }

            // ATMPlacers Prefab
            ModdedATMPlacers.Clear();
            Misc.Msg($"[Loading] ATMPlacers From Save: {obj.ATMPlacers.Count.ToString()}");
            foreach (var atmData in obj.ATMPlacers)
            {
                Misc.Msg("[Loading] Creating New ATMPlacers");
                if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
                {
                    Prefab.ATMPlacer.PlacePrefab(atmData.Position, atmData.Rotation, false, atmData.UniqueId);
                }
            }

            // Update Players And Players Currency
            LiveData.Players.UpdatePlayersAndCash(obj.SavedPlayers, obj.SavedPlayersCurrency);

            // Add Host Player To System
            LiveData.Host.AddHostPlayerToSystem();
        }
    }
}
