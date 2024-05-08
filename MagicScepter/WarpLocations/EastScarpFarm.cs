using MagicScepter.Mods;
using MagicScepter.Tools;
using Microsoft.Xna.Framework;

namespace MagicScepter.WarpLocations
{
  public class EastScarpFarm : WarpLocationBase
  {
    public override int Order => 41;
    internal override string LocationName => "EastScarp_MeadowFarm";
    public override string DialogLabel => GetDialogLabel();
    internal override string ObeliskName => "Scarp Obelisk";
    public override bool CanWarp => CanWarpHere();
    public override Rectangle SpirteSource => new(704, 0, 64, 64);

    public override void Warp()
    {
      BetterWand.Warp(LocationName, 44, 10);
    }

    private bool CanWarpHere()
    {
      return ModManager.IsModLoaded(SupportedMod.EastScarp)
        && LocationHelper.FindBuilding(ObeliskName) != null;
    }

    private static string GetDialogLabel()
    {
      return ModUtility.Config.UseOldDialogMenu 
        ? "dialog.location.eastScarpFarm" 
        : "menu.location.eastScarpFarm";
    }
  }
}