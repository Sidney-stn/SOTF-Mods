using RedLoader;
using UnityEngine;

namespace WirelessSignals.Mono
{
    [RegisterTypeInIl2Cpp]
    public class OverlapSphereVisualizer : MonoBehaviour
    {
        public float objectRange = 1f;  // 1f is the default value
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
            verticalObj.transform.rotation = Quaternion.Euler(0, 0, 90); // Rotate to vertical
        }

        void SetupLineRenderer(LineRenderer line)
        {
            line.material = visualMaterial;
            line.positionCount = segments + 1;
            line.useWorldSpace = false;
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        void Update()
        {
            DrawCircle(horizontalLine);
            DrawCircle(verticalLine);
        }

        void DrawCircle(LineRenderer line)
        {
            float angle = 0f;
            for (int i = 0; i <= segments; i++)
            {
                float x = Mathf.Sin(Mathf.Deg2Rad * angle) * objectRange;
                float y = Mathf.Cos(Mathf.Deg2Rad * angle) * objectRange;
                line.SetPosition(i, new Vector3(x, y, 0));
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
