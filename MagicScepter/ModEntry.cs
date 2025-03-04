﻿using System;
using HarmonyLib;
using MagicScepter.Helpers;
using MagicScepter.Managers;
using MagicScepter.Models;
using MagicScepter.Mods;
using MagicScepter.Mods.GenericModConfigMenu;
using MagicScepter.Multiplayer;
using MagicScepter.Patches;
using StardewModdingAPI;
using StardewValley.Tools;

namespace MagicScepter
{
  public class ModEntry : Mod
  {
    public override void Entry(IModHelper helper)
    {
      ModUtility.Initialize(helper, Monitor, ModManifest);

      try
      {
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.Patch(
          original: AccessTools.Method(typeof(Wand), nameof(Wand.DoFunction)),
          prefix: new HarmonyMethod(typeof(WandDoFunctionPatch), nameof(WandDoFunctionPatch.WandDoFunction_Prefix))
        );
      }
      catch (Exception e)
      {
        Monitor.Log($"Issue with Harmony patch: {e}", LogLevel.Error);
        return;
      }

      Helper.Events.Multiplayer.ModMessageReceived += MultiplayerManager.OnModMessageReceived;
      Helper.Events.GameLoop.DayStarted += MultiplayerManager.OnDayStarted;
      Helper.Events.GameLoop.DayStarted += ModDataHelper.OnDayStarted;
      Helper.Events.GameLoop.DayStarted += TeleportBackManager.OnDayStarted;

      if (!ModManager.IsModLoaded(SupportedMod.MultipleMiniObelisks))
      {
        Helper.Events.World.ObjectListChanged += MiniObeliskScroll.OnObjectListChanged;
      }

      if (ModManager.IsModLoaded(SupportedMod.GenericModConfigMenu))
      {
        Helper.Events.GameLoop.GameLaunched += GenericModConfigMenu.OnGameLaunched;
      }

      I18n.Init(Helper.Translation);
    }
  }
}
