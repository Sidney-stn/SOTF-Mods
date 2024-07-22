using Construction;
using Construction.Utils;
using Endnight.Utilities;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using RedLoader;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;

namespace StructureDamageViewer;

public class StructureDamageViewer : SonsMod
{
    public StructureDamageViewer()
    {

        // Uncomment any of these if you need a method to run on a specific update loop.
        OnUpdateCallback = OnUpdate;
        //OnLateUpdateCallback = MyLateUpdateMethod;
        //OnFixedUpdateCallback = MyFixedUpdateMethod;
        //OnGUICallback = MyGUIMethod;

        // Uncomment this to automatically apply harmony patches in your assembly.
        //HarmonyPatchAll = true;
    }

    protected override void OnInitializeMod()
    {
        // Do your early mod initialization which doesn't involve game or sdk references here
        Config.Init();
    }

    protected override void OnSdkInitialized()
    {
        // Do your mod initialization which involves game or sdk references here
        // This is for stuff like UI creation, event registration etc.
        StructureDamageViewerUi.Create();

        // Add in-game settings ui for your mod.
        // SettingsRegistry.CreateSettings(this, null, typeof(Config));
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.

    }

    public void OnUpdate()
    {

    }

    public void Trigger()
    {
        Misc.Msg("[StructureDamageViewer] Trigger()");
        Vector3 position = LocalPlayer.MainCamTr.position;
        Vector3 to = position + LocalPlayer.MainCamTr.forward * 4f;

        Transform transform = LocalPlayer._instance._mainCam.transform;
        RaycastHit raycastHit;
        Physics.Raycast(transform.position, transform.forward, out raycastHit, 25f, Layers.PropMask);

        RaycastHit[] hits;
        int num = RaycastHelper.LineCastAllNonAlloc(position, to, 0.5f, out hits, Layers.PropMask, QueryTriggerInteraction.Collide);

        if (num == 0)
        {
            Misc.Msg("[StructureDamageViewer] Return Trigger()");
            return;
        }

        for (int i = 0; i < num; i++)
        {
            Structure structure;
            if (hits[i].collider.gameObject.TryGetComponentInParent(out structure))
            {
                float num2;
                float num3;
                StructureDestructionManager.TryGetStructureHealth(structure, out num2, out num3);
                string name = structure.name;
                float structuralResistanceFactor = structure.GetStructuralResistanceFactor(6);
                Misc.Msg($"{string.Format("Resistance Factor={0}\n", structuralResistanceFactor)} {string.Format("Health={0}/{1}", num2, num3)} ");
            }
        }

    }
}