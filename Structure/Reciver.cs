using Il2CppInterop.Runtime;
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
            bookPage = Assets.BookPageReciver;
            placerComponent = Il2CppType.Of<Mono.PlaceStructure>();
            boltComponentToAdd = Il2CppType.Of<Network.Reciver.ReciverSetter>();
            setupBoltEntity = true;
            SetupStructure(Assets.Reciver, completeSetup: CompleteSetup);
        }
        internal override void CompleteSetup(GameObject obj)
        {
            CustomBlueprintManager.TryRegister(new ScrewStructureRegistration(setupGameObject, structureId, blueprintName));
            CustomBlueprintManager.OnCraftingNodeCreated.Subscribe(OnCraftingNodeCreated);
        }
    }
}
