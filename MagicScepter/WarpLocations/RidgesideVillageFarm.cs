using MagicScepter.Mods;
using MagicScepter.Tools;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class RidgesideVillageFarm : WarpLocationBase
  {
    public override int Order => 31;
    internal override string LocationName => "Custom_Ridgeside_SummitFarm";
    public override string DialogLabel => GetDialogLabel();
    internal override string ObeliskName => "RSV_Obelisk";
    public override bool CanWarp => CanWarpHere();
    public override Rectangle SpirteSource => new(576, 0, 64, 64);

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

    private static string GetDialogLabel()
    {
      return ModUtility.Config.UseOldDialogMenu 
        ? "dialog.location.ridgesideFarm" 
        : "menu.location.ridgesideFarm";
    }
  }
}
