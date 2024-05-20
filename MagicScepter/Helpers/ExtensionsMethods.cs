using System;
using System.Collections.Generic;
using System.Linq;
using MagicScepter.Models;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

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

    public static bool IsNotEmpty(this string text)
    {
      return text.Length > 0;
    }

    public static bool IsOneOf(this string text, List<string> strings)
    {
      return strings.Where(x => x == text).Any();
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

    public static Response[] ToResponses(this List<TeleportScroll> teleportScrolls)
    {
      var responses = new List<Response>();

      foreach (var tp in teleportScrolls)
      {
        responses.Add(new Response(tp.ID, tp.Text));
      }
      responses.Add(new Response(I18n.Common_Cancel(), I18n.Common_Cancel()));

      return responses.ToArray();
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

    public static List<TeleportScroll> FilterItemsWithoutKeybind(this List<TeleportScroll> list)
    {
      return list.Where(x => x.Keybind != SButton.None).ToList();
    }

    public static TeleportScroll FindScrollWithKeybind(this List<TeleportScroll> list, SButton sButton)
    {
      return list.FirstOrDefault(x => x.Keybind == sButton);
    }

    public static string GetNextScrollID(this List<TeleportScroll> list, int currOrder)
    {
      var nextOrder = list.Count < (currOrder + 1) ? 1 : (currOrder + 1);
      return list.First(x => x.Order == nextOrder).ID;
    }

    public static string GetPreviousScrollID(this List<TeleportScroll> list, int currOrder)
    {
      var prevOrder = (currOrder - 1) < 1 ? list.Count : (currOrder - 1);
      return list.First(x => x.Order == prevOrder).ID;
    }

    public static SaveDataEntry ConvertToSaveDataEntry(this TeleportScroll teleportScroll)
    {
      return new SaveDataEntry(teleportScroll.ID, teleportScroll.Text, teleportScroll.Order, teleportScroll.Hidden, teleportScroll.Keybind.MapToString());
    }

    public static SButton ToKeybindList(this KeybindList keybindList)
    {
      if (keybindList == null)
      {
        return SButton.None;
      }

      foreach (Keybind keybind in keybindList.Keybinds)
      {
        if (keybind.IsBound)
          return keybind.Buttons.First();
      }

      return SButton.None;
    }

    public static string MapToString(this SButton sButton, bool includeNone = false)
    {
      return sButton switch
      {
        SButton.D0 => "0",
        SButton.D1 => "1",
        SButton.D2 => "2",
        SButton.D3 => "3",
        SButton.D4 => "4",
        SButton.D5 => "5",
        SButton.D6 => "6",
        SButton.D7 => "7",
        SButton.D8 => "8",
        SButton.D9 => "9",
        SButton.None => includeNone ? I18n.KeybindMenu_NoKey() : string.Empty,
        _ => sButton.ToString()
      };
    }

    public static SButton MapToSButton(this string button)
    {
      try
      {
        return button switch
        {
          "0" => SButton.D0,
          "1" => SButton.D1,
          "2" => SButton.D2,
          "3" => SButton.D3,
          "4" => SButton.D4,
          "5" => SButton.D5,
          "6" => SButton.D6,
          "7" => SButton.D7,
          "8" => SButton.D8,
          "9" => SButton.D9,
          _ => Enum.TryParse(button, out SButton sButton) ? sButton : SButton.None
        };
      }
      catch
      {
        return SButton.None;
      }
    }
  }
}