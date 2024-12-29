using SonsSdk.Attributes;
using UnityEngine;

namespace Banking
{
    [AssetBundle("banking")]
    public static class Assets
    {
        [AssetReference("BankingUI")]
        public static GameObject BankingUI { get; set; }

        [AssetReference("ATM")]
        public static GameObject ATM { get; set; }

        [AssetReference("ATMIcon")]
        public static Texture2D ATMIcon { get; set; }

        [AssetReference("ATMPlacer")]
        public static GameObject ATMPlacer { get; set; }

        [AssetReference("ATMPlaceUI")]
        public static GameObject ATMPlaceUI { get; set; }
    }
}
