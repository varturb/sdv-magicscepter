using System.Linq;
using MagicScepter.Multiplayer;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace MagicScepter.WarpLocations
{
  public class IslandFarm : WarpLocationBase
  {
    public override int Order => 6;
    internal override string LocationName => "IslandWest";
    public override string DialogLabel => "dialog.location.islandFarm";
    internal override string ObeliskName => "Island Obelisk";
    public override bool CanWarp => CanWarpHere();

    public override void Warp()
    {
      BetterWand.Warp(LocationName, 77, 40);
    }

    private bool CanWarpHere()
    {
      if (Context.IsMainPlayer)
      {
        try
        {
          var hasIslandObelisk = LocationHelper.FindBuilding(ObeliskName) != null;
          if (!hasIslandObelisk) return false;

          var islandWest = Game1.locations.FirstOrDefault(loc => loc.Name == LocationName);
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
  }
}