using SimpleNetworkEvents;
using Steamworks;
using UnityEngine;


namespace Signs.Network
{
    internal class JoinedServer : SimpleEvent<JoinedServer>
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public override void OnReceived()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer)
            {
                Misc.Msg("[JoinedServer] Player Joined, Sending Get All Signs Event");
                if (Saving.Load.ModdedSigns.Count > 0)
                {
                    (ulong steamId, string stringSteamId) = Misc.MySteamId();
                    if (SenderId != stringSteamId)
                    {
                        Misc.Msg("[JoinedServer] Player Joined, Sending All Shops Sync Event");
                        foreach (var sign in Saving.Load.ModdedSigns)
                        {
                            Mono.SignController signController = sign.GetComponent<Mono.SignController>();
                            string uniqueId = signController.UniqueId.ToString();
                            string vector3Position = Network.CustomSerializable.Vector3ToString((Vector3)signController.GetPos());
                            string rotation = Network.CustomSerializable.QuaternionToString((Quaternion)signController.GetCurrentRotation());

                            if (!string.IsNullOrEmpty(uniqueId))
                            {
                                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SpawnSingeSign
                                {
                                    Vector3Position = vector3Position,
                                    QuaternionRotation = rotation,
                                    UniqueId = uniqueId,
                                    Sender = stringSteamId,
                                    SenderName = Misc.GetLocalPlayerUsername(),
                                    Line1Text = signController.GetLineText(1),
                                    Line2Text = signController.GetLineText(2),
                                    Line3Text = signController.GetLineText(3),
                                    Line4Text = signController.GetLineText(4),
                                    ToSteamId = "None"
                                });
    }
                            else
                            {
                                Misc.Msg("[JoinedServer] UniqueId is Null/Empty Witch It Shoud Never Be!");
                            }
                        }
                    }
                    else { Misc.Msg("[JoinedServer] Got Joined Server Event From MySelf, while being host, Skipped"); }
                }
            }
        }
    }
}
