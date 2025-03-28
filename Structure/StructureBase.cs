using RedLoader;
using Sons.Crafting.Structures;
using SonsSdk;
using SonsSdk.Building;
using SonsSdk.Networking;
using UnityEngine;

namespace SimpleElevator.Structure
{
    internal class StructureBase
    {
        internal virtual GameObject SetupGameObject { get; set; } = null;
        internal virtual int StructureId { get; set; } = 0;
        internal virtual string BlueprintName { get; set; } = null;
        internal virtual bool RegisterInBook { get; set; } = false;
        /// <summary>
        /// BookPage: Book Page Texture2D, if RegisterInBook is true
        /// </summary>
        internal virtual Texture2D BookPage { get; set; } = null;
        /// <summary>
        /// AddComponents: Add Components To GameObject
        /// </summary>
        internal virtual List<Il2CppSystem.Type> AddComponents { get; set; } = null;
        internal virtual Il2CppSystem.Type BoltSetterComponent { get; set; } = null;
        /// <summary>
        /// AddGrassAndSnow: Adds Structure Into Game via Redloader's CustomBlueprintManager
        /// </summary>
        internal virtual bool RegisterStructure { get; set; } = true;
        /// <summary>
        /// AddGrassAndSnow: Grass Remover And Snow Remover To GameObject
        /// </summary>
        internal virtual bool AddGrassAndSnow { get; set; } = false;
        /// <summary>
        /// GrassSize: Size (Scale) of the grass remover GameObject
        /// </summary>
        internal virtual Vector3? GrassSize { get; set; } = null;
        /// <summary>
        /// SnowSize: Size (Scale) of the snow remover GameObject
        /// </summary>
        internal virtual Vector3? SnowSize { get; set; } = null;

        /// <summary>
        /// MaxPlacementAngle: Maximum angle for placement of the structure
        /// </summary>
        internal virtual float? MaxPlacementAngle { get; set; } = null;

        /// <summary>
        /// SetupStructure: Setup structure, add components, register structure, add grass and snow remover GameObjects
        /// </summary>
        /// <param name="goToInstantiate">GameObject To Instantiate</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        internal virtual void SetupStructure(GameObject goToInstantiate)
        {
            Misc.Msg("[StructureBase] [Setup] Setting up structure");
            if (StructureId == 0) { throw new InvalidOperationException("[StructureBase] [Setup] StructureId Is 0!"); }
            if (string.IsNullOrEmpty(BlueprintName)) { throw new InvalidOperationException("[StructureBase] [Setup] BlueprintName Is Null Or Empty!"); }
            if (goToInstantiate == null) { throw new ArgumentNullException("[StructureBase] [Setup] goToInstantiate Is Null!"); }
            SetupGameObject = GameObject.Instantiate(goToInstantiate);
            if (SetupGameObject == null) { throw new InvalidOperationException("[StructureBase] [Setup] SetupGameObject Is Null!"); }
            SetupGameObject.HideAndDontSave().DontDestroyOnLoad();

            List<Transform> allPlacableChilds = SetupGameObject.transform.GetChild(0).GetChildren();  // GameObject -> Base -> Under Objects
            List<Transform> allPlacableChilds2 = SetupGameObject.transform.GetChild(1).GetChildren();  // GameObject -> Base -> Over Objects
            allPlacableChilds.AddRange(allPlacableChilds2);
            List<Transform> allPlacableChilds3 = SetupGameObject.transform.GetChild(2).GetChildren();  // GameObject -> Base -> Under Objects
            allPlacableChilds.AddRange(allPlacableChilds3);
            for (int i = 0; i < allPlacableChilds.Count; i++)
            {
                if (allPlacableChilds[i] == null) { continue; }

                string name = allPlacableChilds[i].gameObject.name;
                int id = ExtractIdFromName(name);
                if (id == 0) { continue; }
                allPlacableChilds[i].gameObject.AddComponent<StructureCraftingNodeIngredient>().SetId(id);  // Set ID dynamically
            }


            if (AddComponents != null && AddComponents.Count > 0)
            {
                for (int i = 0; i < AddComponents.Count; i++)
                {
                    if (AddComponents[i] == null) { continue; }
                    dynamic comp = SetupGameObject.AddComponent(AddComponents[i]);
                    comp.isSetupPrefab = true;
                }
            }

            if (BoltSetterComponent != null)
            {
                SetupGameObject.AddComponent(BoltSetterComponent);
            }

            if (AddGrassAndSnow)
            {
                CleanGrassAndSnow(null, GrassSize, SnowSize);
            }

            BoltEntity boltEntity = SetupGameObject.AddComponent<BoltEntity>();
            boltEntity.Init(StructureId, BoltFactories.RigidbodyState);
            EntityManager.RegisterPrefab(boltEntity);
            Misc.Msg($"[StructureBase] [Setup] {BlueprintName} BoltEntity Component Added");

            if (RegisterStructure)
            {
                CustomBlueprintManager.TryRegister(new ScrewStructureRegistration(SetupGameObject, StructureId, BlueprintName));
                Misc.Msg($"[StructureBase] [Setup] {BlueprintName} Registered");
            }
            if (RegisterInBook)
            {
                CustomBlueprintManager.OnCraftingNodeCreated.Subscribe(OnCraftingNodeCreated);
                Misc.Msg($"[StructureBase] [Setup] {BlueprintName} Trying To Register In Book...");
            }


        }
        internal virtual void OnCraftingNodeCreated(StructureCraftingNode craftingNode)
        {
            if (RegisterInBook == true || MaxPlacementAngle != null)  // RegisterInBook if true
            {
                if (craftingNode == null) { throw new ArgumentNullException("[StructureBase] [OnCraftingNodeCreated] craftingNode Is Null!"); }
                if (craftingNode.Recipe == null) { throw new ArgumentNullException("[StructureBase] [OnCraftingNodeCreated] craftingNode.Recipe Is Null!"); }
                if (craftingNode.Recipe.Id == StructureId)
                {
                    if (RegisterInBook == true && BookPage != null)
                    {
                        Misc.Msg("[OnCraftingNodeCreated] Creating Book Page");
                        CustomBlueprintManager.CreateBookPage(craftingNode.Recipe, null, BookPage);
                    }
                    else if (RegisterInBook == true && BookPage == null)
                    {
                        RLog.Error("[OnCraftingNodeCreated] Failed To Create Book Page!");
                    }
                    else if (MaxPlacementAngle != null)
                    {
                        craftingNode.Recipe._forceUpAngleThreshold = MaxPlacementAngle.Value;
                    }
                }
            }
        }
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

        internal virtual void CleanGrassAndSnow(GameObject addToObj = null, Vector3? grassSize = null, Vector3? snowSize = null)
        {
            if (addToObj == null) { addToObj = SetupGameObject; }
            if (SetupGameObject == null) { Misc.Msg("[StructureBase] [CleanGrassAndSnow] Can't add to null object"); return; }
            Bolt.PrefabId prefabId = BoltPrefabs.StorageFirewoodStructure;  // Find StorageFirewoodStructure PrefabId for copying GameObjects
            Bolt.PrefabId nullId = new Bolt.PrefabId(0);
            if (prefabId == nullId) { Misc.Msg("[StructureBase] [CleanGrassAndSnow] Can't find prefabId"); return; }
            GameObject obj = Bolt.PrefabDatabase.Find(prefabId);  // Find StorageFirewoodStructure GameObject for copying GameObjects
            if (obj == null) { Misc.Msg("[StructureBase] [CleanGrassAndSnow] Can't find prefabId"); return; }
            GameObject grassRemover = obj.transform.FindChild("GrassRemover").gameObject;  // Find GrassRemover GameObject
            if (grassRemover != null)
            {
                GameObject grassRemoverCopy = GameObject.Instantiate(grassRemover);  // Copy GrassRemover GameObject
                if (grassSize != null) { grassRemoverCopy.transform.localScale = grassSize.Value; }  // Set GrassRemover GameObject scale
                grassRemoverCopy.SetParent(addToObj.transform);  // Set GrassRemover GameObject as child of addToObj
            }
            else
            {
                Misc.Msg("[StructureBase] [CleanGrassAndSnow] Can't find GrassRemover"); return;
            }
            GameObject snowRemover = obj.transform.FindChild("SnowRemover").gameObject;  // Find SnowRemover GameObject
            if (snowRemover != null)
            {
                GameObject snowRemoverCopy = GameObject.Instantiate(snowRemover);  // Copy SnowRemover GameObject
                if (snowSize != null) { snowRemoverCopy.transform.localScale = snowSize.Value; }  // Set SnowRemover GameObject scale
                snowRemoverCopy.SetParent(addToObj.transform);  // Set SnowRemover GameObject as child of addToObj
            }
            else
            {
                Misc.Msg("[StructureBase] [CleanGrassAndSnow] Can't find SnowRemover"); return;
            }
            GameObject structureEnvironmentCleaner = obj.transform.FindChild("StructureEnvironmentCleaner").gameObject;  // Find StructureEnvironmentCleaner GameObject
            if (structureEnvironmentCleaner != null)
            {
                GameObject structureEnvironmentCleanerCopy = GameObject.Instantiate(structureEnvironmentCleaner);  // Copy StructureEnvironmentCleaner GameObject
                structureEnvironmentCleanerCopy.SetParent(addToObj.transform);  // Set StructureEnvironmentCleaner GameObject as child of addToObj
            }
            else
            {
                Misc.Msg("[StructureBase] [CleanGrassAndSnow] Can't find StructureEnvironmentCleaner"); return;
            }
        }
    }
}
