using SonsSdk;
using UnityEngine;

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

        public static void AddCash(GetCurrencyType currencyType, string playerIdOrSteamName, int amount)
        {
            if (string.IsNullOrEmpty(playerIdOrSteamName)) { throw new ArgumentNullException(nameof(playerIdOrSteamName), "Invalid PlayerIdOrSteamName"); }
            if (amount <= 0) { throw new ArgumentOutOfRangeException(nameof(amount), $"Invalid Currency Amount: {amount}"); }
            switch (currencyType)
            {
                case GetCurrencyType.SteamID:
                    LiveData.Players.AddCashToPlayer(LiveData.Players.GetCurrencyType.SteamID, playerIdOrSteamName, amount);
                    break;

                case GetCurrencyType.PlayerName:
                    LiveData.Players.AddCashToPlayer(LiveData.Players.GetCurrencyType.PlayerName, playerIdOrSteamName, amount);
                    break;

                default:
                    Misc.Msg($"[API] [AddCash] Invalid Currency Type: {currencyType}");
                    throw new ArgumentOutOfRangeException(nameof(currencyType), $"Invalid Currency Type: {currencyType}");
            }
        }

        public static void RemoveCash(GetCurrencyType currencyType, string playerIdOrSteamName, int amount)
        {
            if (string.IsNullOrEmpty(playerIdOrSteamName)) { throw new ArgumentNullException(nameof(playerIdOrSteamName), "Invalid PlayerIdOrSteamName"); }
            if (amount <= 0) { throw new ArgumentOutOfRangeException(nameof(amount), $"Invalid Currency Amount: {amount}"); }
            switch (currencyType)
            {
                case GetCurrencyType.SteamID:
                    LiveData.Players.RemoveCashFromPlayer(LiveData.Players.GetCurrencyType.SteamID, playerIdOrSteamName, amount);
                    break;

                case GetCurrencyType.PlayerName:
                    LiveData.Players.RemoveCashFromPlayer(LiveData.Players.GetCurrencyType.PlayerName, playerIdOrSteamName, amount);
                    break;

                default:
                    Misc.Msg($"[API] [RemoveCash] Invalid Currency Type: {currencyType}");
                    throw new ArgumentOutOfRangeException(nameof(currencyType), $"Invalid Currency Type: {currencyType}");
            }
        }

        public static void ForceSyncCash(SyncType type)  // Syncs Cash For All Players
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[API] [SyncCash] Syncing Cash");
                switch (type)
                {
                    case SyncType.All:
                        SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RequestSyncCash
                        {
                            SenderName = Misc.GetLocalPlayerUsername(),
                            SenderId = Misc.MySteamId().Item2,
                            RepsondToId = "None"
                        });
                        break;
                    case SyncType.Me:
                        SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RequestSyncCash
                        {
                            SenderName = Misc.GetLocalPlayerUsername(),
                            SenderId = Misc.MySteamId().Item2,
                            RepsondToId = Misc.MySteamId().Item2
                        });
                        break;
                    default:
                        Misc.Msg($"[API] [ForceSyncCash] Invalid Sync Type: {type}");
                        throw new ArgumentOutOfRangeException(nameof(type), $"Invalid Sync Type: {type}");
                }
            } else { throw new Exception("ForceSyncCash() Can't be called when in SinglePlayer Or NotIngame"); }
        }

        public static int? GetCash(GetCurrencyType currencyType, string playerIdOrSteamName)  // Gets Cash For Spesific Player
        {
            if (string.IsNullOrEmpty(playerIdOrSteamName)) { throw new ArgumentNullException(nameof(playerIdOrSteamName), "Invalid PlayerIdOrSteamName"); }
            switch (currencyType)  // Returns Cash Amount (int
            {
                case GetCurrencyType.SteamID:
                    return LiveData.Players.GetPlayerCurrency(LiveData.Players.GetCurrencyType.SteamID, playerIdOrSteamName);

                case GetCurrencyType.PlayerName:
                    return LiveData.Players.GetPlayerCurrency(LiveData.Players.GetCurrencyType.PlayerName, playerIdOrSteamName);

                default:
                    Misc.Msg($"[API] [GetCash] Invalid Currency Type: {currencyType}");
                    throw new ArgumentOutOfRangeException(nameof(currencyType), $"Invalid Currency Type: {currencyType}");
            }
        }

        public static Dictionary<string, int> GetAllCash()  // Gets Cash For All Players. <StringId, CashAmount>
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { throw new Exception("GetAllCash() Can't be called when in SinglePlayer Or NotIngame"); }
            Misc.Msg($"[API] [GetAllCash] Getting Cash For All Players");

            // Returns Dictonary of <StringId, CashAmount>
            return LiveData.Players.GetPlayersCurrency();
        }

        public static string GetLocalPlayerName()  // Gets Local Player Name
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { throw new Exception("GetLocalPlayerName() Can't be called when NotIngame"); }
            return Misc.GetLocalPlayerUsername();
        }

        public static string GetLocalPlayerId()  // Gets Local Player SteamID
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { throw new Exception("GetLocalPlayerId() Can't be called when NotIngame"); }
            return Misc.MySteamId().Item2;
        }

        public static GameObject SpawnNewAtm(Vector3 position, Quaternion rotation)
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { throw new Exception("SpawnNewAtm() Can't be called when in SinglePlayer Or NotIngame"); }
            if (position == Vector3.zero) { throw new ArgumentNullException(nameof(position), "Invalid Position"); }
            GameObject newAtm = Prefab.ActiveATM.SpawnATM(position, rotation);
            if (newAtm != null)
            {
                return newAtm;
            } else
            {
                return null;
            }
        }

        public static bool DeleteAtm(GameObject atm)
        {
            if (atm == null) { throw new ArgumentNullException(nameof(atm), "Invalid ATM GameObject"); }
            bool sucess = Prefab.ActiveATM.DeleteATM(atm);
            return sucess;
        }

        public static bool DeleteAtm(string uniqueId)
        {
            if (string.IsNullOrEmpty(uniqueId)) { throw new ArgumentNullException(nameof(uniqueId), "Invalid UniqueId"); }
            GameObject atm = Prefab.ActiveATM.FindShopByUniqueId(uniqueId);
            if (atm == null) { return false; }
            bool sucess = Prefab.ActiveATM.DeleteATM(atm);
            return sucess;
        }

        public static GameObject GetATMFromUniqueId(string uniqueId)
        {
            if (string.IsNullOrEmpty(uniqueId)) { throw new Exception("GetATMFromUniqueId() Invalid UniqueId"); }
            return Prefab.ActiveATM.FindShopByUniqueId(uniqueId);
        }
        public static List<GameObject> GetAllSpawnedATMs()  // Returns All Spawned ATM's In A List Of GameObjects
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { throw new Exception("GetAllSpawnedATMs() Can't be called when in SinglePlayer Or NotIngame"); }
            return Saving.Load.ModdedAtms;
        }

        public static Dictionary<string, GameObject> GetAllSpawnedATMsWithUniqueId()  // Return All Spawned ATM's In A Dictionary Of <UniqueId, GameObject>
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { throw new Exception("GetAllSpawnedATMsWithUniqueId() Can't be called when in SinglePlayer Or NotIngame"); }
            return Prefab.ActiveATM.spawnedAtms;
        }

        public static string GetUniqueIdFromATM(GameObject atm)
        {
            if (atm == null) { throw new ArgumentNullException(nameof(atm), "Invalid ATM GameObject"); }
            Mono.ATMController atmController = atm.GetComponent<Mono.ATMController>();
            if (atmController == null) { return null; }
            return atmController.UniqueId;
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
