using Construction;
using Endnight.Utilities;
using RedLoader;
using Sons.Gui;
using SonsSdk;
using System.Collections;
using TheForest.Utils;
using UnityEngine;

namespace StructureDamageViewer
{
    [RegisterTypeInIl2Cpp]
    public class DamageMono : MonoBehaviour
    {
        public Structure thisStructure;
        public GameObject attatcedToGameObject;

        // List to hold the MeshRenderers for LODs
        private List<MeshRenderer> lodRenderers;

        // Store the original color
        private Color originalColor;

        // Colors for different health ranges
        public Color highHealthColor = Color.green;
        public Color mediumHealthColor = new Color(1.0f, 0.5f, 0.0f);
        public Color lowHealthColor = Color.red;

        private void Start ()
        {
            // Initialize the list
            lodRenderers = new List<MeshRenderer>();

            // Get all mesh renderers in children
            MeshRenderer[] allRenderers = GetComponentsInChildren<MeshRenderer>();

            foreach (var renderer in allRenderers)
            {
                // Check if the renderer's GameObject name contains "LOD0", "LOD1", or "LOD2"
                if (renderer.gameObject.name.Contains("LOD0") || renderer.gameObject.name.Contains("LOD1") || renderer.gameObject.name.Contains("LOD2"))
                {
                    lodRenderers.Add(renderer);
                }
            }

            if (lodRenderers.Count > 0)
            {
                // Store the original color from the first LOD renderer (assuming all LODs have the same initial color)
                originalColor = lodRenderers[0].material.GetColor("_BaseColor"); // Use "_Color" if the shader is older
            }
            else
            {
                Misc.Msg("LOD renderers not found on the game object.");
                Destroy(this);
            }

            // Start the coroutine
            LoopEveryFiveSeconds().RunCoro();
        }

        IEnumerator LoopEveryFiveSeconds()
        {
            while (true) // This creates an infinite loop
            {
                // Perform your action here
                //Misc.Msg("Action performed at " + Time.time);


                // Wait for 5 seconds
                yield return new WaitForSeconds(5f);

                CheckDamage();
            }
        }

        private void CheckDamage()
        {
            if (!LocalPlayer.IsInWorld || LocalPlayer.IsInInventory || PauseMenu.IsActive) { Misc.Msg("Requriements failed retuned"); return; }

            float num2;
            float num3;
            bool healthRetrieved = StructureDestructionManager.TryGetStructureHealth(thisStructure, out num2, out num3);

            if (healthRetrieved)
            {
                Misc.Msg($"{string.Format("Health={0}/{1}", num2, num3)} ");
                if (num2 >= num3)
                {
                    // Reset the color to the original before destroying the script
                    SetLODColors(originalColor);

                    Destroy(this);
                }
                else if (attatcedToGameObject.name.Contains("DefensiveWall"))
                {
                    int childCount = attatcedToGameObject.transform.childCount;
                    float newMaxHealth;
                    switch (childCount)
                    {
                        case 0-4:
                            newMaxHealth = num3 - (60 * childCount);
                            UpdateColorBasedOnHealth(num2, newMaxHealth);
                            break;
                        case 5:
                            UpdateColorBasedOnHealth(num2, num3);
                            break;
                    }
                }
                else
                {
                    UpdateColorBasedOnHealth(num2, num3);
                }
            }
            else
            {
                Misc.Msg("[StructureDamageViewer] Could not retrieve health for structure");
            }
        }

        private void UpdateColorBasedOnHealth(float currentHealth, float maxHealth)
        {
            if (lodRenderers == null || lodRenderers.Count == 0) return;

            float healthPercentage = (currentHealth / maxHealth) * 100f;
            Color targetColor;

            if (healthPercentage >= 85f && healthPercentage < 100f)
            {
                targetColor = highHealthColor;
            }
            else if (healthPercentage >= 40f && healthPercentage < 85f)
            {
                targetColor = mediumHealthColor;
            }
            else
            {
                targetColor = lowHealthColor;
            }

            SetLODColors(targetColor);
        }

        private void SetLODColors(Color color)
        {
            foreach (var renderer in lodRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.SetColor("_BaseColor", color); // Use "_Color" if the shader is older
                }
            }
        }

    }

    [RegisterTypeInIl2Cpp]
    public class LocalPlayerTrackMono : MonoBehaviour
    {
        private void Start()
        {
            // Start the coroutine
            LoopEveryTenSeconds().RunCoro();
        }


        IEnumerator LoopEveryTenSeconds()
        {
            while (true) // This creates an infinite loop
            {
                // Perform your action here
                //Misc.Msg("Action performed at " + Time.time);

                // Wait for 5 seconds
                yield return new WaitForSeconds(10f);

                CheckStructure();
            }
        }

        private void CheckStructure()
        {
            if (!LocalPlayer.IsInWorld || LocalPlayer.IsInInventory || PauseMenu.IsActive) { Misc.Msg("Requriements failed retuned"); return; }
            Collider[] hitColliders = Physics.OverlapSphere(LocalPlayer.Transform.position, Config.StructureDamageViewerScanDistance.Value, LayerMask.GetMask(new string[]
                {
                "Prop"
                }));
            foreach (var hitCollider in hitColliders)
            {
                GameObject currentHit;
                if (hitCollider.gameObject.name.Contains("Log")) { currentHit = hitCollider.gameObject; }
                else { currentHit = null; }
                if (currentHit != null)
                {
                    DamageMono damageMono;
                    if (currentHit.TryGetComponentInParent(out damageMono))
                    {
                        continue;
                    }
                    else
                    {
                        try
                        {
                            Structure structure;
                            if (currentHit.TryGetComponentInParent(out structure))
                            {
                                Misc.Msg($"[StructureDamageViewer] Found structure: {structure.name}");
                                float num2;
                                float num3;
                                bool healthRetrieved = StructureDestructionManager.TryGetStructureHealth(structure, out num2, out num3);

                                if (healthRetrieved)
                                {
                                    float structuralResistanceFactor = structure.GetStructuralResistanceFactor(6);
                                    //Misc.Msg($"{string.Format("Health={0}/{1}", num2, num3)} ");
                                    if (num2 < num3)  // If Structure Has Taken Damage
                                    {
                                        DamageMono localInstance = currentHit.transform.root.gameObject.AddComponent<DamageMono>();
                                        localInstance.thisStructure = structure;
                                        localInstance.attatcedToGameObject = currentHit.transform.root.gameObject;
                                        Misc.Msg($"Added DamageMono To: {currentHit.transform.root.gameObject.name}");
                                    }
                                }
                                else
                                {
                                    Misc.Msg("[StructureDamageViewer] Could not retrieve health for structure");
                                }
                            }
                            else
                            {
                                Misc.Msg($"[StructureDamageViewer] No structure found in hit");
                            }
                            continue;
                        }
                        catch (Exception e)
                        {
                            RLog.Error($"Error while finding Damaged Objects. Error: {e}");
                            continue;
                        }
                    }
                }
            }
        }
    }
}
