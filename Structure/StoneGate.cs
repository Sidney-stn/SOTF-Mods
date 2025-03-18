using Il2CppInterop.Runtime;
using RedLoader;
using SonsSdk;
using UnityEngine;


namespace StoneGate.Structure
{
    internal class StoneGate : StructureBase
    {
        //// Singleton instance
        //private static StoneGate _instance;

        //// Private constructor to prevent direct instantiation
        //private StoneGate()
        //{
        //    GameObject setupPrefab = CreateGameObjectToSetupStructure();
        //    // Initialization code if needed
        //    StructureId = 751151;
        //    BlueprintName = "StoneGate";
        //    RegisterInBook = true;
        //    BookPage = null; // Assets.Instance.ConveyorBeltBookPage;
        //    AddComponents = new List<Il2CppSystem.Type> { Il2CppType.Of<Mono.StoneGateMono>() };
        //    BoltSetterComponent = null; //Il2CppType.Of<Network.StoneGateSetter>();
        //    RegisterStructure = true;
        //    AddGrassAndSnow = true;
        //    GrassSize = new Vector3(0.4f, 0.2f, 1.3f);
        //    SnowSize = new Vector3(0.7f, 1, 0.2f);
        //    MaxPlacementAngle = null;
        //    SetupStructure(setupPrefab);
        //}

        //// Public accessor for the singleton instance
        //public static StoneGate Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = new StoneGate();
        //        }
        //        return _instance;
        //    }
        //}

        //private GameObject CreateGameObjectToSetupStructure()
        //{
        //    GameObject main = new GameObject("StoneGate");
        //    main.DontDestroyOnLoad().HideAndDontSave();
        //    GameObject baseChild = new GameObject("Base");
        //    baseChild.transform.SetParent(main.transform);

        //    Bolt.PrefabId stoneStoragePrefab = new Bolt.PrefabId(247);
        //    GameObject ref_singleStone = Bolt.PrefabDatabase.Find(stoneStoragePrefab);
        //    if (ref_singleStone == null)
        //    {
        //        RLog.Error("[StoneGate] [CreateGameObjectToSetupStructure] SingleStone is null");
        //        return null;
        //    }

        //    // Get the reference stone we want to copy
        //    Transform stonePath = ref_singleStone.transform
        //        .FindChild("StoneLayoutGroup")
        //        .GetChild(1)  // StoneLayoutItem
        //        .GetChild(0); // StoneLogSledRender(Clone)

        //    // Instead of instantiating it first, just use it as a template
        //    int neededStones = 24;
        //    for (int i = 0; i < neededStones; i++)
        //    {
        //        GameObject stone = GameObject.Instantiate(stonePath.gameObject);
        //        stone.name = "Stone (640)"; // Rename it immediately

        //        // Remove unnecessary components
        //        GameObject.DestroyImmediate(stone.GetComponent<Endnight.Rendering.AssetReferenceRenderableCollisionLink>());
        //        GameObject.DestroyImmediate(stone.GetComponent<Sons.Items.Core.ItemRenderableTag>());

        //        stone.transform.SetParent(baseChild.transform);
        //        stone.transform.localPosition = new Vector3(0, 0, 0);
        //        stone.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //    }

        //    return main;
        //}
    }
}
