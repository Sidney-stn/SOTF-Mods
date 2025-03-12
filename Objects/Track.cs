

using UnityEngine;

namespace StoneGate.Objects
{
    internal class Track
    {
        public static Dictionary<BoltEntity, GameObject> spawendStoneGates = new Dictionary<BoltEntity, GameObject>();

        public static GameObject FindGo(BoltEntity entity)
        {
            if (spawendStoneGates.ContainsKey(entity))
            {
                return spawendStoneGates[entity];
            }
            return null;
        }

        public static BoltEntity FindEntity(GameObject go)
        {
            foreach (var item in spawendStoneGates)
            {
                if (item.Value == go)
                {
                    return item.Key;
                }
            }
            return null;
        }
    }
}
