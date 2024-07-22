using SonsSdk.Attributes;
using TheForest.Utils;
using UnityEngine;

namespace StructureDamageViewer
{
    public class Commands : StructureDamageViewer
    {
        [DebugCommand("StructureDamageViewer")]
        private void StructureDamageViewerCommand(string args)
        {
            Misc.Msg("StructureDamageViewer Command");
            switch (args.ToLower())
            {
                case "off":
                    Misc.Msg("OFF");
                    foreach (DamageMono mono in Misc.damageMonos)
                    {
                        mono.isColoringUpdated = false;
                        mono.isColoringEnabled = false;
                        
                    }
                    break;
                case "on":
                    Misc.Msg("ON");
                    foreach (DamageMono mono in Misc.damageMonos)
                    {
                        mono.isColoringUpdated = false;
                        mono.isColoringEnabled = true;

                    }
                    break;
                case "layermask":
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
