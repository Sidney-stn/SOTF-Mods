using UnityEngine;
using RedLoader;
using System.Collections;
using Bolt;
using TheForest.Utils;
using TheForest.UI.Multiplayer;



namespace BroadcastMessage
{
    public class BroadCastMono
    {
        [RegisterTypeInIl2Cpp]
        internal class BroadCastCheckTextFileMonoBehaviour : MonoBehaviour
        {
            // Singleton instance
            private static BroadCastCheckTextFileMonoBehaviour instance;

            // Queue to hold messages to be sent
            private Queue<(string name, string text)> messageQueue = new Queue<(string name, string text)>();

            // Flag to check if the coroutine is running
            private bool isProcessingMessages = false;

            // Awake is called when the script instance is being loaded
            private void Awake()
            {
                // If there's already an instance, destroy the new one
                if (instance != null && instance != this)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
            }

            // Method to send chat messages
            internal static void SendChatMessage(string name = "[Discord] Unknown", string text = "Message")
            {
                if (!BoltNetwork.isRunning)
                {
                    Misc.Msg("BoltNetwork Is Not Running!");
                    return;
                }

                // Check if the provided name matches the last used name
                if (instance != null)
                {
                    // Add the message to the queue
                    instance.EnqueueMessage(name, text);
                }
            }

            // Method to enqueue messages and start the processing coroutine if not running
            private void EnqueueMessage(string name, string text)
            {
                messageQueue.Enqueue((name, text));
                if (!isProcessingMessages)
                {
                    ProcessMessageQueue().RunCoro();
                }
            }

            // Coroutine to process the message queue
            private IEnumerator ProcessMessageQueue()
            {
                isProcessingMessages = true;

                while (messageQueue.Count > 0)
                {
                    var (name, text) = messageQueue.Dequeue();
                    BroadcastInfo.SetName(name);

                    // Delay before sending each message
                    yield return new WaitForSeconds(5f);

                    // Send the message
                    SendMessageNow(name, text);
                }

                isProcessingMessages = false;
            }

            // Method to send the chat message immediately
            private static void SendMessageNow(string name, string text)
            {
                ChatEvent chatEvent = ChatEvent.Create(GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
                chatEvent.Message = text;
                chatEvent.Sender = LocalPlayer.Transform.GetComponent<BoltEntity>().networkId;

                if (Config.PrintSentChatEvent.Value)
                {
                    Misc.Msg($"ChatEvent To String: {chatEvent.ToString()}");
                }

                chatEvent.Send();
                if (BroadcastInfo.isDedicatedFromBroadCastMessage == false)  // Fixes So MulitplayerHost Also Get Messages
                {

                    HudGui.Instance.Chatbox.RegisterPlayer($"{name}", LocalPlayer.Entity.networkId, Color.cyan);
                    ChatEvent newChatEvent = ChatEvent.Create(GlobalTargets.OnlySelf, ReliabilityModes.ReliableOrdered);
                    newChatEvent.Message = text;
                    newChatEvent.Sender = LocalPlayer.Transform.GetComponent<BoltEntity>().networkId;
                    newChatEvent.Send();
                    WaitToChangeNameToYou().RunCoro();
                }
                
            }

            private static IEnumerator WaitToChangeNameToYou()
            {
                yield return new WaitForSeconds(1f);
                HudGui.Instance.Chatbox.RegisterPlayer($"You", LocalPlayer.Entity.networkId, Color.cyan);
            }

            private void Start()
            {
                // Start the coroutine for checking received messages
                Misc.Msg("Start() BroadCastCheckTextFileMonoBehaviour");
                CheckIfReceivedMessages().RunCoro();
            }

            // Coroutine to periodically check for received messages
            private IEnumerator CheckIfReceivedMessages()
            {
                while (true) // This creates an infinite loop
                {
                    BroadcastInfo.botManager.CheckForResponses();
                    // Wait for 10 seconds
                    yield return new WaitForSeconds(10f);
                }
            }

            internal void KillSelf()
            {
                Destroy(instance);
                Destroy(this.gameObject);
            }
        }
    }
}
