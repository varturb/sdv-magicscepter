using System;
using MagicScepter.Helpers;
using StardewModdingAPI;

namespace MagicScepter.Patches
{
  public class GameOnFadeToBlackCompletePatch
  {
    internal static void GameOnFadeToBlackComplete_Postfix(StardewValley.Object __instance)
    {
      try
      {
        HorseHelper.TryWarpHorse();
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(GameOnFadeToBlackComplete_Postfix)}:\n{ex}", LogLevel.Error);
      }
    }
  }
}
