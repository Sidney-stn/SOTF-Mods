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

        private void Start ()
        {
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
                if (num2 >= num3) { Destroy(this); }
            }
            else
            {
                Misc.Msg("[StructureDamageViewer] Could not retrieve health for structure");
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
