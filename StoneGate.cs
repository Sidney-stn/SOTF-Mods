using Bolt;
using Endnight.Utilities;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using RedLoader;
using Sons.Items;
using Sons.Items.Core;
using Sons.Weapon;
using SonsSdk;
using SonsSdk.Attributes;
using SUI;
using System.Diagnostics.Tracing;
using TheForest;
using UnityEngine;
using static Sons.Input.InputSystem;

namespace StoneGate;

public class StoneGate : SonsMod, IOnAfterSpawnReceiver
{
    public StoneGate()
    {

        // Uncomment any of these if you need a method to run on a specific update loop.
        //OnUpdateCallback = MyUpdateMethod;
        //OnLateUpdateCallback = MyLateUpdateMethod;
        //OnFixedUpdateCallback = MyFixedUpdateMethod;
        //OnGUICallback = MyGUIMethod;

        // Uncomment this to automatically apply harmony patches in your assembly.
        HarmonyPatchAll = true;
        Instance = this;
    }

    public static StoneGate Instance;

    public const int ToolItemId = 751152;

    protected override void OnInitializeMod()
    {
        if (Testing.DedicatedServer.IsDeticatedServer())
        {
            RLog.Msg(System.ConsoleColor.Blue, "[StoneGate] [DEDICATED SERVER] OnInitializeMod");
        }
        // Do your early mod initialization which doesn't involve game or sdk references here
        Config.Init();

        // Load assets
        Assets.Instance.LoadAssets();
        if (Testing.Settings.useFakeItemForTesting == false)
        {
            stoneGateCreatorTexture = AssetLoaders.LoadTexture(Assets.Instance.GetStoneGateToolPath());
            stoneGateCreatorTexture.hideFlags = HideFlags.HideAndDontSave;

            stoneGateOpenCloseIcon = AssetLoaders.LoadTexture(Assets.Instance.GetOpenCloseIconPath());
            stoneGateOpenCloseIcon.hideFlags = HideFlags.HideAndDontSave;
        } else
        {
            stoneGateCreatorTexture = null;
            stoneGateOpenCloseIcon = null;
        }
            

        // Registier Classes In Il2cpp
        ClassInjector.RegisterTypeInIl2Cpp<Mono.StoneGateItemMono>();
        ClassInjector.RegisterTypeInIl2Cpp<Mono.StoneGateStoreMono>();

        // Register network classes
        Network.Manager.Register();

        if (Assets.Instance.IsLoaded())
        {
            Tools.MoveScene.MoveToScene(Assets.Instance.StoneGateTool);  // Move Scene

            stoneGateCreatorHeldPrefab = Assets.Instance.StoneGateTool;
            stoneGateCreatorHeldPrefab.transform.localScale = Vector3.one * 2f;

            Tools.MoveScene.MoveToScene(stoneGateCreatorHeldPrefab);  // Move Scene

            stoneGateCreatorPrefab = stoneGateCreatorHeldPrefab.Instantiate();
            foreach (var renderer in stoneGateCreatorPrefab.GetComponentsInChildren<MeshRenderer>())
            {
                // the renderers need colliders
                if (!renderer.gameObject.TryGetComponent(out Collider _))
                {
                    renderer.gameObject.GetOrAddComponent<BoxCollider>();
                }

                // it doesn't snow in our inventory :)
                renderer.sharedMaterial.SetFloat("_EnableSnow", 0);
            }

            stoneGateCreatorHeldPrefab.AddComponent<Mono.StoneGateItemMono>();

            Tools.MoveScene.MoveToScene(stoneGateCreatorPrefab);

            Misc.Msg("StoneGateTool Set");
        }
        else { RLog.Error("Asset Not Loaded"); }

        if (Testing.DedicatedServer.IsDeticatedServer())
        {
            RLog.Msg(System.ConsoleColor.Blue, "[StoneGate] [DEDICATED SERVER] Running Setup Code in OnInitializeMod, instead of OnSdkInitialized");

            SdkEvents.OnGameActivated.Subscribe(OnFirstGameActivation, unsubscribeOnFirstInvocation: true);

            // Instantiate Ui
            StoneGateToolUI = GameObject.Instantiate(Assets.Instance.StoneGateToolUI);
            if (StoneGateToolUI == null)
            {
                RLog.Error("[StoneGate] StoneGateToolUI Asset Not Found");
            }
            else { 
                RLog.Msg("[StoneGate] StoneGateToolUI Asset Found");
                StoneGateToolUI.SetActive(false);
                Misc.Msg("StoneGateToolUI Set");

                // Move Scene
                Tools.MoveScene.MoveToScene(StoneGateToolUI);
            }

            // Registering Save System
            var manager = new Saving.Manager();
            SonsSaveTools.Register(manager);

            // Ensure CreateGateParent is initialized
            var _ = Objects.CreateGateParent.Instance;
        }
    }

    protected override void OnSdkInitialized()
    {
        if (Testing.DedicatedServer.IsDeticatedServer())
        {
            // This Never Runs On Dedicated Server
            RLog.Msg(System.ConsoleColor.Blue, "[StoneGate] [DEDICATED SERVER] OnSdkInitialized");
        }
        // Do your mod initialization which involves game or sdk references here
        // This is for stuff like UI creation, event registration etc.
        StoneGateUi.Create();

        // Add in-game settings ui for your mod.
        SettingsRegistry.CreateSettings(this, null, typeof(Config));

        SdkEvents.OnGameActivated.Subscribe(OnFirstGameActivation, unsubscribeOnFirstInvocation: true);

        // Instantiate Ui
        StoneGateToolUI = GameObject.Instantiate(Assets.Instance.StoneGateToolUI);
        if (StoneGateToolUI == null)
        {
            RLog.Error("[StoneGate] StoneGateToolUI Asset Not Found");
        } else { 
            RLog.Msg("[StoneGate] StoneGateToolUI Asset Found");
            StoneGateToolUI.SetActive(false);
            Misc.Msg("StoneGateToolUI Set");

            // Move Scene
            Tools.MoveScene.MoveToScene(StoneGateToolUI);
        }

        // Registering Save System
        var manager = new Saving.Manager();
        SonsSaveTools.Register(manager);

        // Ensure CreateGateParent is initialized
        var _ = Objects.CreateGateParent.Instance;

        Tools.MoveScene.MoveToScene(Assets.Instance.StoneGateTool);
        Tools.MoveScene.MoveToScene(stoneGateCreatorPrefab);
    }

    protected override void OnGameStart()
    {
        if (Testing.DedicatedServer.IsDeticatedServer())
        {
            RLog.Msg(System.ConsoleColor.Blue, "[StoneGate] [DEDICATED SERVER] OnGameStart");
        }
        // This is called once the player spawns in the world and gains control.
        // Register Network Event Handlers
        Network.Manager.RegisterEventHandlers();
    }

    private void OnFirstGameActivation()
    {
        if (Testing.DedicatedServer.IsDeticatedServer())
        {
            RLog.Msg(System.ConsoleColor.Blue, "[StoneGate] [DEDICATED SERVER] OnFirstGameActivation");
        }
        if (Testing.Settings.useFakeItemForTesting)
        {
            Misc.Msg("Using Fake Item For Testing");
            GameObject test = DebugTools.CreatePrimitive(PrimitiveType.Sphere, null, Color.red);
            Tools.MoveScene.MoveToScene(test);
            stoneGateCreatorItemData = ItemTools.CreateAndRegisterItem(ToolItemId, "Stone Gate Creator", maxAmount: 1, description: "Create Stone Gates");
            stoneGateCreatorItemData.SetupHeld(EquipmentSlot.RightHand, new[] { AnimatorVariables.molotovHeld });
            stoneGateCreatorPickupPrefab = test;
            stoneGateCreatorItemData._heldPrefab = test.transform;
            Misc.Msg("Fake Item Created");
            Tools.MoveScene.MoveToScene(stoneGateCreatorPickupPrefab);
            Tools.MoveScene.MoveToScene(stoneGateCreatorHeldPrefab);
            return;
        }

        stoneGateCreatorItemData = ItemTools.CreateAndRegisterItem(ToolItemId, "Stone Gate Creator", maxAmount: 1, description: "Create Stone Gates");
        //stoneGateCreatorItemData.SetIcon(stoneGateCreatorTexture);

        // Some parameters for holdable stuff
        stoneGateCreatorItemData.SetupHeld(EquipmentSlot.RightHand, new[] { AnimatorVariables.molotovHeld });

        // use the same model for the pickup
        stoneGateCreatorPickupPrefab = stoneGateCreatorPrefab;

        // also use the same model for the held item
        stoneGateCreatorItemData._heldPrefab = stoneGateCreatorHeldPrefab.transform;

        // Move Scene
        Tools.MoveScene.MoveToScene(stoneGateCreatorPickupPrefab);
        Tools.MoveScene.MoveToScene(stoneGateCreatorHeldPrefab);

    }

    public void OnAfterSpawn()
    {
        if (Testing.DedicatedServer.IsDeticatedServer())
        {
            RLog.Msg(System.ConsoleColor.Blue, "[StoneGate] [DEDICATED SERVER] OnAfterSpawn");
        }
        new ItemTools.ItemBuilder(stoneGateCreatorPrefab, stoneGateCreatorItemData)
            .AddInventoryItem() // add a location in the inventory
            .AddIngredientItem() // add a location on the mat as an ingredient
            .AddCraftingResultItem() // add a location on the mat as a crafting result
            /*.SetupHeld(new(0f, 0f, 0.1f), new(0f, 280f, 70f))*/ // add a location in the hand. This will enable you to hold the item
            .SetupHeld(new(0f, 0f, 0f), new(0f, 0f, 0f))
            .SetupPickup(stoneGateCreatorPickupPrefab) // setup a pickup for the item given a model
            .Recipe // recipe setup
            //.AddIngredient(ItemTools.Identifiers.SolafiteOre, 2) // ingredients of the recipe
            .AddIngredient(ItemTools.Identifiers.Stick, 2)
            .AddIngredient(ItemTools.Identifiers.Rock, 2)
            .AddResult(stoneGateCreatorItemData._id) // resulting item of the recipe
            .Animation(ItemTools.CraftAnimations.CraftedArrows) // crafting animation
            .BuildAndAdd(); // register the recipe

        new ItemTools.RecipeBuilder() // Workaround: in order to be able to add the custom item to the mat it needs to be an ingredient of a recipe
            .AddIngredient(stoneGateCreatorItemData._id, 2)
            .AddResult(ItemTools.Identifiers.Stick)
            .BuildAndAdd();

        RLog.Msg(System.Drawing.Color.SeaGreen, "[ ADDED ITEM: StoneGateTool]");

        //DebugConsole.Instance.SendCommand($"additem {ToolItemId}");
        //DebugConsole.Instance.SendCommand($"removeitem {ToolItemId}");

        // Ensure all critical objects are in the right scene
        EnsureObjectsInCorrectScene();

        // Load Saved Gates
        if (Testing.Settings.logSavingSystem)
            Misc.Msg("[Loading] Processing deferred load.");

        Network.CustomEventHandler.Instance.OnEnterWorld();

        if (Assets.Instance.StoneGateToolUI != null && Assets.Instance.StoneGateToolUI.active)
        {
            Assets.Instance.StoneGateToolUI.SetActive(false);
        }

        if (BoltNetwork.isRunning && BoltNetwork.isClient)
        {
            if (Testing.Settings.logSavingSystem)
                Misc.Msg("[Loading] Skipped Loading StoneGates On Multiplayer Client");
            return;
        }

        // Process all deferred load data, if host
        while (Saving.Load.deferredLoadQueue.Count > 0)
        {
            var obj = Saving.Load.deferredLoadQueue.Dequeue();
            Saving.Load.ProcessLoadData(obj);
        }

    }

    private void EnsureObjectsInCorrectScene()
    {
        Misc.Msg("[StoneGate] Ensuring objects are in correct scene...");

        if (stoneGateCreatorHeldPrefab != null)
        {
            if (stoneGateCreatorHeldPrefab.scene.name != Tools.MoveScene.targetSceneName)
            {
                Misc.Msg($"[StoneGate] Moving stoneGateCreatorHeldPrefab to {Tools.MoveScene.targetSceneName}");
                Tools.MoveScene.MoveToScene(stoneGateCreatorHeldPrefab);
            }
        }
        else
        {
            Misc.Msg("[StoneGate] stoneGateCreatorHeldPrefab is null");
        }

        if (stoneGateCreatorPrefab != null)
        {
            if (stoneGateCreatorPrefab.scene.name != Tools.MoveScene.targetSceneName)
            {
                Misc.Msg($"[StoneGate] Moving stoneGateCreatorPrefab to {Tools.MoveScene.targetSceneName}");
                Tools.MoveScene.MoveToScene(stoneGateCreatorPrefab);
            }
        }
        else
        {
            Misc.Msg("[StoneGate] stoneGateCreatorPrefab is null");
        }

        if (stoneGateCreatorPickupPrefab != null)
        {
            if (stoneGateCreatorPickupPrefab.scene.name != Tools.MoveScene.targetSceneName)
            {
                Misc.Msg($"[StoneGate] Moving stoneGateCreatorPickupPrefab to {Tools.MoveScene.targetSceneName}");
                Tools.MoveScene.MoveToScene(stoneGateCreatorPickupPrefab);
            }
        }
        else
        {
            Misc.Msg("[StoneGate] stoneGateCreatorPickupPrefab is null");
        }

        if (StoneGateToolUI != null)
        {
            if (StoneGateToolUI.scene.name != Tools.MoveScene.targetSceneName)
            {
                Misc.Msg($"[StoneGate] Moving StoneGateToolUI to {Tools.MoveScene.targetSceneName}");
                Tools.MoveScene.MoveToScene(StoneGateToolUI);
            }
        }
        else
        {
            Misc.Msg("[StoneGate] StoneGateToolUI is null");
        }
    }

    internal static Structure.StoneGate stoneGate;

    public static ItemData stoneGateCreatorItemData;
    public static GameObject stoneGateCreatorPrefab;
    public static GameObject stoneGateCreatorHeldPrefab;
    public static GameObject stoneGateCreatorPickupPrefab;
    public static Texture2D stoneGateCreatorTexture;

    internal static GameObject StoneGateToolUI;  // UI For StoneGateTool, Complete or Add Objects

    internal static Texture2D stoneGateOpenCloseIcon;

    internal static bool isStoneGateToolOneTimeUse = true;

}