using UnityEngine;

namespace WirelessSignals.Tools
{
    public class GameObjectComparer : IEqualityComparer<GameObject>
    {
        public bool Equals(GameObject x, GameObject y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.GetInstanceID() == y.GetInstanceID();
        }

        public int GetHashCode(GameObject obj)
        {
            return obj?.GetInstanceID() ?? 0;
        }
    }
}
