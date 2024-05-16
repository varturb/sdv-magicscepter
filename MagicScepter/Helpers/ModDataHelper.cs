using System.Collections.Generic;
using System.Linq;
using MagicScepter.Handlers;
using MagicScepter.Models;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MagicScepter.Helpers
{
  public static class ModDataHelper
  {
    private static readonly string SaveDataKey = $"{ModUtility.Manifest.UniqueID}/warp-options";

    public static List<SaveDataEntry> GetSaveData()
    {
      try
      {
        if (Context.IsMainPlayer && !Game1.player.modData.ContainsKey(SaveDataKey))
        {
          Game1.player.modData.Add(SaveDataKey, string.Empty);
        }

        var entires = JsonConvert.DeserializeObject<List<SaveDataEntry>>(Game1.MasterPlayer.modData[SaveDataKey]);
        return entires ?? new List<SaveDataEntry>();
      }
      catch
      {
        return new List<SaveDataEntry>();
      }
    }

    public static SaveDataEntry GetWarpLocationEntry(string id)
    {
      var warpLocations = GetSaveData();
      return warpLocations.FirstOrDefault(x => x.ID == id);
    }

    public static void UpdateSaveData()
    {
      if (!Context.IsMainPlayer)
        return;

      if (!Game1.player.modData.ContainsKey(SaveDataKey))
      {
        Game1.player.modData.Add(SaveDataKey, string.Empty);
      }

      var saveData = GetEntriesToSave(new());
      Game1.player.modData[SaveDataKey] = JsonConvert.SerializeObject(saveData);
    }

    public static void UpdateSaveData(SaveDataEntry entry)
    {
      if (!Context.IsMainPlayer)
        return;

      if (!Game1.player.modData.ContainsKey(SaveDataKey))
      {
        Game1.player.modData.Add(SaveDataKey, string.Empty);
      }

      var saveData = GetEntriesToSave(new() { entry });
      Game1.player.modData[SaveDataKey] = JsonConvert.SerializeObject(saveData);
    }

    public static void UpdateSaveData(List<SaveDataEntry> entriesToSave)
    {
      if (!Context.IsMainPlayer)
        return;

      if (!Game1.player.modData.ContainsKey(SaveDataKey))
      {
        Game1.player.modData.Add(SaveDataKey, string.Empty);
      }

      var saveData = GetEntriesToSave(entriesToSave);
      Game1.player.modData[SaveDataKey] = JsonConvert.SerializeObject(saveData);
    }

    public static void OnDayStarted(object sender, DayStartedEventArgs e)
    {
      UpdateSaveData();
    }

    private static List<SaveDataEntry> GetEntriesToSave(List<SaveDataEntry> entriesToSave)
    {
      var warpObjects = ResponseHandler.GetWarpObjects();
      var saveData = new List<SaveDataEntry>();

      foreach (var wo in warpObjects)
      {
        var entry = entriesToSave.Find(x => x.ID == wo.ID) ?? wo.ConvertToSaveDataEntry();
        saveData.Add(entry);
      }

      return saveData.AdjustOrder();
    }
  }
}