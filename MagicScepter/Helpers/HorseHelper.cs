using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;

namespace MagicScepter.Helpers
{
  public static class HorseHelper
  {
    private const float HorseWarpRadius = 10f;
    private static bool isHorseNearby = false;
    private static Vector2? fallbackHorsePosition = null;
    private static GameLocation fallbackHorseLocation = null;

    public static void BeforeTryWarpHorse()
    {
      if (!Game1.IsMasterGame) return;

      isHorseNearby = IsHorseNearby();

      if (!isHorseNearby) return;

      var player = Game1.player;
      var horse = GetHorse(player);

      horse.mutex.RequestLock(delegate
        {
          horse.mutex.ReleaseLock();

          var currentLocation = horse.currentLocation;
          var tile = horse.Tile;
          for (int i = 0; i < 8; i++)
          {
            Game1.Multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(10, new Vector2(tile.X + Utility.RandomFloat(-1f, 1f), tile.Y + Utility.RandomFloat(-1f, 0f)) * 64f, Color.White, 8, flipped: false, 50f)
            {
              layerDepth = 1f,
              motion = new Vector2(Utility.RandomFloat(-0.5f, 0.5f), Utility.RandomFloat(-0.5f, 0.5f))
            });
          }
          currentLocation.playSound("wand", horse.Tile);
          fallbackHorsePosition = horse.Tile;
          fallbackHorseLocation = horse.currentLocation;
          Game1.warpCharacter(horse, player.currentLocation, new Vector2(-1, -1));
        });
    }

    public static void TryWarpHorse()
    {
      if (isHorseNearby)
      {
        WarpHorse();
        isHorseNearby = false;
      }
    }

    private static void WarpHorse()
    {
      if (!Game1.IsMasterGame) return;

      var player = Game1.player;
      var horse = GetHorse(player);

      if (CanWarpHorse(horse, player))
      {
        var horsePosition = FindPositionForHorse();
        if (horsePosition != null)
        {
          horse.mutex.RequestLock(delegate
          {
            horse.mutex.ReleaseLock();
            Game1.warpCharacter(horse, player.currentLocation, horsePosition.Value);
            fallbackHorsePosition = null;
            fallbackHorseLocation = null;
          });

          return;
        }
      }

      if (fallbackHorsePosition != null && fallbackHorseLocation != null)
      {
        horse.mutex.RequestLock(delegate
        {
          horse.mutex.ReleaseLock();
          Game1.warpCharacter(horse, fallbackHorseLocation, fallbackHorsePosition.Value);
          fallbackHorsePosition = null;
          fallbackHorseLocation = null;
        });
      }
    }

    private static Horse GetHorse(Farmer player)
    {
      if (player.horseName.Value == null) return null;

      Horse horse = null;
      Utility.ForEachBuilding(delegate (Stable stable)
      {
        Horse stableHorse = stable.getStableHorse();
        if (stableHorse != null && stableHorse.getOwner() == player)
        {
          horse = stableHorse;
          return false;
        }
        return true;
      });

      return horse;
    }

    private static bool CanWarpHorse(Horse horse, Farmer player)
    {
      if (horse == null) return false;

      var restrictions = Utility.GetHorseWarpRestrictionsForFarmer(player);
      if (horse == null || (restrictions != 0 && !restrictions.HasFlag(Utility.HorseWarpRestrictions.NoRoom)))
      {
        return false;
      }

      return true;
    }

    private static bool IsHorseNearby()
    {
      var player = Game1.player;
      var horse = GetHorse(player);
      if (horse == null) return false;

      return IsWithinRadius(player.Position / 64f, horse.Position / 64f, HorseWarpRadius);
    }

    private static bool IsWithinRadius(Vector2 center, Vector2 targetPoint, float radius)
    {
      return Vector2.DistanceSquared(center, targetPoint) <= (radius * radius);
    }

    private static Vector2? FindPositionForHorse()
    {
      var xAxis = new int[] { -1, -2, 1, 2, 0 };
      var yAxis = new int[] { 2, 1, -1, -2, 0 };

      for (var i = 0; i < xAxis.Length; i++)
      {
        for (var j = 0; j < yAxis.Length; j++)
        {
          var x = Game1.player.TilePoint.X + xAxis[i];
          var y = Game1.player.TilePoint.Y + yAxis[j];
          var newHorsePosition = new Rectangle(x * 64, y * 64, 128, 64);
          if (!Game1.player.currentLocation.isCollidingPosition(newHorsePosition, Game1.viewport, isFarmer: true, 0, glider: false, Game1.player))
          {
            return new Vector2(x, y);
          }
        }
      }

      return null;
    }
  }
}