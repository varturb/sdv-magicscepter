using System;
using System.Linq;
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
      var hasIslandObelisk = LocationHelper.FindBuilding(ObeliskName) != null;
      if (!hasIslandObelisk) return false;

      try
      {
        var islandWest = Game1.locations.First(loc => loc.Name == LocationName) as IslandWest;
        return (bool)islandWest?.farmObelisk.Value;
      }
      catch 
      {
        return false;
      }
    }
  }
}