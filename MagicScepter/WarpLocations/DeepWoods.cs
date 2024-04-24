using MagicScepter.Mods;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class DeepWoods : WarpLocationBase
  {
    public override int Order => 7;
    internal override string LocationName => "DeepWoods";
    public override string DialogLabel => "dialog.location.deepWoods";
    internal override string ObeliskName => "Woods Obelisk";
    public override bool CanWarp => CanWarpHere();

    public override void Warp()
    {
      var obelisk = LocationHelper.FindBuilding(ObeliskName);
      obelisk?.doAction(new Vector2(obelisk.tileX.Value, obelisk.tileY.Value), Game1.player);
    }

    private bool CanWarpHere()
    {
      return ModManager.IsModLoaded(SupportedMod.DeepWoods)
        && LocationHelper.FindBuilding(ObeliskName) != null;
    }
  }
}