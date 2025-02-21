using Il2CppInterop.Runtime;
using RedLoader;
using Sons.Crafting.Structures;
using SonsSdk;
using SonsSdk.Building;
using TheForest.Items.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace WirelessSignals.Structure
{
    internal class TransmitterDetector : StructureBase
    {
        internal void Setup()
        {
            structureId = 751124;
            blueprintName = "TransmitterDetector";
            bookPage = null;
            placerComponent = Il2CppType.Of<Mono.PlaceStructure>();
            SetupStructure(Assets.TransmitterDetector, completeSetup: CompleteSetup);
        }
        internal override void CompleteSetup(GameObject obj)
        {
            // Add wire to the structure
            Transform wirePlacement = obj.transform.GetChild(0).GetChild(22);
            if (wirePlacement == null) { throw new InvalidOperationException("[TransmitterSwitch] [CompleteSetup] WirePlacement Is Null!"); }
            GameObject wire = GameObject.Instantiate(ItemTools.GetHeldPrefab(418).gameObject, wirePlacement);
            HeldItemIdentifier itemIdent = wire.GetComponent<HeldItemIdentifier>();
            GameObject.Destroy(itemIdent);
            wire.transform.localScale = new Vector3(5, 3, 5);
            wire.transform.rotation = Quaternion.Euler(90, 0, 0);
            wire.transform.localPosition = new Vector3(0, 0.16f, 0.1f);

            CustomBlueprintManager.TryRegister(new ScrewStructureRegistration(setupGameObject, structureId, blueprintName));
            CustomBlueprintManager.OnCraftingNodeCreated.Subscribe(OnCraftingNodeCreated);
        }
    }
}
