using UnityEngine;
using RedLoader;
using System.Collections;



namespace BroadcastMessage
{
    public class BroadCastMono
    {
        [RegisterTypeInIl2Cpp]
        internal class BroadCastCheckTextFileMonoBehaviour : MonoBehaviour
        {
            private void Start()
            {
                // Start the coroutine
                //StartCoroutine(LoopEveryFiveSeconds().WrapToIl2Cpp());
                Misc.Msg("Start() BroadCastMonoBehaviour");
                CheckIfRecivedMessages().RunCoro();
            }

            IEnumerator CheckIfRecivedMessages()
            {
                while (true) // This creates an infinite loop
                {
                    // Perform your action here
                    Misc.Msg("Action performed at " + Time.time);
                    BroadcastInfo.SendChatMessage();

                    // Wait for 5 seconds
                    yield return new WaitForSeconds(10f);
                }
            }
        }


        [RegisterTypeInIl2Cpp]
        internal class BroadCastMonoBehaviour : MonoBehaviour
        {
            private void Start()
            {
                // Start the coroutine
                //StartCoroutine(LoopEveryFiveSeconds().WrapToIl2Cpp());
                Misc.Msg("Start() BroadCastMonoBehaviour");
                LoopEveryFiveSeconds().RunCoro();
            }

            IEnumerator LoopEveryFiveSeconds()
            {
                while (true) // This creates an infinite loop
                {
                    // Perform your action here
                    Misc.Msg("Action performed at " + Time.time);
                    BroadcastInfo.SendChatMessage();

                    // Wait for 5 seconds
                    yield return new WaitForSeconds(5f);
                }
            }
        }
    }
}
