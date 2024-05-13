using System.Collections.Generic;
using System.Linq;
using MagicScepter.Models;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;

namespace MagicScepter.Helpers
{
  public static class ModDataHelper
  {
    internal static string SaveDataKey = $"{ModUtility.Manifest.UniqueID}/warp-locations";

    public static List<WarpLocationSaveDataEntry> GetWarpLocationEntries()
    {
      var entires = JsonConvert.DeserializeObject<List<WarpLocationSaveDataEntry>>(Game1.MasterPlayer.modData[SaveDataKey]);
      return entires ?? new List<WarpLocationSaveDataEntry>();
    }

    public static WarpLocationSaveDataEntry GetWarpLocationEntry(string id)
    {
      var warpLocations = GetWarpLocationEntries();
      return warpLocations.FirstOrDefault(x => x.ID == id);
    }

    public static int GetWarpLocationEntryIndex(string id)
    {
      var warpLocations = GetWarpLocationEntries();
      return warpLocations.FindIndex(x => x.ID == id);
    }

    public static void UpdateSaveData()
    {
      if (!Context.IsMainPlayer)
        return;

      if (!Game1.player.modData.ContainsKey(SaveDataKey))
      {
        Game1.player.modData.Add(SaveDataKey, string.Empty);
      }

      // return;

      // var warpLocationsSaveData = new List<WarpLocationSaveDataEntry>();
      // var warpLocations = ResponseManager.GetWarpLocations();
      // foreach (var warpLocation in warpLocations)
      // {
      //   var entry = new WarpLocationSaveDataEntry(warpLocation.ID, warpLocation.Order);
      //   warpLocationsSaveData.Add(entry);
      // }

      // Game1.player.modData[SaveDataKey] = JsonConvert.SerializeObject(warpLocationsSaveData);
    }

    public static void UpdateSaveData(WarpLocationSaveDataEntry entry)
    {
      if (!Context.IsMainPlayer)
        return;

      if (!Game1.player.modData.ContainsKey(SaveDataKey))
      {
        Game1.player.modData.Add(SaveDataKey, string.Empty);
      }

      var entires = GetWarpLocationEntries();
      var index = GetWarpLocationEntryIndex(entry.ID);
      if (index == -1)
      {
        entires.Add(entry);
      }
      else
      {
        entires[index] = entry;
      }

      Game1.player.modData[SaveDataKey] = JsonConvert.SerializeObject(entires);
    }
  }
}