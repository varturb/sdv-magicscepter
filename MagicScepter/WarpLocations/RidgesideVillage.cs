using MagicScepter.Mods;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class RidgesideVillage : WarpLocationBase
  {
    public override int Order => 30;
    internal override string LocationName => "Custom_Ridgeside_Ridge";
    public override string DialogLabel => "dialog.location.ridgeside";
    internal override string ObeliskName => "RSV_Obelisk";
    public override bool CanWarp => CanWarpHere();
    public override Rectangle SpirteSource => new(512, 0, 64, 64);

    public override void Warp()
    {
      var obelisk = LocationHelper.FindBuilding(ObeliskName);
      obelisk?.doAction(new Vector2(obelisk.tileX.Value, obelisk.tileY.Value), Game1.player);
    }

    private bool CanWarpHere()
    {
      return ModManager.IsModLoaded(SupportedMod.RidgesideVillage)
        && LocationHelper.FindBuilding(ObeliskName) != null;
    }
  }
}