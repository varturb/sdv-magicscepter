using System;
using MagicScepter.Constants;
using StardewModdingAPI;

namespace MagicScepter
{
  public static class ModUtility
  {
    public static IModHelper Helper { get; private set; }
    public static IMonitor Monitor { get; private set; }
    public static IManifest Manifest { get; private set; }
    public static GamePlatform Platform { get; private set; }
    public static ModConfig Config { get; set; }

    public static void Initialize(IModHelper helper, IMonitor monitor, IManifest manifest)
    {
      Helper = helper;
      Monitor = monitor;
      Manifest = manifest;
      Platform = StardewModdingAPI.Constants.TargetPlatform;
      Config = ValidateConfig(helper.ReadConfig<ModConfig>());
    }

    private static ModConfig ValidateConfig(ModConfig config)
    {
      try
      {
        var radiusRange = ModConstants.ScrollsRadiusRange;
        config.Radius = Math.Max(radiusRange.Min, Math.Min(radiusRange.Max, config.Radius));
      }
      catch
      {
        config.Radius = ModConstants.DefaultScrollsRadius;
      }

      try
      {
        var scaleRange = ModConstants.ScrollsScaleRange;
        config.Scale = Math.Max(scaleRange.Min, Math.Min(scaleRange.Max, config.Scale));
      }
      catch
      {
        config.Scale = ModConstants.DefaultScrollsScale;
      }

      try
      {
        var selecectScaleRange = ModConstants.SelectedScrollScaleRange;
        config.SelectedScale = Math.Max(selecectScaleRange.Min, Math.Min(selecectScaleRange.Max, config.SelectedScale));
      }
      catch
      {
        config.SelectedScale = ModConstants.DefaultSelectedScrollScale;
      }

      return config;
    }
  }
}