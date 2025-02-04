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
                Misc.Msg($"[Joined Server] [OnReceived] Amount Of ATMS In World: {Saving.Load.ModdedAtms.Count}");  // Logging
                int index = 0;
                foreach (var atm in Saving.Load.ModdedAtms)
                {
                    Mono.ATMController atmController = atm.GetComponent<Mono.ATMController>();
                    if (atmController == null) { Misc.Msg("[JoinedServer] [OnReceived] ATMController Is Null When Trying To Send SpawnATM Event To Joined Player"); continue; }

                    string uniqueId = atmController.UniqueId;
                    if (string.IsNullOrEmpty(uniqueId)) { Misc.Msg("[JoinedServer] [OnReceived] UniqueId Is Null Or Empty When Trying To Send SpawnATM Event To Joined Player"); continue; }
                    string vector3Position = Network.CustomSerializable.Vector3ToString((Vector3)atmController.GetPos());
                    if (string.IsNullOrEmpty(vector3Position)) { Misc.Msg("[JoinedServer] [OnReceived] Vector3Position Is Null Or Empty When Trying To Send SpawnATM Event To Joined Player"); continue; }
                    string rotation = Network.CustomSerializable.QuaternionToString((Quaternion)atmController.GetCurrentRotation());
                    if (string.IsNullOrEmpty(rotation)) { Misc.Msg("[JoinedServer] [OnReceived] Rotation Is Null Or Empty When Trying To Send SpawnATM Event To Joined Player"); continue; }

                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SpawnATM
                    {
                        Vector3Position = vector3Position,
                        QuaternionRotation = rotation,
                        UniqueId = uniqueId,
                        Sender = Misc.MySteamId().Item2,
                        SenderName = Misc.GetLocalPlayerUsername(),
                        ToSteamId = SenderId
                    });
                    index++;
                }
                Misc.Msg($"[JoinedServer] [OnReceived] Sent {index} ATM Spawn Commands");

                // Trigger Event
                API.SubscribableEvents.TriggerOnPlayerJoin();
            }
        }
    }
}
