using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class Desert : WarpLocationBase
  {
    public override int Order => 4;
    internal override string LocationName => "Desert";
    public override string DialogLabel => "dialog.location.desert";
    internal override string ObeliskName => "Desert Obelisk";
    public override bool CanWarp => CanWarpHere();
    public override Rectangle SpirteSource => new(192, 0, 64, 64);

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