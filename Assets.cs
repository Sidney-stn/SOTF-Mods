using SonsSdk.Attributes;
using UnityEngine;

namespace Warps
{
    [AssetBundle("warps")]
    public static class Assets
    {
        [AssetReference("WarpsUI")]
        public static GameObject WarpsUI { get; set; }
    }
}
