using MagicScepter.Models;

namespace MagicScepter.Constants
{
  public static class ModConstants
  {
    public static readonly string ConfigurationSaveKey = $"{ModUtility.Manifest.UniqueID}/configuration";
    public static readonly string SettingsSaveKey = $"{ModUtility.Manifest.UniqueID}/settings";
    public const string SpritesheetTexturePath = "assets/spritesheet.png";
    public const string EmoteMenuTexturePath = "LooseSprites\\EmoteMenu";
    public const string MiniObeliskNeedsSpaceMessagePath = "Strings\\StringsFromCSFiles:MiniObelisk_NeedsSpace";
    public const string MiniObeliskID = "MagicScepter_MiniObelisk";
    public const string EastScarpFarmLocation = "EastScarp_MeadowFarm";
    public const string RidgesideFarmLocation = "Custom_Ridgeside_SummitFarm";
    public const string MiniObeliskObjectName = "Mini-Obelisk";
    public const string IslandFarmID = "MagicScepter_IslandFarm";
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
  }
}