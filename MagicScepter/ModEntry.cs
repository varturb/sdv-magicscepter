using System;
using StardewModdingAPI;
using StardewValley.Tools;
using HarmonyLib;
using MagicScepter.Patches;
using MagicScepter.Mods;
using MagicScepter.WarpLocations;

namespace MagicScepter
{
  public class ModEntry : Mod
  {
    public override void Entry(IModHelper helper)
    {
      try
      {
        ModManager.Initialize(Helper);
        ResponseManager.Initialize(Helper);
        WandDoFunctionPatch.Initialize(Monitor, Helper);

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
    }
  }
}
