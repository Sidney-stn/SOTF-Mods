using Construction;
using Construction.Utils;
using Endnight.Utilities;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using RedLoader;
using Sons.Gui;
using Sons.Weapon;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;

namespace StructureDamageViewer;

public class StructureDamageViewer : SonsMod
{
    public StructureDamageViewer()
    {

        // Uncomment any of these if you need a method to run on a specific update loop.
        //OnUpdateCallback = OnUpdate;
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
        localPlayerTrackMono = LocalPlayer.GameObject.AddComponent<LocalPlayerTrackMono>();
    }

    // Stores Instance
    private LocalPlayerTrackMono localPlayerTrackMono = null;

    public void Trigger2()
    {

        if (!LocalPlayer.IsInWorld || LocalPlayer.IsInInventory || PauseMenu.IsActive) { Misc.Msg("Requriements failed retuned"); return; }
        Collider[] hitColliders = Physics.OverlapSphere(LocalPlayer.Transform.position, Config.StructureDamageViewerScanDistance.Value, LayerMask.GetMask(new string[]
            {
                "Prop"
            }));
        foreach (var hitCollider in hitColliders)
        {
            //Misc.Msg($"Hit: {hitCollider.gameObject.name}"); // For Eleveted Debugging
            GameObject currentHit;
            if (hitCollider.gameObject.name.Contains("Log")) { currentHit = hitCollider.gameObject; }
            else { currentHit = null; }
            if (currentHit != null)
            {
                Misc.Msg($"Detected: {hitCollider.gameObject.name}, Root: {hitCollider.transform.root.gameObject.name}");
                SonsTools.ShowMessage($"Detected: {hitCollider.gameObject.name}, Root: {hitCollider.transform.root.gameObject.name}");
                try
                {
                    Structure structure;
                    if (currentHit.TryGetComponentInParent(out structure))
                    {
                        Misc.Msg($"[StructureDamageViewer] Found structure: {structure.name}");
                        float num2;
                        float num3;
                        bool healthRetrieved = StructureDestructionManager.TryGetStructureHealth(structure, out num2, out num3);

                        if (healthRetrieved)
                        {
                            float structuralResistanceFactor = structure.GetStructuralResistanceFactor(6);
                            Misc.Msg($"{string.Format("Resistance Factor={0}\n", structuralResistanceFactor)} {string.Format("Health={0}/{1}", num2, num3)} ");
                        }
                        else
                        {
                            Misc.Msg("[StructureDamageViewer] Could not retrieve health for structure");
                        }
                    }
                    else
                    {
                        Misc.Msg($"[StructureDamageViewer] No structure found in hit");
                    }
                    continue;
                }
                catch (Exception e)
                {
                    RLog.Error($"Error while finding Damaged Objects. Error: {e}");
                    continue;
                }
            }
        }
    }
}