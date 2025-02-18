using Newtonsoft.Json;
using RedLoader;
using UnityEngine;

namespace WirelessSignals.Saving
{
    [RegisterTypeInIl2Cpp]
    public class SaveData
    {
        public string UniqueId { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}
