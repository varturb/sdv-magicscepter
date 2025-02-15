using MagicScepter.Tools;
using StardewModdingAPI.Events;
using StardewValley;

namespace MagicScepter.Managers
{
  public static class TeleportBackManager
  {
    private static string location;
    private static float? x;
    private static float? y;

    public static bool IsTeleportBackEnabled()
    {
      return ModUtility.Config.EnableTeleportBack;
    }

    public static void SetCurrentLocationAsLast()
    {
      location = Game1.player.currentLocation.NameOrUniqueName;
      x = Game1.player.Tile.X;
      y = Game1.player.Tile.Y;
    }

    public static bool CanTeleportBack()
    {
      return location != null && x != null && y != null;
    }

    public static void TeleportBack()
    {
      if (CanTeleportBack())
      {
        BetterWand.Teleport(location, (int)x, (int)y);
        ClearLastLocation();
      }
    }

    public static void OnDayStarted(object sender, DayStartedEventArgs e)
    {
      ClearLastLocation();
    }

    private static void ClearLastLocation()
    {
      location = null;
      x = null;
      y = null;
    }
  }
}