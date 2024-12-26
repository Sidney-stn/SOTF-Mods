using SonsSdk.Attributes;
using UnityEngine;

namespace Banking
{
    [AssetBundle("banking")]
    public static partial class Assets
    {
        [AssetReference("BankingUI")]
        public static GameObject BankingUI { get; set; }

        [AssetReference("ATM")]
        public static GameObject ATM { get; set; }
    }
}
