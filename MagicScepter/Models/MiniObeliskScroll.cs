using System.Linq;
using MagicScepter.Constants;
using MagicScepter.Helpers;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MagicScepter.Models
{
  public class MiniObeliskScroll : TeleportScroll
  {
    public MiniObeliskScroll(DataEntry data, int index, int x, int y) : base(data)
    {
      ID = $"{ID}#{x}_{y}";

      var saveEntry = ModDataHelper.GetSaveDataEntry(ID);
      Order = saveEntry?.Order ?? (data.Order + ModConstants.TeleportScrollOrderOffset + index);
      DefaultText = $"{TranslationHelper.Get(data.TranslationKey)} #{index + 1}";
      Text = saveEntry?.Name ?? DefaultText;
      Hidden = saveEntry?.Hidden ?? false;
      Keybind = saveEntry?.Keybind.MapToSButton() ?? SButton.None;

      ActionDoWhen.Do.Point = new ActionDoPoint(x, y);
      CanTeleport = TeleportHelper.CanTeleport(ActionDoWhen.When);
    }

    public static void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
    {
      var shouldUpdateSaveData = e.Added.Concat(e.Removed)
        .Where(x => x.Value.Name == ModConstants.MiniObeliskObjectName)
        .Any();
      if (shouldUpdateSaveData)
      {
        ModDataHelper.UpdateSaveData();
      }
    }
  }
}