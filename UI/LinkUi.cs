using Sons.Gui;
using Sons.Gui.Input;
using TheForest.Utils;
using UnityEngine;
using WirelessSignals.Mono;

namespace WirelessSignals.UI
{
    internal class LinkUi
    {
        internal static LinkUiElement CreateLinkUi(GameObject toAddLinkUiOn, float maxDistance, Texture? texture, Texture2D? texture2D, Vector3? worldSpaceOffset, string elementId = "screen.take")
        {
            Vector3 _worldOffset = worldSpaceOffset ?? new Vector3(0, (float)0.2, 0);
            LinkUiElement linkUiAdd = toAddLinkUiOn.AddComponent<LinkUiElement>();
            linkUiAdd._applyMaterial = false;
            linkUiAdd._applyText = false;
            linkUiAdd._applyTexture = true;
            if (texture != null)
            {
                linkUiAdd._texture = texture;
            }
            else if (texture2D != null)
            {
                linkUiAdd._texture = texture2D;
            }
            linkUiAdd._maxDistance = maxDistance;
            linkUiAdd._worldSpaceOffset = _worldOffset;
            linkUiAdd._uiElementId = elementId; // "screen.take", "screen.use", "screen.takeAndUse", "PickUps"
            linkUiAdd.enabled = false;
            linkUiAdd.enabled = true;
            return linkUiAdd;
        }

        internal static void TryInteractWithUi()
        {
            if (!LocalPlayer.IsInWorld || LocalPlayer.IsInInventory || PauseMenu.IsActive) { return; }
            Transform transform = LocalPlayer._instance._mainCam.transform;
            RaycastHit raycastHit;
            Physics.Raycast(transform.position, transform.forward, out raycastHit, 5f, LayerMask.GetMask(new string[]
            {
                "Default"
            }));
            if (raycastHit.collider == null) { return; }
            if (raycastHit.collider.transform.root == null) { return; }
            if (string.IsNullOrEmpty(raycastHit.collider.transform.root.name)) { return; }
            //Misc.Msg($"Hit: {raycastHit.collider.transform.root.name}");
            if (raycastHit.collider.transform.root.name.Contains("TransmitterSwitch"))
            {
                GameObject open = raycastHit.collider.transform.root.gameObject;
                Mono.TransmitterSwitch controller = open.GetComponent<TransmitterSwitch>();
                if (controller != null)
                {
                    controller.Toggle();
                }
                else { Misc.Msg("Controller is null!"); }

            }
        }
    }
}
