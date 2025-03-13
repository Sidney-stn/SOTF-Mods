using RedLoader;
using StoneGate.Objects;
using System.Collections;
using TheForest.Utils;
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
        private float raycastDistance = 1f;

        // Store original materials by GameObject instance ID for more reliable lookups
        private Dictionary<int, Dictionary<int, Material[]>> originalMaterials = new Dictionary<int, Dictionary<int, Material[]>>();
        private HashSet<GameObject> markedObjects = new HashSet<GameObject>(new GameObjectInstanceIDComparer());

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

            // Clean up any marked objects when the item is disabled
            CleanupAllMarkedObjects();
        }

        private void CleanupAllMarkedObjects()
        {
            // Make a copy of the list to avoid modification during iteration
            GameObject[] objectsToUnmark = new GameObject[markedObjects.Count];
            markedObjects.CopyTo(objectsToUnmark);

            foreach (var obj in objectsToUnmark)
            {
                if (obj != null)
                {
                    UnmarkHit(obj);
                }
            }

            // Clear dictionaries
            markedObjects.Clear();
            originalMaterials.Clear();
        }

        public void InitHit()
        {
            if (isAnimRunning) { return; }
            //Misc.Msg($"[StoneGateItemMono] [InitHit]");
            StartAnim().RunCoro();
        }

        private IEnumerator StartAnim()
        {
            isAnimRunning = true;
            animController.SetTrigger("Hit");
            yield return new WaitForSeconds(animTime / 2);
            TryHitObject();  // Should Be Called After Half Of The Animation Time (Peak Of Hit)
            yield return new WaitForSeconds(animTime / 2);
            isAnimRunning = false;
            yield break;
        }

        private void TryHitObject()
        {
            if (!isAnimRunning) { return; }
            Transform transform = LocalPlayer._instance._mainCam.transform;  // Player Camera
            RaycastHit raycastHit;  // Raycast Hit
            Physics.Raycast(transform.position, transform.forward, out raycastHit, raycastDistance, LayerMask.GetMask(new string[]
            {
                    "Prop"  // Prop Layer
            }));
            if (raycastHit.collider != null)
            {
                //Misc.Msg($"[StoneGateItemMono] [TryHitObject] Hit {raycastHit.collider.gameObject.name}");
                string rootName = raycastHit.collider.gameObject.transform.root.gameObject.name;
                if (allowedHits.Any(prefix => rootName.StartsWith(prefix)))
                {
                    GameObject rootGo = raycastHit.collider.gameObject.transform.root.gameObject;
                    Misc.Msg($"[StoneGateItemMono] [TryHitObject] Hit {rootName}");
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
        }

        //private void OnTriggerEnter(Collider other)  // Was not satisfied with this method
        //{
        //    if (!isAnimRunning) { return; }
        //    //Misc.Msg($"[StoneGateItemMono] [OnTriggerEnter] {other.gameObject.name} Root: {other.gameObject.transform.root.gameObject.name}");
        //    string rootName = other.gameObject.transform.root.gameObject.name;
        //    // Check if haset contains rootName, not exact match
        //    if (allowedHits.Any(prefix => rootName.StartsWith(prefix)))
        //    {
        //        GameObject rootGo = other.gameObject.transform.root.gameObject;
        //        Misc.Msg($"[StoneGateItemMono] [OnTriggerEnter] Hit {rootName}");
        //        if (markedObjects.Contains(rootGo))
        //        {
        //            UnmarkHit(rootGo);
        //        }
        //        else
        //        {
        //            MarkHit(rootGo);
        //        }
        //    }
        //}

        private void MarkHit(GameObject rootGo, UnityEngine.Color? color = null)
        {
            Misc.Msg($"[StoneGateItemMono] [MarkHit] Object: {rootGo.name}, InstanceID: {rootGo.GetInstanceID()}");
            Renderer[] renderers = rootGo.GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                Misc.Msg("Failed To Mark Objects - No renderers found");
                return;
            }

            Shader shader = ShaderRuntimeLookup.Find("Sons/Outline/StructuresGhostHLSL");
            UnityEngine.Color color1 = color ?? new UnityEngine.Color(224f / 255f, 91f / 255f, 111f / 255f, 1f);
            Material ghostMat = new Material(shader)
            {
                name = "StructureGhostMaterial",
                color = color1
            };

            int rootInstanceID = rootGo.GetInstanceID();
            // Create a new dictionary for this game object if needed
            if (!originalMaterials.ContainsKey(rootInstanceID))
            {
                originalMaterials[rootInstanceID] = new Dictionary<int, Material[]>();
            }

            foreach (Renderer renderer in renderers)
            {
                int rendererId = renderer.GetInstanceID();
                // Store original materials for restoration later
                if (!originalMaterials[rootInstanceID].ContainsKey(rendererId))
                {
                    // Make a copy of the original materials array
                    Material[] originalMats = new Material[renderer.sharedMaterials.Length];
                    System.Array.Copy(renderer.sharedMaterials, originalMats, renderer.sharedMaterials.Length);
                    originalMaterials[rootInstanceID][rendererId] = originalMats;

                    Misc.Msg($"[StoneGateItemMono] [MarkHit] Stored {originalMats.Length} original materials for renderer: {renderer.name}, ID: {rendererId}");
                }

                // Create ghost material array matching the original count
                Material[] ghostedMats = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < ghostedMats.Length; i++)
                {
                    ghostedMats[i] = ghostMat;
                }

                // Apply ghost materials
                renderer.sharedMaterials = ghostedMats;
                Misc.Msg($"[StoneGateItemMono] [MarkHit] Applied ghost materials to: {renderer.name}");
            }

            markedObjects.Add(rootGo);
        }

        private void UnmarkHit(GameObject rootGo)
        {
            if (rootGo == null)
            {
                Misc.Msg("[StoneGateItemMono] [UnmarkHit] GameObject is null");
                return;
            }

            int rootInstanceID = rootGo.GetInstanceID();
            Misc.Msg($"[StoneGateItemMono] [UnmarkHit] Object: {rootGo.name}, InstanceID: {rootInstanceID}");

            // Check if we have stored materials for this GameObject
            if (!originalMaterials.ContainsKey(rootInstanceID))
            {
                Misc.Msg($"[StoneGateItemMono] [UnmarkHit] No original materials found for {rootGo.name}");
                return;
            }

            Renderer[] renderers = rootGo.GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                Misc.Msg("[StoneGateItemMono] [UnmarkHit] Failed To Unmark Objects - No renderers found");
                return;
            }

            foreach (Renderer renderer in renderers)
            {
                int rendererId = renderer.GetInstanceID();

                // Restore original materials if we have them stored
                if (originalMaterials[rootInstanceID].ContainsKey(rendererId))
                {
                    Material[] origMats = originalMaterials[rootInstanceID][rendererId];
                    Misc.Msg($"[StoneGateItemMono] [UnmarkHit] Restoring {origMats.Length} materials for {renderer.name}, ID: {rendererId}");

                    // Apply the original materials back
                    renderer.sharedMaterials = origMats;

                    // Remove from tracked renderers
                    originalMaterials[rootInstanceID].Remove(rendererId);
                }
                else
                {
                    Misc.Msg($"[StoneGateItemMono] [UnmarkHit] No materials found for renderer: {renderer.name}, ID: {rendererId}");
                }
            }

            // Clean up dictionary entry if empty
            if (originalMaterials[rootInstanceID].Count == 0)
            {
                originalMaterials.Remove(rootInstanceID);
            }

            if (markedObjects.Contains(rootGo))
            {
                markedObjects.Remove(rootGo);
                Misc.Msg($"[StoneGateItemMono] [UnmarkHit] Removed {rootGo.name} from marked objects");
            }
        }
    }
}