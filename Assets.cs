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

        [AssetReference("OnOff")]
        public static Texture2D UIOnOff { get; set; }

        [AssetReference("Adjust")]
        public static Texture2D UIAdjust { get; set; }

        [AssetReference("WirelessSwitchBookPage")]
        public static Texture2D BookPageSwitch { get; set; }

        [AssetReference("WirelessReciverBookPage")]
        public static Texture2D BookPageReciver { get; set; }

        [AssetReference("WirelessDetectorBookPage")]
        public static Texture2D BookPageDetector { get; set; }
        [AssetReference("TakeOwner")]
        public static Texture2D UITakeOwner { get; set; }
        
    }
}
