
using Sons.Multiplayer.Gui;

namespace Currency.LiveData
{
    public class Players
    {
        // These will only include players that are currently in the game.
        private static Dictionary<string, string> _players = new Dictionary<string, string>();  // PlayerSteamID, PlayerName
        private static Dictionary<string, int> _playersCurrency = new Dictionary<string, int>();  // PlayerSteamID, PlayerCash

        public static void AddPlayer(string steamID, string playerName, int cash)  // Adds Or Updates Player
        {
            if (!_players.ContainsKey(steamID))
            {
                _players.Add(steamID, playerName);
                _playersCurrency.Add(steamID, cash);
            }
            else
            {
                _players[steamID] = playerName;
                _playersCurrency[steamID] = cash;
            }
        }

        public static void RemovePlayer(string steamID)
        {
            if (_players.ContainsKey(steamID))
            {
                _players.Remove(steamID);
                _playersCurrency.Remove(steamID);
            }
        }

        public static Dictionary<string, string> GetPlayers()
        {
            return _players;
        }

        public static Dictionary<string, int> GetPlayersCurrency()
        {
            return _playersCurrency;
        }

        public enum GetCurrencyType
        {
            SteamID,
            PlayerName
        }

        public static int? GetPlayerCurrency(GetCurrencyType type, string steamIdOrPlayerName)
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
    }
}
