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
  }
}