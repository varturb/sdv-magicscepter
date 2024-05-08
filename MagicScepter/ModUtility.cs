using StardewModdingAPI;

namespace MagicScepter
{
  public static class ModUtility
  {
    public static IModHelper Helper;
    public static IMonitor Monitor;
    public static IManifest Manifest;
    public static ModConfig Config;

    public static void Initialize(IModHelper helper, IMonitor monitor, IManifest manifest)
    {
      Helper = helper;
      Monitor = monitor;
      Manifest = manifest;
      Config = helper.ReadConfig<ModConfig>();
    }
  }
}