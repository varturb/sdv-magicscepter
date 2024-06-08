using MagicScepter.Constants;
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
        name: () => I18n.SettingsMenu_Option_OldDialog_Label(),
        getValue: () => ModUtility.Config.UseOldDialogMenu,
        setValue: value => ModUtility.Config.UseOldDialogMenu = value
      );

      configMenu.AddParagraph(
        mod: ModUtility.Manifest,
        text: () => string.Empty
      );

      configMenu.AddSectionTitle(
        mod: ModUtility.Manifest,
        text: () => I18n.TeleportMenuSettings_Title()
      );

      configMenu.AddBoolOption(
        mod: ModUtility.Manifest,
        name: () => I18n.TeleportMenuSettings_PlaySound(),
        getValue: () => ModUtility.Config.PlaySound,
        setValue: value => ModUtility.Config.PlaySound = value
      );

      configMenu.AddNumberOption(
        mod: ModUtility.Manifest,
        name: () => I18n.TeleportMenuSettings_Radius(),
        getValue: () => ModUtility.Config.Radius,
        setValue: value => ModUtility.Config.Radius = value,
        min: ModConstants.ScrollsRadiusRange.Min,
        max: ModConstants.ScrollsRadiusRange.Max,
        interval: ModConstants.ScrollsRadiusInterval
      );

      configMenu.AddNumberOption(
        mod: ModUtility.Manifest,
        name: () => I18n.TeleportMenuSettings_Scale(),
        getValue: () => ModUtility.Config.Scale,
        setValue: value => ModUtility.Config.Scale = value,
        min: ModConstants.ScrollsScaleRange.Min,
        max: ModConstants.ScrollsScaleRange.Max,
        interval: ModConstants.ScrollsScaleInterval
      );

      configMenu.AddNumberOption(
        mod: ModUtility.Manifest,
        name: () => I18n.TeleportMenuSettings_SelectedScale(),
        getValue: () => ModUtility.Config.SelectedScale,
        setValue: value => ModUtility.Config.SelectedScale = value,
        min: ModConstants.SelectedScrollScaleRange.Min,
        max: ModConstants.SelectedScrollScaleRange.Max,
        interval: ModConstants.SelectedScrollScaleInterval
      );
    }
  }
}