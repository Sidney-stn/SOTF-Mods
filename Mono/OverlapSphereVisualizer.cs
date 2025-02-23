using RedLoader;
using TheForest.Utils;
using UnityEngine;
namespace WirelessSignals.Mono
{
    [RegisterTypeInIl2Cpp]
    public class OverlapSphereVisualizer : MonoBehaviour
    {
        public float objectRange = 1f;
        public Material visualMaterial = WirelessSignals.redMat;
        private LineRenderer horizontalLine;
        private LineRenderer verticalLine;
        private const int segments = 32;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        void OnEnable()
        {
            // Create horizontal circle
            GameObject horizontalObj = new GameObject("HorizontalCircle");
            horizontalObj.transform.SetParent(transform);
            horizontalLine = horizontalObj.AddComponent<LineRenderer>();
            SetupLineRenderer(horizontalLine);

            // Create vertical circle
            GameObject verticalObj = new GameObject("VerticalCircle");
            verticalObj.transform.SetParent(transform);
            verticalLine = verticalObj.AddComponent<LineRenderer>();
            SetupLineRenderer(verticalLine);
        }

        void SetupLineRenderer(LineRenderer line)
        {
            line.material = visualMaterial;
            line.positionCount = segments + 1;
            line.useWorldSpace = true;
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        void Update()
        {
            // Draw horizontal circle (XZ plane)
            DrawCircle(horizontalLine, transform.position, Vector3.up);

            // Draw vertical circle (XY plane)
            DrawCircle(verticalLine, transform.position, Vector3.forward);

            // Every 60 frames, check range
            if (Time.frameCount % 60 == 0)
            {
                // Check PlayerRange, If Player Is In Range, Do Nothing
                if (Vector3.Distance(transform.position, LocalPlayer.Transform.position) > 50f)
                {
                    // Destroy Script If Player Is Out Of Range
                    Destroy(this);
                }
            }
        }

        void DrawCircle(LineRenderer line, Vector3 center, Vector3 normal)
        {
            Vector3 forward = normal;
            Vector3 right = Vector3.Cross(normal, Vector3.up).normalized;
            if (right == Vector3.zero)
            {
                right = Vector3.Cross(normal, Vector3.forward).normalized;
            }
            Vector3 up = Vector3.Cross(right, forward);

            float angle = 0f;
            for (int i = 0; i <= segments; i++)
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * angle);
                float y = Mathf.Sin(Mathf.Deg2Rad * angle);
                Vector3 pos = center + (right * x + up * y) * objectRange;
                line.SetPosition(i, pos);
                angle += (360f / segments);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        void OnDestroy()
        {
            if (horizontalLine) Destroy(horizontalLine.gameObject);
            if (verticalLine) Destroy(verticalLine.gameObject);
        }
    }
}