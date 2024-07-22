using UnityEngine;

namespace StructureDamageViewer
{
    public static class RaycastHelper
    {
        public static bool ShowGizmo = false;
        public static int MaxHits = 100;

        public static int LineCastAllNonAlloc(Vector3 from, Vector3 to, float radius, out RaycastHit[] hits, int castLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide)
        {
            float maxDistance = Vector3.Distance(from, to);
            Vector3 direction = (to - from).normalized;
            RaycastHit[] results = new RaycastHit[MaxHits];

            // Perform the sphere cast
            int hitCount = Physics.SphereCastNonAlloc(from, radius, direction, results, maxDistance, castLayers, queryTriggerInteraction);

            // Optionally show gizmos for debugging
            if (hitCount > 0 && ShowGizmo)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    // Implement gizmo drawing logic here, if required
                }
            }

            hits = results;
            return hitCount;
        }
    }
}
