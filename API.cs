using SonsSdk;

namespace Banking
{
    public class API
    {
        public enum GetCurrencyType
        {
            SteamID,
            PlayerName
        }

        public enum SyncType
        {
            All,
            Me
        }

        public static void AddCash(GetCurrencyType currencyType, string playerIdOrSteamName, int amount)  // Adds Cash To Player
        {
            LiveData.Players.AddCashToPlayer(LiveData.Players.GetCurrencyType.SteamID, playerIdOrSteamName, amount);
        }

        public static void RemoveCash(GetCurrencyType currencyType, string playerIdOrSteamName, int amount)  // Removes Cash From Player
        {
            LiveData.Players.RemoveCashFromPlayer(LiveData.Players.GetCurrencyType.SteamID, playerIdOrSteamName, amount);
        }

        public static void ForceSyncCash(SyncType type)  // Syncs Cash For All Players
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[API] [SyncCash] Syncing Cash");
                if (type == SyncType.All)
                {
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RequestSyncCash
                    {
                        SenderName = Misc.GetLocalPlayerUsername(),
                        SenderId = Misc.MySteamId().Item2,
                        RepsondToId = "None"
                    });
                } else if (type == SyncType.Me)
                {
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RequestSyncCash
                    {
                        SenderName = Misc.GetLocalPlayerUsername(),
                        SenderId = Misc.MySteamId().Item2,
                        RepsondToId = Misc.MySteamId().Item2
                    });
                }

            }
        }

        public static int? GetCash(GetCurrencyType currencyType, string playerIdOrSteamName)  // Gets Cash For Spesific Player
        {
            if (currencyType == GetCurrencyType.SteamID)
            {
                return LiveData.Players.GetPlayerCurrency(LiveData.Players.GetCurrencyType.SteamID, playerIdOrSteamName);
            }
            else if (currencyType == GetCurrencyType.PlayerName)
            {
                return LiveData.Players.GetPlayerCurrency(LiveData.Players.GetCurrencyType.PlayerName, playerIdOrSteamName);
            }
            else
            {
                Misc.Msg($"[API] [GetCash] Invalid Currency Type");
                return null;
            }
        }

        public static Dictionary<string, int> GetAllCash()  // Gets Cash For All Players. <StringId, CashAmount>
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { return null; }
            Misc.Msg($"[API] [GetAllCash] Getting Cash For All Players");

            // Returns Dictonary of <StringId, CashAmount>
            return LiveData.Players.GetPlayersCurrency();
        }

        public static string GetLocalPlayerName()  // Gets Local Player Name
        {
            return Misc.GetLocalPlayerUsername();
        }

        public static string GetLocalPlayerId()  // Gets Local Player SteamID
        {
            return Misc.MySteamId().Item2;
        }

        public static class SubscribableEvents
        {
            public static event Action OnPlayerJoin;

            public static event Action OnCashChange;

            public static event Action OnLeaveWorld;

            public static event Action OnJoinWorld;  // When Player Joins World Or More Important: When LocalPlayer Host Mode Is Gotten Correctly

            internal static void TriggerOnPlayerJoin()
            {
                OnPlayerJoin?.Invoke();
            }

            internal static void TriggerOnCashChange()
            {
                OnCashChange?.Invoke();
            }

            internal static void TriggerOnLeaveWorld()
            {
                OnLeaveWorld?.Invoke();
            }

            internal static void TriggerOnJoinWorld()
            {
                OnJoinWorld?.Invoke();
            }
        }
    }
}
