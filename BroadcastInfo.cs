using Bolt;
using TheForest.Utils;
using UnityEngine;


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
            if (Config.PrintSentChatEvent.Value) { Misc.Msg($"ChatEvent To String: {chatEvent.ToString()}"); }
            chatEvent.Send();
        }

        internal static void CheckName()
        {
            IPlayerState state = LocalPlayer.Transform.GetComponent<BoltEntity>().GetState<IPlayerState>();
            if (state == null) { Misc.ErrorMsg("IPlayerState state is null! Unable to get the name"); return; }
            Misc.Msg($"Server Name: {state.name}");
        }

        private static void SetName(string name)
        {
            if (name == null || name == "") { Misc.ErrorMsg("Unable To Set Server Name, since input string is null or empty"); return; }
            IPlayerState state = LocalPlayer.Transform.GetComponent<BoltEntity>().GetState<IPlayerState>();
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
