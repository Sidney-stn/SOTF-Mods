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
        private float raycastDistance = 1.5f;

        // Store original materials by GameObject instance ID for more reliable lookups
        private Dictionary<int, Dictionary<int, Material[]>> originalMaterials = new Dictionary<int, Dictionary<int, Material[]>>();
        private HashSet<GameObject> markedObjects = new HashSet<GameObject>(new GameObjectInstanceIDComparer());
        private Dictionary<ModeGameObjectKey, GameObject> markedObjectsWithMode = new Dictionary<ModeGameObjectKey, GameObject>();

        private LineRenderer axisLineRenderer;

        private void Start()
        {
            Misc.Msg($"[StoneGateItemMono] [Start]");
            Objects.ActiveItem.active = this;

            animController = GetComponent<Animator>();

            if (LocalPlayer.Inventory?.LeftHandItem?._itemID == StoneGate.ToolItemId)
            {
                Misc.Msg($"[StoneGateItemMono] [Start] StoneGateItem Found");
                //StoneGateUi.OpenMainPanel();
            }
            StoneGateUi.OpenMainPanel();
        }

        private void OnDisable()
        {
            Misc.Msg($"[StoneGateItemMono] [OnDisable]");
            Objects.ActiveItem.active = null;

            // Clean up any marked objects when the item is disabled
            CleanupAllMarkedObjects();

            StoneGateUi.CloseMainPanel();
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
            markedObjectsWithMode.Clear();
            Destroy(axisLineRenderer);
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
                        RemoveObjectWithoutMode(rootGo);
                    }
                    else
                    {
                        string currentToolMode = UiController.GetMode();
                        if (currentToolMode == UiController.GetAllowedModes().ElementAt(0))  // MARK
                        {
                            MarkHit(rootGo);
                            AddObjectWithMode(UiController.GetAllowedModes().ElementAt(0), rootGo);

                        }
                        else if (currentToolMode == UiController.GetAllowedModes().ElementAt(1))  // ROTATE
                        {
                            MarkHit(rootGo, UnityEngine.Color.green);
                            AddObjectWithMode(UiController.GetAllowedModes().ElementAt(1), rootGo);
                            CreateAxisLineRenderer(rootGo);
                        }
                        else
                        {
                            Misc.Msg($"[StoneGateItemMono] [TryHitObject] Unknown Tool Mode: {currentToolMode}");
                        }
                        
                    }
                }
            }
        }

        private void CreateAxisLineRenderer(GameObject go)
        {
            // If the GameObject doesn't have a LineRenderer component, add one
            axisLineRenderer = go.GetComponent<LineRenderer>();
            if (axisLineRenderer == null)
            {
                axisLineRenderer = go.AddComponent<LineRenderer>();
            }

            // Configure the LineRenderer
            axisLineRenderer.startWidth = 0.05f;
            axisLineRenderer.endWidth = 0.05f;
            axisLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            axisLineRenderer.startColor = Color.red;
            axisLineRenderer.endColor = Color.red;

            // Calculate the bounds to find the longest axis
            Bounds bounds = CalculateBounds(go);
            Vector3 size = bounds.size;

            // Determine which axis is the longest
            Vector3 direction = Vector3.forward; // Default to Z-axis
            float maxSize = size.z;

            if (size.x > maxSize)
            {
                direction = Vector3.right;
                maxSize = size.x;
            }

            if (size.y > maxSize)
            {
                direction = Vector3.up;
                maxSize = size.y;
            }

            // Set the LineRenderer positions to follow the longest axis
            axisLineRenderer.positionCount = 2;
            float halfLength = maxSize / 2;
            axisLineRenderer.SetPosition(0, go.transform.position - direction * halfLength);
            axisLineRenderer.SetPosition(1, go.transform.position + direction * halfLength);

            Misc.Msg($"[StoneGateMono] [CreateAxisLineRenderer] Created line renderer along axis {direction} with length {maxSize}");
        }

        private Bounds CalculateBounds(GameObject go)
        {
            // Initialize bounds with center at the GameObject's position
            Bounds bounds = new Bounds(go.transform.position, Vector3.zero);

            // Get all renderers in the GameObject and its children
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

            // If there are no renderers, try to use colliders
            if (renderers.Length == 0)
            {
                Collider[] colliders = go.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                {
                    bounds.Encapsulate(collider.bounds);
                }
            }
            else
            {
                // Encapsulate the bounds of all renderers
                foreach (Renderer renderer in renderers)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            // If we still have zero size (no renderers or colliders), use a default size
            if (bounds.size == Vector3.zero)
            {
                bounds.size = new Vector3(1, 1, 1);
            }

            return bounds;
        }
    

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

        public void AddObjectWithMode(string mode, GameObject gameObject)
        {
            if (!UiController.IsValidMode(mode) || gameObject == null)
                return;

            var key = new ModeGameObjectKey(mode, gameObject);
            markedObjectsWithMode[key] = gameObject;
        }

        public bool HasObjectWithMode(string mode, GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            var key = new ModeGameObjectKey(mode, gameObject);
            return markedObjectsWithMode.ContainsKey(key);
        }

        public GameObject GetObjectWithMode(string mode, GameObject gameObject)
        {
            var key = new ModeGameObjectKey(mode, gameObject);
            return markedObjectsWithMode.TryGetValue(key, out var result) ? result : null;
        }

        public void RemoveObjectWithMode(string mode, GameObject gameObject)
        {
            var key = new ModeGameObjectKey(mode, gameObject);
            markedObjectsWithMode.Remove(key);
        }

        public void RemoveObjectWithoutMode(GameObject gameObject)
        {
            foreach (var mode in UiController.GetAllowedModes())
            {
                var key = new ModeGameObjectKey(mode, gameObject);
                if (markedObjectsWithMode.ContainsKey(key))
                {
                    markedObjectsWithMode.Remove(key);
                    if (mode == UiController.GetAllowedModes().ElementAt(1))  // ROTATE
                    {
                        Destroy(axisLineRenderer);
                    }
                }
            }
        }
    }
}