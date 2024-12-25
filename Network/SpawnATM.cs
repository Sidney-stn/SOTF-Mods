using SimpleNetworkEvents;
using UnityEngine;

namespace Banking.Network
{
    internal class SpawnATM : SimpleEvent<SpawnATM>
    {
        public string Vector3Position { get; set; }
        public string QuaternionRotation { get; set; }
        public string UniqueId { get; set; }
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string ToSteamId { get; set; }

        public override void OnReceived()
        {
            Misc.Msg("Recived Network SpawnATM Event");
            if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
            {
                Misc.Msg("[SpawnATM] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                return;
            }
            if (Assets.ATM == null)
            {
                Misc.Msg("[SpawnATM] [OnReceived()] Sign Prefab has not been created yet, skipped");
                return;
            }
            (ulong uSteamId, string sSteamId) = Misc.MySteamId();
            if (string.IsNullOrEmpty(sSteamId))
            {
                Misc.Msg("[SpawnATM] [OnReceived()] Could Not Get My SteamId, Cant Be Sure Who Has The Sign, Skipped");
                return;
            }
            else
            {
                if (Sender == sSteamId)
                {
                    Misc.Msg("[SpawnATM] [OnReceived()] Not Creating Sign Over Network When Its From My SteamID, skipped");
                    return;
                }
            }
            if (ToSteamId != null && ToSteamId != "None")
            {
                if (ToSteamId != sSteamId.ToString())
                {
                    Misc.Msg("[SpawnATM] [OnReceived()] Not Creating Sign Over Network When Its Not For Me, skipped");
                    return;
                }

            }

            if (Config.NetworkDebugIngameBanking.Value) { Misc.Msg($"[SpawnATM] [OnReceived()] Spawning Prefab From Network Event"); }

            ulong resultSteamID;
            if (!ulong.TryParse(Sender, out resultSteamID)) { Misc.Msg($"Failed To Parse SenderId: {Sender} To String"); return; }
            Misc.Msg($"[SpawnATM] [OnReceived()] Spawn Sign To STRING Pos: {Vector3Position}, STRING Rot: {QuaternionRotation}");
            Vector3 pos = Network.CustomSerializable.Vector3FromString(Vector3Position);
            Quaternion rot = Network.CustomSerializable.QuaternionFromString(QuaternionRotation);
            Misc.Msg($"[SpawnATM] [OnReceived()] Spawn Sign To Pos: {pos}, Rot: {rot}");

            Prefab.ActiveATM.SpawnATM(pos, rot, UniqueId);

        }
    }
}