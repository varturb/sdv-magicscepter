using System;
using System.Linq;
using MagicScepter.Mods;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class Island : WarpLocationBase
  {
    public override int Order => 5;
    internal override string LocationName => "IslandWest";
    public override string DialogLabel => "dialog.location.island";
    internal override string ObeliskName => "Island Obelisk";
    public override bool CanWarp => CanWarpHere();

    public override void Warp()
    {
      var obelisk = LocationHelper.FindBuilding(ObeliskName);
      obelisk?.doAction(new Vector2(obelisk.tileX.Value, obelisk.tileY.Value), Game1.player);
    }

    private bool CanWarpHere()
    {
      return LocationHelper.FindBuilding(ObeliskName) != null;
    }
  }
}