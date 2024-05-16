using System.Collections.Generic;
using System.Linq;
using MagicScepter.Constants;
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
    private static readonly string saveDateKey = AllConstants.ConfigurationSaveKey;

    public static List<SaveDataEntry> GetSaveData()
    {
      try
      {
        if (Context.IsMainPlayer && !Game1.player.modData.ContainsKey(saveDateKey))
        {
          Game1.player.modData.Add(saveDateKey, string.Empty);
        }

        var entires = JsonConvert.DeserializeObject<List<SaveDataEntry>>(Game1.MasterPlayer.modData[saveDateKey]);
        return entires ?? new List<SaveDataEntry>();
      }
      catch
      {
        return new List<SaveDataEntry>();
      }
    }

    public static SaveDataEntry GetSaveDataEntry(string id)
    {
      var saveData = GetSaveData();
      return saveData.FirstOrDefault(x => x.ID == id);
    }

    public static void UpdateSaveData()
    {
      if (!Context.IsMainPlayer)
        return;

      if (!Game1.player.modData.ContainsKey(saveDateKey))
      {
        Game1.player.modData.Add(saveDateKey, string.Empty);
      }

      var saveData = GetEntriesToSave(new());
      Game1.player.modData[saveDateKey] = JsonConvert.SerializeObject(saveData);
    }

    public static void UpdateSaveData(SaveDataEntry entry)
    {
      if (!Context.IsMainPlayer)
        return;

      if (!Game1.player.modData.ContainsKey(saveDateKey))
      {
        Game1.player.modData.Add(saveDateKey, string.Empty);
      }

      var saveData = GetEntriesToSave(new() { entry });
      Game1.player.modData[saveDateKey] = JsonConvert.SerializeObject(saveData);
    }

    public static void UpdateSaveData(List<SaveDataEntry> entriesToSave)
    {
      if (!Context.IsMainPlayer)
        return;

      if (!Game1.player.modData.ContainsKey(saveDateKey))
      {
        Game1.player.modData.Add(saveDateKey, string.Empty);
      }

      var saveData = GetEntriesToSave(entriesToSave);
      Game1.player.modData[saveDateKey] = JsonConvert.SerializeObject(saveData);
    }

    public static void OnDayStarted(object sender, DayStartedEventArgs e)
    {
      UpdateSaveData();
    }

    private static List<SaveDataEntry> GetEntriesToSave(List<SaveDataEntry> entriesToSave)
    {
      var teleportScrolls = ResponseHandler.GetTeleportScrolls();
      var saveData = new List<SaveDataEntry>();

      foreach (var tp in teleportScrolls)
      {
        var entry = entriesToSave.Find(x => x.ID == tp.ID) ?? tp.ConvertToSaveDataEntry();
        saveData.Add(entry);
      }

      return saveData.AdjustOrder();
    }
  }
}