﻿using StardewValley;
using StardewModdingAPI;
using System;

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
          ActionHandler.HandleWarpAction();
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
