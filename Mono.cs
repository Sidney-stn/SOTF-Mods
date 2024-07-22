using RedLoader;
using System.Collections;
using UnityEngine;

namespace StructureDamageViewer
{
    [RegisterTypeInIl2Cpp]
    public class DamageMono : MonoBehaviour
    {
        private void Start ()
        {
            // Start the coroutine
            LoopEveryFiveSeconds().RunCoro();
        }
        private void Update ()
        {

        }

        IEnumerator LoopEveryFiveSeconds()
        {
            while (true) // This creates an infinite loop
            {
                // Perform your action here
                Misc.Msg("Action performed at " + Time.time);


                // Wait for 5 seconds
                yield return new WaitForSeconds(5f);
            }
        }
    }
}
