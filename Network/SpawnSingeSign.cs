using Endnight.Types;
using SimpleNetworkEvents;
using UnityEngine;

namespace Signs.Network
{
    internal class SpawnSingeSign : SimpleEvent<SpawnSingeSign>
    {
        public string Vector3Position { get; set; }
        public string QuaternionRotation { get; set; }
        public string UniqueId { get; set; }
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string Line1Text { get; set; }
        public string Line2Text { get; set; }
        public string Line3Text { get; set; }
        public string Line4Text { get; set; }
        public string ToSteamId { get; set; }


        public override void OnReceived()
        {
            Misc.Msg("Recived Network SpawnSign Event");
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
            {
                Misc.Msg("[SpawnSign] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                return;
            }
            if (Assets.SignObj == null)
            {
                Misc.Msg("[SpawnSign] [OnReceived()] Sign Prefab has not been created yet, skipped");
                return;
            }
            (ulong uSteamId, string sSteamId) = Misc.MySteamId();
            if (string.IsNullOrEmpty(sSteamId))
            {
                Misc.Msg("[SpawnSign] [OnReceived()] Could Not Get My SteamId, Cant Be Sure Who Has The Sign, Skipped");
                return;
            }
            else
            {
                if (Sender == sSteamId)
                {
                    Misc.Msg("[SpawnSign] [OnReceived()] Not Creating Sign Over Network When Its From My SteamID, skipped");
                    return;
                }
            }
            if (ToSteamId != null && ToSteamId != "None")
            {
                if (ToSteamId != sSteamId.ToString())
                {
                    Misc.Msg("[SpawnSign] [OnReceived()] Not Creating Sign Over Network When Its Not For Me, skipped");
                    return;
                }

            }

            if (Config.NetworkDebugIngameSign.Value) { Misc.Msg($"[SpawnSign] [OnReceived()] Spawning Prefab From Network Event"); }

            ulong resultSteamID;
            if (!ulong.TryParse(Sender, out resultSteamID)) { Misc.Msg($"Failed To Parse SenderId: {Sender} To String"); return; }
            Misc.Msg($"[SpawnSign] [OnReceived()] Spawn Sign To STRING Pos: {Vector3Position}, STRING Rot: {QuaternionRotation}");
            Vector3 pos = Network.CustomSerializable.Vector3FromString(Vector3Position);
            Quaternion rot = Network.CustomSerializable.QuaternionFromString(QuaternionRotation);
            Misc.Msg($"[SpawnSign] [OnReceived()] Spawn Sign To Pos: {pos}, Rot: {rot}");

            Prefab.SignPrefab.spawnSignMultiplayer(pos, rot, Line1Text, Line2Text, Line3Text, Line4Text, UniqueId);

        }
    }
}
