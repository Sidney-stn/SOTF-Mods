using UnityEngine;
using WirelessSignals.Prefab;

namespace WirelessSignals.Saving
{
    // In a new file, e.g., SaveDataStructs.cs
    namespace WirelessSignals.Saving
    {
        [Serializable]
        public class SerializableVector3
        {
            public float x;
            public float y;
            public float z;

            public SerializableVector3(Vector3 vector)
            {
                x = vector.x;
                y = vector.y;
                z = vector.z;
            }

            public Vector3 ToVector3()
            {
                return new Vector3(x, y, z);
            }
        }

        [Serializable]
        public class SerializableQuaternion
        {
            public float x;
            public float y;
            public float z;
            public float w;

            public SerializableQuaternion(Quaternion quaternion)
            {
                x = quaternion.x;
                y = quaternion.y;
                z = quaternion.z;
                w = quaternion.w;
            }

            public Quaternion ToQuaternion()
            {
                return new Quaternion(x, y, z, w);
            }
        }

        [Serializable]
        public class SerializableSaveData
        {
            public string UniqueId { get; set; }
            public SerializableVector3 Position { get; set; }
            public SerializableQuaternion Rotation { get; set; }

            public static SerializableSaveData FromSaveData(SaveData data)
            {
                return new SerializableSaveData
                {
                    UniqueId = data.UniqueId,
                    Position = new SerializableVector3(data.Position),
                    Rotation = new SerializableQuaternion(data.Rotation)
                };
            }

            public SaveData ToSaveData()
            {
                return new SaveData
                {
                    UniqueId = UniqueId,
                    Position = Position.ToVector3(),
                    Rotation = Rotation.ToQuaternion()
                };
            }
        }

        [Serializable]
        public class SerializableTransmitterSwitchData : SerializableSaveData
        {
            public bool? IsOn { get; set; }

            public static SerializableTransmitterSwitchData FromSaveData(TransmitterSwitchSaveData data)
            {
                var baseData = FromSaveData((SaveData)data);
                return new SerializableTransmitterSwitchData
                {
                    UniqueId = baseData.UniqueId,
                    Position = baseData.Position,
                    Rotation = baseData.Rotation,
                    IsOn = data.IsOn
                };
            }

            public new TransmitterSwitchSaveData ToSaveData()
            {
                return new TransmitterSwitchSaveData
                {
                    UniqueId = UniqueId,
                    Position = Position.ToVector3(),
                    Rotation = Rotation.ToQuaternion(),
                    IsOn = IsOn
                };
            }
        }

        // Add similar classes for Receiver and Detector
    }
}
