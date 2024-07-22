using UnityEngine;

namespace StructureDamageViewer
{
    public static class RaycastHelper
    {
        public static bool ShowGizmo = false;
        public static int MaxHits = 100;
        private static RaycastHit[] hitsBuffer = new RaycastHit[MaxHits];

        public static int LineCastAllNonAlloc(Vector3 from, Vector3 to, float radius, out RaycastHit[] hits, int castLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide)
        {
            float maxDistance = Vector3.Distance(from, to);
            Vector3 direction = (to - from).normalized;

            // Perform the sphere cast
            int hitCount = Physics.SphereCastNonAlloc(from, radius, direction, hitsBuffer, maxDistance, castLayers, queryTriggerInteraction);

            // Optionally show gizmos for debugging
            if (hitCount > 0 && ShowGizmo)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    // Implement gizmo drawing logic here, if required
                }
            }

            // Copy the valid hits to the output array
            hits = new RaycastHit[hitCount];
            for (int i = 0; i < hitCount; i++)
            {
                hits[i] = hitsBuffer[i];
            }

            return hitCount;
        }
    }
}
