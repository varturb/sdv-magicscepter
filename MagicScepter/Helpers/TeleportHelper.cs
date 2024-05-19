using System.Collections.Generic;
using System.Linq;
using MagicScepter.Constants;
using MagicScepter.Models;
using MagicScepter.Mods;
using MagicScepter.Mods.MultipleMiniObelisks;
using MagicScepter.Multiplayer;
using MagicScepter.Tools;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace MagicScepter.Helpers
{
  public static class TeleportHelper
  {
    public static bool CanTeleport(List<ActionWhen> whenList)
    {
      if (whenList == null)
      {
        return true;
      }

      foreach (var when in whenList)
      {
        if (!HandleWhen(when))
        {
          return false;
        }
      }

      return true;
    }

    public static void Teleport(ActionDo @do)
    {
      HandleDo(@do);
    }

    private static bool HandleWhen(ActionWhen when)
    {
      return when.Type switch
      {
        ActionWhenType.Obelisk => CanTeleportUsingObelisk(when.Is),
        ActionWhenType.IslandObelisk => HasIslandObelisk(when.Is),
        ActionWhenType.MiniObelisk => HasMiniObelisk(when.Is),
        ActionWhenType.MultipleMiniObelisk => MultipleMiniObelisks.CanTeleport(),
        ActionWhenType.Mod => when.Is != null ? IsModLoaded(when.Is) : !IsModLoaded(when.IsNot),
        ActionWhenType.Quest => IsQuestCompleted(when.Is),
        ActionWhenType.Event => IsEventSeen(when.Is),
        _ => false,
      };
    }

    private static void HandleDo(ActionDo @do)
    {
      switch (@do.Type)
      {
        case ActionDoType.Farm:
          TeleportToHome(@do.Location, @do.Point);
          break;
        case ActionDoType.Teleport:
          TeleportUsingWand(@do.Location, @do.Point.X, @do.Point.Y);
          break;
        case ActionDoType.Obelisk:
          TeleportUsingObelisk(@do.Name);
          break;
        case ActionDoType.MiniObelisk:
          TeleportUsingMiniObelisk(@do.Location, @do.Point.X, @do.Point.Y);
          break;
        case ActionDoType.MultipleMiniObelisk:
          MultipleMiniObelisks.OpenTeleportMenu();
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
      var obelisk = LocationHelper.FindBuilding(obeliskName);
      obelisk?.doAction(new Vector2(obelisk.tileX.Value, obelisk.tileY.Value), Game1.player);
    }

    private static void TeleportUsingMiniObelisk(string location, int x, int y)
    {
      var obeliskCoords = GetValidTile(x, y);
      if (obeliskCoords == null)
      {
        Game1.showRedMessage(Game1.content.LoadString(ModConstants.MiniObeliskNeedsSpaceMessagePath));
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