

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
                    break;
                case "2":
                    Misc.Msg("2");
                    Trigger2();
                    break;
                case "3":
                    Transform transform = LocalPlayer._instance._mainCam.transform;
                    Vector3 raycastStartPosition = transform.position + transform.forward * 0.2f; // Offset the start position 0.1 units in front of the player
                    RaycastHit raycastHit;
                    Physics.Raycast(raycastStartPosition, transform.forward, out raycastHit, 1f, ~(1 << 26));
                    GameObject topLevelParent = raycastHit.collider.gameObject.transform.root.gameObject;
                    Misc.Msg($"Hit: {raycastHit.collider.gameObject.name}");
                    Misc.Msg($"Hit topLevelParent: {topLevelParent.name}");

                    int layer = raycastHit.collider.gameObject.layer;
                    string layerName = LayerMask.LayerToName(layer);
                    Misc.Msg($"LAYERMASK: {layerName}");
                    break;
                default:
                    Misc.Msg("Default");
                    break;
            }
        }
    }
}
