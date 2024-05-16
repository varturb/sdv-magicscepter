using StardewModdingAPI;

namespace MagicScepter
{
  public static class ModUtility
  {
    public static IModHelper Helper { get; private set; }
    public static IMonitor Monitor { get; private set; }
    public static IManifest Manifest { get; private set; }
    public static ModConfig Config { get; set; }

    public static void Initialize(IModHelper helper, IMonitor monitor, IManifest manifest)
    {
      Helper = helper;
      Monitor = monitor;
      Manifest = manifest;
      Config = helper.ReadConfig<ModConfig>();
    }
  }
}