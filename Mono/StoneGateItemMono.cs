

using Il2CppInterop.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using static Sons.Input.InputSystem;

namespace StoneGate.Mono
{
    internal class StoneGateItemMono : MonoBehaviour
    {
        private void Start()
        {
            Misc.Msg($"[StoneGateItemMono] [Start]");

            // If Sons.Input.InputSystem.Action is an enum, you might need to use something like:
            Sons.Input.InputSystem.Action inputActionEnum = Sons.Input.InputSystem.Actions.Find(ActionId.PrimaryAction);

            // Convert your delegates using Il2CppInterop
            var startedCallback = DelegateSupport.ConvertDelegate<Il2CppSystem.Action<UnityEngine.InputSystem.InputAction.CallbackContext>>(OnActionStarted);
            var performedCallback = DelegateSupport.ConvertDelegate<Il2CppSystem.Action<UnityEngine.InputSystem.InputAction.CallbackContext>>(OnActionPerformed);
            var cancelledCallback = DelegateSupport.ConvertDelegate<Il2CppSystem.Action<UnityEngine.InputSystem.InputAction.CallbackContext>>(OnActionCancelled);
            var alreadyStartedCallback = DelegateSupport.ConvertDelegate<Il2CppSystem.Action>(OnActionAlreadyStarted);

            // Now register with the proper types
            Sons.Input.InputSystem.RegisterActionUpdate(
                inputActionEnum, // Use the enum value
                startedCallback,
                performedCallback,
                cancelledCallback,
                alreadyStartedCallback
            );
        }

        private void OnDisable()
        {
            Misc.Msg($"[StoneGateItemMono] [OnDisable]");
        }

        private void OnActionStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Misc.Msg($"[StoneGateItemMono] Action started");
            // Your code here
        }

        private void OnActionPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Misc.Msg($"[StoneGateItemMono] Action performed");
            // Your code here
        }

        private void OnActionCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Misc.Msg($"[StoneGateItemMono] Action cancelled");
            // Your code here
        }

        private void OnActionAlreadyStarted()
        {
            Misc.Msg($"[StoneGateItemMono] Action was already started");
            // Your code here
        }
    }
}
