using Sons.Crafting.Structures;
using SonsSdk;
using UnityEngine;

namespace WirelessSignals.Structure
{
    internal abstract class StructureBase
    {
        internal virtual GameObject setupGameObject { get; set; }
        internal virtual int structureId { get; set; }
        internal virtual string blueprintName { get; set; }
        internal virtual Texture2D bookPage { get; set; }
        internal abstract void OnCraftingNodeCreated(StructureCraftingNode arg1);

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

            

            // Configure components if provided
            completeSetup?.Invoke(setupGameObject);
        }

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
