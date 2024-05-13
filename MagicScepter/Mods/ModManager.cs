namespace MagicScepter.Mods
{
  public static class ModManager
  {
    internal static bool IsModLoaded(SupportedMod mod)
    {
      return mod switch
      {
        SupportedMod.MultipleMiniObelisks => IsModLoaded("PeacefulEnd.MultipleMiniObelisks"),
        SupportedMod.RidgesideVillage => IsModLoaded("Rafseazz.RidgesideVillage"),
        SupportedMod.EastScarp => IsModLoaded("atravita.EastScarp"),
        SupportedMod.DowntownZuzu => IsModLoaded("DTZ.DowntownZuzuDLL"),
        SupportedMod.DeepWoods => IsModLoaded("maxvollmer.deepwoodsmod"),
        SupportedMod.VisitMountVapius => IsModLoaded("lumisteria.visitmountvapius.code"),
        SupportedMod.StardewValleyExpanded => IsModLoaded("FlashShifter.SVECode"),
        SupportedMod.GenericModConfigMenu => IsModLoaded("spacechase0.GenericModConfigMenu"),
        _ => false,
      };
    }

    internal static bool IsModLoaded(string modId)
    {
      return ModUtility.Helper.ModRegistry.IsLoaded(modId);
    }
  }
}