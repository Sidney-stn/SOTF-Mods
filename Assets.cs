using SonsSdk.Attributes;
using UnityEngine;

namespace Signs
{
    [AssetBundle("signs")]
    public static class Assets
    {
        [AssetReference("Sign")]
        public static GameObject SignObj { get; set; }

        [AssetReference("SignUi")]
        public static GameObject SignUi { get; set; }

        [AssetReference("SignPlaceUI")]
        public static GameObject SignPlaceUI { get; set; }

        [AssetReference("SignIcon")]
        public static Texture2D SignIcon { get; set; }
    }
}
