using MagicScepter.Models;

namespace MagicScepter.Constants
{
  public static class ModConstants
  {
    public static readonly string ConfigurationSaveKey = $"{ModUtility.Manifest.UniqueID}/configuration";
    public static readonly string SettingsSaveKey = $"{ModUtility.Manifest.UniqueID}/settings";
    public const string EmoteMenuTexturePath = "LooseSprites\\EmoteMenu";
    public const string MiniObeliskNeedsSpaceMessagePath = "Strings\\StringsFromCSFiles:MiniObelisk_NeedsSpace";
    public const string MiniObeliskID = "MagicScepter_MiniObelisk";
    public const string EastScarpFarmLocation = "EastScarp_MeadowFarm";
    public const string RidgesideFarmLocation = "Custom_Ridgeside_SummitFarm";
    public const string MiniObeliskObjectName = "Mini-Obelisk";
    public const string IslandFarmID = "MagicScepter_IslandFarm";
    public const string PlaySound = "throw";
    public const string ScrollsTexturePath = "assets/scrolls.png";
    public static string SpritesheetTexturePath => GetSpritesheetTexturePath();
    public const string ThemeDefault = "Default";
    public const string ThemeVintage = "Vintage";
    public const string ThemeWitchy = "Witchy";
    public const int TeleportScrollOrderOffset = 100;
    public const float DefaultLayerDepth = 0.86f;
    public static readonly MinMax ScrollsRadiusRange = new(20, 100);
    public static readonly int ScrollsRadiusInterval = 1;
    public static readonly int DefaultScrollsRadius = 42;
    public static readonly MinMaxFloat ScrollsScaleRange = new(0.5f, 3f);
    public static readonly float ScrollsScaleInterval = 0.05f;
    public static readonly float DefaultScrollsScale = 1f;
    public static readonly MinMaxFloat SelectedScrollScaleRange = new(0.5f, 3f);
    public static readonly float SelectedScrollScaleInterval = 0.05f;
    public static readonly float DefaultSelectedScrollScale = 1.5f;

    private static string GetSpritesheetTexturePath()
    {
      return ModUtility.Config.Theme switch
      {
        ThemeDefault => "assets/spritesheet.png",
        ThemeVintage => "assets/spritesheet_vintage.png",
        ThemeWitchy => "assets/spritesheet_witchy.png",
        _ => "assets/spritesheet.png"
      };
    }
  }
}