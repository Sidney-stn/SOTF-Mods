

namespace Banking.LiveData
{
    internal static class Players
    {
        // These will only include players that are currently in the game.
        private static Dictionary<string, string> _players = new Dictionary<string, string>();  // PlayerSteamID, PlayerName
        private static Dictionary<string, int> _playersCurrency = new Dictionary<string, int>();  // PlayerSteamID, PlayerCash

        internal static void AddPlayer(string steamId, string playerName)
        {
            if (_players.ContainsKey(steamId))
            {
                _players[steamId] = playerName;
            }
            else
            {
                _players.Add(steamId, playerName);
                _playersCurrency.Add(steamId, 0);
                Misc.Msg($"[JoinedServer] Added New Player: {playerName}");
            }
        }

        internal static Dictionary<string, string> GetPlayers()
        {
            return _players;
        }

        internal static Dictionary<string, int> GetPlayersCurrency()
        {
            return _playersCurrency;
        }

        internal static void UpdatePlayersAndCash(Dictionary<string, string> players, Dictionary<string, int> playersCurrency)
        {
            _players = players;
            _playersCurrency = playersCurrency;
        }

        internal enum GetCurrencyType
        {
            SteamID,
            PlayerName
        }

        internal static int? GetPlayerCurrency(GetCurrencyType type, string steamIdOrPlayerName)
        {
            if (type == GetCurrencyType.SteamID)
            {
                if (_playersCurrency.ContainsKey(steamIdOrPlayerName))
                {
                    return _playersCurrency[steamIdOrPlayerName];
                }
                return null;
            }
            else if (type == GetCurrencyType.PlayerName)
            {
                foreach (var player in _players)
                {
                    if (player.Value == steamIdOrPlayerName)
                    {
                        return _playersCurrency[player.Key];
                    }
                }
                return null;
            }

            return null;
        }

        internal static void CleanUp()
        {
            _players.Clear();
            _playersCurrency.Clear();
        }

        internal static void AddCashToPlayer(GetCurrencyType type, string steamIdOrPlayerName, int amount)
        {
            if (Misc.hostMode != Misc.SimpleSaveGameType.Multiplayer) { Misc.Msg("[Players] [AddCashToPlayer] Only Host Can Add Cash To Player"); return; }
            if (!string.IsNullOrEmpty(steamIdOrPlayerName)) { Misc.Msg("[Players] [AddCashToPlayer] Invalid Steam Id Or Username"); return; }
            if (amount < 0) { Misc.Msg("[Players] [AddCashToPlayer] Invalid Amount"); return; }
            if (type == GetCurrencyType.SteamID)
            {
                bool found = false;
                if (_playersCurrency.ContainsKey(steamIdOrPlayerName))
                {
                    _playersCurrency[steamIdOrPlayerName] += amount;
                }
                if (!found) { Misc.Msg("[Players] [AddCashToPlayer] Player Not Found"); }
            }
            else if (type == GetCurrencyType.PlayerName)
            {
                bool found = false;
                foreach (var player in _players)
                {
                    if (player.Value == steamIdOrPlayerName)
                    {
                        _playersCurrency[player.Key] += amount;
                    }
                }
                if (!found) { Misc.Msg("[Players] [AddCashToPlayer] Player Not Found"); }
            }
            else { Misc.Msg("[Players] [AddCashToPlayer] Invalid GetCurrencyType"); }
        }

        internal static void RemoveCashFromPlayer(GetCurrencyType type, string steamIdOrPlayerName, int amount)
        {
            if (Misc.hostMode != Misc.SimpleSaveGameType.Multiplayer) { Misc.Msg("[Players] [RemoveCashFromPlayer] Only Host Can Remove Cash From Player"); return; }
            if (!string.IsNullOrEmpty(steamIdOrPlayerName)) { Misc.Msg("[Players] [RemoveCashFromPlayer] Invalid Steam Id Or Username"); return; }
            if (amount < 0) { Misc.Msg("[Players] [RemoveCashFromPlayer] Invalid Amount"); return; }
            if (type == GetCurrencyType.SteamID)
            {
                bool found = false;
                if (_playersCurrency.ContainsKey(steamIdOrPlayerName))
                {
                    _playersCurrency[steamIdOrPlayerName] -= amount;
                }
                if (!found) { Misc.Msg("[Players] [RemoveCashFromPlayer] Player Not Found"); }
            }
            else if (type == GetCurrencyType.PlayerName)
            {
                bool found = false;
                foreach (var player in _players)
                {
                    if (player.Value == steamIdOrPlayerName)
                    {
                        _playersCurrency[player.Key] -= amount;
                    }
                }
                if (!found) { Misc.Msg("[Players] [RemoveCashFromPlayer] Player Not Found"); }
            }
            else { Misc.Msg("[Players] [RemoveCashFromPlayer] Invalid GetCurrencyType"); }
        }
    }
}
