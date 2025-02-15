using MagicScepter.Constants;
using MagicScepter.Helpers;
using MagicScepter.Managers;
using StardewModdingAPI;

namespace MagicScepter.Models
{
  public class TeleportBackScroll : TeleportScroll
  {
    public TeleportBackScroll(DataEntry data) : base(data)
    {
      var saveEntry = ModDataHelper.GetSaveDataEntry(ID);
      Order = saveEntry?.Order ?? (data.Order + ModConstants.TeleportBackScrollOrderOffset);
      DefaultText = TranslationHelper.Get(data.TranslationKey);
      Text = saveEntry?.Name ?? DefaultText;
      Hidden = !TeleportBackManager.CanTeleportBack();
      Keybind = saveEntry?.Keybind.MapToSButton() ?? SButton.None;
      CanTeleport = TeleportBackManager.CanTeleportBack();
    }
  }
}