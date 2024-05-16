using StardewValley;
using StardewModdingAPI;
using System;
using MagicScepter.Handlers;

namespace MagicScepter.Patches
{
  public class WandDoFunctionPatch
  {
    internal static bool WandDoFunction_Prefix(StardewValley.Object __instance, GameLocation location, int x, int y, int power, Farmer who)
    {
      try
      {
        if (who.IsLocalPlayer)
        {
          ActionHandler.HandleTeleportAction();
        }

        return false;
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(WandDoFunction_Prefix)}:\n{ex}", LogLevel.Error);
        return false;
      }
    }
  }
}
