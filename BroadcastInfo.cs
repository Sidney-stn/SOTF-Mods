using Bolt;
using Endnight.Extensions;
using TheForest.Utils;
using UnityEngine;


namespace BroadcastMessage
{
    internal class BroadcastInfo
    {

        internal static void CheckName()
        {
            IPlayerState state = LocalPlayer.Transform.GetComponent<BoltEntity>().GetState<IPlayerState>();
            if (state == null) { Misc.ErrorMsg("IPlayerState state is null! Unable to get the name"); return; }
            Misc.Msg($"Server Name: {state.name}");
        }

        internal static void SetName(string name)
        {
            if (name == null || name == "") { Misc.ErrorMsg("Unable To Set Server Name, since input string is null or empty"); return; }
            IPlayerState state = LocalPlayer.Transform.GetComponent<BoltEntity>().GetState<IPlayerState>();
            if (state == null) { Misc.ErrorMsg("IPlayerState state is null! Unable to get the name"); return; }
            state.name = name;
        }

        internal static string VerifyName(NetworkId evntsender)
        {
            var playerSetup = GameObject.FindObjectsOfType<BoltPlayerSetup>()
                                        .FirstOrDefault(bps => bps._entity._entity.NetworkId == evntsender);

            if (playerSetup != null)
            {
                string name = playerSetup.state.name;
                return string.IsNullOrEmpty(name) ? "UNKNOWN" : name;
            }

            return "UNKNOWN";
        }

        public static DiscordBotManager botManager;

        public static void SetAndActivateBotManager()
        {
            // Create the DiscordBotManager instance
            botManager = new DiscordBotManager();

            // Start the Discord bot
            botManager.StartBot();
        }

        public static void StopBot()
        {
            // Stop the Discord bot
            botManager.StopBot();
        }

        public static GameObject monoGameObject;
        internal static void InitilizeMonoBehavior()
        {
            monoGameObject = new GameObject();
            monoGameObject.AddComponent<BroadCastMono.BroadCastCheckTextFileMonoBehaviour>();
        }

        internal static void KillMonoBehavior()
        {
            monoGameObject.GetComponent<BroadCastMono.BroadCastCheckTextFileMonoBehaviour>().KillSelf();
        }

        internal static bool? isDedicatedFromBroadCastMessage
        {
            get { return BroadcastMessage.isDedicated; }
        }

    }
}
