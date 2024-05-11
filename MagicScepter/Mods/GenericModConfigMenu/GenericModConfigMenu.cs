using StardewModdingAPI.Events;

namespace MagicScepter.Mods.GenericModConfigMenu
{
  public static class GenericModConfigMenu
  {
    public static void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
      // get Generic Mod Config Menu's API (if it's installed)
      var configMenu = ModUtility.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
      if (configMenu is null)
        return;

      // register mod
      configMenu.Register(
          mod: ModUtility.Manifest,
          reset: () => ModUtility.Config = new ModConfig(),
          save: () => ModUtility.Helper.WriteConfig(ModUtility.Config)
      );

      // add some config options
      configMenu.AddBoolOption(
          mod: ModUtility.Manifest,
          name: () => "Old dialog menu",
          tooltip: () => "Use old dialog menu.",
          getValue: () => ModUtility.Config.UseOldDialogMenu,
          setValue: value => ModUtility.Config.UseOldDialogMenu = value
      );
    }
  }
}