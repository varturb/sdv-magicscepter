using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class Farm : WarpLocationBase
  {
    public override int Order => 1;
    internal override string LocationName => "Farm";
    public override string DialogLabel => "dialog.location.farm"; 
    internal override string ObeliskName => "Water Obelisk";
    public override bool CanWarp => true;

    public override void Warp()
    {
      var home = Utility.getHomeOfFarmer(Game1.player);
      var x = home == null ? 64 : home.getFrontDoorSpot().X;
      var y = home == null ? 15 : home.getFrontDoorSpot().Y;

      BetterWand.Warp(LocationName, x, y);
    }
  }
}