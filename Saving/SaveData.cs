using Newtonsoft.Json;
using UnityEngine;

namespace WirelessSignals.Saving
{
    [Serializable]
    public class SaveData
    {
        public string UniqueId { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}
