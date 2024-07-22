

using Sons.Prefabs;
using SonsSdk.Attributes;
using TheForest.Utils;
using UnityEngine;

namespace StructureDamageViewer
{
    public class Commands : StructureDamageViewer
    {
        [DebugCommand("StructureDamageViewer")]
        private void SpawnGolfCartMods(string args)
        {
            Misc.Msg("StructureDamageViewer Command");
            switch (args.ToLower())
            {
                case "1":
                    Misc.Msg("1");
                    Trigger();
                    break;
                default:
                    Misc.Msg("Default");
                    break;
            }
        }
    }
}
