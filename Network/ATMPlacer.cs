using SimpleNetworkEvents;
using UnityEngine;

namespace Banking.Network
{
    internal class ATMPlacer
    {
        internal class SpawnATMPlacer : SimpleEvent<SpawnATMPlacer>
        {
            public string Vector3Position { get; set; }
            public string QuaternionRotation { get; set; }
            public string UniqueId { get; set; }
            public string Sender { get; set; }
            public string SenderName { get; set; }
            public string ToSteamId { get; set; }

            public override void OnReceived()
            {
                Misc.Msg("Recived Network SpawnATMPlacer Event");
                if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
                {
                    Misc.Msg("[SpawnATMPlacer] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                    return;
                }
                if (Assets.ATM == null)
                {
                    Misc.Msg("[SpawnATMPlacer] [OnReceived()] Atm Prefab has not been created yet, skipped");
                    return;
                }
                (ulong uSteamId, string sSteamId) = Misc.MySteamId();
                if (string.IsNullOrEmpty(sSteamId))
                {
                    Misc.Msg("[SpawnATMPlacer] [OnReceived()] Could Not Get My SteamId, Cant Be Sure Who Has The Atm, Skipped");
                    return;
                }
                else
                {
                    if (Sender == sSteamId)
                    {
                        Misc.Msg("[SpawnATMPlacer] [OnReceived()] Not Creating Atm Over Network When Its From My SteamID, skipped");
                        return;
                    }
                }
                if (ToSteamId != null && ToSteamId != "None")
                {
                    if (ToSteamId != sSteamId.ToString())
                    {
                        Misc.Msg("[SpawnATMPlacer] [OnReceived()] Not Creating Atm Over Network When Its Not For Me, skipped");
                        return;
                    }

                }

                if (Config.NetworkDebugIngameBanking.Value) { Misc.Msg($"[SpawnATMPlacer] [OnReceived()] Spawning Prefab From Network Event"); }

                ulong resultSteamID;
                if (!ulong.TryParse(Sender, out resultSteamID)) { Misc.Msg($"Failed To Parse SenderId: {Sender} To String"); return; }
                Misc.Msg($"[SpawnATMPlacer] [OnReceived()] Spawn Sign To STRING Pos: {Vector3Position}, STRING Rot: {QuaternionRotation}");
                Vector3 pos = Network.CustomSerializable.Vector3FromString(Vector3Position);
                Quaternion rot = Network.CustomSerializable.QuaternionFromString(QuaternionRotation);
                Misc.Msg($"[SpawnATMPlacer] [OnReceived()] Spawn Sign To Pos: {pos}, Rot: {rot}");

                Prefab.ATMPlacer.PlacePrefab(pos, rot, false, UniqueId, true);

            }
        }

        internal class RemoveATMPlacer : SimpleEvent<RemoveATMPlacer>
        {
            public string UniqueId { get; set; }

            public override void OnReceived()
            {
                Misc.Msg("Recived Network Remove ATMPlacer Event");
                if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
                {
                    Misc.Msg("[RemoveATMPlacer] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                    return;
                }
                if (Misc.hostMode == Misc.SimpleSaveGameType.NotIngame)
                {
                    Misc.Msg("[RemoveATMPlacer] [OnReceived()] Skipped Reciving Network Event Since Not Ingame");
                    return;
                }

                if (Config.NetworkDebugIngameBanking.Value) { Misc.Msg($"[RemoveATMPlacer] [OnReceived()] Removing Prefab From Network Event"); }

                if (Prefab.ActiveATM.DoesShopWithUniqueIdExist(UniqueId))
                {
                    GameObject atm = Prefab.ATMPlacer.FindByUniqueId(UniqueId);
                    if (atm != null)
                    {
                        Prefab.ActiveATM.spawnedAtms.Remove(UniqueId);
                        Saving.Load.ModdedAtms.Remove(atm);
                        UnityEngine.Object.Destroy(atm);
                    }
                    else
                    {
                        Misc.Msg($"[RemoveATMPlacer] [OnReceived()] ATMPlacer With UniqueId: {UniqueId} Does Not Exist, Cant Remove");
                    }
                }
                else
                {
                    Misc.Msg($"[RemoveATMPlacer] [OnReceived()] ATMPlacer With UniqueId: {UniqueId} Does Not Exist, Cant Remove");
                }

            }
        }

        internal class UpdateATMPlacer : SimpleEvent<UpdateATMPlacer>
        {
            public string UniqueId { get; set; }
            public Dictionary<int, int> RecivedAddedItems { get; set; }
            public float RemainingTime { get; set; }
            public string Sender { get; set; }
            public string SenderName { get; set; }
            public string ToSteamId { get; set; }

            public override void OnReceived()
            {
                Misc.Msg("Recived Network UpdateATMPlacer Event");
                if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
                {
                    Misc.Msg("[UpdateATMPlacer] [OnReceived()] Skipped Reciving Network Event On SinglePlayer");
                    return;
                }
                if (Assets.ATM == null)
                {
                    Misc.Msg("[UpdateATMPlacer] [OnReceived()] Atm Prefab has not been created yet, skipped");
                    return;
                }
                (ulong uSteamId, string sSteamId) = Misc.MySteamId();
                if (string.IsNullOrEmpty(sSteamId))
                {
                    Misc.Msg("[UpdateATMPlacer] [OnReceived()] Could Not Get My SteamId, Cant Be Sure Who Has The Atm, Skipped");
                    return;
                }
                else
                {
                    if (Sender == sSteamId)
                    {
                        Misc.Msg("[UpdateATMPlacer] [OnReceived()] Not Creating Atm Over Network When Its From My SteamID, skipped");
                        return;
                    }
                }
                if (ToSteamId != null && ToSteamId != "None")
                {
                    if (ToSteamId != sSteamId.ToString())
                    {
                        Misc.Msg("[UpdateATMPlacer] [OnReceived()] Not Creating Atm Over Network When Its Not For Me, skipped");
                        return;
                    }

                }

                if (Config.NetworkDebugIngameBanking.Value) { Misc.Msg($"[UpdateATMPlacer] [OnReceived()] Spawning Prefab From Network Event"); }

                ulong resultSteamID;
                if (!ulong.TryParse(Sender, out resultSteamID)) { Misc.Msg($"Failed To Parse SenderId: {Sender} To String"); return; }

                if (RecivedAddedItems == null) { Misc.Msg("[UpdateATMPlacer] [OnReceived()] RecivedAddedItems Is Null, Cant Update ATMPlacer"); return; }

                GameObject atm = Prefab.ATMPlacer.FindByUniqueId(UniqueId);
                if (atm != null)
                {
                    Mono.ATMPlacerController atmController = atm.GetComponent<Mono.ATMPlacerController>();
                    if (atmController == null) { Misc.Msg("[UpdateATMPlacer] [OnReceived] ATMPlacerController Is Null When Trying To UpdateATMPlacer Event"); return; }
                    // Update Items From Network
                    atmController.SetAddedObjects(RecivedAddedItems);

                    // Update Remaining Time
                    atmController.StartNetworkCountdown(RemainingTime);
                }
                else
                {
                    Misc.Msg($"[UpdateATMPlacer] [OnReceived()] ATMPlacer With UniqueId: {UniqueId} Does Not Exist, Cant Update");
                }
            }
        }
    }
}
