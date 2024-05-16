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

    public static List<TeleportScroll> AdjustOrder(this List<TeleportScroll> list)
    {
      var order = 1;
      var ordered = list.OrderBy(e => e.Order).ToList();
      for (int i = 0; i < list.Count; i++)
      {
        ordered[i].Order = order++;
      }
      return ordered;
    }

    public static List<TeleportScroll> FilterHiddenItems(this List<TeleportScroll> list)
    {
      return list.Where(x => !x.Hidden).ToList();
    }

    public static SaveDataEntry ConvertToSaveDataEntry(this TeleportScroll teleportScroll)
    {
      return new SaveDataEntry(teleportScroll.ID, teleportScroll.Text, teleportScroll.Order, teleportScroll.Hidden);
    }
  }
}