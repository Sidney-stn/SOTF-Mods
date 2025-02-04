using SonsSdk.Attributes;
using UnityEngine;

namespace Shops
{
    [AssetBundle("singleshop")]
    public static class Assets
    {
        [AssetReference("SingleShop")]
        public static GameObject SingleShop { get; set; }

        [AssetReference("Buy")]
        public static Texture2D BuyIcon { get; set; }

        [AssetReference("InsertWhite")]
        public static Texture2D DepositIcon { get; set; }

        [AssetReference("BookPageShop")]
        public static Texture2D BookPage { get; set; }
    }
}
