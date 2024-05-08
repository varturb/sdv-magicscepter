using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class Beach : WarpLocationBase
  {
    public override int Order => 2;
    internal override string LocationName => "Beach";
    public override string DialogLabel => "dialog.location.beach";
    internal override string ObeliskName => "Water Obelisk";
    public override bool CanWarp => CanWarpHere();
    public override Rectangle SpirteSource => new(64, 0, 64, 64);

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