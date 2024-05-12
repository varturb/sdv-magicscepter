using MagicScepter.Mods;
using MagicScepter.Tools;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class EnchantedGrave : WarpLocationBase
  {
    public override int Order => 10;
    internal override string LocationName => "Custom_EnchantedGrove";
    public override string DialogLabel => "dialog.location.enchantedGrave";
    internal override string ObeliskName => "";
    public override bool CanWarp => CanWarpHere();
    public override Rectangle SpirteSource => new(896, 0, 64, 64);
    private const string EventId = "908073";

    public override void Warp()
    {
      BetterWand.Warp(LocationName, 30, 27);
    }

    private bool CanWarpHere()
    {
      return ModManager.IsModLoaded(SupportedMod.StardewValleyExpanded)        
        && Game1.MasterPlayer.eventsSeen.Contains(EventId);
    }
  }
}
