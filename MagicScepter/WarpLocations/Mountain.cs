using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class Mountain : WarpLocationBase
  {
    public override int Order => 3;
    internal override string LocationName => "Mountain";
    public override string DialogLabel => "dialog.location.mountain";
    internal override string ObeliskName => "Earth Obelisk";
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