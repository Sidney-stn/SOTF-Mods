using Endnight.Types;
using Il2CppInterop.Runtime;
using RedLoader;
using Sons.Crafting.Structures;
using SonsSdk;
using SonsSdk.Building;
using SonsSdk.Networking;
using UnityEngine;

namespace WirelessSignals.Structure
{
    internal abstract class StructureBase
    {
        internal virtual GameObject setupGameObject { get; set; }
        internal virtual int structureId { get; set; }
        internal virtual string blueprintName { get; set; }
        internal virtual Texture2D bookPage { get; set; }
        internal virtual Il2CppSystem.Type placerComponent { get; set; }
        internal virtual List<Il2CppSystem.Type> boltComponentsToAdd { get; set; }  // For Adding Multiple Components
        internal virtual Il2CppSystem.Type boltComponentToAdd { get; set; }  // For Adding Single Component
        internal virtual bool setupBoltEntity { get; set; }
        internal virtual void OnCraftingNodeCreated(StructureCraftingNode arg1)
        {
            if (arg1.Recipe.Id == structureId)
            {
                Misc.Msg("[StructureBase] [OnCraftingNodeCreated] Crafting Node Created");
                GameObject builtPrefab = arg1.Recipe._builtPrefab;
                if (placerComponent != null)
                {
                    Misc.Msg("[StructureBase] [OnCraftingNodeCreated] Adding Placer Component");
                    var placerComponentVar = builtPrefab.AddComponent(placerComponent);
                    if (placerComponentVar == null) { throw new InvalidOperationException("[StructureBase] [OnCraftingNodeCreated] Placer Component Is Null!"); }
                    // Try direct property or field setting
                    dynamic dynamicComponent = placerComponentVar;
                    dynamicComponent.isSetupPrefab = true;
                    dynamicComponent.structureName = blueprintName;
                    if (!setupBoltEntity)
                    {
                        dynamicComponent.destroyBoltEntity = true;
                    }

                    //arg1.StructureCraftingSystem.OnStructureComplete += DelegateSupport.ConvertDelegate<Il2CppSystem.Action>(
                    //new System.Action(() => OnStructureCompleted(arg1.gameObject, builtPrefab)));

                }

                if (bookPage != null)
                {
                    Misc.Msg("[Reciver] [OnCraftingNodeCreated] Creating Book Page");
                    CustomBlueprintManager.CreateBookPage(arg1.Recipe, null, bookPage);
                }
                else
                {
                    RLog.Error("[Reciver] [OnCraftingNodeCreated] Failed To Create Book Page!");
                }
            }
        }

        internal virtual void SetupStructure(GameObject goToInstantiate, Action<GameObject> completeSetup = null)
        {
            Misc.Msg("[StructureBase] [Setup] Setting up structure");
            if (goToInstantiate == null) { throw new ArgumentNullException("[StructureBase] [Setup] goToInstantiate Is Null!"); }
            setupGameObject = GameObject.Instantiate(goToInstantiate);
            if (setupGameObject == null) { throw new InvalidOperationException("[StructureBase] [Setup] setupGameObject Is Null!"); }
            setupGameObject.HideAndDontSave().DontDestroyOnLoad();

            List<Transform> allPlacableChilds = setupGameObject.transform.GetChild(0).GetChildren();  // GameObject -> Base -> Under Objects

            for (int i = 0; i < allPlacableChilds.Count; i++)
            {
                if (allPlacableChilds[i] == null) { continue; }

                string name = allPlacableChilds[i].gameObject.name;
                int id = ExtractIdFromName(name);
                if (id == 0) { continue; }
                allPlacableChilds[i].gameObject.AddComponent<StructureCraftingNodeIngredient>().SetId(id);  // Set ID dynamically
            }

            if (setupBoltEntity)
            {
                
                if (boltComponentsToAdd != null)
                {
                    foreach (var componentType in boltComponentsToAdd)
                    {
                        setupGameObject.AddComponent(componentType);
                    }
                }
                if (boltComponentToAdd != null)
                {
                    setupGameObject.AddComponent(boltComponentToAdd);
                }
                Misc.Msg($"[StructureBase] [Setup] {blueprintName} - BoltEntity Custom Components Added");

                // Add Network Owner Component
                //setupGameObject.AddComponent<Mono.NetworkOwner>().isSetupPrefab = true;
                //Misc.Msg($"[StructureBase] [Setup] {blueprintName} NetworkOwner Component Added", true);
                setupGameObject.AddComponent<Network.Sync.NetworkOwnerSetter>();

                BoltEntity boltEntity = setupGameObject.AddComponent<BoltEntity>();
                boltEntity.Init(structureId, BoltFactories.RigidbodyState);
                EntityManager.RegisterPrefab(boltEntity);
                Misc.Msg($"[StructureBase] [Setup] {blueprintName} BoltEntity Component Added", true);
                
                
            }

            // Configure components if provided
            completeSetup?.Invoke(setupGameObject);
        }

        //internal virtual void OnStructureCompleted(UnityEngine.GameObject gameObject, GameObject builtPrefab)
        //{
        //    Misc.Msg($"OnStructureCompleted for GameObject: {gameObject.name}");
        //    // You can now access the GameObject and its components here
        //}

        internal abstract void CompleteSetup(GameObject obj);

        internal int ExtractIdFromName(string name)
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
    }
}
