using RedLoader;
using Sons.Multiplayer;
using Steamworks;
using TheForest.Utils;


namespace StoneGate
{
    internal class Misc
    {
        internal static void Msg(string msg, bool network = false)
        {
            if (!Config.LoggingToConsole.Value) { return; }
            RLog.Msg($"[StoneGate] {msg}");
        }
        internal static string GetLocalPlayerSteamId()
        {
            ulong? mySteamId = MultiplayerUtilities.GetSteamId(LocalPlayer.Entity);
            if (mySteamId == null || mySteamId == 0)
            {
                Steamworks.CSteamID SteamID = SteamUser.GetSteamID();
                string SteamIDString = SteamID.ToString();
                return SteamIDString;
            }
            return mySteamId.ToString();
        }
    }
}
