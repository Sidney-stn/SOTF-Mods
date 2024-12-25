using SimpleNetworkEvents;
using UnityEngine;

namespace Banking.Network
{
    internal class JoinedServer : SimpleEvent<JoinedServer>
    {
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public override void OnReceived()
        {
            if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer)
            {
                if (SenderId == Misc.MySteamId().Item2) { Misc.Msg("[JoinedServer] [OnReceived] Recived Joined Server Event From My Self. As Host, Skipping"); return; } // Skip If Host (Myself
                Misc.Msg($"[JoinedServer] Player Joined: {SenderId} - {SenderName}");
                LiveData.Players.AddPlayer(SenderId, SenderName);
                
                Misc.Msg($"[JoinedServer] Sending New Sync Event To JoinedPlayer - FROM HOST");

                // Player Joined, Send Sync Cash
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SyncCash
                {
                    SenderName = Misc.GetLocalPlayerUsername(),
                    SenderId = Misc.MySteamId().Item2,
                    PlayerName = LiveData.Players.GetPlayers(),
                    PlayerCash = LiveData.Players.GetPlayersCurrency(),
                    ToPlayerId = SenderId
                });

                // Player Joined, Sending ATM Spawn CMD
                foreach (var sign in Saving.Load.ModdedAtms)
                {
                    Mono.ATMController atmController = sign.GetComponent<Mono.ATMController>();
                    if (atmController == null) { continue; }

                    string uniqueId = atmController.UniqueId.ToString();
                    string vector3Position = Network.CustomSerializable.Vector3ToString((Vector3)atmController.GetPos());
                    string rotation = Network.CustomSerializable.QuaternionToString((Quaternion)atmController.GetCurrentRotation());

                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SpawnATM
                    {
                        Vector3Position = vector3Position,
                        QuaternionRotation = rotation,
                        UniqueId = uniqueId,
                        Sender = Misc.MySteamId().Item2,
                        SenderName = Misc.GetLocalPlayerUsername(),
                        ToSteamId = SenderId
                    });
                }
                    
            }

            // Trigger Event
            API.SubscribableEvents.TriggerOnPlayerJoin();
        }
    }
}
