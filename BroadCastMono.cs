using UnityEngine;
using RedLoader;
using System.Collections;
using Bolt;
using TheForest.Utils;



namespace BroadcastMessage
{
    public class BroadCastMono
    {
        [RegisterTypeInIl2Cpp]
        internal class BroadCastCheckTextFileMonoBehaviour : MonoBehaviour
        {
            // Static variable to keep track of the last used name
            private static string lastUsedName = "[Discord] Unknown";

            // Singleton instance
            private static BroadCastCheckTextFileMonoBehaviour instance;

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
                BroadcastInfo.SetName(name);
                if (name == lastUsedName)
                {
                    // If the name is the same, send the message instantly
                    SendMessageNow(text);
                }
                else
                {
                    // If the name is different, start a coroutine to delay the message
                    // Note: Coroutines can only be started from a MonoBehaviour instance
                    if (instance != null)
                    {
                        instance.SendDelayedMessage(name, text).RunCoro();
                    }
                        
                }

                // Update the last used name
                lastUsedName = name;
            }

            // Coroutine to send the message after a delay
            private IEnumerator SendDelayedMessage(string name, string text)
            {
                // Wait for 5 seconds
                yield return new WaitForSeconds(5);

                // Send the message after the delay
                SendMessageNow(text);
            }

            // Method to send the chat message immediately
            private static void SendMessageNow(string text)
            {
                ChatEvent chatEvent = ChatEvent.Create(GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);  //AllClients Works Perfect On Dedicated Server, On Multiplayer Host Do Not Get Any Messages
                chatEvent.Message = text;
                chatEvent.Sender = LocalPlayer.Transform.GetComponent<BoltEntity>().networkId;

                if (Config.PrintSentChatEvent.Value)
                {
                    Misc.Msg($"ChatEvent To String: {chatEvent.ToString()}");
                }

                chatEvent.Send();
            }



            private void Start()
            {
                // Start the coroutine
                Misc.Msg("Start() BroadCastCheckTextFileMonoBehaviour");
                CheckIfRecivedMessages().RunCoro();
            }

            internal void KillSelf()
            {
                Destroy(instance);
                Destroy(this.gameObject);
            }

            IEnumerator CheckIfRecivedMessages()
            {
                while (true) // This creates an infinite loop
                {
                    BroadcastInfo.botManager.CheckForResponses();
                    // Wait for 10 seconds
                    yield return new WaitForSeconds(10f);
                }
            }
        }
    }
}
