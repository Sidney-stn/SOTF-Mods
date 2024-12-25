using SonsSdk;

namespace Currency
{
    public class API
    {
        public enum GetCurrencyType
        {
            SteamID,
            PlayerName
        }

        public static void AddCash(GetCurrencyType currencyType, string playerIdOrSteamName, int amount)  // Adds Cash To Player
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { return; }
            if (string.IsNullOrEmpty(playerIdOrSteamName)) { Misc.Msg("[API] [AddCash] PlayerId Invalid"); return; }
            if (amount <= 0) { Misc.Msg("[API] [AddCash] Currency Invalid"); return; }

            if (currencyType == GetCurrencyType.SteamID)
            {
                // If AddCash Is Added To Local Player
                if (playerIdOrSteamName == Misc.MySteamId().Item2)
                {
                    LiveData.LocalPlayerData.AddCashToLocalPlayer(amount);
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SendSingleSyncCash
                    {
                        SenderName = Misc.GetLocalPlayerUsername(),
                        SenderId = Misc.MySteamId().Item2,
                        Currency = LiveData.LocalPlayerData.GetLocalPlayerCurrency() ?? 0,
                        ToPlayerId = "None"
                    });
                    Misc.Msg($"[API] [AddCash] Adding {amount} to Local Player");
                    return;
                }

                // Over Network
                Misc.Msg($"[API] [AddCash] Adding {amount} to {playerIdOrSteamName}");
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.AddCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    Currency = amount,
                    ToPlayerId = playerIdOrSteamName
                });
            } else if (currencyType == GetCurrencyType.PlayerName)
            {
                // If AddCash Is Added To Local Player
                if (playerIdOrSteamName == Misc.GetLocalPlayerUsername())
                {
                    LiveData.LocalPlayerData.AddCashToLocalPlayer(amount);
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SendSingleSyncCash
                    {
                        SenderName = Misc.GetLocalPlayerUsername(),
                        SenderId = Misc.MySteamId().Item2,
                        Currency = LiveData.LocalPlayerData.GetLocalPlayerCurrency() ?? 0,
                        ToPlayerId = "None"
                    });
                    Misc.Msg($"[API] [AddCash] Adding {amount} to Local Player");
                    return;
                }

                // Over Network
                var allPlayers = LiveData.Players.GetPlayers();
                bool found = false;
                foreach (var player in allPlayers)
                {
                    if (player.Value == playerIdOrSteamName)
                    {
                        // Over Network
                        SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.AddCash
                        {
                            SenderName = Misc.GetLocalPlayerUsername(),
                            SenderId = Misc.MySteamId().Item2,
                            Currency = amount,
                            ToPlayerId = player.Key
                        });
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Misc.Msg($"[API] [AddCash] Player: {playerIdOrSteamName} not found");
                }
                else
                {
                    Misc.Msg($"[API] [AddCash] Player: {playerIdOrSteamName} - Currency: {amount}");
                }
            }
        }

        public static void RemoveCash(GetCurrencyType currencyType, string playerIdOrSteamName, int amount)  // Removes Cash From Player
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { return; }
            if (string.IsNullOrEmpty(playerIdOrSteamName)) { Misc.Msg("[API] [RemoveCash] PlayerId Invalid"); return; }
            if (amount <= 0) { Misc.Msg("[API] [RemoveCash] Currency Invalid"); return; }

            if (currencyType == GetCurrencyType.SteamID)
            {
                // If RemoveCash Is Added To Local Player
                if (playerIdOrSteamName == Misc.MySteamId().Item2)
                {
                    LiveData.LocalPlayerData.RemoveCashFromLocalPlayer(amount);
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SendSingleSyncCash
                    {
                        SenderName = Misc.GetLocalPlayerUsername(),
                        SenderId = Misc.MySteamId().Item2,
                        Currency = LiveData.LocalPlayerData.GetLocalPlayerCurrency() ?? 0,
                        ToPlayerId = "None"
                    });
                    Misc.Msg($"[API] [RemoveCash] Removing {amount} from Local Player");
                    return;
                }

                // Over Network
                Misc.Msg($"[API] [RemoveCash] Adding {amount} to {playerIdOrSteamName}");
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RemoveCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    Currency = amount,
                    ToPlayerId = playerIdOrSteamName
                });
            }
            else if (currencyType == GetCurrencyType.PlayerName)
            {
                // If RemoveCash Is Added To Local Player
                if (playerIdOrSteamName == Misc.GetLocalPlayerUsername())
                {
                    LiveData.LocalPlayerData.RemoveCashFromLocalPlayer(amount);
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SendSingleSyncCash
                    {
                        SenderName = Misc.GetLocalPlayerUsername(),
                        SenderId = Misc.MySteamId().Item2,
                        Currency = LiveData.LocalPlayerData.GetLocalPlayerCurrency() ?? 0,
                        ToPlayerId = "None"
                    });
                    Misc.Msg($"[API] [RemoveCash] Removing {amount} from Local Player");
                    return;
                }

                // Over Network
                var allPlayers = LiveData.Players.GetPlayers();
                bool found = false;
                foreach (var player in allPlayers)
                {
                    if (player.Value == playerIdOrSteamName)
                    {
                        // Over Network
                        SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RemoveCash
                        {
                            SenderName = Misc.GetLocalPlayerUsername(),
                            SenderId = Misc.MySteamId().Item2,
                            Currency = amount,
                            ToPlayerId = player.Key
                        });
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Misc.Msg($"[API] [RemoveCash] Player: {playerIdOrSteamName} not found");
                }
                else
                {
                    Misc.Msg($"[API] [RemoveCash] Player: {playerIdOrSteamName} - Currency: {amount}");
                }
            }
        }

        public static void ForceSyncCash()  // Syncs Cash For All Players
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[API] [SyncCash] Syncing Cash");
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RequestSyncAllCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    ToPlayerId = "None"
                });
            }
        }

        public static void ForceSyncCashForSpesificPlayer(GetCurrencyType currencyType, string playerIdOrSteamName)  // Syncs Cash For Spesific Player
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[API] [SyncCash] Syncing Cash");
                if (currencyType == GetCurrencyType.SteamID)
                {
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RequestSyncAllCash
                    {
                        SenderName = Misc.GetLocalPlayerUsername(),
                        SenderId = Misc.MySteamId().Item2,
                        ToPlayerId = playerIdOrSteamName
                    });
                }
                else if (currencyType == GetCurrencyType.PlayerName)
                {
                    var allPlayers = LiveData.Players.GetPlayers();
                    bool found = false;
                    foreach (var player in allPlayers)
                    {
                        if (player.Value == playerIdOrSteamName)
                        {
                            SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RequestSyncAllCash
                            {
                                SenderName = Misc.GetLocalPlayerUsername(),
                                SenderId = Misc.MySteamId().Item2,
                                ToPlayerId = player.Key
                            });
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Misc.Msg($"[API] [SyncCash] Player: {playerIdOrSteamName} not found");
                    }
                }
                else
                {
                    Misc.Msg($"[API] [SyncCash] Invalid Currency Type");
                }
            }
        }

        public static int? GetCash(GetCurrencyType currencyType, string playerIdOrSteamName)  // Gets Cash For Spesific Player
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { return null; }
            if (string.IsNullOrEmpty(playerIdOrSteamName)) { Misc.Msg("[API] [GetCash] PlayerId Invalid"); return null; }
            Misc.Msg($"[API] [GetCash] Getting Cash For {playerIdOrSteamName}");
            if (currencyType == GetCurrencyType.SteamID)
            {
                return LiveData.Players.GetPlayerCurrency(LiveData.Players.GetCurrencyType.SteamID, playerIdOrSteamName);
            }
            else if (currencyType == GetCurrencyType.PlayerName)
            {
                var allPlayers = LiveData.Players.GetPlayers();
                foreach (var player in allPlayers)
                {
                    if (player.Value == playerIdOrSteamName)
                    {
                        return LiveData.Players.GetPlayerCurrency(LiveData.Players.GetCurrencyType.SteamID, player.Key);
                    }
                }
                return null;
            }
            return null;
        }

        public static Dictionary<string, int> GetAllCash()  // Gets Cash For All Players. <StringId, CashAmount>
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer || Misc.hostMode == Misc.SimpleSaveGameType.NotIngame) { return null; }
            Misc.Msg($"[API] [GetAllCash] Getting Cash For All Players");

            return LiveData.Players.GetPlayersCurrency();
        }
    }
}
