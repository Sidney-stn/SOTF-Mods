using SonsSdk;
using TheForest.Utils;

namespace Currency.LiveData
{
    internal class LocalPlayerData
    {
        public static int? GetLocalPlayerCurrency()
        {
            if (LocalPlayer.GameObject != null && LocalPlayer.IsInWorld)
            {
                int cash = LocalPlayer.Inventory.AmountOf(496);
                return cash;
            }
            return null;
        }

        internal static void AddCashToLocalPlayer(int amount)
        {
            if (LocalPlayer.GameObject != null && LocalPlayer.IsInWorld)
            {
                LocalPlayer.Inventory.AddItem(496, amount, true);
                Misc.Msg($"Added {amount} cash to local player");
            }
        }

        internal static void RemoveCashFromLocalPlayer(int amount)
        {
            if (LocalPlayer.GameObject != null && LocalPlayer.IsInWorld)
            {
                if (LocalPlayer.Inventory.AmountOf(496) < amount)
                {
                    Misc.Msg("Not enough cash to remove");
                    SonsTools.ShowMessage("Not enough cash to remove", 5);
                    return;
                }
                LocalPlayer.Inventory.RemoveItem(496, amount, true);
                Misc.Msg($"Removed {amount} cash from local player");
            }
        }
    }
}
