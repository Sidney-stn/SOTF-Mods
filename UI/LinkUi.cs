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
            float range = 5f; // How far to check
            float squareSize = 1f; // Size of the square to check
            int gridDensity = 3; // Number of rays per side (3x3 grid)

            for (int x = 0; x < gridDensity; x++)
            {
                for (int y = 0; y < gridDensity; y++)
                {
                    // Calculate offset from center
                    float xOffset = (x - (gridDensity - 1) / 2f) * (squareSize / (gridDensity - 1));
                    float yOffset = (y - (gridDensity - 1) / 2f) * (squareSize / (gridDensity - 1));

                    // Calculate start position with offset
                    Vector3 rayStart = transform.position +
                                     transform.right * xOffset +
                                     transform.up * yOffset;

                    RaycastHit raycastHit;
                    if (Physics.Raycast(rayStart, transform.forward, out raycastHit, range, LayerMask.GetMask("Default")))
                    {
                        if (raycastHit.collider != null &&
                            raycastHit.collider.transform.root != null &&
                            !string.IsNullOrEmpty(raycastHit.collider.transform.root.name) &&
                            raycastHit.collider.transform.root.name.Contains("TransmitterSwitch"))
                        {
                            GameObject open = raycastHit.collider.transform.root.gameObject;
                            Mono.TransmitterSwitch controller = open.GetComponent<TransmitterSwitch>();
                            if (controller != null)
                            {
                                controller.Toggle();
                                return; // Exit after first hit is found and toggled
                            }
                            else
                            {
                                Misc.Msg("Controller is null!");
                            }
                        }
                    }
                }
            }
        }
    }
}
