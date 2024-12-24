using SimpleNetworkEvents;
using Steamworks;
using UnityEngine;


namespace Warps.Network
{
    internal class JoinedServer : SimpleEvent<JoinedServer>
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public override void OnReceived()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer)
            {
                Misc.Msg("[JoinedServer] Player Joined, Sending Get All Warps Event");
                if (Saving.Load.Warps.Count > 0)
                {
                    (ulong steamId, string stringSteamId) = Misc.MySteamId();
                    if (SenderId != stringSteamId)
                    {
                        Misc.Msg("[JoinedServer] Player Joined, Sending All Warps Sync Event");
                        foreach (var sign in Saving.LoadedWarps.loadedWarps)
                        {
                            if (sign.Key == null) { continue; }
                            if (sign.Value == Vector3.zero) { continue; }

                            SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.AddWarp
                            {
                                WarpName = sign.Key,
                                Vector3String = Network.CustomSerializable.Vector3ToString(sign.Value),
                                Sender = stringSteamId,
                                SenderName = Misc.GetLocalPlayerUsername(),
                                ToSteamId = SenderId
                            });
                        }
                    }
                    else { Misc.Msg("[JoinedServer] Got Joined Server Event From MySelf, while being host, Skipped"); }
                }
            }
        }
    }
}
