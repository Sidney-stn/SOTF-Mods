using Il2CppInterop.Runtime;
using RedLoader;
using Sons.Crafting.Structures;
using SonsSdk;
using SonsSdk.Building;
using TheForest.Items.Inventory;
using UnityEngine;

namespace WirelessSignals.Structure
{
    internal class TransmitterSwitch : StructureBase
    {
        internal void Setup()
        {
            structureId = 751122;
            blueprintName = "TransmitterSwitch";
            bookPage = Assets.BookPageSwitch;
            //placerComponent = Il2CppType.Of<Mono.TransmitterSwitch>();
            placerComponent = Il2CppType.Of<Mono.PlaceStructure>();
            SetupStructure(Assets.TransmitterSwitch, completeSetup: CompleteSetup);
        }
        internal override void CompleteSetup(GameObject obj)
        {
            CustomBlueprintManager.TryRegister(new ScrewStructureRegistration(setupGameObject, structureId, blueprintName));
            CustomBlueprintManager.OnCraftingNodeCreated.Subscribe(OnCraftingNodeCreated);
        }
    }
}
