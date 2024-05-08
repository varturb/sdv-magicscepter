using MagicScepter.Tools;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class Farm : WarpLocationBase
  {
    public override int Order => 1;
    internal override string LocationName => "Farm";
    public override string DialogLabel => "dialog.location.farm";
    internal override string ObeliskName => null;
    public override bool CanWarp => true;
    public override Rectangle SpirteSource => new(0, 0, 64, 64);

    public override void Warp()
    {
      var home = Utility.getHomeOfFarmer(Game1.player);
      var x = home == null ? 64 : home.getFrontDoorSpot().X;
      var y = home == null ? 15 : home.getFrontDoorSpot().Y;

      BetterWand.Warp(LocationName, x, y);
    }
  }
}