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
  public static class WarpHelper
  {
    public static bool CanWarp(List<WarpWhen> whenList)
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

    public static void Warp(WarpDo @do)
    {
      HandleDo(@do);
    }

    private static bool HandleWhen(WarpWhen when)
    {
      return when.Type switch
      {
        WarpWhenType.Obelisk => CanWarpUsingObelisk(when.Is),
        WarpWhenType.IslandObelisk => HasIslandObelisk(when.Is),
        WarpWhenType.MiniObelisk => HasMiniObelisk(when.Is),
        WarpWhenType.MultipleMiniObelisk => MultipleMiniObelisks.CanWarp(),
        WarpWhenType.Mod => when.Is != null ? IsModLoaded(when.Is) : !IsModLoaded(when.IsNot),
        WarpWhenType.Quest => IsQuestCompleted(when.Is),
        WarpWhenType.Event => IsEventSeen(when.Is),
        _ => false,
      };
    }

    private static void HandleDo(WarpDo @do)
    {
      switch (@do.Type)
      {
        case WarpDoType.Farm:
          WarpToHome(@do.Location, @do.Point);
          break;
        case WarpDoType.Warp:
          WarpUsingWand(@do.Location, @do.Point.X, @do.Point.Y);
          break;
        case WarpDoType.Obelisk:
          WarpUsingObelisk(@do.Name);
          break;
        case WarpDoType.MiniObelisk:
          WarpUsingMiniObelisk(@do.Location, @do.Point.X, @do.Point.Y);
          break;
        case WarpDoType.MultipleMiniObelisk:
          MultipleMiniObelisks.OpenWarpMenu();
          break;
      }
    }

    private static bool HasMiniObelisk(string obeliskName)
    {
      return LocationHelper.FindObjects(obeliskName).Count > 0;
    }

    private static bool CanWarpUsingObelisk(string obelisk)
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
        return MultiplayerManager.CanWarpToIslandFarm;
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

    private static void WarpToHome(string location, WarpDoPoint point)
    {
      var home = Utility.getHomeOfFarmer(Game1.player);
      var x = home == null ? point.X : home.getFrontDoorSpot().X;
      var y = home == null ? point.Y : home.getFrontDoorSpot().Y;
      WarpUsingWand(location, x, y);
    }

    private static void WarpUsingWand(string location, int x, int y)
    {
      BetterWand.Warp(location, x, y);
    }

    private static void WarpUsingObelisk(string obeliskName)
    {
      var obelisk = LocationHelper.FindBuilding(obeliskName);
      obelisk?.doAction(new Vector2(obelisk.tileX.Value, obelisk.tileY.Value), Game1.player);
    }

    private static void WarpUsingMiniObelisk(string location, int x, int y)
    {
      var obeliskCoords = GetValidTile(x, y);
      if (obeliskCoords == null)
      {
        Game1.showRedMessage(Game1.content.LoadString(PathConstants.MiniObeliskNeedsSpaceMessagePath));
        return;
      }

      BetterWand.Warp(location, obeliskCoords.X, obeliskCoords.Y);
    }

    private static WarpDoPoint GetValidTile(int x, int y)
    {
      var tilePoint = new WarpDoPoint(x, y + 1);

      if (IsTileValid(tilePoint))
      {
        return tilePoint;
      }
      tilePoint = new WarpDoPoint(x - 1, y);
      if (IsTileValid(tilePoint))
      {
        return tilePoint;
      }
      tilePoint = new WarpDoPoint(x + 1, y);
      if (IsTileValid(tilePoint))
      {
        return tilePoint;
      }
      tilePoint = new WarpDoPoint(x, y - 1);
      if (IsTileValid(tilePoint))
      {
        return tilePoint;
      }

      return null;
    }

    private static bool IsTileValid(WarpDoPoint tilePoint)
    {
      return Game1.getFarm().CanItemBePlacedHere(new Vector2(tilePoint.X, tilePoint.Y));
    }
  }
}