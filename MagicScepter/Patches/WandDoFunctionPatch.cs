using StardewValley;
using StardewModdingAPI;
using System;

namespace MagicScepter.Patches
{
    public class WandDoFunctionPatch
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;

        internal static void Initialize(IMonitor monitor, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
        }

        internal static bool WandDoFunction_Prefix(StardewValley.Object __instance, GameLocation location, int x, int y, int power, Farmer who)
        {
            try
            {
                LocationDialog.Initialize(Helper);
                LocationDialog.ShowLocationDialog();

                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(WandDoFunction_Prefix)}:\n{ex}", LogLevel.Error);
                return false;
            }
        }
    }
}
