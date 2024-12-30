
using TheForest.Utils;
using UnityEngine;

namespace Banking.LiveData
{
    internal class Helper
    {
        internal static List<GameObject> GetLookingAtGameObjects()
        {
            RaycastHit[] raycastHits = Physics.SphereCastAll(
                LocalPlayer.Transform.position,    // Origin
                1f,                   // Sphere radius
                LocalPlayer.Transform.forward,     // Direction
                25f,                  // Max distance
                LayerMask.GetMask(new string[] {
                    "Default"
                })
            );

            // Get list of hit GameObjects
            List<GameObject> hitObjects = new List<GameObject>();
            foreach (RaycastHit hit in raycastHits)
            {
                if (hit.collider != null)
                {
                    hitObjects.Add(hit.collider.transform.root.gameObject);
                }
            }
            return hitObjects;
        }
        internal static List<GameObject> GetLookingAtATMGameObjects()
        {
            RaycastHit[] raycastHits = Physics.SphereCastAll(
                LocalPlayer.Transform.position,    // Origin
                1f,                   // Sphere radius
                LocalPlayer.Transform.forward,     // Direction
                25f,                  // Max distance
                LayerMask.GetMask(new string[] {
                    "Default"
                })
            );

            // Get list of hit GameObjects
            List<GameObject> hitObjects = new List<GameObject>();
            foreach (RaycastHit hit in raycastHits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.transform.root.name.Contains("ATM"))
                    {
                        hitObjects.Add(hit.collider.transform.root.gameObject);
                    }                        
                }
            }
            return hitObjects;
        }
    }
}
