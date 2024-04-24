using StardewModdingAPI;

namespace MagicScepter.Mods
{
  public static class ModManager
  {
    private static IModHelper Helper;

    internal static void Initialize(IModHelper helper)
    {
      Helper = helper;
    }

    internal static bool IsModLoaded(SupportedMod mod)
    {
      return mod switch
      {
        SupportedMod.MultipleMiniObelisks => Helper.ModRegistry.IsLoaded("PeacefulEnd.MultipleMiniObelisks"),
        SupportedMod.RidgesideVillage => Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage"),
        SupportedMod.EastScarp => Helper.ModRegistry.IsLoaded("atravita.EastScarp"),
        SupportedMod.DowntownZuzu => Helper.ModRegistry.IsLoaded("DTZ.DowntownZuzuDLL"),
        SupportedMod.DeepWoods => Helper.ModRegistry.IsLoaded("maxvollmer.deepwoodsmod"),
        _ => false,
      };
    }
  }
}