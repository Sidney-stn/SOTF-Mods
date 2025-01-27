using Il2CppInterop.Runtime;
using Sons.Crafting.Structures;
using SonsSdk.Building;
using UnityEngine;

namespace Signs.Structure
{
    internal class Setup
    {
        internal static GameObject signStructure;
        internal static GameObject signStructurePrefab;
        internal const int signStructureId = 751100;
        internal static void Crafting()
        {
            Misc.Msg("[Structure Setup] Crafting");
            if (Assets.SignObj == null) { Misc.Msg("Cant Setup Sign Prefab, Sign Asset is null!"); return; }

            signStructure = GameObject.Instantiate(Assets.SignObj);
            signStructure.hideFlags = HideFlags.HideAndDontSave;

            //Mono.SignController signMono = signStructure.AddComponent<Mono.SignController>();
            //Mono.DestroyOnC destroyOnC = signStructure.AddComponent<Mono.DestroyOnC>();

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

            // Setup The In World Built Prefab
            PlacementPrefab();
        }

        private static void PlacementPrefab()
        {
            signStructurePrefab = GameObject.Instantiate(Assets.SignObj);
            signStructurePrefab.hideFlags = HideFlags.HideAndDontSave;
            Mono.NewSign newSign = signStructurePrefab.AddComponent<Mono.NewSign>();
            newSign.IsPlaceHolder = true;
        }

        internal static void OnCraftingNodeCreated(StructureCraftingNode arg1)
        {
            Misc.Msg($"[Setup] OnCraftingNodeCreated: {arg1}, Name: {arg1.name} {arg1.Recipe.Id}");
            if (arg1.Recipe.Id == signStructureId)
            {
                Misc.Msg("Adding OnStructureComplete Event");
                // Store the GameObject reference
                var gameObject = arg1.gameObject;

                arg1.StructureCraftingSystem.OnStructureComplete += DelegateSupport.ConvertDelegate<Il2CppSystem.Action>(
                    new System.Action(() => OnStructureCompleted(gameObject))
                );

                arg1.Recipe.SetCompletedStructurePrefab(signStructurePrefab);
            }
        }

        private static void OnStructureCompleted(UnityEngine.GameObject gameObject)
        {
            Misc.Msg($"OnStructureCompleted for GameObject: {gameObject.name}");
            // You can now access the GameObject and its components here
            //GameObject signChild = gameObject.transform.GetChild(2).gameObject;
            //if (signChild != null)
            //{
            //    Mono.SignController signController = signChild.AddComponent<Mono.SignController>();
            //    Mono.DestroyOnC destroyOnC = signChild.AddComponent<Mono.DestroyOnC>();

            //    signController.SetLineText(1, $"Press {Config.ToggleMenuKey.Value.ToUpper()}");
            //    signController.SetLineText(2, "To Edit");
            //    signController.SetLineText(3, "Sign");
            //    signController.SetLineText(4, "");

            //    string uniqueId = Guid.NewGuid().ToString();

            //    signController.UniqueId = uniqueId;

            //    Saving.Load.ModdedSigns.Add(signChild);
            //    Prefab.SignPrefab.spawnedSigns.Add(signController.UniqueId, signChild);

            //    if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
            //    {
            //        Misc.Msg("Multiplayer Sign Spawned");
            //        (ulong steamId, string stringSteamId) = Misc.MySteamId();
            //        SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SpawnSingeSign
            //        {
            //            Vector3Position = Network.CustomSerializable.Vector3ToString((Vector3)signController.GetPos()),
            //            QuaternionRotation = Network.CustomSerializable.QuaternionToString((Quaternion)signController.GetCurrentRotation()),
            //            UniqueId = uniqueId,
            //            Sender = stringSteamId,
            //            SenderName = Misc.GetLocalPlayerUsername(),
            //            Line1Text = signController.GetLineText(1),
            //            Line2Text = signController.GetLineText(2),
            //            Line3Text = signController.GetLineText(3),
            //            Line4Text = signController.GetLineText(4),
            //            ToSteamId = "None"
            //        });
            //    }
            //} else { Misc.Msg("[OnStructureCompleted] signChild == null"); }
        }


    }
}
