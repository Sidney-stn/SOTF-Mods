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

        internal static void SendChatMessage(string name = "[Discord] Unkown", string text = "Message")
        {
            if (!BoltNetwork.isRunning)
            {
                Misc.Msg("BoltNetwork Is Not Running!");
                return;
            }
            SetName(name);
            if (Config.CheckNamePrinting.Value) { CheckName(); }

            ChatEvent chatEvent = ChatEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            chatEvent.Message = text;
            chatEvent.Sender = LocalPlayer.Transform.GetComponent<BoltEntity>().networkId;
            Misc.Msg($"ChatEvent To String: {chatEvent.ToString()}");
            chatEvent.Send();
        }

        internal static void CheckName()
        {
            IPlayerState state = LocalPlayer.Transform.GetComponent<BoltEntity>().GetState<IPlayerState>();
            if (state == null) { Misc.ErrorMsg("IPlayerState state is null! Unable to get the name"); return; }
            Misc.Msg($"Server Name: {state.name}");
        }

        internal static void SetName(string name)
        {
            if (name == null) { Misc.ErrorMsg("Unable To Set Server Name, since input string is null"); return; }
            IPlayerState state = LocalPlayer.Transform.GetComponent<BoltEntity>().GetState<IPlayerState>();
            Misc.Msg($"Server Name: {state.name}");
            if ( state == null ) { Misc.ErrorMsg("IPlayerState state is null! Unable to get the name"); return; }
            state.name = name;
        }

        internal static void GenerateObjectWithMono()
        {
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<BroadCastMono.BroadCastMonoBehaviour>();
        }

    }
}
