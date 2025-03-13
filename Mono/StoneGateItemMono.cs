using RedLoader;
using System.Collections;
using UnityEngine;

namespace StoneGate.Mono
{
    internal class StoneGateItemMono : MonoBehaviour
    {
        private bool isAnimRunning = false;
        private float animTime = 0.3f;
        private Animator animController;
        private HashSet<string> allowedHits = new HashSet<string>()
        {
            "RockWall",
            "RockPilar",
            "RockBeam"
        };

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
            StartAnim().RunCoro();
        }

        private IEnumerator StartAnim()
        {
            isAnimRunning = true;
            animController.SetTrigger("Hit");
            yield return new WaitForSeconds(animTime);
            isAnimRunning = false;
            yield break;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isAnimRunning) { return; }
            //Misc.Msg($"[StoneGateItemMono] [OnTriggerEnter] {other.gameObject.name} Root: {other.gameObject.transform.root.gameObject.name}");
            string rootName = other.gameObject.transform.root.gameObject.name;
            // Check if haset contains rootName, not exact match
            if (allowedHits.Any(prefix => rootName.StartsWith(prefix)))
            {
                GameObject rootGo = other.gameObject.transform.root.gameObject;
                Misc.Msg($"[StoneGateItemMono] [OnTriggerEnter] Hit {rootName}");
                if (markedObjects.Contains(rootGo))
                {
                    UnmarkHit(rootGo);
                }
                else
                {
                    MarkHit(rootGo);
                }
            }
        }

        private HashSet<GameObject> markedObjects = new HashSet<GameObject>();
        private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

        private void MarkHit(GameObject rootGo)
        {
            Misc.Msg($"[StoneGateItemMono] [MarkHit]");
            Renderer[] renderers = rootGo.GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                Misc.Msg("Failed To Mark Objects - No renderers found");
                return;
            }

            Shader shader = ShaderRuntimeLookup.Find("Sons/Outline/StructuresGhostHLSL");
            Material ghostMat = new Material(shader)
            {
                name = "StructureGhostMaterial",
                color = new UnityEngine.Color(224, 91, 111),
            };

            foreach (Renderer renderer in renderers)
            {
                // Store original materials for restoration later
                if (!originalMaterials.ContainsKey(renderer))
                {
                    originalMaterials[renderer] = renderer.sharedMaterials;
                }

                // Create ghost material array matching the original count
                Material[] ghostedMats = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < ghostedMats.Length; i++)
                {
                    ghostedMats[i] = ghostMat;
                }

                // Apply ghost materials
                renderer.sharedMaterials = ghostedMats;
            }

            markedObjects.Add(rootGo);
        }

        private void UnmarkHit(GameObject rootGo)
        {
            Misc.Msg($"[StoneGateItemMono] [UnmarkHit]");
            Renderer[] renderers = rootGo.GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                Misc.Msg("Failed To Unmark Objects - No renderers found");
                return;
            }

            foreach (Renderer renderer in renderers)
            {
                // Restore original materials if we have them stored
                if (originalMaterials.ContainsKey(renderer))
                {
                    renderer.sharedMaterials = originalMaterials[renderer];
                    originalMaterials.Remove(renderer);
                }
            }

            if (markedObjects.Contains(rootGo))
            {
                markedObjects.Remove(rootGo);
            }
        }

    }
}
