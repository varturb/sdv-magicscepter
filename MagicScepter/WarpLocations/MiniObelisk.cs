using System.Diagnostics.Contracts;
using MagicScepter.Mods;
using MagicScepter.Mods.MultipleMiniObelisks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public class MiniObelisk : WarpLocationBase
  {
    public override int Order => _orderNumber;
    public override string DialogLabel => _dialogLabel;
    internal override string LocationName => "Farm";
    internal override string ObeliskName => "Mini-Obelisk";
    public override bool CanWarp => CanWarpHere();
    public int DisplayNumber;

    private string _dialogLabel = "dialog.location.miniObelisk";
    private readonly Coords Coords;
    private readonly int _orderNumber = 100;

    public MiniObelisk() { }

    public MiniObelisk(int index, Coords coords)
    {
      _orderNumber += index;
      _dialogLabel = "dialog.location.miniObeliskNumber";
      DisplayNumber = index + 1;
      DialogKey = $"{DialogKey}_{DisplayNumber}";
      Coords = coords;
    }

    public override void Warp()
    {
      if (ModManager.IsModLoaded(SupportedMod.MultipleMiniObelisks))
      {
        MultipleMiniObelisks.OpenWarpMenu();
        return;
      }

      var obeliskCoords = GetValidTile(Coords.X, Coords.Y);
      if (obeliskCoords == null)
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MiniObelisk_NeedsSpace"));
        return;
      }

      BetterWand.Warp(LocationName, obeliskCoords.X, obeliskCoords.Y);
    }

    private bool CanWarpHere()
    {
      if (ModManager.IsModLoaded(SupportedMod.MultipleMiniObelisks))
      {
        return MultipleMiniObelisks.CanWarp();
      }

      return LocationHelper.FindObjects(ObeliskName).Count > 0;
    }

    private static Coords GetValidTile(int x, int y)
    {
      var targetLocation = new Coords(x, y + 1);

      if (IsTileValid(targetLocation))
      {
        return targetLocation;
      }
      targetLocation = new Coords(x - 1, y);
      if (IsTileValid(targetLocation))
      {
        return targetLocation;
      }
      targetLocation = new Coords(x + 1, y);
      if (IsTileValid(targetLocation))
      {
        return targetLocation;
      }
      targetLocation = new Coords(x, y - 1);
      if (IsTileValid(targetLocation))
      {
        return targetLocation;
      }

      return null;
    }

    private static bool IsTileValid(Coords coords)
    {
      return Game1.getFarm().CanItemBePlacedHere(new Vector2(coords.X, coords.Y));
    }
  }
}