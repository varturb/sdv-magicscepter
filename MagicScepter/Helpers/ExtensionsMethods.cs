using System.Collections.Generic;
using System.Linq;
using MagicScepter.Models;
using Newtonsoft.Json;

namespace MagicScepter.Helpers
{
  public static class ExtensionMethods
  {
    public static T DeepCopy<T>(this T self)
    {
      var serialized = JsonConvert.SerializeObject(self);
      return JsonConvert.DeserializeObject<T>(serialized);
    }

    public static bool IsEmpty(this string text)
    {
      return text.Length == 0;
    }

    public static List<SaveDataEntry> AdjustOrder(this List<SaveDataEntry> list)
    {
      var order = 1;
      var ordered = list.OrderBy(e => e.Order).ToList();
      for (int i = 0; i < list.Count; i++)
      {
        ordered[i].Order = order++;
      }
      return ordered;
    }

    public static int GetEntryIndexForID(this List<SaveDataEntry> list, string id)
    {
      return list.FindIndex(x => x.ID == id);
    }

    public static List<WarpObject> AdjustOrder(this List<WarpObject> list)
    {
      var order = 1;
      var ordered = list.OrderBy(e => e.Order).ToList();
      for (int i = 0; i < list.Count; i++)
      {
        ordered[i].Order = order++;
      }
      return ordered;
    }

    public static List<WarpObject> FilterHiddenItems(this List<WarpObject> list)
    {
      return list.Where(x => !x.Hidden).ToList();
    }

    public static SaveDataEntry ConvertToSaveDataEntry(this WarpObject warpObject)
    {
      return new SaveDataEntry(warpObject.ID, warpObject.Text, warpObject.Order, warpObject.Hidden);
    }
  }
}