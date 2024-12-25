
namespace Banking.LiveData
{
    internal class Host
    {
        internal static void AddHostPlayerToSystem()
        {
            if (Misc.hostMode != Misc.SimpleSaveGameType.Multiplayer) { return; }
            int? cash = LocalPlayerData.GetLocalPlayerCurrency();
            if (cash == null)
            {
                Misc.Msg("Local player cash is null");
                return;
            }
            LiveData.Players.AddPlayer(Misc.MySteamId().Item2, Misc.GetLocalPlayerUsername());
        }
    }
}
