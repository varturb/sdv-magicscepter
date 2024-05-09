using MagicScepter.Mods;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class VisitMountVapius : WarpLocationBase
  {
    public override int Order => 60;
    internal override string LocationName => "Lumisteria.MtVapius_Hamlet";
    public override string DialogLabel => "dialog.location.visitMountVapius";
    internal override string ObeliskName => "Lumisteria.MtVapius_Obelisk";
    public override bool CanWarp => CanWarpHere();
    public override Rectangle SpirteSource => new(832, 0, 64, 64);

    public override void Warp()
    {
      var obelisk = LocationHelper.FindBuilding(ObeliskName);
      obelisk?.doAction(new Vector2(obelisk.tileX.Value, obelisk.tileY.Value), Game1.player);
    }

    private bool CanWarpHere()
    {
      return ModManager.IsModLoaded(SupportedMod.VisitMountVapius)
        && LocationHelper.FindBuilding(ObeliskName) != null;
    }
  }
}