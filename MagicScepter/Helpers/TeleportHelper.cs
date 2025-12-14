using System.Collections.Generic;
using System.Linq;
using MagicScepter.Constants;
using MagicScepter.Managers;
using MagicScepter.Models;
using MagicScepter.Mods;
using MagicScepter.Mods.MultipleMiniObelisks;
using MagicScepter.Multiplayer;
using MagicScepter.Tools;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace MagicScepter.Helpers
{
  public static class TeleportHelper
  {
    public static bool CanTeleport(List<ActionWhen> whenList, string scrollID = null)
    {
      if (whenList == null)
      {
        return true;
      }

      foreach (var when in whenList)
      {
        if (!HandleWhen(when, scrollID))
        {
          return false;
        }
      }

      return true;
    }

    public static void Teleport(ActionDo @do, string scrollID)
    {
      HandleDo(@do, scrollID);
    }

    private static bool HandleWhen(ActionWhen when, string scrollID)
    {
      var isMemory = false;
      if (ModUtility.Config.MemoryMode && scrollID != null)
      {
        var memoryList = ModDataHelper.GetMemorySaveData();
        if (memoryList.Contains(scrollID))
        {
          isMemory = true;
        }
      }

      return when.Type switch
      {
        ActionWhenType.Obelisk => isMemory || CanTeleportUsingObelisk(when.Is),
        ActionWhenType.IslandObelisk => isMemory || HasIslandObelisk(when.Is) ,
        ActionWhenType.MiniObelisk => HasMiniObelisk(when.Is),
        ActionWhenType.MultipleMiniObelisk => MultipleMiniObelisks.CanTeleport(),
        ActionWhenType.Mod => when.Is != null ? IsModLoaded(when.Is) : !IsModLoaded(when.IsNot),
        ActionWhenType.Quest => isMemory || IsQuestCompleted(when.Is) ,
        ActionWhenType.Event => isMemory || IsEventSeen(when.Is) ,
        _ => false,
      };
    }

    private static void HandleDo(ActionDo @do, string scrollID)
    {
      if (TeleportBackManager.IsTeleportBackEnabled() && @do.Type != ActionDoType.TeleportBack)
      {
        TeleportBackManager.SetCurrentLocationAsLast();
      }

      switch (@do.Type)
      {
        case ActionDoType.Farm:
          TeleportToHome(@do.Location, @do.Point);
          break;
        case ActionDoType.Teleport:
          TeleportUsingWand(@do.Location, @do.Point.X, @do.Point.Y);
          ModDataHelper.UpdateMemorySaveData(scrollID);
          break;
        case ActionDoType.Obelisk:
          TeleportUsingObelisk(@do.Name);
          ModDataHelper.UpdateMemorySaveData(scrollID);
          break;
        case ActionDoType.MiniObelisk:
          TeleportUsingMiniObelisk(@do.Location, @do.Point.X, @do.Point.Y);
          break;
        case ActionDoType.MultipleMiniObelisk:
          MultipleMiniObelisks.OpenTeleportMenu();
          break;
        case ActionDoType.TeleportBack:
          TeleportBackManager.TeleportBack();
          break;
      }
    }

    private static bool HasMiniObelisk(string obeliskName)
    {
      return LocationHelper.FindObjects(obeliskName).Count > 0;
    }

    private static bool CanTeleportUsingObelisk(string obelisk)
    {
      return LocationHelper.FindBuilding(obelisk) != null;
    }

    public static bool HasIslandObelisk(string location)
    {
      if (Context.IsMainPlayer)
      {
        try
        {
          var islandWest = Game1.locations.FirstOrDefault(loc => loc.Name == location);
          if (islandWest != null)
          {
            return (islandWest as IslandWest).farmObelisk.Value;
          }
          return false;
        }
        catch
        {
          return false;
        }
      }
      else
      {
        return MultiplayerManager.CanTeleportToIslandFarm;
      }
    }

    private static bool IsModLoaded(string modId)
    {
      return ModManager.IsModLoaded(modId);
    }

    private static bool IsQuestCompleted(string questId)
    {
      return Game1.MasterPlayer.team.completedSpecialOrders.Contains(questId);
    }

    private static bool IsEventSeen(string eventId)
    {
      return Game1.MasterPlayer.eventsSeen.Contains(eventId);
    }

    private static void TeleportToHome(string location, ActionDoPoint point)
    {
      var home = Utility.getHomeOfFarmer(Game1.player);
      var x = home == null ? point.X : home.getFrontDoorSpot().X;
      var y = home == null ? point.Y : home.getFrontDoorSpot().Y;
      TeleportUsingWand(location, x, y);
    }

    private static void TeleportUsingWand(string location, int x, int y)
    {
      BetterWand.Teleport(location, x, y);
    }

    private static void TeleportUsingObelisk(string obeliskName)
    {
      var obelisk = new Building(obeliskName, new Vector2(-1, -1));
      obelisk.doAction(new Vector2(-1, -1), Game1.player);
    }

    private static void TeleportUsingMiniObelisk(string location, int x, int y)
    {
      var obeliskCoords = GetValidTile(x, y);
      if (obeliskCoords == null)
      {
        GameHelper.ShowMessage(Game1.content.LoadString(ModConstants.MiniObeliskNeedsSpaceMessagePath), MessageType.Error);
        return;
      }

      BetterWand.Teleport(location, obeliskCoords.X, obeliskCoords.Y);
    }

    private static ActionDoPoint GetValidTile(int x, int y)
    {
      var tilePoint = new ActionDoPoint(x, y + 1);

      if (IsTileValid(tilePoint))
      {
        return tilePoint;
      }
      tilePoint = new ActionDoPoint(x - 1, y);
      if (IsTileValid(tilePoint))
      {
        return tilePoint;
      }
      tilePoint = new ActionDoPoint(x + 1, y);
      if (IsTileValid(tilePoint))
      {
        return tilePoint;
      }
      tilePoint = new ActionDoPoint(x, y - 1);
      if (IsTileValid(tilePoint))
      {
        return tilePoint;
      }

      return null;
    }

    private static bool IsTileValid(ActionDoPoint tilePoint)
    {
      return Game1.getFarm().CanItemBePlacedHere(new Vector2(tilePoint.X, tilePoint.Y));
    }
  }
}