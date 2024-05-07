using MagicScepter.Mods;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class RidgesideVillageFarm : WarpLocationBase
  {
    public override int Order => 31;
    internal override string LocationName => "Custom_Ridgeside_SummitFarm";
    public override string DialogLabel => "dialog.location.ridgesideFarm";
    internal override string ObeliskName => "RSV_Obelisk";
    public override bool CanWarp => CanWarpHere();

    private const string QuestName = "RSV.UntimedSpecialOrder.SpiritRealmFlames";

    public override void Warp()
    {
      BetterWand.Warp(LocationName, 18, 47);
    }

    private bool CanWarpHere()
    {
      return ModManager.IsModLoaded(SupportedMod.RidgesideVillage)
        && LocationHelper.FindBuilding(ObeliskName) != null
        && Game1.MasterPlayer.team.completedSpecialOrders.Contains(QuestName);
    }
  }
}
