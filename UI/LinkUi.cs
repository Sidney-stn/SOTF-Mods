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
            float range = 5f;
            float squareSize = 0.3f;
            int gridDensity = 3;

            Material blackMat = WirelessSignals.blackMat;
            Material redMat = WirelessSignals.redMat;

            // Create LineRenderers if visualization is enabled
            List<LineRenderer> lineRenderers = new List<LineRenderer>();
            if (Config.VisualRayCast.Value)
            {
                
                GameObject lineContainer = new GameObject("RaycastLines");
                Debug.RayCast.gameObjects.Add(lineContainer);

                //lineContainer.transform.parent = transform;

                // Create a LineRenderer for each ray
                for (int i = 0; i < gridDensity * gridDensity; i++)
                {
                    GameObject lineObj = new GameObject($"RayLine_{i}");
                    lineObj.transform.parent = lineContainer.transform;

                    LineRenderer line = lineObj.AddComponent<LineRenderer>();
                    line.material = redMat;
                    line.startWidth = 0.01f;
                    line.endWidth = 0.01f;
                    line.positionCount = 2;

                    lineRenderers.Add(line);
                }
            }

            int lineIndex = 0;
            for (int x = 0; x < gridDensity; x++)
            {
                for (int y = 0; y < gridDensity; y++)
                {
                    float xOffset = (x - (gridDensity - 1) / 2f) * (squareSize / (gridDensity - 1));
                    float yOffset = (y - (gridDensity - 1) / 2f) * (squareSize / (gridDensity - 1));

                    Vector3 rayStart = transform.position +
                                     transform.right * xOffset +
                                     transform.up * yOffset;

                    RaycastHit raycastHit;
                    bool hitSomething = Physics.Raycast(rayStart, transform.forward, out raycastHit, range, LayerMask.GetMask("Default"));

                    // Visualize raycast if enabled
                    if (Config.VisualRayCast.Value)
                    {
                        LineRenderer line = lineRenderers[lineIndex];
                        line.SetPosition(0, rayStart);

                        // Set end position based on whether we hit something
                        if (hitSomething)
                        {
                            line.SetPosition(1, raycastHit.point);
                            line.material = blackMat; // Green for hits
                        }
                        else
                        {
                            line.SetPosition(1, rayStart + transform.forward * range);
                            line.material = redMat; // Red for misses
                        }
                        lineIndex++;
                    }

                    if (hitSomething &&
                        raycastHit.collider != null &&
                        raycastHit.collider.transform.root != null &&
                        !string.IsNullOrEmpty(raycastHit.collider.transform.root.name))
                    {
                        string hitName = raycastHit.collider.transform.root.name;
                        if (hitName.Contains("TransmitterSwitch"))  // For Toggeling Physical Switch
                        {
                            GameObject open = raycastHit.collider.transform.root.gameObject;
                            Mono.TransmitterSwitch controller = open.GetComponent<TransmitterSwitch>();
                            if (controller != null)
                            {
                                controller.Toggle();

                                // Clean up line renderers before returning
                                //if (Config.VisualRayCast.Value)
                                //{
                                //    GameObject.Destroy(lineRenderers[0].transform.parent.gameObject);
                                //}
                                return;
                            }
                            else
                            {
                                Misc.Msg("Controller is null!");
                            }
                        }
                        else if (hitName.Contains("Reciver"))  // For Opening Reciver UI
                        {
                            GameObject open = raycastHit.collider.transform.root.gameObject;
                            Mono.Reciver controller = open.GetComponent<Reciver>();
                            if (controller != null)
                            {
                                LinkUiElement linkUi = controller._linkUi;
                                if (linkUi == null) { return; }
                                if (linkUi.IsActive)
                                {
                                    UI.ReciverUI.activeReciverPrefab = controller.gameObject;
                                    UI.ReciverUI.OpenUI();
                                }
                                return;
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