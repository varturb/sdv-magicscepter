namespace MagicScepter.Mods
{
  public static class ModManager
  {
    internal static bool IsModLoaded(SupportedMod mod)
    {
      return mod switch
      {
        SupportedMod.MultipleMiniObelisks => ModUtility.Helper.ModRegistry.IsLoaded("PeacefulEnd.MultipleMiniObelisks"),
        SupportedMod.RidgesideVillage => ModUtility.Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage"),
        SupportedMod.EastScarp => ModUtility.Helper.ModRegistry.IsLoaded("atravita.EastScarp"),
        SupportedMod.DowntownZuzu => ModUtility.Helper.ModRegistry.IsLoaded("DTZ.DowntownZuzuDLL"),
        SupportedMod.DeepWoods => ModUtility.Helper.ModRegistry.IsLoaded("maxvollmer.deepwoodsmod"),
        SupportedMod.VisitMountVapius => ModUtility.Helper.ModRegistry.IsLoaded("lumisteria.visitmountvapius.code"),
        _ => false,
      };
    }
  }
}