using Il2CppInterop.Runtime;
using RedLoader;
using Sons.Crafting.Structures;
using SonsSdk;
using SonsSdk.Building;
using UnityEngine;

namespace Banking.Structure
{
    internal class Setup
    {
        internal static GameObject atmStructure;
        internal const int atmStructureId = 751200;
        internal static void Crafting()
        {
            Misc.Msg("[Structure Setup] Crafting");
            if (Assets.ATM == null) { Misc.Msg("Cant Setup ATM Placer Prefab, Sign Asset is null!"); return; }

            atmStructure = GameObject.Instantiate(Assets.ATM);
            atmStructure.hideFlags = HideFlags.HideAndDontSave;

            if (atmStructure == null) { Misc.Msg("[Setup] atmStructure == null!"); return; }

            List<Transform> allPlacableChilds = atmStructure.transform.GetChild(1).GetChildren();  // ATM -> Base -> Under Objects

            for (int i = 0; i < allPlacableChilds.Count; i++)
            {
                if (allPlacableChilds[i] == null) { continue; }

                string name = allPlacableChilds[i].gameObject.name;
                int id = ExtractIdFromName(name);
                if (id == 0) { continue; }
                allPlacableChilds[i].gameObject.AddComponent<StructureCraftingNodeIngredient>().SetId(id);  // Set ID dynamically
            }

            int ExtractIdFromName(string name)
            {
                int startIndex = name.IndexOf('(') + 1;
                int endIndex = name.IndexOf(')');
                if (startIndex > 0 && endIndex > startIndex)
                {
                    string idString = name.Substring(startIndex, endIndex - startIndex);
                    if (int.TryParse(idString, out int id))
                    {
                        return id;
                    }
                }
                return 0;  // Default ID if parsing fails
            }


            CustomBlueprintManager.TryRegister(new ScrewStructureRegistration(atmStructure, atmStructureId, "ATMRecipie"));
            CustomBlueprintManager.OnCraftingNodeCreated.Subscribe(OnCraftingNodeCreated);

        }

        internal static void OnCraftingNodeCreated(StructureCraftingNode arg1)
        {
            if (arg1.Recipe.Id == atmStructureId)
            {
                Misc.Msg("Adding OnStructureComplete Event");
                // Store the GameObject reference
                GameObject gameObject = arg1.gameObject;
                GameObject builtPrefab = arg1.Recipe._builtPrefab;
                Mono.NewATM NewATM = builtPrefab.AddComponent<Mono.NewATM>();
                NewATM.IsPlaceHolder = true;

                arg1.StructureCraftingSystem.OnStructureComplete += DelegateSupport.ConvertDelegate<Il2CppSystem.Action>(
                    new System.Action(() => OnStructureCompleted(gameObject, builtPrefab))
                );

                //arg1.Recipe.SetCompletedStructurePrefab(signStructurePrefab);
                arg1.Recipe._allowsTreePlacement = true;
                arg1.Recipe._allowsNonTreePlacement = true;
                arg1.Recipe._alignToSurface = false;
                //arg1.Recipe._initialPlacementRotationOffset = new Vector3(0, 270f, 0);


                if (Assets.ATMBookPage != null)
                {
                    Misc.Msg("Creating Book Page");
                    CustomBlueprintManager.CreateBookPage(arg1.Recipe, null, Assets.ATMBookPage);
                }
                else
                {
                    RLog.Error("[Adding Book Page] [Signs] Failed To Create Book Page!");
                }
            }
        }

        private static void OnStructureCompleted(UnityEngine.GameObject gameObject, GameObject builtPrefab)
        {
            Misc.Msg($"OnStructureCompleted for GameObject: {gameObject.name}");
            // You can now access the GameObject and its components here
        }


    }
}
