using SonsSdk.Attributes;
using UnityEngine;

namespace WirelessSignals
{
    [AssetBundle("wireless")]
    public static class Assets
    {
        [AssetReference("TransmitterSwitch")]
        public static GameObject TransmitterSwitch { get; set; }

        [AssetReference("Reciver")]
        public static GameObject Reciver { get; set; }

        [AssetReference("TransmitterDetector")]
        public static GameObject TransmitterDetector { get; set; }

        [AssetReference("ReciverUI")]
        public static GameObject ReciverUI { get; set; }

    }
}
