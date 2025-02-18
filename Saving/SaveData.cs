using RedLoader;
using UnityEngine;

namespace WirelessSignals.Saving
{
    [RegisterTypeInIl2Cpp]
    [Serializable] // Add this attribute
    public class SaveData : Il2CppSystem.Object
    {
        public string UniqueId;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
