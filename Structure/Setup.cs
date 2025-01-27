using Il2CppInterop.Runtime;
using Sons.Crafting.Structures;
using SonsSdk.Building;
using UnityEngine;

namespace Signs.Structure
{
    internal class Setup
    {
        internal static GameObject signStructure;
        internal const int signStructureId = 751100;
        internal static void Crafting()
        {
            Misc.Msg("[Structure Setup] Crafting");
            if (Assets.SignObj == null) { Misc.Msg("Cant Setup Sign Prefab, Sign Asset is null!"); return; }

            signStructure = GameObject.Instantiate(Assets.SignObj);
            signStructure.hideFlags = HideFlags.HideAndDontSave;

            Mono.SignController signMono = signStructure.AddComponent<Mono.SignController>();
            Mono.DestroyOnC destroyOnC = signStructure.AddComponent<Mono.DestroyOnC>();

            if (signStructure == null) { Misc.Msg("[Setup] signStructure == null!"); return; }

            GameObject logPlank = signStructure.transform.GetChild(0).GetChild(0).gameObject;
            GameObject stick1 = signStructure.transform.GetChild(0).GetChild(1).gameObject;
            GameObject stick2 = signStructure.transform.GetChild(0).GetChild(2).gameObject;
            GameObject stick3 = signStructure.transform.GetChild(0).GetChild(3).gameObject;
            GameObject stick4 = signStructure.transform.GetChild(0).GetChild(4).gameObject;

            if (logPlank != null) { logPlank.AddComponent<StructureCraftingNodeIngredient>().SetId(576); } else { Misc.Msg("logPlank == null"); }
            if (stick1 != null) { stick1.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick1 == null"); }
            if (stick2 != null) { stick2.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick2 == null"); }
            if (stick3 != null) { stick3.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick3 == null"); }
            if (stick4 != null) { stick4.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick4 == null"); }


            CustomBlueprintManager.TryRegister(new ScrewStructureRegistration(signStructure, signStructureId, "SignRecipie"));
            CustomBlueprintManager.OnCraftingNodeCreated.Subscribe(OnCraftingNodeCreated);
        }

        internal static void OnCraftingNodeCreated(StructureCraftingNode arg1)
        {
            Misc.Msg($"[Setup] OnCraftingNodeCreated: {arg1}, Name: {arg1.name} {arg1.Recipe.Id}");
            if (arg1.Recipe.Id == signStructureId)
            {
                Misc.Msg("Adding OnStructureComplete Event");
                arg1.StructureCraftingSystem.OnStructureComplete += DelegateSupport.ConvertDelegate<Il2CppSystem.Action>(
                    new System.Action(OnStructureCompleted)
                );

            }
        }

        private static void OnStructureCompleted()
        {
            Misc.Msg("OnStructureCompleted");
        }


    }
}
