using MagicScepter.Mods;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class EastScarp : WarpLocationBase
  {
    public override int Order => 40;
    internal override string LocationName => "EastScarp_Village";
    public override string DialogLabel => "dialog.location.eastScarp";
    internal override string ObeliskName => "Scarp Obelisk";
    public override bool CanWarp => CanWarpHere();

    public override void Warp()
    {
      var obelisk = LocationHelper.FindBuilding(ObeliskName);
      obelisk?.doAction(new Vector2(obelisk.tileX.Value, obelisk.tileY.Value), Game1.player);
    }

    private bool CanWarpHere()
    {
      return ModManager.IsModLoaded(SupportedMod.EastScarp)
        && LocationHelper.FindBuilding(ObeliskName) != null;
    }
  }
}