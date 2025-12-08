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
        SupportedMod.DeepWoods => IsModLoaded("maxmakesmods.deepwoodsmod"),
        SupportedMod.VisitMountVapius => IsModLoaded("lumisteria.visitmountvapius.code"),
        SupportedMod.StardewValleyExpanded => IsModLoaded("FlashShifter.SVECode"),
        SupportedMod.GenericModConfigMenu => IsModLoaded("spacechase0.GenericModConfigMenu"),
        SupportedMod.PelicanTownObelisk => IsModLoaded("Tikamin557.CP.PelicanTownObelisk_NEW"),
        SupportedMod.GrampFieldsWarp => IsModLoaded("lamplight.GrampFieldsWarp"),
        SupportedMod.ChaosObelisk => IsModLoaded("TheSamePlant.ChaosObelisk_ChaosObelisk"),
        SupportedMod.DarkClub => IsModLoaded("simezi21.Dark_Club_CP_Furniture"),
        SupportedMod.Lilybrook => IsModLoaded("8BitAlien.Lilybrook"),
        SupportedMod.SunberryVillage => IsModLoaded("skellady.SBVCP"),
        _ => false,
      };
    }

    internal static bool IsModLoaded(string modId)
    {
      return ModUtility.Helper.ModRegistry.IsLoaded(modId);
    }
  }
}