using RedLoader;
using Sons.Multiplayer;
using Steamworks;
using TheForest.Utils;


namespace SimpleElevator
{
    internal class Misc
    {
        internal static void Msg(string msg, bool network = false)
        {
            if (!Config.LoggingToConsole.Value) { return; }
            RLog.Msg($"[SimpleElevator] {msg}");
        }
        internal static string GetLocalPlayerSteamId()
        {
            if (SonsSdk.Networking.NetUtils.IsDedicatedServer == true || Tools.DedicatedServer.IsDeticatedServer() == true) { return ""; }
            ulong? mySteamId = MultiplayerUtilities.GetSteamId(LocalPlayer.Entity);
            if (mySteamId == null || mySteamId == 0)
            {
                Steamworks.CSteamID SteamID = SteamUser.GetSteamID();
                string SteamIDString = SteamID.ToString();
                return SteamIDString;
            }
            return mySteamId.ToString();
        }

        public static (ulong, string) MySteamId()
        {
            if (BoltNetwork.isRunning && BoltNetwork.isServer && SonsSdk.Networking.NetUtils.IsDedicatedServer) { return (0, null); }
            ulong? mySteamId = MultiplayerUtilities.GetSteamId(LocalPlayer.Entity);
            if (mySteamId == null || mySteamId == 0)
            {
                Steamworks.CSteamID SteamID = SteamUser.GetSteamID();
                string SteamIDString = SteamID.ToString();
                ulong resultSteamID;
                if (ulong.TryParse(SteamIDString, out resultSteamID))
                {
                    return (resultSteamID, SteamIDString);
                }
                else { Misc.Msg("[MySteamId()] Failed To Get MySteamId! Returned null"); return (0, null); }
            }
            else
            {
                return ((ulong)mySteamId, mySteamId.ToString());
            }
        }

        public static string SteamId()
        {
            if (SonsSdk.Networking.NetUtils.IsDedicatedServer == true || Tools.DedicatedServer.IsDeticatedServer() == true) { return ""; }
            return MySteamId().Item2;
        }
    }
}
