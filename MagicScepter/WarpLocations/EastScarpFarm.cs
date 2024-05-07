using MagicScepter.Mods;

namespace MagicScepter.WarpLocations
{
  public class EastScarpFarm : WarpLocationBase
  {
    public override int Order => 41;
    internal override string LocationName => "EastScarp_MeadowFarm";
    public override string DialogLabel => "dialog.location.eastScarpFarm";
    internal override string ObeliskName => "Scarp Obelisk";
    public override bool CanWarp => CanWarpHere();

    public override void Warp()
    {
      BetterWand.Warp(LocationName, 44, 10);
    }

    private bool CanWarpHere()
    {
      return ModManager.IsModLoaded(SupportedMod.EastScarp)
        && LocationHelper.FindBuilding(ObeliskName) != null;
    }
  }
}