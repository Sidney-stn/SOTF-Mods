using Bolt;
using Endnight.Utilities;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using RedLoader;
using Sons.Items;
using Sons.Items.Core;
using SonsSdk;
using SonsSdk.Attributes;
using SUI;
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
        //HarmonyPatchAll = true;
    }

    protected override void OnInitializeMod()
    {
        // Do your early mod initialization which doesn't involve game or sdk references here
        Config.Init();

        // Load assets
        Assets.Instance.LoadAssets();
        stoneGateCreatorTexture = AssetLoaders.LoadTexture(Assets.Instance.GetStoneGateToolPath());
        stoneGateCreatorTexture.hideFlags = HideFlags.HideAndDontSave;

        // Registier Classes In Il2cpp
        ClassInjector.RegisterTypeInIl2Cpp<Mono.StoneGateMono>();
        ClassInjector.RegisterTypeInIl2Cpp<Mono.StoneGateItemMono>();

        // Register network classes
        Network.Manager.Register();

        if (Assets.Instance.IsLoaded())
        {
            stoneGateCreatorHeldPrefab = Assets.Instance.StoneGateTool;
            stoneGateCreatorHeldPrefab.transform.localScale = Vector3.one * 2f;

            stoneGateCreatorPrefab = stoneGateCreatorHeldPrefab.Instantiate().DontDestroyOnLoad().HideAndDontSave();
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

            Misc.Msg("StoneGateTool Set");
        }
        else { RLog.Error("Asset Not Loaded"); }
        //stoneGate = Structure.StoneGate.Instance;
    }

    protected override void OnSdkInitialized()
    {
        // Do your mod initialization which involves game or sdk references here
        // This is for stuff like UI creation, event registration etc.
        StoneGateUi.Create();

        // Add in-game settings ui for your mod.
        SettingsRegistry.CreateSettings(this, null, typeof(Config));

        SdkEvents.OnGameActivated.Subscribe(OnFirstGameActivation, unsubscribeOnFirstInvocation: true);

    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.
    }

    private void OnFirstGameActivation()
    {
        stoneGateCreatorItemData = ItemTools.CreateAndRegisterItem(751152, "Stone Gate Creator", maxAmount: 1, description: "Create Stone Gates");
        stoneGateCreatorItemData.SetIcon(stoneGateCreatorTexture);

        // Some parameters for holdable stuff
        stoneGateCreatorItemData.SetupHeld(EquipmentSlot.RightHand, new[] { AnimatorVariables.molotovHeld });

        // use the same model for the pickup
        stoneGateCreatorPickupPrefab = stoneGateCreatorPrefab;

        // also use the same model for the held item
        stoneGateCreatorItemData._heldPrefab = stoneGateCreatorHeldPrefab.transform;

    }

    public void OnAfterSpawn()
    {
        new ItemTools.ItemBuilder(stoneGateCreatorPrefab, stoneGateCreatorItemData)
            .AddInventoryItem() // add a location in the inventory
            .AddIngredientItem() // add a location on the mat as an ingredient
            .AddCraftingResultItem() // add a location on the mat as a crafting result
            /*.SetupHeld(new(0f, 0f, 0.1f), new(0f, 280f, 70f))*/ // add a location in the hand. This will enable you to hold the item
            .SetupHeld(new(0f, 0f, 0f), new(0f, 0f, 0f))
            .SetupPickup(stoneGateCreatorPickupPrefab) // setup a pickup for the item given a model
            .Recipe // recipe setup
            .AddIngredient(ItemTools.Identifiers.Stick, 2) // ingredients of the recipe
            .AddResult(stoneGateCreatorItemData._id) // resulting item of the recipe
            .Animation(ItemTools.CraftAnimations.CraftedArrows) // crafting animation
            .BuildAndAdd(); // register the recipe

        new ItemTools.RecipeBuilder() // Workaround: in order to be able to add the custom item to the mat it needs to be an ingredient of a recipe
            .AddIngredient(stoneGateCreatorItemData._id, 2)
            .AddResult(ItemTools.Identifiers.Stick)
            .BuildAndAdd();

        RLog.Msg(System.Drawing.Color.SeaGreen, "[ ADDED ITEM ]");
    }

    internal static Structure.StoneGate stoneGate;

    public static ItemData stoneGateCreatorItemData;
    public static GameObject stoneGateCreatorPrefab;
    public static GameObject stoneGateCreatorHeldPrefab;
    public static GameObject stoneGateCreatorPickupPrefab;
    public static Texture2D stoneGateCreatorTexture;

}