using MagicScepter.Mods;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class DowntownZuzu : WarpLocationBase
  {
    public override int Order => 50;
    internal override string LocationName => "Custom_DTZ_ZuzuCity1";
    public override string DialogLabel => "dialog.location.downtownZuzu";
    internal override string ObeliskName => "DTZ.DowntownZuzuCP_Obelisk";
    public override bool CanWarp => CanWarpHere();
    public override Rectangle SpirteSource => new(768, 0, 64, 64);

    public override void Warp()
    {
      var obelisk = LocationHelper.FindBuilding(ObeliskName);
      obelisk?.doAction(new Vector2(obelisk.tileX.Value, obelisk.tileY.Value), Game1.player);
    }

    private bool CanWarpHere()
    {
      return ModManager.IsModLoaded(SupportedMod.DowntownZuzu)
        && LocationHelper.FindBuilding(ObeliskName) != null;
    }
  }
}