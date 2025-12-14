using System;
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
    private static readonly string configSaveDataKey = ModConstants.ConfigurationSaveKey;
    private static readonly string memorySaveDataKey = ModConstants.MemorySaveDataKey;

    public static List<SaveDataEntry> GetSaveData()
    {
      try
      {
        if (Context.IsMainPlayer && !Game1.player.modData.ContainsKey(configSaveDataKey))
        {
          Game1.player.modData.Add(configSaveDataKey, string.Empty);
        }

        var entires = JsonConvert.DeserializeObject<List<SaveDataEntry>>(Game1.MasterPlayer.modData[configSaveDataKey]);
        return entires ?? new List<SaveDataEntry>();
      }
      catch
      {
        return new List<SaveDataEntry>();
      }
    }

    public static List<string> GetMemorySaveData()
    {
      try
      {
        if (Context.IsMainPlayer && !Game1.player.modData.ContainsKey(memorySaveDataKey))
        {
          Game1.player.modData.Add(memorySaveDataKey, string.Empty);
        }

        var memoryList = JsonConvert.DeserializeObject<List<string>>(Game1.player.modData[memorySaveDataKey]);
        return memoryList ?? new List<string>();
      }
      catch
      {
        return new List<string>();
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

      if (!Game1.player.modData.ContainsKey(configSaveDataKey))
      {
        Game1.player.modData.Add(configSaveDataKey, string.Empty);
      }

      var saveData = GetEntriesToSave(new());
      Game1.player.modData[configSaveDataKey] = JsonConvert.SerializeObject(saveData);
    }

    public static void UpdateSaveData(SaveDataEntry entry)
    {
      if (!Context.IsMainPlayer)
        return;

      if (!Game1.player.modData.ContainsKey(configSaveDataKey))
      {
        Game1.player.modData.Add(configSaveDataKey, string.Empty);
      }

      var saveData = GetEntriesToSave(new() { entry });
      Game1.player.modData[configSaveDataKey] = JsonConvert.SerializeObject(saveData);
    }

    public static void UpdateSaveData(List<SaveDataEntry> entriesToSave)
    {
      if (!Context.IsMainPlayer)
        return;

      if (!Game1.player.modData.ContainsKey(configSaveDataKey))
      {
        Game1.player.modData.Add(configSaveDataKey, string.Empty);
      }

      var saveData = GetEntriesToSave(entriesToSave);
      Game1.player.modData[configSaveDataKey] = JsonConvert.SerializeObject(saveData);
    }

    public static void UpdateMemorySaveData(string scrollID)
    {
      if (!Context.IsMainPlayer)
        return;

      var memoryList = new List<string>();
      try
      {
        memoryList = JsonConvert.DeserializeObject<List<string>>(Game1.player.modData[memorySaveDataKey]) ?? new List<string>();
      }
      catch { }

      memoryList.Add(scrollID);
      memoryList = memoryList.Distinct().ToList();
      Game1.player.modData[memorySaveDataKey] = JsonConvert.SerializeObject(memoryList);
    }

    public static void RestoreConfiguraiton()
    {
      Game1.player.modData[configSaveDataKey] = string.Empty;
    }

    public static void RestoreKeybinds()
    {
      var entires = GetSaveData();
      foreach (var entry in entires)
      {
        entry.Keybind = null;
      }
      Game1.player.modData[configSaveDataKey] = JsonConvert.SerializeObject(entires);
    }

    public static void RestoreSettings()
    {
      ModUtility.Config = new ModConfig();
      ModUtility.Helper.WriteConfig(ModUtility.Config);
    }

    public static void SetMenuType(bool value)
    {
      ModUtility.Config.UseOldDialogMenu = value;
      ModUtility.Helper.WriteConfig(ModUtility.Config);
    }

    public static void SetMemoryMode(bool value)
    {
      ModUtility.Config.MemoryMode = value;
      ModUtility.Helper.WriteConfig(ModUtility.Config);
    }

    public static void SetPlaySound(bool value)
    {
      ModUtility.Config.PlaySound = value;
      ModUtility.Helper.WriteConfig(ModUtility.Config);
    }

    public static void SetTeleportBack(bool value)
    {
      ModUtility.Config.EnableTeleportBack = value;
      ModUtility.Helper.WriteConfig(ModUtility.Config);
    }

    public static void SetRadius(int value)
    {
      ModUtility.Config.Radius = value;
      ModUtility.Helper.WriteConfig(ModUtility.Config);
    }

    public static void SetScale(float value)
    {
      ModUtility.Config.Scale = value;
      ModUtility.Helper.WriteConfig(ModUtility.Config);
    }

    public static void SetSelectedScale(float value)
    {
      ModUtility.Config.SelectedScale = value;
      ModUtility.Helper.WriteConfig(ModUtility.Config);
    }

    public static void SetRotation(string value)
    {
      ModUtility.Config.Rotation = value;
      ModUtility.Helper.WriteConfig(ModUtility.Config);
    }

    public static void SetTheme(string value)
    {
      ModUtility.Config.Theme = value;
      ModUtility.Helper.WriteConfig(ModUtility.Config);
      FileHelper.ReloadTexture();
    }

    public static void ResetScrollsSettings()
    {
      var defaultConfig = new ModConfig();
      ModUtility.Config.PlaySound = defaultConfig.PlaySound;
      ModUtility.Config.EnableTeleportBack = defaultConfig.EnableTeleportBack;
      ModUtility.Config.Theme = defaultConfig.Theme;
      ModUtility.Config.Rotation = defaultConfig.Rotation;
      ModUtility.Config.Radius = defaultConfig.Radius;
      ModUtility.Config.Scale = defaultConfig.Scale;
      ModUtility.Config.SelectedScale = defaultConfig.SelectedScale;

      ModUtility.Helper.WriteConfig(ModUtility.Config);
      FileHelper.ReloadTexture();
    }

    public static void OnDayStarted(object sender, DayStartedEventArgs e)
    {
      UpdateSaveData();
    }

    private static List<SaveDataEntry> GetEntriesToSave(List<SaveDataEntry> entriesToSave)
    {
      var teleportScrolls = ScrollHandler.GetTeleportScrolls();
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