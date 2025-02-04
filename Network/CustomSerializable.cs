using System.Globalization;
using UnityEngine;



namespace Shops.Network
{
    internal static class CustomSerializable
    {
        public static string Vector3ToString(Vector3 vector)
        {
            return $"{vector.x.ToString(CultureInfo.InvariantCulture)},{vector.y.ToString(CultureInfo.InvariantCulture)},{vector.z.ToString(CultureInfo.InvariantCulture)}";
        }

        public static Vector3 Vector3FromString(string vectorString)
        {
            string[] parts = vectorString.Split(',');
            if (parts.Length != 3)
            {
                Misc.Msg("String is not a valid Vector3 format");
                return new Vector3(0, 0, 0);
            }

            return new Vector3(
                float.Parse(parts[0], CultureInfo.InvariantCulture),
                float.Parse(parts[1], CultureInfo.InvariantCulture),
                float.Parse(parts[2], CultureInfo.InvariantCulture)
            );
        }

        public static string QuaternionToString(Quaternion quaternion)
        {
            return $"{quaternion.x.ToString(CultureInfo.InvariantCulture)},{quaternion.y.ToString(CultureInfo.InvariantCulture)},{quaternion.z.ToString(CultureInfo.InvariantCulture)},{quaternion.w.ToString(CultureInfo.InvariantCulture)}";
        }

        public static Quaternion QuaternionFromString(string quaternionString)
        {
            string[] parts = quaternionString.Split(',');
            if (parts.Length != 4)
            {
                Misc.Msg("String is not a valid Quaternion format");
                return new Quaternion(0, 0, 0, 0);
            }

            return new Quaternion(
                float.Parse(parts[0], CultureInfo.InvariantCulture),
                float.Parse(parts[1], CultureInfo.InvariantCulture),
                float.Parse(parts[2], CultureInfo.InvariantCulture),
                float.Parse(parts[3], CultureInfo.InvariantCulture)
            );
        }
    }
}
