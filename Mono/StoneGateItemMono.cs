using System.Collections;
using UnityEngine;

namespace StoneGate.Mono
{
    internal class StoneGateItemMono : MonoBehaviour
    {
        private bool isAnimRunning = false;
        private float animTime = 0.3f;
        private Animator animController;

        private void Start()
        {
            Misc.Msg($"[StoneGateItemMono] [Start]");
            Objects.ActiveItem.active = this;

            animController = GetComponent<Animator>();
        }

        private void OnDisable()
        {
            Misc.Msg($"[StoneGateItemMono] [OnDisable]");
            Objects.ActiveItem.active = null;
        }

        public void InitHit()
        {
            if (isAnimRunning) { return; }
            Misc.Msg($"[StoneGateItemMono] [InitHit]");
            StartAnim();
        }

        private IEnumerator StartAnim()
        {
            isAnimRunning = true;
            animController.SetTrigger("Hit");
            yield return new WaitForSeconds(animTime);
            isAnimRunning = false;
        }

    }
}
