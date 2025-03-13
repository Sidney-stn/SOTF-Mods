using Sons.Gui;
using StoneGate.Mono;
using TheForest.Utils;

namespace StoneGate.Objects
{
    internal static class ActiveItem
    {
        public static StoneGateItemMono active = null;

        public static void OnKeyPress()
        {
            if (LocalPlayer.IsInWorld == false || LocalPlayer.IsInInventory || PauseMenu.IsActive) { return; }
            if (active == null)
            {
                return;
            }
            active.InitHit();
        }
    }
}
