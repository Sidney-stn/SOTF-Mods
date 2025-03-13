using UnityEngine;

namespace StoneGate.Objects
{
    public class GameObjectInstanceIDComparer : IEqualityComparer<GameObject>
    {
        public bool Equals(GameObject x, GameObject y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.GetInstanceID() == y.GetInstanceID();
        }

        public int GetHashCode(GameObject obj)
        {
            if (obj is null) return 0;
            return obj.GetInstanceID().GetHashCode();
        }
    }
}
