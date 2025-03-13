using UnityEngine;

namespace StoneGate.Objects
{
    public struct ModeGameObjectKey : IEquatable<ModeGameObjectKey>
    {
        public string Mode { get; }
        public int GameObjectInstanceID { get; }

        public ModeGameObjectKey(string mode, GameObject gameObject)
        {
            Mode = mode;
            GameObjectInstanceID = gameObject != null ? gameObject.GetInstanceID() : 0;
        }

        public bool Equals(ModeGameObjectKey other)
        {
            return string.Equals(Mode, other.Mode, StringComparison.OrdinalIgnoreCase) &&
                   GameObjectInstanceID == other.GameObjectInstanceID;
        }

        public override bool Equals(object obj)
        {
            return obj is ModeGameObjectKey key && Equals(key);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Mode?.ToUpperInvariant().GetHashCode() ?? 0) * 397) ^ GameObjectInstanceID;
            }
        }
    }
}
