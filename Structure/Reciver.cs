using RedLoader;
using Sons.Crafting.Structures;
using SonsSdk.Building;
using UnityEngine;

namespace WirelessSignals.Structure
{
    internal class Reciver : StructureBase
    {
        internal void Setup()
        {
            structureId = 751120;
            blueprintName = "Reciver";
            bookPage = null;
            SetupStructure(Assets.Reciver, completeSetup: CompleteSetup);
        }
        internal override void CompleteSetup(GameObject obj)
        {
            CustomBlueprintManager.TryRegister(new ScrewStructureRegistration(setupGameObject, structureId, blueprintName));
            CustomBlueprintManager.OnCraftingNodeCreated.Subscribe(OnCraftingNodeCreated);
        }

        internal override void OnCraftingNodeCreated(StructureCraftingNode arg1)
        {
            if (arg1.Recipe.Id == structureId)
            {
                GameObject builtPrefab = arg1.Recipe._builtPrefab;
                // Add Placer Component


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
    }
}
