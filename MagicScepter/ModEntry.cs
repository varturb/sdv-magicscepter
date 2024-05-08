using System;
using StardewModdingAPI;
using StardewValley.Tools;
using HarmonyLib;
using MagicScepter.Patches;
using MagicScepter.Multiplayer;

namespace MagicScepter
{
  public class ModEntry : Mod
  {
    public override void Entry(IModHelper helper)
    {
      ModUtility.Initialize(helper, Monitor, ModManifest);

      try
      {
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.Patch(
          original: AccessTools.Method(typeof(Wand), nameof(Wand.DoFunction)),
          prefix: new HarmonyMethod(typeof(WandDoFunctionPatch), nameof(WandDoFunctionPatch.WandDoFunction_Prefix))
        );
      }
      catch (Exception e)
      {
        Monitor.Log($"Issue with Harmony patch: {e}", LogLevel.Error);
        return;
      }

      Helper.Events.Multiplayer.ModMessageReceived += MultiplayerManager.OnModMessageReceived;
      Helper.Events.GameLoop.DayStarted += MultiplayerManager.OnDayStarted;
    }
  }
}
