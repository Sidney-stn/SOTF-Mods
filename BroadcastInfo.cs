using Bolt;
using TheForest.Utils;
using UnityEngine;
using Sons.Multiplayer;
using Endnight.Types;
using Steamworks;

namespace BroadcastMessage
{
    internal class BroadcastInfo
    {

        internal static void SendChatMessage(string name = "[Discord] rnd", string text = "Test")
        {
            if (!BoltNetwork.isRunning)
            {
                Misc.Msg("BoltNetwork Is Not Running!");
                return;
            }

            FixPlayerName((ulong)CoopSteamServer.SteamId);

            ChatEvent chatEvent = ChatEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            chatEvent.Message = text;
            chatEvent.Sender = LocalPlayer.Transform.GetComponent<BoltEntity>().networkId;
            Misc.Msg($"ChatEvent To String: {chatEvent.ToString()}");
            chatEvent.Send();
        }

        internal static void FixPlayerName(ulong steamID)
        {
            try
            {
                if (GameServerManager.IsDedicatedServer)
                {
                    SingletonBehaviour<MultiplayerPlayerRoles>.TryGetInstance(out MultiplayerPlayerRoles multiplayerPlayerRoles);
                    string text;
                    GameServerManager.GetRegisteredClientName(new CSteamID(steamID), out text);
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        text = steamID.ToString();
                    }
                    Misc.Msg($"Dedicated Server Name: {text}, steamID: {steamID}");
                    //multiplayerPlayerRoles.UpdatePlayerName(steamID, text);
                    multiplayerPlayerRoles.UpdatePlayerName(steamID, "Mjau");
                }
            }
            catch (Exception ex) { Misc.Msg(ex.Message); }
        }

        internal static void GenerateObjectWithMono()
        {
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<BroadCastMono.BroadCastMonoBehaviour>();
        }

    }
}
