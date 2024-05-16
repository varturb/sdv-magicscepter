using System.Linq;
using MagicScepter.Helpers;
using StardewModdingAPI.Events;

namespace MagicScepter.Models
{
  public class MiniObeliskObject : WarpObject
  {
    private const int orderOffset = 100;
    private const string miniObeliskObjectName = "Mini-Obelisk";

    public MiniObeliskObject(DataEntry data, int index, int x, int y) : base(data)
    {
      ID = $"{ID}#{index + 1}";

      var saveEntry = ModDataHelper.GetWarpLocationEntry(ID);
      Order = saveEntry?.Order ?? (data.Order + orderOffset + index);
      DefaultText = $"{TranslationHelper.Get(data.TranslationKey)} #{index + 1}";
      Text = saveEntry?.Name ?? DefaultText;
      Hidden = saveEntry?.Hidden ?? false;

      WarpDoWhen.Do.Point = new WarpDoPoint(x, y);
      CanWarp = WarpHelper.CanWarp(WarpDoWhen.When);
    }

    public static void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
    {
      var shouldUpdateSaveData = e.Added.Concat(e.Removed)
        .Where(x => x.Value.Name == miniObeliskObjectName)
        .Any();
      if (shouldUpdateSaveData)
      {
        ModDataHelper.UpdateSaveData();
      }
    }
  }
}