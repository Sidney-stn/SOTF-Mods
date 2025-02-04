using Shops.Mono;
using Sons.Gui;
using TheForest.Utils;
using UnityEngine;

namespace Shops.Prefab
{
    internal class SingeShop
    {
        internal static GameObject gameObjectWithComps;
        internal static void SetupPrefab()
        {
            // Setup Prefab
            if (gameObjectWithComps == null)
            {
                if (Assets.SingleShop == null) { Misc.Msg("Cant Setup Prefab, Asset is null!"); return; }
                gameObjectWithComps = GameObject.Instantiate(Assets.SingleShop);
                gameObjectWithComps.hideFlags = HideFlags.HideAndDontSave;
                Shop shop = gameObjectWithComps.AddComponent<Shop>();
                shop.isSetupPrefab = true;
            }
        }

        internal static GameObject Spawn(Vector3 pos, Quaternion rot, string owner = null, string uniqueId = null, bool raiseNetworkEvent = false)
        {
            return null;
        }

        internal static void TryOpenUi()
        {
            if (!LocalPlayer.IsInWorld || LocalPlayer.IsInInventory || PauseMenu.IsActive) { return; }
            Transform transform = LocalPlayer._instance._mainCam.transform;
            RaycastHit raycastHit;
            Physics.Raycast(transform.position, transform.forward, out raycastHit, 5f, LayerMask.GetMask(new string[]
            {
                "Default"
            }));
            if (raycastHit.collider == null) { return; }
            if (raycastHit.collider.transform.root == null) { return; }
            if (string.IsNullOrEmpty(raycastHit.collider.transform.root.name)) { return; }
            //Misc.Msg($"Hit: {raycastHit.collider.transform.root.name}");
            if (raycastHit.collider.transform.root.name.Contains("Shop"))
            {
                GameObject open = raycastHit.collider.transform.root.gameObject;
                Shop controller = open.GetComponent<Shop>();
                if (controller != null)
                {
                    controller.OnInteractButtonPressed();
                }
                else { Misc.Msg("Controller is null!"); }

            }
        }
    }
}
