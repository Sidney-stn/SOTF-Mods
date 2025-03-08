using Il2CppInterop.Runtime;
using RedLoader;
using Sons.Crafting.Structures;
using SonsSdk;
using SonsSdk.Building;
using SonsSdk.Networking;
using UnityEngine;

namespace Signs.Structure
{
    internal class Setup
    {
        internal static GameObject signStructure;
        internal const int signStructureId = 751100;
        //internal static bool swapModel = false;
        private static bool isSetup = false;

        internal static void Crafting()
        {
            if (isSetup) { return; }
            isSetup = true;

            Misc.Msg("[Structure Setup] Crafting");
            if (Assets.SignObj == null)
            {
                signStructure = new GameObject("Sign");
                // Add Childs
                GameObject goParent = new GameObject("LogFront");
                goParent.SetParent(signStructure.transform);
                GameObject go1 = new GameObject("LogPlank (576)");
                GameObject go2 = new GameObject("Stick (392)");
                GameObject go3 = new GameObject("Stick (392)");
                GameObject go4 = new GameObject("Stick (392)");
                GameObject go5 = new GameObject("Stick (392)");
                GameObject go6 = new GameObject("Stick (392)");
                go1.SetParent(goParent.transform);
                go2.SetParent(goParent.transform);
                go3.SetParent(goParent.transform);
                go4.SetParent(goParent.transform);
                go5.SetParent(goParent.transform);
                go6.SetParent(goParent.transform);

                signStructure.hideFlags = HideFlags.HideAndDontSave;
                //swapModel = true;

                Misc.Msg("[Setup] Created Fake Sign Since Assets.SignObj Is Null");
            } 
            else
            {
                signStructure = GameObject.Instantiate(Assets.SignObj);
                signStructure.hideFlags = HideFlags.HideAndDontSave;
            }

            

            if (signStructure == null) { Misc.Msg("[Setup] signStructure == null!"); return; }

            GameObject logPlank = signStructure.transform.GetChild(0).GetChild(0).gameObject;
            GameObject stick1 = signStructure.transform.GetChild(0).GetChild(1).gameObject;
            GameObject stick2 = signStructure.transform.GetChild(0).GetChild(2).gameObject;
            GameObject stick3 = signStructure.transform.GetChild(0).GetChild(3).gameObject;
            GameObject stick4 = signStructure.transform.GetChild(0).GetChild(4).gameObject;
            GameObject stick5 = signStructure.transform.GetChild(0).GetChild(5).gameObject;

            if (logPlank != null) { logPlank.AddComponent<StructureCraftingNodeIngredient>().SetId(576); } else { Misc.Msg("logPlank == null"); }
            if (stick1 != null) { stick1.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick1 == null"); }
            if (stick2 != null) { stick2.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick2 == null"); }
            if (stick3 != null) { stick3.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick3 == null"); }
            if (stick4 != null) { stick4.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick4 == null"); }
            if (stick5 != null) { stick5.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick4 == null"); }

            Mono.SignController signMono = signStructure.AddComponent<Mono.SignController>();
            signMono.isSetupPrefab = true;
            Mono.DestroyOnC destroyOnC = signStructure.AddComponent<Mono.DestroyOnC>();

            // Setup Bolt
            signStructure.AddComponent<Network.SignSetter>();

            BoltEntity bolt = signStructure.AddComponent<BoltEntity>();
            bolt.Init(signStructureId, BoltFactories.RigidbodyState);
            EntityManager.RegisterPrefab(bolt);

            CustomBlueprintManager.TryRegister(new ScrewStructureRegistration(signStructure, signStructureId, "SignRecipie"));
            CustomBlueprintManager.OnCraftingNodeCreated.Subscribe(OnCraftingNodeCreated);

        }

        //internal static void Crafting()
        //{
        //    Misc.Msg("[Structure Setup] Crafting");
        //    if (Assets.SignObj == null) { Misc.Msg("Cant Setup Sign Prefab, Sign Asset is null!"); return; }

        //    signStructure = GameObject.Instantiate(Assets.SignObj);
        //    signStructure.hideFlags = HideFlags.HideAndDontSave;

        //    if (signStructure == null) { Misc.Msg("[Setup] signStructure == null!"); return; }

        //    GameObject logPlank = signStructure.transform.GetChild(0).GetChild(0).gameObject;
        //    GameObject stick1 = signStructure.transform.GetChild(0).GetChild(1).gameObject;
        //    GameObject stick2 = signStructure.transform.GetChild(0).GetChild(2).gameObject;
        //    GameObject stick3 = signStructure.transform.GetChild(0).GetChild(3).gameObject;
        //    GameObject stick4 = signStructure.transform.GetChild(0).GetChild(4).gameObject;
        //    GameObject stick5 = signStructure.transform.GetChild(0).GetChild(5).gameObject;

        //    if (logPlank != null) { logPlank.AddComponent<StructureCraftingNodeIngredient>().SetId(576); } else { Misc.Msg("logPlank == null"); }
        //    if (stick1 != null) { stick1.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick1 == null"); }
        //    if (stick2 != null) { stick2.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick2 == null"); }
        //    if (stick3 != null) { stick3.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick3 == null"); }
        //    if (stick4 != null) { stick4.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick4 == null"); }
        //    if (stick5 != null) { stick5.AddComponent<StructureCraftingNodeIngredient>().SetId(392); } else { Misc.Msg("stick4 == null"); }

        //    Mono.SignController signMono = signStructure.AddComponent<Mono.SignController>();
        //    signMono.isSetupPrefab = true;
        //    Mono.DestroyOnC destroyOnC = signStructure.AddComponent<Mono.DestroyOnC>();

        //    // Setup Bolt
        //    signStructure.AddComponent<Network.SignSetter>();

        //    BoltEntity bolt = signStructure.AddComponent<BoltEntity>();
        //    bolt.Init(signStructureId, BoltFactories.RigidbodyState);
        //    EntityManager.RegisterPrefab(bolt);

        //    CustomBlueprintManager.TryRegister(new ScrewStructureRegistration(signStructure, signStructureId, "SignRecipie"));
        //    CustomBlueprintManager.OnCraftingNodeCreated.Subscribe(OnCraftingNodeCreated);

        //}

        internal static void OnCraftingNodeCreated(StructureCraftingNode arg1)
        {
            Misc.Msg($"[Setup] OnCraftingNodeCreated: {arg1}, Name: {arg1.name} {arg1.Recipe.Id}");
            if (arg1.Recipe.Id == signStructureId)
            {
                Misc.Msg("Adding OnStructureComplete Event");
                // Store the GameObject reference
                //GameObject gameObject = arg1.gameObject;
                //GameObject builtPrefab = arg1.Recipe._builtPrefab;

                //arg1.StructureCraftingSystem.OnStructureComplete += DelegateSupport.ConvertDelegate<Il2CppSystem.Action>(
                //    new System.Action(() => OnStructureCompleted(gameObject, builtPrefab))
                //);

                //arg1.Recipe.SetCompletedStructurePrefab(signStructurePrefab);
                arg1.Recipe._allowsTreePlacement = true;
                arg1.Recipe._allowsNonTreePlacement = true;
                arg1.Recipe._alignToSurface = false;
                arg1.Recipe._initialPlacementRotationOffset = new Vector3(0, 270f, 0);
                //arg1.Recipe._placeMode = StructureRecipe.PlaceModeType.Tree;
                //arg1.Recipe._anchor = StructureRecipe.AnchorType.;
                //arg1.Recipe._relocateMode = StructureRecipe.RelocateModeType.Relocate;


                if (Assets.BookPageSign != null)
                {
                    Misc.Msg("Creating Book Page");
                    CustomBlueprintManager.CreateBookPage(arg1.Recipe, null, Assets.BookPageSign);
                } else
                {
                    RLog.Error("[Adding Book Page] [Signs] Failed To Create Book Page!");
                }

                
            }
        }

        //private static void OnStructureCompleted(UnityEngine.GameObject gameObject, GameObject builtPrefab)
        //{
        //    Misc.Msg($"OnStructureCompleted for GameObject: {gameObject.name}");
        //    // You can now access the GameObject and its components here
        //}


    }
}
